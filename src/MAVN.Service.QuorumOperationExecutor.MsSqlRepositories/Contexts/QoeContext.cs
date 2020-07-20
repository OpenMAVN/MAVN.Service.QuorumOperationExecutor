﻿using System.Data.Common;
using JetBrains.Annotations;
using MAVN.Persistence.PostgreSQL.Legacy;
using MAVN.Service.QuorumOperationExecutor.MsSqlRepositories.Entities;
using Microsoft.EntityFrameworkCore;

namespace MAVN.Service.QuorumOperationExecutor.MsSqlRepositories.Contexts
{
    public class QoeContext : PostgreSQLContext
    {
        private const string Schema = "quorum_operation_executor";

        internal DbSet<OperationEntity> Operations { get; set; }

        // C-tor for migrations
        [UsedImplicitly]
        public QoeContext()
            : base(Schema)
        {
        }

        public QoeContext(string connectionString, bool isTraceEnabled)
            : base(Schema, connectionString, isTraceEnabled)
        {
        }

        public QoeContext(DbConnection dbConnection)
            : base(Schema, dbConnection)
        {
        }

        protected override void OnMAVNModelCreating(
            ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<OperationEntity>()
                .HasIndex(b => b.TransactionHash);
        }
    }
}
