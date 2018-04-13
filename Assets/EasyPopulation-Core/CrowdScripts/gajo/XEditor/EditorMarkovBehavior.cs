//C# Example
#if UNITY_EDITOR
using UnityEditor;

using UnityEngine;
using System.Collections;
namespace Crowd
{
    public class EditorMarkovBehavior : EditorWindow
    {
        enum Modo { visualizacao, edicao, nova_transicao, novo_estado };
        private static Texture2D tex;
        private static Texture2D tex2;
        //	string myString = "Hello World";
        //	bool groupEnabled=true;
        bool myBool = true;
        //	float myFloat = 1.23f;
        Modo modo = Modo.visualizacao;
        int qualJanelaActiva = -1;
        const byte numMaxEstados = 20;
        const byte numMaxCondicoes = 15;
        const byte numMaxTransicoes = 200;
        bool[] condicoes = new bool[numMaxCondicoes];
        //	bool[] transicoes = new bool[numMaxTransicoes];
        public Rect[] windows = new Rect[numMaxEstados];

        public Rect windowTemplate = new Rect(20, 30, 120, 50);

        float editorWindowWidth = 500f;
        float editorWindowHeight = 500f;
        System.Array listaEstados;
        // Add menu item named "My Window" to the Window menu
        [MenuItem("Window/Behavior Editor")]
        public static void ShowWindow()
        {   //Show existing window instance. If one doesn't exist, make one.
            EditorWindow.GetWindow(typeof(EditorMarkovBehavior));
        }
        //------------------------------------------------------------------------------
        void Awake()
        {
            listaEstados = System.Enum.GetValues(typeof(MarkovTemplate.Sensor));
            modo = Modo.visualizacao;

            for (int i = 0; i < MarkovTemplate.stateList.Count; i++)
            {
                windows[i] = windowTemplate;
                windows[i].x = editorWindowWidth / MarkovTemplate.stateList.Count * i + windows[i].width / 2;
                windows[i].y = editorWindowHeight / MarkovTemplate.stateList.Count * i + windows[i].height / 2;
            }
            for (int i = 0; i < numMaxCondicoes; i++)
                condicoes[i] = true;
            tex = new Texture2D(1, 1, TextureFormat.RGBA32, false);

            tex.SetPixel(0, 0, new Color(0.55f, 0.84f, 0.85f));
            tex.Apply();
            tex2 = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            tex2.SetPixel(0, 0, new Color(0.89f, 0.85f, 0.80f));
            tex2.Apply();

        }
        //------------------------------------------------------------------------------
        void OnGUI()
        {
            GUI.DrawTexture(new Rect(0, 0, maxSize.x, maxSize.y), tex, ScaleMode.StretchToFill);
            editorWindowWidth = position.width;
            editorWindowHeight = position.height;
            //Initialize Windows
            BeginWindows();
            fazDiagrama();
            fazJanelaOptions();

            EndWindows();
            processaActions();
        }
        //------------------------------------------------------------------------------
        void processaActions()
        {
            if (modo == Modo.novo_estado)
            {
                addNewState();
                modo = Modo.visualizacao;
            }
            if (modo == Modo.nova_transicao)
            {
            }
        }
        //FAZ JANELA OPTIONS
        //------------------------------------------------------------------------------
        Rect janela = new Rect(1, 1, 110, 50);
        void fazJanelaOptions()
        {
            janela = GUI.Window(999, janela, fazOptions, "Options");
        }

        //	string nomeEstado="";
        void fazOptions(int windowId)
        {

            GUI.DrawTexture(new Rect(0, 15, maxSize.x, maxSize.y), tex2, ScaleMode.StretchToFill);
            editorWindowWidth = position.width;
            editorWindowHeight = position.height;

            GUI.DragWindow(new Rect(0, 0, 10000, 20));
            //-//
            if (modo == Modo.visualizacao)
            {
                janela.width = 110;
                janela.height = 50;
                if (GUI.Button(new Rect(5, 20, 100, 20), "Add new state"))
                {
                    modo = Modo.novo_estado;
                }
            }
            //-//
            if (modo == Modo.edicao)
            {
                janela.width = 200;
                janela.height = 50;
                if (GUI.Button(new Rect(5, 20, 30, 20), "<-"))
                {
                    modo = Modo.visualizacao;
                }
                if (GUI.Button(new Rect(35, 20, 150, 20), "New Transition"))
                {
                    modo = Modo.nova_transicao;
                }
                dialogEdicao();
            }
            //-//
            if (modo == Modo.nova_transicao)
            {
                janela.width = 180;
                janela.height = 180;
                verticalSpace();
                if (GUI.Button(new Rect(5, 20, 30, 20), "<-"))
                {
                    modo = Modo.visualizacao;
                }
                if (GUI.Button(new Rect(35, 20, 135, 20), "Create"))
                {
                    modo = Modo.edicao;
                    guardaTransicao();
                }
                dialogAddNewTransition();
            }
        }
        //-------------------------------------------------------------------------
        //ADD NEW STATE
        //--------------------------------------------------------------------------
        void addNewState()
        {
            int index = MarkovTemplate.stateList.Count;
            windows[index] = windowTemplate;
            windows[index] = GUI.Window(index, windows[index], DoMyWindow, "State " + index);
            windows[index].x += windows[index].x + index * 10;
            windows[index].y += windows[index].y + index * 10;
            MarkovTemplate.newState("State Name");
        }

        void verticalSpace()
        {
            EditorGUILayout.LabelField("     ", EditorStyles.boldLabel);//PAra passar por cima dos botoes
        }
        //-------------------------------------------------------------------------
        //EDICAO
        //--------------------------------------------------------------------------
        Vector2 scrollPos = new Vector2(0, 0);

        void dialogEdicao()
        {

            Estado estado = MarkovTemplate.getState(qualJanelaActiva);
            if (estado != null)
                if (estado.transicoes.Count == 0)
                    janela.height = 100;
                else
                    janela.height = 500;

            verticalSpace();

            EditorGUILayout.LabelField("State: " + qualJanelaActiva, EditorStyles.boldLabel);

            ((Estado)MarkovTemplate.getState(qualJanelaActiva)).titulo = EditorGUILayout.TextField(estado.titulo,
                                                      GUILayout.Width(125));//, GUILayout.Width(75),GUILayout.ExpandWidth(true),EditorStyles.textField);
            verticalSpace();

            //Visualiza Check Boxes das Transicoes 
            if (estado != null)
            {
                scrollPos =
                    EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(190), GUILayout.Height(300));

                //Para todas as transicoes deste estado
                for (int t = estado.transicoes.Count - 1; t >= 0; t--)
                {
                    bool aux = false;
                    Estado.Transicao transicao = (Estado.Transicao)estado.transicoes[t];
                    aux = condicoes[t];
                    condicoes[t] = EditorGUILayout.BeginToggleGroup("Transition to: " + transicao.destino, condicoes[t]);
                    if (aux != condicoes[t])
                        if (!condicoes[t])
                        {
                            estado.transicoes.RemoveAt(t);
                            condicoes[t] = true;
                        }
                    //Condicoes 
                    foreach (MarkovTemplate.Sensor sTemplate in listaEstados)
                    {
                        aux = false;
                        int idx = -1;
                        for (int s = 0; s < transicao.condicao.Count; s++)
                        {
                            MarkovTemplate.Sensor sr = (MarkovTemplate.Sensor)transicao.condicao[s];
                            if (sr == (MarkovTemplate.Sensor)sTemplate)
                            { aux = true; idx = s; }
                        }
                        myBool = EditorGUILayout.Toggle("Sensor: " + sTemplate, aux);
                        if (aux != myBool)
                            if (myBool)
                                transicao.condicao.Add(sTemplate);
                            else
                                transicao.condicao.RemoveAt(idx);
                    }
                    EditorGUILayout.EndToggleGroup();
                }
                EditorGUILayout.EndScrollView();
                //EditorGUILayout.EndVertical();
            }
        }
        //--------------------------------------------------------------------------
        //ADD NEW TRANSITION
        int targetId = 0;
        void dialogAddNewTransition()
        {

            Estado e = (Estado)MarkovTemplate.stateList[qualJanelaActiva];
            EditorGUILayout.LabelField("State" + e.id, EditorStyles.boldLabel);
            targetId = EditorGUILayout.IntField("Transition to state: ",
                                           targetId);

            if (targetId < 0)
                targetId = 0;
            if (targetId > listaEstados.Length)
                targetId = listaEstados.Length;

            verticalSpace();
            for (int i = 0; i < listaEstados.Length; i++)
            {
                MarkovTemplate.Sensor sTemplate = (MarkovTemplate.Sensor)listaEstados.GetValue(i);
                condicoes[i] = EditorGUILayout.Toggle("Sensor: " + sTemplate, condicoes[i]);
            }
        }
        void guardaTransicao()
        {
            Estado e = (Estado)MarkovTemplate.stateList[qualJanelaActiva];
            Estado.Transicao transicao = new Estado.Transicao();
            System.Array listaEstados = System.Enum.GetValues(typeof(MarkovTemplate.Sensor));

            ArrayList transitionCondition = new ArrayList();
            for (int i = 0; i < listaEstados.Length; i++)
            {
                if ((bool)condicoes[i])
                {
                    transitionCondition.Add(listaEstados.GetValue(i));
                }
            }
            transicao.destino = targetId;
            transicao.condicao = transitionCondition;
            e.transicoes.Add(transicao);
        }


        //------------------------------------------------------------------------------
        //DIAGRAM
        //---------------------------------------------------------------------------------
        void fazDiagrama()
        {   //Faz todas as windows 
            for (int i = 0; i < MarkovTemplate.stateList.Count; i++)
            {
                windows[i] = GUI.Window(i, windows[i], DoMyWindow, i + " - " + ((Estado)MarkovTemplate.stateList[i]).titulo);
            }

            for (int i = 0; i < MarkovTemplate.stateList.Count; i++)
            {
                //Faz todas as linhas das transicoes 
                ArrayList bucket = new ArrayList();
                ArrayList bucketTexto = new ArrayList();

                Estado s = (Estado)MarkovTemplate.stateList[i];
                if (s.transicoes.Count > 0)
                {
                    //Prepara
                    foreach (Estado.Transicao t in s.transicoes)
                    {//vai verificar se ja existe no bucket
                        bool existeNoBucket = false;
                        int indice = -1;
                        string str = "";
                        for (int b = 0; b < bucket.Count; b++)
                        {
                            if ((int)bucket[b] == t.destino)
                            {
                                existeNoBucket = true;
                                indice = b;
                            }
                        }
                        if (existeNoBucket)
                        {
                            str = (string)bucketTexto[indice] + "\n";
                            str += t.getCondicaoAsString();
                            bucketTexto[indice] = str;
                        }
                        else
                        {
                            bucket.Add(t.destino);
                            str = t.getCondicaoAsString();
                            bucketTexto.Add(str);
                        }
                    }//for

                    //LEGENDAS + LINHAS + ARROWS 
                    for (int b = 0; b < bucket.Count; b++)
                    {
                        int dest = (int)bucket[b];
                        float tangent = Mathf.Clamp((-1) * (windows[i].x - windows[dest].x), -100, 100);
                        Vector2 endtangent = new Vector2(windows[i].x - tangent, windows[dest].y);
                        if (dest != i)
                        {
                            //Lines	
                            Handles.DrawBezier(windows[i].center,
                                                  windows[dest].center,
                                                  endtangent,
                                                  endtangent,
                                                  Color.red,
                                                  null,
                                                  1);
                            //Labels			                   
                            Handles.Label((windows[dest].center + endtangent) / 2, (string)bucketTexto[b]);
                            //Setas
                            Handles.color = Color.red;
                            Vector2 pivot = (windows[dest].center + endtangent) / 2;
                            float r = Vector2.Distance(pivot, windows[dest].center);
                            float angle = Mathf.Atan2(pivot.x - windows[dest].center.x, pivot.y - windows[dest].center.y);
                            Vector2 target1 = new Vector2(r * Mathf.Cos(angle + 15), r * Mathf.Sin(angle + 15));
                            target1.Normalize();
                            target1 *= 10;
                            Handles.DrawLine(pivot, pivot - target1);
                            Vector2 target2 = new Vector2(r * Mathf.Cos(angle - 15), r * Mathf.Sin(angle - 15));
                            target2.Normalize();
                            target2 *= 10;
                            Handles.DrawLine(pivot, pivot + target2);
                        }
                        else //De si para si proprio
                        {
                            Handles.color = Color.red;
                            
                            Handles.CircleCap(1, windows[i].center - Vector2.up * 35 - Vector2.left * 10, Quaternion.identity, 30);
                            Handles.Label((windows[i].center - Vector2.up * 70), (string)bucketTexto[b]);

                            Vector2 pivot = windows[i].center - Vector2.up * 25 - Vector2.left * 40;
                            Handles.DrawLine(pivot - Vector2.up * 10 - Vector2.left * 10, pivot);
                            Handles.DrawLine(pivot - Vector2.up * 10 + Vector2.left * 10, pivot);
                        }
                    }// Para todos os que estao no bucket
                }//Se tem transicoes.Count >0 
            }// Para todos as transicoes
             //EndWindows ();
        }

        //------------------------------------------------------------------------------
        void DoMyWindow(int windowID)
        {

            if (GUI.Button(new Rect(5, 20, 55, 20), "Edit"))
            {
                GUI.BringWindowToFront(windowID);
                modo = Modo.edicao;
                qualJanelaActiva = windowID;
                for (int i = 0; i < numMaxCondicoes; i++)
                    condicoes[i] = true;
            }
            if (GUI.Button(new Rect(55, 20, 55, 20), "Delete"))
            {
                MarkovTemplate.deleteState(windowID);
            }

            if (Event.current.type == EventType.MouseDown && modo == Modo.edicao)
            { qualJanelaActiva = windowID; }


            //GUI.DragWindow();
            GUI.DragWindow(new Rect(0, 0, 10000, 20));
        }
    }//eoclass
}
#endif