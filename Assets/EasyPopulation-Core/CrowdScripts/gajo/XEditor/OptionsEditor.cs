#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Text;


namespace Crowd{
	public class OptionsEditor: EditorWindow{
		
	//	private string text="";
		
		[MenuItem("Window/Simulation Options")]
		public static void ShowWindow()
		{	//Show existing window instance. If one doesn't exist, make one.
			EditorWindow.GetWindow(typeof(OptionsEditor));
		}
		void Awake(){
			//Debug.Log ((float)(5f / 10f) + " " + Mathf.Log (5f / 50f));
		}
		//-------------------------------------------------------------------------------------------------------		 
		void OnGUI() {
		//	EditorGUILayout.LabelField ("Gajo: " + Global.whoIsBeingTracked, EditorStyles.boldLabel);			
		//	EditorGUILayout.LabelField ("Action: " + Global.whatAmIDoing, EditorStyles.boldLabel);	
			
		//	EditorGUILayout.TextArea (text, EditorStyles.label);

			Global.optionMoveBetweenNurseryAndCemetery = EditorGUILayout.Toggle("Multi - On/Off", Global.optionMoveBetweenNurseryAndCemetery);
				
			Global.optionBellRing = EditorGUILayout.Toggle( "Call to the illuminated point - On/Off", Global.optionBellRing);

			Global.isMonitoringFPS = EditorGUILayout.Toggle( "Monitoring FPS - On/Off", Global.isMonitoringFPS);

			Global.maxGajos = EditorGUILayout.IntField ("Max individuals ", Global.maxGajos);

			Global.isInhibitingDeathsBirths =EditorGUILayout.Toggle( "Inhibiting Births and Deaths - On/Off", Global.isInhibitingDeathsBirths);
		}
		//-------------------------------------------------------------------------------------------------------	
	}//class
}//namesace
#endif
