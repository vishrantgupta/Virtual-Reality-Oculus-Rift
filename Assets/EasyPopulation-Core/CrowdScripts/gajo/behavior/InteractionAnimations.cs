using UnityEngine;
using System.Collections;
namespace Crowd
{
    public class InteractionAnimations
    {//: MonoBehaviour {

        public enum MyState
        {
            ATACK_NOinitiative_LOSE,
            ATACK_NOinitiative_WIN,
            ATACK_YESinitiative_LOSE,
            ATACK_YESinitiative_WIN,
            MATE_NOinitiative_LOSE,
            MATE_NOinitiative_WIN,
            MATE_YESinitiative_LOSE,
            MATE_YESinitiative_WIN,
            REPOUSO, CONHECIDO, DESCONHECIDO, ATTEMPTMATE, PREYCONHECIDA,PREYDESCONHECIDA, ATACA,
            ATACAPRODUTOR
        };

        public MyState interactionState;
        private GajoCitizen _owner;
        public InteractionAnimations(GajoCitizen owner)
        {
            _owner = owner; 
         }

        public void anima()
        {//Chamado em BehaviorAI::PerformAction
            switch (interactionState)
            {
                case MyState.ATACK_YESinitiative_WIN:
                           _owner.walker.Animacao.SetBool("initiativeWin", true);
                           _owner.walker.Animacao.SetBool("ataca", true); /**/
                    break;

                case MyState.ATACK_YESinitiative_LOSE:
                           _owner.walker.Animacao.SetBool("initiativeLose", true);
                           _owner.walker.Animacao.SetBool("ataca", true);/**/
                    break;
                case MyState.ATACK_NOinitiative_WIN:
                          _owner.walker.Animacao.SetBool("noInitiativeWin", true);
                          _owner.walker.Animacao.SetBool("ataca", true);/**/
                    break;
                case MyState.ATACK_NOinitiative_LOSE:
                           _owner.walker.Animacao.SetBool("noInitiativeLose", true);
                           _owner.walker.Animacao.SetBool("ataca", true);/**/
                    break;

                case MyState.MATE_NOinitiative_LOSE:
                           _owner.walker.Animacao.SetBool("initiativeLose", false);
                           _owner.walker.Animacao.SetBool("mate", true);/**/
                    break;
              case MyState.MATE_NOinitiative_WIN:
                           _owner.walker.Animacao.SetBool("noInitiativeWin", true);
                           _owner.walker.Animacao.SetBool("mate", true);/**/
                    break;
              case MyState.MATE_YESinitiative_LOSE:
                           _owner.walker.Animacao.SetBool("initiativeLose", false);
                           _owner.walker.Animacao.SetBool("mate", true);/**/
                    break;
              case MyState.MATE_YESinitiative_WIN:
                           _owner.walker.Animacao.SetBool("initiativeWin", true);
                            _owner.walker.Animacao.SetBool("mate", true);/**/
                    break;


              case MyState.REPOUSO:
                     _owner.walker.Animacao.SetInteger("action",1);
                     break;
              case MyState.CONHECIDO:
                    _owner.walker.Animacao.SetInteger("action", 2);
                    _owner.walker.Animacao.SetBool("mate", true);
                     break;
              case MyState.DESCONHECIDO:
                     _owner.walker.Animacao.SetInteger("action", 3);
                    _owner.walker.Animacao.SetBool("mate", true);
                     break;
              case MyState.ATTEMPTMATE:
                    _owner.walker.Animacao.SetInteger("action", 4);
                    _owner.walker.Animacao.SetBool("mate",true);
                     break;
              case MyState.PREYCONHECIDA:
                     _owner.walker.Animacao.SetInteger("action", 5);
                    _owner.walker.Animacao.SetBool("ataca", true);
                     break;
              case MyState.PREYDESCONHECIDA:
                    _owner.walker.Animacao.SetInteger("action", 6);
                    _owner.walker.Animacao.SetBool("ataca", true);
                     break;
              case MyState.ATACA:
                    _owner.walker.Animacao.SetInteger("action", 7);
                    _owner.walker.Animacao.SetBool("ataca", true);
                    break;
              case MyState.ATACAPRODUTOR:
                    _owner.walker.Animacao.SetInteger("action", 8);
                    break;
              default:
                  break;
            }   
        }
    }//class
}//namespace
