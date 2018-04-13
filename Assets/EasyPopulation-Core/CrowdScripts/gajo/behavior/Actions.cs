using UnityEngine;
using System.Collections;
namespace Crowd {
    public class Trade {
        public GajoCitizen _g1, _g2;
        public Trade(GajoCitizen g1, GajoCitizen g2) { _g1 = g1; _g2 = g2; }
    
    }
    public class Socialize {
        public GajoCitizen _g1, _g2;
        public Socialize (GajoCitizen g1, GajoCitizen g2) { _g1 = g1; _g2 = g2; }
    }

    public class Actions{
		//ATENCAO
		//ATENCAO
		//ATENCAO
		public enum TypeOfActionBeingPerformed {NEPIA, MOVE, WANDER,
			ATACA, ATACA_COOPERA, ATACA_COOPERA_WIN, ATACA_COOPERA_LOSE, ATACA_NAOCOOPERA,
			ATTEMPT_MATE, ATTEMPT_MATE_COOPERA,
			ATTEMPT_MATE_NAOCOOPERA,
			REPRODUZ, 
			COME_GAJO, 
			COME_PRODUTOR}
        //ACHO QUE È REDUNDANTE DADO QUE SABEMOS NO MARKOV O QUE E QUE ELE ESTA A FAZER 

        //ATENCAO
        //ATENCAO
        //ATENCAO

        GajoCitizen _owner;
        public  InteractionAnimations myanimations;
        public Actions(GajoCitizen owner)
        {
            _owner = owner;
            myanimations = new InteractionAnimations(_owner);
        }



//-------------------------------------------------------------------------------------	
//MOVE --------------------------------------------------------------------------------	
//-------------------------------------------------------------------------------------				
        public float move(Vector3 posicaoObjecto) {
            return makeMovement(posicaoObjecto);
		}
        public float move(GajoCitizen posicaoObjecto)    {
            if (posicaoObjecto != null)//Exception se morreu entretanto
                return makeMovement(posicaoObjecto.transform.position);
            else    {   return 0;
            }
        }
        //-------------------------------------------------------------------------------------				
        public float makeMovement(Vector3 posicaoObjecto)
        {
          if (_owner.getId() == Global.whoIsBeingTracked)
                Global.whatAmIDoing = TypeOfActionBeingPerformed.MOVE;
            _owner.whatAmIDoing = TypeOfActionBeingPerformed.MOVE;
            
            float reward = 0;
            if (posicaoObjecto != Vector3.zero)
            {               
                _owner.walker.target = posicaoObjecto;
                _owner.metabolism.energy -= _owner.priceMovement;
                if(_owner.behavior.psy.mood.x < -0.5 &&
                    _owner.behavior.psy.mood.y > 0.5 &&
                    _owner.behavior.psy.mood.z < -0.5  )
                    _owner.walker.Navigation.startRunning();
                else
                    _owner.walker.Navigation.startWalking();
                _owner.behavior.dispatcherSpeed = 1f;
            }
            else
                wander();
            return reward;
        }
       
//-------------------------------------------------------------------------------------				
//WANDER-------------------------------------------------------------------------------				
//-------------------------------------------------------------------------------------					
		public float wander()	
		{
         if (_owner.getId ()==Global.whoIsBeingTracked) 
			Global.whatAmIDoing = TypeOfActionBeingPerformed.WANDER;
		 _owner.whatAmIDoing = TypeOfActionBeingPerformed.WANDER;
		 float reward = 0;    
    
         if (Vector3.Distance (_owner.transform.position, _owner.walker.preferedPosition) < 15 * _owner.walker.Navigation.RAIO) {
               _owner.walker.getNextGoalPoint();
			}
	     _owner.walker.target=_owner.walker.preferedPosition;
         move (_owner.walker.preferedPosition);
//--//
         _owner.walker.Navigation.startWalking();
//--//
         _owner.metabolism.energy -= _owner.priceMovement;									
		 reward = -0.01f;
		 _owner.behavior.dispatcherSpeed=5f;
		return reward;	
		}

//----------------------------------------------------------------------------------------				
//ATACA-GAJO-----------------------------------------------------------------------------				
//----------------------------------------------------------------------------------------				
        public float ataca(GajoCitizen prey)
        {float reward = 0;
         if (_owner.getId() == Global.whoIsBeingTracked)
                Global.whatAmIDoing = TypeOfActionBeingPerformed.ATACA;
         _owner.whatAmIDoing = TypeOfActionBeingPerformed.ATACA;            
         _owner.walker.target = prey.transform.position;

            if (prey.iEstado == GajoCitizen.GajoState.VIVO &&
                _owner.iEstado == GajoCitizen.GajoState.VIVO)
            {
                if (prey.walker.Animacao != null)
                {
                    _owner.behavior.Interacao.isInteracting = true;
                    prey.behavior.Interacao.isInteracting = true;
                    prey.whatAmIDoing = TypeOfActionBeingPerformed.ATACA_COOPERA;
                    //**//
                    reward = negoceia(prey, true);
                    //**//
                    if (reward > 0)
                    {
                        myanimations.interactionState = InteractionAnimations.MyState.ATACK_YESinitiative_WIN;
                        prey.behavior.actions.myanimations.interactionState = InteractionAnimations.MyState.ATACK_NOinitiative_WIN;
                        _owner.whatAmIDoing = TypeOfActionBeingPerformed.ATACA_COOPERA;
                        _owner.walker.Animacao.enabled = true;
                        _owner.walker.target = _owner.transform.position;
                        _owner.behavior.Interacao.Memoria.isCooperativeInteractionC = true;
                        prey.behavior.Interacao.Memoria.isCooperativeInteractionC = true;
                    }
                    else
                    {
                        myanimations.interactionState = InteractionAnimations.MyState.ATACK_YESinitiative_LOSE;
                        prey.behavior.actions.myanimations.interactionState = InteractionAnimations.MyState.ATACK_NOinitiative_LOSE;
                        _owner.whatAmIDoing = TypeOfActionBeingPerformed.ATACA_NAOCOOPERA;
                        _owner.behavior.Interacao.Memoria.isCooperativeInteractionC = false;
                        prey.behavior.Interacao.Memoria.isCooperativeInteractionC = false;
                    }
                    float durationInteraction = 10f - (_owner.behavior.relaxation * 10f * 0.25f);
                    _owner.startInteraction(prey, durationInteraction);
                }

                myanimations.anima();
                prey.behavior.actions.myanimations.anima();
            }
            else reward = 0;
            _owner.metabolism.energy -= _owner.priceAttack;
            return reward;
        }
        //-----------------------------------------------------------------------------------				
        //COME GAJO--------------------------------------------------------------------------	
        //-----------------------------------------------------------------------------------				
        struct PotencialNegocio{ public float index_I; public float quantidade_I;
                                 public float index_J; public float quantidade_J;
        }
        public float negoceia(GajoCitizen prey, bool isTradingForReal)
        {
            float reward = 0;
            float overplusThreshold_I =  _owner.metabolism.gajoFomeEnergyThereshold *0.25f;
            float needThreshold_I = 1000;//  _owner.metabolism.gajoFomeEnergyThereshold * 2.00f;
            float overplusThreshold_J =  prey.metabolism.gajoFomeEnergyThereshold  * 0.25f;
            float needThreshold_J = 1000;// prey.metabolism.gajoFomeEnergyThereshold * 2.00f;
            PotencialNegocio[] listaDePotenciais = new PotencialNegocio[3];
            int indiceNaLista = 0;
            int totalOfTradedResources = 0;
            if (prey != null)
            {
                indiceNaLista = 0;
//MARCA OS SURPLUS DO I, em que o J tem falta
                for (int idx = 0; idx < 3; idx++)
                {
                    PotencialNegocio negocio = new PotencialNegocio();
                    negocio.index_I = -1;
                    negocio.index_J = -1;
                    negocio.quantidade_I = 0;
                    negocio.quantidade_J = 0;
//Marca Surplus de I com correspondencia na necessidade de J
                    if (_owner.metabolism.chemicals[idx] != -1 && prey.metabolism.chemicals[idx] != -1)
                    {
                        if (_owner.metabolism.chemicals[idx] > overplusThreshold_I)//Se i tem surplus                          
                            if (prey.metabolism.chemicals[idx] < needThreshold_J) //Se j tem necessidade 
                            {
                                negocio.index_I = idx;
                                negocio.quantidade_I = _owner.metabolism.chemicals[idx] - (overplusThreshold_I);
                                listaDePotenciais[indiceNaLista++] = negocio;
                            }
                    }
                }
               
//MARCA OS surplus Do J na lista
                if (listaDePotenciais.Length > 0)
                {
                for (int idx = 0; idx < 3; idx++)
                    {
                     if (_owner.metabolism.chemicals[idx] != -1 && prey.metabolism.chemicals[idx] != -1)
                        {
                         if (prey.metabolism.chemicals[idx] > overplusThreshold_J)//Se j tem surplus
                              if (_owner.metabolism.chemicals[idx] < needThreshold_I) //Se i tem necessidade 
                                  //vamos ver se encontramos um registo vazio para preencher
                                  for (int i_index = 0; i_index < listaDePotenciais.Length; i_index++)
                                    {  
                                     if (listaDePotenciais[i_index].index_J == -1)//se ainda esta em branco
                                        {
                                            listaDePotenciais[i_index].index_J = idx;
                                            listaDePotenciais[i_index].quantidade_J = prey.metabolism.chemicals[idx] - (overplusThreshold_J);
                                        }
                                    }
                        }
                    }
                }
//FAZ TRANSACOES
                for (int i = 0; i < listaDePotenciais.Length; i++) {
                    PotencialNegocio pn = listaDePotenciais[i];
                    if (listaDePotenciais[i].index_I == -1 || listaDePotenciais[i].index_J == -1)
                        break;
                    int amount = 0;
                    if (pn.quantidade_I > pn.quantidade_J)
                            amount = (int)pn.quantidade_J;
                    else amount = (int)pn.quantidade_I;
                    totalOfTradedResources += amount;

                    if (isTradingForReal)
                     {                    
                      //Carrega traded resources que sao oferecidas pelo partner
                      _owner.metabolism.chemicals[(int)pn.index_J] += amount;
                      prey.metabolism.chemicals[(int)pn.index_I] += amount;

                      //Remove traded resources from repositories                 
                      _owner.metabolism.chemicals[(int)pn.index_I] -= amount;
                      prey.metabolism.chemicals[(int)pn.index_J] -= amount;
                      }
                }               
//CARREGAR MOTIVACAO
                _owner.behavior.Interacao.Memoria.ganhoTradingOrSocializing = totalOfTradedResources;
                prey.behavior.Interacao.Memoria.ganhoTradingOrSocializing = totalOfTradedResources;
            }
     
        if(totalOfTradedResources>0)
            reward = 1- 1/(float)totalOfTradedResources;
        return reward;
        }

        //--------------------------------------------------------------------------------------
        //ATACA PRODUTOR--------------------------------------------------------------------------------------
        //-------------------------------------//
        //Chamado em BehaviorAI.performAction()
        public float ataca(GajoWorker produtor)
        {//Ficou identico ao comeGajo
            if (_owner.getId() == Global.whoIsBeingTracked)
                Global.whatAmIDoing = TypeOfActionBeingPerformed.COME_PRODUTOR;
            _owner.whatAmIDoing = TypeOfActionBeingPerformed.COME_PRODUTOR;
            float reward = 0;

            if (produtor != null)
            {
				reward = comeProdutor (produtor);
            }
            return reward;
        } 

		private float comeProdutor(GajoWorker produtor)
		{   float total = 0;
			float amount=0;
			float dose = 10;
			float reward = 0;
			float qtResourcesQueTinha = 0;
			myanimations.anima();
			produtor.startInteraction(_owner);
			_owner.startInteraction(produtor, 0.05f);
		
			if (_owner.metabolism.chemicals[0] != -1)
			{
				qtResourcesQueTinha += _owner.metabolism.chemicals [0];//RFA REWARD------------------------------------------------------------------
				if (produtor.chemicals[1]>dose)
				{
					amount = produtor.chemicals[1] - dose;
					produtor.chemicals[1] -= dose;
				} else
				{ amount = produtor.chemicals[1];
					produtor.chemicals[1]=0;
				}
				_owner.metabolism.chemicals[0] += amount;
				total = total + amount;                    
			}

			if (_owner.metabolism.chemicals[1] != -1)
			{
				qtResourcesQueTinha += _owner.metabolism.chemicals [1];//RFA REWARD------------------------------------------------------------------
				if (produtor.chemicals[2] > dose)
				{
					amount = produtor.chemicals[2] - dose;
					produtor.chemicals[2] -= dose;
				}
				else
				{
					amount = produtor.chemicals[2];
					produtor.chemicals[2] = 0;
				}
				_owner.metabolism.chemicals[1] += amount;
				total = total + amount;                    
			}
			if (_owner.metabolism.chemicals[2] != -1)
			{
				qtResourcesQueTinha += _owner.metabolism.chemicals [2];//RFA REWARD------------------------------------------------------------------
				if (produtor.chemicals[0] > dose)
				{
					amount = produtor.chemicals[0] - dose;
					produtor.chemicals[0] -= dose;
				}
				else
				{
					amount = produtor.chemicals[0];
					produtor.chemicals[0] = 0;
				}
				_owner.metabolism.chemicals[2] += amount;
				total = total + amount;
			}
			reward += total;
			_owner.behavior.dispatcherSpeed = 12f;
			_owner.walker.Animacao.enabled = true;

			if(qtResourcesQueTinha>0)//RFA REWARD----------------------------------------------------------------------------------------------------
				_owner.behavior.Interacao.Memoria.ganhoTradingOrSocializing = amount / qtResourcesQueTinha;//RFA REWARD-------------------------------------------
			else //RFA REWARD------------------------------------------------------------------------------------------------------------------------
				_owner.behavior.Interacao.Memoria.ganhoTradingOrSocializing = amount / 0.01f;//RFA REWARD----------------------------------------------------------
			return reward;
		}
//--------------------------------------------------------------------------------------
//--------------------------------------------------------------------------------------
//--------------------------------------------------------------------------------------
//--------------------------------------------------------------------------------------

//--------------------------------------------------------------------------------------
//--------------------------------------------------------------------------------------
//--------------------------------------------------------------------------------------
//--------------------------------------------------------------------------------------

//--------------------------------------------------------------------------------------					
//ATTEMPT TO MATE-----------------------------------------------------------------------
//--------------------------------------------------------------------------------------
		public float attemptMate(GajoCitizen closestMate, bool isSocializingForReal)
        { 
		_owner.whatAmIDoing = TypeOfActionBeingPerformed.ATTEMPT_MATE;
        float reward = 0;
		
		if (closestMate != null)
             if (closestMate.iEstado == GajoCitizen.GajoState.VIVO &&
                              _owner.iEstado == GajoCitizen.GajoState.VIVO)
              {//Exception se entretanto o outro morreu_owner.walker.target = closestMate.transform.position;
		      closestMate.whatAmIDoing = TypeOfActionBeingPerformed.ATTEMPT_MATE;
              float other_excessiveBonding = closestMate.metabolism.connectednessLevelForSuccessThreshold * 2f;
              if ( closestMate.metabolism.connectedness < other_excessiveBonding )
                {//DIALOGAM
                 //-------------//  
                 //Mensagens
                    _owner.behavior.actions.myanimations.interactionState = InteractionAnimations.MyState.MATE_YESinitiative_WIN;
                    if (closestMate.walker.Animacao != null)
                        closestMate.behavior.actions.myanimations.interactionState = InteractionAnimations.MyState.MATE_NOinitiative_WIN;
                    _owner.whatAmIDoing = TypeOfActionBeingPerformed.ATTEMPT_MATE_COOPERA;
                    closestMate.whatAmIDoing = TypeOfActionBeingPerformed.ATTEMPT_MATE_COOPERA;
                    _owner.behavior.Interacao.Memoria.isCooperativeInteractionC = true;
                    closestMate.behavior.Interacao.Memoria.isCooperativeInteractionC = true;
                    //-------------// 
                    float durationInteraction1 = 5f - (_owner.behavior.relaxation * 10f * 0.25f);
                     _owner.startInteraction(closestMate, durationInteraction1);
                     _owner.behavior.Interacao.isInteracting = true;
                     closestMate.behavior.Interacao.isInteracting = true;
                     _owner.behavior.dispatcherSpeed = 2.5f;
                     reward = 1;
                //     _owner.behavior.Interacao.Memoria.ganhoTradingOrSocializing = 1;
                //     closestMate.behavior.Interacao.Memoria.ganhoTradingOrSocializing = 1;
                        //-------------// 
                        if (isSocializingForReal)
                        {
                            reproduz();
                            _owner.metabolism.connectedness += _owner.metabolism.boostConnectednessWithEncounters;
                            closestMate.metabolism.connectedness += closestMate.metabolism.boostConnectednessWithEncounters;
                        }
                    }
                else
                {//NAO DIALOGAM
                    //Mensagens
                    _owner.behavior.actions.myanimations.interactionState = InteractionAnimations.MyState.MATE_YESinitiative_LOSE;
                    if (closestMate.walker.Animacao != null)
                        closestMate.behavior.actions.myanimations.interactionState = InteractionAnimations.MyState.MATE_NOinitiative_LOSE;
                    _owner.whatAmIDoing = TypeOfActionBeingPerformed.ATTEMPT_MATE_NAOCOOPERA;
                    closestMate.whatAmIDoing = TypeOfActionBeingPerformed.ATTEMPT_MATE_NAOCOOPERA;
                    _owner.behavior.Interacao.Memoria.isCooperativeInteractionC = false;
                    closestMate.behavior.Interacao.Memoria.isCooperativeInteractionC = false;
                    _owner.behavior.Interacao.isInteracting = false;
                    closestMate.behavior.Interacao.isInteracting = false;
                    reward = 0;
             //       _owner.behavior.Interacao.Memoria.ganhoTradingOrSocializing = 0;
             //       closestMate.behavior.Interacao.Memoria.ganhoTradingOrSocializing = 0;
                    }
             
             //reset walking target
		      _owner.walker.target = _owner.transform.position; 
		      _owner.walker.Animacao.enabled = true;

     
			    
			}//closestMate!=null

			//--------------------------------------------------
			//temos que forçar aqui porque closestMate é passivo
			closestMate.behavior.registaMemoria (); 
			//-------------------------------------------------			
          return reward;
        }

//----------------------------------------------------------------------------------				
//REPRODUZ--------------------------------------------------------------------------			
//----------------------------------------------------------------------------------				
        public float reproduz()		
		{	if(_owner.getId ()==Global.whoIsBeingTracked) 	
				Global.whatAmIDoing = TypeOfActionBeingPerformed.REPRODUZ;
            _owner.whatAmIDoing = TypeOfActionBeingPerformed.REPRODUZ;
			float reward = 0;
            _owner.metabolism.energy -= 5;				
            _owner.behavior.dispatcherSpeed =2f;
            _owner.walker.Animacao.enabled = true;
			return reward;
		}	
	
//-----------------------------------------------------------------------------------				
//MOVE TO MATE-----------------------------------------------------------------------				
//-----------------------------------------------------------------------------------				
		public static float moveToMate(GajoCitizen owner, Transform objecto) { 
			float reward = 0;	
			owner.walker.target=objecto.position ;				
			owner.metabolism.energy -= owner.priceMovement;									
			reward +=0.01f;
        	return reward;
		}
//---------------------------------------------------------------------------
        public void die()
        {
            _owner = null;
            myanimations = null;
        }
    }//eo class
}//eo package
