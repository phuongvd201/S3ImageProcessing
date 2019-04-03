using System;
using System.Data.Common;
using System.IO;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using MySql.Data.MySqlClient;

using S3ImageProcessing.Data;
using S3ImageProcessing.S3Bucket;
using S3ImageProcessing.Services.Implementations;
using S3ImageProcessing.Services.Interfaces;

namespace S3ImageProcessing
{
    class Program
    {
        private static IServiceProvider _serviceProvider;

        private static IConfiguration _configuration;

        static void Main(string[] args)
        {
            var configurationBuilder = new ConfigurationBuilder().SetBasePath(Path.Combine(AppContext.BaseDirectory)).AddJsonFile("appsettings.json", true, true);

            _configuration = configurationBuilder.Build();

            RegisterServices(_configuration);

            _serviceProvider.GetService<S3ImageProcessingApp>().Start().GetAwaiter().GetResult();

            DisposeServices();
        }

        private static void RegisterServices(IConfiguration configuration)
        {
            DbProviderFactories.RegisterFactory("MySql.Data.MySqlClient", MySqlClientFactory.Instance);

            var serviceCollection = new ServiceCollection();

            serviceCollection
                .AddOptions()
                .AddLogging(opt => opt.AddConsole())
                .Configure<S3ClientOption>(configuration.GetSection(nameof(S3ClientOption)))
                .Configure<DatabaseOption>(configuration.GetSection(nameof(DatabaseOption)))
                .AddSingleton<S3CBucketClient>()
                .AddSingleton<IDbAccess, DbAccess>()
                .AddSingleton<IImageStorageProvider, S3ImageStorageProvider>()
                .AddSingleton<IParsedImageStore, ParsedImageStore>()
                .AddSingleton<IImageHistogramService, ImageHistogramService>()
                .AddSingleton<S3ImageProcessingApp>();

            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        private static void DisposeServices()
        {
            if (_serviceProvider == null)
            {
                return;
            }

            if (_serviceProvider is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}