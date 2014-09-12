namespace Kramer.Common.Settings
{
    public class AppSettings : IAppSettings
    {
        private static ISettingsStore _store;

        public static void Init(ISettingsStore store)
        {
            _store = store;
        }


        public int AutoPlayRangeMinInMinutes
        {
            get
            {
                return GetValueOrDefault("AprMin", 0);
            }
            set
            {
                _store.Set("AprMin", value);
            }
        }

        public int AutoPlayRangeMaxInMinutes
        {
            get
            {
                return GetValueOrDefault("AprMax", 5);
            }
            set
            {
                _store.Set("AprMax", value);
            }
        }

        public bool IsAutoPlaying
        {
            get
            {
                return GetValueOrDefault("IsAutoPlaying", true);
            }
            set
            {
                _store.Set("IsAutoPlaying", value);
            }
        }

        public int AutoPlayMaxAgeInHours
        {
            get
            {
                return GetValueOrDefault("Apma", 3);
            }
            set
            {
                _store.Set("Apma", value);
            }
        }

        private static T GetValueOrDefault<T>(string key, T defaultValue)
        {
            if (_store.HasKey(key))
            {
                return (T) _store.Get(key);
            }
            return defaultValue;
        }
    }
}