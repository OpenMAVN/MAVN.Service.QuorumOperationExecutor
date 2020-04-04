using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MAVN.Service.QuorumOperationExecutor.MsSqlRepositories.Entities
{
    [Table("operations")]
    public class OperationEntity
    {
        [Key, Required]
        [Column("id")]
        public Guid Id { get; set; }
        
        [Column("transaction_hash")]
        public string TransactionHash { get; set; }
        
        [Column("transaction_data")]
        public string TransactionData { get; set; }
    }
}
