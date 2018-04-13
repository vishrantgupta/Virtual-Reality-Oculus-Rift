//#define Reaper
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace Crowd {
    //--------------------------------------------
    //--------------------------------------------
    //Existem 5 tipos de individuos 
    //CITIZENS; 
    // -> GajoCitizen 
    // -> Followers - Tipo Adulto (side-by-side) - Nao existem nas listas de Citizen ou Produtores
    // -> Folowers - Tipo Children - Nao existem nas listas de Citizen ou Produtores
    //MERCADEIROS (FONTES);  
    //WORKERS (Invisiveis para o metabolismo, Criados a partir do Population Manager) 
    //--------------------------------------------
    //--------------------------------------------
    public struct ClassWorkerProducao {//identica a que existe em XMLManager usada para guardar os dados, mas com GameObjects
        public Profession.ProfessionType tipo;
        public int count;
        public GameObject boneco;
        public GameObject posicaoInicial;
         public AnimationClip animacaoInicial;
   
        public GameObject posicaoDelivery;
        public float raioAtracao;
        public float freedom;
        public AnimationClip animacao2;
      
    }
    public class CitizenFactory : Singleton<CitizenFactory>
    {
        //INTERFACE
     public static List<GajoCitizen> listaGajos = new List<GajoCitizen>();
     public static List<GajoWorker> listaProdutores = new List<GajoWorker>();
     public static int qtGajosSinceBeginning=0;
     public static int qtGajosBeginingPopulation = 0;
        //PRODUCAO
        public static int quantity_Production;
        public static float anchorRadius_Production;
        public static float anchorHeight_Production;
        public static List<GameObject> anchors_Production = new List<GameObject>();
        public static List<GameObject> keys_Production = new List<GameObject>();
        public static List<GameObject> listaTemplates_Adults_Production = new List<GameObject>();
        public static List<GameObject> listaTemplates_Children_Production = new List<GameObject>();

        public static List<ClassWorkerProducao> listaDefinicaoWorkers = new List<ClassWorkerProducao>();


        public static int i = 0;
//--------------------------------------------
    void Start() {            
            MarkovTemplate md = new MarkovTemplate();
            md.init(); 
	}   
    public void start(){
            begin();
            //InvokeRepeating ("startMoving",0.25f,0.0001f);           
     }

   void begin(){
            StartCoroutine("GenerateAdultPopulation");
            GenerateWorkingPopulation();
#if Reaper                 
        StartCoroutine("reaper");
#endif
        }
        //------------------------------------
     IEnumerator  GenerateAdultPopulation()
        {
            for (i = 0; i < quantity_Production; i++)
            {
                yield return new WaitForSeconds(0.05f);

                criaGajo_Producao(i, anchorRadius_Production);
                Global.gameState = Global.GameStateType.GAME_READY_HAS_AT_LEAST_ONE_CHARACTER;
            }
        }
        //------------------------------------
        public static void criaGajo_Producao(int i, float radius)
        {
            if(anchors_Production.Count>0)
            {
               int aux = Random.Range(0, anchors_Production.Count);
               Vector3 position = v3GetValidCoordinate(anchors_Production[aux].transform.position, radius);

                //CRIA CITIZEN
                aux = Random.Range(0, listaTemplates_Adults_Production.Count);
                GameObject template = listaTemplates_Adults_Production[aux];

                GameObject go = (GameObject)Instantiate(template, position, Quaternion.identity);
                go.transform.gameObject.name += i;
                GajoCitizen g = go.AddComponent<GajoCitizen>();
                g.init(qtGajosSinceBeginning++, null);
                listaGajos.Add(g);
             
                //CRIA FOLLOWER
                if (Random.Range(0, 100.0f) > 97.5f)//02.5% chances
                {
                    aux = Random.Range(0, listaTemplates_Adults_Production.Count);
                    template = listaTemplates_Adults_Production[aux];
                    if (template != null)
                    {
                        GameObject go2 = (GameObject)Instantiate(template, position + Vector3.right, Quaternion.identity);

                        GajoFollower gaf = go2.AddComponent<GajoFollower>();
                        gaf.init(g, GajoFollower.WalkingMode.ADULT);
                        go2.SetActive(true);
                        i++;
                    }
                    else   Debug.LogError("Follower:Citizen template is null!");
                }
                else

                //CRIA CRIANCA
                if (listaTemplates_Children_Production.Count > 0)
                {
                    aux = Random.Range(0, listaTemplates_Children_Production.Count);
                    template = listaTemplates_Children_Production[aux];
                    if (template != null)
                    {
                        if (Random.Range(0, 100.0f) > 95)// 5 % chances
                        {
                            GameObject go1 = (GameObject)Instantiate(template, position + Vector3.right, Quaternion.identity);
                            GajoFollower g1 = go1.AddComponent<GajoFollower>();
                            g1.init(g, GajoFollower.WalkingMode.CHILD);
                            i++;
                        }
                    } else Debug.LogError("Children template is null!");
                }
                }else { Debug.LogError("Please add at least one anchor to the scene, to define where characters should appear"); }
 
        }

        //--//
        void GenerateWorkingPopulation()
        {
            int dbIndex = 0;
           
            foreach (ClassWorkerProducao item in CitizenFactory.listaDefinicaoWorkers)
            {
                GameObject prefab = item.boneco;
                if (prefab == null)
                    Debug.LogError("Please add a character to the list of workers!! Alternatively, please delete the empty entry.");
                else
                {
                    prefab.SetActive(true);
                    for (int i = 0; i < item.count; i++)
                    {                       
                        if (item.posicaoInicial && item.boneco)
                        {
                            GameObject baseOperacoes = item.posicaoInicial;
                            Vector3 pos = CitizenFactory.v3GetValidCoordinate(baseOperacoes.transform.position, 5);
                            GameObject go = (GameObject)Instantiate(prefab, pos, Quaternion.identity);

                            // workersList.Add(go);

                            go.AddComponent(typeof(GajoWorker));
                            GajoWorker g = (GajoWorker)go.GetComponent(typeof(GajoWorker));

                            CitizenFactory.listaProdutores.Add(g);
                            g.type = item.tipo;
                            g.basePositionTr = baseOperacoes.transform;
                            g.freedomBasePosition = item.raioAtracao;
                            if (item.animacaoInicial != null)
                                g.animationProduction = item.animacaoInicial;

                            if (item.tipo == Profession.ProfessionType.Two_States)
                            {
                                if (item.posicaoDelivery != null) //item.deliveryLocation.transform;
                                {
                                    g.entregaPositionTr = item.posicaoDelivery.transform;
                                    g.animationTransaction = item.animacao2;
                                    g.isWalkingAgent = true;
                                }
                                else Debug.LogError("There is no Delivery Object defined in a two state indivivual - Worker section");

                            }
                            else g.isWalkingAgent = false;

                            g.classe_animacao = dbIndex + 1;
                            g.init();
                        }
                        else Debug.LogError("Please add a base object to the worker definition!");
                    }
                    prefab.SetActive(false);
                }
            }
        }
        public static void createGajo(int[] dna)
        {
            //Boneco//
            int aux = Random.Range(0, listaTemplates_Adults_Production.Count);
            GameObject prefab = listaTemplates_Adults_Production[aux];
            aux = Random.Range(0, anchors_Production.Count);
            Vector3 position = v3GetValidCoordinate(anchors_Production[aux].transform.position, 30);
            GameObject go = (GameObject)Instantiate(prefab, position, Quaternion.identity);

            go.transform.gameObject.name += listaGajos.Count - 1;
            GajoCitizen g = go.AddComponent<GajoCitizen>();

            g.init(qtGajosSinceBeginning++, dna);
            listaGajos.Add(g);
        }
//------------------------------------	
        public static Vector3 v3GetValidCoordinate(Vector3 pos, float range)
        {
            UnityEngine.AI.NavMeshHit hit;
            int quantos = 0;

            if (range < 5)
                range = 5;

            for (int i = 0; i < 250; i++)
            {
                Vector3 randomPoint = pos + Random.insideUnitSphere * range;
                //   randomPoint.y=Terrain.activeTerrain.terrainData.GetHeight((int)randomPoint.x, (int)randomPoint.z);
                quantos = 0;
                if (UnityEngine.AI.NavMesh.SamplePosition(randomPoint, out hit, 50.0f, UnityEngine.AI.NavMesh.AllAreas))
                {
                    Gajo[] listGGajos = GameObject.FindObjectsOfType(typeof(Gajo)) as Gajo[];
                    foreach (Gajo g in listGGajos)
                        if (Vector3.Distance(hit.position, g.transform.position) < 2f)
                        {
                            quantos++; Debug.DrawLine(hit.position, g.transform.position, Color.red);
                        }

                    if (quantos < 2)
                    {
                        return hit.position;
                    }
                }
            }
            return pos;
        }
//------------------------------------	
    public int getIndexGajo(GajoCitizen g1)  {
            for (int i = 0; i < listaGajos.Count; i++)
                if (g1.getId() == listaGajos[i].getId())
                    return i;
            return -1;
        }
    public static int getIndexGajo(int index)  {
            for (int i = 0; i < listaGajos.Count; i++)
                if(listaGajos[i]!=null)
                    if (index == listaGajos[i].getId())
                        return i;
            return -1;
        }
//------------------------------------
#if Reaper
	IEnumerator reaper()	{
			while (true) {

				int i=listaGajos.Count-1;
				for(; i>=0; i--)
				{Gajo g=listaGajos[i];
                 if (g != null)
                    {
                        if (g.iEstado == Gajo.GajoState.MORTO)
                        {
                            listaGajos.Remove(g);
                            //    Destroy(g.gameObject);
                            Destroy(g);
                        }
                    }
				}					
				listaGajos.RemoveAll (delegate (Gajo o) { return o == null; });
				yield return new WaitForSeconds (0.5f);		
			}
		}
#endif	
  }//eo class
}//eo package