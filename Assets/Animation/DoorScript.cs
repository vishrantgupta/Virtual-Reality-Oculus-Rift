using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();
	}

    public AudioClip doorOpening;

    public AudioSource audioSource;

	Animator anim;

	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider other) {
        audioSource.PlayOneShot(doorOpening);
        anim.SetTrigger ("OpenDoor");
	}

	void OnTriggerExit(Collider other) {
		anim.enabled = true;
		anim.SetTrigger ("DoNothing");

        // audioSource.PlayOneShot(doorOpening);
    }

    void pauseAnimationEvent() {
		anim.enabled = false;
	}
}
