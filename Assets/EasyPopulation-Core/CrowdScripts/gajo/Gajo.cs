

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Crowd
{
    public class Gajo : MonoBehaviour
    {
        public WalkerAI walker;
        public Animator anim;
      
        // Use this for initialization
        public void initializeGajo()
        {
            transform.position = new Vector3(transform.position.x,
                                  Terrain.activeTerrain.SampleHeight(transform.position),
                                  transform.position.z);


          
            NavMeshAgent nav = gameObject.GetComponent<NavMeshAgent>();
            if (nav == null)
            {
                nav = gameObject.AddComponent<NavMeshAgent>();
            }
            //Esta definição passou para o Navigation
            //    nav.baseOffset = -0.1f;
            //    nav.agentTypeID = 0;
            //    nav.speed = 3.5f;
            //    nav.angularSpeed = 120f;
            //    nav.acceleration = 8f;
            //    nav.stoppingDistance = 0;
            //    nav.autoBraking = true;
            //    nav.radius = 0.5f;
            //    nav.height = 2;
            //    nav.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
            //    nav.avoidancePriority = 50;
            //    nav.autoRepath = false;
            //    nav.areaMask = NavMesh.AllAreas;
            //}
            
            Rigidbody rb = gameObject.GetComponent<Rigidbody>();
            if (rb==null)
            rb = gameObject.AddComponent<Rigidbody>();
            rb.drag = 0;
            rb.mass = 1;
            rb.angularDrag = 0.05f;
            rb.useGravity = true;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.isKinematic = true;
            rb.constraints = RigidbodyConstraints.FreezeRotationY;
         
            CapsuleCollider col = gameObject.AddComponent<CapsuleCollider>();
            col.isTrigger = false;
            col.center = new Vector3(0, 1, 0);
            col.radius = 0.5f;
            col.height = 2;

            gameObject.AddComponent<WalkerAI>();
            walker = gameObject.GetComponent<WalkerAI>();
            walker.myawake();
          
            anim = gameObject.GetComponent<Animator>();
            if(anim==null)
                anim = gameObject.AddComponent<Animator>();
            //anim.runtimeAnimatorController = Resources.Load("Characters/LocomotionController") as UnityEditor.Animations.AnimatorController;
            anim.runtimeAnimatorController = Resources.Load("EasyPopulation/Animators/LocomotionController") as RuntimeAnimatorController;
            anim.applyRootMotion = true;
            anim.cullingMode = AnimatorCullingMode.CullUpdateTransforms;
            anim.updateMode = AnimatorUpdateMode.Normal;

            int layer = LayerMask.NameToLayer("Gajos");
            gameObject.layer = layer;
        }
    }//class
}//namescpae