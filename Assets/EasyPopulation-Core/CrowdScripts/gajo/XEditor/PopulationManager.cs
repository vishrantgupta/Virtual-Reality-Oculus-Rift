
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;

namespace Crowd
{
    //----------------------------------------------------------------------------------------------
    //-----WORKER-------WORKER-------------------WORKER--------------WORKER-------------------
    //----------------------------------------------------------------------------------------------
    //INSPECTOR VARIABLES  
    [System.Serializable]
    // [CustomEditor(typeof(PopulationManager))]
    public class Worker//: Editor
    {

        [Tooltip("Number of action states")]
        public Profession.ProfessionType type;
        [Tooltip("Quantity of characters with this profession")]
        public int count;
        [Tooltip("3d model of the Character - T-pose")]
        public GameObject actor;
        [Tooltip("Initial location - state 0")]
        public GameObject baseAnchor;
        [Tooltip("How far can the character be from the base?")]
        public float radiusAttraction;
        //[ConditionalHide("TwoAnimationStates", true)]
        [Tooltip("Animation of the character during *Production* mode - state 1")]
        public AnimationClip animationProduction;

        //  public bool TwoAnimationStates = false;
        // [ConditionalHide("TwoAnimationStates", true)]
        [ConditionalEnumHide("type", 1)]
        [Tooltip("Delivery location - Where second state takes place")]
        public GameObject deliveryLocation;

        [ConditionalEnumHide("type", 1)]
        [Tooltip("Degree of freedom of the distance to delivery point")]
        public float freedomDelivery;

        [ConditionalEnumHide("type", 1)]
        [Tooltip("Animation of the character during *Transaction* mode - state 3")]
        public AnimationClip animationTransaction;

        //[ConditionalEnumHide("type", 1)]
        private bool isWalkingAgent;
    }//Worker

    [System.Serializable]
    public class workersDatabase
    {
        [SerializeField]
        public List<Worker> list = new List<Worker>();
    }//WorkerDatabase
     //----------------------------------------------------------------------------------------------
     //----------------------------------------------------------------------------------------------
     //----------------------------------------------------------------------------------------------
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
#endif
    [ExecuteInEditMode]
    public class PopulationManager : MonoBehaviour
    {
        /*----------
          The functions are not called constantly like they are in play mode.
        - Update is only called when something in the scene changed.
        - OnGUI is called when the Game View recieves an Event.
        - OnRenderObject and the other rendering callback functions are called on every repaint of the Scene View or Game View.

         ----------------------------------------------------------------
         Thsi class handles the component visible at the Inspector panel
         This operates as an interface for data stored at CitizenFactory. 
        ----------------------------------------------------------------*/

        public bool visualizeAnchors = false;
        [Header("---------------------------------------------")]
        [Header("Standard inhabitants")]
        //Quantity
        [Range(5, 100)]
        [Tooltip("Number of citizens")]
        //            [ConditionalHide("visualizeAnchors", true)]  
        public int quantity;
        //Gajos Templates
        [Tooltip("Model templates of the citizens")]
        //            [ConditionalHide("visualizeAnchors", true)]
        public GameObject[] templatesCitizens;

        [Tooltip("Model templates of children")]
        public GameObject[] templatesChildren;
        //Bercarios 
        //      [ConditionalHide("visualizeAnchors", true)]    


        //[ReadOnly]
        // [Space(10)]
        [Header("    ")]
        //[Range(0, 100)]


        [InspectorButton("OnAddOriginClicked")]
        [Tooltip("Adds a new anchor point")]
        public bool addAnchor;
        [InspectorButton("OnClearClicked")]
        [Tooltip("Reset anchors")]
        public bool clearAll;

        [Tooltip("Key landmarks - characters will use them as way points")]
        [HideInInspector]
        public GameObject[] anchors;


        [Range(1, 50)]
        [Tooltip("This radius is used to determine the range of the initial position of Citizens (anchor, radius)")]
        public int influenceRadius;
        public static int global_influenceRadius; //necessitamos ter isto porque se o outro for static nao e visivel no ispector

        [Range(1, 20)]
        [Tooltip("Height dimension of the anchor")]
        //    [ConditionalHide("visualizeAnchors", true)]
        public int heightAnchors = 1;
        public static int global_heightAnchors;
        /// //////////////////////////////////
        private bool isAddingWorker = false;
        [Header("---------------------------------------------")]
        [Header("Workers")]
        public workersDatabase workersDB;

        [Header("    ")]

        [InspectorButton("OnAddWorkerClicked")]
        public bool addKeys;
        [InspectorButton("OnClearAllWorkerClicked")]
        public bool clearKeys;
        // [ReadOnly]
        [HideInInspector]
        public GameObject[] KeyPointsWorkers;

        //[Header("---------------------------------------------")]
        //[Header("Prepare to Finalize")]
        //[InspectorButton("OnPrepareShipping")]
        //public bool prepareShipping;

        [Header("---------------------------------------------")]
        [Header("Standard inhabitants")]

        // private XMLManager xmlman;
        private ArrayList workersOnStage;
        bool isAddingOrigin = false;

        GameObject seed;
        bool _cameraOrthoBeforeEditMode;
        Quaternion _cameraRotBeforeEditMode;
        Vector3 _cameraPositionBeforeEditMode;

        //----------------------------------------------------------------------------------------------
        public void Awake()
        {
            createLayerGajos();

            gameObject.name = "Easy Population";
            //----//
            FactoryWorld fw =gameObject.GetComponent<FactoryWorld>();
            if(fw==null)
                gameObject.AddComponent<FactoryWorld>();
            //----//

            if (UnityEngine.AI.NavMesh.GetSettingsCount() == 0)
                Debug.Log("Looks like you forgot to create a navmesh before running, so that characters can have a walkable area. Check the window>Navigation, please :-)!");

            if (anchors != null)
                for (int i = 0; i < anchors.Length; i++)
                {
                    Vector3 pos = anchors[i].transform.position;
                    float posicaoSolo = Terrain.activeTerrain.SampleHeight(pos);
                    if (pos.y != posicaoSolo)
                        anchors[i].transform.position = new Vector3(pos.x, posicaoSolo, pos.z);
                }

            if (KeyPointsWorkers != null)
                for (int i = 0; i < KeyPointsWorkers.Length; i++)
                {
                    Vector3 pos = KeyPointsWorkers[i].transform.position;
                    float posicaoSolo = Terrain.activeTerrain.SampleHeight(pos);
                    if (pos.y != posicaoSolo)
                        KeyPointsWorkers[i].transform.position = new Vector3(pos.x, posicaoSolo, pos.z);
                }
            global_influenceRadius = influenceRadius;
            global_heightAnchors = heightAnchors;
            //--//
            copyDataFromInspectorToCitizenFactory();
         
        }

        void createLayerGajos()
        {
#if UNITY_EDITOR        
            string layerName = "Gajos";
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            // For Unity 5 we need this too
            SerializedProperty layersProp = tagManager.FindProperty("layers");

            // --- Unity 5 ---
            SerializedProperty sp = layersProp.GetArrayElementAtIndex(10);

            if (sp != null) sp.stringValue = layerName;
            // and to save the changes
            tagManager.ApplyModifiedProperties();
#endif
        }

        //START-------------------------------------------------------------------------------
        void Start()
        {
            if (Application.isPlaying || !visualizeAnchors)
            {
                if (anchors != null)
                    for (int i = 0; i < anchors.Length; i++)
                    {
                        if (anchors[i] != null)
                        {
                            Renderer renderer = anchors[i].GetComponent<Renderer>();
                            renderer.enabled = false;
                        }
                        else Debug.LogError("Size is greater than number of Anchors- Press the Add Key button instead. Alternatively, readjust the size number?");
                    }
                if (KeyPointsWorkers != null)
                    for (int i = 0; i < KeyPointsWorkers.Length; i++)
                    {
                        if (KeyPointsWorkers[i] != null)
                        {
                            Renderer renderer = KeyPointsWorkers[i].GetComponent<Renderer>();
                            renderer.enabled = false;
                        }
                        else Debug.LogError("Size is greater than number of Key workers - Press the Add Key button instead. Alternatively, readjust the size number?");
                    }
            }

            copyDataFromInspectorToCitizenFactory();
 
        }
#if UNITY_EDITOR
        //ONENABLE------------------------------------------------------------------------------------------------
        /// 
        void OnEnable()
        {
            SceneView.onSceneGUIDelegate += SceneGUI;
            Selection.selectionChanged += OnSelectionChange;
            copyDataFromInspectorToCitizenFactory();
            ShippingHandler.PrepareShipping();//saves xml
            //XMLSaver xml = new XMLSaver();
            //xml.SaveItems();
        }

        void OnDisable()
        {
            SceneView.onSceneGUIDelegate -= SceneGUI;
            Selection.selectionChanged -= OnSelectionChange;
        }
#endif
#if UNITY_EDITOR
        /// ///////////////////////////////////////////////////////////////////////////////////////// 
        /// BUTTONS ////////////////////////////////////////////////////////////////////////////////////////////
        /// /////////////////////////////////////////////////////////////////////////////////////////

        /// WALKERS/// /////////////////////////////////////////////////////////////////////////////////////////
        private void OnAddOriginClicked()
        {
            gameObject.SetActive(true);// to avoid another head ache
            isAddingOrigin = true;
            visualizeAnchors = true;
            seed = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            seed.GetComponent<Renderer>().sharedMaterial.color = new Color(0, 1, 0);
            DestroyImmediate(seed.GetComponent<Collider>(), false);
            if (anchors == null)
                anchors = new GameObject[0];
            seed.name = "Walker-Anchor-" + anchors.Length;
            seed.transform.localScale = new Vector3(1f, heightAnchors, 1f);
            if (Camera.current != null)
            {
                Vector3 mousePosInScene = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).origin;
                mousePosInScene.y = Terrain.activeTerrain.SampleHeight(mousePosInScene);
                seed.transform.position = mousePosInScene;
            }
            if (SceneView.lastActiveSceneView != null)
            {
                SceneView.lastActiveSceneView.orthographic = true;
                SceneView.lastActiveSceneView.rotation = Quaternion.Euler(90, 0, 0);
            }
            else Debug.Log("Please select the terrain and open Scene panel ");
        }
        /// WALKERS/// /////////////////////////////////////////////////////////////////////////////////////////
        /// ////////////////////////////////////////////////////////////////////////////////////////////     /// 
        private void OnClearClicked()
        {
            if (anchors != null)
                for (int i = 0; i < anchors.Length; i++)
                {
                    DestroyImmediate(anchors[i], false);
                    anchors[i] = null;
                }
            anchors = new GameObject[0];

        }
        /// WORKERS/// ///////////////////////////////////////////////////////////////////////////////////////// 
        /// ////////////////////////////////////////////////////////////////////////////////////////////
        /// 
        private void OnAddWorkerClicked()
        {
            isAddingWorker = true;
            visualizeAnchors = true;
            seed = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            Material materialColored = new Material(Shader.Find("Diffuse"));
            materialColored.color = Color.blue;
            seed.GetComponent<Renderer>().material = materialColored;

            DestroyImmediate(seed.GetComponent<Collider>(), false);
            if (KeyPointsWorkers == null)
                KeyPointsWorkers = new GameObject[0];
            seed.name = "Worker-Key-" + KeyPointsWorkers.Length;
            seed.transform.localScale = new Vector3(1f, heightAnchors, 1f);
            if (Camera.current != null)
            {
                Vector3 mousePosInScene = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).origin;
                mousePosInScene.y = Terrain.activeTerrain.SampleHeight(mousePosInScene);
                seed.transform.position = mousePosInScene;
                Debug.LogError("There is no active camera in the scene!");
            }
            SceneView.lastActiveSceneView.orthographic = true;
            SceneView.lastActiveSceneView.rotation = Quaternion.Euler(90, 0, 0);
        }
        //-----------------------------------------------------------------------------------------------------
        private void OnClearAllWorkerClicked()
        {
            if (KeyPointsWorkers != null)
                for (int i = 0; i < KeyPointsWorkers.Length; i++)
                {
                    DestroyImmediate(KeyPointsWorkers[i], false);
                    KeyPointsWorkers[i] = null;
                }
            KeyPointsWorkers = new GameObject[0];
        }
#endif
        //-----------------------------------------------------------------------------------------------------

        //-----------------------------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------------------------
        //-----SHIPPING-------------------SHIPPING---------------------SHIPPING-------------SHIPPING
        //------------------SHIPPING------------------------SHIPPING-------------------------------------------
        //-----------------------------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------------------------

        //-----------------------------------------------------------------------------------------------------
        //private void OnPrepareShipping()
        //{   ShippingHandler.PrepareShipping(); //Same as the screen button
        //    PlayerPrefs.SetInt("EditorMode", 0);
        //    PlayerPrefs.SetInt("MessageClose", 1);
        //    Application.Quit();//Is ignored on Editor
        //}
        //-----------------------------------------------------------------------------------------------------

        //-----------------------------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------------------------
        //-----VALIDADE-------------------VALIDADE---------------------VALIDADE-------------VALIDADE
        //------------------VALIDADE------------------------VALIDADE-------------------------------------------
        //-----------------------------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------------------------

        //-----------------------------------------------------------------------------------------------------
        private void OnValidate()
        {
          
            copyDataFromInspectorToCitizenFactory();
        }

        ClassWorkerProducao dummy;
       public void copyDataFromInspectorToCitizenFactory()
        {
            CitizenFactory.quantity_Production = quantity;
            CitizenFactory.anchorRadius_Production = influenceRadius;
            CitizenFactory.anchorRadius_Production = global_heightAnchors;
            CitizenFactory.anchors_Production = new List<GameObject>();

            if (anchors != null)
            {
                CitizenFactory.anchors_Production.Clear();
                foreach (GameObject anch in anchors)
                    CitizenFactory.anchors_Production.Add(anch);
            }
            if (templatesCitizens != null)
            {
                CitizenFactory.listaTemplates_Adults_Production.Clear();
                foreach (GameObject adult in templatesCitizens)
                    CitizenFactory.listaTemplates_Adults_Production.Add(adult);
            }
            if (templatesChildren != null)
            {
                CitizenFactory.listaTemplates_Children_Production.Clear();
                foreach (GameObject child in templatesChildren)
                    CitizenFactory.listaTemplates_Children_Production.Add(child);
            }
            if (workersDB != null)
            {
                CitizenFactory.listaDefinicaoWorkers.Clear();
                foreach (Worker worker in workersDB.list)
                {   dummy.tipo=worker.type;
                    dummy.count = worker.count;
                    dummy.boneco = worker.actor;
                    dummy.posicaoInicial = worker.baseAnchor ;
                    dummy.animacaoInicial = worker.animationProduction ;
                    dummy.posicaoDelivery = worker.deliveryLocation;
                    dummy.raioAtracao=worker.freedomDelivery;
                    dummy.freedom = worker.freedomDelivery ;
                    dummy.animacao2=worker.animationTransaction;
                    CitizenFactory.listaDefinicaoWorkers.Add(dummy);
                }
            }
         }
#if UNITY_EDITOR
        /// ///////////////////////////////////////////////////////////////////////////////////////////////////
        /// SCENE GUI ///////////////////////////////////////////////////////////////////////////////////////////////////
        /// ///////////////////////////////////////////////////////////////////////////////////////////////////
        // This will have scene events including mouse down on scenes objects
        void SceneGUI(UnityEditor.SceneView sceneView)
        {
            placeHandles();

            hideWhenRunning();

            handleMouseMove();

            handleMouseDown();

            global_influenceRadius = influenceRadius;
            global_heightAnchors = heightAnchors;
        }
#endif
#if UNITY_EDITOR
        /// ///////////////////////////////////////////////////////////////////////////////////////////////////
        private void placeHandles()
        {
            Handles.color = Color.magenta;
            global_influenceRadius = influenceRadius;
            //       if (!isAddingOrigin && seed != null && visualizeAnchors)
            //       {
            //           Handles.Disc(Quaternion.identity, seed.transform.position, new Vector3(0, 1, 0), influenceRadius, false, 1);
            //       }

            if (anchors != null)
                for (int i = 0; i < anchors.Length; i++)
                {
                    anchors[i].transform.localScale = new Vector3(1f, heightAnchors, 1f);
                    if (visualizeAnchors)
                        Handles.Disc(Quaternion.identity, anchors[i].transform.position, new Vector3(0, 1, 0), influenceRadius, false, 1);
                }
            if (KeyPointsWorkers != null)
                for (int i = 0; i < KeyPointsWorkers.Length; i++)
                {
                    KeyPointsWorkers[i].transform.localScale = new Vector3(1f, heightAnchors, 1f);
                }

            // foreach (GameObject go in anchors)
            //   go.transform.localScale = new Vector3(1f, heightAnchors, 1f);
        }
#endif
#if UNITY_EDITOR
        /// ///////////////////////////////////////////////////////////////////////////////////////////////////
        private void hideWhenRunning()
        {
            //Enable
            if (Application.isEditor && visualizeAnchors)
            {

                if (anchors != null)
                    for (int i = 0; i < anchors.Length; i++)
                    {
                        if (anchors[i] != null)
                        {
                            Renderer renderer = anchors[i].GetComponent<Renderer>();
                            renderer.enabled = true;
                        }
                        else Debug.LogError("Size is greater than number of Anchors - Press the Add Anchor button instead. Alternatively, readjust the size number?");
                    }

                if (KeyPointsWorkers != null)
                    for (int i = 0; i < KeyPointsWorkers.Length; i++)
                    {
                        if (KeyPointsWorkers[i] != null)
                        {
                            Renderer renderer = KeyPointsWorkers[i].GetComponent<Renderer>();
                            renderer.enabled = true;
                        }
                        else Debug.LogError("Size is greater than number of Anchors - Press the Add Anchor button instead. Alternatively, readjust the size number?");
                    }
            }
            //Disable
            if (Application.isPlaying || !visualizeAnchors)
            {

                if (anchors != null)
                    for (int i = 0; i < anchors.Length; i++)
                    {
                        if (anchors[i] != null)
                        {
                            Renderer renderer = anchors[i].GetComponent<Renderer>();
                            renderer.enabled = false;
                        }
                        else Debug.LogError("Size is greater than number of Anchors- Press the Add Key button instead. Alternatively, readjust the size number?");
                    }
                if (KeyPointsWorkers != null)
                    for (int i = 0; i < KeyPointsWorkers.Length; i++)
                    {
                        if (KeyPointsWorkers[i] != null)
                        {
                            Renderer renderer = KeyPointsWorkers[i].GetComponent<Renderer>();
                            renderer.enabled = false;
                        }
                        else Debug.LogError("Size is greater than number of Key workers - Press the Add Key button instead. Alternatively, readjust the size number?");
                    }
            }
        }
#endif
#if UNITY_EDITOR
        /// ///////////////////////////////////////////////////////////////////////////////////////////////////
        private void handleMouseDown()
        {
            Event cur = Event.current;
            //ON MOUSE DOWN///////////////////////////////////////////////////////////////////////
            if (cur.type == EventType.MouseDown)
            {
                if (isAddingOrigin || isAddingWorker)
                {
                    if (seed != null && visualizeAnchors)
                    {
                        Selection.activeGameObject = seed;
                        Selection.activeObject = seed;
                        Selection.activeTransform = seed.transform;
                        EditorGUIUtility.PingObject(seed);
                    }
                    SceneView.lastActiveSceneView.FrameSelected();
                }

                //WITH ANCHOR
                if (isAddingOrigin)
                {
                    isAddingOrigin = false;
                    GameObject[] newMaternities = new GameObject[anchors.Length + 1];
                    int i = 0;
                    for (i = 0; i < anchors.Length; i++)
                    {
                        newMaternities[i] = anchors[i];
                    }
                    newMaternities[i] = seed;
                    anchors = newMaternities;
                    //SceneView.lastActiveSceneView.pivot = seed.transform.position;//Centra no objecto, mas depois perde focus
                    //SceneView.lastActiveSceneView.Repaint();

                }
                //WITH WORKER
                if (isAddingWorker)
                {
                    isAddingWorker = false;
                    GameObject[] newKeyWorkers = new GameObject[KeyPointsWorkers.Length + 1];
                    int i = 0;
                    for (i = 0; i < KeyPointsWorkers.Length; i++)
                    {
                        newKeyWorkers[i] = KeyPointsWorkers[i];
                    }
                    newKeyWorkers[i] = seed;
                    KeyPointsWorkers = newKeyWorkers;
                    //   SceneView.lastActiveSceneView.pivot = seed.transform.position;//Centra no objecto, mas depois perde focus
                    //   SceneView.lastActiveSceneView.Repaint();

                }
            }
        }
#endif
#if UNITY_EDITOR
        /// ///////////////////////////////////////////////////////////////////////////////////////////////////
        private void handleMouseMove()
        {
            Event cur = Event.current;
            //ON MOUSE MOVE ///////////////////////////////////////////////////////////////////////
            if (cur.type == EventType.MouseMove && (isAddingOrigin || isAddingWorker))
            {
                Vector3 mousePosInScene = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).origin;
                mousePosInScene.y = Terrain.activeTerrain.SampleHeight(mousePosInScene);
                seed.transform.position = mousePosInScene;
            }
        }
#endif
#if UNITY_EDITOR
        ///////////////////////////////////////////////////////////////////////////////////////////////////
        /// 
        void OnSelectionChange()
        {
            cleanArraysOfEmptyEntries();
            // use the Selection class to determine which object(s) are selected
            //Wlakers
            for (int m = 0; m < anchors.Length; m++)
            {
                if (Selection.activeGameObject == anchors[m])
                    seed = anchors[m];
            }
            //workers
            if (KeyPointsWorkers != null)
                for (int m = 0; m < KeyPointsWorkers.Length; m++)
                {
                    if (Selection.activeGameObject == KeyPointsWorkers[m])
                        seed = KeyPointsWorkers[m];
                }
        }
#endif
#if UNITY_EDITOR
        /// ///////////////////////////////////////////////////////////////////////////////////////////////////
        ///
        private void cleanArraysOfEmptyEntries()
        {
            //Walkers
            if(anchors!=null)
            if (anchors.Length > 0)
            {
                GameObject[] auxArray = new GameObject[anchors.Length];
                int n = 0;
                for (int m = 0; m < anchors.Length; m++)
                {
                    if (anchors[m] != null)
                    {
                        auxArray[n] = anchors[m];
                        n++;
                    }
                }
                if (n != anchors.Length)
                {
                    anchors = new GameObject[n];
                    for (int i = 0; i < n; i++)
                        anchors[i] = auxArray[i];
                }
            }
            //Workers
            if (KeyPointsWorkers != null)
                if (KeyPointsWorkers.Length > 0)
                {
                    GameObject[] auxArray = new GameObject[KeyPointsWorkers.Length];
                    int n = 0;
                    for (int m = 0; m < KeyPointsWorkers.Length; m++)
                    {
                        if (KeyPointsWorkers[m] != null)
                        {
                            auxArray[n] = KeyPointsWorkers[m];
                            n++;
                        }
                    }
                    if (n != KeyPointsWorkers.Length)
                    {
                        KeyPointsWorkers = new GameObject[n];
                        for (int i = 0; i < n; i++)
                            KeyPointsWorkers[i] = auxArray[i];
                    }
                }
        }
#endif
#if UNITY_EDITOR
        void OnApplicationQuit()
        {
            //foreach (GameObject go in workersList)
            foreach (GajoWorker go in CitizenFactory.listaProdutores)
            {
                GameObject g = GameObject.Find(go.name);
                Destroy(go);
                Destroy(g);
                g = null;
            }
        }
#endif
        //----------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------

    }

}
