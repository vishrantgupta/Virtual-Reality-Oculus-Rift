using UnityEngine;
using System.Collections;
/*
Processes the metabolic functions 
a) generating energy from resources
b) hormonal production and regulation 
 */
public class Metabolism : MonoBehaviour {
    public float metabolicRate_Day = 5.151f; //consumo de energia
    public float metabolicRate_Night = 1.151f;
    float speedOfChemicalConversion = 5.151f;
    
    int day_night_cycle_duration = 0;
  
	public float connectedness = 0;    
	const float multiplierConnectedness = 15f;
	public float decayUnitOfConnectedness = 0.1f;
	public float boostConnectednessWithEncounters = 5f; //Chamado em InteractionHandler quando ha interacoes = 50*o decay e em Actions
	public float connectednessLevelForSuccessThreshold; //Usado em BehaviorAI como threshold (se é menor activa)
    
    public float energy = 0;
    public float decayUnitOfEnergy = 0.1f;
    public float gajoFomeEnergyThereshold;
    const float multiplierEnergy = 15f;
  

    public float[] chemicals = new float[3];	//--chemical repository 
	public float age=0;	
	public float digestionTime=10;
    public DNA blueprint;
   
	//---------------------------------------------------------------------------------------------
	public void initialize(int[] dna)
    {   //--------//
        blueprint = GetComponent<DNA>();
        //--------//     
        chemicals[0] = -1;
		chemicals[1] = -1;
		chemicals[2] = -1;
		int max = 200;
	    //--//os repositorios quimicos sao inicializados a partir da body chemistry 
		if (dna[10] ==1)
            chemicals[0] = (int)Random.Range(0, max); 
        //quantity = max - quantity;
		if (dna[11] ==1)
            chemicals[1] = (int)Random.Range(0, max);
        //quantity = max - quantity;		
		if (dna[12] ==1 || (dna[10]==0 && dna[11]==0))
            chemicals[2] = (int)Random.Range(0, max);
        //--------//
        day_night_cycle_duration = blueprint.getSleepCycle();
        day_night_cycle_duration += (int)(Random.value * blueprint.getSleepCycle() / 3);
        //--------//
        gajoFomeEnergyThereshold = blueprint.getEnergyTolerance() * 15 + multiplierEnergy;//[0,60] mais precisamente {15, 30, 45, 60} 
                                                                                          //--------//
                                                                                          // connectednessLevelForSuccessThreshold = blueprint.getConnectedness()*0.5f + multiplierConnectedness; //[0,5] mais precisamente {0.75, 1.25, 1.75, 2.25}
        connectednessLevelForSuccessThreshold = blueprint.getConnectedness() * 15f + multiplierConnectedness; //[0,5] mais precisamente {0.75, 1.25, 1.75, 2.25}
        //--------//
        energize();
        connectedize();

    }
    //-----------------------------------------------------------------------------------------------
    public void energize()
    { energy = gajoFomeEnergyThereshold + (Random.value * 50) + -1*gajoFomeEnergyThereshold/5;
    }
    public void connectedize()
    { connectedness = connectednessLevelForSuccessThreshold + (Random.value * 50) + -1* connectednessLevelForSuccessThreshold/5;

	}
    //---------------------------------------------------------------------------------------------
    bool day = true;
	public void performMetabolicFunctions()
	{
		age+=blueprint.agingFactor;
	    
    	float minutes=Mathf.RoundToInt(Time.time/60);
        if (minutes == day_night_cycle_duration)
        { day = !day; }
        if (day) 
		{   //quando e dia
            connectedness -= decayUnitOfConnectedness*metabolicRate_Day;
            //respiracao 
            energy -= decayUnitOfEnergy * metabolicRate_Day;
        }		
		else 
		{  //night 	
            connectedness -= decayUnitOfConnectedness * metabolicRate_Night;
            //respiracao 
            energy -= decayUnitOfEnergy * metabolicRate_Night;
         }

        //convert food in energy
        digestionTime--; 
		if ((digestionTime<0) && (energy<100))
		{	digestionTime=10; 			
			energy+=convertEnergy();
		}				
	}	
	//----------------------------------------------------------------------------------------------------	
	private float convertEnergy()	{
		float total=0;
		for(int i=0; i<3; i++)
			if (chemicals[i] >0)	
		        {chemicals[i]-=speedOfChemicalConversion;
			        if(chemicals[i]>0) 
				        total += speedOfChemicalConversion;
		        }
		if(total< speedOfChemicalConversion) 
			total=0; 		  
		
		return total;			
	}
	//---------------------------------------------------------------------------------------------------
	public void die(){
		blueprint = null;}
}//eo class
