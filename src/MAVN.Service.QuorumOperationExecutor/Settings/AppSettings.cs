using JetBrains.Annotations;
using Lykke.Sdk.Settings;
using MAVN.Service.QuorumTransactionSigner.Client;

namespace MAVN.Service.QuorumOperationExecutor.Settings
{
    [UsedImplicitly]
    public class AppSettings : BaseAppSettings
    {
        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public QuorumOperationExecutorSettings QuorumOperationExecutorService { get; set; }
        
        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public QuorumTransactionSignerServiceClientSettings QuorumTransactionSignerService { get; set; }
    }
}
