using System;
using Quasar.Contracts.Events;

namespace Quasar.Saga.Messages.Events
{
	public class MessageReadEvent : IMessageReadEvent
	{
		public MessageReadEvent(Guid messageId, Guid chatId)
		{
			MessageId = messageId;
			ChatId = chatId;
		}

		/// <inheritdoc />
		public Guid MessageId { get; }

		/// <inheritdoc />
		public Guid ChatId { get; }
	}
}
