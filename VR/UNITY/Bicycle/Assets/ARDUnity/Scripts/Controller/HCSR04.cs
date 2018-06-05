using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UINT16 = System.UInt16;


namespace Ardunity
{
	[AddComponentMenu("ARDUnity/Controller/Sensor/HCSR04")]
	[HelpURL("https://sites.google.com/site/ardunitydoc/references/controller/hcsr04")]
	public class HCSR04 : ArdunityController, IWireInput<float>
	{
		public int trig;
		public int echo;

		public FloatEvent OnDistanceChanged;

		private UINT16 _distance = 0;

		protected override void OnExecuted()
		{
			if(OnWireInputChanged != null)
				OnWireInputChanged(distance);

			OnDistanceChanged.Invoke(distance);
		}

		protected override void OnPop()
		{
			UINT16 newDistance = _distance;
			Pop(ref newDistance);
			if(newDistance != _distance)
			{
				_distance = newDistance;
				updated = true;
			}
		}

		public override string GetCodeDeclaration()
		{
			return string.Format("{0} {1}({2:d}, {3:d}, {4:d});", this.GetType().Name, GetCodeVariable(), id, trig, echo);
		}

		public override string GetCodeVariable()
		{
			return string.Format("hcsr04_{0:d}", id);
		}

		public float distance
		{
			get
			{
				// Unit: cm
				if(_distance == 0)
					return 80f;
				else
					return (float)_distance * 0.1f; 
			}
		}

		#region Wire Editor
		public event WireEventHandler<float> OnWireInputChanged;

		float IWireInput<float>.input
		{
			get
			{
				return distance;
			}
		}

		protected override void AddNode(List<Node> nodes)
		{
			base.AddNode(nodes);

			nodes.Add(new Node("trig", "", null, NodeType.None, "Trig Pin"));
			nodes.Add(new Node("echo", "", null, NodeType.None, "Echo Pin"));
			nodes.Add(new Node("distance", "Distance", typeof(IWireInput<float>), NodeType.WireTo, "Input<bool>"));
		}

		protected override void UpdateNode(Node node)
		{
			if(node.name.Equals("trig"))
			{
				node.updated = true;
				node.text = string.Format("Trig: {0:d}", trig);
				return;
			}
			else if(node.name.Equals("echo"))
			{
				node.updated = true;
				node.text = string.Format("Echo: {0:d}", echo);
				return;
			}
			else if(node.name.Equals("distance"))
			{
				node.updated = true;
				return;
			}

			base.UpdateNode(node);
		}
		#endregion
	}
}
