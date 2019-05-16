using System;
using Crestron.SimplSharp;                          			
using Crestron.SimplSharpPro;

using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Config;

using PepperDash.Essentials.Core.Factory;    
using CrestronShades;               			

namespace PepperDash.Essentials.Plugin
{
	public class Factory: IGetCrestronDevice
	{

		public static CrestronControlSystem ControlSystem;

		#region IGetDevice Members

		public PepperDash.Core.IKeyed GetDevice(PepperDash.Essentials.Core.Config.DeviceConfig dc, CrestronControlSystem cs)
		{
			var type = dc.Type.ToLower();
			if (type == "csmqmtdc2564cn")
			{
				return new CrestronBasicShadesController(dc, cs);
			}

			return null;
		}
		#endregion
	}
}

