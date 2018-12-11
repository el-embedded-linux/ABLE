using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour {

	public AudioClip screemSound;
	private AudioSource _audio;

	// Use this for initialization
	void Start () {
		_audio = GetComponent<AudioSource>();

	}
	
	// Update is called once per frame
	void Update () {
		
	}
    private void OnTriggerEnter(Collider other)
    {
        SceneManager.LoadScene("07_die");
		_audio.PlayOneShot(screemSound);
    }
}
