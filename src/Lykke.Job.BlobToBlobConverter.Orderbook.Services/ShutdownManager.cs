﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Common;
using JetBrains.Annotations;
using Lykke.Job.BlobToBlobConverter.Orderbook.Core.Services;

namespace Lykke.Job.BlobToBlobConverter.Orderbook.Services
{
    [UsedImplicitly]
    public class ShutdownManager : IShutdownManager
    {
        private readonly List<IStopable> _items = new List<IStopable>();

        public ShutdownManager(IEnumerable<IStartStop> stopables)
        {
            _items.AddRange(stopables);
        }

        public Task StopAsync()
        {
            Parallel.ForEach(_items, i => i.Stop());

            return Task.CompletedTask;
        }
    }
}
