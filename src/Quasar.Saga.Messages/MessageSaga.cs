using System;
using Automatonymous;

namespace Quasar.Saga.Messages
{
	public class MessageSaga : SagaStateMachineInstance
	{
		/// <inheritdoc />
		public Guid CorrelationId { get; set; }

		public string CurrentState { get; set; }
		
		public Guid ChatId { get; set; }
	}
}
