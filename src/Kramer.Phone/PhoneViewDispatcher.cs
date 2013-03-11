using System;
using System.Threading.Tasks;
using System.Windows;
using Kramer.Common.ViewModels;

namespace Kramer.Phone
{
    public class PhoneViewDispatcher : IViewDispatcher
    {
        public Task RunAsync(Action action)
        {
            return Task.Factory.StartNew(() => Deployment.Current.Dispatcher.BeginInvoke(action));
        }
    }
}