
using UnityEngine;
using System.Collections;

//Notas: Debugging parameters #
//Navigation -> debugNavigation

namespace Crowd {
	public class Global : Singleton<Global> {
      
        //----------------------
        //Mode 
        public enum GameStateType {GAME_INITIALIZING, GAME_READY_HAS_AT_LEAST_ONE_CHARACTER }
        public static GameStateType gameState; //GameReady=There is at least one Gajo on stage (to be updated in GajosFactory and used in GajoDebug.OnGui
                                               //Mode 
        //public enum GameModeType { EDIT_MODE, PRODUCTION_MODE}
        //public static GameModeType gameMode; //If Population Manager is available or game is ready to be deployed, or is already shipped and running
        //----------------------
        //Ha produtores 
        public static bool hasProdutores = true;
        //----------------------
         //Tem efeito directo no GajosFactory com createA e createB
        public static bool optionBellRing = false; //Se activo - Vao todos para o ponto comum onde esta o foco de luz que se acende
        public static bool optionMoveBetweenNurseryAndCemetery = true;  //Se nao estiver activo, o wander faz-se entre 2 pontos: o bercario e o cemiterio

        public static bool isInhibitingDeathsBirths = false;
        public static bool hasNurseries = true; //OS individuos saem das  nurseries 
        public static bool hasCemetery = true;
        public static bool isMonitoringFPS = false;
        public static bool followDoubleClickTarget = false;  //procura object named "Target"
        //----------------------        
		public static int numStartGajos = 50;
		public static int maxGajos = 150;
	
		public static byte maxWidth=100;
		public static byte maxDepth=100;

		public static string msg;
		public static float normalDispatcherInterval = 0.5f;
		//----------------------
		public static int whoIsBeingTracked=0;
		public static Actions.TypeOfActionBeingPerformed whatAmIDoing= Actions.TypeOfActionBeingPerformed.NEPIA;		
 	 
		//----------------------------------
		//RVO params
		/*public static float maxNeighbours = 10;
		public static float maxSpeed=2;
		public static float neighbourDist=10;
		public static float obstacleTimeHorizon=2;
		public static float agentTimeHorizon=2;
		public static bool lockWhenNotMoving=true;
		public static float radius =0.5f;
        */
		//--------------------------------------
		// Use this for initialization
		public static GameObject spot;
	
      
    }//eo class
}//eo package