using azure_functions.Infrastructure;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

[assembly: FunctionsStartup(typeof(azure_functions.Startup))]
namespace azure_functions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            string dbEndpoint = Environment.GetEnvironmentVariable("CosmosDbEndpoint");
            string dbKey = Environment.GetEnvironmentVariable("CosmosDbKey");
            string dbName = Environment.GetEnvironmentVariable("CosmosDbName");
            builder.Services.AddDbContext<NoteDbContext>(
              options => options.UseCosmos(dbEndpoint, dbKey, dbName));
        }
    }
}
