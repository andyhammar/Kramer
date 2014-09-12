namespace Kramer.Common.Settings
{
    public class AppSettings : IAppSettings
    {
        private const bool DefaultIsAutoPlaying = true;
        private const int DefaultAprMin = 0;
        private const int DefaultAprMax = 5;
        private const int DefaultApma = 3;
        private static ISettingsStore _store;

        public static void Init(ISettingsStore store)
        {
            _store = store;
        }


        public bool IsAutoPlaying
        {
            get
            {
                return GetValueOrDefault("IsAutoPlaying", DefaultIsAutoPlaying);
            }
            set
            {
                _store.Set("IsAutoPlaying", value);
            }
        }

        public int AutoPlayRangeMinInMinutes
        {
            get
            {
                return GetValueOrDefault("AprMin", DefaultAprMin);
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
                return GetValueOrDefault("AprMax", DefaultAprMax);
            }
            set
            {
                _store.Set("AprMax", value);
            }
        }

        public int AutoPlayMaxAgeInHours
        {
            get
            {
                return GetValueOrDefault("Apma", DefaultApma);
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