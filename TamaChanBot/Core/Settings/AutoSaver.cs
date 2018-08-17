﻿using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace TamaChanBot.Core.Settings
{
    public class AutoSaver
    {
        private const string BACKUP_FOLDER = @"backups\";
        private const int SAVE_FREQUENCY = 15 * 60 * 1000; //=15 minutes

        private Queue<SaveScheduleInfo> saveQueue = new Queue<SaveScheduleInfo>();

        private readonly Logger logger = new Logger("Auto-Saver");

        public async void Run(CancellationToken cancellationToken)
        {
            logger.LogInfo("Auto-saving service started.");
            while(!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(SAVE_FREQUENCY, cancellationToken);
                }
                catch (TaskCanceledException) { }

                while(saveQueue.Count > 0 && !cancellationToken.IsCancellationRequested)
                {
                    SaveScheduleInfo scheduleInfo = saveQueue.Dequeue();
                    logger.LogInfo($"Saving \"{scheduleInfo.path}\".");
                    try
                    {
                        CreateBackup(scheduleInfo.path);
                        scheduleInfo.settings.SaveToFile(scheduleInfo.path);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError($"An error occured while trying to save \"{scheduleInfo.settings}\" to \"{scheduleInfo.path}\":\r\n{ex.ToString()}");
                    }
                }
            }
            logger.LogInfo("Auto-saving service stopped.");
        }

        private void CreateBackup(string originalFilePath)
        {
            string backupFilePath = BACKUP_FOLDER + originalFilePath;
            FileInfo backupFileInfo = new FileInfo(backupFilePath);
            if (!backupFileInfo.Directory.Exists)
                backupFileInfo.Directory.Create();

            if (backupFileInfo.Exists)
                backupFileInfo.Delete();
            File.Copy(originalFilePath, backupFilePath);
        }

        public void Add(SaveScheduleInfo scheduleInfo)
        {
            if (!saveQueue.Contains(scheduleInfo))
                saveQueue.Enqueue(scheduleInfo);
        }

        public class SaveScheduleInfo
        {
            internal readonly Settings settings;
            internal readonly string path;

            public SaveScheduleInfo(Settings settings, string path)
            {
                this.settings = settings;
                this.path = path;
            }

            public override bool Equals(object obj)
            {
                if (obj is SaveScheduleInfo)
                {
                    SaveScheduleInfo other = obj as SaveScheduleInfo;
                    return other.settings.Equals(settings) && other.path.Equals(path);
                }
                else
                    return false;
            }

            public override int GetHashCode()
            {
                return settings.GetHashCode() ^ path.GetHashCode();
            }
        }
    }
}
