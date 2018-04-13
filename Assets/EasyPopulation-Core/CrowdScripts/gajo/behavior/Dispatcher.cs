//#define debug_interactions
//#define debugGuy
using UnityEngine;
using System.Collections;

namespace Crowd {
public class Dispatcher:MonoBehaviour
{	
	private GajoCitizen _owner =null; 
	private bool isDispatcherActive=false;

	public int lastActionPerformed=-1;
//RFA-delegate-inicio (ver tambem IntractionHandler que e onde sao chamados)
    public delegate void StopInteractionHandler();
    public static event StopInteractionHandler requestStopInteraction;
//RFAfim
//RFA-delegate- Inicio (ver la em baixo - passa o: owner, partner)
        // private void PublishStopInteraction(Gajo g1, Gajo g2)
        private void PublishStopInteractionOfPartner(GajoCitizen g1)
        { if (requestStopInteraction != null) { requestStopInteraction(); } }
//RFAfim
        //------------------------------------		
        //------------------------------------			
        public void setOwner (GajoCitizen owner)
	{
		_owner=owner;
	}
//------------------------------------			
	public IEnumerator init()
		{
        isDispatcherActive =true;
		
		yield return new WaitForSeconds(1f);

		while (true && _owner!=null)			
		if(_owner.iEstado== GajoCitizen.GajoState.VIVO)
		{	
				#if debug_interactions
				if(owner.getId ()==Global.whoIsBeingTracked) print("***************************antes" + owner.interactionState + " " +owner.dispatcherSpeed + " " + Time.time);
				#endif
				//--
				switch(_owner.behavior.Interacao.interactionState)
				{case InteractionHandler.InteractionState.NEPIA://estado normal
						#if debug_interactions
						if(owner.getId ()==Global.whoIsBeingTracked) print("***0 LIVRE " + Time.time);
						#endif
						//the scheduler will suspend the execution until the yielded coroutine Dispatch has finished.
						yield return StartCoroutine(dispatch());							
						break;
			
				case InteractionHandler.InteractionState.WANTS_TO_GET_LINK://estado comecou animacao Interacao
					#if debug_interactions
					if(owner.getId ()==Global.whoIsBeingTracked) print("***1 ANIMA " + Time.time);
					#endif
					yield return new WaitForSeconds(_owner.behavior.dispatcherSpeed);
					_owner.behavior.Interacao.interactionState = InteractionHandler.InteractionState.IN_LINK;				
					break;

				case InteractionHandler.InteractionState.IN_LINK://estado quer terminar Interacao
					#if debug_interactions
					if(owner.getId ()==Global.whoIsBeingTracked) print("***2 PROCURA FECHO " + owner.interactionPartner+ Time.time);	
					#endif
					_owner.behavior.Interacao.interactionState = InteractionHandler.InteractionState.NEPIA;
                    if (_owner.behavior.Interacao.isInteracting)
                            {
                               // _owner.behavior.Interacao.stop();
                                //RFA-delegate-inicio
                                if (_owner.behavior.Interacao.interactingPartner != null)
                                {
                                    _owner.behavior.Interacao.interactingPartner.stopInteraction();
                                  }
                            }
					_owner.stopInteraction();
					#if debug_interactions
							if(owner.getId ()==Global.whoIsBeingTracked) print("***(pos a 0) FECHOU " + Time.time + " " + owner.interactionPartner);
					#endif				
					yield return new WaitForSeconds(1.0f);							
					break;
				}
				#if  debug_interactions
				if(owner.getId ()==Global.whoIsBeingTracked) print("*********************************************depois" + Time.time);
				#endif
				yield return null;
		}	 		
	}

//---------------------------------------------------------------------------------------------
//---------------------------------------------------------------------------------------------
//---------------------------------------------------------------------------------------------
//Avalia a situacao (com o recalcula vizinhanca) e activa flags que descrevem o quadro (fome-heat-bPreyExists-bPertoPrey-bMateExists-bPertoMate)
		IEnumerator dispatch()		{
            if (_owner.iEstado == GajoCitizen.GajoState.VIVO) {
                overlaysLocomotionWithEmotion ();
                if(_owner.walker!=null)
                   _owner.anim.SetFloat ("value_MoodArousal", _owner.behavior.psy.mood.y);
                if(_owner.behavior!=null )                 
                   _owner.behavior.processBehavior ();
			}
			yield return new WaitForSeconds(_owner.behavior.dispatcherSpeed);
		}//end dispatch
//-----------------------------------------------------------------------------------------------
	
		private void overlaysLocomotionWithEmotion()
		{
            if (_owner.behavior != null)//pode ter morrido neste instante
            {
                //Parametros de Locomocao sao ajustados de acordo com a mood e dados pelo AROUSAL
                if (_owner.behavior.psy.mood.x > 0)
                {
                    if (_owner.behavior.psy.mood.y > 0)
                    {
                        if (_owner.behavior.psy.mood.z > 0)
                            _owner.behavior.exuberance = _owner.behavior.psy.mood.y; //exuberant (+++)
                        else
                        { }//dependent (++-)
                    }
                    else
                    {
                        if (_owner.behavior.psy.mood.z > 0)
                            _owner.behavior.relaxation = _owner.behavior.psy.mood.y; //relaxed (+-+}
                        else
                        { } //docile (+--)
                    }
                }
                else
                {
                    if (_owner.behavior.psy.mood.y > 0)
                    {
                        if (_owner.behavior.psy.mood.z > 0)
                        { } //hostile (-++)
                        else
                            _owner.behavior.relaxation = _owner.behavior.psy.mood.y; ; //anxious (-+-)
                    }
                    else
                    {
                        if (_owner.behavior.psy.mood.z > 0)
                        { } //disdainful (-+-)
                        else
                            _owner.behavior.exuberance = _owner.behavior.psy.mood.y; //bored (---)
                    }
                }
                if(_owner.walker!=null)
                    _owner.walker.Gait.adjust(_owner.behavior.exuberance, _owner.behavior.relaxation);
            }
//ajustamos os parametros de interacao pelos parametros de locomocao (raios tem que estar sincronizados)
//deprecated			owner.eatingDistance=owner.walker.Navigation.Radius+1.5f;
//deprecated			owner.hitDistance=owner.walker.Navigation.Radius+1.5f;
//deprecated			owner.reproductiveDistance=owner.walker.Navigation.Radius+1.5f;
		}


        //----------------------------------------------------------------------
        //----------------------------------------------------------------------	
        private void hitTest()
	    {	
		int contaProximos=0;
		_owner.bNumNeighbours = 0;
		_owner.bInteractingNeighbours = 0;

		foreach (GajoCitizen gajo in CitizenFactory.listaGajos)
		    {
			if (_owner ==null) {Destroy (this); return;}
			else
			if(gajo!=null)			
				if (gajo.getId()!=_owner.getId()) 
			     {
			      float dist = Vector3.Distance(_owner.transform.position, gajo.transform.position);
				  if(dist<_owner.hitDistance)
					{//newActiveMessage("11111101", 0, gajo.transform);
							Debug.Log ("collision");
					 }

				  if(dist<_owner.hitDistance+1)
						  _owner.bInteractingNeighbours++;

				  if(dist < _owner.proxemia) 
						{contaProximos++;  
						 _owner.bNumNeighbours++;
						}
				 }			   
			 }	
		 }	
//------------------------------------
//------------------------------------
//------------------------------------
//------------------------------------					
   public void enable()
	{	isDispatcherActive=true;    }			
//------------------------------------				
	public void disable()
	{	isDispatcherActive=false;	}	
//------------------------------------				
	public bool isActive()
	{  return isDispatcherActive;  }	
//------------------------------------									
	public void die()	{      
		    isDispatcherActive=false;
			_owner=null;
		}
	}//eo classe				
}//eo namespace


