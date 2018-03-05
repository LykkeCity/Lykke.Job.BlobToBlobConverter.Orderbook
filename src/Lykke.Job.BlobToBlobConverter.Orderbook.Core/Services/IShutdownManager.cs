﻿using System.Threading.Tasks;
using Common;

namespace Lykke.Job.BlobToBlobConverter.Orderbook.Core.Services
{
    public interface IShutdownManager
    {
        Task StopAsync();

        void Register(IStopable stopable);
    }
}
