using System;
using System.Collections.Generic;

namespace Lykke.Job.BlobToBlobConverter.Orderbook.Core.Domain.InputModels
{
    public class InOrderBook
    {
        private static int _maxStringFieldsLength = 255;

        public string AssetPair { get; set; }

        public bool IsBuy { get; set; }

        public DateTime Timestamp { get; set; }

        public List<InVolumePrice> Prices { get; set; }

        public bool IsValid()
        {
            if (AssetPair == null || AssetPair.Length > _maxStringFieldsLength)
                return false;

            if (Prices != null)
                foreach (var price in Prices)
                {
                    if (!price.IsValid())
                        return false;
                }

            return true;
        }
    }
}
