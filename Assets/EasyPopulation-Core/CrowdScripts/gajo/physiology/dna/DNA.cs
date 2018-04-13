
using UnityEngine;
using System.Collections;
/*
Recebe dna de 1 parent e faz mutacoes de cada alela (com probabilidade de 10%)

DNA:
2 bytes  - age
2 bytes  - introspection
3 bytes  - hormone cycles
3 bytes  - hormon uptakes
3 bytes  - body chemistry
2 bytes  - Gregariousness - Connectedness tolerance
3 bytes  - Metabolic needs (what chemicals can be digested)
1 byte	 - Sex
3 byte   - duration in minutes (base) of active/sleep cycle
6 byte   - Personality - Pleasure
6 byte   - Personality - Arousal
6 byte   - Personality - Dominance
2 bytes  - Fome tolerance
*/

public class DNA : MonoBehaviour
{
    public static int DNA_LENGHT = 42;
	public int[] dna=new int[DNA_LENGHT];
	public float agingFactor=0; //Usado aqui no metabolismo
	public float introspectionFactor=0;//usado no Dispatcher

//---------------------------------------------------------------------------------------------------- 
	public void initializeGType(int[] dnaMyParent)	{
        if (dnaMyParent == null)
            { initDNA();
        }		
		else	
		    {	dna=dnaMyParent;
			    mutacoes();							
		    }

        int[] aux= { dna[0], dna[1] };
		agingFactor=Utils.convertFromBinaryValue(aux)*0.1f;

        aux[0] = dna[2]; aux[1]=dna[3];		
		introspectionFactor=Utils.convertFromBinaryValue(aux)*0.1f;
	
	}			
//----------------------------------------------------------------------------------------------------
	private void initDNA()	{ 
		//gAge-Age	
		initAlleles(0,2);
         //gPerc-Introspection
        initAlleles(2,2);
        //gCycx-Hormon Cycles
        initAlleles(4,3);
        //gUpx-Hormon Uptakes	
        initAlleles(7,3);
		//gChem-Chemistry of body 	
		initAlleles(10,3);
        //Gregariosness-Connectedness Threshold
        initAlleles(13, 2);
        //gMetab-Metabolic needs (what can he digest)	
        initAlleles(15,3);
		//SEXO
		initAlleles(18,1);
        //duration sleep/active cycle (in minutes)
        initAlleles(19,3);
        //personality - P
        initAlleles(22,6);
        //personality - A
        initAlleles(28,6);
        //personality - D
        initAlleles(34,6);
        //Fome tolerance
        initAlleles(40, 2);
    }  
//----------------------------------------------------------------------------------------------------	
	private void initAlleles(int index, int qt)	{        
		for(var i=index; i<index+qt; i++)
		{	if (Random.value > 0.5f)
                dna[i] = 1;
            else dna[i] = 0;
		}
    }	
//----------------------------------------------------------------------------------------------------		
	private void mutacoes()	{
		//--Cada alela tem probabilidade de mutacao de 10%	
		for(int i=0; i<dna.Length; i++) 
		    {if (Random.value < 0.1f) //10% hipotese de mutacao em cada alela		    
                if (dna[i] == 0)
                    dna[i] = 1;// Utils.replaceChar(dna, i, "1"); 
                else dna[i] = 0;// = Utils.replaceChar(dna, i, "0"); 
		    }		
	}
//----------------------------------------------------------------------------------------------------
    public int getSleepCycle()
    {   int[] sleepCycle={ dna[19], dna[20], dna[21]};
        return Utils.convertFromBinaryValue(sleepCycle);
    }
    public int getP()
    {   int[] sleepCycle = { dna[22], dna[23], dna[24], dna[25], dna[26], dna[27] };
        return Utils.convertFromBinaryValue(sleepCycle);
    }
    public int getA()
    {   int[] sleepCycle = { dna[28], dna[29], dna[30], dna[31], dna[32], dna[33] };
        return Utils.convertFromBinaryValue(sleepCycle);
    }
    public int getD()
    {   int[] sleepCycle = { dna[34], dna[35], dna[36], dna[37], dna[38], dna[39] };
        return Utils.convertFromBinaryValue(sleepCycle);
    }
    public int getConnectedness()
    {
        int[] conn = { dna[13], dna[14] };
        return Utils.convertFromBinaryValue(conn);
    }
    public int getEnergyTolerance()
    {
        int[] nrg = { dna[40], dna[41] };
        return Utils.convertFromBinaryValue(nrg);
    }
    //---//
    public Vector3 getPersonality()
    {   float P, A, D;
     
	/*	P = getP(); //Valor entre [0..63]        
        if (P < 32) P *= -1;
            else P -= 32;//Metade esquerda são negativos 
        P = P  / 32; //Proporção numa escala de 32: se tem isto em 32 quanto teria em 1
        A = getA();
        if (A < 32) A *= -1;
        else        A -= 32;
        A = A / 32;
        D = getD();
        if (D < 32) D *= -1;
        else        D -= 32;
        D = D / 32;
    */
		P = getP();//Proporção numa escala de 63: se tem isto em 63 quanto teria em 1
		P = P / 63;
		A = getA();
		A = A / 63;
		D = getD();
		D = D / 63;
        return new Vector3(P, A, D);
    }
//----------------------------------------------------------------------------------------------------	
	public void die()
	{dna = null;}
}//eo class