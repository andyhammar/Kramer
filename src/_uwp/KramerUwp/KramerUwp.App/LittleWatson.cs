using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Popups;

namespace KramerUwp.App
{
    public class LittleWatson
    {
        private static string _filePath;

        private const string EMAIL_TO = "ahamapps@live.com";
        private const string EMAIL_SUBJECT = "Echo news (uwp) - error report";
        const string Filename = "LittleWatson.txt";


        static LittleWatson()
        {
            _filePath = Path.Combine(Path.GetTempPath(), Filename);
        }

        internal static async Task ReportException(Exception ex, string extra)
        {
            try
            {
                var file = await StorageFile.GetFileFromPathAsync(_filePath);
                
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
                var file = await StorageFile.GetFileFromPathAsync(_filePath);

                if (file == null)
                    return;

                var error = await FileIO.ReadTextAsync(file);

                var messageDialog = new MessageDialog("you had error");
                await messageDialog.ShowAsync();

                await file.DeleteAsync();
                //string contents = null;
                //using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                //{
                //    if (store.FileExists(Filename))
                //    {
                //        using (TextReader reader = new StreamReader(store.OpenFile(Filename, FileMode.Open, FileAccess.Read, FileShare.None)))
                //        {
                //            contents = reader.ReadToEnd();
                //        }
                //        //SafeDeleteFile(store);
                //    }
                //}
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

        //private static void SafeDeleteFile(IsolatedStorageFile store)
        //{
        //    try
        //    {
        //        store.DeleteFile(Filename);
        //    }
        //    catch (Exception)
        //    {
        //    }
        //}
    }
}

