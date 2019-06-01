using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

using Newtonsoft.Json.Linq;

using PepperDash.Core;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Config;
using PepperDash.Essentials.Core.Shades;

namespace CrestronShades
{
	public class SceneController: Device, ISceneFeedback, IShadesStopFeedback
	{
		/// <summary>
		/// Essentials loader method.  Called on all implementing classes in plugin dll
		/// </summary>
		public static void LoadPlugin() 
		{
			DeviceFactory.AddFactoryForType("scenecontroller", SceneController.GetSceneController);
		}

		/// <summary>
		/// Factory for scene controller
		/// </summary>
		/// <param name="dc"></param>
		/// <returns></returns>
		public static SceneController GetSceneController(DeviceConfig dc) 
		{
			try
			{
				var sc = new SceneController(dc.Key, dc.Name);
				sc.SceneItems = dc.Properties["items"].Values<SceneItem>().ToList();
				return sc;
			}
			catch (Exception e)
			{
				Debug.Console(0, Debug.ErrorLogLevel.Error, "Error creating scene controller '{0}': {1}", dc.Key, e);
				return null;
			}

		}

		public BoolFeedback AllAreAtSceneFeedback { get; private set; }

		public BoolFeedback IsStoppedFeedback { get; private set; }

		List<SceneItem> SceneItems = new List<SceneItem>();

		bool _AllAreAtScene;

		bool _IsStopped;

		/// <summary>
		/// Constructor, does nothing
		/// </summary>
		/// <param name="key"></param>
		/// <param name="name"></param>
		public SceneController(string key, string name): base(key, name)
		{
		}

		/// <summary>
		/// Registers for events on loads and sets up feedbacks
		/// </summary>
		/// <returns></returns>
		public override bool CustomActivate()
		{
			foreach (var item in SceneItems)
			{
				var isf = item as IShadesFeedback;
				if (isf != null)
				{
					isf.IsStoppedFeedback.OutputChange += new EventHandler<FeedbackEventArgs>(IsStoppedFeedback_OutputChange);
					isf.PositionFeedback.OutputChange += new EventHandler<FeedbackEventArgs>(PositionFeedback_OutputChange);
				}
			}

			this.AllAreAtSceneFeedback = new BoolFeedback(GetAllAreAtScene);
			this.IsStoppedFeedback = new BoolFeedback(GetAllAreStopped);

			return base.CustomActivate();
		}

		/// <summary>
		/// Event handler for each shade position change
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void PositionFeedback_OutputChange(object sender, FeedbackEventArgs e)
		{
			var a = this.GetAllAreAtScene();
			if (a != this._AllAreAtScene)
			{
				this._AllAreAtScene = a;
				this.AllAreAtSceneFeedback.FireUpdate();
			}
		}

		/// <summary>
		/// Event handler for each shade stop change
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void IsStoppedFeedback_OutputChange(object sender, FeedbackEventArgs e)
		{
			var a = this.GetAllAreStopped();
			if (a != this._IsStopped)
			{
				this._IsStopped = a;
				this.IsStoppedFeedback.FireUpdate();
			}
		}

		/// <summary>
		/// Helper to get if all shades are stopped
		/// </summary>
		/// <returns></returns>
		bool GetAllAreStopped()
		{
			foreach (var i in SceneItems)
			{
				var d = DeviceManager.GetDeviceForKey(i.DeviceKey);
				if (d == null) { continue; }
				var dfb = d as IShadesFeedback;
				if (dfb != null)
				{
					if (!dfb.IsStoppedFeedback.BoolValue)
					{
						return false; // if any are moving, break and return false
					}
				}
			}
			return true;
		}

		/// <summary>
		/// Func for at scene feedback
		/// </summary>
		/// <returns></returns>
		bool GetAllAreAtScene()
		{
			if (!this.GetAllAreStopped()) { return false; } // don't look at position if this is in motion
			foreach (var i in SceneItems)
			{
				var d = DeviceManager.GetDeviceForKey(i.DeviceKey);
				if(d == null) { continue; }
				var dfb = d as IShadesFeedback;
				if (dfb != null && dfb.PositionFeedback.IntValue != i.Level)
				{
					return false; // if any are not at position, break and return false
				}
			}
			return true;
		}

		/// <summary>
		/// 
		/// </summary>
		public void Run()
		{
			foreach (var si in SceneItems)
			{
				var device = DeviceManager.GetDeviceForKey(si.DeviceKey);
				if (device is IShadesPosition)
				{
					(device as IShadesPosition).SetPosition(si.Level);
				}
				// other load types...
			}
		}
	}
}