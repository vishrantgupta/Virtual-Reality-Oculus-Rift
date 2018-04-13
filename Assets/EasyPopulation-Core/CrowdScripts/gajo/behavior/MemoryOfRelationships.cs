using UnityEngine;
using System.Collections;
namespace Crowd
{
    public class IndividualRelatioship
    {
        public int id;
        public int qtInteractions;
        public int qtCooperations;
        public int[] dna= new int[DNA.DNA_LENGHT];
    }
    //--//
    public class MemoryOfRelationships
    {
        public int totalInteractionsN = 0;   //Numero total das interacoes
        public int totalCooperativeInteractionsW = 0; //Numero total de interacoes Cooperativas
        public bool isCooperativeInteractionC = false;
        public int totalCooperativeActionComEsteGajo = 0;//momentary info da classe Interaction
        public int totalInteracoesComEsteGajo = 0;//momentary info da classe Interaction
        
        public int idInteractionPartner = -1;
        public float duracaoMediaInteracoes = 0;
        public int numeroInteracoes = 0;

        public ArrayList listOfInteractionPartners = new ArrayList();
        public enum TypeRelationship { VOID, SOCIAL, TRADE};
        public TypeRelationship typeOfInteraction = TypeRelationship.VOID;
		public float ganhoTradingOrSocializing = 0;
        //---------------------------------------------------------------------------------------------
        public void setInteractionPartner(int val, float success, TypeRelationship typeRelationship)
        {            
            if (typeRelationship != TypeRelationship.VOID )
            { 
                idInteractionPartner = val;
              //Vai buscar o dna do Gajo partner e grava-o em dnaPartner
                    int[] dnaPartner = new int[DNA.DNA_LENGHT];
                    foreach (GajoCitizen gj in CitizenFactory.listaGajos)
                    {
					if(gj!=null)
                        if (gj.getId() == val && gj.iEstado == GajoCitizen.GajoState.VIVO)
                        { dnaPartner = gj.blueprint.dna;
                          
                        }
                    }

                //Passo 1: Verifica se ja houve interacoes anteriores com este individuo
                    bool bInteractedBefore = false;
                    int index = 0;
                    for (index = 0; index < listOfInteractionPartners.Count; index++)
                    {
                        if (val == ((IndividualRelatioship)listOfInteractionPartners[index]).id)
                        {
                            bInteractedBefore = true;
                            break;                      
                        }
                    }

                //Passo 2: Updata a lista de Partners com este
                    //2.1 - Se já interagi, Updata a Lista de Interaction Partners 
                    if (bInteractedBefore)
                    {   //Update do numero das interacoes
                        totalInteracoesComEsteGajo = ((IndividualRelatioship)listOfInteractionPartners[index]).qtInteractions++;
                        //Update valor das cooperaçoes 
                        if (success > 0)
                        {
                         totalCooperativeActionComEsteGajo = ((IndividualRelatioship)listOfInteractionPartners[index]).qtCooperations++;
                         //Guarda o dna
                         if (typeRelationship == TypeRelationship.SOCIAL)
                            { ((IndividualRelatioship)listOfInteractionPartners[index]).dna = dnaPartner;                            
                             }
                        }
               		}
                    //2.2 - Se nao interagi vou criar um novo individuo
                    if (!bInteractedBefore)
                    {
                        IndividualRelatioship it = new IndividualRelatioship();
                        it.id = val;
                        //Updata o numero das interacoes
                        it.qtInteractions = 1;
                        //Updata o total das cooperaçoes    
                        //it.dna = null;
                        if (success > 0)
                        {
                            it.qtCooperations = 1;
                            //Guarda o dna
                            if (typeRelationship == TypeRelationship.SOCIAL)
                            {
                              it.dna = dnaPartner;
                            }
                        }
                        listOfInteractionPartners.Add(it);
                    }

                    //SOMATORIO DAS MINHAS INTERACOES
                    //Updata as minhas interactions 
                    totalInteractionsN++;
                    if (success > 0)
						{
							isCooperativeInteractionC = true;
							totalCooperativeInteractionsW++;
						}    else
					   			 isCooperativeInteractionC = false;
                
                typeOfInteraction = typeRelationship;
            }
        }

		public int howManyTimesHaveWeInteractedBefore(int val)
		{
		
		int index = 0;
		int ret=0;
		for (index = 0; index < listOfInteractionPartners.Count; index++)
		{
			if (val == ((IndividualRelatioship)listOfInteractionPartners[index]).id)
			{
					ret=((IndividualRelatioship)listOfInteractionPartners [index]).qtInteractions;			                    
			}
		}
		return ret;
		}
    }
}//eo class