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
        private BackgroundTaskDeferral _deferral;

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            var player = BackgroundMediaPlayer.Current;
            player.SystemMediaTransportControls.IsEnabled = true;
            player.SystemMediaTransportControls.IsPauseEnabled = true;
            player.SystemMediaTransportControls.IsPlayEnabled = true;
            player.SystemMediaTransportControls.ButtonPressed += SystemMediaTransportControls_ButtonPressed;
            BackgroundMediaPlayer.MessageReceivedFromForeground += BackgroundMediaPlayer_MessageReceivedFromForeground;
            _deferral = taskInstance.GetDeferral();
            taskInstance.Task.Completed += Task_Completed;
            taskInstance.Canceled += TaskInstance_Canceled;
        }

        private void BackgroundMediaPlayer_MessageReceivedFromForeground(object sender, 
            MediaPlayerDataReceivedEventArgs e)
        {
            var title = e.Data["title"] as string;
            if (title == null)
                return;
            var updater = BackgroundMediaPlayer.Current.SystemMediaTransportControls.DisplayUpdater;
            updater.Type = MediaPlaybackType.Music;
            var properties = updater.MusicProperties;
            properties.Title = $"Ekot {title}";
            updater.Update();
        }

        private void TaskInstance_Canceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            _deferral.Complete();
        }

        private void Task_Completed(BackgroundTaskRegistration sender, BackgroundTaskCompletedEventArgs args)
        {
            _deferral.Complete();
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
