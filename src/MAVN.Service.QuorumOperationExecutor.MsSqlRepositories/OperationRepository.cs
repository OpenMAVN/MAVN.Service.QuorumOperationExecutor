using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using MAVN.Persistence.PostgreSQL.Legacy;
using MAVN.Service.QuorumOperationExecutor.Domain.Repositories;
using MAVN.Service.QuorumOperationExecutor.MsSqlRepositories.Contexts;
using MAVN.Service.QuorumOperationExecutor.MsSqlRepositories.Entities;
using Microsoft.EntityFrameworkCore;
using MoreLinq;

namespace MAVN.Service.QuorumOperationExecutor.MsSqlRepositories
{
    public class OperationRepository : IOperationRepository
    {
        private readonly PostgreSQLContextFactory<QoeContext> _contextFactory;

        public OperationRepository(PostgreSQLContextFactory<QoeContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<(string Data, string Hash)> GetOrCreateOperationAsync(Guid operationId)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var operation = await context.Operations.SingleOrDefaultAsync(x => x.Id == operationId);

                if (operation == null)
                {
                    operation = new OperationEntity
                    {
                        Id = operationId
                    };

                    context.Operations.Add(operation);

                    await context.SaveChangesAsync();
                }

                return (operation.TransactionData, operation.TransactionHash);
            }
        }

        public async Task<Dictionary<Guid, (string, string)>> GetOrCreateOperationsAsync(IEnumerable<Guid> operationIds)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var result = new Dictionary<Guid, (string, string)>();
                var batches = operationIds.Batch(10);

                foreach (var batch in batches)
                {
                    var dbEntities = await context.Operations
                        .AsQueryable()
                        .Where(i => batch.Contains(i.Id))
                        .ToListAsync();

                    foreach (var operationId in batch)
                    {
                        var dbEntity = dbEntities.FirstOrDefault(i => i.Id == operationId);
                        if (dbEntity == null)
                            context.Operations.Add(new OperationEntity { Id = operationId });
                        else
                            result.Add(operationId, (dbEntity.TransactionData, dbEntity.TransactionHash));
                    }
                }

                await context.SaveChangesAsync();

                return result;
            }
        }

        public async Task SetTransactionDataAndHashAsync(
            Guid operationId,
            string transactionData,
            string transactionHash)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            using (var context = _contextFactory.CreateDataContext())
            {
                try
                {
                    var operation = await context.Operations.SingleAsync(x => x.Id == operationId);

                    if (string.IsNullOrEmpty(operation.TransactionData))
                    {
                        operation.TransactionData = transactionData;
                        operation.TransactionHash = transactionHash;

                        context.Operations.Update(operation);

                        await context.SaveChangesAsync();
                    }
                }
                finally
                {
                    scope.Complete();
                }
            }
        }

        public async Task SetTransactionsDataAndHashesAsync(Dictionary<Guid, (string, string)> transactionsDataAndHashesDict)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var batches = transactionsDataAndHashesDict.Batch(10);

                foreach (var batch in batches)
                {
                    var ids = batch.Select(b => b.Key).ToList();

                    var dbEntities = await context.Operations
                        .AsQueryable()
                        .Where(i => ids.Contains(i.Id))
                        .ToListAsync();

                    foreach (var pair in batch)
                    {
                        var dbEntity = dbEntities.First(i => i.Id == pair.Key);
                        if (dbEntity.TransactionData == null)
                        {
                            dbEntity.TransactionData = pair.Value.Item1;
                            dbEntity.TransactionHash = pair.Value.Item2;
                            context.Operations.Update(dbEntity);
                        }
                    }
                }

                await context.SaveChangesAsync();
            }
        }

        public async Task<(string Data, string Hash)> TryGetOperationAsync(Guid operationId)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var operation = await context.Operations.SingleOrDefaultAsync(x => x.Id == operationId);

                if (operation != null)
                    return (operation.TransactionData, operation.TransactionHash);

                return (null, null);
            }
        }

        public async Task<Guid?> TryGetOperationIdAsync(string transactionHash)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var operation = await context.Operations
                    .SingleOrDefaultAsync(x => x.TransactionHash == transactionHash);

                return operation?.Id;
            }
        }
    }
}
