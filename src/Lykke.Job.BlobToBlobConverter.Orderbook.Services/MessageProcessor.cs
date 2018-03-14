using System.Linq;
using System.Collections.Generic;
using Common;
using Common.Log;
using Lykke.Job.BlobToBlobConverter.Common.Abstractions;
using Lykke.Job.BlobToBlobConverter.Common.Helpers;
using Lykke.Job.BlobToBlobConverter.Orderbook.Core.Domain.InputModels;
using Lykke.Job.BlobToBlobConverter.Orderbook.Core.Domain.OutputModels;
using System;
using System.Threading.Tasks;

namespace Lykke.Job.BlobToBlobConverter.Orderbook.Services
{
    public class MessageProcessor : IMessageProcessor
    {
        private const string _mainContainer = "orderbook";
        private const int _maxBatchCount = 1000000;

        private readonly ILog _log;

        public MessageProcessor(ILog log)
        {
            _log = log;
        }

        public async Task ProcessAsync(IEnumerable<byte[]> messages, Func<string, ICollection<string>, Task> processTask)
        {
            var list = new List<string>();

            foreach (var message in messages)
            {
                AddConvertedMessage(message, list);

                if (list.Count >= _maxBatchCount)
                {
                    await processTask(_mainContainer, list);
                    list.Clear();
                }
            }

            if (list.Count >= 0)
                await processTask(_mainContainer, list);
        }

        public Dictionary<string, string> GetMappingStructure()
        {
            var result = new Dictionary<string, string>
            {
                { _mainContainer, OutOrderbook.GetColumnsString() },
            };
            return result;
        }

        private void AddConvertedMessage(byte[] message, List<string> list)
        {
            var book = JsonDeserializer.Deserialize<InOrderBook>(message);
            if (!book.IsValid())
                _log.WriteWarning(nameof(MessageProcessor), nameof(Convert), $"Orderbook {book.ToJson()} is invalid!");

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
            list.Add(orderbook.GetValuesString());
        }
    }
}
