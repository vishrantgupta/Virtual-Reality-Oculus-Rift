//Cria gigantes nas interacoes
//#define hasGigantonesInteraction  
//#define debug_interactions
#define BUILD_MODE
//#define visualizeStates
//Poe indicacao estado Markov sobre o gajo (numero em branco)
//---------------------------------------------------------------------------------------------------#define BUILD_MODE
//Adiciona Colliders aos Gajos


using UnityEngine;
using System.Collections;
using UnityEngine.AI;

namespace Crowd {
	[RequireComponent(typeof(DNA))]
	public class GajoCitizen : Gajo {
       
        public int id;
        public enum GajoState {INITIALIZE, VIVO, MORTO, DECOMPOS, ZOMBIE}
		public GajoState iEstado = GajoState.INITIALIZE;

        public DNA blueprint;
		public Metabolism metabolism;
		//public WalkerAI walker;
        public BehaviorAI behavior;
        private LookAt orientation;
        //-------------
        public float proxemia = 5.25f;
		public byte bNumNeighbours = 0;
		public float hitDistance= 3.1f;
        public float reproductiveDistance = 3.1f;
        public byte bInteractingNeighbours = 0;
		public float joyfullness;
      
		//Prices
		public float priceBreathing= 0.1f; 
		public float priceJoy= 1.0f;		
		public float priceCollision = 1.0f;
		public float priceRotation = 0.001f;
		public float priceSearchForFood = 0.01f;
		public float priceMovement = 0.01f;
		public float priceAttack = 5.0f; //multiplica pelo sizeGajo	
		public float priceOutOfBounds=1.0f; 
		public float pricePlay=1f;
		public float priceMate=0.5f;//pays twice
		public float priceReproduz=11f;//pays twice
		public float priceMoveAway=0.01f;//pays t
		public float priceWander=0.01f;//pays t
		public float priceAttemptToMate=2.5f;//pays t
		public float priceFomeGajo=0.001f;//pays t
		public float priceFomeCarc=0.001f;//pays t
		public float priceComeGajo=0.001f;//pays t
		public float priceRoda=0.001f;//pays t

		public Actions.TypeOfActionBeingPerformed whatAmIDoing = Actions.TypeOfActionBeingPerformed.NEPIA;	
		int indexLayer;
       
        float deathRadius=250f;
///---------------------------------------------------------------------------------------------

        ///---------------------------------------------------------------------------------------------
        //Initializes components
        ///---------------------------------------------------------------------------------------------
        public void myawake()
            //Gajo Recebe o Estado VIVO no Behavior
		{

            initializeGajo();//Super
            TextMesh tm= gameObject.GetComponent<TextMesh>();
            if(tm==null)
                tm    = gameObject.AddComponent<TextMesh>();
            tm.characterSize = 0.5f;
            tm.offsetZ = 0;
            tm.lineSpacing = 1;
            tm.anchor = TextAnchor.UpperCenter;
            tm.alignment = TextAlignment.Center;
            tm.tabSize = 4;
            tm.color = Color.white;
            tm.richText = true;
         
            orientation = gameObject.AddComponent<LookAt>();
            orientation.head = gameObject.transform.Find("Head");
            //orientation.lookAtCoolTime = 0.2f;
            //orientation.lookAtHeatTime = 0.2f;
            //orientation.lookAtTargetPosition = new Vector3(0, 0, 0);
            
//###### DNA #############################################################
            gameObject.AddComponent<DNA> ();
			blueprint = GetComponent<DNA> ();
//###### METABOLISM  #####################################################			
			gameObject.AddComponent<Metabolism> ();
			metabolism = GetComponent<Metabolism> ();
//###### NAVIGATION ######################################################	
//			gameObject.AddComponent<WalkerAI> (); chamado no super
//			walker = GetComponent<WalkerAI>();
            walker.init(this);
            walker.initializeGoalPoints();
//###### BEHAVIOR #########################################################	           
        }
//---------------------------------------------------------------------------------------------
        public void init(int regNum, int[] dnaMyParent)
		{
            myawake();
            //GType
			blueprint.initializeGType(dnaMyParent);
       
            behavior = new BehaviorAI(this);
            metabolism.initialize(blueprint.dna);        
            
            //Sai de um dos bercarios
            if (Global.hasNurseries && dnaMyParent!=null)
				transform.position = ((Vector3)(walker.goalPoints [Random.Range (0, walker.goalPoints.Count)]));
			id=regNum;
        }
//---------------------------------------------------------------------------------------------
		public int getId()
		{	return id;	}

    
        //--------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------        
        public void Update()
        {
          
            //---------------------------------------------------
            //CHECKA ESTADO
            //---------------------------------------------------
            switch (iEstado)
			{
			case GajoState.VIVO:
				if (metabolism.energy < 0) {	
					turnToZombie ();				
				}
                    /*
                     *****************************************************************************************
                     *****************************************************************************************
                     * *****************************************************************************************
                     *  * REMOVE OS QUE ESTAO MARCADOS COMO ESTANDO FORA DA AREA
                     * if (walker.Navigation.locomotionMode == Navigation.LocomotionMode.OUTSIDE_NAVMESH) {
                                        turnToZombie();				
                                        remove();
                                    }
                    *****************************************************************************************
                    *****************************************************************************************
                    ******************************************************************************************
                    */
                    break;
			case GajoState.MORTO:
			case GajoState.ZOMBIE:
				if(walker!=null)
					if (Vector3.Distance (transform.position, walker.target) < deathRadius) {
						die_e_renasce ();	
					}
				break;
			}				

#if BUILD_MODE
            UpdateVisualizacaoDebugEstadoMessage();
#endif
//---------------------------------------------------
//AJUSTA DISTANCIA QUANDO PARCEIROS ESTAO MUITO PROXIMOS 
//CONTROLA (INIBE) ANIMACOES DE MEIA DISTANCIA
//---------------------------------------------------
            switch(iEstado)
            {
                case GajoCitizen.GajoState.VIVO:
                    if (walker != null && behavior != null)
                        if (behavior.Interacao != null)
                            if (behavior.Interacao.isInteracting || (walker.Animacao.GetBool("interact")))
                            {
                                if (behavior.Interacao.interactingPartner != null)
                                {
                                    //-----------
                                    //Ajusta distancia se os parceiros estao muito em cima um dos outro 
                                    behavior.Interacao.stepAwayYourBreathIsTooStinky();

                                    //-----------
                                    orientation.rotateBodyToPartner();

                                    //-----------
                                    //Large Distance => Stop Interaction
                                    //Inibe as interacoes de meia distancia 
                                    //TODO pode fazer animacao especifica em vez de inibir
                                    behavior.Interacao.breakIfFarAway();

                                    //-----------------------------------
                                    //Outside of navmesh => Stop Interaction                        
                                    UnityEngine.AI.NavMeshHit hit;
                                    if (!UnityEngine.AI.NavMesh.SamplePosition(transform.position + Vector3.up, out hit, 5.0f, UnityEngine.AI.NavMesh.AllAreas))
                                    {
                                        behavior.Interacao.stop();
                                    }
                                    //-----------------------------------
                                }
                                else
                                    if (behavior.Interacao != null)
                                    behavior.Interacao.stop();
                            }
                            else
                            {
                                behavior.Interacao.resetInteractionAnimationVariables();
                                if(Vector3.Distance(transform.position, walker.Navigation.agent.destination)>0.15f)//walker.Navigation.agent.radius)
                                     walker.Animacao.SetBool("walk", true);
                                else
                                    walker.Animacao.SetBool("walk", false);
                                   }
                    break;
            }
        }   

//--------------------------------------------------------------------------------------------
//--------------------------------------------------------------------------------------------
//---------------------------------------------------------------------------------------------
        public void startInteraction(GajoCitizen g, float duracao)		{
            behavior.Interacao.start(g, duracao);         
        }
        //-------------------------------------------------------------------------------------
        public void startInteraction(GajoWorker m, float duracao)
        {
            behavior.Interacao.start(m, duracao);
        }
        //-------------------------------------------------------------------------------------
        public void stopInteraction()		{
            if(iEstado ==GajoState.VIVO)
                behavior.Interacao.stop();
        }
//--------------------------------------------------------------------------------------------
//-----TURN TO ZOMBIE-------------------------------------------------------------------------
//--------------------------------------------------------------------------------------------
        public void turnToZombie(){
            if (iEstado == GajoState.VIVO)
            {
                if (behavior.Interacao.interactionState != InteractionHandler.InteractionState.NEPIA)
                {
                    behavior.Interacao.interactionState = InteractionHandler.InteractionState.WANTS_TO_BREAK_LINK;
                    stopInteraction();
                }
            }
           
			iEstado = GajoState.ZOMBIE;
			//Coloca em estado NUMB
			//----------------------------------------------
			//Clean memory----------------------------------
			behavior.psy.mood = behavior.psy.personality;
			behavior.psy.emotion = behavior.psy.personality;
			behavior.myMarkov.currentState = 1;
			behavior.cleanMemoria ();
			//----------------------------------------------

            StopAllCoroutines ();

            walker.target = walker.cemiterio;

            if(Global.hasCemetery)
               walker.preferedPosition=walker.cemiterio;           
            metabolism.enabled = false;
            /**/
			walker.goalPoints.Clear();
            walker.preferedPosition = walker.cemiterio;
            walker.Navigation.startWalking();
//Debug.DrawLine(transform.position, transform.position + Vector3.up * 10, Color.cyan);
		}
//--------------------------------------------------------------------------------------------
//---REMOVE-----------------------------------------------------------------------------------
//--------------------------------------------------------------------------------------------
		public void remove()
        {
	//---------------------------------------------------------
    //Luis * Por cada um que e removido temos que criar um novo 
	// pois a população tem que ficar constante
			int[] auxDna = blueprint.dna;
            CitizenFactory.createGajo (auxDna);
    //---------------------------------------------------------
            iEstado = GajoState.MORTO;
		    StopAllCoroutines ();
            
            //Limpa referencias nos outros                
            int indexDesteNaLista = -1;
            for (int i=0; i< CitizenFactory.listaGajos.Count; i++) {
                GajoCitizen g = CitizenFactory.listaGajos [i];
                
                 //Quem o quer comer, fica a apontar o walker para si proprio
				if (g != null) {
					if (g.walker != null)
					if (g.walker.target == transform.position)
						g.walker.target = g.transform.position;

					//quem interagiu, limpa
					if (g.behavior != null) {//Exception para quando acabou de morrer

						g.behavior.Interacao.stop ();
						
						if (g.behavior.dispatcher != null)
						if (g.behavior.Interacao.Memoria.idInteractionPartner == getId ())
							g.behavior.Interacao.Memoria.idInteractionPartner = -1;						

						if (g == this)
							indexDesteNaLista = i;
					}
				}
              }

            //Limpa as sub-rotinas
            if (behavior != null)
            {
                if (behavior.dispatcher != null)
                    behavior.dispatcher.die();
                Destroy(behavior.dispatcher);
                behavior.dispatcher = null;
                behavior.die();                
            }

            behavior = null;
            if (walker!=null)
                    walker.die ();
            Destroy(walker);
            walker = null;

			if(indexDesteNaLista!=-1)
                CitizenFactory.listaGajos[indexDesteNaLista] = null;
          	Destroy (gameObject);
        }
//--------------------------------------------------------------------------------------------
//---DIE-------------------------------------------------------------------------------------
        int[] emptyDNA = new int[DNA.DNA_LENGHT];
//-------------------------------------------------------------------------------------------- 
        public void die_e_renasce()
		{   //inicializa auxiliar
//-------------------------------------------------
//Vai buscar as relacoes de sucesso e updata o DNA
//-------------------------------------------------
            float[] somatorioDnas = new float[blueprint.dna.Length];            
            for (int i = 0; i < blueprint.dna.Length; i++)
                somatorioDnas[i]=0;
            int numInteracoesCAprendizagem = 0;

			//Soma todos os dnas que vem na list of Interaction Partners (celula por celula)
            foreach (IndividualRelatioship ir in behavior.Interacao.Memoria.listOfInteractionPartners)
            {
                if (!Utils.arrayCompare(ir.dna, emptyDNA))//tem dna preenchido como tal houve aprendizagem
                {
                    numInteracoesCAprendizagem++;
                  
                    for (int i = 0; i < ir.dna.Length; i++)
                    {
                        if (ir.dna[i] == 1) somatorioDnas[i] += 1;
                        else somatorioDnas[i] += 0;
                    }
                }
            }

			//Copia para a blueprint a media das DNAs em memoria
            if (numInteracoesCAprendizagem > 0)
            {
				//Faz a media dos valores somados
				for (int i = 0; i < somatorioDnas.Length; i++)
                {
				    somatorioDnas[i] += (float)(blueprint.dna[i]);
				    somatorioDnas[i] /= numInteracoesCAprendizagem + 1;
					if (somatorioDnas[i] > 0.5f) somatorioDnas[i] = 1;
					else if (somatorioDnas[i] < 0.5f) somatorioDnas[i] = 0;
					else if (Random.value > 0.5) somatorioDnas[i] = 1;
					else somatorioDnas[i] = 0;
                }
            	//copia para blueprint
				for (int i=0; i<somatorioDnas.Length; i++)
					blueprint.dna[i] = Mathf.RoundToInt(somatorioDnas[i]);               
            }
//-------------------------------------------------------------------------
//INICIA AS VARIAVEIS A PARTIR DO NOVO DNA
//-------------------------------------------------------------------------
			//Novo metabolismo
            metabolism.initialize(blueprint.dna);	
	
			behavior.restart ();
            metabolism.enabled = true;
//<--
            walker.init(this);
            walker.initializeGoalPoints();
			walker.getNextGoalPoint ();
			iEstado = GajoState.VIVO;
   }

//--------------------------------------------------------------------------------------------------
//- GRAFICOS ------------------------------------------------------------------------------------------
//--------------------------------------------------------------------------------------------------   

#if BUILD_MODE
        public void OnDrawGizmos()
        { 
         if (iEstado == GajoState.VIVO)
            {               
/*Path*/
/*              Gizmos.color = Color.green;
                Vector3 prevPos = transform.position;
                foreach (Vector3 v in walker.Navigation.agent.path.corners)
                {   Gizmos.DrawLine(prevPos, v);
                    prevPos = v;
                }            
*/
/*Target interaction*/
 /*             bool bTargetBercario = false;
                string s= " " + walker.target + " - " ;
                foreach (Vector3 t in walker.goalPoints)
                    {
                    s += t + " ";                             
                    if ((Vector3.Distance (walker.target, t)<5 ))
                        bTargetBercario = true;
                    }
                if (!bTargetBercario)
                    {
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawLine(walker.target+Vector3.up*2, transform.position+Vector3.up*2);
                    }
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(walker.target+Vector3.up*2, 0.25f);         
*/            }

            //--------------------------------------------
            /*Interaction*/
            //--------------------------------------------
#if visualizeStates
           if (iEstado == GajoState.VIVO)
             if(behavior!=null)
              if(behavior.Interacao != null)
                if (behavior.Interacao.isInteracting) {
                  if (behavior.Interacao.interactingPartner != null)
                                //Interaction zone

                                Gizmos.color = Color.red;

                      if (behavior.Interacao.interactingPartner == null)
                         {
                          Gizmos.color = Color.black;
                          Gizmos.DrawSphere(transform.position, behavior.Interacao.interactingDistance_min);
                         }
                         else if (Vector3.Distance(gameObject.transform.position,
                                behavior.Interacao.interactingPartner.transform.position) <
                                behavior.Interacao.interactingDistance_min)
                                {
                                    Gizmos.DrawSphere(transform.position, behavior.Interacao.interactingDistance_min);
                                }
                            else if (Vector3.Distance(gameObject.transform.position, 
                                      behavior.Interacao.interactingPartner.transform.position) > 
                                      behavior.Interacao.interactingDistance_min)
                                      {
Gizmos.color = Color.black;
Gizmos.DrawWireSphere(transform.position, behavior.Interacao.interactingDistance_min);
Gizmos.DrawWireSphere(behavior.Interacao.interactingPartner.transform.position, behavior.Interacao.interactingDistance_min);
                                       }

//Interaction direction
Gizmos.color = Color.cyan;
                     if (behavior.Interacao.interactingPartner != null) {
                         Gizmos.DrawLine(transform.position , 
                                         behavior.Interacao.interactingPartner.transform.position + Vector3.up);
//Interaction state
                     switch (behavior.Interacao.interactionState) {
                              case InteractionHandler.InteractionState.WANTS_TO_GET_LINK:
                                        Gizmos.color = Color.yellow;
                                        Gizmos.DrawWireSphere(transform.position, behavior.Interacao.interactingDistance_min);
                              break;
                              case InteractionHandler.InteractionState.IN_LINK:
                                        Gizmos.color = Color.green;
                                        Gizmos.DrawWireSphere(transform.position, behavior.Interacao.interactingDistance_min);
                              break;
                              case InteractionHandler.InteractionState.WANTS_TO_BREAK_LINK:
                                        Gizmos.color = Color.blue;
                                        Gizmos.DrawWireSphere(transform.position, behavior.Interacao.interactingDistance_min);
                              break;
                              case InteractionHandler.InteractionState.FINISHED:
                                        Gizmos.color = Color.red;
                                        Gizmos.DrawWireSphere(transform.position, behavior.Interacao.interactingDistance_min);
                              break;
                         }
                     }//has interactingPartner                         

        }//isInteract

            //------------------------------------------
            /* Estado */
            //------------------------------------------
            if (iEstado == GajoState.ZOMBIE)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawWireSphere(transform.position, walker.Navigation.agent.radius);
            }
            else
            if (iEstado == GajoState.MORTO)
            {
                //REDUNDANTE Esta morto, foi retirado
                Gizmos.color = Color.grey;
                if(walker!=null)
                    Gizmos.DrawWireSphere(transform.position, walker.Navigation.agent.radius);
            }
            else //ESTA VIVO
            {
                Gizmos.color = Color.white;
                Gizmos.DrawWireSphere(transform.position, walker.Navigation.agent.radius);
            }
#endif
        }
        //--------------------------------------------------------------------------------------------------
        //-----DEBUG MESSAGES-------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------
        void UpdateVisualizacaoDebugEstadoMessage()
        {
            /*
            if (walker != null)
            {
                AnimatorStateInfo currentState = walker.Animacao.GetCurrentAnimatorStateInfo(0);
                string s = "";

                if (currentState.fullPathHash == Animator.StringToHash("Base Layer.Walk"))
                {
                    s = "I'm walking";
                }
                if (currentState.fullPathHash == Animator.StringToHash("Base Layer.Run"))
                {
                    s = "I'm running";
                }
                if (currentState.fullPathHash == Animator.StringToHash("Base Layer.Interact"))
                {
                    s = "I'm interacting";
                }
                if (currentState.fullPathHash == Animator.StringToHash("Base Layer.Waving"))
                {
                    s = "I'm waving";
                }

                if (currentState.fullPathHash == Animator.StringToHash("Base Layer.BlendTree-Idle"))
                {
                    s = "I'm BlendTree";
                }
                if (currentState.fullPathHash == Animator.StringToHash("Base Layer.StepInSamePlace"))
                {
                    s = "I'm SteppingInSamePlace";
                }
                if (currentState.fullPathHash == Animator.StringToHash("Base Layer.Desvia"))
                {
                    s = "I'm Desvia";
                }
                if (currentState.fullPathHash == Animator.StringToHash("Base Layer.WalkBack"))
                {
                    s = "I'm WalkBack";
                }
                if (iEstado == GajoState.VIVO)
                    if(behavior!=null)
                        gameObject.GetComponent<TextMesh>().text = " " + behavior.myMarkov.currentState + " " + "\n"
                    //               + iEstado + " " +
                    //              " w:" + walker.Animacao.GetBool("walk") +
                    //               " r:" + walker.Animacao.GetBool("run") +
                    //               " i:" + walker.Animacao.GetBool("interact") + 
                    //              " wv:" + walker.Animacao.GetBool("waving") + "\n"+ 
                    //              Vector3.Distance(transform.position, walker.target) + " " + Vector3.Distance(transform.position, walker.preferedPosition) + "\n"+
                    //             " "+behavior.dispatcherSpeed + " " + s;
                    ;
            }
            */
          }
#endif

                                //--------------------------------------------------------------------------------------------------
                                //- MOUSE ------------------------------------------------------------------------------------------
                                //--------------------------------------------------------------------------------------------------
#if mouseMathers
        public void OnMouseDown()
        {
            Debug.Log("-------------" + iEstado + " " + walker.Animacao.GetFloat("walk") + walker.Animacao.GetFloat("run") + walker.Animacao.GetFloat("interact") + walker.Animacao.GetBool("waving"));
            Global.whoIsBeingTracked = id;
            Debug.Log(gameObject.name +
                " bInterNeighb:" + bInteractingNeighbours +
                " interPart:" + behavior.Interacao.interactingPartner +
                " intState:" + behavior.Interacao.interactionState +
                " iEstado:" + iEstado +
                " markovState:" + behavior.myMarkov.currentState);

            RaycastHit[] hits;
            string s = "";
            hits = Physics.SphereCastAll(transform.position, //origin
                                         1.5f, //raio
                                         transform.forward, //direcao
                                         2f //distance
                                          ); //layer
            foreach (RaycastHit hit in hits)
            {
                s += (hit.collider.gameObject.name + " - ");
            }
            Debug.Log(s);
        }
#endif
                                //---------------------------------------------------------------------------------------------
                        }//eo class
}//eo namespace
