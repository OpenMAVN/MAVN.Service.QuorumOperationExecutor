using JetBrains.Annotations;

namespace MAVN.Service.QuorumOperationExecutor.Settings
{
    [UsedImplicitly]
    public class QuorumOperationExecutorSettings
    {
        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public BlockchainSettings Blockchain { get; set; }
        
        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public DbSettings Db { get; set; }
    }
}
