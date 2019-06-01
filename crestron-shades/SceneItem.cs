using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace CrestronShades
{
	public class SceneItem
	{
		public int Delay { get; set; }
		public int Time { get; set; }
		public ushort Level { get; set; }
		public string DeviceKey { get; set; }
	}
}