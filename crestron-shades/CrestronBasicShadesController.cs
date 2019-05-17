using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.Shades;

using PepperDash.Core;

using PepperDash.Essentials.Core.Config;

namespace CrestronShades
{
	public class CrestronBasicShadesController : Device, PepperDash.Essentials.Core.Shades.ICrestronBasicShade
	{
		public ShadeWithBasicSettings Shade { get; private set; }

		public CrestronBasicShadesController(DeviceConfig dc, CrestronControlSystem cs) :
			base(dc.Key, dc.Name)
		{
			var type = dc.Type.ToLower();
			if (type == "csmqmtdc2564cn")
			{
				var control = PepperDash.Essentials.Core.CommFactory.GetControlPropertiesConfig(dc);
				Shade = new CsmQmtdc2564Cn(control.CresnetIdInt, cs);
			}
		}

		public override bool CustomActivate()
		{
			var result = Shade.Register();
			return result == eDeviceRegistrationUnRegistrationResponse.Success;
		}




		#region IShadesOpenClose Members

		public void Close()
		{
			Shade.Close();
		}

		public void Open()
		{
			Shade.Open();
		}

		#endregion

		#region IShadesStop Members

		public void Stop()
		{
			Shade.Stop();
		}

		#endregion

		#region IShadesStopOrMove Members

		public void CloseOrStop()
		{
			if (IsMovigFeedback.BoolValue)
			{
				Stop();
			}
			else
			{
				Close();
			}
		}

		public void OpenOrStop()
		{
			if (IsMovigFeedback.BoolValue)
			{
				Stop();
			}
			else
			{
				Open();
			}
		}

		#endregion

		#region IShadesFeedback Members

		public PepperDash.Essentials.Core.BoolFeedback IsMovigFeedback
		{
			get { throw new NotImplementedException(); }
		}

		public PepperDash.Essentials.Core.IntFeedback PositionFeedback
		{
			get { throw new NotImplementedException(); }
		}

		#endregion

		#region IShadesRaiseLowerFeedback Members

		public PepperDash.Essentials.Core.BoolFeedback ShadeIsClosedFeedback
		{
			get { throw new NotImplementedException(); }
		}

		public PepperDash.Essentials.Core.BoolFeedback ShadeIsOpenFeedback
		{
			get { throw new NotImplementedException(); }
		}

		#endregion
	}
}