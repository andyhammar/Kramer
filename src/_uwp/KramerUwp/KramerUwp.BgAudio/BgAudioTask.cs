using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Media;
using Windows.Media.Playback;

namespace KramerUwp.BgAudio
{
    public sealed class BgAudioTask : IBackgroundTask
    {
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            var player = BackgroundMediaPlayer.Current;
            player.SystemMediaTransportControls.IsEnabled = true;
            player.SystemMediaTransportControls.IsPauseEnabled = true;
            player.SystemMediaTransportControls.IsPlayEnabled = true;
            player.SystemMediaTransportControls.ButtonPressed += SystemMediaTransportControls_ButtonPressed;

        }

        private void SystemMediaTransportControls_ButtonPressed(
    Windows.Media.SystemMediaTransportControls sender,
    Windows.Media.SystemMediaTransportControlsButtonPressedEventArgs args)
        {
            try
            {
                if (args.Button == SystemMediaTransportControlsButton.Pause)
                {
                    BackgroundMediaPlayer.Current.Pause();
                }
                else if (args.Button == SystemMediaTransportControlsButton.Play)
                {
                    BackgroundMediaPlayer.Current.Play();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

    }
}
