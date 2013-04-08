using System;
using Windows.UI.ApplicationSettings;

namespace Kramer.Common
{
    public static class SettingsHelper
    {
        public static void AddSettingsCommands(SettingsPaneCommandsRequestedEventArgs args)
        {
            args.Request.ApplicationCommands.Clear();

            var privacyPref = new SettingsCommand("privacyPref", "Sekretess", (uiCommand) =>
                {
                    Windows.System.Launcher.LaunchUriAsync(new Uri("http://ahamprivacypolicy.azurewebsites.net/"));
                });

            args.Request.ApplicationCommands.Add(privacyPref);
        }

    }
}