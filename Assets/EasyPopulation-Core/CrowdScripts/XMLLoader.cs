using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;//access xml serializer
using System.IO;//file management 
using System.Text;


namespace Crowd
{
    [System.Serializable]
    public class XMLLoader : XMLManager {
        /*---------------------------------------------------*/
        // public static string dataPath = string.Empty;
        
        populationDB popDBFile;
       //load function
       [ExecuteInEditMode]   
     public void LoadItems()
    {       readFile();
            loadData();       
    }

        private void readFile()
        {
            TextAsset xml_Text = (TextAsset)Resources.Load("EasyPopulation/population", typeof(TextAsset));

            MemoryStream stream = new MemoryStream(xml_Text.bytes);
            Encoding encoding = Encoding.GetEncoding("UTF-8");
            StreamReader stream2 = new StreamReader(stream, encoding);
          
            popDBFile = new populationDB();
            XmlSerializer serializer = new XmlSerializer(typeof(populationDB));
            popDBFile = serializer.Deserialize(stream2) as populationDB;
            stream.Close();
            stream2.Close();
        }

        private void loadData()
        {
            if (popDBFile.list.Count > 0)
            {
                foreach (ActorData ad in popDBFile.list)
                {
                    CitizenFactory.quantity_Production = ad.quantidade;
                    Debug.Log("LoadXML: quantity citizens " + ad.quantidade);
                    CitizenFactory.anchorRadius_Production = ad.anchorRadius;
                    Debug.Log("LoadXML: anchorRadius " + ad.anchorRadius);
                    CitizenFactory.anchors_Production = new List<GameObject>();
                    for (int i = 0; i < ad.anchors.Count; i++)
                    {
                        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                        go.name = "WalkerAnchor" + i;
                        go.transform.position = new Vector3(ad.anchors[i].x, ad.anchors[i].y, ad.anchors[i].z);
                        Renderer renderer = go.GetComponent<Renderer>();
                        renderer.enabled = false;
                        CitizenFactory.anchors_Production.Add(go);
                    }
                    Debug.Log("LoadXML:Carregou anchors " + CitizenFactory.anchors_Production.Count);

                    foreach (string adult in ad.adults)
                    {
                        GameObject template = (GameObject)Resources.Load(adult);
                        if (template != null)
                            CitizenFactory.listaTemplates_Adults_Production.Add(template);
                        else
                            Debug.Log("Nao consegui encontrar a resource (GameObject) espcificada:" + adult);
                    }
                    foreach (string child in ad.children)
                    {
                        GameObject template = (GameObject)Resources.Load(child);
                        if (template != null)
                            CitizenFactory.listaTemplates_Children_Production.Add(template);
                        else
                            Debug.Log("Nao consegui encontrar a resource (GameObject) espcificada:" + child);
                    }
                    Debug.Log("LoadXML:Carregou templates Adult Citizens " + CitizenFactory.listaTemplates_Adults_Production.Count);
                    Debug.Log("LoadXML:Carregou templates Children " + CitizenFactory.listaTemplates_Children_Production.Count);
                  

                    for (int i = 0; i < ad.worker.Count; i++)
                    {
                        TrabalhadorData worker = ad.worker[i];
                        ClassWorkerProducao family = new ClassWorkerProducao();
                        family.tipo = (Profession.ProfessionType)worker.tipo;
                        family.count = worker.count;
                        family.boneco = (GameObject)Resources.Load(worker.nome);

                        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                        go.name = "KeyAnchor" + i;
                        go.transform.position = new Vector3(worker.posicao.x, worker.posicao.y, worker.posicao.z);
                        Renderer renderer = go.GetComponent<Renderer>();
                        renderer.enabled = false;
                        CitizenFactory.keys_Production.Add(go);
                        family.posicaoInicial = go;

                      
                      //  string dataPath = string.Empty;
                        //if (Application.platform == RuntimePlatform.IPhonePlayer)
                        //    dataPath = System.IO.Path.Combine(Application.persistentDataPath, "Resources/EasyPopulation" + worker.animacao);
                        //else
                        //    dataPath = System.IO.Path.Combine(Application.dataPath, "Resources/EasyPopulation" + worker.animacao);

                        AnimationClip[] anim = Resources.FindObjectsOfTypeAll<AnimationClip>();
                        foreach (AnimationClip ac in anim)
                            if (ac.name == worker.animacao)
                                family.animacaoInicial = ac;
                            else if (ac.name == worker.animacao2)
                                family.animacao2 = ac;

                        //AnimationClip aclip1 = Resources.Load(path, typeof(AnimationClip)) as AnimationClip;
                        //family.animacaoInicial = aclip1;
                        //if (!aclip1) Debug.LogError("XML Loader-1 Clip Production, failed to load " + path);

                        // AnimationClip aclip2 = Resources.Load(path, typeof(AnimationClip)) as AnimationClip;
                        //family.animacaoInicial = aclip2;
                        //if (!aclip2) Debug.LogError("XML Loader-1 Clip Production, failed to load " + path);
                        // family.animacao2 = aclip2;

                        GameObject goDelivery = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                        goDelivery.name = "KeyAnchor" + i;
                        goDelivery.transform.position = new Vector3(worker.delivery.x, worker.delivery.y, worker.delivery.z);
                        renderer = goDelivery.GetComponent<Renderer>();
                        renderer.enabled = false;
                        CitizenFactory.keys_Production.Add(goDelivery);
                        family.posicaoDelivery = goDelivery;

                        family.raioAtracao = worker.raioAtracao;
                        family.freedom = worker.freedom;
                        //--//

                        CitizenFactory.listaDefinicaoWorkers.Add(family);
                    }
                    Debug.Log("LoadXML:Carregou workers ");
                }
            }
        }
}//eo classs
}//eo namespace
