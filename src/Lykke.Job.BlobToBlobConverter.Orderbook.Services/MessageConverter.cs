using System.Linq;
using System.Collections.Generic;
using Common;
using Common.Log;
using Lykke.Job.BlobToBlobConverter.Common.Abstractions;
using Lykke.Job.BlobToBlobConverter.Common.Helpers;
using Lykke.Job.BlobToBlobConverter.Orderbook.Core.Domain.InputModels;
using Lykke.Job.BlobToBlobConverter.Orderbook.Core.Domain.OutputModels;

namespace Lykke.Job.BlobToBlobConverter.Orderbook.Services
{
    public class MessageConverter : IMessageConverter
    {
        private const string _mainContainer = "orderbook";

        private readonly ILog _log;

        public MessageConverter(ILog log)
        {
            _log = log;
        }

        public Dictionary<string, List<string>> Convert(IEnumerable<byte[]> messages)
        {
            var result = new Dictionary<string, List<string>>
            {
                { _mainContainer, new List<string>() },
            };

            foreach (var message in messages)
            {
                AddConvertedMessage(message, result);
            }

            return result;
        }

        public Dictionary<string, string> GetMappingStructure()
        {
            var result = new Dictionary<string, string>
            {
                { _mainContainer, OutOrderbook.GetColumns() },
            };
            return result;
        }

        private void AddConvertedMessage(byte[] message, Dictionary<string, List<string>> result)
        {
            var book = JsonDeserializer.Deserialize<InOrderBook>(message);
            if (!book.IsValid())
                _log.WriteWarning(nameof(MessageConverter), nameof(Convert), $"Orderbook {book.ToJson()} is invalid!");

            decimal bestPrice = 0;
            if (book.Prices != null && book.Prices.Count > 0)
                bestPrice = book.IsBuy
                    ? (decimal)book.Prices.Max(p => p.Price)
                    : (decimal)book.Prices.Min(p => p.Price);

            var orderbook = new OutOrderbook
            {
                AssetPairId = book.AssetPair,
                IsBuy = book.IsBuy,
                Timestamp = DateTimeConverter.Convert(book.Timestamp),
                BestPrice = bestPrice,
            };
            result[_mainContainer].Add(orderbook.ToString());
        }
    }
}
