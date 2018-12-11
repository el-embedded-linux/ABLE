using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGM : MonoBehaviour {
	public AudioClip BGMS;
	private AudioSource _audio;

	// Use this for initialization
	void Start () {
		_audio = GetComponent<AudioSource>();
		_audio.PlayOneShot(BGMS);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
