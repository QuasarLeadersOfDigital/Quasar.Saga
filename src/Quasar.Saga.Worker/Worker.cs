using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Quasar.Saga.Worker
{
	public class Worker : BackgroundService
	{
		private readonly ILogger<Worker> _logger;
		private readonly IBusControl _busControl;

		public Worker(ILogger<Worker> logger, IBusControl busControl)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_busControl = busControl ?? throw new ArgumentNullException(nameof(busControl));
		}

		protected override async Task ExecuteAsync(CancellationToken cancellationToken)
		{
			await _busControl.StartAsync(cancellationToken);
		}

		/// <inheritdoc />
		public override async Task StopAsync(CancellationToken cancellationToken)
		{
			await _busControl.StopAsync(cancellationToken);
		}
	}
}
