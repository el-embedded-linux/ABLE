using UnityEngine;
using System.Collections.Generic;

using UINT16 = System.UInt16;


namespace Ardunity
{	
	[AddComponentMenu("ARDUnity/Controller/Sensor/CapSense")]
	[HelpURL("https://sites.google.com/site/ardunitydoc/references/controller/capsense")]
	public class CapSense : ArdunityController, IWireInput<bool>
	{
		public int send;
		public int receive;
		public int threshold;

		public BoolEvent OnValueChanged;

		private UINT16 _value = 0;


		protected override void OnPush()
		{
		}

		protected override void OnExecuted()
		{
			if(OnWireInputChanged != null)
				OnWireInputChanged(Value);

			OnValueChanged.Invoke(Value);
		}

		protected override void OnPop()
		{
			UINT16 newValue = _value;
			Pop(ref newValue);
			if(newValue != _value)
			{
				bool curState = Value;
				_value = newValue;
				bool newState = Value;

				if(curState != newState)
					updated = true;
			}
		}

		public override string GetCodeDeclaration()
		{
			return string.Format("{0} {1}({2:d}, {3:d}, {4:d});", this.GetType().Name, GetCodeVariable(), id, send, receive);
		}

		public override string GetCodeVariable()
		{
			return string.Format("capSense{0:d}", id);
		}

		public int RawValue
		{
			get
			{
				return (int)_value;
			}
		}

		public bool Value
		{
			get
			{
				if(connected)
				{
					if(_value > threshold)
						return true;
					else
						return false;
				}
				else
					return false;
			}
		}

		#region Wire Editor
		public event WireEventHandler<bool> OnWireInputChanged;

		bool IWireInput<bool>.input
		{
			get
			{
				return Value;
			}
		}

		protected override void AddNode(List<Node> nodes)
		{
			base.AddNode(nodes);

			nodes.Add(new Node("send", "", null, NodeType.None, "Arduino Send Pin"));
			nodes.Add(new Node("receive", "", null, NodeType.None, "Arduino Receive Pin"));
			nodes.Add(new Node("Value", "Value", typeof(IWireInput<bool>), NodeType.WireTo, "Input<bool>"));
		}

		protected override void UpdateNode(Node node)
		{
			if(node.name.Equals("send"))
			{
				node.updated = true;
				node.text = string.Format("Send: {0:d}", send);
				return;
			}
			else if(node.name.Equals("receive"))
			{
				node.updated = true;
				node.text = string.Format("Receive: {0:d}", receive);
				return;
			}
			else if(node.name.Equals("Value"))
			{
				node.updated = true;
				return;
			}

			base.UpdateNode(node);
		}
		#endregion
	}
}
