using Automatonymous;
using Microsoft.Extensions.Logging;
using Quasar.Contracts.Commands;
using Quasar.Saga.Messages.Events;

namespace Quasar.Saga.Messages
{
	public class MessageSagaStateMachine : MassTransitStateMachine<MessageSaga>
	{
		public State Sent { get; private set; }
		
		public Event<IMessageSendCommand> MessageReceived { get; private set; }
		
		public Event<IMessageReadCommand> MessageRead { get; private set; }
		
		/// <inheritdoc />
		public MessageSagaStateMachine(ILogger<MessageSagaStateMachine> logger)
		{
			this.InstanceState(instance => instance.CurrentState);
			
			this.Event(() => MessageReceived, x => x.CorrelateById(@event => @event.Message.MessageId));
			this.Event(() => MessageRead, x => x.CorrelateById(@event => @event.Message.MessageId));
			
			this.Initially(When(MessageReceived)
				.Then(context =>
				{
					logger.LogInformation($"State: {context.Instance.CurrentState}; CorrelationId: {context.Instance.CorrelationId}");
					context.Instance.ChatId = context.Data.ChatId;
				})
				.Publish(context => new MessageSentEvent(context.Data.MessageId, context.Data.ChatId, context.Data.Content))
				.TransitionTo(Sent)
				.Then(context => logger.LogInformation($"State: {context.Instance.CurrentState}; CorrelationId: {context.Instance.CorrelationId}")));
			
			this.During(Sent,
				When(MessageRead)
					.Then(context => context.Instance.ChatId = context.Data.ChatId)
					.Publish(context => new MessageReadEvent(context.Data.MessageId, context.Data.ChatId))
					.Finalize()
					.Then(context => logger.LogInformation($"State: {context.Instance.CurrentState}; CorrelationId: {context.Instance.CorrelationId}")));
		}
	}
}
