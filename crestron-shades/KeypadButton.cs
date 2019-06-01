using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace CrestronShades
{
	public class KeypadButton
	{
		public string Type { get; set; }
		public string LinkToKey { get; set; }
		public string Command { get; set; }
		public KeypadButtonFeedback Feedback { get; set; }
	}

	public class KeypadButtonFeedback
	{
		public string Type { get; set; }
		public string LinkToKey { get; set; }
	}
}