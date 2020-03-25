using Autofac;
using JetBrains.Annotations;
using Lykke.Common.MsSql;
using Lykke.Service.QuorumOperationExecutor.Domain.Repositories;
using Lykke.Service.QuorumOperationExecutor.MsSqlRepositories;
using Lykke.Service.QuorumOperationExecutor.MsSqlRepositories.Contexts;
using Lykke.Service.QuorumOperationExecutor.Settings;
using Lykke.SettingsReader;

namespace Lykke.Service.QuorumOperationExecutor.Modules
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
                .RegisterMsSql(
                    _dbSettings.DataConnString,
                    connString => new QoeContext(connString, false),
                    dbConn => new QoeContext(dbConn));

            builder
                .Register(ctx => new OperationRepository
                (
                    ctx.Resolve<MsSqlContextFactory<QoeContext>>()
                ))
                .As<IOperationRepository>()
                .SingleInstance();
        }
    }
}
