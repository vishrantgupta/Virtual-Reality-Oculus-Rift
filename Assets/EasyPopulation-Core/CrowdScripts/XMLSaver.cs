using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;//access xml serializer
using System.IO;//file management 

using System.Text;
namespace Crowd
{  [System.Serializable]
    public class XMLSaver 
    {
        populationDB popDBFile;
        //PopulationManager pmInterface;
        /*---------------------------------------------------*/
        //save function
        //Sava data currently in the Inspector Panel (Population Manager) to Xml file
      
        public void SaveItems()
        {
        
            GameObject pf = GameObject.Find("EasyPopulation");
            if (pf == null)
                pf = GameObject.Find("Easy Population");
            if (pf == null)
                pf = GameObject.Find("Easy-Population");
            if (pf != null)
            {
                loadDataFromCitizenFactoryData();
                writeFile();
            }
    
        }

        private void loadDataFromCitizenFactoryData()
        {
            popDBFile = new populationDB();
          
            if (popDBFile != null)
            {
                popDBFile.list.Clear();
                ActorData ad = new ActorData();
                ad.quantidade = CitizenFactory.quantity_Production;
                foreach (GameObject adult in CitizenFactory.listaTemplates_Adults_Production)
                {   if (adult == null) Debug.LogError("There is no associated 3d model (citizen)!");
                    else 
                    ad.adults.Add(adult.name); }
                foreach (GameObject child in CitizenFactory.listaTemplates_Children_Production)
                {
                    if (child== null) Debug.LogError("There is no associated 3d model (children)!");
                    else ad.children.Add(child.name); }
                foreach (GameObject anchor in CitizenFactory.anchors_Production)
                {
                    if (anchor == null) Debug.LogError("There is no anchors (anchor)!");
                    else ad.anchors.Add(new Vector3(anchor.transform.position.x, anchor.transform.position.y, anchor.transform.position.z)); }
                ad.anchorRadius = (int)CitizenFactory.anchorRadius_Production;
                ad.anchorHeight = (int) CitizenFactory.anchorHeight_Production;
                foreach (ClassWorkerProducao worker in CitizenFactory.listaDefinicaoWorkers)                
                {
                    TrabalhadorData tr = new TrabalhadorData();
                    tr.tipo = (int)worker.tipo;
                    tr.count = worker.count;
                    if (worker.boneco != null)
                        tr.nome = worker.boneco.name;
                    if (worker.posicaoInicial != null)
                        tr.posicao = worker.posicaoInicial.transform.position;
                    tr.raioAtracao = worker.raioAtracao;
                    if (worker.animacaoInicial != null)
                        tr.animacao = worker.animacaoInicial.name;
                    if (worker.posicaoDelivery != null)
                        tr.delivery = worker.posicaoDelivery.transform.position;
                    tr.freedom = worker.freedom;
                    if (worker.animacao2 != null)
                        tr.animacao2 = worker.animacao2.name;
                    ad.worker.Add(tr);
                }
                popDBFile.list.Add(ad);
            }
            else Debug.LogError("XMLManager Before Serialize popPrep e null");
        }

        private void writeFile()
        {
            //O Load faz Resources.Load
            string dataPath = string.Empty;
            if (Application.platform == RuntimePlatform.IPhonePlayer)
                dataPath = System.IO.Path.Combine(Application.persistentDataPath, "Resources/EasyPopulation/population.xml");
            else
                //  dataPath = System.IO.Path.Combine(Application.dataPath, "Resources/EasyPopulation//population.xml");
                dataPath = System.IO.Path.Combine(Application.dataPath, "Resources/EasyPopulation/population.xml");

            XmlSerializer serializer = new XmlSerializer(typeof(populationDB));
            Encoding encoding = Encoding.GetEncoding("UTF-8");
            StreamWriter stream = new StreamWriter(dataPath, false, encoding);
            serializer.Serialize(stream, popDBFile);
            stream.Close();
        }

      
    }//eo class
}//eo namespace
   
