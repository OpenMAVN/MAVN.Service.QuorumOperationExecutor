using System.Net.Http;
using Autofac;
using JetBrains.Annotations;
using Lykke.Common.Log;
using MAVN.Service.QuorumOperationExecutor.Clients;
using MAVN.Service.QuorumOperationExecutor.DomainServices;
using MAVN.Service.QuorumOperationExecutor.DomainServices.Strategies;
using MAVN.Service.QuorumOperationExecutor.Settings;
using Lykke.SettingsReader;

namespace MAVN.Service.QuorumOperationExecutor.Modules
{
    [UsedImplicitly]
    public class BlockchainModule : Module
    {
        private readonly IReloadingManager<BlockchainSettings> _blockchainSettings;

        public BlockchainModule(IReloadingManager<AppSettings> appSettings)
        {
            _blockchainSettings = appSettings.Nested(e => e.QuorumOperationExecutorService.Blockchain);
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder
                .Register(ctx => new BlockchainWrapper(
                    async () => (await _blockchainSettings.Reload()).TransactionNodeUrlList,
                    _blockchainSettings.CurrentValue.TokenContractAddress,
                    _blockchainSettings.CurrentValue.PrivateBlockchainGatewayContractAddress,
                    _blockchainSettings.CurrentValue.WebSocketsConnectionTimeOut,
                    ctx.Resolve<IHttpClientFactory>(),
                    ctx.Resolve<ILogFactory>()))
                .As<IBlockchain>()
                .As<IStartable>()
                .SingleInstance();

            var blockchainSettingsValue = _blockchainSettings.CurrentValue;

            builder
                .Register(ctx => new BuildCustomerWalletCreationTransactionStrategy
                (
                    blockchainSettingsValue.CustomerRegistryAddress
                ))
                .As<IBuildTransactionStrategy>()
                .SingleInstance();

            builder
                .Register(ctx => new BuildBonusRewardTransactionStrategy
                (
                    blockchainSettingsValue.TokenContractAddress
                ))
                .As<IBuildTransactionStrategy>()
                .SingleInstance();

            builder
                .Register(ctx => new BuildTransactionStrategy())
                .As<IBuildTransactionStrategy>()
                .SingleInstance();

            builder
                .Register(ctx => new BuildTransferStrategy
                (
                    blockchainSettingsValue.TokenContractAddress
                ))
                .As<IBuildTransactionStrategy>()
                .SingleInstance();

            builder
                .Register(ctx => new BuildWalletLinkingTransactionStrategy
                (
                    blockchainSettingsValue.PrivateBlockchainGatewayContractAddress
                ))
                .As<IBuildTransactionStrategy>()
                .SingleInstance();

            builder
                .Register(ctx => new BuildWalletUnlinkingTransactionStrategy
                (
                    blockchainSettingsValue.PrivateBlockchainGatewayContractAddress
                ))
                .As<IBuildTransactionStrategy>()
                .SingleInstance();

            //We are using TokenContractAddress cause this strategy requires transfer to a specific address.
            builder
                .Register(ctx => new BuildTransferToExternalTransactionStrategy
                (
                    blockchainSettingsValue.TokenContractAddress
                ))
                .As<IBuildTransactionStrategy>()
                .SingleInstance();

            builder
                .Register(ctx => new BuildTransferToInternalTransactionStrategy
                (
                    blockchainSettingsValue.PrivateBlockchainGatewayContractAddress
                ))
                .As<IBuildTransactionStrategy>()
                .SingleInstance();

            builder
                .Register(ctx => new BuildSetTransferToPublicNetworkFeeStrategy
                (
                    blockchainSettingsValue.PrivateBlockchainGatewayContractAddress
                ))
                .As<IBuildTransactionStrategy>()
                .SingleInstance();

            builder
                .Register(ctx => new BuildSeizeToInternalStrategy
                (
                    blockchainSettingsValue.TokenContractAddress
                ))
                .As<IBuildTransactionStrategy>()
                .SingleInstance();
        }
    }
}
