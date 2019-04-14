﻿using System;

namespace RedVsGreen
{
	public class MessageID
	{
		public string MessageId { get; set; }
		public string Endpoint { get; set; }

		public MessageID ()
		{
		}

		public MessageID (string messageId, string endpoint)
		{
			MessageId = messageId;
			Endpoint = endpoint;
		}
	}
}

