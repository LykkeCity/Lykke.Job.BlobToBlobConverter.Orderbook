using System.Linq;
using System.Collections.Generic;
using JetBrains.Annotations;
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
    [UsedImplicitly]
    public class MessageProcessor : IMessageProcessor
    {
        private const int _maxBatchCount = 1000000;

        private readonly ILog _log;

        private List<string> _list;
        private Func<string, List<string>, Task> _messagesHandler;

        public MessageProcessor(ILog log)
        {
            _log = log;
        }

        public void StartBlobProcessing(Func<string, List<string>, Task> messagesHandler)
        {
            _list = new List<string>();
            _messagesHandler = messagesHandler;
        }

        public async Task FinishBlobProcessingAsync()
        {
            if (_list.Count > 0)
                await _messagesHandler(StructureBuilder.MainContainer, _list);
        }

        public async Task<bool> TryProcessMessageAsync(byte[] data)
        {
            bool result = JsonDeserializer.TryDeserialize(data, out InOrderBook orderbook);
            if (!result)
                return false;

            AddConvertedMessage(orderbook, _list);

            if (_list.Count >= _maxBatchCount)
            {
                await _messagesHandler(StructureBuilder.MainContainer, _list);
                _list.Clear();
            }

            return true;
        }

        private void AddConvertedMessage(InOrderBook book, List<string> list)
        {
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
