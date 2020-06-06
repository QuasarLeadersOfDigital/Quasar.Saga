using System;
using Quasar.Contracts.Events;

namespace Quasar.Saga.Messages.Events
{
	public class MessageSentEvent : IMessageSentEvent
	{
		public MessageSentEvent(Guid messageId, Guid channelId, string content)
		{
			MessageId = messageId;
			ChatId = channelId;
			Content = content;
		}

		public Guid MessageId { get; }

		/// <inheritdoc />
		public Guid ChatId { get;  }

		/// <inheritdoc />
		public string Content { get; }
	}
}
