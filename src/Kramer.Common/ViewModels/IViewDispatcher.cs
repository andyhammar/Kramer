using System;
using System.Threading.Tasks;

namespace Kramer.Common.ViewModels
{
    public interface IViewDispatcher
    {
        Task RunAsync(Action action);
    }
}