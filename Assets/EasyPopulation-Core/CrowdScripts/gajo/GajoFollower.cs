//#define visualizeStatesCHILD
//#define visualizeStatesADULT
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Crowd
{
    public class GajoFollower : Gajo
    {
        public enum WalkingMode { CHILD, ADULT };
        WalkingMode walkingMode;//ver init
        Gajo _pai;
        public bool isReady = false;
        float mood = 0;
        Vector3 closestSide;
        float distanceToPai;

        Vector3 destination = Vector3.zero;
        Vector3 velocity = Vector3.zero;
        int layer;

        Vector3 orientation;
        float rotSpeed;
        float step;
        float runningDistance;
        float threeQuarterRunningDistance;
        float halfRunningDistance;
        //--------------
        //--------------
        //--------------
        public void init(Gajo pai, WalkingMode type)
        {
          
            walkingMode = type;
            initializeGajo();//super   
            _pai = pai;
         
            if (walker.Navigation.agent.isOnNavMesh)
                {
                    walker.target = pai.gameObject.transform.position;
                    walker.Navigation.startWalking();
                }
           
            if(walker.Animacao.gameObject.activeSelf)
                walker.Animacao.SetBool("walk", true); //mexe                 
           
            orientation = Vector3.zero;
            rotSpeed = 0.05f;
            step = rotSpeed * Time.deltaTime;
            runningDistance = walker.Navigation.agent.radius * 4;
            threeQuarterRunningDistance = walker.Navigation.agent.radius * 3;
            halfRunningDistance = walker.Navigation.agent.radius * 2;

           isReady = true;                        
        }


        //-----------------------------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------------------------
        bool podeAndar = false;

        void FixedUpdate()
        {
         podeAndar = true;

         //Vector3 forceFromNeighbours = Vector3.zero;
         //RaycastHit[] rays = Physics.SphereCastAll(gameObject.transform.position, walker.Navigation.agent.radius, walker.Navigation.agent.velocity, walker.Navigation.agent.radius * 2f);
         //foreach (RaycastHit r in rays)
         //    {
         //           if (gameObject.layer == r.collider.gameObject.layer &&
         //               gameObject.name != r.collider.gameObject.name)
         //               if (podeAndar)
         //               {
         //                   podeAndar = false;
         //                   Vector3 toOther = gameObject.transform.position - r.collider.gameObject.transform.position;
         //                   forceFromNeighbours += toOther;

         //                   toOther.Normalize();
         //                   float dot = Vector3.Dot(transform.forward, toOther);
         //                   float theta = Mathf.Acos(dot) * Mathf.Rad2Deg;
         //                   float multiplier = walker.Navigation.agent.radius * 2;
         //                   if (theta < 45)
         //                   {
         //                       multiplier = 2 * walker.Navigation.agent.radius;
         //                   }
         //                   forceFromNeighbours += (gameObject.transform.position - r.point);
         //                   forceFromNeighbours += (gameObject.transform.position - r.collider.gameObject.transform.position);
         //                   forceFromNeighbours += r.collider.gameObject.transform.forward * multiplier;
         //               }
         //       }
         //       if (!podeAndar)
         //       {
         //           Vector3 velocityFinal = forceFromNeighbours + walker.Navigation.agent.velocity.normalized;
         //           //--
         //           velocityFinal = velocityFinal.normalized * walker.Navigation.agent.radius * 2;
         //           if (Random.Range(0, 100) > 10)
         //               velocityFinal = Quaternion.AngleAxis(15, Vector3.up) * velocityFinal;
         //           else
         //               velocityFinal = Quaternion.AngleAxis(-15, Vector3.up) * velocityFinal;
         //           //--
         //           Vector3 velocityFinalDelted = velocityFinal * (0.9f * Time.deltaTime);

         //           walker.Navigation.agent.nextPosition = transform.position + velocityFinalDelted;
         //           walker.Navigation.agent.velocity = velocityFinalDelted;
         //       //--------------------------------------------------------------------------------------
         //       //Quaternion rotation = Quaternion.LookRotation(velocityFinal);
         //       //transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, 0.5f * Time.deltaTime);
         //       //--------------------------------------------------------------------------------------
         //       int layerIndex = walker.Animacao.GetLayerIndex("Base Layer");
         //       int rotateLeftAnimation = Animator.StringToHash("Base Layer.turning-left");
         //       int rotateRightAnimation = Animator.StringToHash("Base Layer.turning-right");

         //       //*** ***
         //       //ESQUERDA OU DIREITA
         //       Vector3 relativePos = -1 * velocityFinalDelted;//_parent.Owner.behavior.Interacao.interactingPartner.transform.position -
         //                                                //_parent.Own.transform.position;
         //                                                //CROSS - Da um novo vector perpendicular ao plano formado pelos dois vectores iniciais 
         //                                                //CROSS - se o y do cross, for UP, entao o ojecto esta onRIGHT 
         //                                                //CROSS - se o y do cross, for DOWN, entao o objecto esta onLEFT                   
         //       if (Vector3.Cross(transform.forward, relativePos).y > 0)
         //       {///Is on the right///    
         //           if (!walker.Animacao.GetCurrentAnimatorStateInfo(0).IsName("turning-right") ||
         //               (walker.Animacao.GetCurrentAnimatorStateInfo(0).IsName("turning-right")
         //             && walker.Animacao.GetCurrentAnimatorStateInfo(0).normalizedTime > 1))
         //               // anim.Play(rotateRightAnimation, 0, 0);                                 
         //               walker.Animacao.SetTrigger("step_right");
         //       }
         //       else
         //       {///Is on the left///                     
         //           if (!walker.Animacao.GetCurrentAnimatorStateInfo(0).IsName("turning-left") ||
         //               (walker.Animacao.GetCurrentAnimatorStateInfo(0).IsName("turning-left")
         //             && walker.Animacao.GetCurrentAnimatorStateInfo(0).normalizedTime > 1))
         //               //            anim.Play(rotateLeftAnimation, 0, 0);
         //               walker.Animacao.SetTrigger("step_left");
         //       }
         //    //-----------------------------------------------------------------
         //}                
        }//Fixed Update
        //-----------------------------------------------------------------------------------------------------
        public void Update()
        {         
            if (isReady && podeAndar)
            {
                step = rotSpeed * Time.deltaTime;

                switch (walkingMode)
                {
                    //FOLLOW ESTILO CRIANCA//
                    case WalkingMode.CHILD:
                        if (Random.Range(0, 10f) > 5)
                        {
                            walker.Navigation.agent.SetDestination(_pai.walker.transform.position +
                                                                  //-----------3X------------//
                                                                  _pai.walker.transform.forward +
                                                                  _pai.walker.transform.forward +
                                                                   _pai.walker.transform.forward +
                                                                  //------------3X------------//
                                                                  _pai.walker.transform.right * _pai.walker.Navigation.agent.radius * 2);
                        }else
                        {
                            walker.Navigation.agent.SetDestination(_pai.walker.transform.position +
                                                                  //-----------3X------------//
                                                                  _pai.walker.transform.forward +
                                                                  _pai.walker.transform.forward +
                                                                   _pai.walker.transform.forward +
                                                                  //------------3X------------//
                                                                  _pai.walker.transform.right * -1* _pai.walker.Navigation.agent.radius * 2);
                        }
                        walker.Animacao.SetFloat("value_MoodArousal", 1);
                       // walker.Navigation.startWalking();
                       // walker.Animacao.SetBool("walk", true);

                        //Se o Pai for atras 
                        if (Vector3.Distance(_pai.walker.transform.position, _pai.walker.target) >
                            Vector3.Distance(transform.position, _pai.walker.target))
                        {                            
                            walker.Navigation.agent.velocity =  Vector3.zero;                            
                            Vector3 targetDir = _pai.walker.transform.position - transform.position;                           
                            Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0F);                          
                            transform.rotation = Quaternion.LookRotation(newDir);                            
                          //  walker.Navigation.agent.speed /=2;
                          //  walker.Animacao.SetBool("walk", true);                          
#if visualizeStatesCHILD
                            Debug.DrawRay(transform.position, transform.up * 4, Color.red);
#endif
                        }
                        else
                        { //Se o Pai for a frente
                            if (Vector3.Distance(transform.position, _pai.walker.transform.position) > runningDistance)
                            {
                              walker.Navigation.agent.speed = _pai.walker.Navigation.agent.speed*2;
                              _pai.walker.Navigation.agent.velocity = Vector3.Normalize(transform.position- _pai.walker.transform.position);
                              walker.Animacao.SetBool("walk", false);
                                walker.Animacao.SetBool("run", true);
                                destination = _pai.walker.Navigation.agent.steeringTarget;
#if visualizeStatesCHILD
                                Debug.DrawRay(transform.position, transform.up * 4, Color.green);
#endif
                            }
                            else
                            { 
                              walker.Navigation.agent.speed = _pai.walker.Navigation.agent.speed/1.5f;
                              walker.Navigation.agent.velocity = _pai.walker.Navigation.agent.velocity;                               
                              walker.Animacao.SetBool("walk", false);
                              walker.Animacao.SetBool("run", true);
                      //        walker.Navigation.startWalking();
#if visualizeStatesCHILD
                                Debug.DrawRay(transform.position, transform.up * 4, Color.cyan);
#endif
                            }
                        }
                        break;//FIM CHILD

                    //FOLLOWING ESTILO SIDE BY SIDE//
                    case WalkingMode.ADULT:
                        //Escolhe o lado do parent que esta mais proximo
                        if (Vector3.Magnitude((_pai.walker.transform.position + _pai.walker.transform.right) - transform.position) <
                                Vector3.Magnitude((_pai.walker.transform.position + _pai.walker.transform.right * -1) - transform.position))
                                        closestSide = _pai.walker.transform.right * walker.Navigation.agent.radius;
                            else
                                        closestSide = _pai.walker.transform.right * walker.Navigation.agent.radius * -1f;

                            destination = _pai.walker.Navigation.agent.steeringTarget + 
                                       closestSide;
                        
                            walker.Navigation.startWalking();                            
                            walker.Animacao.SetBool("walk", true);

                            //determina distancia ao partner
                            distanceToPai = Vector3.Distance(_pai.walker.transform.position, transform.position);
                      
                            //SE O PAI VAI ATRAS---------------------------------------------------------------
                            if (Vector3.Distance( _pai.walker.target, _pai.walker.transform.position) >
                                Vector3.Distance( _pai.walker.target, transform.position))
                                 {                          
                                  if (distanceToPai > runningDistance)   //se o pai esta bue longe o gajo tem que se aproximar
                                  {
                                //Tem q alterar destination porque o steering target esta la muito longe 
                                destination = _pai.transform.position + _pai.walker.Navigation.agent.velocity;
                                mood = _pai.walker.Animacao.GetFloat("value_MoodArousal") * 0.75f;
                                walker.Navigation.agent.speed = _pai.walker.Navigation.agent.speed * 0.5f;
#if visualizeStatesADULT                                                                
Debug.DrawRay(transform.position, transform.up * 4, Color.gray);
Debug.DrawRay(transform.position, velocity, Color.grey);
Debug.DrawRay(transform.position+Vector3.up, velocity, Color.grey);
#endif
                            } else if (distanceToPai > halfRunningDistance)
                                         {
                                         velocity = _pai.walker.transform.forward*2+ Vector3.Normalize(destination  - transform.position);
                                         mood = _pai.walker.Animacao.GetFloat("value_MoodArousal") * 0.85f;
                                         walker.Navigation.agent.speed = _pai.walker.Navigation.agent.speed / 10;
#if visualizeStatesADULT
                                Debug.DrawRay(transform.position, transform.up * 4, Color.red);
#endif
                                         } else
                                            {//se o pai esta atras o gajo tem que slow down
                                             velocity = (_pai.walker.transform.forward + Vector3.Normalize(destination - transform.position))/10; 
                                             walker.Navigation.agent.speed = _pai.walker.Navigation.agent.speed / 10;
                                             mood=_pai.walker.Animacao.GetFloat("value_MoodArousal") * 0.95f;
#if visualizeStatesADULT
                               Debug.DrawRay(transform.position, transform.up * 4, Color.yellow);
#endif
                            }
                            } else //SE O PAI VAI A FRENTE--------------------------------------------------------------------------------------
                                { 
                                if (distanceToPai > runningDistance)//pai esta longe
                                   {
                                   velocity = Vector3.zero;// Vector3.Normalize(destination- transform.position ) * runningDistance; 
                                   walker.Navigation.agent.speed = _pai.walker.Navigation.agent.speed * 3f;
                                   _pai.walker.Navigation.agent.velocity *= 0.90f/ distanceToPai;
                                   mood = _pai.walker.Animacao.GetFloat("value_MoodArousal") * 1.25f;
#if visualizeStatesADULT
Debug.DrawRay(transform.position, velocity, Color.green);
Debug.DrawRay(transform.position, transform.up * 8, Color.green);
Debug.DrawRay(transform.position + Vector3.up, Vector3.Normalize(destination - transform.position), Color.green);
#endif
                            } else if (distanceToPai > threeQuarterRunningDistance)//esta ali por perto
                                             {
                                              velocity = Vector3.Normalize(destination - transform.position) * threeQuarterRunningDistance;
                                              mood = _pai.walker.Animacao.GetFloat("value_MoodArousal") * 1.10f;                                  
                                              walker.Navigation.agent.speed = 3;
#if visualizeStatesADULT
Debug.DrawRay(transform.position, transform.up * 4, Color.black);
#endif
                                              } else if (distanceToPai > halfRunningDistance)//esta ali por perto
                                                          {   
                                                          velocity = Vector3.Normalize(destination - transform.position) * halfRunningDistance;                                
                                                           mood = _pai.walker.Animacao.GetFloat("value_MoodArousal") * 1.05f;                                  
                                                           walker.Navigation.agent.speed = 2;
#if visualizeStatesADULT
Debug.DrawRay(transform.position, transform.up * 4, Color.blue);
#endif
                                                          }
                                                          else //estamos ao lado um do outro
                                                          {
                                                             velocity = _pai.walker.transform.forward + closestSide + 
                                                                  Vector3.Normalize(destination - transform.position) * walker.Navigation.agent.radius;
                                                             mood= _pai.walker.Animacao.GetFloat("value_MoodArousal");
#if visualizeStatesADULT
Debug.DrawRay(transform.position, transform.up * 4, Color.cyan);
#endif
                            }                          
                        }
                        walker.Navigation.agent.SetDestination(destination + velocity);
                        walker.Animacao.SetFloat("value_MoodArousal", mood);
                        break;//FIM ADULTO
                }//Switch walking mode
            }//is ready
        }//Update
        //-----------------------------------------------------------------------------------------------------
        //void LateUpdate()
        //{
        //    //Atencao que o InteractionHandler tem tambem o metodo stepAwayYourBreathIsTooStinky
        //    //Se esta em interacao
        //    //que move o carater para tras
        //    if (podeAndar)
        //    {
        //        RaycastHit hit;
        //        if (Physics.Raycast(transform.position + Vector3.up, walker.Navigation.agent.velocity, out hit, walker.Navigation.agent.radius * 1.25f))
        //        {
        //            Vector3 alternativa = Quaternion.AngleAxis(30, Vector3.up) * walker.Navigation.agent.velocity;
        //            if (Physics.Raycast(transform.position + Vector3.up, alternativa, out hit, walker.Navigation.agent.radius * 1.25f))
        //            {
        //                alternativa = Quaternion.AngleAxis(-30, Vector3.up) * walker.Navigation.agent.velocity;
        //                if (Physics.Raycast(transform.position + Vector3.up, alternativa, out hit, walker.Navigation.agent.radius * 1.25f))
        //                {
        //                    walker.Navigation.agent.isStopped = true;
        //                    walker.Navigation.agent.velocity = Vector3.zero;
        //                    walker.Navigation.parent.Animacao.speed = walker.Navigation.parent.Animacao.speed * 0.90f;
        //                }
        //                //TODAY    else
        //                //TODAY    { walker.Navigation.agent.velocity = alternativa * Time.time * 0.5f; }//right is free
        //            }
        //            //TODAYelse
        //            //TODAY{ walker.Navigation.agent.velocity = alternativa * Time.time * 0.5f; } //left is free

        //            ///                    Debug.DrawLine(transform.position+Vector3.up, transform.position +Vector3.up+ _agent.velocity.normalized * 1, Color.cyan, 1f);
        //        }
        //    }
        //    else walker.Navigation.agent.velocity = Vector3.zero;


        //    if (walker.Animacao.speed > 1)
        //        walker.Animacao.speed = 1;
        //}


     

#if visualizeStates
        void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            //         Gizmos.DrawWireSphere(transform.position, 0.5f);
            // Gizmos.DrawWireSphere(walker.Navigation.agent.steeringTarget, 0.5f);

            Gizmos.DrawWireSphere(transform.position+walker.Navigation.agent.velocity, 0.2f);
            Gizmos.DrawLine(walker.Navigation.agent.transform.position, transform.position+(walker.Navigation.agent.velocity));
            Gizmos.DrawLine(transform.position+Vector3.up, _pai.transform.position + Vector3.up);
        }
#endif       
    }//eo class
}//eo namespace
