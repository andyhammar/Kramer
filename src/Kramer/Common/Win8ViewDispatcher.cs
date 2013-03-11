using System;
using System.Threading.Tasks;
using Kramer.Common.ViewModels;
using Windows.UI.Core;

namespace Kramer.Common
{
    public class Win8ViewDispatcher : IViewDispatcher
    {
        public async Task RunAsync(Action action)
        {
            await CoreWindow.GetForCurrentThread().Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal, action.Invoke);
        }
    }
}