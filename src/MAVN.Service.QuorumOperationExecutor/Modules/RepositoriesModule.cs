using Autofac;
using JetBrains.Annotations;
using MAVN.Service.QuorumOperationExecutor.Domain.Repositories;
using MAVN.Service.QuorumOperationExecutor.MsSqlRepositories;
using MAVN.Service.QuorumOperationExecutor.MsSqlRepositories.Contexts;
using MAVN.Service.QuorumOperationExecutor.Settings;
using Lykke.SettingsReader;
using MAVN.Persistence.PostgreSQL.Legacy;

namespace MAVN.Service.QuorumOperationExecutor.Modules
{
    [UsedImplicitly]
    public class RepositoriesModule : Module
    {
        private readonly DbSettings _dbSettings;

        public RepositoriesModule(
            IReloadingManager<AppSettings> appSettings)
        {
            _dbSettings = appSettings.CurrentValue.QuorumOperationExecutorService.Db;
        }

        protected override void Load(
            ContainerBuilder builder)
        {
            builder
                .RegisterPostgreSQL(
                    _dbSettings.DataConnString,
                    connString => new QoeContext(connString, false),
                    dbConn => new QoeContext(dbConn));

            builder
                .Register(ctx => new OperationRepository
                (
                    ctx.Resolve<PostgreSQLContextFactory<QoeContext>>()
                ))
                .As<IOperationRepository>()
                .SingleInstance();
        }
    }
}
