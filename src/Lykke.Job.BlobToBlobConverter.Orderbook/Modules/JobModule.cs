﻿using Autofac;
using Common.Log;
using Lykke.Job.BlobToBlobConverter.Common.Abstractions;
using Lykke.Job.BlobToBlobConverter.Common.Services;
using Lykke.Job.BlobToBlobConverter.Orderbook.Core.Services;
using Lykke.Job.BlobToBlobConverter.Orderbook.PeriodicalHandlers;
using Lykke.Job.BlobToBlobConverter.Orderbook.Services;
using Lykke.Job.BlobToBlobConverter.Orderbook.Settings;

namespace Lykke.Job.BlobToBlobConverter.Orderbook.Modules
{
    public class JobModule : Module
    {
        private readonly BlobToBlobConverterOrderbookSettings _settings;
        private readonly ILog _log;

        public JobModule(BlobToBlobConverterOrderbookSettings settings, ILog log)
        {
            _settings = settings;
            _log = log;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(_log)
                .As<ILog>()
                .SingleInstance();

            builder.RegisterType<HealthService>()
                .As<IHealthService>()
                .SingleInstance();

            builder.RegisterType<StartupManager>()
                .As<IStartupManager>()
                .SingleInstance();

            builder.RegisterType<ShutdownManager>()
                .As<IShutdownManager>()
                .AutoActivate()
                .SingleInstance();

            builder.RegisterType<BlobReader>()
                .As<IBlobReader>()
                .SingleInstance()
                .WithParameter("container", _settings.InputContainer)
                .WithParameter("blobConnectionString", _settings.InputBlobConnString)
                .WithParameter("skipCorrupted", _settings.SkipCorrupted);

            builder.RegisterType<BlobSaver>()
                .As<IBlobSaver>()
                .SingleInstance()
                .WithParameter("blobConnectionString", _settings.OutputBlobConnString)
                .WithParameter("rootContainer", _settings.InputContainer);

            builder.RegisterType<MessageProcessor>()
                .As<IMessageProcessor>()
                .As<IMessageTypeResolver>()
                .SingleInstance();

            builder.RegisterType<StructureBuilder>()
                .As<IStructureBuilder>()
                .SingleInstance();

            builder.RegisterType<BlobProcessor>()
                .As<IBlobProcessor>()
                .SingleInstance();

            builder.RegisterType<PeriodicalHandler>()
                .As<IStartStop>()
                .AutoActivate()
                .SingleInstance()
                .WithParameter(TypedParameter.From(_settings.BlobScanPeriod));
        }
    }
}
