﻿using System.Threading.Tasks;

namespace Lykke.Job.BlobToBlobConverter.Orderbook.Core.Services
{
    public interface IStartupManager
    {
        Task StartAsync();
    }
}