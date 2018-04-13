using UnityEngine;
using System.Collections;
namespace Crowd
{
    public class Estado
    {
        public int id = -1;
        public MarkovTemplate.Tipo tipo;
        public string titulo;
        //Um estado pode ter mais que uma transicao para o mesmo destino (mas com condicoes diferentes)
        public ArrayList transicoes = new ArrayList();

        //-------------------------------------------------------------------
        public class Transicao
        {
            public int destino;
            public ArrayList condicao;
            public float probabilidade;
            public int numTimesPerfomed = 0;

            //---//
            public string getCondicaoAsString()
            {
                string s = "";
                foreach (MarkovTemplate.Sensor sensor in condicao)
                    s += sensor.ToString() + ",";
                return s;
            }

            public void die()
            {
                for (int i = condicao.Count-1; i > 0; i--)
                    condicao.RemoveAt(i);
            }
        }
        //-------------------------------------------------------------------

        public void addTransition(int destino, ArrayList condicao, float probab)
        {
            Transicao t = new Transicao();
            t.destino = destino;
            t.condicao = condicao;
            t.probabilidade = probab;
            transicoes.Add(t);
        }

        public void die()
        {
            for (int i = transicoes.Count; i > 0; i--)
            {
                ((Transicao)(transicoes[i])).die();
                transicoes.RemoveAt(i);
            }
            transicoes.Clear();
            transicoes = null;
        }

        /*	public string getCondicaoTransicaoAsString(int numTransicao)
            {Transicao t = transicoes [numTransicao];
             string s="";
             //if (t!=null)
            //	foreach(
            }
            */
    }
}