using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SA
{


    public class CameraHandler : MonoBehaviour
    {

        public Transform target;
        public float speed = 9;


        private void FixedUpdate()
        {
            if (target == null)
                return;


            Vector3 p = Vector3.Lerp(transform.position, target.position, Time.deltaTime);
            transform.position = p;
            
        }

    }
}