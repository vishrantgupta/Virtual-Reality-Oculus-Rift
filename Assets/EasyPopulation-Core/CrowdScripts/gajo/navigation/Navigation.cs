using UnityEngine;
using System.Collections;
using UnityEngine.AI;

//using UnityEditorInternal;
namespace Crowd
{
    [RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
    public class Navigation : MonoBehaviour
    {
        public enum LocomotionMode { MODE_WALKING, MODE_RUNNING, MODE_STOPPED, OUTSIDE_NAVMESH }
        public LocomotionMode locomotionMode = LocomotionMode.MODE_STOPPED;
        private WalkerAI _parent;
        public WalkerAI parent { get { return _parent; } set { _parent = value; } }
        private UnityEngine.AI.NavMeshAgent _agent;
        public UnityEngine.AI.NavMeshAgent agent { get { return _agent; } set { _agent = value; } }
       
        Vector2 smoothDeltaPosition = Vector2.zero;
        public Vector2 velocity = Vector2.zero;
        int layer;
        public float DISTANCE_PERSONAL_SPACE = 0;
        LayerMask mask;
      //  float DIST_RAY;
        NavMeshHit hitNav;
        RaycastHit hitRay;
        int walkBackAnimation;
        int restartAnimation;
        int rotateLeftAnimation;
        int rotateRightAnimation;
        int layerIndex;
        public float RAIO;
        float VELOCIDADE_MINIMA = 0;
        float VELOCIDADE_MINIMA_VEZES2 = 0;
        float RAIO_CONGESTAO;
        float RACIO_ALTURA_RAIO = 0;
        float PERTO_MAS_DIALOGO_SER_POSSIVEL = 0;
        
        Vector3[]  listaPontosCorrigidos = new Vector3[5];
        int indexListaCorrigidos = 0;

        Vector3 rayOrigin;
        float radius;
        float distance;

    
        //--------------------------------------------- 
        public void init()
        {
            _agent = gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>();
            PERTO_MAS_DIALOGO_SER_POSSIVEL = agent.radius * 1.25f;
            RAIO = _agent.radius;// * transform.localScale.x;
            layer = LayerMask.NameToLayer("Gajos");       
        }
        public void Awake() {
            init();
        
        }
//---------------------------------------------------------------------------------------------------------------------------------------    
        void Start()
        { mask = 1 << layer;

         DISTANCE_PERSONAL_SPACE = RAIO * 4f;
            _agent.baseOffset = -0.1f;
            _agent.agentTypeID = 0;
            _agent.acceleration = 8f;
            _agent.radius = 0.5f;
            _agent.height = 2;
            _agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
            _agent.avoidancePriority = 50;
            _agent.autoRepath = false;
            _agent.areaMask = NavMesh.AllAreas;
            _agent.stoppingDistance = PERTO_MAS_DIALOGO_SER_POSSIVEL;// DISTANCE_PERSONAL_SPACE;
         _agent.updatePosition = false;//Update da posicao do transform com o agente. 
                                       //Se estiver a true nao acompanha o RootMotion 
         _agent.autoBraking = true;
         _agent.autoRepath = true;
            //Overwrite por causa das escalas 
         _agent.speed = agent.height * 2 ;
         _agent.angularSpeed = 45;
         _agent.updateRotation = true;
         _agent.angularSpeed = 20;

         _agent.updateUpAxis = true;
       


         VELOCIDADE_MINIMA = 0.0005f / RAIO;  //Sera 0.0005 num maximo de 1, dai a divisao pelo raio para dar a proporção
         VELOCIDADE_MINIMA_VEZES2 = VELOCIDADE_MINIMA * 2;
         RAIO_CONGESTAO = RAIO * 7;
         RACIO_ALTURA_RAIO = _agent.radius / _agent.height;
         layerIndex = _parent.Animacao.GetLayerIndex("Base Layer");
       
         walkBackAnimation = Animator.StringToHash("Base Layer.WalkBack");
         restartAnimation = Animator.StringToHash("Base Layer.Restart");
         rotateLeftAnimation = Animator.StringToHash("Base Layer.turning-left");
         rotateRightAnimation = Animator.StringToHash("Base Layer.turning-right");

         rayOrigin = transform.position + Vector3.up * agent.radius * 2;
         radius = agent.radius / 4;
         distance = agent.radius / 2;
        }
//--------------------------------------------- 
public void startRunning() {
             _agent.isStopped = false;
             locomotionMode = LocomotionMode.MODE_RUNNING;
             _parent.Animacao.enabled = true;
             _parent.Animacao.SetFloat("value_WalkingSpeed", 1);
             _parent.Animacao.SetBool("walk", false);
             _parent.Animacao.SetBool("run", true);
        }
//--------------------------------------------- 
public void startWalking() {
            _agent.isStopped = false;
            _agent.updateRotation = true;
            locomotionMode = LocomotionMode.MODE_WALKING;
            _parent.Animacao.enabled = true;
            _parent.Animacao.speed = 1;
           _parent.Animacao.SetFloat("value_WalkingSpeed", 1);
            _parent.Animacao.SetBool("run", false);
            _parent.Animacao.SetBool("walk", true);
            // makeOneRotationStep(_parent.preferedPosition);
        }
        public void stopMoving() {
            _agent.isStopped = false;
            _agent.updateRotation = false;
            locomotionMode = LocomotionMode.MODE_STOPPED;
            _parent.Animacao.SetBool("run", false);
            _parent.Animacao.SetBool("walk", false);
        }
//---------------------------------------------------------------------------------------------------------------------------------------
//-------FIXED UPDATE---------------------------------------------------------------------------------------------------------------------
//---------------------------------------------------------------------------------------------------------------------------------------     
        bool podeAndar = false;
        void FixedUpdate()
        {
            if (locomotionMode == LocomotionMode.MODE_RUNNING ||
                locomotionMode == LocomotionMode.MODE_WALKING)
            {
                podeAndar = true;

                Vector3 forceFromNeighbours = Vector3.zero;
                RaycastHit[] rays = Physics.SphereCastAll(gameObject.transform.position, _agent.radius / 2, _agent.velocity, _agent.radius * 1.5f);
                foreach (RaycastHit r in rays)
                {
                    if (gameObject.layer == r.collider.gameObject.layer &&
                        gameObject.name != r.collider.gameObject.name)
                        if (podeAndar)
                        {
                            podeAndar = false;
                            Vector3 toOther = gameObject.transform.position - r.collider.gameObject.transform.position;
                            forceFromNeighbours += toOther;
                            toOther.Normalize();
                            float dot = Vector3.Dot(transform.forward, toOther);
                            float theta = Mathf.Acos(dot) * Mathf.Rad2Deg;
                            float multiplier = _agent.radius * 2;
                            if (theta < 45)
                            {
                                multiplier = 2 * _agent.radius;
                            }
                            forceFromNeighbours += (gameObject.transform.position - r.point);
                            forceFromNeighbours += (gameObject.transform.position - r.collider.gameObject.transform.position);
                            forceFromNeighbours += r.collider.gameObject.transform.forward * multiplier;
                        }
                }
      
                if (!podeAndar)
                {
                    Vector3 velocityFinal = forceFromNeighbours + _agent.velocity.normalized;
                    //--
                    velocityFinal = velocityFinal.normalized * _agent.radius * 2;
                    if (Random.Range(0, 100) > 10)
                        velocityFinal = Quaternion.AngleAxis(15, Vector3.up) * velocityFinal;
                    else
                        velocityFinal = Quaternion.AngleAxis(-15, Vector3.up) * velocityFinal;
                    //--
                    Vector3 velocityFinalDelted = velocityFinal * (0.9f * Time.deltaTime);

                    _agent.nextPosition = transform.position + velocityFinalDelted;
                    _agent.velocity = velocityFinalDelted;

                    //*** ***
                    //ESQUERDA OU DIREITA
                    //Quaternion rotation = Quaternion.LookRotation(velocityFinal);              
                    //transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, 0.5f * Time.deltaTime);
                    Vector3 relativePos = -1 * velocityFinal;//_parent.Owner.behavior.Interacao.interactingPartner.transform.position -
                    makeOneRotationStep(relativePos);
                }
            }
        }
        public void makeOneRotationStep(Vector3 relativePos)
        {   //CROSS - Da um novo vector perpendicular ao plano formado pelos dois vectores iniciais 
            //CROSS - se o y do cross, for UP, entao o ojecto esta onRIGHT 
            //CROSS - se o y do cross, for DOWN, entao o objecto esta onLEFT                   
            if (Vector3.Cross(transform.forward, relativePos).y > 0)
            {///Is on the right///    
                stepRight();
            }
            else
            {///Is on the left///                     
                stepLeft();
            }
        }
        private void stepLeft()
        {    if (!_parent.Animacao.GetCurrentAnimatorStateInfo(0).IsName("turning-left") ||
                             (_parent.Animacao.GetCurrentAnimatorStateInfo(0).IsName("turning-left")
                           && _parent.Animacao.GetCurrentAnimatorStateInfo(0).normalizedTime > 1))
                //            anim.Play(rotateLeftAnimation, 0, 0);
                _parent.Animacao.SetTrigger("step_left");
        }
        private void stepRight()
        {
            if (!_parent.Animacao.GetCurrentAnimatorStateInfo(0).IsName("turning-right") ||
                            (_parent.Animacao.GetCurrentAnimatorStateInfo(0).IsName("turning-right")
                          && _parent.Animacao.GetCurrentAnimatorStateInfo(0).normalizedTime > 1))
            // anim.Play(rotateRightAnimation, 0, 0);                                 
            {  _parent.Animacao.SetTrigger("step_right");
              
            }
        }
        //---------------------------------------------------------------------------------------------------------------------------------------    
        //----UPDATE------------------------------------------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------------------------------------------------------        
        void Update()
        {
            //se esta a andar mas tem que fazer uma grande rotacao, entao faz animacao         
            if ((locomotionMode == LocomotionMode.MODE_RUNNING ||
                 locomotionMode == LocomotionMode.MODE_WALKING
              ) && podeAndar)
            {
                try
                {
                    verifyPositionInNavmesh();
                    if (!inNavMesh || !_agent.hasPath)
                    {
                        stopAndRestart();
                    }
                    else
                        moveGajo();
                }
                catch (System.Exception e)
                {
                    Debug.LogException(e, this);
                    Debug.Log("Erro exception");
                }
            }
            else //Nao pode andar, mas tEm interacao marcada e pode andar activo!!!
                 //stopMoving();
            { 
               if (_parent != null)
                    if (_parent.Owner != null)
                        if (_parent.Owner.behavior != null)
                            if (_parent.Owner.behavior.Interacao != null)
                                if (!_parent.Owner.behavior.Interacao.isInteracting)
                                {
                                    if (podeAndar)
                                    {
                                        startWalking();
                                        //Debug.DrawLine(transform.position, transform.position + Vector3.up * 50, Color.magenta);
                                        if (Vector3.Angle(transform.forward, _agent.destination - transform.position) > 120)
                                        {
                                            makeOneRotationStep(_agent.destination - transform.position);
                                        }
                                    }
                                }
            }
        }

        bool inNavMesh;
        private bool verifyPositionInNavmesh()
        { //----------------------
          //Corrige quem esta fora da Navmesh
          //----------------------
            inNavMesh = NavMesh.SamplePosition(transform.position, out hitNav, 1.0f, NavMesh.AllAreas);
            if (!agent.isOnNavMesh && !agent.isOnOffMeshLink)
            {
                Debug.LogError("Please define a navmesh! / Place the population anchors on the soil");
                QuitGame();
                Application.Quit();
            }
            else
            if (!inNavMesh)
            {
                fixThoseOutsideNavmesh();
            }
            return inNavMesh;
        }

        private void fixThoseOutsideNavmesh()
        {
            if (_agent.FindClosestEdge(out hitNav))
            {
                if ( (!float.IsInfinity(hitNav.position.x) && !float.IsNegativeInfinity(hitNav.position.x)) &&
                     (!float.IsInfinity(hitNav.position.y) && !float.IsNegativeInfinity(hitNav.position.y)) &&
                     (!float.IsInfinity(hitNav.position.z) && !float.IsNegativeInfinity(hitNav.position.z)))
                {
                    transform.position = Vector3.MoveTowards(transform.position, hitNav.position, 0.02f * Time.deltaTime);
                    transform.position = hitNav.position;
                    _agent.SetDestination(parent.target);
//Debug.DrawLine(transform.position, transform.position + Vector3.up * 50, Color.grey);
                    if (indexListaCorrigidos < 5)
                        listaPontosCorrigidos[indexListaCorrigidos] = hitNav.position;
                    indexListaCorrigidos++;
                    if (indexListaCorrigidos == 5)
                    {
                        bool bTodosiguais = true;
                        foreach (Vector3 p in listaPontosCorrigidos)
                            if (p != listaPontosCorrigidos[0])
                                bTodosiguais = false;
                        if (bTodosiguais)
                        {
                            locomotionMode = LocomotionMode.OUTSIDE_NAVMESH;//Se o GajoCitizen.FixedUpdate detectar isto remove o caracter
                        }
                    }
                }
            }
            if (indexListaCorrigidos > 5) indexListaCorrigidos = 0;
        }

        private void stopAndRestart()
        {  //----------------------
           //Se nao tem path, para e faz (Restart)
           //----------------------
            if (_parent.target != Vector3.zero)
            {
                _agent.SetDestination(_parent.target);
                _agent.isStopped = false;
//Debug.DrawLine(transform.position, transform.position + Vector3.up * 50, Color.black);
                stepRight();
            }
            else
            {
                _agent.isStopped = true;
                _parent.Animacao.CrossFade(restartAnimation, 0.10f, layerIndex);
//Debug.DrawLine(transform.position, transform.position + Vector3.up * 50, Color.grey);

                if (Vector3.Angle(transform.forward, _agent.destination - transform.position) > 120)
                {
                    makeOneRotationStep(_agent.destination - transform.position);
                }
            }
        }

        private void moveGajo()
        {  //----------------------
           //Se tem path entao move
           //----------------------     
            {
                Vector3 worldDeltaPosition = _agent.nextPosition - transform.position;
                //Map 'worldDeltaPosition' to local space
                float dx = Vector3.Dot(transform.right, worldDeltaPosition);
                float dy = Vector3.Dot(transform.forward, worldDeltaPosition);
                Vector2 deltaPosition = new Vector2(dx, dy);

                // Low-pass filter the deltaMove
                float smooth = Mathf.Min(1.0f, Time.deltaTime / (RACIO_ALTURA_RAIO)); // 0.15f);              
                smoothDeltaPosition = Vector2.Lerp(smoothDeltaPosition, deltaPosition, smooth);

                // Update velocity if delta time is safe
                if (Time.deltaTime > 1e-5f)
                {
                    velocity = smoothDeltaPosition / Time.deltaTime;
                }

                bool shouldMove = velocity.magnitude > VELOCIDADE_MINIMA && agent.remainingDistance > RAIO; //0.05=VELOCIDADE_MINIMA 

                //--------------------------------------------------------------------
                //Parametros da animacao
                //--------------------------------------------------------------------
                switch (locomotionMode)
                {
                    case LocomotionMode.MODE_RUNNING:
                        _parent.Animacao.SetBool("run", shouldMove);
                        if (shouldMove && velocity.magnitude < RAIO) //agent.radius=0.9
                        {
                            _parent.Animacao.speed = velocity.magnitude;
                        }
                        break;
                    case LocomotionMode.MODE_WALKING:
                        _parent.Animacao.SetBool("walk", shouldMove);
                        if (velocity.magnitude < VELOCIDADE_MINIMA) //0.0005 0.0025f)
                        {
                            _parent.Animacao.speed = 0.05f;
                        }
                        else if (velocity.magnitude < VELOCIDADE_MINIMA_VEZES2) //0.0055f)
                        {
                            _parent.Animacao.speed = 0.10f;
                        }
                        else if (shouldMove && velocity.magnitude < RAIO) //0.9f)
                        {
                            if (locomotionMode == LocomotionMode.MODE_WALKING) //Porque se estiver a correr tem que mexer os membros rapido
                                _parent.Animacao.speed = 0.15f; //velocity.magnitude*RAIO;    
                        }
                        else
                        {
                            _parent.Animacao.speed = 1;
                        }
                        break;
                }
                _parent.Animacao.SetFloat("velx", velocity.x);
                _parent.Animacao.SetFloat("vely", velocity.y);

                //		// Pull character towards agent
                //		if (worldDeltaPosition.magnitude > agent.radius)
                //			transform.position = agent.nextPosition - 0.9f * worldDeltaPosition;
                //		// Pull agent towards character

                if ((locomotionMode == LocomotionMode.MODE_RUNNING ||
                     locomotionMode == LocomotionMode.MODE_WALKING) &&
                    smoothDeltaPosition.magnitude > RAIO)
                {
                    _agent.nextPosition = transform.position + 0.09f * worldDeltaPosition;
                }
                else
                {
                    //    _agent.nextPosition = transform.position + 0.01f * worldDeltaPosition;
                    _agent.nextPosition = transform.position + worldDeltaPosition * transform.localScale.x;
                }
                // if(velocity.magnitude<radius)
                if (smoothDeltaPosition.magnitude < radius / 2)
                {
//Debug.DrawLine(transform.position, transform.position + Vector3.up * 50, Color.cyan);
                    // makeOneRotationStep(_agent.nextPosition- transform.position);
                    stepRight();
                    podeAndar = false;
                }
            }
        }
//---------------------------------------------------------------------------------------------------------------------------------------
//---------LATEUPDATE----------------------------------------------------------------------------------------------------------------
//---------------------------------------------------------------------------------------------------------------------------------------        
        void LateUpdate()
        {
            //Atencao que o InteractionHandler tem tambem o metodo stepAwayYourBreathIsTooStinky
            //Se esta em interacao
            //que move o carater para tras
            if ((locomotionMode == LocomotionMode.MODE_RUNNING ||
                 locomotionMode == LocomotionMode.MODE_WALKING 
                ) && podeAndar)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position + Vector3.up, _agent.velocity, out hit, _agent.radius * 1.25f))
                {
                    Vector3 alternativa = Quaternion.AngleAxis(30, Vector3.up) * _agent.velocity;
                    if (Physics.Raycast(transform.position + Vector3.up, alternativa, out hit, _agent.radius * 1.25f))
                    {
                        alternativa = Quaternion.AngleAxis(-30, Vector3.up) * _agent.velocity;
                        if (Physics.Raycast(transform.position + Vector3.up, alternativa, out hit, _agent.radius * 1.25f))
                        {
                            _agent.isStopped = true;
                            _agent.velocity = Vector3.zero;
                            _parent.Animacao.speed = _parent.Animacao.speed * 0.90f;                        
                        }
                        else
                        { _agent.velocity=alternativa*Time.time; }//right is free
                    }
                    else
                    { _agent.velocity = alternativa * Time.time; } //left is free
///                    Debug.DrawLine(transform.position+Vector3.up, transform.position +Vector3.up+ _agent.velocity.normalized * 1, Color.cyan, 1f);
                }              
            }
            if (_parent.Animacao.speed > 1)
                _parent.Animacao.speed = 1;
            //-----------------------------------------------------------------------------------
            //-----------------------------------------------------------------------------------
            //-----------------------------------------------------------------------------------
            //-----------------------------------------------------------------------------------
            //-----------------------------------------------------------------------------------
            if ((_parent.Animacao.speed < 0.4) || _agent.velocity.magnitude < 0.4f)
            {
                //               _agent.updateRotation = false;

                if ((locomotionMode == LocomotionMode.MODE_RUNNING ||
                     locomotionMode == LocomotionMode.MODE_WALKING) &&
                    podeAndar)
                {
                    if (ContainsParam(_parent.Animacao, "step_right"))
                        stepRight();
                    else
                        _parent.Animacao.SetTrigger("nothingToDo");
 //                   Debug.DrawLine(transform.position, transform.position + Vector3.up * 5, Color.green);
                }
                else
                {
                    if (!podeAndar)
                    {
                        //                          _agent.updateRotation = true;
//                        Debug.DrawLine(transform.position, transform.position + Vector3.up * 5, Color.yellow);
                    }
                    if (locomotionMode == LocomotionMode.MODE_STOPPED)
                    {
                        //                                   _agent.updateRotation = true;
 //                       Debug.DrawLine(transform.position, transform.position + Vector3.up * 5, Color.white);
                    }
                }
              
            }
            //DEVE ANDAR PORQUE TEM VELOCIDADE PARA ISSO
            else
            {
                if (!podeAndar)
                {
                    //                   _agent.updateRotation = true;
 //                   Debug.DrawLine(transform.position, transform.position + Vector3.up * 5, Color.blue);
                }
                else if (locomotionMode == LocomotionMode.MODE_STOPPED)
                {
                    //                     _agent.updateRotation = true;
 //                   Debug.DrawLine(transform.position, transform.position + Vector3.up * 5, Color.black);
                }
//PODE ANDAR E LOCOMOTION E DIFERENTE DE STOPPED
                else
                {
                    if (Vector3.Angle(_parent.transform.forward, _agent.velocity) > 30)
                    { 
                            makeOneRotationStep(_agent.velocity);
                    }
                }
            }
            //-----------------------------------------------------------------------------------
            //-----------------------------------------------------------------------------------
            //-----------------------------------------------------------------------------------
            //-----------------------------------------------------------------------------------
            //-----------------------------------------------------------------------------------
        }
        //passar para o utils
        public void ResetParameters(Animator animator)
        {
            AnimatorControllerParameter[] parameters = animator.parameters;
            for (int i = 0; i < parameters.Length; i++)
            {
                AnimatorControllerParameter parameter = parameters[i];
                switch (parameter.type)
                {
                    case AnimatorControllerParameterType.Int:
                        animator.SetInteger(parameter.name, parameter.defaultInt);
                        break;
                    case AnimatorControllerParameterType.Float:
                        animator.SetFloat(parameter.name, parameter.defaultFloat);
                        break;
                    case AnimatorControllerParameterType.Bool:
                        animator.SetBool(parameter.name, parameter.defaultBool);
                        break;
                }
            }
        }
        public bool ContainsParam(Animator _Anim, string _ParamName)
        {
            foreach (AnimatorControllerParameter param in _Anim.parameters)
            {
                if (param.name == _ParamName) return true;
            }
            return false;
        }
        //---------------------------------------------------------------------------------------------------------------------------------------       
        void OnAnimatorMove()
        {
// Update position based on animation movement using navigation surface height
            if (locomotionMode != LocomotionMode.MODE_STOPPED &&
                 (_parent.Animacao.GetFloat("value_WalkingSpeed") > 0)
                )
            {
                //Vector3 position = _parent.Animacao.rootPosition;
                //position.y = agent.nextPosition.y;
                //transform.position = position;

                //hack for random guys walking on the air
                //transform.position = new Vector3(transform.position.x,
                //                      Terrain.activeTerrain.SampleHeight(transform.position)+agent.baseOffset*5.75f,
                //                      transform.position.z);

                NavMesh.SamplePosition(_parent.Animacao.rootPosition, out hitNav, 1.0f, NavMesh.AllAreas);
                if (hitNav.position.y != Mathf.Infinity)
                {
                    //hack for random guys walking on the air
                    transform.position = new Vector3(_parent.Animacao.rootPosition.x,
                                          hitNav.position.y, //agent.baseOffset//Terrain.activeTerrain.SampleHeight(transform.position),
                                          _parent.Animacao.rootPosition.z);
                }

                // Update position based on animation movement using navigation surface height
                //Vector3 position = _parent.Animacao.rootPosition;
                //position.y = agent.nextPosition.y;
                //transform.position = position;
            }
        }

//---------------------------------------------------------------------------------------------------------------------------------------    
        public void QuitGame()
        {
            // save any game data here
#if UNITY_EDITOR
            // Application.Quit() does not work in the editor so
            // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
//---------------------------------------------------------------------------------------------------------------------------------------
        public void die()
        {   _parent = null;
            _agent = null;
        }
    }//eo class
}//eo namespace






