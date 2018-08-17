
namespace TamaChanBot.Core.Settings
{
    public abstract class Settings
    {
        public abstract Settings LoadFromFile(string filepath);
        public abstract void SaveToFile(string filepath);
    }
}
