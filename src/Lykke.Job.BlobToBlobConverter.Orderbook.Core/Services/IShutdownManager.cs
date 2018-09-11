using System.Threading.Tasks;

namespace Lykke.Job.BlobToBlobConverter.Orderbook.Core.Services
{
    public interface IShutdownManager
    {
        Task StopAsync();
    }
}
