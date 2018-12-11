using UnityEngine;
using System.Collections.Generic;

using UINT8 = System.Byte;


namespace Ardunity
{	
	[AddComponentMenu("ARDUnity/Controller/Sensor/DHT11")]
	[HelpURL("https://sites.google.com/site/ardunitydoc/references/controller/dht11")]
	public class DHT11 : ArdunityController
	{
		public int pin;
		
		private UINT8 _humidity = 0;
		private UINT8 _temperature = 0;
		
		
		protected override void OnExecuted()
		{
		}
		
		protected override void OnPop()
		{
			UINT8 newValue = _humidity;
			Pop(ref newValue);
			if(newValue != _humidity)
			{
				_humidity = newValue;
				updated = true;
			}

			newValue = _temperature;
			Pop(ref newValue);
			if(newValue != _temperature)
			{
				_temperature = newValue;
				updated = true;
			}
		}

		public override string GetCodeDeclaration()
		{
			return string.Format("{0} {1}({2:d}, {3:d});", this.GetType().Name, GetCodeVariable(), id, pin);
		}
		
		public override string GetCodeVariable()
		{
			return string.Format("dht11_{0:d}", id);
		}
		
		public int humidity
		{
			get
			{
				return (int)_humidity;
			}
		}

		public int temperature
		{
			get
			{
				return (int)_temperature;
			}
		}
		
        #region Wire Editor		
		protected override void AddNode(List<Node> nodes)
        {
			base.AddNode(nodes);
			
            nodes.Add(new Node("pin", "", null, NodeType.None, "Arduino Pin"));
        }
        
        protected override void UpdateNode(Node node)
        {
            if(node.name.Equals("pin"))
            {
				node.updated = true;
                node.text = string.Format("Pin: {0:d}", pin);
                return;
            }
            
            base.UpdateNode(node);
        }
        #endregion
	}
}
