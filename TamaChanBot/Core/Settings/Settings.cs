
namespace TamaChanBot.Core.Settings
{
    public abstract class Settings<T> where T:Settings<T>
    {
        public abstract T LoadFromFile(string filepath);
        public abstract void SaveToFile(string filepath);
    }
}
