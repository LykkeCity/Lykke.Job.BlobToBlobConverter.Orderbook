using Autofac;
using Common;

namespace Lykke.Job.BlobToBlobConverter.Orderbook.Core.Services
{
    public interface IStartStop : IStartable, IStopable
    {
    }
}
