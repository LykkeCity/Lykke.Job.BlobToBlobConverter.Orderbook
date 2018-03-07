namespace Lykke.Job.BlobToBlobConverter.Orderbook.Core.Domain.OutputModels
{
    public class OutOrderbook
    {
        public string AssetPairId { get; set; }

        public bool IsBuy { get; set; }

        public string Timestamp { get; set; }

        public decimal BestPrice { get; set; }

        public override string ToString()
        {
            return $"{nameof(AssetPairId)},{AssetPairId},{nameof(IsBuy)},{IsBuy},{nameof(Timestamp)},{Timestamp},{nameof(BestPrice)},{BestPrice}";
        }
    }
}
