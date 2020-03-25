using System.Collections.Generic;
using Autofac;
using JetBrains.Annotations;
using Lykke.Common.Log;
using Lykke.Service.QuorumOperationExecutor.Domain.Repositories;
using Lykke.Service.QuorumOperationExecutor.Domain.Services;
using Lykke.Service.QuorumOperationExecutor.DomainServices;
using Lykke.Service.QuorumOperationExecutor.DomainServices.Strategies;
using Lykke.Service.QuorumOperationExecutor.Settings;
using Lykke.Service.QuorumTransactionSigner.Client;
using Lykke.SettingsReader;

namespace Lykke.Service.QuorumOperationExecutor.Modules
{
    [UsedImplicitly]
    public class ServiceModule : Module
    {
        private readonly QuorumOperationExecutorSettings _settings;

        public ServiceModule(IReloadingManager<AppSettings> appSettings)
        {
            _settings = appSettings.CurrentValue.QuorumOperationExecutorService;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder
                .Register(ctx => new OperationService
                (
                    ctx.Resolve<IEnumerable<IBuildTransactionStrategy>>(),
                    ctx.Resolve<IBlockchain>(),
                    ctx.Resolve<ILogFactory>(),
                    ctx.Resolve<IOperationRepository>(),
                    ctx.Resolve<ITransactionService>(),
                    ctx.Resolve<IQuorumTransactionSignerClient>(),
                    _settings.Blockchain.MaxThreadCount
                ))
                .As<IOperationService>()
                .SingleInstance();

            builder
                .Register(ctx => new TransactionService(
                    ctx.Resolve<IOperationRepository>(),
                    ctx.Resolve<IBlockchain>()
                ))
                .As<ITransactionService>()
                .SingleInstance();

            builder
                .Register(ctx => new TokenService
                (
                    ctx.Resolve<IBlockchain>()
                ))
                .As<ITokenService>()
                .SingleInstance();

            builder
                .Register(ctx => new TokenGatewayService
                (
                    ctx.Resolve<IBlockchain>()
                ))
                .As<ITokenGatewayService>()
                .SingleInstance();
        }
    }
}
