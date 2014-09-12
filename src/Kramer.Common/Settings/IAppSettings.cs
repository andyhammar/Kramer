namespace Kramer.Common.Settings
{
    public interface IAppSettings
    {
        int AutoPlayRangeMinInMinutes { get; set; }
        int AutoPlayRangeMaxInMinutes { get; set; }
        bool IsAutoPlaying { get; set; }
        int AutoPlayMaxAgeInHours { get; set; }
    }
}