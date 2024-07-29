using AwsLambdaDotnet.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Enrichers.Sensitive;

namespace AwsLambdaDotnet
{
    public static class Startup
    {
        public static readonly string? EnvironmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        private static readonly IConfiguration Configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{ EnvironmentName ?? "Production"}.json", optional: true)
            .Build();

        public static IServiceProvider BuildServices()
        {
            var serviceCollection = new ServiceCollection()
                // setup configuration
                .AddSingleton(Configuration)
                .Configure<AppOptions>(Configuration.GetRequiredSection("App"))
                .Configure<SecretsManagerOptions>(Configuration.GetRequiredSection("Aws:SecretsManager"))

                // setup logging
                .AddLogging(builder =>
                {
                    using var log = new LoggerConfiguration()
                        .ReadFrom.Configuration(Configuration)
                        .Enrich
                        .WithSensitiveDataMasking(options =>
                        {
                            options.MaskingOperators = [ new EmailAddressMaskingOperator() ];
                        })
                        .CreateLogger();

                    builder.AddSerilog(log);
                })

                // register services
                ;

            return serviceCollection.BuildServiceProvider();
        }
    }
}
