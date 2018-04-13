
using UnityEngine;
using System.Collections;
namespace Crowd {
public class BehaviorAI {

 	private GajoCitizen _owner;
    public GajoCitizen owner { get { return _owner; } set { _owner = value; } }

    public int currentTimeT=0;
	public int lastActionPerformed=255;

	public Psychology psy;
	public MarkovGajo myMarkov;
	public Rewards rewards;
    public Actions actions;

    public float dispatcherSpeed = 0.09f;  //velocidade call dispatcher	
    public Dispatcher dispatcher;
	Coroutine dispatcherCoRoutine;
    //---------------
    private InteractionHandler _interacaoHandler = null;
    public InteractionHandler Interacao { get { return _interacaoHandler; } set { _interacaoHandler = value; } }
//------------------------------------------------------------------------------------------------------------------------------
//------------------------------------------------------------------------------------------------------------------------------	
//------------------------------------------------------------------------------------------------------------------------------
    public string debugString="";	
	public bool bFome=false;
	public bool bHeat=false;
    public bool bPertoProdutor = false;
    public bool bProdutorExists = false;
    public bool bPertoPrey=false;        
    public bool bPreyExists=false;
	public bool bPertoMate=false;
	public bool bMateExists=false;
	public bool bConhecido=false;
	public bool bDesconhecido=false;
	public bool bInteract=false;
	public bool bNInteract=false;
    public bool bEmergency = false;
	//-----------------
	public GajoCitizen closestPrey = null;
	public GajoCitizen closestMate = null;
    public GajoWorker closestProdutor = null;

    private float distanceToClosestPrey = 999999;
	private	float distanceToClosestMate = 999999;
    private float distanceToClosestProdutor = 999999;
    public  float distanciaObjectivo=0;
  
    public float levelOfResources = 0;
    public float sucessoAccaoAnterior = 0;

    public float exuberance = 0;
    public float relaxation = 0;

    
//------------------------------------------------------------------------------------------------------------------------------
//------------------------------------------------------------------------------------------------------------------------------
//Class to record data in Log file, preserving the data after each cycle
//------------------------------------------------------------------------------------------------------------------------------
	public class Snapshot {
			public Vector3 position;
			public GajoCitizen.GajoState iEstado;
			public Vector3 mood;
			public float duracaoMediaInteracoes;
			public string action;
			public Vector3 emotion; 
			public Vector3 personality;
			public float energy;
			public float connectedness;
			public Vector3 chemicals;
			public int[] blueprint;
			public int numeroInteracoes;
			public int totalCooperativeInteractionsW;
			public int totalInteractionsN;
			public MemoryOfRelationships.TypeRelationship typeOfInteraction;
			public float energyTolerance;
			public int actionPerformed;
            public char sucessoInteracao;
		}
		public Snapshot snapshot=new Snapshot();
//------------------------------------------------------------------------------------------------------------------------------	
//------------------------------------------------------------------------------------------------------------------------------
// BEHAVIOR AI 
//------------------------------------------------------------------------------------------------------------------------------
        public BehaviorAI(GajoCitizen owner)
		{
		 _owner = owner;
         actions = new Actions(_owner);
         _interacaoHandler = new InteractionHandler(_owner);
         myMarkov = new MarkovGajo (this);
		 psy = new Psychology ();
         psy.personality = owner.blueprint.getPersonality();
         rewards = new Rewards(this);
         
         _owner.gameObject.AddComponent<Dispatcher>();
         dispatcher = _owner.GetComponent<Dispatcher>();    	
         dispatcher.setOwner(_owner);
         _owner.iEstado = GajoCitizen.GajoState.VIVO;
         _owner.gameObject.SetActive(true);
		 dispatcherCoRoutine=_owner.StartCoroutine(dispatcher.init());          
        }
//------------------------------------------------------------------------------------------------------------------------------
	public void restart()
		{	_owner.StopCoroutine(dispatcherCoRoutine);
			//limpa memoria - (Interaction Handler)
			Interacao.Memoria.listOfInteractionPartners.Clear();    
			dispatcherCoRoutine=_owner.StartCoroutine(dispatcher.init());      
		}
//----------------------------------------------------------------------------------------------- 
//----------------------------------------------------------------------------------------------- 
//PROCESS BEHAVIOR 
//----------------------------------------------------------------------------------------------- 
        float reward = 0;
        public int actionPerformed = -1;
//Main function //Called no Dispatcher
     public void processBehavior()        {
			if (_owner.iEstado == GajoCitizen.GajoState.VIVO) {
             //   if (!_owner.behavior.Interacao.isInteracting) {
                    _owner.metabolism.performMetabolicFunctions();

                    levelOfResources = _owner.metabolism.chemicals[0] + _owner.metabolism.chemicals[1] + _owner.metabolism.chemicals[2];
                    //Calcula distancias e conta neighbours
                    scanNeighbourhood();
                    //Activates the boolean flags (fome, heat, bPertoPrey, bPertoMate) 
                    //according to the surrounding environment and the neighbours
                    accessesSituation();
                    //Chooses the next state based on this configuration
                    //Chooses action according to the previous experiences

                    actionPerformed = myMarkov.processMarkovChoice();
                    performAction(actionPerformed);

                    //calculate reward, mot, urg, dom, ctrl
                    reward = rewards.calculatesAppraisal();
                    rewards.determinesBonusReward(actionPerformed, reward);

                    lastActionPerformed = actionPerformed;
                    //normalized reward - tentativa de ver se não overcome
                    if (reward >= 10)
                        reward = 9;
                    myMarkov.updatesChain(reward / 10);

                    emotionalUpdate();
                    //Cria um registo de memoria que é o que vai ser guardado no Log
         //       }
			}
			//Guardar na memoria (é independente de estar VIVO ou nao). 
			registaMemoria ();

			if (_owner.iEstado == GajoCitizen.GajoState.VIVO) {
//---------------------------------------
				_owner.behavior.Interacao.Memoria.ganhoTradingOrSocializing = 0;
			//	psy.emotion = psy.personality;
//-----------------------------------------
			}
        }

    public void setDistanciaObjectivo(float dist) { distanciaObjectivo=dist; }
	public float getDistanciaObjectivo() { return distanciaObjectivo;}
//---------------------------------------------------------------------------------------------------
//-----------------------------------------------------------------------------------------------
	public void scanNeighbourhood()		{
            if (_owner.iEstado == GajoCitizen.GajoState.VIVO)
            {
                closestPrey = null;
                closestMate = null;
                closestProdutor = null;
                distanceToClosestPrey = 999999;
                distanceToClosestMate = 999999;
                distanceToClosestProdutor = 999999;

                //CHECKS WORLD AROUND - Find closest 'Mate' and closest 'Prey'		
                _owner.bNumNeighbours = 0;
                _owner.bInteractingNeighbours = 0;

                //Produtores
                for (int p = 0; p < CitizenFactory.listaProdutores.Count; p++)
                {
                    GajoWorker produtor = (GajoWorker)(CitizenFactory.listaProdutores[p]);
                    if (produtor != null)
                    {
                        float estaDistance = Vector3.Distance(_owner.transform.position, produtor.transform.position);
                        if (estaDistance < distanceToClosestProdutor)
                            if ((_owner.blueprint.dna[13] == produtor.dna[10]) ||
                                    (_owner.blueprint.dna[14] == produtor.dna[11]) ||
                                    (_owner.blueprint.dna[15] == produtor.dna[12]))
                            {
                                distanceToClosestProdutor = estaDistance;
                                closestProdutor = produtor;
                            }
                    }
                }
                //Gajos
                for (int i = 0; i < CitizenFactory.listaGajos.Count; i++)
                {
                    GajoCitizen gajo = (GajoCitizen)(CitizenFactory.listaGajos[i]);
                    if (gajo != null)
                    {
                        if ((gajo.getId() != _owner.getId()) &&
                            (gajo.iEstado == GajoCitizen.GajoState.VIVO))
                        {
                            float estaDistance = Vector3.Distance(_owner.transform.position, gajo.transform.position);
                            //PREY	
                            if (estaDistance < distanceToClosestPrey &&
                                !gajo.behavior.Interacao.isInteracting)
                                if (
                                    ((_owner.blueprint.dna[13] == gajo.blueprint.dna[10]) ||
                                     (_owner.blueprint.dna[14] == gajo.blueprint.dna[11]) ||
                                     (_owner.blueprint.dna[15] == gajo.blueprint.dna[12]))
                                    )
                                {
                                    distanceToClosestPrey = estaDistance;
                                    closestPrey = gajo;
                                }
                            //MATE
                            if (estaDistance < distanceToClosestMate &&
                                !gajo.behavior.Interacao.isInteracting)
                                if (
                                    ((_owner.blueprint.dna[13] == gajo.blueprint.dna[13]) && (_owner.blueprint.dna[14] == gajo.blueprint.dna[14])) ||
                                    ((_owner.blueprint.dna[15] == gajo.blueprint.dna[15]) && (_owner.blueprint.dna[14] == gajo.blueprint.dna[14])) ||
                                    ((_owner.blueprint.dna[13] == gajo.blueprint.dna[13]) && (_owner.blueprint.dna[15] == gajo.blueprint.dna[15]))
                                )
                                {
                                    distanceToClosestMate = estaDistance;
                                    closestMate = gajo;
                                }

                            //CONTA NEIGHBOURS
                            if (estaDistance < _owner.hitDistance + 1)
                                _owner.bInteractingNeighbours++;

                            if (estaDistance < _owner.proxemia)
                                _owner.bNumNeighbours++;
                        }//if gajo vivo
                    }//if gajo !=null	
                }


                //CHECKA CROWDedNESS OF CLOSEST PREY AND MATE
                //AND IF CLOSEST IS CROWDED GETS ANYONE ELSE AVAILABLE, ANYWHERE ELSE
                Gajo[] listGObjects = GameObject.FindObjectsOfType(typeof(Gajo)) as Gajo[];
                int layerGajos = LayerMask.NameToLayer("Gajos");
                int neighboursMate = 0;
                int neighboursPrey = 0;
                Gajo alternativaAoClosestMate = null;
                Gajo alternativaAoClosestPrey = null;
                foreach (Gajo go in listGObjects)
                {
                    if (go.gameObject.layer == layerGajos)
                    {
                        GajoCitizen castedGo = go as GajoCitizen;
                        if(closestMate!=null)
                        if (Vector3.Distance(go.transform.position, closestMate.transform.position) < _owner.walker.Navigation.agent.radius * 5)
                            neighboursMate++;
                        else
                        {
                            if (castedGo != null)
                                if (
                                 ((_owner.blueprint.dna[13] == castedGo.blueprint.dna[13]) && (_owner.blueprint.dna[14] == castedGo.blueprint.dna[14])) ||
                                 ((_owner.blueprint.dna[15] == castedGo.blueprint.dna[15]) && (_owner.blueprint.dna[14] == castedGo.blueprint.dna[14])) ||
                                 ((_owner.blueprint.dna[13] == castedGo.blueprint.dna[13]) && (_owner.blueprint.dna[15] == castedGo.blueprint.dna[15]))
                                 )
                                    alternativaAoClosestMate = go;
                        }
                        if (closestMate != null)
                            if (Vector3.Distance(go.transform.position, closestPrey.transform.position) < _owner.walker.Navigation.agent.radius * 5)
                            neighboursPrey++;
                        else
                        {
                            if (castedGo != null)
                                if (
                                 ((_owner.blueprint.dna[13] == castedGo.blueprint.dna[10]) ||
                                  (_owner.blueprint.dna[14] == castedGo.blueprint.dna[11]) ||
                                  (_owner.blueprint.dna[15] == castedGo.blueprint.dna[12]))
                                 )
                                    alternativaAoClosestPrey = go;
                        }
                    }
                }//for each gajo
                if (neighboursMate > 5)
                    closestMate = (GajoCitizen)alternativaAoClosestMate;
                if (neighboursPrey > 5)
                    closestPrey = (GajoCitizen)alternativaAoClosestPrey;

            }
		}
//---------------------------------------------------------------------------------------------------
//---------------------------------------------------------------------------------------------------
//ACCESSES THE SITUATION
//Activates the boolean flags (fome, heat, bPertoPrey, bPertoMate) 
//according to the surrounding environment and the neighbours
//---------------------------------------------------------------------------------------------------
        public void accessesSituation(){
            //            if(Global.optionMarchNormalVsRVO)   
            //                    bEmergency = true;
            if (_owner.iEstado == GajoCitizen.GajoState.VIVO)
            {
                bFome = false;
                bHeat = false;
                bPertoProdutor = false;
                bProdutorExists = false;
                bPertoPrey = false;
                bPreyExists = false;
                bPertoMate = false;
                bMateExists = false;
                bConhecido = false;
                bDesconhecido = false;
                bInteract = false;
                bNInteract = false;

                if (_owner.metabolism.energy < _owner.metabolism.gajoFomeEnergyThereshold)
                    bFome = true;
                else
                    bFome = false;

                if (_owner.metabolism.connectedness < _owner.metabolism.connectednessLevelForSuccessThreshold)
                    bHeat = true;
                else
                    bHeat = false;

                //HA PRODUTOR-------------------------------------------------- 
                if (Global.hasProdutores)
                {
                    bPertoProdutor = false;
                    bProdutorExists = false;
                    if (closestProdutor != null)
                    {
                        bProdutorExists = true;
                        //ESTA NEARBY 
                        if (distanceToClosestProdutor < _owner.behavior.Interacao.interactingDistance_max)
                        {
                            bProdutorExists = true;
                            bPertoProdutor = true;
                            bConhecido = true;
                            bNInteract = true;
                        }
                    }
                }
                //HA PREY-----------------------------------------------------
                bPertoPrey = false;
                bPreyExists = false;
                if (closestPrey != null)
                {
                    //ESTA EM EATING ZONE 
                    if (distanceToClosestPrey < _owner.behavior.Interacao.interactingDistance_max)
                    {
                        bPreyExists = true;
                        bPertoPrey = true;
                        bConhecido = false;
                        bDesconhecido = true;
                        foreach (IndividualRelatioship inter_id in Interacao.Memoria.listOfInteractionPartners)
                            if (closestPrey.getId() == inter_id.id)
                            {
                                bConhecido = true;
                                bDesconhecido = false;
                            }
                    }
                    else
                    {   //ESTA LONGE
                        bPreyExists = true;
                        bPertoPrey = false;
                    }
                }
                else
                {  //NAO HA PREY
                    bPreyExists = false;
                    bPertoPrey = false;
                }

                //HA MATE
                bPertoMate = false;
                bMateExists = false;
                if (closestMate != null)
                {
                    //ESTA EM MATING ZONE 
                    if (distanceToClosestMate < _owner.reproductiveDistance)
                    {
                        bMateExists = true;
                        bPertoMate = true;

                        bConhecido = false;
                        bDesconhecido = true;
                        foreach (IndividualRelatioship inter_id in Interacao.Memoria.listOfInteractionPartners)
                            if (closestMate.getId() == inter_id.id)
                            {
                                bConhecido = true;
                                bDesconhecido = false;
                            }
                    }
                    else
                    {
                        //ESTA LONGE
                        bMateExists = true;
                        bPertoMate = false;
                    }
                }
                else
                {
                    //NAO HA MATE
                    bMateExists = false;
                    bPertoMate = false;
                }
                if (myMarkov.currentState == 5 ||
                    myMarkov.currentState == 6 ||
                    myMarkov.currentState == 9 ||
                    myMarkov.currentState == 10)
                    bInteract = true;
                else
                    bNInteract = true;
            }
	}
        //---------------------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------------------
        //PERFORM ACTION  //Activate the actuators //Performs the actions and rewards accordigly
        //---------------------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------------------
        float valueRet =0;
        int previousPartnerID = -1;
        int qtInteractionsWithThisPartner = 0;
        public void performAction(int actionPerformed)
        {
//------------------------------
			_owner.behavior.actions.myanimations.interactionState = InteractionAnimations.MyState.REPOUSO;
//-------------------------------

//-------------------------------
//-------------------------------
//HACK PARA ESTAREM SEMPRE COM O MESMO PARCEIRO A COMEREM
            if (actionPerformed == 7)
                if (closestMate.getId() == previousPartnerID)
                {
                    qtInteractionsWithThisPartner++;
                    if (qtInteractionsWithThisPartner > 1)
                    {
                        actionPerformed = 3;
                    }
                }
                else
                {
                    qtInteractionsWithThisPartner = 0;
                    previousPartnerID = closestMate.getId();
                }
            if (actionPerformed == 10 || actionPerformed == 11)
                if (closestPrey.getId() == previousPartnerID)
                {
                    qtInteractionsWithThisPartner++;
                    if (qtInteractionsWithThisPartner > 1) actionPerformed = 3;
                }
                else
                {
                    qtInteractionsWithThisPartner = 0;
                    previousPartnerID = closestPrey.getId();
                }
//-----------------------------
//-----------------------------

            if (_owner.iEstado == GajoCitizen.GajoState.VIVO)
            {
                valueRet = 0;
             switch (actionPerformed)
                {
                    //REPOUSO//
                    case 0:
                        seEstaEmInteracaoFinalizaPoruqeEstaLonge();
                        _owner.behavior.actions.myanimations.interactionState = InteractionAnimations.MyState.REPOUSO;
                        _owner.behavior.actions.myanimations.anima();
                        distanciaObjectivo = 100;
                    
                        Interacao.isInteracting = false;
                        //--
                        Interacao.interactingPartner = null;
                        Interacao.Memoria.isCooperativeInteractionC = false;
                        Interacao.Memoria.typeOfInteraction = MemoryOfRelationships.TypeRelationship.VOID;
                        //--
                        Interacao.Memoria.totalInteracoesComEsteGajo = 0;
                        Interacao.Memoria.totalCooperativeActionComEsteGajo = 0;                       
                        break;
                    //MOVE MATE//
                    case 1:
                        //if (Vector3.Distance(owner.gameObject.transform.position, _owner.walker.target) < 0.01f)
                        //{//idle state se perto de destino (ou ainda nao arrancou e esta ali parado)
                        //    _owner.behavior.actions.myanimations.interactionState = InteractionAnimations.MyState.REPOUSO;
                        //    _owner.behavior.actions.myanimations.anima();
                        //    distanciaObjectivo = 100;
                        //}
                        seEstaEmInteracaoFinalizaPoruqeEstaLonge();
                        if (closestMate != null)//Exception para quando acabou de morrer
                            valueRet = actions.move(closestMate);
					    else 
							Debug.LogError("Move MATE mas sem closestMate "+ _owner.iEstado + "please increase the size of the population?");
                        Interacao.Memoria.setInteractionPartner(-1, -1, MemoryOfRelationships.TypeRelationship.VOID);
                        distanciaObjectivo = distanceToClosestMate;
                        Interacao.isInteracting = false;
                        Interacao.Memoria.totalInteracoesComEsteGajo = 0;
                        Interacao.Memoria.totalCooperativeActionComEsteGajo = 0;
                        break;
                    //MOVE PREY
                    case 2:
                        //if (Vector3.Distance(owner.gameObject.transform.position, _owner.walker.target) < 0.01f)
                        //{//idle state se perto de destino (ou ainda nao arrancou e esta ali parado)
                        //    _owner.behavior.actions.myanimations.interactionState = InteractionAnimations.MyState.REPOUSO;
                        //    _owner.behavior.actions.myanimations.anima();
                        //    distanciaObjectivo = 100;
                        //}
                        seEstaEmInteracaoFinalizaPoruqeEstaLonge();
                        if (Global.hasProdutores)
                        {
                            if (distanceToClosestProdutor < distanceToClosestPrey)
                            {
                                valueRet = actions.move(closestProdutor.transform.position);
                                distanciaObjectivo = distanceToClosestProdutor;
                            }
                            else
                            {
							if (closestPrey!= null)//Exception para quando acabou de morrer
                                    valueRet = actions.move(closestPrey);
							   else 
								Debug.LogError("MovePrey mas sem closestMate "+ _owner.iEstado);
                            distanciaObjectivo = distanceToClosestPrey;
                            }
                        }
                        else
                        {
                            if (closestMate != null)//Exception para quando acabou de morrer
                                valueRet = actions.move(closestPrey);
							else 
								Debug.LogError("MovePrey mas sem closestMate "+ _owner.iEstado);
                            distanciaObjectivo = distanceToClosestPrey;
                        }
                        Interacao.Memoria.setInteractionPartner(-1, -1, MemoryOfRelationships.TypeRelationship.VOID);
                        Interacao.isInteracting = false;
                        Interacao.Memoria.totalInteracoesComEsteGajo = 0;
                        Interacao.Memoria.totalCooperativeActionComEsteGajo = 0;
                        break;
                    default:
                    //WANDER//
                    case 3:
                        _owner.walker.target = _owner.walker.preferedPosition;
                        Interacao.isInteracting = false;
                        if (Vector3.Distance(owner.gameObject.transform.position,  _owner.walker.target) <0.01f)
                        {//idle state se perto de destino (ou ainda nao arrancou e esta ali parado)
                            _owner.behavior.actions.myanimations.interactionState = InteractionAnimations.MyState.REPOUSO;
                            _owner.behavior.actions.myanimations.anima();
                            distanciaObjectivo = 100;
                        }
                        seEstaEmInteracaoFinalizaPoruqeEstaLonge();
                      
                        valueRet = actions.wander();
                        actions.move(_owner.walker.target);
                        Interacao.Memoria.setInteractionPartner(-1, -1, MemoryOfRelationships.TypeRelationship.VOID);
                        distanciaObjectivo = 100;
                        Interacao.isInteracting = false;
                        Interacao.Memoria.totalInteracoesComEsteGajo = 0;
                        Interacao.Memoria.totalCooperativeActionComEsteGajo = 0;
                        
                        break;
				case 4: //Found mate                        
					distanciaObjectivo = distanceToClosestMate;
					Interacao.isInteracting = false;
					if (closestMate == null) {
						Debug.LogError ("~FOUND MATE mas sem closestMate " + _owner.iEstado);
                            seEstaEmInteracaoFinalizaPoruqeEstaLonge();
					}
                        break;
				case 5: //Conhecido
					if (closestMate != null) {//Exception para quando acabou de morrer
						Interacao.interactingPartner = closestMate;
						//Para gerar reação emocional-----------
						actions.attemptMate (closestMate, false);
						//--------------------------------------
						Interacao.Memoria.setInteractionPartner (closestMate.getId (), valueRet, MemoryOfRelationships.TypeRelationship.SOCIAL);
						_owner.behavior.actions.myanimations.interactionState = InteractionAnimations.MyState.CONHECIDO;
						_owner.behavior.actions.myanimations.anima ();
						distanciaObjectivo = 0;
						Interacao.isInteracting = true;
					} else {
						Debug.LogError ("CONHECIDO mas sem closestMate " + _owner.iEstado);
                            seEstaEmInteracaoFinalizaPoruqeEstaLonge();
					}
                        break;
				case 6: //Desconhecido 
//Debug.Log ("At performAction " + closestMate + _owner.getId());
					if (closestMate != null) {//Exception para quando acabou de morrer
						Interacao.interactingPartner = closestMate;
						//Para gerar reação emocional-----------
						actions.attemptMate (closestMate, false);
						//--------------------------------------
						Interacao.Memoria.setInteractionPartner (closestMate.getId (), valueRet, MemoryOfRelationships.TypeRelationship.SOCIAL);					
//Debug.Log ("Depois de definir interacao " + closestMate + _owner.getId() + Interacao.interactingPartner);
						_owner.behavior.actions.myanimations.interactionState = InteractionAnimations.MyState.DESCONHECIDO;
						_owner.behavior.actions.myanimations.anima ();                       
						distanciaObjectivo = 0;
						Interacao.isInteracting = true;
					} else {
						Debug.LogError ("DESCONHECIDO mas sem closestMate " + _owner.iEstado);
                            seEstaEmInteracaoFinalizaPoruqeEstaLonge();
					}
                        break;
                    //ATTEMPT MATE
				case 7:
						if (closestMate != null) {//Exception para quando acabou de morrer
							Interacao.Memoria.setInteractionPartner (closestMate.getId (), valueRet, MemoryOfRelationships.TypeRelationship.SOCIAL);
							valueRet = actions.attemptMate (closestMate, true);
							_owner.behavior.actions.myanimations.interactionState = InteractionAnimations.MyState.ATTEMPTMATE;
							_owner.behavior.actions.myanimations.anima ();

							setDistanciaObjectivo (0);
	                       
							distanciaObjectivo = 0;
							Interacao.isInteracting = true;
					}
                        break;
                    case 8: //Found prey
                        if (Global.hasProdutores)
	                        {
	                         if (distanceToClosestProdutor < distanceToClosestPrey)
	                            distanciaObjectivo = distanceToClosestProdutor;
	                         else
	                            distanciaObjectivo = distanceToClosestPrey;
	                         Interacao.isInteracting = false;
	                        }
	                        else
	                        {
	                         distanciaObjectivo = distanceToClosestPrey;
	                         Interacao.isInteracting = false;
	                        }
                        break;
				case 9: //Conhecido
					if (closestPrey != null) {//Exception se morreu entretanto
						Interacao.interactingPartner = closestPrey;
						//Para gerar reação emocional-----------
						actions.negoceia(closestPrey, false);
						//--------------------------------------
						Interacao.Memoria.setInteractionPartner (closestPrey.getId (), valueRet, MemoryOfRelationships.TypeRelationship.TRADE);
						_owner.behavior.actions.myanimations.interactionState = InteractionAnimations.MyState.PREYCONHECIDA;
						_owner.behavior.actions.myanimations.anima ();
                      
						distanciaObjectivo = 0;
						Interacao.isInteracting = true;
						} else {
							Debug.LogError ("Conhecido Prey Prey mas sem closestPrey" + _owner.iEstado);
                            seEstaEmInteracaoFinalizaPoruqeEstaLonge();
						}
                        break;
                    case 10: //Desconhecido 
                        if (closestPrey != null)
                        {
                        Interacao.interactingPartner = closestPrey;
						//Para gerar reação emocional-----------
						actions.negoceia(closestPrey,false);
						//--------------------------------------
                        _owner.behavior.actions.myanimations.interactionState = InteractionAnimations.MyState.PREYDESCONHECIDA;
                        _owner.behavior.actions.myanimations.anima();

                        distanciaObjectivo = 0;
                        Interacao.isInteracting = true;
						} else {
							Debug.LogError ("DesConhecido Prey mas sem closestPrey" + _owner.iEstado);
                            seEstaEmInteracaoFinalizaPoruqeEstaLonge();
						}
                        break;
                    //ATACA
                    case 11:
                        if (closestPrey != null)//Exception se entretanto morreu
                        {   Interacao.interactingPartner = closestPrey;
                            Interacao.Memoria.setInteractionPartner(closestPrey.getId(), valueRet, MemoryOfRelationships.TypeRelationship.TRADE);
                        }
                        else                         
                        {//idle state se nao tem partner
                            _owner.behavior.actions.myanimations.interactionState = InteractionAnimations.MyState.REPOUSO;
                            _owner.behavior.actions.myanimations.anima();
                            distanciaObjectivo = 100;
                        }
                        if (Global.hasProdutores)
                        {
                            if (distanceToClosestProdutor < distanceToClosestPrey)
                            {
                                _owner.behavior.actions.myanimations.interactionState = InteractionAnimations.MyState.ATACAPRODUTOR;
                                valueRet = actions.ataca(closestProdutor);
                            }
                            else
                            {   valueRet = actions.ataca(closestPrey);
                                _owner.behavior.actions.myanimations.interactionState = InteractionAnimations.MyState.ATACA;
                            }
                        }
                        else
                        {   valueRet = actions.ataca(closestPrey);
                            _owner.behavior.actions.myanimations.interactionState = InteractionAnimations.MyState.ATACA;
                        }
                        _owner.behavior.actions.myanimations.anima();
                         setDistanciaObjectivo(0);
                        Interacao.isInteracting = true;
                        break;
                }
            }
        }
//---------------------------------------------------------------------------------------------------
        private void seEstaEmInteracaoFinalizaPoruqeEstaLonge()
        {
            if (Interacao.isInteracting)
                //Large Distance => Stop Interaction
                if (Vector3.Distance(_owner.gameObject.transform.position,
                    Interacao.interactingPartner.transform.position) >
                    Interacao.interactingDistance_max)                 
                    { Interacao.stop(); }
        }

//---------------------------------------------------------------------------------------------------
    Vector3 v3Aux;
    float x, y, z;
    int time = 0;
   
    public void emotionalUpdate()
        {
           // Vector3 antesEmotion = psy.emotion;
    //        Vector3 antesMood = psy.mood;
            if (_owner.iEstado == GajoCitizen.GajoState.VIVO)
            {
                //EMOTION  
                float gamma = 0.55f;
                float delta = 0.45f;
                x = gamma*(rewards._mot)+delta*psy.mood.x;
                if (x > 1) x = 1;
                if (x < 0) x = 0;
                y = gamma*(rewards._urg)+ delta*psy.mood.y;
                if (y > 1) y = 1;
                if (y < 0) y = 0;
                z = 0;
				psy.emotion =  new Vector3(x, y, z);

                //MOOD
                float alpha = 0.95f;
                float beta = 0.05f;
                psy.mood = alpha * psy.mood + beta * (psy.emotion);
                time++;

                /*              string s = "";
                            if (actionPerformed == 6 || actionPerformed == 5 || actionPerformed == 7)
                             {
                              //Para Motivation  if (Interacao.Memoria.ganhoTradingOrSocializing > 1)
                              if (Interacao.Memoria.isCooperativeInteractionC)
                                 s += "GANHOU : ";
                                 else
                                     s += "PERDEU : ";
                       //          if (antesEmotion.x < psy.emotion.x)
                       //              s += "Emot: V SUBIU; ";
                       //          else
                       //              s += "Emot: V DESCEU; ";
                                 if (antesEmotion.y < psy.emotion.y)
                                     s += "Emot: A SUBIU; ";
                                 else
                                     s += "Emot: A DESCEU; ";
                       //          if (antesMood.x < psy.mood.x)
                       //              s += "Mood: V SUBIU; ";
                       //          else
                       //              s += "Mood: V DESCEU; ";
                                 if (antesMood.y < psy.mood.y)
                                     s += "Mood: A SUBIU;";
                                 else
                                     s += "Mood: A DESCEU;";
                                 Debug.Log(owner.behavior.actions.myanimations.interactionState + " " + s + 
                         //            " Val:" +
                         //            antesEmotion.x + "->"+ psy.emotion.x + " " +
                         //            antesMood.x + "->" + psy.mood.x +
                                     " Arou:" +
                                     antesEmotion.y + "->" + psy.emotion.y + " " +
                                     antesMood.y + "->" + psy.mood.y +
                                      " Mot:" + rewards._mot + " Aro" + rewards._urg);
                
            } */
            }
        }
//------------------------------------------------------------------------------------------------------------------------------
//Captura os dados no final de cada ciclo para fazer depois o Log com os dados todos certinhos
//------------------------------------------------------------------------------------------------------------------------------
        public void registaMemoria()
        {
    		snapshot.iEstado = _owner.iEstado;
            snapshot.position.x = owner.transform.position.x;
            snapshot.position.y = owner.transform.position.y;
            snapshot.position.z = owner.transform.position.z;
            snapshot.mood.x = psy.mood.x;
            snapshot.mood.y = psy.mood.y;
            snapshot.mood.z = psy.mood.z;

            snapshot.duracaoMediaInteracoes = Interacao.Memoria.duracaoMediaInteracoes;
            if (_owner.iEstado == GajoCitizen.GajoState.VIVO)
            {
                switch (actionPerformed)
                {
                    case 0: snapshot.action = "Still"; break;
                    case 1: snapshot.action = "Move"; break;
                    case 2: snapshot.action = "Move"; break;
                    case 3: snapshot.action = "Wander"; break;
                    case 4: snapshot.action = "Attempt_Social_Start"; break;
                    case 5: snapshot.action = "Attempt_Social_Conhecido"; break;
                    case 6: snapshot.action = "Attempt_Social_Desconhecido"; break;
				case 7:
					if (Interacao.Memoria.isCooperativeInteractionC)
					if (Interacao.Memoria.ganhoTradingOrSocializing > 1)
						snapshot.action = "Attempt_Social_Coop_Win";
					else { snapshot.action = "Attempt_Social_Coop_Lose"; }
                        else
                            snapshot.action = "Attempt_Social_NCoop";

                        break;
                    case 8: snapshot.action = "Trade_Begin"; break;
                    case 9: snapshot.action = "Trade_Conhecido"; break;
                    case 10: snapshot.action = "Trade_Desconhecido"; break;
				case 11:
					if (Interacao.Memoria.isCooperativeInteractionC)
					if (Interacao.Memoria.ganhoTradingOrSocializing > 1)
						snapshot.action = "Trade_Coop_Win";
					else { snapshot.action = "Trade_Coop_Lose"; }
                        else
                            snapshot.action = "Trade_NCoop";
                        break;
                    case 12:
                        snapshot.action = "Move";
                        break;
                }
            }
            else snapshot.action = "Move";

            snapshot.emotion.x = psy.emotion.x;
            snapshot.emotion.y = psy.emotion.y;
            snapshot.emotion.z = psy.emotion.z;
            snapshot.personality.x = psy.personality.x;
            snapshot.personality.y = psy.personality.y;
            snapshot.personality.z = psy.personality.z;
            snapshot.energy = _owner.metabolism.energy;
            snapshot.connectedness = _owner.metabolism.connectedness;
            snapshot.chemicals.x = _owner.metabolism.chemicals[0];
            snapshot.chemicals.y = _owner.metabolism.chemicals[1];
            snapshot.chemicals.z = _owner.metabolism.chemicals[2];
            snapshot.blueprint = _owner.blueprint.dna;
            snapshot.numeroInteracoes = Interacao.Memoria.numeroInteracoes;
            snapshot.totalInteractionsN = Interacao.Memoria.totalInteractionsN;
            snapshot.totalCooperativeInteractionsW = Interacao.Memoria.totalCooperativeInteractionsW;
            snapshot.typeOfInteraction = Interacao.Memoria.typeOfInteraction;
            snapshot.energyTolerance = _owner.blueprint.getEnergyTolerance();
            snapshot.actionPerformed = actionPerformed;
        }

		public void cleanMemoria()
		{
			snapshot.iEstado = _owner.iEstado;
			snapshot.position.x = owner.transform.position.x;
			snapshot.position.y = owner.transform.position.y;
			snapshot.position.z = owner.transform.position.z;
			snapshot.mood.x = psy.mood.x;//Foram reset com a personalidade quando tornou ZOMBIE
			snapshot.mood.y = psy.mood.y;//Foram reset com a personalidade quando tornou ZOMBIE
			snapshot.mood.z = psy.mood.z;//Foram reset com a personalidade quando tornou ZOMBIE

			snapshot.duracaoMediaInteracoes = Interacao.Memoria.duracaoMediaInteracoes;

			snapshot.action = "Move";

			snapshot.emotion.x = psy.emotion.x;//Foram reset com a personalidade quando tornou ZOMBIE
			snapshot.emotion.y = psy.emotion.y;//Foram reset com a personalidade quando tornou ZOMBIE
			snapshot.emotion.z = psy.emotion.z;//Foram reset com a personalidade quando tornou ZOMBIE
			snapshot.personality.x = psy.personality.x;
			snapshot.personality.y = psy.personality.y;
			snapshot.personality.z = psy.personality.z;
			snapshot.energy = _owner.metabolism.energy;
			snapshot.connectedness = _owner.metabolism.connectedness;
			snapshot.chemicals.x = _owner.metabolism.chemicals[0];
			snapshot.chemicals.y = _owner.metabolism.chemicals[1];
			snapshot.chemicals.z = _owner.metabolism.chemicals[2];
			snapshot.blueprint = _owner.blueprint.dna;
			snapshot.numeroInteracoes = 0;
			snapshot.totalInteractionsN = 0;
			snapshot.totalCooperativeInteractionsW = 0;
			snapshot.typeOfInteraction = MemoryOfRelationships.TypeRelationship.VOID;
			snapshot.energyTolerance = _owner.blueprint.getEnergyTolerance();
			snapshot.actionPerformed = 1;
		}
//------------------------------------------------------------------------------------------------------------------------------
        public void die()
		{   closestMate = null;
			closestPrey = null;
            if (_interacaoHandler != null)
                _interacaoHandler = null;
            psy.die ();
			psy=null;
			myMarkov.die ();
			myMarkov=null;
			rewards.die ();
			rewards=null;
            if (dispatcher != null)
                dispatcher.setOwner(null);
            if (dispatcher != null)
                dispatcher.die();   
            dispatcher = null;
            actions.die();
        }
//------------------------------------------------------------------------------------------------------------------------------
//------------------------------------------------------------------------------------------------------------------------------	
	public string showSensors()
		{string s="";
			s+= " FOME-" + bFome;
			s+= " HEAT-" + bHeat;
			s+= " PERTO_PREY-" + bPertoPrey;
			s+= " PREY_EXISTS-" + bPreyExists;
			s+= " PERTO_MATE-" + bPertoMate;
			s+= " MATE_EXISTS-" + bMateExists;
			s+= " CONHECIDO-" + bConhecido;
			s+= " DESCONHECIDO-" + bDesconhecido;
			s+= " INTERACT-" + bInteract;
			s+= " NINTERACT-" + bNInteract;
            s += " EMERGENCY-" + bEmergency;
            return s;
		}

    public string getDebugString()
        {
            debugString = "";
            debugString += "\n";
            debugString += "Time Step " + currentTimeT;
            debugString += "\n";
            debugString += "Personality: " + psy.personality.x + " " + psy.personality.y + " " + psy.personality.z;
            debugString += "\n-PERSONALITY------------------------------------------------------";
            debugString += "\nMood: " + psy.mood.x + " " + psy.mood.y + " " + psy.mood.z;
            debugString += "\n";
            debugString += "Emotion: " + psy.emotion.x + " " + psy.emotion.y + " " + psy.emotion.z;
            debugString += "\n";
            debugString += "\n-HISTORICAL---------------------------------------------------------\n";

            debugString += "Interactions w/:";
            foreach (IndividualRelatioship it in Interacao.Memoria.listOfInteractionPartners)
                debugString += "(" + it.id + "," + it.qtInteractions + "," + it.qtCooperations + ") ";
            debugString += "\n";
            debugString += "\n-LAST------------------------------------------------------------------\n";
            debugString += "Last action Performed: " + lastActionPerformed;
            debugString += "\n";
            debugString += "RTot: " + rewards.rTotal;
            debugString += "\n";
            debugString += "Mot: " + rewards._mot;
            debugString += "\n";
            
            debugString += "Urg: " + rewards._urg;
            debugString += "\n";
    //        debugString += "Con: " + rewards._con;
     //       debugString += "\n";
            debugString += "Dom: " + rewards._dom;
            debugString += "\n";
            debugString += "sucessoAccaoAnterior: " + sucessoAccaoAnterior;
            debugString += "Last actions: " + lastActionPerformed;
            debugString += "\n";
            debugString += "Fome: " + bFome + "; Heat: " + bHeat + "; bPertoPrey: " + bPertoPrey + "; bPertoMate: " + bPertoMate;
            debugString += "\n";
            return debugString;
        }
//------------------------------------------------------------------------------------------------------------------------------
//------------------------------------------------------------------------------------------------------------------------------
    }//eo class
}//eo namespace
