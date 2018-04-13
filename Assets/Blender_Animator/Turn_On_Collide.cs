using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turn_On_Collide : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

    public AudioClip greeting;

    public AudioSource audioSource;

    void OnCollisionEnter(Collision collidedObject)
    {
        Debug.Log("Collided");
        // this.transform.rotation = Quaternion.Euler(0, this.transform.rotation.y * -1F, 0);
        // audioSource.PlayOneShot(greeting);

    }
	
	bool flag = false;
	
	// Update is called once per frame
	void Update () {

        // this.transform.position.y = 0.566F;


        if (this.transform.position.x > -4F) {
			// Debug.Log("Rotating");
			// transform.Rotate(0, Time.deltaTime, 0, Space.World);
			
			// flag = true;
			
			this.transform.rotation = Quaternion.Euler(0, -90, 0);
			
			// this.transform.rotation = 90;
		}
			
		if(this.transform.position.x < -45F) {
			// Debug.Log("Change");
			// transform.Rotate(0, Time.deltaTime, 0, Space.World);
			
			
			// flag = !flag;
			
			this.transform.rotation = Quaternion.Euler(0, 90, 0);
			
			// this.transform.rotation = 90;
		}
		
	}
}
