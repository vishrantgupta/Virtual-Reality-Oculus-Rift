//#define debugGuy
using UnityEngine;
using System.Collections;
namespace Crowd {
public class Rewards
	{	
		//---------------------------------------------------------------------------------------------------
		public float _mot=0;
		public float _urg=0;
	//	public float _con=0;
		public float _dom=0;
		public float mWeight=0.25f;
	    public float uWeight=0.25f;
	//	public float cWeight=0.25f;
		public float dWeight=0.25f;

		public string debugString="";
		public BehaviorAI _parent;	
		public float rTotal=0;
        private InteractionHandler _interaction;
		//---------------------------------------------------------------------------------------------------
		//---------------------------------------------------------------------------------------------------
		//---------------------------------------------------------------------------------------------------
		public Rewards (BehaviorAI parent)
		{_parent = parent;
         _interaction = _parent.Interacao;
        }
        //---------------------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------------------
        //Rewards last action that was performed
        public float calculatesAppraisal()
        {
            _mot = mot();
            _urg = urg();
            rTotal = _mot * mWeight + _urg * uWeight + _parent.sucessoAccaoAnterior;
            _parent.sucessoAccaoAnterior = 0;

            return rTotal;
        }
		//---------------------------------------------------------------------------------------------------
        public void determinesBonusReward(int lastActionPerformed, float valueReturned)		{
			_parent.sucessoAccaoAnterior = 0;
			//Ficou perto de Prey depois de andar 
			if (_parent.bPertoPrey && (lastActionPerformed == 2 || lastActionPerformed == 3))
				_parent.sucessoAccaoAnterior = valueReturned;
			else
				//Ficou perto de Mate depois de andar 
				if(_parent.bPertoMate && (lastActionPerformed== 1 || lastActionPerformed== 3))
					_parent.sucessoAccaoAnterior = valueReturned;
			else
				//Sucesso na interacao
				if(valueReturned>0)
					_parent.sucessoAccaoAnterior = valueReturned;
		}
		//---------------------------------------------------------------------------------------------------
		public float mot()	{//Vai afectar directamente o PLEASURE   
            float intrinsicFactor = 0;

            switch (_parent.Interacao.Memoria.typeOfInteraction)
            {
			//----------------------------------------------
			case MemoryOfRelationships.TypeRelationship.VOID:
                    intrinsicFactor = 1 / (float)_parent.distanciaObjectivo;                 
                    break;
		    //----------------------------------------------
			case MemoryOfRelationships.TypeRelationship.TRADE:
				//ForÅca um calculo das motivacoes, pois se for passivo pode estar a fazer outra coisa qualquer 
				if (_parent.Interacao.Memoria.ganhoTradingOrSocializing == 0 && _parent.Interacao.isInteracting)
				    if (_parent.Interacao.interactingPartner != null)
					    _parent.actions.negoceia (_parent.Interacao.interactingPartner, false);
                    /**/
                if (_parent.Interacao.Memoria.ganhoTradingOrSocializing != 0)
                        intrinsicFactor = (1 - 1 / _parent.Interacao.Memoria.ganhoTradingOrSocializing) * 0.85f;
                    else
                        intrinsicFactor = 0;
                break;
			//----------------------------------------------
			case MemoryOfRelationships.TypeRelationship.SOCIAL:
                //---------------------------------
                //Quando este e passivo forca um measurement
				if (_parent.Interacao.Memoria.ganhoTradingOrSocializing == 0 && _parent.Interacao.isInteracting)
				if (_parent.Interacao.interactingPartner != null) 
					_parent.actions.attemptMate (_parent.Interacao.interactingPartner, false);
                    /**/
                float connectedness = _parent.owner.metabolism.connectedness;
                if (connectedness < 0) connectedness = 0;

                float theta=0;
                    if (_parent.Interacao.Memoria.isCooperativeInteractionC)
                        theta = 1;
                intrinsicFactor = theta*(1 - Mathf.Pow((1 / 10), (connectedness / _parent.owner.metabolism.connectednessLevelForSuccessThreshold)));
                    
				break;
            }
            
			if (intrinsicFactor > 1)
				intrinsicFactor = 1;
			if (intrinsicFactor < 0)
				intrinsicFactor = 0;
            return intrinsicFactor;
		}
//-----------		
		public float urg()	{
			float connectedness= _parent.owner.metabolism.connectedness;
			float energy = _parent.owner.metabolism.energy;
			if(connectedness<0) connectedness=0;
			if(energy<0) energy=0;

            float intrinsicFactor = 0;
            switch(_parent.Interacao.Memoria.typeOfInteraction)
                { case MemoryOfRelationships.TypeRelationship.VOID:            	
				    intrinsicFactor= Mathf.Pow(0.25f, connectedness / _parent.owner.metabolism.connectednessLevelForSuccessThreshold) +
                                     Mathf.Pow(0.25f, energy / _parent.owner.metabolism.gajoFomeEnergyThereshold);

                    break;
			case MemoryOfRelationships.TypeRelationship.SOCIAL:
				    intrinsicFactor=
					2f*Mathf.Pow (0.25f,connectedness / _parent.owner.metabolism.connectednessLevelForSuccessThreshold);
                  break;
			case MemoryOfRelationships.TypeRelationship.TRADE:
					intrinsicFactor=
					2f*Mathf.Pow (0.25f, energy / _parent.owner.metabolism.gajoFomeEnergyThereshold);
				break;   
		      }
            if (intrinsicFactor > 1)
                intrinsicFactor = 1;
            if (intrinsicFactor < 0)
                intrinsicFactor = 0;
            return intrinsicFactor;
		}		

        //-----------
        //-----------
        //-----------
        public void die()
		{
			_parent = null;
            _interaction = null;
		}
        //-----------        
    }//eo class
}//eo namespace

