//Grava dados no ficheiro
//#define gravaEstadosEmFicheiroGlobal
//#define gravaEstadosEmFicheiroIndividuo


namespace Crowd
{
    using UnityEngine;
    using System.IO;
    using System.Collections;

    public class Log : MonoBehaviour
    {
        public static int timeFrame = 0;
        //Variables para monitoring e record 
#if gravaEstadosEmFicheiroGlobal
		const float durationShortCyleGlobal = 30f; //Penso que sao segundos 
                                                   //const float durationShortCyleGlobal = 30f; //Penso que sao segundos 
                                                   //const float durationLongCyle = 3600f;
#endif
#if gravaEstadosEmFicheiroIndividuo
        const float durationShortCyleIndividual = 5f; //Penso que sao segundos 
#endif

        //--------------------------------------------
        void Awake()
        {
           
#if gravaEstadosEmFicheiroGlobal
            writerGlobal = new StreamWriter ("C:\\Users\\Utilizador\\Desktop\\logFileUnity-Global.txt");            
			StartCoroutine(Timer.Start(durationShortCyleGlobal, true, () =>
			                           {    calculaIndices();
											recordSystemStateInFileGlobal();                                            
										}));
#endif
/*#if gravaEstadosEmFicheiroIndividuo
			writerIndividuo = new StreamWriter("C:\\Users\\Utilizador\\Desktop\\logFileUnity-Individual.txt");
            StartCoroutine(Timer.Start(durationShortCyleIndividual, true, () =>
                                       {
                                           calculaIndices();
                                           recordSystemStateInFileIndividuo();
                                       }));
#endif
*/
        }
        void Start()
		{ Debug.Log("Começou o Log!" + 	Time.time); }
        //-------------------------------
        void calculaIndices()
        {
            int _000 = 0;
            int _001 = 0;
            int _010 = 0;
            int _011 = 0;
            int _100 = 0;
            int _101 = 0;
            int _110 = 0;
            int _111 = 0;
            float Hmax = Mathf.Log(8);
            

			int vivos = 0;
			foreach (GajoCitizen g in CitizenFactory.listaGajos) {
				if (g != null)
				if (g.iEstado == GajoCitizen.GajoState.VIVO)
					vivos++;			
			}
			float totalPopulation = (float)vivos;
	
			//------------------------------------------------

            foreach (GajoCitizen g in CitizenFactory.listaGajos)
            {if(g!=null)
                if (g.blueprint != null)
                    if (g.blueprint.dna[13] == 0)
                    {
                        if (g.blueprint.dna[14] == 0)
                        {
                            if (g.blueprint.dna[15] == 0)
                                _000++;
                            else
                                _001++;
                        }
                        else
                        { //14=1
                            if (g.blueprint.dna[15] == 0)
                                _010++;
                            else
                                _011++;
                        }
                    }
                    else
                    {
                        if (g.blueprint.dna[14] == 0)
                        {
                            if (g.blueprint.dna[15] == 0)
                                _100++;
                            else
                                _101++;
                        }
                        else
                        { //14=1
                            if (g.blueprint.dna[15] == 0)
                                _110++;
                            else
                                _111++;
                        }
                    }
            }

            float pi_000 = _000 / totalPopulation;
            float pi_001 = _001 / totalPopulation;
            float pi_010 = _010 / totalPopulation;
            float pi_011 = _011 / totalPopulation;
            float pi_100 = _100 / totalPopulation;
            float pi_101 = _101 / totalPopulation;
            float pi_110 = _110 / totalPopulation;
            float pi_111 = _111 / totalPopulation;

            double pilnpi000 = pi_000 * Mathf.Log((float)pi_000);
            if (double.IsNaN(pilnpi000) || double.IsInfinity(pilnpi000))
                pilnpi000 = 0;
            double pilnpi001 = pi_001 * Mathf.Log((float)pi_001);
            if (double.IsNaN(pilnpi001) || double.IsInfinity(pilnpi001))
                pilnpi001 = 0;
            double pilnpi010 = pi_010 * Mathf.Log((float)pi_010);
            if (double.IsNaN(pilnpi010) || double.IsInfinity(pilnpi010))
                pilnpi010 = 0;
            double pilnpi011 = pi_011 * Mathf.Log((float)pi_011);
            if (double.IsNaN(pilnpi011) || double.IsInfinity(pilnpi011))
                pilnpi011 = 0;
            double pilnpi100 = pi_100 * Mathf.Log((float)pi_100);
            if (double.IsNaN(pilnpi100) || double.IsInfinity(pilnpi100))
                pilnpi100 = 0;
            double pilnpi101 = pi_101 * Mathf.Log((float)pi_101);
            if (double.IsNaN(pilnpi101) || double.IsInfinity(pilnpi101))
                pilnpi101 = 0;
            double pilnpi110 = pi_110 * Mathf.Log((float)pi_110);
            if (double.IsNaN(pilnpi110) || double.IsInfinity(pilnpi110))
                pilnpi110 = 0;
            double pilnpi111 = pi_111 * Mathf.Log((float)pi_111);
            if (double.IsNaN(pilnpi111) || double.IsInfinity(pilnpi111))
                pilnpi111 = 0;

            double shannon = -1d * (pilnpi000 + pilnpi001 + pilnpi010 + pilnpi011 + pilnpi100 + pilnpi101 + pilnpi110 + pilnpi111);

            double evenness = shannon / Hmax;
			Debug.Log(Time.time + "Evenness " + evenness + " " + "Shannon " + shannon + " " + pi_000 + " " + pilnpi000 + " " + Mathf.Log((float)pi_000));
            //		Debug.Log (_000 + " " + _001 + " " + _010 + " " + _011 + " " + _100 + " " + _101 + " " + _110 + " " + _111);
            //		Debug.Log (pi_000 + " " + pi_001 + " " + pi_010 + " " + pi_011 + " " + pi_100 + " " + pi_101 + " " + pi_110 + " " + pi_111);
            //		Debug.Log (pilnpi000 + " " + pilnpi001 + " " + pilnpi010 + " " + pilnpi011 + " " + pilnpi100 + " " + pilnpi101 + " " + pilnpi110 + " " + pilnpi111);
        }
        //---------------------------------------------------------------------------------------------------------------------------	
        // Update is called once per frame
        void Update()
        {
  /*          if (Input.GetKeyDown(KeyCode.H))
            {
                //StreamWriter writer = new StreamWriter("Resources/" + FileName + ".txt"); // Does this work?
            }
            if (Input.GetKeyDown(KeyCode.J))
            {
                //StreamWriter writer = new StreamWriter("Resources/" + FileName + ".txt"); // Does this work?
#if gravaEstadosEmFicheiroGlobal
				writerGlobal.Close();
#endif
#if gravaEstadosEmFicheiroIndividuo
                writerIndividuo.Close();
#endif
            }
*/        }
        //------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------
#if gravaEstadosEmFicheiroGlobal
        StreamWriter writerGlobal;
		string str;
		int numRunGlobal=1;
		private void recordSystemStateInFileGlobal ()
		{	

			timeFrame++;
			/*		if (durationShortCyleGlobal * timeFrame >= durationLongCyle)
			{	numRunGlobal++;
				timeFrame = 0;
				for(int i=0; i<GajosFactory.listaGajos.Count; i++)
				    {Gajo g=GajosFactory.listaGajos[i];
                    if (g != null)
                        {
                            Global.gameState = Global.GameStateType.GAME_INITIALIZING;
                            g.iEstado = Gajo.GajoState.MORTO;
                            g.remove();
                        }
				    }
				//REAPER			
				int j=GajosFactory.listaGajos.Count-1;
				for(; j>=0; j--)
				    {Gajo g=GajosFactory.listaGajos[j];
                     if(g!=null)
					    if(g.iEstado== Gajo.GajoState.MORTO)
					    {
						    Destroy(g);
						    //					DestroyObject(g.gameObject);
						    Destroy(g.gameObject);
						    GajosFactory.listaGajos.Remove(g);
					    }
				    }
                //--
                GajosFactory.listaGajos.RemoveAll (delegate (Gajo o) 
				                                   { return o == null; });
				//--	
				GajosFactory.Restart();
			}
*/


			foreach (GajoCitizen g1 in CitizenFactory.listaGajos) {
				str = "";
				if (g1 != null) {   

					str += numRunGlobal + ",";
					str += timeFrame + ",";
					str += CitizenFactory.listaGajos.Count + ",";
					str += g1.getId () + ",";
					str += g1.behavior.snapshot.iEstado + ",";

					str += g1.behavior.snapshot.position.x + ",";
					str += g1.behavior.snapshot.position.y + ",";
					str += g1.behavior.snapshot.position.z + ",";

					if (g1.behavior.snapshot.iEstado == GajoCitizen.GajoState.VIVO)
						str += g1.behavior.snapshot.mood.x + ",";
					else
						str += "nul" + ",";
					if (g1.behavior.snapshot.iEstado == GajoCitizen.GajoState.VIVO)
						str += g1.behavior.snapshot.mood.y + ",";
					else
						str += "nul" + ",";
					if (g1.behavior.snapshot.iEstado == GajoCitizen.GajoState.VIVO)
						str += g1.behavior.snapshot.mood.z + ",";
					else
						str += "nul" + ",";

					str += g1.behavior.snapshot.duracaoMediaInteracoes + ",";

					//s += g1.whatAmIDoing + ",";
					if (g1.behavior.snapshot.iEstado == GajoCitizen.GajoState.VIVO) {						
						str += g1.behavior.snapshot.action;
					} else
						str += "Move";
					
					str += ",";
						
					if (g1.behavior.snapshot.iEstado == GajoCitizen.GajoState.VIVO)
						str += g1.behavior.snapshot.emotion.x + ",";
					else
						str += "nul" + ",";
					if (g1.behavior.snapshot.iEstado == GajoCitizen.GajoState.VIVO)
						str += g1.behavior.snapshot.emotion.y + ",";
					else
						str += "nul" + ",";
					if (g1.behavior.snapshot.iEstado == GajoCitizen.GajoState.VIVO)
						str += g1.behavior.snapshot.emotion.z + ",";
					else
						str += "nul" + ",";
					if (g1.behavior.snapshot.iEstado == GajoCitizen.GajoState.VIVO)
						str += g1.behavior.snapshot.personality.x + ",";
					else
						str += "nul" + ",";
					if (g1.behavior.snapshot.iEstado == GajoCitizen.GajoState.VIVO)
						str += g1.behavior.snapshot.personality.y + ",";
					else
						str += "nul" + ",";

					if (g1.behavior.snapshot.iEstado == GajoCitizen.GajoState.VIVO)
						str += g1.behavior.snapshot.personality.z + ",";
					else
						str += "nul" + ",";

					//--//
					if (g1.behavior.snapshot.iEstado == GajoCitizen.GajoState.VIVO)
						str += g1.behavior.snapshot.energy + ",";
					else
						str += "nul" + ",";
					if (g1.behavior.snapshot.iEstado == GajoCitizen.GajoState.VIVO)
						str += g1.behavior.snapshot.connectedness + ",";
					else
						str += "nul" + ",";
					if (g1.behavior.snapshot.iEstado == GajoCitizen.GajoState.VIVO)
						str += g1.behavior.snapshot.chemicals [0] + ",";
					else
						str += "nul" + ",";
					if (g1.behavior.snapshot.iEstado == GajoCitizen.GajoState.VIVO)
						str += g1.behavior.snapshot.chemicals [1] + ",";
					else
						str += "nul" + ",";
					if (g1.behavior.snapshot.iEstado == GajoCitizen.GajoState.VIVO)
						str += g1.behavior.snapshot.chemicals [2] + ",";
					else
						str += "nul" + ",";
					//--//
					if (g1.behavior.snapshot.iEstado == GajoCitizen.GajoState.VIVO) {
						str += "*";
						for (int i = 0; i < g1.behavior.snapshot.blueprint.Length; i++)
							str += g1.behavior.snapshot.blueprint [i];
						str += ",";
					}else str += "nul" + ",";
					//--//
				
					str += g1.behavior.snapshot.numeroInteracoes;
					str += ",";
					str += g1.behavior.snapshot.totalCooperativeInteractionsW;
					str += ",";
					str += g1.behavior.snapshot.totalInteractionsN;
					str += ",";
					switch (g1.behavior.snapshot.typeOfInteraction) {
					case MemoryOfRelationships.TypeRelationship.VOID:
						str += "Void";
						break;
					case MemoryOfRelationships.TypeRelationship.SOCIAL:
						str += "Social";
						break;
					case MemoryOfRelationships.TypeRelationship.TRADE:
						str += "Trade";
						break;
					}
					str += ",";
					if (g1.behavior.snapshot.iEstado == GajoCitizen.GajoState.VIVO)
						str += g1.metabolism.blueprint.getP () + ",";
					else
						str += "nul" + ",";
					if (g1.behavior.snapshot.iEstado == GajoCitizen.GajoState.VIVO)
						str += g1.metabolism.blueprint.getA () + ",";
					else
						str += "nul" + ",";
					if (g1.behavior.snapshot.iEstado == GajoCitizen.GajoState.VIVO)
						str += g1.metabolism.blueprint.getD () + ",";
					else
						str += "nul" + ",";
					if (g1.behavior.snapshot.iEstado == GajoCitizen.GajoState.VIVO)
						str += g1.behavior.snapshot.energyTolerance + ",";
					else
						str += "nul" + ",";
					if (g1.behavior.snapshot.iEstado == GajoCitizen.GajoState.VIVO)
						str += g1.metabolism.blueprint.getConnectedness () + ",";
					else
						str += "nul" + ",";
					if (g1.behavior.snapshot.iEstado == GajoCitizen.GajoState.VIVO)
						str += g1.metabolism.blueprint.getSleepCycle () + ",";
					else
						str += "nul" + ",";
			}
				writerGlobal.WriteLine (str);
			}//for

		
					
 /*RUISNAPSHOT NO BEHAVIOR                str += numRunGlobal + ",";
                    str += timeFrame + ",";
                    str += GajosFactory.listaGajos.Count + ",";
                    str += g1.getId() + ",";
                    str += g1.iEstado + ",";
                    str += g1.transform.position.x + ",";
                    str += g1.transform.position.y + ",";
                    str += g1.transform.position.z + ",";
                    
                    if (g1.iEstado == Gajo.GajoState.VIVO)
                        str += g1.behavior.psy.mood.x + ",";
                    else
                        str += "nul" + ",";
                    if (g1.iEstado == Gajo.GajoState.VIVO)
                        str += g1.behavior.psy.mood.y + ",";
                    else
                        str += "nul" + ",";
                    if (g1.iEstado == Gajo.GajoState.VIVO)
                        str += g1.behavior.psy.mood.z + ",";
                    else
                        str += "nul" + ",";

                    str += g1.behavior.Interacao.Memoria.duracaoMediaInteracoes + ",";

                    //s += g1.whatAmIDoing + ",";
                    if (g1.iEstado == Gajo.GajoState.VIVO)
                    {
						switch (g1.behavior.actionPerformed)
						{
						case 0: str += "Still"; break;
						case 1: str += "Move"; break;
						case 2: str += "Move"; break;
						case 3: str += "Wander"; break;
						case 4: str += "Attempt_Social_Start"; break;
						case 5: str += "Attempt_Social_Conhecido"; break;
						case 6: str += "Attempt_Social_Desconhecido"; break;
						case 7:
							if(g1.behavior.Interacao.Memoria.isCooperativeInteractionC)
								str += "Attempt_Social_Win";
							else 
							    str += "Attempt_Social_Lose";						
							break;
						case 8: str += "Trade_Begin"; break;
						case 9: str += "Trade_Conhecido"; break;
						case 10: str += "Trade_Desconhecido"; break;
						case 11:
							if(g1.behavior.Interacao.Memoria.isCooperativeInteractionC)
								str += "Trade_Win";
							else								
								str += "Trade_Lose";
							break;
						case 12:
							str += "Move";
							break;
						}
                    }
                    else str += "Move";
                    str += ",";
                 
                    if (g1.iEstado == Gajo.GajoState.VIVO)
                        str += g1.behavior.psy.emotion.x + ",";
                    else str += "nul" + ",";
                    if (g1.iEstado == Gajo.GajoState.VIVO)
                        str += g1.behavior.psy.emotion.y + ",";
                    else str += "nul" + ",";
                    if (g1.iEstado == Gajo.GajoState.VIVO)
                        str += g1.behavior.psy.emotion.z + ",";
                    else str += "nul" + ",";
                    if (g1.iEstado == Gajo.GajoState.VIVO)
                        str += g1.behavior.psy.personality.x + ",";
                    else str += "nul" + ",";
                    if (g1.iEstado == Gajo.GajoState.VIVO)
                        str += g1.behavior.psy.personality.y + ",";
                    else str += "nul" + ",";

                    if (g1.iEstado == Gajo.GajoState.VIVO)
						str += g1.behavior.psy.personality.z+ ",";
                    else str += "nul" + ",";

                    //--//
                    if (g1.iEstado == Gajo.GajoState.VIVO)
						str += g1.metabolism.energy + ",";
					else str += "nul" + ",";
                    if (g1.iEstado == Gajo.GajoState.VIVO)
						str += g1.metabolism.connectedness + ",";
					else str += "nul" + ",";
                    if (g1.iEstado == Gajo.GajoState.VIVO)
						str += g1.metabolism.chemicals[0] + ",";
					else str += "nul" + ",";
                    if (g1.iEstado == Gajo.GajoState.VIVO)
						str += g1.metabolism.chemicals[1] + ",";
					else str += "nul" + ",";
                    if (g1.iEstado == Gajo.GajoState.VIVO)
						str += g1.metabolism.chemicals[2] + ",";
					else str += "nul" + ",";
                    //--//
					str += "*";
                    for (int i = 0; i < g1.blueprint.dna.Length; i++)
                        str+= g1.blueprint.dna[i];
					//--//
					str += ",";
					str += g1.behavior.Interacao.Memoria.numeroInteracoes;
					str += ",";
					str += g1.behavior.Interacao.Memoria.totalCooperativeInteractionsW;
					str+= ",";
					str += g1.behavior.Interacao.Memoria.totalInteractionsN;
					str += ",";
					switch (g1.behavior.Interacao.Memoria.typeOfInteraction) {
					case MemoryOfRelationships.TypeRelationship.VOID:
						str += "Void";
						break;
					case MemoryOfRelationships.TypeRelationship.SOCIAL:
						str += "Social";
						break;
					case MemoryOfRelationships.TypeRelationship.TRADE:
						str += "Trade";
						break;
					}
					str += ",";
					if (g1.iEstado == Gajo.GajoState.VIVO)
						str += g1.metabolism.blueprint.getP() + ",";
					else str += "nul" + ",";
					if (g1.iEstado == Gajo.GajoState.VIVO)
						str += g1.metabolism.blueprint.getA() + ",";
					else str += "nul" + ",";
					if (g1.iEstado == Gajo.GajoState.VIVO)
						str += g1.metabolism.blueprint.getD() + ",";
					else str += "nul" + ",";
					if (g1.iEstado == Gajo.GajoState.VIVO)
						str += g1.metabolism.blueprint.getEnergyTolerance() + ",";
					else str += "nul" + ",";
					if (g1.iEstado == Gajo.GajoState.VIVO)
						str += g1.metabolism.blueprint.getConnectedness() + ",";
					else str += "nul" + ",";
					if (g1.iEstado == Gajo.GajoState.VIVO)
						str += g1.metabolism.blueprint.getSleepCycle() + ",";
					else str += "nul" + ","; 
					}

                writerGlobal.WriteLine(str);
            }//For	
            */
		}
#endif

#if gravaEstadosEmFicheiroIndividuo
        //''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''//
        StreamWriter writerIndividuo;
       
        int numRun = 1;
        private void recordSystemStateInFileIndividuo()
        {
            timeFrame++;
      
            Gajo g1 = GajosFactory.listaGajos[0];
            str = "";
            if (g1 != null)
            {
                str += numRun + ",";
                str += timeFrame + ",";
                str += Global.livingGajos + ",";
                str += g1.getId() + ",";
                str += g1.iEstado + ",";
                str += g1.transform.position.x + ",";
                str += g1.transform.position.y + ",";
                str += g1.transform.position.z + ",";
                if (g1.iEstado == Gajo.GajoState.VIVO)
                    str += g1.behavior.psy.mood.x + ",";
                else
                    str += "nul" + ",";
                if (g1.iEstado == Gajo.GajoState.VIVO)
                    str += g1.behavior.psy.mood.y + ",";
                else
                    str += "nul" + ",";
                if (g1.iEstado == Gajo.GajoState.VIVO)
                    str += g1.behavior.psy.mood.z + ",";
                else
                    str += "nul" + ",";

                str += g1.behavior.Interacao.Memoria.duracaoMediaInteracoes + ",";

                //s += g1.whatAmIDoing + ",";
                if (g1.iEstado == Gajo.GajoState.VIVO)
                {
                    switch (g1.behavior.myMarkov.currentState)
                    {
                        case 0: str += "Still"; break;
                        case 1: str += "Move"; break;
                        case 2: str += "Move"; break;
                        case 3: str += "Wander"; break;
                        case 4: str += "Attempt_Social_Start"; break;
                        case 5: str += "Attempt_Social_Win"; break;
                        case 6: str += "Attempt_Social_Lose"; break;
                        case 7:
                            if (g1.behavior.dispatcher.lastActionPerformed == 5)
                                str += "Attempt_Social_Win";
                            else
                            if (g1.behavior.dispatcher.lastActionPerformed == 6)
                                str += "Attempt_Social_Lose";
                            else str += "Attempt_Social_End";
                            break;
                        case 8: str += "Trade_Begin"; break;
                        case 9: str += "Trade_Win"; break;
                        case 10: str += "Trade_Lose"; break;
                        case 11:
                            if (g1.behavior.dispatcher.lastActionPerformed == 9)
                                str += "Trade_Win";
                            else
                           if (g1.behavior.dispatcher.lastActionPerformed == 10)
                                str += "Trade_Lose";
                            else str += "Trade_End";
                            break;
                        case 12:
                            str += "Move";
                            break;
                    }
                }
                else str += "Move";
                str += ",";
                //deprecated          if (g1.iEstado == "vivo")
                //deprecated                s += g1.walker.Navigation.MaxSpeed + ",";
                //deprecated            else s += "nul" + ",";
                //deprecated        if (g1.iEstado == "vivo")
                //deprecated        s += g1.walker.Navigation.MaxNeighbours + ",";
                //deprecated        else s += "nul" + ",";
                //deprecated        if (g1.iEstado == "vivo")
                //deprecated        s += g1.walker.Navigation.NeighboursDist + ",";
                //deprecated        else s += "nul" + ",";
                //deprecated        if (g1.iEstado == "vivo")
                //deprecated        s += g1.walker.Navigation.Radius + ",";
                //deprecated        else s += "nul" + ",";
                if (g1.iEstado == Gajo.GajoState.VIVO)
                    str += g1.behavior.psy.emotion.x + ",";
                else str += "nul" + ",";
                if (g1.iEstado == Gajo.GajoState.VIVO)
                    str += g1.behavior.psy.emotion.y + ",";
                else str += "nul" + ",";
                if (g1.iEstado == Gajo.GajoState.VIVO)
                    str += g1.behavior.psy.emotion.z + ",";
                else str += "nul" + ",";
                if (g1.iEstado == Gajo.GajoState.VIVO)
                    str += g1.behavior.psy.personality.x + ",";
                else str += "nul" + ",";
                if (g1.iEstado == Gajo.GajoState.VIVO)
                    str += g1.behavior.psy.personality.y + ",";
                else str += "nul" + ",";
                if (g1.iEstado == Gajo.GajoState.VIVO)
                    str += g1.behavior.psy.personality.z + ",";
                else str += "nul" + ",";

                //--//
                if (g1.iEstado == Gajo.GajoState.VIVO)
                    str += g1.metabolism.energy + ",";
				else str += "nul" + ",";
                if (g1.iEstado == Gajo.GajoState.VIVO)
                    str += g1.metabolism.connectedness + ",";
                else str += "nul" + ",";
                if (g1.iEstado == Gajo.GajoState.VIVO)
                    str += g1.metabolism.chemicals[0] + ",";
                else str += "nul" + ",";
                if (g1.iEstado == Gajo.GajoState.VIVO)
                    str += g1.metabolism.chemicals[1] + ",";
                else str += "nul" + ",";
                if (g1.iEstado == Gajo.GajoState.VIVO)
                    str += g1.metabolism.chemicals[2] + ",";
                else str += "nul" + ",";
                //--//
                /* if (g1.iEstado == Gajo.GajoState.VIVO)
                     s += g1.metabolism.ht_testosterone + ",";
                 else s += "nul" + ",";
                 if (g1.iEstado == Gajo.GajoState.VIVO)
                     s += g1.metabolism.ha_adrenaline + ",";
                 else s += "nul" + ",";
                 if (g1.iEstado == Gajo.GajoState.VIVO)
                     s += g1.metabolism.hs_serotonin + ",";
                 else s += "nul" + ",";
                 if (g1.iEstado == Gajo.GajoState.VIVO)
                     s += g1.metabolism.hm_melatonin + ",";
                 else s += "nul" + ",";
                 if (g1.iEstado == Gajo.GajoState.VIVO)
                     s += g1.metabolism.hl_leptin;
                 else s += "nul";
                 */
                //--//
				str += "*";
                for (int i = 0; i < g1.blueprint.dna.Length; i++)
                    str += g1.blueprint.dna[i];
				//--//
				str += ",";
				str += g1.behavior.Interacao.Memoria.numeroInteracoes;
				str += ",";
				str += g1.behavior.Interacao.Memoria.totalCooperativeInteractionsW;
				str += ",";
				str += g1.behavior.Interacao.Memoria.totalInteractionsN;
				str += ",";
				switch (g1.behavior.Interacao.Memoria.typeOfInteraction) {
				case MemoryOfRelationships.TypeRelationship.VOID:
					str+= "Void";
					break;
				case MemoryOfRelationships.TypeRelationship.SOCIAL:
					str+= "Social";
					break;
				case MemoryOfRelationships.TypeRelationship.TRADE:
					str+= "Trade";
					break;
				}
				str += ",";
				if (g1.iEstado == Gajo.GajoState.VIVO)
					str+= g1.metabolism.blueprint.getP() + ",";
				else str+= "nul" + ",";
				if (g1.iEstado == Gajo.GajoState.VIVO)
					str+= g1.metabolism.blueprint.getA() + ",";
				else str+= "nul" + ",";
				if (g1.iEstado == Gajo.GajoState.VIVO)
					str+= g1.metabolism.blueprint.getD() + ",";
				else str+= "nul" + ",";
				if (g1.iEstado == Gajo.GajoState.VIVO)
					str+= g1.metabolism.blueprint.getEnergyTolerance() + ",";
				else str+= "nul" + ",";
				if (g1.iEstado == Gajo.GajoState.VIVO)
					str+= g1.metabolism.blueprint.getConnectedness() + ",";
				else str+= "nul" + ",";
				if (g1.iEstado == Gajo.GajoState.VIVO)
					str+= g1.metabolism.blueprint.getSleepCycle() + ",";
				else str+= "nul" + ",";
            }
            writerIndividuo.WriteLine(str);
        }

#endif
        //-------------------------------------------------------------------
        private void OnApplicationQuit()
        {
#if gravaEstadosEmFicheiroGlobal
             writerGlobal.Close();
            Debug.Log("Fechou ficheiro global");
#endif
/*
#if gravaEstadosEmFicheiroIndividuo
            writerIndividuo.Close();
            Debug.Log("Fechou ficheiro individual");
#endif
*/
        }
    }
}