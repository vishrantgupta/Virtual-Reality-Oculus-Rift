using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using System.IO;
using System.Text;
using System.Xml;
//using UnityEditor;
using System;

namespace Crowd {
    public class ShippingHandler {
      public static void PrepareShipping() {
            OnPrepareShipping();
        }

        private static void OnPrepareShipping()
        {
            saveDataToXML();
           // copyAssetsToResources();
            //moveFolder();
        }
        //---------------------------------------
        private static void saveDataToXML()
        {
            try
            {
                XMLSaver xml = new XMLSaver();
                xml.SaveItems();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
        //private static void moveFolder()
        //{
        //    string sourceDirectory = Application.dataPath + "/Actors/Easy-Population/CrowdScripts/gajo/XEditor";
        //    string destinationDirectory = Application.dataPath + "/Actors/Easy-Population/CrowdScripts/gajo/Editor";
       
        //     if (System.IO.Directory.Exists(destinationDirectory))
        //        System.IO.Directory.Delete(destinationDirectory);
        //    System.IO.Directory.Move(sourceDirectory, destinationDirectory);
        //    if (System.IO.Directory.Exists(sourceDirectory))
        //        System.IO.Directory.Delete(sourceDirectory);
        //    AssetDatabase.Refresh();
        //}
        //---------------------------------
        //private static void copyAssetsToResources()
        //{   //NOTE: 
        //    //This should Move the assets to the Resource folder, not copy them, 
        //    //but for some reason AssetDatabase.Move, seems to forbid moving the assets from within the asset folder
        
        //        if (!AssetDatabase.IsValidFolder("Assets/Resources/Easy-Population"))
        //        {
        //            AssetDatabase.CreateFolder("Assets/Resources", "Easy-Population");
        //        }

        //        foreach (GameObject adult in CitizenFactory.listaTemplates_Adults_Production)
        //        {
        //            if (adult != null)
        //            {
        //                string origin = AssetDatabase.GetAssetPath(adult);
        //                string destination = Application.persistentDataPath + "/Assets/Resources/Easy-Population" + "/" + adult.name + ".fbx";
        //                AssetDatabase.CopyAsset(origin, destination);
        //            }
        //        }
        //        foreach (GameObject child in CitizenFactory.listaTemplates_Children_Production)
        //        {
        //            if (child != null)
        //            {
        //                string origin = AssetDatabase.GetAssetPath(child);
        //                string destination = Application.persistentDataPath + "/Assets/Resources/Easy-Population" + "/" + child.name + ".fbx";
        //                AssetDatabase.CopyAsset(origin, destination);
        //            }
        //        }

        //       foreach (ClassWorkerProducao worker in CitizenFactory.listaDefinicaoWorkers)
        //            {
        //                if (worker.boneco != null)
        //                {
        //                    string origin = AssetDatabase.GetAssetPath(worker.boneco);
        //                    string destination = Application.persistentDataPath + "/Assets/Resources/Easy-Population" + "/" + worker.boneco.name + ".fbx";
        //                    AssetDatabase.CopyAsset(origin, destination);
        //                }
        //                if (worker.animacaoInicial != null)
        //                {
        //                    string origin = AssetDatabase.GetAssetPath(worker.animacaoInicial);
        //                    string destination = Application.persistentDataPath + "/Assets/Resources/Easy-Population" + "/" + worker.animacaoInicial.name + ".fbx";
        //                    AssetDatabase.CopyAsset(origin, destination);
        //                }
        //                if (worker.animacao2 != null)
        //                {
        //                    string origin = AssetDatabase.GetAssetPath(worker.animacao2);
        //                    string destination = Application.persistentDataPath + "/Assets/Resources/Easy-Population" + "/" + worker.animacao2.name + ".fbx";
        //                    AssetDatabase.CopyAsset(origin, destination);
        //                }
        //            }
        //}
        }//eoclass
}//eo namespace