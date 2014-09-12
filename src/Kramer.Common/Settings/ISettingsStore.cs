namespace Kramer.Common.Settings
{
    public interface ISettingsStore
    {
        bool HasKey(string key);
        object Get(string key);
        void Set(string key, object value);
    }
}
