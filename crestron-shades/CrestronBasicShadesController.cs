using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharpPro.Shades;

using CrestronShadeBase = Crestron.SimplSharpPro.DeviceSupport.ShadeBase;

using PepperDash.Core;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Config;
using PepperDash.Essentials.Core.Shades;

namespace CrestronShades
{
	public class CrestronBasicShadesController : CrestronGenericBaseDevice, ICrestronBasicShade
	{
		/// <summary>
		/// Use the static constructor to register types with the Core DeviceFactory
		/// </summary>
		public static void LoadPlugin() 
		{
			DeviceFactory.AddFactoryForType("csmqmtdc2564cn", CrestronBasicShadesController.GetCsmQmtdc2564Cn);
			DeviceFactory.AddFactoryForType("csmqmt50dccn", CrestronBasicShadesController.GetCsmQmt50Dccn);
			DeviceFactory.AddFactoryForType("csmqmtdc1631cn", CrestronBasicShadesController.GetCsmQmtdc1631Cn);
		}

		public ShadeWithBasicSettings Shade { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dc"></param>
		/// <returns></returns>
		public static CrestronBasicShadesController GetCsmQmtdc2564Cn(DeviceConfig dc)
		{
			var control = CommFactory.GetControlPropertiesConfig(dc);
			var shade = new CsmQmtdc2564Cn(control.CresnetIdInt, Global.ControlSystem);
			return new CrestronBasicShadesController(dc.Key, dc.Name, shade);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dc"></param>
		/// <returns></returns>
		public static CrestronBasicShadesController GetCsmQmt50Dccn(DeviceConfig dc)
		{
			var control = CommFactory.GetControlPropertiesConfig(dc);
			var shade = new CsmQmt50Dccn(control.CresnetIdInt, Global.ControlSystem);
			return new CrestronBasicShadesController(dc.Key, dc.Name, shade);
		}

		/// <summary>
		/// Ugly form
		/// </summary>
		/// <param name="dc"></param>
		/// <returns></returns>
		public static CrestronBasicShadesController GetCsmQmtdc1631Cn(DeviceConfig dc)
		{
			return new CrestronBasicShadesController(dc.Key, dc.Name,
				new CsmQmtdc1631Cn(CommFactory.GetControlPropertiesConfig(dc).CresnetIdInt, Global.ControlSystem));
		}

		/// <summary>
		/// Private constructor. Use GetDevice factory method
		/// </summary>
		/// <param name="key"></param>
		/// <param name="name"></param>		
		/// <param name="shade"></param>
		CrestronBasicShadesController(string key, string name, ShadeWithBasicSettings shade) :
			base(key, name, shade)
		{
			Shade = shade;
			IsStoppedFeedback = new BoolFeedback(() => Shade.IsStopped.BoolValue);
			PositionFeedback = new IntFeedback(() => Shade.PositionFeedback.UShortValue);
			ShadeIsOpenFeedback = new BoolFeedback(() => Shade.IsFullyOpened.BoolValue);
			ShadeIsClosedFeedback = new BoolFeedback(() => shade.IsFullyClosed.BoolValue);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override bool CustomActivate()
		{
			var result = Shade.Register();
			Shade.BaseEvent += new BaseEventHandler(Shade_BaseEvent);

			return result == eDeviceRegistrationUnRegistrationResponse.Success;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="device"></param>
		/// <param name="args"></param>
		void Shade_BaseEvent(GenericBase device, BaseEventArgs args)
		{
			if (args.EventId == CrestronShadeBase.IsStoppedEventId)
			{
				IsStoppedFeedback.FireUpdate();
				Debug.Console(1, this, "Shade event IsStoppedFeedback={0}", IsStoppedFeedback.BoolValue);
				Debug.Console(1, this, "Shade event PositionFeedback={0}", PositionFeedback.UShortValue);
			}
			else if (args.EventId == CrestronShadeBase.PositionFeedbackEventId)
			{
				PositionFeedback.FireUpdate();
			}
			else if (args.EventId == CrestronShadeBase.IsFullyClosedEventId)
			{
				ShadeIsClosedFeedback.FireUpdate();
				Debug.Console(1, this, "Shade event ShadeIsClosedFeedback={0}", ShadeIsClosedFeedback.BoolValue);
			}
			else if (args.EventId == CrestronShadeBase.IsFullyOpenedEventId)
			{
				ShadeIsOpenFeedback.FireUpdate();
				Debug.Console(1, this, "Shade event ShadeIsOpenFeedback={0}", ShadeIsOpenFeedback.BoolValue);
			}
		}

		public void Close()
		{
			Shade.Close();
		}

		public void Open()
		{
			Shade.Open();
		}

		public void Stop()
		{
			Shade.Stop();
		}

		public void CloseOrStop()
		{
			if (IsStoppedFeedback.BoolValue)
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
			if (IsStoppedFeedback.BoolValue)
			{
				Stop();
			}
			else
			{
				Open();
			}
		}


		public void OpenCloseOrStop()
		{
			if (IsStoppedFeedback.BoolValue)
			{
				if (Shade.LastDirection == eShadeMovement.Closed)
				{
					Open();
				}
				else // if (Shade.LastDirection == eShadeMovement.Opened) NOT SURE ABOUT N/A STATE
				{
					Close();
				}
			}
			else
			{
				Stop();
			}
		}

		public BoolFeedback IsStoppedFeedback { get; private set; }

		public IntFeedback PositionFeedback { get; private set; }

		public BoolFeedback ShadeIsClosedFeedback { get; private set; }

		public BoolFeedback ShadeIsOpenFeedback { get; private set; }

		public void Lower(bool state)
		{
			Shade.Lower.BoolValue = state;
		}

		public void Raise(bool state)
		{
			Shade.Raise.BoolValue = state;
		}

		public BoolFeedback ShadeIsLoweringFeedback
		{
			get { throw new NotImplementedException(); }
		}

		public BoolFeedback ShadeIsRaisingFeedback
		{
			get { throw new NotImplementedException(); }
		}

		public void SetPosition(ushort value)
		{
			Shade.SetPosition(value);
		}
	}
}