using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.Saga;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quasar.Saga.Messages;
using Quasar.Saga.Worker.Settings;
using Serilog;

namespace Quasar.Saga.Worker
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Log.Logger = new LoggerConfiguration()
				.Enrich.FromLogContext()
				.WriteTo.Console()
				.CreateLogger();

			try
			{
				Log.Information("Starting up");
				CreateHostBuilder(args).Build().Run();
			}
			catch (Exception ex)
			{
				Log.Fatal(ex, "Application start-up failed");
			}
			finally
			{
				Log.CloseAndFlush();
			}
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureAppConfiguration((hostContext, config) =>
				{
					if (hostContext.HostingEnvironment.IsProduction())
					{
						config.AddSystemsManager("/Quasar.Saga");
					}
				})
				.ConfigureServices((hostContext, services) =>
				{
					var sagaSettings = hostContext.Configuration
						.GetSection("SagaSettings")
						.Get<SagaSettings>();
					
					services.AddMassTransit(massTransit =>
					{
						massTransit.AddBus(servicesProvider =>
						{
							return Bus.Factory.CreateUsingRabbitMq(configure =>
							{
								configure.Host(sagaSettings.Host);

								configure.ReceiveEndpoint(sagaSettings.EndpointName, queue =>
								{
									queue.StateMachineSaga(new MessageSagaStateMachine(
											servicesProvider.Container.GetService<ILogger<MessageSagaStateMachine>>()),
										new InMemorySagaRepository<MessageSaga>());
								});
							});
						});
					});
					
					services.AddHostedService<Worker>();
				});
	}
}
