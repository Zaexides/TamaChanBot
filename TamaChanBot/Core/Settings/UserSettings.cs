using System;
using System.IO;
using System.Security.Cryptography;
using System.Collections.Generic;
using Microsoft.Win32;
using Newtonsoft.Json;
using TamaChanBot.API;

namespace TamaChanBot.Core.Settings
{
    public abstract class UserSettings : Settings
    {
        private const string REGISTRY_SOFTWARE = "Software";
        private const string REGISTRY_APP_NAME = "TamaChan";
        private const string REGISTRY_ENC_KEY_NAME = "EK";
        private const string REGISTRY_ENC_IV_NAME = "EIV";

        private readonly JsonSerializerSettings serializerSettings;

        protected abstract byte[] EncryptionEntropy { get; }
        internal abstract string RegistrySub { get; }

        protected UserSettings() : this(false, 0) { }

        public UserSettings(bool isGuild, ulong id)
        {
            DefaultPath = $@"Settings\{(isGuild ? "Guild" : "User")}\{id}";
            serializerSettings = new JsonSerializerSettings();
            serializerSettings.TypeNameHandling = TypeNameHandling.Auto;
        }
        
        public override sealed Settings LoadFromFile(string filepath)
        {
            UserSettings userSettings = null;
            try
            {
                if(File.Exists(filepath))
                {
                    byte[] encryptedFileContents = File.ReadAllBytes(filepath);
                    string json = DecryptJson(encryptedFileContents);
                    if (json != string.Empty)
                        userSettings = JsonConvert.DeserializeObject(json, GetType(), serializerSettings) as UserSettings;
                }
            }
            catch (Exception ex)
            {
                TamaChan.Instance.Logger.LogError($"Failed to deserialize \"{GetType().FullName}\" class: " + ex.ToString());
            }
            return userSettings;
        }

        public override sealed void SaveToFile(string filepath)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(filepath);
                if (!fileInfo.Directory.Exists)
                    fileInfo.Directory.Create();

                string json = JsonConvert.SerializeObject(this, Formatting.None, serializerSettings);
                byte[] encryptedContents = EncryptJson(json);
                File.WriteAllBytes(filepath, encryptedContents);
            }
            catch (Exception ex)
            {
                TamaChan.Instance.Logger.LogError($"Failed to serialize \"{GetType().FullName}\" class: " + ex.ToString());
            }
        }

        private byte[] EncryptJson(string json)
        {
            byte[] encryptedData;
            using (AesManaged aes = new AesManaged())
            {
                aes.GenerateKey();
                aes.GenerateIV();
                SaveEncryptionInfoToRegistry(aes.Key, aes.IV);
                aes.Padding = PaddingMode.PKCS7;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter writer = new StreamWriter(cryptoStream))
                            writer.Write(json);
                        encryptedData = memoryStream.ToArray();
                    }
                }
            }

            return encryptedData;
        }

        private string DecryptJson(byte[] encryptedData)
        {
            string json;
            if(LoadEncryptionInfoFromRegistry(out byte[] key, out byte[] iV))
            {
                using (AesManaged aes = new AesManaged())
                {
                    aes.Key = key;
                    aes.IV = iV;
                    aes.Padding = PaddingMode.PKCS7;

                    ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                    using (MemoryStream memoryStream = new MemoryStream(encryptedData))
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader reader = new StreamReader(cryptoStream))
                                json = reader.ReadToEnd();
                        }
                    }
                }

                return json;
            }
            else
                return string.Empty;
        }

        private void SaveEncryptionInfoToRegistry(byte[] key, byte[] iV)
        {
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(REGISTRY_SOFTWARE, true);
            registryKey = registryKey.CreateSubKey(REGISTRY_APP_NAME, true);
            registryKey = registryKey.CreateSubKey(RegistrySub, true);
            registryKey.SetValue(REGISTRY_ENC_KEY_NAME, ProtectedData.Protect(key, null, DataProtectionScope.CurrentUser));
            registryKey.SetValue(REGISTRY_ENC_IV_NAME, ProtectedData.Protect(iV, null, DataProtectionScope.CurrentUser));
        }

        private bool LoadEncryptionInfoFromRegistry(out byte[] key, out byte[] iV)
        {
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(REGISTRY_SOFTWARE, false);
            registryKey = registryKey.OpenSubKey(REGISTRY_APP_NAME, false);
            registryKey = registryKey.OpenSubKey(RegistrySub, false);
            if (registryKey == null)
            {
                key = null;
                iV = null;
                return false;
            }
            key = ProtectedData.Unprotect((byte[])registryKey.GetValue(REGISTRY_ENC_KEY_NAME), null, DataProtectionScope.CurrentUser);
            iV = ProtectedData.Unprotect((byte[])registryKey.GetValue(REGISTRY_ENC_IV_NAME), null, DataProtectionScope.CurrentUser);

            return true;
        }
    }
}
