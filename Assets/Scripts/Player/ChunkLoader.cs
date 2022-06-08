using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class ChunkLoader : MonoBehaviour
    {
        Vector3 prevLoc = Vector3.zero;

        private void Awake()
        {
            
        }

        
        void Update()
        {
            var curVel = (transform.position - prevLoc) / Time.deltaTime;
            curVel = curVel.normalized;

            Debug.Log(curVel);
            //if (curVel.x > 0 && curVel.z == 0 )
            //    Debug.Log("Forward");
            //else if (curVel.x > 0 && curVel.z > 0)
            //    Debug.Log("Forward_Right");
            //else if (curVel.x == 0 && curVel.z == 0)
            //    Debug.Log("Right");
            //else if (curVel.x < 0 && curVel.z > 0)
            //    Debug.Log("Down_Right");
            //else if (curVel.x < 0 && curVel.z == 0)
            //    Debug.Log("Down");
            //else if (curVel.x < 0 && curVel.z < 0)
            //    Debug.Log("Down_Left");
            //else if (curVel.x == 0 && curVel.z < 0)
            //    Debug.Log("Left");
            // else if (curVel.x == 0 && curVel.z < 0)
            //    Debug.Log("Left");

            prevLoc = transform.position;
        }
    }

}
 
