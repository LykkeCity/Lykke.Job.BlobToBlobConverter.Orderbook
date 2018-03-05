namespace Lykke.Job.BlobToBlobConverter.Orderbook.Core.Domain.OutputModels
{
    public class OutOrderbook
    {
        public string AssetPair { get; set; }

        public bool IsBuy { get; set; }

        public string Timestamp { get; set; }

        public decimal BestPrice { get; set; }
    }
}
