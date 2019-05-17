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
		/// <summary>
		/// Factory method for assembly.
		/// </summary>
		/// <param name="dc">Device config object</param>
		/// <param name="cs">CrestronControlSystem, because we need it for Crestron things</param>
		/// <returns>Shade device controller</returns>
		public PepperDash.Core.IKeyed GetDevice(PepperDash.Essentials.Core.Config.DeviceConfig dc, CrestronControlSystem cs)
		{
			// Possibly add switching if more device controller types are necessary
			return CrestronBasicShadesController.GetDevice(dc, cs);
		}
	}
}

