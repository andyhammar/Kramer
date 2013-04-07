using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows;
using Microsoft.Phone.Tasks;

namespace Kramer.Phone
{
    public class LittleWatson
    {
        private const string EMAIL_TO = "ahamapps@live.com";
        private const string EMAIL_SUBJECT = "Ekot - error report";
        const string Filename = "LittleWatson.txt";

        internal static void ReportException(Exception ex, string extra)
        {
            try
            {
                using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    SafeDeleteFile(store);
                    using (TextWriter output = new StreamWriter(store.CreateFile(Filename)))
                    {
                        output.WriteLine(extra);
                        output.WriteLine(ex.Message);
                        output.WriteLine(ex.StackTrace);
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        internal static void CheckForPreviousException()
        {
            try
            {
                string contents = null;
                using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (store.FileExists(Filename))
                    {
                        using (TextReader reader = new StreamReader(store.OpenFile(Filename, FileMode.Open, FileAccess.Read, FileShare.None)))
                        {
                            contents = reader.ReadToEnd();
                        }
                        SafeDeleteFile(store);
                    }
                }
                if (contents != null)
                {
                    //if (MessageBox.Show(
                    //    "A problem occurred the last time you ran this application. Would you like to send an email to report it?",
                    //    "Problem Report",
                    //    MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    if (MessageBox.Show(
                        "Ett fel uppstod vid senaste körningen, vill du skicka en felrapport till utvecklaren?", 
                        "Skicka felrapport", 
                        MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        var email = new EmailComposeTask();
                        email.To = EMAIL_TO;
                        email.Subject = EMAIL_SUBJECT;
                        email.Body = contents;
                        SafeDeleteFile(IsolatedStorageFile.GetUserStoreForApplication()); // line added 1/15/2011
                        email.Show();
                    }
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                SafeDeleteFile(IsolatedStorageFile.GetUserStoreForApplication());
            }
        }

        private static void SafeDeleteFile(IsolatedStorageFile store)
        {
            try
            {
                store.DeleteFile(Filename);
            }
            catch (Exception)
            {
            }
        }
    }
}
