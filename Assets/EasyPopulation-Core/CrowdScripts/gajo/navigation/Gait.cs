using UnityEngine;
using System.Collections;

namespace Crowd
{
    public class Gait : MonoBehaviour {

        private WalkerAI _parent;
        public WalkerAI parent { get { return _parent; } set { _parent = value; } }

        //-------------------------------------------------------------------
        //-------------------------------------------------------------------	
        //Chamado no Dispatcher
        public void adjust(float exuberance, float relaxation)
        {
            /*deprecated
             * TODO
             *              //TALVEZ FAZER ISTO QUANDO HAJA VIZINHOS 
                         _parent.Navigation.MaxSpeed = _parent.Navigation.MaxSpeed + 
                                                               (exuberance * _parent.Navigation.MaxSpeed);
                         _parent.Animacao.speed = _parent.Navigation.MaxSpeed / Global.maxSpeed;
                         if (exuberance < -0.6f)
                             _parent.Navigation.MaxSpeed = 0.8f;
                         else
                             if (_parent.Navigation.MaxSpeed < 1.15f)
                             _parent.Navigation.MaxSpeed = 1.15f;

                         if (_parent.Navigation.MaxSpeed > 1.15f)
                             _parent.Navigation.MaxSpeed = 1.15f;

                         _parent.Navigation.Radius = _parent.Navigation.Radius + (0.5f * relaxation * _parent.Navigation.Radius);

                         if (_parent.Navigation.Radius < 0.75f) _parent.Navigation.Radius = 0.75f;
                         if (_parent.Navigation.Radius > 1.25f) _parent.Navigation.Radius = 1.25f;
                         _parent.Navigation.AgentTimeHorizon = _parent.Navigation.AgentTimeHorizon + (relaxation * _parent.Navigation.AgentTimeHorizon);
                         _parent.Navigation.ObstacleTimeHorizon = _parent.Navigation.ObstacleTimeHorizon + (relaxation * _parent.Navigation.ObstacleTimeHorizon);
                         _parent.Navigation.NeighbourDist = _parent.Navigation.NeighbourDist + (0.1f * relaxation * _parent.Navigation.NeighbourDist);
                         _parent.Navigation.endReachedDistance = _parent.Navigation.Radius;
                  }
             */
        }

        public void die()
        {
            _parent = null;
        }
    }//class
}//navigation
