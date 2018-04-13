using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
public class Teste : StateMachineBehaviour
{

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("Vem aqui");
    }

}
*/
 public class TestaRotacaoPersonagem : MonoBehaviour
{
    //Animator anim;
    //int rotateLeftAnimation;
    //int rotateRightAnimation;

    //private void Start()
    //{
    //  anim= gameObject.GetComponent<Animator>();
     
    //  rotateLeftAnimation = Animator.StringToHash("Base Layer.turning-left");
    //  rotateRightAnimation = Animator.StringToHash("Base Layer.turning-right");
    //}

    //bool isRotating = false;
    //public void OnAnimatorMove()
    //{   
    //   transform.rotation = anim.targetRotation;
    //   transform.position = anim.targetPosition;  
    //}
   
    //public void Update()
    //{      
    //    Vector3 target = Camera.main.gameObject.transform.forward;

    //    //INDICA ORIENTACAO        
    //    float dotOrientacoes;
    //    dotOrientacoes = Vector3.Dot(transform.forward,
    //                                target);
    //    Debug.DrawRay(Camera.main.gameObject.transform.position + Vector3.up, 
    //                  target*10f, Color.black);
    //    Debug.DrawRay(transform.position + Vector3.up,
    //                 transform.forward * 5f, Color.red);
       
    //    //------------------------------------------------------------------------------------------
    //    //**** 
    //    if (dotOrientacoes > 0f)//Ambas apontam no mesmo sentido => ORIENTACAO INVALIDA
    //    {
    //        anim.speed = 3;
    //        Debug.DrawRay(transform.position + Vector3.up, Vector3.up * 5, Color.blue);
    //    }
    //    else if (dotOrientacoes > -0.925f) //De 90 graus ao mesmo sentido
    //    {
    //        anim.speed = 2;
    //        Debug.DrawRay(transform.position + Vector3.up, Vector3.up * 5, Color.magenta);
    //    }      
    //    else //Apontam em sentidos contrarios => ORIENTACAO CERTA
    //    {
    //        anim.speed = 1;
    //        Debug.DrawRay(transform.position + Vector3.up, transform.up * 5, Color.green);
    //    }
         
    //    if (Input.GetKey("up"))
    //        print("up arrow key is held down");

    //    if (Input.GetKey("down"))
    //        print("down arrow key is held down");

    //    if (Input.GetKey("left"))
    //    {
    //        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("turning-left") ||
    //           (anim.GetCurrentAnimatorStateInfo(0).IsName("turning-left")
    //         && anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1))
    //                    anim.Play(rotateLeftAnimation, 0, 0);
                
    //    }

    //    if (Input.GetKey("right"))
    //    {
    //        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("turning-right")  ||
    //            (anim.GetCurrentAnimatorStateInfo(0).IsName("turning-right") 
    //          && anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 )      )
    //                  anim.Play(rotateRightAnimation, 0, 0);
                
    //    }
    //}


}