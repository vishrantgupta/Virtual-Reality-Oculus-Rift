//#define debugBehaviors
//#define monitorStates
using UnityEngine;
using System.Collections;
namespace Crowd {
public class MarkovGajo
	{
	BehaviorAI parent;
//------------------------------------------------------------------------------------------
	public ArrayList stateList=new ArrayList(); 
	public	int currentState=0;
	private int previousState = 0;
    public int familiaridadeComEstadoMarkov=0;
//------------------------------------------------------------------------------------------
//-------------------------------------------------------------------------------------------
//-------------------------------------------------------------------------------------------
        public MarkovGajo (BehaviorAI parent)
		{
			this.parent = parent;
			foreach(Estado template in MarkovTemplate.stateList) {
				Estado s1 = new Estado ();
				s1.id=template.id;
				s1.tipo=template.tipo;
				s1.titulo = template.titulo;
				ArrayList transicoes=new ArrayList();
				foreach (Estado.Transicao trTemplate in template.transicoes)
					{Estado.Transicao t=new Estado.Transicao();
					 t.destino=trTemplate.destino;
					 t.condicao=trTemplate.condicao;
					 t.probabilidade=trTemplate.probabilidade;
					 transicoes.Add(t);
					}
				s1.transicoes=transicoes;
				stateList.Add (s1);
			}
		}
//-------------------------------------------------------------------------------------------
        public void die()
        {
            parent = null;
            foreach (Estado template in stateList)
            {
                //Estado s1 = null;
                for (int i = template.transicoes.Count - 1; i > 0; i--)
                {
                    ((Estado.Transicao)(template.transicoes[i])).die();
                }
                template.transicoes.Clear();
            }
            stateList.Clear();
        }
//-------------------------------------------------------------------------------------------
//-------------------------------------------------------------------------------------------
//-------------------------------------------------------------------------------------------
//Chamado em: BehaviorAI
        public int processMarkovChoice()
		{
		//get State
			Estado estado=null;
			foreach (Estado template in MarkovTemplate.stateList) 
				if (template.id == currentState)
					estado = template;

			if (estado != null) {
				//Enche bucket com potenciais transicoes			
				ArrayList bucketTransicoes = new ArrayList ();
				foreach (Estado.Transicao transicao in estado.transicoes) {
//#if debugBehaviors
//string aux = "";
//#endif
					//valida a condicao		
					bool condicaoSatisfeita = true;
					foreach (MarkovTemplate.Sensor s in transicao.condicao) {
						switch (s) {
						case MarkovTemplate.Sensor.conhecido:
							if (!parent.bConhecido)
								condicaoSatisfeita = false;
							break; 
						case MarkovTemplate.Sensor.desconhecido:
							if (!parent.bDesconhecido)
								condicaoSatisfeita = false;
							break;
						case MarkovTemplate.Sensor.ht:
							if (!parent.bHeat)
								condicaoSatisfeita = false;
							break; 
						case MarkovTemplate.Sensor.hu:
							if (!parent.bFome)
								condicaoSatisfeita = false;
							break; 
						case MarkovTemplate.Sensor.interact:
							if (!parent.bInteract)
								condicaoSatisfeita = false;
							break; 
						case MarkovTemplate.Sensor.n_interact:
							if (!parent.bNInteract)
								condicaoSatisfeita = false;
							break; 
						case MarkovTemplate.Sensor.longe:
                             if (Global.hasProdutores)
                                {
                                    if (parent.bPertoMate)
                                    {
                                        if (parent.bPertoPrey)//Perto mate e perto prey
                                        {
                                            if (parent.bPertoProdutor)//Perto mate e Perto de Prey e perto Produtor
                                                { condicaoSatisfeita = false; }
                                            else //Perto de Mate e perto Prey mas nao perto de produtor
                                                { if (parent.bFome) condicaoSatisfeita = false; }
                                        }
                                        else
                                            if (parent.bPertoProdutor)//Perto mate mas Nao Perto de Prey e perto Produtor
                                            { }
                                            else //Perto de Mate mas Nao perto Prey e nao perto de produtor
                                            { if (parent.bHeat) condicaoSatisfeita = false; } 
                                    }
                                    else//NOT PERTO MATE
                                          if (parent.bPertoPrey) //Perto Prey mas Nao perto de Mate 
                                            {
                                                if (parent.bPertoProdutor)//Nao Perto mate e Perto de Prey e perto Produtor
                                                    { if (parent.bFome) condicaoSatisfeita = false; }
                                                else //Nao Perto de Mate e Perto Prey e nao perto de produtor
                                                    { if (parent.bFome) condicaoSatisfeita = false; }
                                            }
                                            else//NOT PERTO PREY
                                                if (parent.bPertoProdutor)
                                                    {
                                                        if (parent.bPertoProdutor)//NAo Perto mate e Nao Perto de Prey e perto Produtor
                                                            { if (parent.bFome) condicaoSatisfeita = false; }
                                                        else //Nao Perto de Mate e Nao perto Prey e Nao perto de produtor
                                                         { }
                                                    }
                                }
                                else
                                {
                                  if (parent.bPertoMate)
                                    {
                                        if (parent.bPertoPrey)//Perto mate e perto prey
                                        { condicaoSatisfeita = false; }
                                        else { if (parent.bHeat) condicaoSatisfeita = false; } //Perto mate mas nao perto prey
                                    }
                                    else
                                        if (parent.bPertoPrey) //Perto Prey mas Nao perto de Mate 
                                        { if (parent.bFome) condicaoSatisfeita = false; }
                                        else { } //Nao perto mate nem perto prey 
                                }
                                break; 							 
						case MarkovTemplate.Sensor.perto:
                                if (Global.hasProdutores)
                                {
                                    if (parent.bPertoMate)
                                    {
                                        if (parent.bPertoPrey)//Perto mate e perto prey
                                        {
                                            if (parent.bPertoProdutor)//Perto mate e Perto de Prey e perto Produtor
                                                { }
                                            else //Perto de Mate e perto Prey mas nao perto de produtor
                                                { }
                                        }
                                        else
                                            if (parent.bPertoProdutor)//Perto mate mas Nao Perto de Prey e perto Produtor
                                                { }
                                            else //Perto de Mate mas Nao perto Prey e nao perto de produtor
                                                { if (parent.bFome) condicaoSatisfeita = false; }
                                    }
                                    else//NOT PERTO MATE
                                          if (parent.bPertoPrey) //Perto Prey mas Nao perto de Mate 
                                            {
                                                if (parent.bPertoProdutor)//Nao Perto mate e Perto de Prey e perto Produtor
                                                    { if (parent.bHeat) condicaoSatisfeita = false; }
                                                else //Nao Perto de Mate e Perto Prey e nao perto de produtor
                                                    { if (parent.bHeat && !parent.bFome) condicaoSatisfeita = false; }
                                            }
                                            else//NOT PERTO PREY
                                                        if (parent.bPertoProdutor)
                                                        {
                                                            if (parent.bPertoProdutor)//NAo Perto mate e Nao Perto de Prey e perto Produtor
                                                                { if (parent.bHeat && !parent.bFome) condicaoSatisfeita = false; }
                                                                     else //Nao Perto de Mate e Nao perto Prey e Nao perto de produtor
                                                                        { condicaoSatisfeita = false; }
                                                        }
                                }
                                else //NOT GLOBAL.HASPRODUTORES
                                {
                                 if (parent.bPertoMate)
                                    {
                                        if (parent.bPertoPrey)//Perto mate e perto prey
                                        { }
                                        else { if (parent.bFome) condicaoSatisfeita = false; } //Perto mate mas nao perto prey
                                    }
                                    else
                                          if (parent.bPertoPrey) //Perto Prey mas Nao perto de Mate 
                                             { if (parent.bHeat) { condicaoSatisfeita = false; } }
                                         else { condicaoSatisfeita = false; } //Nao perto mate nem perto prey 
                                }
                                break;
                         case MarkovTemplate.Sensor.emergency:
                                if (!parent.bEmergency)
                                    condicaoSatisfeita = false;
                                break;
                         default:
							    break;
						}
//#if debugBehaviors
//if(parent.owner.getId ()==0) 
//if(condicaoSatisfeita==false)
//			aux+="," + s.ToString() + " HasP:" + Global.hasProdutores + " F:" + parent.bFome + " H:" + parent.bHeat + " PPd:" + parent.bPertoProdutor + " PPy:" + parent.bPertoPrey + " PMt:" + parent.bPertoMate + "-";
                        //#endif
                    }//fim da validacao
//if (condicaoSatisfeita == false)
//Debug.Log("Condicao nao satisteita" + aux);
                    //se a condicao for valida poe no bucket
                        if (condicaoSatisfeita)
						    bucketTransicoes.Add (transicao);
                }

                //Identifica qual a que tem mais probabilidade (das q estao no bucket)
                float maior = 0;
				int indiceMaior = -1;
				Estado.Transicao transicaoEscolhida = null;
				for (int i=0; i<bucketTransicoes.Count; i++) {
					transicaoEscolhida = ((Estado.Transicao)bucketTransicoes [i]);                 
					if (maior < transicaoEscolhida.probabilidade) {
						maior = transicaoEscolhida.probabilidade;
						indiceMaior = i;
					} else 
						if (maior == transicaoEscolhida.probabilidade) {//EQUAL----Some stochasticity para evitar quando o primeiro tem probablilidsade 1 e fica ali
						if (Random.Range (0, 10) >= 5) {
							maior = transicaoEscolhida.probabilidade;
							indiceMaior = i;
						}
					}
				}
//---------TODAY
//if (indiceMaior == -1)
//{
//Debug.Log(parent.showSensors());
//Debug.Log("Debug Info" + parent.bFome + " " + parent.bHeat + " " + parent.bPertoPrey + " " + parent.bPertoMate + " ");
//Debug.Log("Bucket tem" + bucketTransicoes.Count); 
//}
//--------------
                if (indiceMaior!=-1) {
#if debugBehaviors
                    int nextState=((Estado.Transicao)bucketTransicoes [indiceMaior]).destino;
		//			if (parent.owner.getId () == Global.whoIsBeingTracked) {
		//				Debug.Log (parent.owner.preferedPosition.position + " " + "State " + currentState + " " + "Tem " + conta + "transicoes em que a maior tem " +
		//				           maior + ", que e para o estado " + nextState + " ");
						parent.showSensors ();
					}
#endif
                    previousState = currentState;
					currentState = ((Estado.Transicao)bucketTransicoes [indiceMaior]).destino;
                    ((Estado.Transicao)bucketTransicoes[indiceMaior]).numTimesPerfomed++;
                    familiaridadeComEstadoMarkov=((Estado.Transicao)bucketTransicoes[indiceMaior]).numTimesPerfomed;
                }
				else 
				{
#if monitorStates
                    if (parent.owner.getId() == Global.whoIsBeingTracked)
                    {

                        Debug.Log("--------------------------------------------------------------------" +
                          "Transicao Escolhida = null como nao sei o que fazer o meu estado e " +
                          currentState + " " +
                          parent.showSensors() +
                          "vou transicionar para Estado 0");
                }
#endif
                    currentState = 0;
				}
			}
		return currentState; 	
		}
//-------------------------------------------------------------------------------------------
	public void updatesChain (float reward) 
		{
            ArrayList bucketEstadosOrigem = new ArrayList ();
			ArrayList bucketTransicoes = new ArrayList ();
			foreach (Estado state in stateList) {
				for (int i=0; i<state.transicoes.Count; i++) {   
					Estado.Transicao trans = (Estado.Transicao)state.transicoes [i];
					if (state.id == previousState && trans.destino == currentState) {//se o estado de onde veio
							trans.probabilidade += reward;
							if (trans.probabilidade > 1)
								trans.probabilidade = 1;
							break;
						} else 
						if (trans.destino == currentState) {
							bucketTransicoes.Add (i);
							bucketEstadosOrigem.Add (state.id);
						}				
					}
				}	

				for (int i=0; i<bucketTransicoes.Count; i++) {
					int indexEstado = (int)bucketEstadosOrigem [i];
					int indexTransicao = (int)bucketTransicoes [i];	
					float quantos = (float)bucketEstadosOrigem.Count;
					if (indexEstado != -1 && indexTransicao != -1) {
						Estado estado = (Estado)stateList [indexEstado]; 
						Estado.Transicao transition = (Estado.Transicao)estado.transicoes [indexTransicao];
						if (quantos != 0)
							transition.probabilidade -= ((float)reward / quantos);
						if (transition.probabilidade < 0)
							transition.probabilidade = 0;
					}
				}
		}

//-------------------------------------------------------------------------------------------
	public void showInternalMarkov()
		{Debug.Log ("---------------Inicio---------------------------------------");
			foreach (Estado e in MarkovTemplate.stateList)
				foreach (Estado.Transicao t in e.transicoes)
			{string s ="";
				foreach (MarkovTemplate.Sensor sen in t.condicao)
					s+=sen.ToString();
				Debug.Log ("Markov Gajo " + e.id + " " + t.destino + " " + s); 
			}
			Debug.Log ("---------------Carregou---------------------------------------");
		}
	//------------------------------------------------------------------------------------------
	}//eo classs
}//eo namespace

