using System;
using JetBrains.Annotations;
using Lykke.SettingsReader.Attributes;

namespace MAVN.Service.QuorumOperationExecutor.Settings
{
    [UsedImplicitly]
    public class BlockchainSettings
    {
        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public string CustomerRegistryAddress { get; set; }

        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public string TokenContractAddress { get; set; }

        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public string PrivateBlockchainGatewayContractAddress { get; set; }

        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public string[] TransactionNodeUrlList { get; set; }

        [Optional]
        public int? MaxThreadCount { get; set; }
        
        [Optional]
        public TimeSpan? WebSocketsConnectionTimeOut { get; set; }
    }
}
