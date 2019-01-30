using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Common;
using Common.Log;
using Lykke.Job.BlobToBlobConverter.Common.Abstractions;
using Lykke.Job.BlobToBlobConverter.Common.Helpers;
using Lykke.Job.BlobToBlobConverter.Orderbook.Core.Domain.InputModels;
using Lykke.Job.BlobToBlobConverter.Orderbook.Core.Domain.OutputModels;

namespace Lykke.Job.BlobToBlobConverter.Orderbook.Services
{
    [UsedImplicitly]
    public class MessageProcessor : IMessageProcessor, IMessageTypeResolver
    {
        private const int _maxBatchCount = 500000;

        private readonly ILog _log;
        private readonly Dictionary<string, Dictionary<int, string>> _minutesDict = new Dictionary<string, Dictionary<int, string>>();

        private Func<string, List<string>, Task> _messagesHandler;

        public MessageProcessor(ILog log)
        {
            _log = log;
        }

        public void StartBlobProcessing(Func<string, List<string>, Task> messagesHandler)
        {
            _messagesHandler = messagesHandler;
        }

        public async Task FinishBlobProcessingAsync()
        {
            if (_minutesDict.Count > 0)
            {
                await _messagesHandler(StructureBuilder.MainContainer, _minutesDict.SelectMany(i => i.Value.Values).ToList());
                _minutesDict.Clear();
            }
        }

        public async Task ProcessMessageAsync(object obj)
        {
            var book = obj as InOrderBook;

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
            if (!_minutesDict.ContainsKey(book.AssetPair))
                _minutesDict.Add(book.AssetPair, new Dictionary<int, string>());
            var assetPairDict = _minutesDict[book.AssetPair];
            int minuteKey = GetMinuteKey(book.Timestamp);

            var allCount = _minutesDict.Sum(i => i.Value.Values.Sum(k => k.Length));
            if (allCount >= _maxBatchCount)
            {
                var allOtherMinutesItems = new List<string>();
                foreach (var pair in _minutesDict)
                {
                    allOtherMinutesItems.AddRange(pair.Value.Keys.Where(k => k != minuteKey).Select(i => pair.Value[i]));
                }
                await _messagesHandler(StructureBuilder.MainContainer, allOtherMinutesItems);
                _minutesDict.Clear();
            }

            assetPairDict[minuteKey] = orderbook.GetValuesString();
        }

        public Task<Type> ResolveMessageTypeAsync()
        {
            return Task.FromResult(typeof(InOrderBook));
        }

        private int GetMinuteKey(DateTime time)
        {
            return (((time.Year * 13 + time.Month) * 32 + time.Day) * 25 + time.Hour) * 61 + time.Minute;
        }
    }
}
