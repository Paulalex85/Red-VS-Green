﻿using System;
using System.Collections;

namespace RedVsGreen
{
	public class Message
	{
		public string name { get; set; }
		public IEnumerable args { get; set; }

		public Message () : this ("", null) {}

		public Message (string Name, IEnumerable Args)
		{
			name = Name;
			args = Args;
		}
	}
}

