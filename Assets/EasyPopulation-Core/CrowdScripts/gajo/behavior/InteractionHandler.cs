//#define hasGigantonesInteraction
//#define visualizeStates
using UnityEngine;
using System.Collections;
//using UnityEditor.Animations;
namespace Crowd {
    public class InteractionHandler
    {
        public enum InteractionState { NEPIA, WANTS_TO_GET_LINK, IN_LINK, WANTS_TO_BREAK_LINK, FINISHED }
        public InteractionState interactionState = InteractionState.NEPIA;
        private GajoCitizen _parent;
        private GajoCitizen _partner;
        public GajoCitizen interactingPartner { get { return _partner; } set { _partner = value; } }
        public bool isInteracting;
        public float interactingDistance_min;
        public float interactingDistance_max;
        public MemoryOfRelationships Memoria;
        int walkBackAnimation = Animator.StringToHash("Base Layer.WalkBack");
        //---------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------
        public InteractionHandler(GajoCitizen parent)
        { _parent = parent;
            Memoria = new MemoryOfRelationships();

            interactingDistance_min = _parent.walker.Navigation.RAIO * 1.50f;
            interactingDistance_max = _parent.walker.Navigation.RAIO * 3f;
            isInteracting = false;
        }
        //---------------------------------------------------------------------------------------------
        //NORMAL INTERACTION---------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------------
        public void start(GajoCitizen outro, float duracao) {
            _partner = outro;
            _partner.behavior.Interacao.interactingPartner = _parent;
            //ANIMACAO
            stepAwayYourBreathIsTooStinky();

            //ESTATISTICA
            //Regista interacao
            Memoria.numeroInteracoes++;
            _partner.behavior.Interacao.Memoria.numeroInteracoes++;
            Memoria.duracaoMediaInteracoes += duracao / Memoria.numeroInteracoes;
            _partner.behavior.Interacao.Memoria.duracaoMediaInteracoes += duracao / _partner.behavior.Interacao.Memoria.numeroInteracoes;

            //BEHAVIOR
            //Increase Connectedness
            _parent.metabolism.connectedness += _parent.metabolism.boostConnectednessWithEncounters; //Ver tambem o adicional de Actions.attemptMate()
            _partner.metabolism.connectedness += _partner.metabolism.boostConnectednessWithEncounters;
//--------------------------------------------
//--------------------------------------------
//--------------------------------------------
            if (duracao < 2f) duracao = 2;
//--------------------------------------------
//--------------------------------------------
//--------------------------------------------
            //Parametros interacao
            _parent.behavior.dispatcherSpeed = duracao;
            _partner.behavior.dispatcherSpeed = duracao;
            interactionState = InteractionState.WANTS_TO_GET_LINK;
            _partner.behavior.Interacao.interactionState = InteractionState.WANTS_TO_GET_LINK;
            isInteracting = true;
            _partner.behavior.Interacao.isInteracting = true;
            _parent.walker.Animacao.SetBool("interact", true);
            _partner.walker.Animacao.SetBool("interact", true);
   //         _parent.walker.Animacao.SetBool("waving", false);
            //----------------------------
#if hasGigantonesInteraction
	_parent.transform.localScale = new Vector3 (1, 8, 1);
	_partner.transform.localScale = new Vector3 (1, 8, 1);
#endif
            //Agora ponho o segundo interacto no estado do primeiro menos 1, 
            //para evitarestarem os dois a fazer a mesma animacao
            _partner.behavior.myMarkov.currentState = _parent.behavior.myMarkov.currentState - 1;

            //Ficam no mesmo sitio
            _partner.walker.Navigation.stopMoving();
            _parent.walker.Navigation.stopMoving();
            _parent.walker.Navigation.locomotionMode = Navigation.LocomotionMode.MODE_STOPPED;
            _partner.walker.Navigation.locomotionMode = Navigation.LocomotionMode.MODE_STOPPED;
        }
        //----------------------------------
        //INTERACTION WITH PRODUTOR---------
        //----------------------------------
        //Comandado a partir do owner(Gajo)
        public void start(GajoWorker outro, float duracao)
        {
            _parent.walker.Animacao.SetBool("interact", true);
            _parent.walker.Navigation.stopMoving();
            isInteracting = true;
          //  _parent.transform.LookAt(outro.transform);//Isto não devia estar aqui, mas por algum motivo não funciona se so estiver no LookAt
            _parent.behavior.dispatcherSpeed = duracao;
            outro.startInteraction(_parent);
        }

        //----------------------------------
        private void OnEnable()
        { Dispatcher.requestStopInteraction += stop; } //invoca a funcao stop e coloca na stack que esta no dispatcher
        private void OnDisable()
        { Dispatcher.requestStopInteraction -= stop; }
        //---------------------------------------------------------------------------------------------
        //STOP INTERACTION   
        //---------------------------------------------------------------------------------------------

        public void stop() {
#if hasGigantonesInteraction
            _parent.transform.localScale = new Vector3(1, 1f, 1);
#endif
            interactionState = InteractionState.NEPIA;
            _parent.walker.Animacao.SetBool("interact", false);
            _parent.walker.Animacao.SetBool("frente_a_frente", false);
            //       _parent.walker.Animacao.SetTrigger("waving");

            _parent.behavior.Interacao.Memoria.typeOfInteraction = MemoryOfRelationships.TypeRelationship.VOID;
            _parent.behavior.Interacao.Memoria.isCooperativeInteractionC = false;

            //****************
            isInteracting = false;
            if (_parent != null)
                if (_parent.iEstado == GajoCitizen.GajoState.VIVO)
                {
                    _parent.behavior.dispatcherSpeed = Global.normalDispatcherInterval;
                }
            if (_partner != null)
            {
                if (_partner.iEstado == GajoCitizen.GajoState.VIVO)
                {
                    if (_partner.behavior != null)
                    {
                        _partner.behavior.Interacao.interactionState = InteractionState.NEPIA;
                        _partner.behavior.Interacao.isInteracting = false;
                        _partner.behavior.dispatcherSpeed = Global.normalDispatcherInterval;
                        //*****************
                        _partner.behavior.Interacao.Memoria.typeOfInteraction = MemoryOfRelationships.TypeRelationship.VOID;
                        _partner.behavior.Interacao.Memoria.isCooperativeInteractionC = false;
                        //*****************
                    }
                    if (_partner.walker != null)
                    {
                        _partner.walker.Animacao.SetBool("interact", false);
                        _partner.walker.Animacao.SetBool("frente_a_frente", false);
                    }
                }
            }
            _partner = null;
            _parent.walker.Navigation.agent.SetDestination(_parent.walker.preferedPosition);

            _parent.walker.Navigation.startWalking();

        }
    //---------------------------------------------------------------------------------------------             
    //BREAK IF FAR AWAY-------------------------------------------------------------------------
    //---------------------------------------------------------------------------------------------
    //Chamado no GajoCitizen:Update() 
        public void breakIfFarAway()
        { 
        //Se demasiado longe termina a ligacao 
        if (Vector3.Distance(_parent.transform.position,
                             interactingPartner.transform.position) >
                             interactingDistance_max)
            stop();
        }
    //---------------------------------------------------------------------------------------------             
    //ADJUST DISTANCE-------------------------------------------------------------------------
    //---------------------------------------------------------------------------------------------                                   
    //Chamado no GajoCitizen:Update() 
    public void stepAwayYourBreathIsTooStinky()
        {
        //Atencoa que o Navigation tem o LateUpdate um metodo para andar para tras 
        //Se demasiado proximos, anda para tras
            if (Vector3.Distance(_parent.transform.position,
                             interactingPartner.transform.position) <
                             interactingDistance_min)
            { 
                _parent.walker.Animacao.CrossFade(walkBackAnimation, 0.001f, _parent.walker.Animacao.GetLayerIndex("Base Layer"));

                _parent.transform.position += 
                                            _parent.walker.Navigation.agent.height / 100 * -1f * _parent.transform.forward +
                                            _parent.walker.Navigation.agent.height / 100 * _partner.transform.forward;
                                            _parent.walker.Animacao.SetBool("interact", true);
#if visualizeStates
Debug.DrawRay(_parent.transform.position + Vector3.right, Vector3.up * 5, Color.cyan);
Debug.DrawRay(_parent.transform.position + Vector3.up, Vector3.up * 5, Color.cyan);
Debug.DrawRay(_parent.transform.position + Vector3.left, Vector3.up * 5, Color.cyan);
#endif
            }
        }
    //---------------------------------------------------------------------------------------------             
    //REWALK-------------------------------------------------------------------------
    //---------------------------------------------------------------------------------------------  
        public void resetInteractionAnimationVariables()
        {//Retoma o walk se nao esta em iteracao 
            //*********************  
            //VEIO DO LOOKAT **INICIO**
            if (_parent.walker.Animacao.GetBool("interact") ||
                    _parent.walker.Animacao.GetBool("ataca") ||
                   _parent.walker.Animacao.GetBool("mate") ||
                    _parent.walker.Animacao.GetBool("interact") ||
                    _parent.walker.Animacao.GetBool("noInitiativeWin") ||
                   _parent.walker.Animacao.GetBool("noInitiativeLose") ||
                    _parent.walker.Animacao.GetBool("initiativeWin") ||
                    _parent.walker.Animacao.GetBool("initiativeLose")
                    )
            {
                _parent.walker.Animacao.SetBool("interact", false);                
                _parent.walker.Animacao.SetBool("ataca", false);
                _parent.walker.Animacao.SetBool("mate", false);
                _parent.walker.Animacao.SetBool("interact", false);
                _parent.walker.Animacao.SetBool("noInitiativeWin", false);
                _parent.walker.Animacao.SetBool("noInitiativeLose", false);
                _parent.walker.Animacao.SetBool("initiativeWin", false);
                _parent.walker.Animacao.SetBool("initiativeLose", false);

                //  gajo.walker.Animacao.Rebind();
                //  _parent.walker.Animacao.SetBool("walk", true);
            }
            //VEIO DO LOOKAT **FIM** 
            //****************
        }

        //---------------------------------------------------------------------------------------------             
        //OUTCOMEOFINTERACTION-------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------------
        //----0<x<1 No
        //----x>1    Yes
        public float engageInDialog(GajoCitizen interactionPartner)
		{
			if (interactionPartner.metabolism == null)
				return 0;
			if (_parent.metabolism == null)
				return 0;
			
			//calcula utility
			float[] my_chemicals = new float[3];
			
			my_chemicals=_parent.metabolism.chemicals;
			float[] other_chemicals = new float[3];
			
			other_chemicals=interactionPartner.metabolism.chemicals;
			float relationRP=0;
			for(int i=0; i<my_chemicals.Length; i++)
				relationRP+=my_chemicals[i]*other_chemicals[i];
			float utility=1-1/(1+relationRP);
			
			//calcula psychology
			float psychol = psychologyAccessment (_parent.behavior.psy.mood, interactionPartner.behavior.psy.mood);
			
			//soma e determina
			float outcome = 0;
			outcome = utility + psychol;

			return outcome;
		}

//---------------------------------------------------------------------------------------------
        float psychologyAccessment(Vector3 mood1, Vector3 mood2)
        {
            Vector3 mRes = (mood1 + mood2) / 2;
            if (mRes.x > 0.5f)
            {
                if (mRes.y > 0.5f)
                {
                    if (mRes.z > 0.5f)
                        return 1; //exuberant (+++)
                    else
                        return 0.25f; //dependent (++-)
                }
                else
                {
                    if (mRes.z > 0.5f)
                        return 1; //relaxed (+-+}
                    else
                        return 1; //docile (+--)
                }
            }
            else
            {
                if (mRes.y > 0.5f)
                {
                    if (mRes.z > 0.5f)
                        return -1; //hostile (-++)
                    else
                        return 0.25f; //anxious (-+-)
                }
                else
                {
                    if (mRes.z > 0.5f)
                        return -1; //disdainful (--+)
                    else
                        return 0.25f; //bored (---)
                }
            }
        }
       
//---------------------------------------------------------------------------------------------        
        public void die()
        {
            _parent = null;
            for (int i = 0; i < Memoria.listOfInteractionPartners.Count; i++)
            {
                Memoria.listOfInteractionPartners[i] = null;
            }
            Memoria.listOfInteractionPartners.Clear();
            Memoria.listOfInteractionPartners = null;
        }
//---------------------------------------------------------------------------------------------
	}//class
}//namespace

