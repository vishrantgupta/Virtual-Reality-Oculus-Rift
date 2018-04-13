
using UnityEngine;
using System.Collections;
namespace Crowd {


	public class Psychology
	{
		//------------------------------------------------------------------------------------------------------------------------------
		public Vector3 personality;
		public Vector3 mood;
		public Vector3 emotion;
		//------------------------------------------------------------------------------------------------------------------------------	
		public Psychology ()
		{	//personality=new Vector3(Random.value*Random.Range(-1,1),Random.value*Random.Range(-1,1),Random.value*Random.Range(-1,1));
			 //mood=new Vector3(Random.value*Random.Range(-1,1),Random.value*Random.Range(-1,1),Random.value*Random.Range(-1,1));
			//emotion=new Vector3(Random.value*Random.Range(-1,1),Random.value*Random.Range(-1,1),Random.value*Random.Range(-1,1));
			//--//		 
			float x=0,y=0,z = 0;				 
			personality=new Vector3(x,y,z);//E genetico, o carregamento é feito no construtor do BehaviorAI
			//--//
			init_Mood_Emotion();
		}
		//------------------------------------------------------------------------------------------------------------------------
		public void init_Mood_Emotion()
		{
			float x=0,y=0,z = 0;	
	//		if (Random.Range (-1, 1) == 0)	
			x=Random.value;
	//	else x=Random.value*-1;
	//	if (Random.Range (-1, 1) == 0)	
			y=Random.value;
	//	else y=Random.value*-1;
	//	if (Random.Range (-1, 1) == 0)	
			z=Random.value;
	//	else z=Random.value*-1;		 
		mood=new Vector3(x,y,z);
		//--//
	//	if (Random.Range (-1, 1) == 0)	
			x=Random.value;
	//	else x=Random.value*-1;
	//	if (Random.Range (-1, 1) == 0)	
			y=Random.value;
	//	else y=Random.value*-1;
	//	if (Random.Range (-1, 1) == 0)	
			z=Random.value;
	//	else z=Random.value*-1;		 
		emotion=new Vector3(x,y,z);
		}
		//------------------------------------------------------------------------------------------------------------------------
		public void die()
		{
		}

	}//eo class
}//eo namespace

