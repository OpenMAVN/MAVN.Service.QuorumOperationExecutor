using JetBrains.Annotations;
using Lykke.Sdk;
using Lykke.Service.QuorumOperationExecutor.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Lykke.Service.QuorumOperationExecutor
{
    [UsedImplicitly]
    public class Startup
    {
        private readonly LykkeSwaggerOptions _swaggerOptions = new LykkeSwaggerOptions
        {
            ApiTitle = "QuorumOperationExecutor API",
            ApiVersion = "v1"
        };

        [UsedImplicitly]
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            return services.BuildServiceProvider<AppSettings>(options =>
            {
                options.SwaggerOptions = _swaggerOptions;

                options.Logs = logs =>
                {
                    logs.AzureTableName = "QuorumOperationExecutorLog";
                    logs.AzureTableConnectionStringResolver = settings => settings.QuorumOperationExecutorService.Db.LogsConnString;
                };
                options.Extend = (collection, manager) => { collection.AddHttpClient(); };
            });
        }

        [UsedImplicitly]
        public void Configure(IApplicationBuilder app)
        {
            app.UseLykkeConfiguration(options =>
            {
                options.SwaggerOptions = _swaggerOptions;
            });
        }
    }
}
