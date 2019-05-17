using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharpPro.Shades;

//using PepperDash.Core;

using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Config;

namespace CrestronShades
{
	public class CrestronBasicShadesController : CrestronGenericBaseDevice, PepperDash.Essentials.Core.Shades.ICrestronBasicShade
	{
		public ShadeWithBasicSettings Shade { get; private set; }

		/// <summary>
		/// Factory method to create a shade controller from config.
		/// </summary>
		/// <param name="dc"></param>
		/// <param name="cs"></param>
		/// <returns></returns>
		public static CrestronBasicShadesController GetDevice(DeviceConfig dc, CrestronControlSystem cs)
		{
			var type = dc.Type.ToLower();
			CrestronBasicShadesController ctrl = null;

			var control = PepperDash.Essentials.Core.CommFactory.GetControlPropertiesConfig(dc);
			if (type == "csmqmtdc2564cn")
			{
				var shade = new CsmQmtdc2564Cn(control.CresnetIdInt, cs);
				ctrl = new CrestronBasicShadesController(dc.Key, dc.Name, shade);
			}

			return ctrl;
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
			if (args.EventId == ShadeBase.IsStoppedEventId)
			{
				IsStoppedFeedback.FireUpdate();
			}
			else if (args.EventId == ShadeBase.PositionFeedbackEventId)
			{
				PositionFeedback.FireUpdate();
			}
			else if (args.EventId == ShadeBase.IsFullyClosedEventId)
			{
				ShadeIsClosedFeedback.FireUpdate();
			}
			else if (args.EventId == ShadeBase.IsFullyOpenedEventId)
			{
				ShadeIsOpenFeedback.FireUpdate();
			}
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
		}

		#endregion

		#region IShadesFeedback Members

		public PepperDash.Essentials.Core.BoolFeedback IsStoppedFeedback { get; private set; }

		public PepperDash.Essentials.Core.IntFeedback PositionFeedback { get; private set; }

		#endregion

		#region IShadesRaiseLowerFeedback Members

		public PepperDash.Essentials.Core.BoolFeedback ShadeIsClosedFeedback { get; private set; }

		public PepperDash.Essentials.Core.BoolFeedback ShadeIsOpenFeedback { get; private set; }

		#endregion

		#region IShadesRaiseLower Members

		public void Lower(bool state)
		{
			Shade.Lower.BoolValue = state;
		}

		public void Raise(bool state)
		{
			Shade.Raise.BoolValue = state;
		}

		#endregion
	}
}