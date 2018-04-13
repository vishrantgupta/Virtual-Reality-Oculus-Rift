using UnityEngine;
using System.Collections;
using System.Collections.Generic; //para lists
using System.Xml;//basic xml attributes
using System.Xml.Serialization;//access xml serializer
using System.IO;//file management 
using System.Xml.Linq;
namespace Crowd
{
    /*Precisa alterar em 
        Note: Nomes dos assets nao podem ter underscores tipo pick_fruit nao pode! Tem que ser pickFruit
    */
   
     //--------------------------------------------------------------------------------------------
     //FILE VARIABLES
    [System.Serializable]   
    public class populationDB
    {
        [XmlArray("Population")]
        public List<ActorData> list = new List<ActorData>();
    }

    public class ActorData
    {
        [XmlAttribute("Quantos:")]
        public int quantidade;

        [XmlArray("Citizens")]
        [XmlArrayItem("Adultos:")]
        public List<string> adults = new List<string>();

        [XmlArray("Children")]
        [XmlArrayItem("Criancas:")]
        public List<string> children = new List<string>();

        [XmlArrayItem("Anchors:")]
        [XmlArrayAttribute("Position:")]
        public List<Vector3> anchors = new List<Vector3>();

        [XmlAttribute("AnchorRadius:")]
        public int anchorRadius;
        [XmlAttribute("AnchorHeight:")]
        public int anchorHeight;

        [XmlArrayItem("Workers:")]
        [XmlArrayAttribute("Trabalhador:")]
        public List<TrabalhadorData> worker = new List<TrabalhadorData>();
    }
    //----------------------------------------------------------------------------
    //----------------------------------------------------------------------------
    //----------------------------------------------------------------------------
    [ExecuteInEditMode]
    public class XMLManager //, ISerializationCallbackReceiver
    {       
    }
       
    public struct TrabalhadorData
    {//identica a que existe em CitizenFactory, usada para guardar os dados 
        public int tipo;
        public int count;
        public string nome;
        public Vector3 posicao;
        public float raioAtracao;
        public string animacao;
        public Vector3 delivery;
        public float freedom;
        public string animacao2;
    }

}//namespace


