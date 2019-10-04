using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro.Keypads;

namespace CrestronShades
{
	public class KeypadButtonFeedbackBase
	{
		protected Crestron.SimplSharpPro.DeviceSupport.FeedbackWithBlinkPattern Feedback;

		public KeypadButtonFeedbackBase(Crestron.SimplSharpPro.DeviceSupport.FeedbackWithBlinkPattern feedback)
		{
			Feedback = feedback;
		}
	}
}