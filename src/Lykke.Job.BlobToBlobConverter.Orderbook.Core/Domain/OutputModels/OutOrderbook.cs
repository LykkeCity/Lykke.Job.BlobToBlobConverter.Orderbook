namespace Lykke.Job.BlobToBlobConverter.Orderbook.Core.Domain.OutputModels
{
    public class OutOrderbook
    {
        public string AssetPairId { get; set; }

        public bool IsBuy { get; set; }

        public string Timestamp { get; set; }

        public decimal BestPrice { get; set; }

        public string GetValuesString()
        {
            return $"{AssetPairId},{IsBuy},{Timestamp},{BestPrice}";
        }

        public static string GetColumnsString()
        {
            return $"{nameof(AssetPairId)},{nameof(IsBuy)},{nameof(Timestamp)},{nameof(BestPrice)}";
        }
    }
}
