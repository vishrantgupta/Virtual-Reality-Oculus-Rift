
//#define visualizeStates

// --------------------------------------------------------------------------------
// LookAt.cs-----------------------------------------------------------------------
// --------------------------------------------------------------------------------

using UnityEngine;
using System.Collections;

namespace Crowd
{
    [RequireComponent (typeof (Animator))]
    public class LookAt : MonoBehaviour {
        public Transform head;
        //public Vector3 lookAtTargetPosition;
        //public float lookAtCoolTime = 0.2f;
        //public float lookAtHeatTime = 0.2f;
      //  public bool looking = true;
        //public Vector3 lookAtPosition;
        private Animator anim;
        GajoCitizen gajo;
      //  private float lookAtWeight = 0.0f;
        int layerIndex;

        float speed;
        float step;
        float angulo;
        Vector3 targetDir;
        //int rotateLeftAnimation;
        //int rotateRightAnimation;
    
        /*--------------------------------------------------*/
        void Start()
        {
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionY;

            gajo = GetComponent<GajoCitizen>();
            anim = GetComponent<Animator>();
            //layerIndex = anim.GetLayerIndex("Base Layer");
            //rotateLeftAnimation = Animator.StringToHash("Base Layer.turning-left");
            //rotateRightAnimation = Animator.StringToHash("Base Layer.turning-right");

            /* ANIMACAO DA CABECA 
             * if (!head)
                     head = findBone(anim.transform, "mixamorig:Head");
                 if (!head)
                     head = findBone(anim.transform, "Head");
                 if (!head)
                     head = findBone(anim.transform, "head");
                 if (!head)
                     head = findBone(anim.transform, "Neck");
                 if (!head)
                     head = findBone(anim.transform, "neck");
                 if (!head)
                     head = anim.transform.Find("mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:Neck/mixamorig:Head");

                 if (!head)
                 {
                     Debug.LogWarning(gameObject.name + " No head transform - LookAt disabled");
                     enabled = false;
                     return;
                 }
                 else
                 {                
                     lookAtTargetPosition = head.position + transform.forward;
                     lookAtPosition = lookAtTargetPosition;
                 }
                 */
        }
        /*--------------------------------------------------*/
        //Transform findBone(Transform parent , string name)  {
        //    foreach (Transform child in parent)
        //    {               
        //        if (child.name == name)
        //        {                 
        //            return child;
        //        }
        //        else
        //            findBone(child, name);
        //    }           
        //    return null;
        //}
        /*--------------------------------------------------*/
        /* ANIMACAO DA CABECA 
         * void OnAnimatorIK()
                {
                    //Move head para lookAtTarget
                    if(head)
                        if (gajo!=null)
                     if (gajo.behavior!=null)
                      if (gajo.behavior.Interacao!=null)
                        if (gajo.behavior.Interacao.interactionPartner != null)
                        {
                            if (Mathf.Abs(gajo.behavior.Interacao.interactionPartner.transform.position.y - transform.position.y) > 0.25f)
                            {
                                lookAtTargetPosition.y = head.position.y;
                                float lookAtTargetWeight = looking ? 1.0f : 0.0f;

                                Vector3 curDir = lookAtPosition - head.position;
                                Vector3 futDir = lookAtTargetPosition - head.position;

                                curDir = Vector3.RotateTowards(curDir, futDir, 6.28f * Time.deltaTime, float.PositiveInfinity);
                                lookAtPosition = head.position + curDir;

                                float blendTime = lookAtTargetWeight > lookAtWeight ? lookAtHeatTime : lookAtCoolTime;
                                lookAtWeight = Mathf.MoveTowards(lookAtWeight, lookAtTargetWeight, Time.deltaTime / blendTime);
                                anim.SetLookAtWeight(lookAtWeight, 0.2f, 0.5f, 0.7f, 0.5f);
                                anim.SetLookAtPosition(lookAtPosition);
                            }
                        }
                }
         */


        //-----------------------------------
        //Rotate towards the partner   
        //Called in GajoCitizen::Update
        bool isRotating = false;
        float dotOrientacoes;

        public void rotateBodyToPartner()   {
            if (gajo.behavior.Interacao.interactingPartner != null && gajo.behavior.Interacao.isInteracting)
            {
                //------------------------------------------------------------------------------------------
                //Indica direcao frente
#if visualizeStates
                Debug.DrawRay(transform.position + Vector3.up, transform.forward * 2, Color.red);
#endif
                determinesOrientation();
                if (isRotating)
                    { 
                    Vector3 relativePos = gajo.behavior.Interacao.interactingPartner.transform.position -
                                              gajo.transform.position;
                    //*** ***
                    //ESQUERDA OU DIREITA
                    gajo.walker.Navigation.makeOneRotationStep(relativePos);
                    }//isRotating 
                else
                    anim.speed = 1;
                snapToInteractor(dotOrientacoes);
            }
        }//rotateBodyToPartner
      

        void determinesOrientation()
        {
            //------------------------------------------------
            //PASSO 1 - Determina as Orientações dos dois (Se estao a olha um para o outro, ou necessitam rodar)
            //DOT - (1) Paralelas,  ambas apontam na mesma direcao 
            //DOT - (0) se forem perpendiculares
            //DOT - (-1) Paralelas, mas de sentidos contrarios          
            dotOrientacoes = Vector3.Dot(transform.forward,
                                         transform.position - gajo.behavior.Interacao.interactingPartner.transform.position);
            //-----------------------------------------------------------------------------------------
            //Excepção: Se estao a apontar em direcoes opostas (como se pretende)
            //mas estao um nas costas do outro
            if (dotOrientacoes < -0.10f &&
                          (Vector3.Distance(transform.position + transform.forward * 0.01f,
                                           (gajo.behavior.Interacao.interactingPartner.transform.position +
                                            gajo.behavior.Interacao.interactingPartner.transform.forward * 0.01f)) >
                           Vector3.Distance(transform.position,
                                            gajo.behavior.Interacao.interactingPartner.transform.position)))
                dotOrientacoes = 999f;//Damos valor de errado que e para ter que rodar

            //----------------------------------------------------------------    
            //Indica interacao partner com linha sobre as cabeças
#if visualizeStates
                Debug.DrawRay(transform.position + Vector3.up * 3,
                   gajo.behavior.Interacao.interactingPartner.transform.position - transform.position,
                   Color.grey);
#endif
            //PASSO 2 - Roda para a direita ou esquerda, consoante seja o caso - e se as orientacoes nao estiverem viradas uma para a outra
            if (dotOrientacoes > 0f && !anim.GetBool("frente_a_frente"))//Paralelas. Ambas apontam no mesmo sentido => ORIENTACAO INVALIDA
            {
                anim.speed = 2.5f;
                isRotating = true;
                anim.SetBool("frente_a_frente", false);
#if visualizeStates
                    Debug.DrawRay(transform.position + Vector3.up, Vector3.up * 2, Color.blue);
#endif
            }
            else if (dotOrientacoes > -0.925f && !anim.GetBool("frente_a_frente")) //De 90 graus ao mesmo sentido
            {
                anim.speed = 1.25f;
                isRotating = true;
                anim.SetBool("frente_a_frente", false);
#if visualizeStates
                    Debug.DrawRay(transform.position + Vector3.up, Vector3.up * 2, Color.cyan);
#endif
            }
            //else if (dotOrientacoes > -0.525f) //De 90 graus ao mesmo sentido
            //Comentei em accord com o SMOOTH ROTATION de abaixo
            else if (dotOrientacoes > -1.001f && !anim.GetBool("frente_a_frente")) //De 90 graus ao mesmo sentido
            {
                anim.speed = 1.05f;
                isRotating = true;
                anim.SetBool("frente_a_frente", false);
#if visualizeStates
                    Debug.DrawRay(transform.position + Vector3.up, Vector3.up * 2, Color.magenta);
#endif
            }
            else //Apontam em sentidos contrarios => ORIENTACAO CERTA
            {
                anim.speed = 1;
                isRotating = false;
                anim.SetBool("frente_a_frente", true);
#if visualizesStates
                    Debug.DrawRay(transform.position + Vector3.up, transform.up * 2, Color.green);
#endif
            }
        }//determinesOrientations

        void snapToInteractor(float dotOrientacoes)
        {
            /*******/
            //SMOOTH ADJUSTMENT DA ROTATION -> ESTA EM ROTATE ANIMATED = FALSE
            //Roda para o partner da interacao para fazer um smooth slow down 
            //(para tentar que nao ande a rodar esquerda, direita, esquerda, direita...)
            //Atencao que isto interfere com a animação
            if (anim.GetBool("frente_a_frente"))
            {
                targetDir = gajo.behavior.Interacao.interactingPartner.transform.position - transform.position;
                targetDir.y = 0;
                if (targetDir != Vector3.zero)
                {
                    //SNAP to Partner if close to Paralell orientation 
                    //if (dotOrientacoes < -0.5f || dotOrientacoes > 0.5f)//Paralelas 
                    if (dotOrientacoes < -0.25f || dotOrientacoes > 0.25f)//Paralelas 
                    {
                        transform.LookAt(gajo.behavior.Interacao.interactingPartner.transform.position);
//Debug.DrawLine(transform.position, transform.position + Vector3.up * 3, Color.blue);
                    }
                    else
                    {
                        speed = 0.5f;
//Debug.DrawLine(transform.position, transform.position + Vector3.up * 3, Color.green);
                        Quaternion rotation = Quaternion.LookRotation(targetDir);
                        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, speed * Time.deltaTime);
                    }
                }
            }
        }//snapInteractor



        public void OnAnimatorMove()
        {
            //update do transform de acordo com a nova posicao da animação 
            if (gajo != null)
                if (gajo.behavior != null)
                    if (gajo.behavior.Interacao != null)
                        if (gajo.behavior.Interacao.interactingPartner != null && gajo.behavior.Interacao.isInteracting)
                        {
                            if (isRotating)
                            {
                                transform.rotation = gajo.anim.targetRotation;
                                transform.position = gajo.anim.targetPosition;
                            }
                        }
        }//OnAnimator
        /*--------------------------------------------------*/
        //Hack para quando já deixou de haver interacao e este ainda ainda vai acenar
        /*       private void lookAtClosestGuy()
               {
                   float d1 = -1f, d2 = -1f;
                   if (gajo.behavior.closestMate != null)
                       d1 = Vector3.Distance(gajo.gameObject.transform.position, gajo.behavior.closestMate.gameObject.transform.position);
                   if ((gajo.behavior.closestPrey != null))
                       d2 = Vector3.Distance(gajo.gameObject.transform.position, gajo.behavior.closestPrey.gameObject.transform.position);
                   if (d1 != -1 && d2 != -1)
                   {
                       if (d1 < d2)
                       {
                           gajo.transform.LookAt(gajo.behavior.closestMate.gameObject.transform);
                           lookAtTargetPosition = gajo.behavior.closestMate.gameObject.transform.position;
                           lookAtTargetPosition.y += Vector3.up.y + 2;
       #if Visualize
       Debug.DrawLine(transform.position + Vector3.up, gajo.behavior.closestMate.gameObject.transform.position + Vector3.up, Color.green);
       #endif
                       }
                       else
                       {
                           gajo.transform.LookAt(gajo.behavior.closestPrey.gameObject.transform);
                           lookAtTargetPosition = gajo.behavior.closestPrey.gameObject.transform.position;
                           lookAtTargetPosition.y += Vector3.up.y + 2;
       #if Visualize
       Debug.DrawLine(transform.position + Vector3.up, gajo.behavior.closestPrey.gameObject.transform.position + Vector3.up, Color.green);
       #endif
                       }
                   }
                   else
                     if (d1 != -1)
                       {
                           gajo.transform.LookAt(gajo.behavior.closestMate.gameObject.transform);
                           lookAtTargetPosition = gajo.behavior.closestMate.gameObject.transform.position;
                           lookAtTargetPosition.y += Vector3.up.y + 2;
       #if Visualize
       Debug.DrawLine(transform.position + Vector3.up, gajo.behavior.closestMate.gameObject.transform.position + Vector3.up, Color.green);
       #endif
                       }
                   else
                     if (d2 != -1)
                       {
                           gajo.transform.LookAt(gajo.behavior.closestPrey.gameObject.transform);
                           lookAtTargetPosition = gajo.behavior.closestPrey.gameObject.transform.position;
                           lookAtTargetPosition.y += Vector3.up.y + 2;
       #if Visualize
       Debug.DrawLine(transform.position + Vector3.up, gajo.behavior.closestPrey.gameObject.transform.position + Vector3.up, Color.green);
       #endif
                       }

                   //      int walkBackAnimation = Animator.StringToHash("Base Layer.roda_esquerda_56_01");
                   //      anim.CrossFade(walkBackAnimation, 0.10f, anim.GetLayerIndex("Base Layer"));
                   //           animaRotacao(lookAtTargetPosition);
               }

               /*--------------------------------------------------*/
        /*        public void lookAtTarget()
                {
                 lookAtTargetPosition = gajo.walker.Navigation.agent.steeringTarget + transform.forward;
        Debug.DrawRay(transform.position + Vector3.up, lookAtTargetPosition-transform.position + Vector3.up, Color.green);
                }
        */
    }
}


/*
 *  public float maxAngle = 95; 
 void OnCollisionEnter(Collision collision){
     Vector3 normal = collision.contacts[0].normal;
     Vector3 vel = rigidbody.velocity;
     // measure angle
     if (Vector3.Angle(vel, -normal) > maxAngle){
         // bullet bounces off the surface
         rigidbody.velocity = Vector3.Reflect(vel, normal);
     } else {
         // bullet penetrates the target - apply damage...
         Destroy(gameObject); // and destroy the bullet
     }
  }

    http://www.theappguruz.com/blog/unity-3d-enemy-obstacle-awarness-ai-code-sample


       int angle = 1f;
                    transform.rotation *= Quaternion.AngleAxis(angle, Vector3.up);
 * */
