using System.IO.IsolatedStorage;
using Kramer.Common.Settings;

namespace Kramer.Phone
{
    public class SettingsStorePhoneImpl : ISettingsStore
    {
        private static IsolatedStorageSettings _store;

        static SettingsStorePhoneImpl()
        {
            _store = IsolatedStorageSettings.ApplicationSettings;
        }
        public bool HasKey(string key)
        {
            return _store.Contains(key);
        }

        public object Get(string key)
        {
            return _store[key];
        }

        public void Set(string key, object value)
        {
            _store[key] = value;
        }
    }
}