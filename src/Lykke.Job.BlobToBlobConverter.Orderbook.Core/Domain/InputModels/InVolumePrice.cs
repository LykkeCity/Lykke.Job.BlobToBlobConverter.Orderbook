namespace Lykke.Job.BlobToBlobConverter.Orderbook.Core.Domain.InputModels
{
    public class InVolumePrice
    {
        public double Volume { get; set; }

        public double Price { get; set; }

        public bool IsValid()
        {
            return Volume != 0 && Price >= 0;
        }
    }
}
