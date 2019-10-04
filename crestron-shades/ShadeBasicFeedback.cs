using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharpPro.Keypads;

namespace CrestronShades
{
	public class ShadeBasicFeedback : KeypadButtonFeedbackBase
	{
		CrestronBasicShadesController Shade;

		public ShadeBasicFeedback(CrestronBasicShadesController shade, Crestron.SimplSharpPro.DeviceSupport.FeedbackWithBlinkPattern feedback)
			: base(feedback)
		{
			Shade.IsStoppedFeedback.OutputChange += new EventHandler<PepperDash.Essentials.Core.FeedbackEventArgs>(IsStoppedFeedback_OutputChange);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void IsStoppedFeedback_OutputChange(object sender, PepperDash.Essentials.Core.FeedbackEventArgs e)
		{
			if (Shade.IsStoppedFeedback.BoolValue)
			{
				Feedback.State = false;
				Feedback.BlinkPattern = eButtonBlinkPattern.AlwaysOn;
			}
			else
			{
				Feedback.BlinkPattern = eButtonBlinkPattern.MediumBlink;
				Feedback.State = true;
			}	
		}
	}
}