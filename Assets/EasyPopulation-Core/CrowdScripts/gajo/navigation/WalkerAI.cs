using UnityEngine;
using System.Collections;

namespace Crowd
{
    [RequireComponent (typeof(Animator))]
    public class WalkerAI : MonoBehaviour
    {       
        private GajoCitizen _owner;
        public GajoCitizen Owner { get { return _owner; } set { _owner = value; } }
        private Navigation _navigation; 
        public Navigation Navigation { get { return _navigation; } }    
        private Gait _gait;
        public Gait Gait{ get { return _gait; } }
        private Animator _animator;
        public Animator Animacao { get { return _animator; }  set { _animator = value; } }
    //    private LookAt _look; - passou para o GajoCitizen para poder fazer a a Orientation toda num memsmo sitio
    //    public LookAt Look { get { return _look; } set { _look = value; } }
        private Vector3 _target;
        public Vector3 target { get { return _navigation.agent.destination; }
                                set { if (_navigation.agent == null || _navigation.agent.enabled == false) return;
                                     _navigation.agent.SetDestination(value); } }
        public Vector3 cemiterio;
        public Vector3 preferedPosition;
        public ArrayList goalPoints = new ArrayList();
        public int currentGoalPoint = 0;
        //--------------------------------------------------------------------------------   
        //--------------------------------------------------------------------------------   
        //--------------------------------------------------------------------------------   
        public void myawake()
        {
            _navigation = gameObject.GetComponent<Navigation>();
            if (_navigation == null)
                _navigation= gameObject.AddComponent<Navigation>();
           
            _navigation.parent = this;
            _navigation.init();

            gameObject.AddComponent<Gait>();
            _gait = GetComponent<Gait>();
            _gait.parent = this;
            _animator= gameObject.GetComponent<Animator>();
         }
//-------------------------------------------------------------------------------- 
        public void init(GajoCitizen g)
        { _owner = g;
          InvokeRepeating("checkIfIsInSamePlaceForTooLong", 1f, 4f);
        }
//--------------------------------------------------------------------------------   
//--------------------------------------------------------------------------------   
//--------------------------------------------------------------------------------   
        public void initializeGoalPoints()    {
         if (CitizenFactory.anchors_Production != null)
            {
                foreach (GameObject go in CitizenFactory.anchors_Production)
                    goalPoints.Add(go.transform.position);

                int iNumBercario = Random.Range(0, CitizenFactory.anchors_Production.Count);
                int iNumCemiterio = Random.Range(0, CitizenFactory.anchors_Production.Count);
               // while (iNumBercario == iNumCemiterio)
               //     iNumCemiterio = Random.Range(0, prefabList.anchors.Length);
                preferedPosition = ((Vector3)goalPoints[iNumBercario]);
                //    bercario = preferedPosition;
                currentGoalPoint = iNumBercario;

                GameObject c_go = CitizenFactory.anchors_Production[iNumCemiterio];
                if (c_go == null) Debug.LogError("WalkerAI - initialize Goal Points - Cemitery is null");
                cemiterio = c_go.transform.position;
            }
            else Debug.LogError("Please add a GameObject with the Population Manager script in it!");
       }
//--------------------------------------------------------------------------------   
        public void getNextGoalPoint()
        {
            if (goalPoints != null)  {            
                currentGoalPoint++;
                if (currentGoalPoint >= goalPoints.Count)
                { currentGoalPoint = 0; }
                preferedPosition = (Vector3)goalPoints[currentGoalPoint];
//Debug.DrawLine(transform.position, transform.position + Vector3.up * 25, Color.cyan);
            }
            else Debug.LogError("Nao tem goal points!!! " + _owner.getId() + _owner.iEstado);
        }
//--------------------------------------------------------------------------------   
//--------------------------------------------------------------------------------
//cleans element if has locomotion problems - stays inside (locked in a place) for too long 
//--------------------------------------------------------------------------------
        //private Vector3[] ultimasPosicoes = new Vector3[20];
       // private int indexUltimasPosicoes = 0;
        public IEnumerator checkIfIsInSamePlaceForTooLong()
        {
            //--------------------------------------
            //Checka se nao esta muito perto do destino
            //--------------------------------------          
            //----------------------
            //Verifica se esta sempre parado
            //----------------------
   //         if (_owner.iEstado == GajoCitizen.GajoState.VIVO) {
		
		
			//	if (indexUltimasPosicoes == ultimasPosicoes.Length) {
			//		foreach (Vector3 v3 in ultimasPosicoes)
			//			if (v3 != transform.position)
			//				bTodosiguais = false;
   //                 if (bTodosiguais)
   //                 {
   //                     _owner.remove();
   //                     Debug.Log("Vai remover" + _owner.getId());
   //                 }
			//		indexUltimasPosicoes = 0;
			//	}
			//}
            return null;
        }
      
//--------------------------------------------------------------------------------   
        public void die()
        {
            _navigation.die();
            Destroy(_navigation);
            _gait.die();
            Destroy(_gait);
            _animator = null;
            _owner = null;
        }
    }//eo class
}//eo namespace