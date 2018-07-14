using UnityEngine;
using System.Collections;
using Ardunity;

// Types of data for transferring
using INT8 = System.SByte;
using UINT8 = System.Byte;
using INT16 = System.Int16;
using UINT16 = System.UInt16;
using INT32 = System.Int32;
using UINT32 = System.UInt32;
using FLOAT32 = System.Single;
using STRING = System.String;


public class MyController : ArdunityController
{
	// For displaying in Inspector
	public int txUINT8;
	public int txINT8;
	public int txUINT16;
	public int txINT16;
	public long txUINT32;
	public int txINT32;
	public float txFLOAT32;
	public string txSTRING = "";

	public int rxUINT8;
	public int rxINT8;
	public int rxUINT16;
	public int rxINT16;
	public long rxUINT32;
	public int rxINT32;
	public float rxFLOAT32;
	public string rxSTRING = "";

	// For transferring with Arduino
	private UINT8 _txUINT8;
	private INT8 _txINT8;
	private UINT16 _txUINT16;
	private INT16 _txINT16;
	private UINT32 _txUINT32;
	private INT32 _txINT32;
	private FLOAT32 _txFLOAT32;
	private STRING _txSTRING = "";

	private UINT8 _rxUINT8;
	private INT8 _rxINT8;
	private UINT16 _rxUINT16;
	private INT16 _rxINT16;
	private UINT32 _rxUINT32;
	private INT32 _rxINT32;
	private FLOAT32 _rxFLOAT32;
	private STRING _rxSTRING = "";

	// If you will use Awake of MonoBehaviour, must override Awake.
	protected override void Awake()
	{
		base.Awake();

		// If set ture then receive data automatically
		// If set false then don't receive data
		enableUpdate = true;
	}

	void Start()
	{

	}

	void Update()
	{
		if(connected)
		{
			// When connected to Arduino
			if(_txUINT8 != (UINT8)txUINT8)
			{
				_txUINT8 = (UINT8)txUINT8;
				SetDirty(); // It must call to run OnPush
			}
			if(_txINT8 != (INT8)txINT8)
			{
				_txINT8 = (INT8)txINT8;
				SetDirty(); // It must call to run OnPush
			}
			if(_txUINT16 != (UINT16)txUINT16)
			{
				_txUINT16 = (UINT16)txUINT16;
				SetDirty(); // It must call to run OnPush
			}
			if(_txINT16 != (INT16)txINT16)
			{
				_txINT16 = (INT16)txINT16;
				SetDirty(); // It must call to run OnPush
			}
			if(_txUINT32 != (UINT32)txUINT32)
			{
				_txUINT32 = (UINT32)txUINT32;
				SetDirty(); // It must call to run OnPush
			}
			if(_txINT32 != (INT32)txINT32)
			{
				_txINT32 = (INT32)txINT32;
				SetDirty(); // It must call to run OnPush
			}
			if(_txFLOAT32 != (FLOAT32)txFLOAT32)
			{
				_txFLOAT32 = (FLOAT32)txFLOAT32;
				SetDirty(); // It must call to run OnPush
			}
			if(txSTRING.Equals(_txSTRING) == false)
			{
				_txSTRING = (STRING)txSTRING;
				SetDirty(); // It must call to run OnPush
			}

			rxUINT8 = _rxUINT8;
			rxINT8 = _rxINT8;
			rxUINT16 = _rxUINT16;
			rxINT16 = _rxINT16;
			rxUINT32 = _rxUINT32;
			rxINT32 = _rxINT32;
			rxFLOAT32 = _rxFLOAT32;
			rxSTRING = _rxSTRING;
		}
		else
		{
			// When disconnected to Arduino			
		}
	}

	protected override void OnPush()
	{
		// When you send data to Arduino
		// 'Push' means that Unity throw data to Arduino
		// The order is important (It must be same with pop order in Arduino)
		Push(_txUINT8);
		Push(_txINT8);
		Push(_txUINT16);
		Push(_txINT16);
		Push(_txUINT32);
		Push(_txINT32);
		Push(_txFLOAT32);
		Push(_txSTRING);
	}

	protected override void OnPop()
	{
		// When you receive data from Arduino
		// 'Pop' means that Unity get data from Arduino
		// The order is important (It must be same with push order in Arduino)
		Pop(ref _rxUINT8);
		Pop(ref _rxINT8);
		Pop(ref _rxUINT16);
		Pop(ref _rxINT16);
		Pop(ref _rxUINT32);
		Pop(ref _rxINT32);
		Pop(ref _rxFLOAT32);
		Pop(ref _rxSTRING);
	}

	protected override void OnExecuted()
	{
		// Do something after receiving data
	}

	protected override void OnConnected()
	{
		// When connected to Arduino
	}
		
	protected override void OnDisconnected()
	{
		// When disconnected to Arduino
	}

	protected override void OnReset()
	{
		// When Arduino is reset
	}

	public override string[] GetAdditionalFiles()
	{
		// To export Arduino sketch
		// If some files are required, you can copy to sketch folder
		// This file must exist in folder name of 'Arduino'
		return null;
	}

	public override string[] GetCodeIncludes()
	{
		// To export Arduino sketch
		// You can describe "#include" in Arduino sketch
		return null;
	}

	public override string[] GetCodeDefines()
	{
		// To export Arduino sketch
		// You can describe "#define" in Arduino sketch
		return null;
	}

	public override string GetCodeDeclaration()
	{
		// To export Arduino sketch
		// You can describe declaration in Arduino sketch
		return string.Format("{0} {1}({2:d});", this.GetType().Name, GetCodeVariable(), id);
	}

	public override string GetCodeVariable()
	{
		// To export Arduino sketch
		// You can describe name of declaration in Arduino sketch
		return string.Format("myController{0:d}", id);
	}
}
