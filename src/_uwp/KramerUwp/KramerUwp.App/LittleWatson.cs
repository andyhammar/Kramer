using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
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

        internal static async Task ReportException(Exception ex, string extra)
        {
            try
            {
                var file = await _folder.CreateFileAsync(FILENAME, CreationCollisionOption.OpenIfExists);

                var text = extra + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace;
                await FileIO.WriteTextAsync(file, text);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        internal static async Task CheckForPreviousException()
        {
            try
            {

                var file = await _folder.GetFileAsync(FILENAME);

                if (file == null)
                    return;

                var error = await FileIO.ReadTextAsync(file);

                var messageDialog = new MessageDialog(
                    "A problem occurred the last time you ran this application. Would you like to send an email to report it?",
                    "Problem Report");
                messageDialog.Commands.Add(
                    new UICommand("Yes", async (x) => await SendEmail(error)));
                messageDialog.Commands.Add(
                    new UICommand("No"));
                await messageDialog.ShowAsync();

                await file.DeleteAsync();


                //if (contents != null)
                //{
                //    //if (MessageBox.Show(
                //    //    "A problem occurred the last time you ran this application. Would you like to send an email to report it?",
                //    //    "Problem Report",
                //    //    MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                //    if (MessageBox.Show(
                //        "Ett fel uppstod vid senaste körningen, vill du skicka en felrapport till utvecklaren?",
                //        "Skicka felrapport",
                //        MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                //    {
                //        var email = new EmailComposeTask();
                //        email.To = EMAIL_TO;
                //        email.Subject = EMAIL_SUBJECT;
                //        email.Body = contents;
                //        SafeDeleteFile(IsolatedStorageFile.GetUserStoreForApplication()); // line added 1/15/2011
                //        email.Show();
                //    }
                //}
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
            finally
            {
                //SafeDeleteFile(IsolatedStorageFile.GetUserStoreForApplication());
            }
        }

        private static async Task SendEmail(string error)
        {
            var email = new EmailMessage { Body = error };
            email.To.Add(new EmailRecipient(EMAIL_TO));
            email.Subject = EMAIL_SUBJECT;
            await EmailManager.ShowComposeNewEmailAsync(email);
        }

        //private static void SafeDeleteFile(IsolatedStorageFile store)
        //{
        //    try
        //    {
        //        store.DeleteFile(FILENAME);
        //    }
        //    catch (Exception)
        //    {
        //    }
        //}
    }
}

