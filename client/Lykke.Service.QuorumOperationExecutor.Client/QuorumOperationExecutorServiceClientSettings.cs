using JetBrains.Annotations;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.QuorumOperationExecutor.Client 
{
    /// <summary>
    ///    QuorumOperationExecutor client settings.
    /// </summary>
    [PublicAPI]
    public class QuorumOperationExecutorServiceClientSettings 
    {
        /// <summary>
        ///    Service url.
        /// </summary>
        [HttpCheck("api/isalive")]
        public string ServiceUrl {get; set;}
    }
}
