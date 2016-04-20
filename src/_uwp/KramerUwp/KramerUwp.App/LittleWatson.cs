using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.ApplicationModel.Email;
using Windows.Storage;
using Windows.UI.Popups;

namespace KramerUwp.App
{
    public class LittleWatson
    {
        private static string _filePath;

        private const string EMAIL_TO = "ahamapps@live.com";
        private const string EMAIL_SUBJECT = "Echo news (uwp) - error report";
        const string FILENAME = "LittleWatson.txt";
        private static readonly StorageFolder _folder;

        static LittleWatson()
        {
            _folder = ApplicationData.Current.LocalFolder;
        }

        internal static void ReportException(Exception ex, string extra)
        {
            try
            {
                ApplicationData.Current.LocalSettings.Values["error"] = extra + Environment.NewLine + ex;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        internal static async Task CheckForPreviousExceptionAsync()
        {
            try
            {
                var error = ApplicationData.Current.LocalSettings.Values["error"] as string;
                if (error == null)
                    return;

                var messageDialog = new MessageDialog(
                    "A problem occurred the last time you ran this application. Would you like to send an email to report it?",
                    "Problem Report");
                messageDialog.Commands.Add(
                    new UICommand("Yes", async (x) => await SendEmail(error)));
                messageDialog.Commands.Add(
                    new UICommand("No"));
                await messageDialog.ShowAsync();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
            finally
            {
                ApplicationData.Current.LocalSettings.Values["error"] = null;
            }
        }

        private static async Task SendEmail(string error)
        {
            var email = new EmailMessage { Body = error };
            email.To.Add(new EmailRecipient(EMAIL_TO));
            email.Subject = EMAIL_SUBJECT;
            await EmailManager.ShowComposeNewEmailAsync(email);
        }
    }
}

