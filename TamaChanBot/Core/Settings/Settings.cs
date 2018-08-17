using Newtonsoft.Json;

namespace TamaChanBot.Core.Settings
{
    public abstract class Settings
    {
        [JsonIgnore] public string DefaultPath { get; protected set; }

        public abstract Settings LoadFromFile(string filepath);
        public abstract void SaveToFile(string filepath);

        public virtual Settings LoadFromFile() => LoadFromFile(DefaultPath);
        public virtual void SaveToFile() => SaveToFile(DefaultPath);

        public virtual void MarkDirty()
        {
            TamaChan.Instance.AutoSaver.Add(new AutoSaver.SaveScheduleInfo(this, DefaultPath));
        }
    }
}
