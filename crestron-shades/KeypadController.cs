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
	public class KeypadController: Device
	{
		public cresKps.C2nCbdBase Keypad { get; private set; }

		Dictionary<uint, KeypadButton> KeypadButtons = new Dictionary<uint, KeypadButton>();

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
			return new KeypadController(dc.Key, dc.Name, kp);	
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
			return new KeypadController(dc.Key, dc.Name, kp);
		}

		/// <summary>
		/// Constructor. Does nothing special
		/// </summary>
		/// <param name="key"></param>
		/// <param name="name"></param>
		/// <param name="keypad"></param>
		public KeypadController(string key, string name, cresKps.C2nCbdBase keypad)
			: base(key, name)
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

			return base.CustomActivate();
		}

		void Keypad_ButtonStateChange(Crestron.SimplSharpPro.GenericBase device, ButtonEventArgs args)
		{
			
		}

		void WireUpButtons(DeviceConfig dc)
		{
			if (dc.Properties == null) { return; }
			var buts = dc.Properties["buttons"];
			if (buts == null) { return; }
			KeypadButtons = buts.ToObject<Dictionary<uint, KeypadButton>>();

			// make the presses do something

			// wire up the feedbacks

		}
	}
}