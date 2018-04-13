#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Text;


namespace Crowd{
public class GajoDebugger : EditorWindow{

	private string text="";

	[MenuItem("Window/Debug Gajo")]
	public static void ShowWindow()
	{	//Show existing window instance. If one doesn't exist, make one.
		EditorWindow.GetWindow(typeof(GajoDebugger));
	}
	//-------------------------------------------------------------------------------------------------------
	void Update(){
			if (Global.gameState > 0) {
				text = "";
                GajoCitizen g = CitizenFactory.listaGajos[Global.whoIsBeingTracked];
				if(g!=null)
				{
				text += "\n";
				if(g.blueprint!=null)
					{text += "\n Id:" + g.getId () + "  Blueprint " + g.blueprint.dna;
				     text += "\n";
					}
				if(g.metabolism!=null)
					text += "\nEnergy " + g.metabolism.energy;
				text += "\nProxemia " + g.proxemia;
				text += "\nDipatcherSpeed " + g.behavior.dispatcherSpeed;
                if (g.metabolism != null)
                {
                    text += "\nChemicals " + g.metabolism.chemicals[0] + " - " + g.metabolism.chemicals[1] + " - " + g.metabolism.chemicals[2] + "\n";

            /*        text += "\nTestosterona " + g.metabolism.ht_testosterone;
                    text += "\nAdrenaline " + g.metabolism.ha_adrenaline;
                    text += "\nLeptine " + g.metabolism.hl_leptin;
                    text += "\nMelatonin " + g.metabolism.hm_melatonin;
                    text += "\nSerotonin " + g.metabolism.hs_serotonin;
              */  }

				text += " ";
				if(g.walker!=null)
					{
					text += "Walk Speed:" + g.walker.Animacao.GetFloat ("WalkingSpeed");//change to Movement	
                    text += "velocity anim "+ g.walker.Animacao.velocity;
                //	text += " ActionAnimation:" + g.walker.animator.GetFloat ("ActionAnimation");//change t
                    }
				text += "\n Message was dispatched:" + Global.whatAmIDoing;
				
				text += "\n";
				text += "\n";
				if (g.behavior.dispatcher!=null)
                    if (g.behavior.dispatcher != null)
					text += g.behavior.getDebugString ();	

//deprecated				if (g.walker!=null)
//deprecated					text += g.walker.Navigation.getDebugString();
				}	




				Repaint ();
			}
		}
	//-------------------------------------------------------------------------------------------------------		 
	void OnGUI() {
			EditorGUILayout.LabelField ("Gajo: " + Global.whoIsBeingTracked, EditorStyles.boldLabel);			
			EditorGUILayout.LabelField ("Action: " + Global.whatAmIDoing, EditorStyles.boldLabel);	

			EditorGUILayout.TextArea (text, EditorStyles.label);
			Global.optionBellRing = EditorGUILayout.Toggle ( "Call - On/Off", Global.optionBellRing);		
	}
	//-------------------------------------------------------------------------------------------------------	
	}//class
}//namespace
#endif