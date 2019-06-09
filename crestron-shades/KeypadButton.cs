using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro.DeviceSupport;

using PepperDash.Essentials.Core;

namespace CrestronShades
{
	/// <summary>
	/// 
	/// </summary>
	public class KeypadButton
	{
		public Dictionary<string, DeviceActionWrapper[]> EventTypes { get; set; }
		public KeypadButtonFeedback Feedback { get; set; }

		public KeypadButton()
		{
			EventTypes = new Dictionary<string, DeviceActionWrapper[]>();
			Feedback = new KeypadButtonFeedback();
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public class KeypadButtonFeedback
	{
		public string Type { get; set; }
		public string LinkToKey { get; set; }
	}
}