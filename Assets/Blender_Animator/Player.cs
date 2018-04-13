using System.Collections;
using System.Collections.Generic;
using UnityEngine;





public   class  Player :  MonoBehaviour  {

	public float speed = 1.0F;
	public float rotateSpeed = 1.0F; // rotate speed in degrees per second
	public float gravity   = 10.0F;

	private Vector3 moveDirection = Vector3.zero;
	private float curDir = 1;




	/*


	private  Animator anim;


	public   float  speed =  6.0f ;
	public   float  turnSpeed =  60.0f ;
	private   Vector3  moveDirection =  Vector3 .zero;
	public   float  gravity =  20.0f ;
	*/
	private  Animator anim;
	private   CharacterController  controller;
	private float timeToChangeDirection;


	void  Start () {
		controller = GetComponent < CharacterController >();
		anim = gameObject.GetComponentInChildren<Animator>();

		//ChangeDirection();
	}

	void  Update (){
        
        /*if  ( Input .GetKey ( "up" )) {
			anim.SetInteger ( "AnimationPar" ,  1 );
		}   else  {
			anim.SetInteger ( "AnimationPar" ,  0 );
		}

		if (controller.isGrounded){
			moveDirection = transform.forward *  Input .GetAxis( "Vertical" ) * speed;
		}

		float  turn =  Input .GetAxis( "Horizontal" );
		transform.Rotate( 0 , turn * turnSpeed *  Time .deltaTime,  0 );
		controller.Move(moveDirection *  Time .deltaTime);
		moveDirection.y -= gravity *  Time .deltaTime;*/



        if ( controller.isGrounded ) {
			if (Random.value < 0.05f){ // if random value < 5/100...
				curDir = -curDir; // flip direction
			}
			// Rotate around y - axis
			transform.Rotate(Vector3.up * curDir * rotateSpeed * Time.deltaTime);
			moveDirection = speed * transform.TransformDirection(Vector3.forward);
		}
		// moveDirection.y -= gravity * Time.deltaTime; // apply gravity

		if(transform.position.x == -8) {
			// Rotate around y - axis
			transform.Rotate(Vector3.up * curDir * rotateSpeed * Time.deltaTime);
		}

		controller.Move(moveDirection * Time.deltaTime);
	}


	/*private void ChangeDirection() {
		float angle = Random.Range(0f, 360f);
		Quaternion quat = Quaternion.AngleAxis(angle, Vector3.forward);
		Vector3 newUp = quat * Vector3.up;
		newUp.z = 0;
		newUp.Normalize();
		transform.up = newUp;
		timeToChangeDirection = 1.5f;
	}*/


}









