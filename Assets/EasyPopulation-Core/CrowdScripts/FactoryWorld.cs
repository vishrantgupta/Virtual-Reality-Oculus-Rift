using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
namespace Crowd
{
  //POpulation is Declared in Population manager
    public class FactoryWorld : MonoBehaviour
    {
        //--//
       // [SerializeField]
        //public bool isPopulationReadyToExport;
        //bool bPreviousStateOfPopulationReadyToExport;
        //--//
        void Awake()
        {
          
            Application.runInBackground = true;

            Global.gameState = Global.GameStateType.GAME_INITIALIZING;
            gameObject.AddComponent<Global>();

            //bPreviousStateOfPopulationReadyToExport = isPopulationReadyToExport;
            //-----//
            //if (Application.isEditor)//Returns true if the game is being run from the Unity editor; false if run from any deployment target.
            //    Global.gameMode = Global.GameModeType.EDIT_MODE;
            //else
            //    Global.gameMode = Global.GameModeType.PRODUCTION_MODE;  

        }
        //--//
        void Start() {
         
            if (Application.isPlaying)
            {     
                gameObject.AddComponent<CitizenFactory>();//Para adicionar Markov Template
                CitizenFactory citizens = GetComponent<CitizenFactory>();

                //Se estiver em PRODUCAO
                //Load data to Citizen Factory. 
                if (!Application.isEditor)
                   {
                    XMLLoader xml = new XMLLoader();
                    xml.LoadItems();
                }

                //Cria populacao
                CitizenFactory.qtGajosBeginingPopulation = CitizenFactory.quantity_Production;
                citizens.start();                   
            }
        }

        //void Update()
        //{
        //    Debug.Log(Time.time + " --------------------- Previous " + bPreviousStateOfPopulationReadyToExport + " Actual " + isPopulationReadyToExport);
        //}
        //void OnGUI()
        //{

        //    if (Application.isEditor)// && PlayerPrefs.GetInt("EditorMode") == 0)
        //    {            
        //        // Make a background box
        //        //  GUI.Box(new Rect(10, 10, 200, 90), "Population Manager", "Top-Left");
        //        GUI.Box(new Rect(20, 10, 200, 80), "Population Manager");
        //        if (GUI.Button(new Rect(30, 40, 180, 20), "Reactivate Edition Mode"))
        //        {
        //           reactivatePopulationManager();
        //            Debug.Log("Clickou em reactivate");
        //        }
        //    }
        //}




//#if UNITY_EDITOR
//        private void RefreshAssets()
//        {
//            UnityEditor.AssetDatabase.Refresh();
//        }
        
//        private void OnValidate()
//        {
//          Debug.Log("OnValidate" + Time.time + " > " +  isPopulationReadyToExport);
//          Debug.Log("FactoryWorld OnValidate(): CitizenFactory.quantity_Production: " + CitizenFactory.quantity_Production);

////        if (Application.isEditor)
////        {
//          Debug.Log(Time.time + " Previous " + bPreviousStateOfPopulationReadyToExport + " Actual " + isPopulationReadyToExport);
//          Debug.Log(CitizenFactory.quantity_Production);
//          if (bPreviousStateOfPopulationReadyToExport != isPopulationReadyToExport)
//                {                  
//                    if (isPopulationReadyToExport)
//                    {
//                        disablePopulationManager();
//                    }
//                    else
//                    {
//                        enablePopulationManager();
//                    }
//                    bPreviousStateOfPopulationReadyToExport = isPopulationReadyToExport;
//                    RefreshAssets();
//                }
////      }
//        }//OnValidate
//#endif
        //void enablePopulationManager()
        //{
        //    string destinationDirectory = Application.dataPath + "/Actors/Easy-Population/CrowdScripts/gajo/XEditor";
        //    string sourceDirectory = Application.dataPath + "/Actors/Easy-Population/CrowdScripts/gajo/Editor";
        //    moveFiles(sourceDirectory, destinationDirectory);            
        //}//reactivate

        //void disablePopulationManager()
        //{
        //    ShippingHandler.PrepareShipping();
        //    string destinationDirectory = Application.dataPath + "/Actors/Easy-Population/CrowdScripts/gajo/Editor";
        //    string sourceDirectory = Application.dataPath + "/Actors/Easy-Population/CrowdScripts/gajo/XEditor";
        //    moveFiles(sourceDirectory, destinationDirectory);
           
        //}//disable 

        //void moveFiles(string source, string dest)
        //{
        //    try
        //    {
        //        if (System.IO.Directory.Exists(source))
        //        {
        //            if (System.IO.Directory.Exists(dest))
        //                System.IO.Directory.Delete(dest);
        //            System.IO.Directory.Move(source, dest);
        //            if (System.IO.Directory.Exists(source))
        //                System.IO.Directory.Delete(source);
        //        }
        //        else
        //        {
        //            Debug.Log("FactoryWorld: MoveFiles: Origin folder não existe!");
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Debug.LogException(e);
        //    }
        //}//move files

    }//class
}//namespace
