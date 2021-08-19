using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace SA
{

    
    public class Climb : MonoBehaviour
    {
        public Animator anime;
        public bool isClimbing;


        bool isLerping;
        bool inPosition;
        float posT;
        Vector3 startPos;
        Vector3 targetPos;
        Quaternion startRot;
        Quaternion targetRot;
        public float posOffset;
        public float rayTowardsMoveDir = 0.5f;
        public float posOffsetFromWall = 0.3f;
        public float speedMultiplier = 0.1f;
        public float climbSpeed = 2;
        public float rotateSpeed = 5;
        public float angleDis = 1;


        public float horizontal;
        public float vertical;
        public bool isMid;


        Transform helpstate;
        float delta;


        public IKSnap baseIKsnap;
        public ClimbAndHook a_hook;

        void Start()
        {
            Init();
        }


        //checking if its possible to climb on the surface
        public void CheckClimb()
        {
            Vector3 origin = transform.position;
            origin.y += 1.4f;
            Vector3 dir = transform.forward;
            RaycastHit hit;
            if (Physics.Raycast(origin, dir, out hit, 5))
            {
                helpstate.position = PosWithOff(origin, hit.point);
                ClimbInit(hit);
            }
        }


        public void Init()
        {
            helpstate = new GameObject().transform;
            helpstate.name = "climb helper";
            a_hook.Init(this, helpstate);
            CheckClimb();
        }

        //transform position to surface
        void ClimbInit(RaycastHit hit)
        {
            isClimbing = true;
            helpstate.transform.rotation = Quaternion.LookRotation(-hit.normal);
            startPos = transform.position;
            targetPos = hit.point + (hit.normal * posOffsetFromWall);
            posT = 0;
            inPosition = false;
            anime.CrossFade("climb_idle", 2);
        }

        void Update()
        {
            delta = Time.deltaTime;
            Tick(delta);
        }

        public void Tick(float delta)
        {
            if (!inPosition)
            {
                getInPos();
                return;
            }

            if (!isLerping)
            {
                horizontal = Input.GetAxis("Horizontal");
                vertical = Input.GetAxis("Vertical");
                float m = Mathf.Abs(horizontal) + Mathf.Abs(vertical);


                Vector3 h = helpstate.right * horizontal;
                Vector3 v = helpstate.up * vertical;
                Vector3 moveDir = (h + v).normalized;

                if (isMid)
                {
                    if (moveDir == Vector3.zero)
                        return;
                }
                else
                {
                    bool canMove = CanMove(moveDir);
                    if (!canMove || moveDir == Vector3.zero)
                        return;
                }

                isMid = !isMid;


                posT = 0;
                isLerping = true;
                startPos = transform.position;
                Vector3 tp = helpstate.position - transform.position;
                float d = Vector3.Distance(helpstate.position, startPos) / 2;
                tp *= posOffset;
                tp += transform.position;
                targetPos = (isMid) ? tp : helpstate.position;
                a_hook.createPos(targetPos, moveDir, isMid); 
                
            }
            else
            {
                posT += delta * climbSpeed;
                if (posT > 1)
                {
                    posT = 1;
                    isLerping = false;
                }

                Vector3 cp = Vector3.Lerp(startPos, targetPos, posT);
                transform.position = cp;
                transform.rotation = Quaternion.Slerp(transform.rotation, helpstate.rotation, delta * rotateSpeed);
            }

            //raycast for checking if climbing is possible while on said surface
        bool CanMove(Vector3 moveDir)
            {
                Vector3 origin = transform.position;
                float distance = rayTowardsMoveDir;
                Vector3 dir = moveDir;

                Debug_liner.singleton.SetLine(origin, origin + (dir * distance), 0);


                //raycast towards the direction you want to move
                RaycastHit hit;
                if (Physics.Raycast(origin, dir, out hit, distance))
                {
                    //check if raycast hits a corner
                    return false;
                }



                origin += moveDir * distance;
                dir = helpstate.forward;
                float distance2 = angleDis;

                //Raycast forward towards the wall
                Debug_liner.singleton.SetLine(origin, origin + (dir * distance2), 1);
                if (Physics.Raycast(origin, dir, out hit, distance2))
                {
                    helpstate.position = PosWithOff(origin, hit.point);
                    helpstate.rotation = Quaternion.LookRotation(-hit.normal);
                    return true;
                }

                origin = origin + (dir * distance2);
                dir = -moveDir;
                if (Physics.Raycast(origin, dir, out hit, rayTowardsMoveDir)){
                    helpstate.position = PosWithOff(origin, hit.point);
                    helpstate.rotation = Quaternion.LookRotation(-hit.normal);
                    return true;
                }


                origin += dir * distance2;
                dir = -Vector3.up;

                Debug_liner.singleton.SetLine(origin, origin + dir, 2);

                if (Physics.Raycast(origin, dir, out hit, distance2))
                {
                    float angle = Vector3.Angle(-helpstate.up, hit.normal);
                    if (angle < 40)
                    {
                        helpstate.position = PosWithOff(origin, hit.point);
                        helpstate.rotation = Quaternion.LookRotation(-hit.point);
                        return true;
                    }
                }
                

                return false;
            }
           

        }
        void getInPos()
        {
            posT += delta;

            if (posT > 1)
            {
                posT = 1;
                inPosition = true;

                a_hook.createPos(targetPos, Vector3.zero, false); 
            }

            Vector3 tp = Vector3.Lerp(startPos, targetPos, posT);
            transform.position = tp;    
        }

        Vector3 PosWithOff(Vector3 origin, Vector3 target)
        {
            Vector3 direction = origin - target;
            direction.Normalize();
            Vector3 offset = direction * posOffsetFromWall;
            return target + offset;
        }
    }

    [System.Serializable]
    public class IKSnap
    {
        public Vector3 rh, lh, lf, rf;
    }
}
