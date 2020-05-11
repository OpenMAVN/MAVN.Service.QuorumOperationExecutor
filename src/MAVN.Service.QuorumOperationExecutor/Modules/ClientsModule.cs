using Autofac;
using JetBrains.Annotations;
using MAVN.Service.QuorumOperationExecutor.Settings;
using MAVN.Service.QuorumTransactionSigner.Client;
using Lykke.SettingsReader;

namespace MAVN.Service.QuorumOperationExecutor.Modules
{
    [UsedImplicitly]
    public class ClientsModule : Module
    {
        private readonly AppSettings _appSettings;

        public ClientsModule(
            IReloadingManager<AppSettings> appSettings)
        {
            _appSettings = appSettings.CurrentValue;
        }

        protected override void Load(
            ContainerBuilder builder)
        {
            builder
                .RegisterQuorumTransactionSignerClient(_appSettings.QuorumTransactionSignerService, null);
        }
    }
}
