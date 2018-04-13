using UnityEngine;
using System.Collections;
using System;

public class Utils {

//--HELPFUL FUNCTIONS 
//-------------------------------------------------------------------------------------------	
//Returns decimal value from a binary string 
 public static int binaryValue(string s) 
 { 
  int len=0; 
  int final_value=0;
  if(s!=null) 
     { s=reverse(s); 
	   len=s.Length;	
       for(var i=0; i<len; i++) 
	      {
			if (s[i] =='1') 
			final_value+=(int)Math.Pow(2,i);
		  }
		return final_value;		 	
	 }
	 else return -1;
 }
    //-------------------------------------------------------------------------------------------	
    //Returns decimal value from array
    public static int convertFromBinaryValue(int[] s)
    {
        int final_value = 0;
        if (s != null)
        {   
            for (var i = s.Length-1; i >= 0; i--)
            {
                if (s[i] == 1)
                    final_value += (int)Math.Pow(2, i);
            }
            return final_value;
        }
        else return -1;
    }
    //-----------------------------------------------------------------------------------------	
    //Reverses string
    public static string reverse(string s1) 
		{string s2="";
		 for(var i=s1.Length-1; i>=0; i--) 
		     s2+=s1[i];
		 return s2;	 
		 }

//----------------------------------------------------------------------------------------------------
public static string replaceChar(string str, int id, string ch)
	{ 
	string aux="";
	if(id<str.Length)
		{	int i=0;	
			for(i=0; i<id; i++) 
				aux+=str[i];
			aux+=ch;   
			for(i=id+1; i<str.Length; i++)
				aux+=str[i];
		}
		return aux;  
}

    public static bool arrayCompare(int[] a, int[] b)
    {
     if (a.Length != b.Length) return false;
        for (int i = 0; i < a.Length; i++)
            if (a[i] != b[i])
                return false; 
        return true;
    }
    //TRANSFORM
    //       GameObject bucketOriginal = GameObject.Find("Bucket 1");
    //       if (bucketOriginal != null)
    //        { GameObject bucketCopia = (GameObject)Instantiate(bucketOriginal, prefab.transform.position, Quaternion.identity); 
    //        if (bucketCopia != null)
    //            bucketCopia.transform.parent = prefab.transform.Find("mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:LeftShoulder/mixamorig:LeftArm/mixamorig:LeftForeArm/mixamorig:LeftHand");
    //            fruitpicker.prop = bucketCopia;
    //        }


    //REPLACE MECHANIM
    //        AnimationClipPlayable clipPlayable;
    //--            clipPlayable = AnimationClipPlayable.Create(animationProduction); 
    //--            GetComponent<Animator>().Play(clipPlayable);

    /*--*/
    //ANIMATOR STUFF 
   // bool AnimatorIsPlaying()
    //{
   //     return GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length >
    //           GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime;
   // }

   // bool AnimatorIsPlaying(string stateName)
   // {
   //     return AnimatorIsPlaying() && GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName(stateName);
   // }

   // IEnumerator ShowCurrentClipLength()
   // {
   //     yield return new WaitForEndOfFrame();
   //     print("current clip length = " + anim.GetCurrentAnimatorStateInfo(0).length);
   // }

    //ANIMATOR USANDO O EDITOR 
   //UnityEditor.Animations.AnimatorStateMachine;
   //     UnityEditor.Animations.AnimatorController;
   //     UnityEditor.Animations.AnimatorStateTransition;
   //     Motion 
}//eo class