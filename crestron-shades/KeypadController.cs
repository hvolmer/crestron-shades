using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro.DeviceSupport;
using cresKps = Crestron.SimplSharpPro.Keypads;
using Newtonsoft.Json.Linq;
using PepperDash.Core;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Config;

namespace CrestronShades
{
	public class KeypadController: CrestronGenericBaseDevice
	{
		public cresKps.C2nCbdBase Keypad { get; private set; }

		Dictionary<uint, KeypadButton> Buttons = new Dictionary<uint, KeypadButton>();
		List<KeypadButtonFeedbackBase> Feedbacks = new List<KeypadButtonFeedbackBase>();

		/// <summary>
		/// Essentials loader method.  Called on all implementing classes in plugin dll
		/// </summary>
		public static void LoadPlugin()
		{
			DeviceFactory.AddFactoryForType("c2ncbdp", KeypadController.GetC2nCbdPController);
			DeviceFactory.AddFactoryForType("c2ncbde", KeypadController.GetC2nCbdPController);
		}

		/// <summary>
		/// Factory for P model controller
		/// </summary>
		/// <param name="dc"></param>
		/// <returns></returns>
		public static KeypadController GetC2nCbdPController(DeviceConfig dc)
		{
			var control = CommFactory.GetControlPropertiesConfig(dc);
			var kp = new cresKps.C2nCbdP(control.CresnetIdInt, Global.ControlSystem);
			return FinishFactory(kp, dc);
		}

		/// <summary>
		/// Factory for P model controller
		/// </summary>
		/// <param name="dc"></param>
		/// <returns></returns>
		public static KeypadController GetC2nCbdEController(DeviceConfig dc)
		{
			var control = CommFactory.GetControlPropertiesConfig(dc);
			var kp = new cresKps.C2nCbdE(control.CresnetIdInt, Global.ControlSystem);
			return FinishFactory(kp, dc);
		}

		/// <summary>
		/// Finishes up building a controller
		/// </summary>
		/// <param name="kp"></param>
		/// <param name="dc"></param>
		/// <returns></returns>
		static KeypadController FinishFactory(cresKps.C2nCbdBase kp, DeviceConfig dc)
		{
			var ctrl = new KeypadController(dc.Key, dc.Name, kp);
			var butToken = dc.Properties["buttons"];
			if (butToken != null)
			{
				ctrl.Buttons = butToken.ToObject<Dictionary<uint, KeypadButton>>();
			}
			return ctrl;
		}

		/// <summary>
		/// Constructor. Does nothing special
		/// </summary>
		/// <param name="key"></param>
		/// <param name="name"></param>
		/// <param name="keypad"></param>
		public KeypadController(string key, string name, cresKps.C2nCbdBase keypad)
			: base(key, name, keypad)
		{
			Keypad = keypad;
		}

		/// <summary>
		/// Registers button events
		/// </summary>
		/// <returns></returns>
		public override bool CustomActivate()
		{
			Keypad.ButtonStateChange += new ButtonEventHandler(Keypad_ButtonStateChange);

			//TODO wire up feedbacks
			

			return base.CustomActivate();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="device"></param>
		/// <param name="args"></param>
		void Keypad_ButtonStateChange(Crestron.SimplSharpPro.GenericBase device, ButtonEventArgs args)
		{
			Debug.Console(0, this, "Button {0}, {1}", args.Button.Number, args.NewButtonState);
			if (Buttons.ContainsKey(args.Button.Number))
			{
				var type = args.NewButtonState.ToString();
				Press(args.Button.Number, type);
			}
		}

		/// <summary>
		/// Runs the function associated with this button/type. One of the following strings:
		/// Pressed, Released, Tapped, DoubleTapped, Held, HeldReleased
		/// </summary>
		/// <param name="number"></param>
		/// <param name="type"></param>
		public void Press(uint number, string type)
		{
			if(!Buttons.ContainsKey(number)) { return; }
			var but = Buttons[number];
			if (but.EventTypes.ContainsKey(type))
			{
				foreach (var a in but.EventTypes[type]) { DeviceJsonApi.DoDeviceAction(a); }
			}
		}
	}
}	