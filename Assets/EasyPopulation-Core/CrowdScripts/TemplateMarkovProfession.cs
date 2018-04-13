using UnityEngine;
using System.Collections;

namespace Crowd
{
    public class TemplateMarkovProfessions
    {
        public TemplateMarkovProfessions() { } // guarantee this will be always a singleton only - can't use the constructor!


        public enum Tipo { performativo, procedural };
        public enum Sensor { ht, hu, longe, perto, conhecido, desconhecido, interact, n_interact, emergency };

        public static ArrayList stateList = new ArrayList();

        public void init()
        {
            //Cria 12 estados
            newState("S0-Origem");
            newState("S1-Producao");
            newState("S2-Entrega");
            newState("S3-Transacion");
            
            //Define as transicoes
            ArrayList cond;
            //transicao S0 com S1
            cond = new ArrayList { Sensor.hu, Sensor.perto};
            addTransition(0, 1, cond, 0.5f);
            
            //transicao S1 com S1
            cond = new ArrayList { Sensor.hu, Sensor.perto};
            addTransition(1, 1, cond, 0.5f);

            //transicao S1 com S2	
            cond = new ArrayList { Sensor.perto };
            addTransition(1, 2, cond, 0.5f);

            //transicao S2 com S2
            cond = new ArrayList {Sensor.longe};
            addTransition(0, 4, cond, 0.5f);
            
            //transicao S2 com S3
            cond = new ArrayList { Sensor.perto };
            addTransition(0, 8, cond, 0.5f);
            
            //transicao S3 com S0
            cond = new ArrayList { Sensor.ht, Sensor.longe };
            addTransition(1, 1, cond, 0.5f);

            distribuiProbabilidades();
            MarkovTemplate.showInternalMarkov();
        }
        //-------------------------------------------------------------------------
        public static void newState(string tit)
        {
            Estado s = new Estado();
            s.id = stateList.Count;
        //    s.tipo = Tipo.procedural;
            s.titulo = tit;
            stateList.Add(s);
        }
        //-------------------------------------------------------------------------
        public static void showInternalMarkov()
        {
            Debug.Log("---------------Vai carregar Markov (MarkovTemplate) ---------------------");
            foreach (Estado e in MarkovTemplate.stateList)
                foreach (Estado.Transicao t in e.transicoes)
                {
                    string s = "";
                    foreach (MarkovTemplate.Sensor sen in t.condicao)
                        s += sen.ToString();
                    Debug.Log(e.id + " " + t.destino + " " + s);
                }
            Debug.Log("---------------Carregou markov  (MarkovTemplate) ------------------------");
        }
        //-------------------------------------------------------------------------			
        public void addState(Estado s)
        {
            stateList.Add(s);
        }
        //-------------------------------------------------------------------------
        public static void deleteState(int id)
        {

            Estado estadoToDelete = getState(id);
            //Decrementa os destinos 
            //(que tenham valor acima do no a remover)
            foreach (Estado estadoLista in stateList)
            {
                ArrayList bucket = new ArrayList();
                for (int i = 0; i < estadoLista.transicoes.Count; i++)
                {
                    Estado.Transicao transicao = (Estado.Transicao)estadoLista.transicoes[i];
                    //1- Marca Transicoes onde este e o destino
                    if (transicao.destino == id)
                    {
                        bucket.Add(i);
                    }
                    //Decrementa as que sao maiores
                    if (transicao.destino > id)
                        transicao.destino--;
                }
                for (int i = bucket.Count - 1; i >= 0; i--)
                {
                    int idxB = (int)bucket[i];
                    Debug.Log(idxB);
                    estadoLista.transicoes.RemoveAt(idxB);
                }
                if (estadoLista.id > id)
                    estadoLista.id--;
                bucket.Clear();
            }//for
            if (estadoToDelete != null)
                stateList.Remove(estadoToDelete);

        }
        //-------------------------------------------------------------------------
        public static Estado getState(int id)
        {
            foreach (Estado s in stateList)
                if (id == s.id)
                    return s;
            return null;
        }
        //-------------------------------------------------------------------------	
        public void addTransition(int origin, int destiny, ArrayList condicao, float prob)
        {
            Estado s = getState(origin);
            if (s != null)
                s.addTransition(destiny, condicao, prob);
        }
        //-------------------------------------------------------------------------	
        public void distribuiProbabilidades()
        {
            //Para cada um dos estados da markov 
            //Cria um bucket com os destinos possiveis. PAra isso, vai ver cada uma transicoes para onde ]e que esta vai
            //Depois disptribui a probabilidade de acontecer cada uma das transicoes, de acordo com o numero de estados destinos:
            //: Esta no estado 2 tem a probabilidade de 50% de ir para o estado 3, dado que tem destinos 3, e 4.
            foreach (Estado s in stateList)
            {
                ArrayList destinos = new ArrayList();
                foreach (Estado.Transicao t in s.transicoes)
                {
                    bool existe = false;
                    foreach (int d in destinos)
                    {
                        if (t.destino == d)
                            existe = true;
                    }
                    if (!existe)
                        destinos.Add(t.destino);
                }

                //   foreach(int d in destinos)
                //		{
                foreach (Estado.Transicao t in s.transicoes)
                    t.probabilidade = 1f / (float)(destinos.Count);
                //		}
            }
        }
        //-------------------------------------------------------------------------
    }//eo class	
}
