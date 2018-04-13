using UnityEngine;
using System.Collections;
using UnityEngine.Experimental;
using UnityEngine.AI;

//VERSAO 15
namespace Crowd
{
    public class GajoWorker : Gajo
    {
        public bool isReady = false;
        private enum estado { BASE, PRODUCAO, ENTREGA, TRANSACIONA }
        private estado state;
        
        int indexLayer1;
        public Crowd.Profession.ProfessionType type = Crowd.Profession.ProfessionType.One_State;
        public Transform basePositionTr;
        public float freedomBasePosition;
        public AnimationClip animationProduction;

        public Transform entregaPositionTr;       
        public float freedomEntregaPosition;
        public AnimationClip animationTransaction;

        public bool isWalkingAgent=true;
        public int classe_animacao;
    
        float timeCycleProduction = 5f;
        float timeCycleTransaction = 5f;
        Animator animator;
        //--------------
        public enum TipoResources { a, b, c, d, e, f, g, h };
        public int idleAction;

        public TipoResources typeOfResourceProduced;
      
        private DNA _dna;
        public int[] dna { get { return _dna.dna; } set { _dna.dna = value; } }
        private float[] _chemicals = new float[3];
        public float[] chemicals { get { return _chemicals; } set { _chemicals = value; } }
        private Quaternion originalRotation;
        private Vector3 originalPosition;
        Crowd.GajoCitizen _client;


        //--------------
        public void init()
        {
            initializeGajo();//super   
  
            if (isWalkingAgent)
                if (walker.Navigation.agent.isOnNavMesh)
                {
                    walker.target = basePositionTr.position;
                    walker.Navigation.startWalking();
                }
         /*Override*/ 
            animator = GetComponent<Animator>();
            animator.runtimeAnimatorController = Resources.Load("EasyPopulation/Animators/Mercadeiro") as RuntimeAnimatorController;
            if (animator.runtimeAnimatorController == null)
                Debug.LogError("Please do not delete Mercadeiro controller from Resources/Characters folder!");
            AnimatorOverrideController myCurrentOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);

            string nome_animacao = "state";
            nome_animacao += classe_animacao;           
            myCurrentOverrideController[nome_animacao] = animationProduction;

            if (type == Profession.ProfessionType.Two_States)
            {
                nome_animacao = "state9";
                //nome_animacao += classe_animacao;
                myCurrentOverrideController[nome_animacao] = animationTransaction;
            }
            animator.runtimeAnimatorController = myCurrentOverrideController;
          
            animator.Update(0.0f);
         /*communicate animation state*/
            state = estado.BASE;
            GetComponent<Animator>().SetInteger("Animation_State", 1);
            GetComponent<Animator>().SetInteger("Num_Mercadeiro", classe_animacao);

         /*DNA*/
            originalRotation = gameObject.transform.rotation;
            originalPosition = gameObject.transform.position;
            //Type of resources being produced by this guy
            gameObject.AddComponent<DNA>();
            _dna = gameObject.GetComponent<DNA>();
            for (int i = 0; i < _dna.dna.Length; i++)
                _dna.dna[i] = 0;

            isReady = true;     
        }
//********************************************************************
//********************************************************************
        void Start()
        {
            switch (typeOfResourceProduced)
            {
                case TipoResources.a: _dna.dna[10] = 0; _dna.dna[11] = 0; _dna.dna[12] = 0; break;
                case TipoResources.b: _dna.dna[10] = 0; _dna.dna[11] = 0; _dna.dna[12] = 1; break;
                case TipoResources.c: _dna.dna[10] = 0; _dna.dna[11] = 1; _dna.dna[12] = 0; break;
                case TipoResources.d: _dna.dna[10] = 0; _dna.dna[11] = 1; _dna.dna[12] = 1; break;
                case TipoResources.e: _dna.dna[10] = 1; _dna.dna[11] = 0; _dna.dna[12] = 0; break;
                case TipoResources.f: _dna.dna[10] = 1; _dna.dna[11] = 0; _dna.dna[12] = 1; break;
                case TipoResources.g: _dna.dna[10] = 1; _dna.dna[11] = 1; _dna.dna[12] = 0; break;
            }

            StartCoroutine("beginFountainOfResources");
        }

        IEnumerator beginFountainOfResources()
        {
            chemicals[0] += 100;
            chemicals[1] += 100;
            chemicals[2] += 100;
            yield return new WaitForSeconds(100f);
        }

        public void startInteraction(Crowd.GajoCitizen gajo)
        {
            _client = gajo;
            gameObject.transform.LookAt(_client.transform.position);
            StartCoroutine("interactionAnimation");
            gameObject.transform.LookAt(_client.transform.position);
        }

        IEnumerator interactionAnimation()
        {
            animator.Rebind();
            animator.SetInteger("Num_Mercadeiro", 0);
            yield return new WaitForSeconds(1f);
            endInteraction();
        }

        public void endInteraction()
        {
            animator.SetInteger("Num_Mercadeiro", idleAction);
            transform.position = originalPosition;
            transform.rotation = originalRotation;
            if (_client != null)
                if (_client.behavior != null)
                    _client.behavior.Interacao.isInteracting = false;
            _client = null;
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            if (_client != null)
                Gizmos.DrawWireSphere(transform.position, 2f);
            if (_client != null)
                if (_client.behavior != null)
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawLine(transform.position, _client.gameObject.transform.position);
                }
        }
//********************************************************************
//********************************************************************

        //-------------
        public IEnumerator cycleProduction()
        {
            string nome_animacao = "";
            nome_animacao += classe_animacao;
            GetComponent<Animator>().SetInteger("Animation_State", 1);//estado animacao 1
            GetComponent<Animator>().SetInteger("Num_Mercadeiro", classe_animacao);
          
            GetComponent<Animator>().Play(nome_animacao);
   
          
            yield return new WaitForSeconds(timeCycleProduction);
            if (type == Profession.ProfessionType.Two_States)
            {
                state = estado.ENTREGA;
                GetComponent<Animator>().SetInteger("Animation_State", 2); //vasi para walk - animacao 2

            }
            else
            {
                state = estado.BASE;
                GetComponent<Animator>().SetInteger("Animation_State", 0); //vai para idle 0
            }

            if (isWalkingAgent)
            {
                if (freedomEntregaPosition != 0)
                    walker.target = entregaPositionTr.position + (Vector3)(Random.insideUnitCircle * freedomEntregaPosition);
                else
                    walker.target = entregaPositionTr.position;
                walker.Navigation.startWalking();
            }
        }
        /*****/
        public IEnumerator cycleTransaction()
        {
            walker.Navigation.stopMoving();
            GetComponent<Animator>().Play("Transaction");
            yield return new WaitForSeconds(timeCycleTransaction);
            state = estado.BASE;

            if (isWalkingAgent)
            {
                if (freedomBasePosition != 0)
                    walker.target = basePositionTr.position + (Vector3)(Random.insideUnitCircle * freedomBasePosition);
                else
                    walker.target = basePositionTr.position;

                walker.Navigation.startWalking();
            }
        }
//-------------       
        void Update()
        {
            if (isReady)          
                {
                    switch (state)
                    {
                        case estado.BASE:
                            if (Vector3.Distance(basePositionTr.position, walker.transform.position) < 5)
                            {
                                state = estado.PRODUCAO;
                                StartCoroutine(cycleProduction());
                            }
                            break;
                        case estado.PRODUCAO:
                            break;
                        case estado.ENTREGA:
                          walker.target = entregaPositionTr.position;
                            if (Vector3.Distance(entregaPositionTr.position, walker.transform.position) < 3)
                            {
                                state = estado.TRANSACIONA;
                                StartCoroutine(cycleTransaction());
                            }
                            break;
                        case estado.TRANSACIONA:
                            break;
                    }
                }             
        }//update
    
    }//class
}//namespace

