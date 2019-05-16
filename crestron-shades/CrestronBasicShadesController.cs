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
	public class CrestronBasicShadesController : Device
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

	}
}