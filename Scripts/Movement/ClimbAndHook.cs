using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{

    public class ClimbAndHook : MonoBehaviour
    {
        Animator anime;

        IKSnap ikBase;
        IKSnap current = new IKSnap();
        IKSnap next = new IKSnap();
        IKGoals goals = new IKGoals();

        public float w_rh;
        public float w_lh;
        public float w_lf;
        public float w_rf;


        Vector3 rh, lh, rf, lf;
        Transform h;
        bool isMirror;
        bool isLeft;
        Vector3 prevMove;

        float delta;
        public float lerp_speed = 1;

        public void Init(Climb c, Transform helper)
        {
            anime = c.anime;
            ikBase = c.baseIKsnap;
            h = helper;

        }



        //create pos for animator hook(right hand, left foot itd...)
        public void createPos(Vector3 origin, Vector3 moveDir, bool isMid)
        {
            delta = Time.deltaTime;
            handleAnime(moveDir, isMid);

            if (!isMid)
            {
                updateGoal(moveDir);
                prevMove = moveDir;
            }
            else
            {
                updateGoal(prevMove);
            }


            IKSnap ik = CreateSnapshot(origin);
            copySnap(ref current, ik);

            CheckIKState(isMid, goals.lf, current.lf, AvatarIKGoal.LeftFoot);
            CheckIKState(isMid, goals.rf, current.rf, AvatarIKGoal.RightFoot);
            CheckIKState(isMid, goals.lh, current.lh, AvatarIKGoal.LeftHand);
            CheckIKState(isMid, goals.rh, current.rh, AvatarIKGoal.RightHand);

            UpdateIKWgh(AvatarIKGoal.LeftFoot, 1);
            UpdateIKWgh(AvatarIKGoal.RightFoot, 1);
            UpdateIKWgh(AvatarIKGoal.LeftHand, 1);
            UpdateIKWgh(AvatarIKGoal.RightHand, 1);
        }


        //update pozicije tokom kretanja po povrsini
        void updateGoal(Vector3 moveDir)
        {
            isLeft = (moveDir.x <= 0);

            if (moveDir.x != 0)
            {
                goals.lh = isLeft;
                goals.rh = !isLeft;
                goals.lf = isLeft;
                goals.rf = !isLeft;
            }
            else
            {
                bool isEnabled = isMirror;
                if (moveDir.y < 0)
                {
                    isEnabled = !isEnabled;
                }


                goals.lh = isEnabled;
                goals.rh = !isEnabled;
                goals.lf = isEnabled;
                goals.rf = !isEnabled;
            }
        }


        void handleAnime(Vector3 moveDir,bool isMid)
        {
            if (isMid)
            {
                if (moveDir.y != 0)
                {

                    if (moveDir.x == 0)
                    {
                        isMirror = !isMirror;
                        anime.SetBool("mirror", isMirror);
                    }

                    else
                    {
                        if (moveDir.y < 0)
                        {

                        }
                        else
                        {

                        }
                    }
                    anime.CrossFade("climb_up", 0.2f);
                }
            }
            else
            {
                anime.CrossFade("climb_idle", 0.2f);
            }
        }
        
        public IKSnap CreateSnapshot(Vector3 o)
        {
            IKSnap r = new IKSnap();
            Vector3 _lh = locToWrld(ikBase.lh);
            r.lh = GetPosAct(_lh, AvatarIKGoal.LeftHand);

            Vector3 _rh = locToWrld(ikBase.rh);
            r.rh = GetPosAct(_rh, AvatarIKGoal.RightHand);

            Vector3 _lf = locToWrld(ikBase.lf);
            r.lf = GetPosAct(_lf, AvatarIKGoal.LeftFoot);

            Vector3 _rf = locToWrld(ikBase.rf);
            r.rf = GetPosAct(_rf, AvatarIKGoal.RightFoot);
            return r;
        }

        public float wallOff = 0f;



        //position of joints of the 3 model relative to the world
        Vector3 GetPosAct(Vector3 o, AvatarIKGoal goal)
        {
            Vector3 r = o;
            Vector3 origin = o;
            Vector3 direction = h.forward;
            origin += -(direction * 0.2f);
            RaycastHit hit;

            bool isHit = false;
            if (Physics.Raycast(origin, direction, out hit, 1.5f))
            {
                Vector3 _r = hit.point + (hit.normal * wallOff);
                r = _r;
                isHit = true;


                if(goal == AvatarIKGoal.LeftFoot || goal == AvatarIKGoal.RightFoot)
                {
                    if(hit.point.y > transform.position.y)
                    {
                        isHit = false;
                    }
                }
            }

            if (!isHit)
            {
                switch (goal)
                {
                    case AvatarIKGoal.LeftFoot:
                        r = locToWrld(ikBase.lf);
                        break;
                    case AvatarIKGoal.RightFoot:
                        r = locToWrld(ikBase.rf);
                        break;
                    case AvatarIKGoal.LeftHand:
                        r = locToWrld(ikBase.lh);
                        break;
                    case AvatarIKGoal.RightHand:
                        r = locToWrld(ikBase.rh);
                        break;
                }

            }
            return r;
        }
        Vector3 locToWrld(Vector3 p)
        {
            Vector3 r = h.position;
            r += h.right * p.x;
            r += h.forward * p.z;
            r += h.up * p.y;
            return r;
        }

        public void copySnap(ref IKSnap to, IKSnap from)
        {
            to.rh = from.rh;
            to.lh = from.lh;
            to.lf = from.lf;
            to.rf = from.rf;
        }

        void CheckIKState(bool isMid, bool isTrue, Vector3 pos, AvatarIKGoal goal)
        {
            if (isMid)
            {
                if (isTrue)
                {
                    Vector3 p = GetPosAct(pos, goal);
                    UpdateIKPos(goal, p);
                }
            }
            else
            {
                if (!isTrue)
                {
                    Vector3 p = GetPosAct(pos, goal);
                    UpdateIKPos(goal, p);
                }
            }
        }

        public void UpdateIKPos(AvatarIKGoal goal, Vector3 pos)
        {
            switch (goal)
            {
                case AvatarIKGoal.LeftFoot:
                    lf = pos;
                    break;
                case AvatarIKGoal.RightFoot:
                    rf = pos;
                    break;
                case AvatarIKGoal.LeftHand:
                    lh = pos;
                    break;
                case AvatarIKGoal.RightHand:
                    rh = pos;
                    break;
                default:
                    break;
            }
        }

        public void UpdateIKWgh(AvatarIKGoal goal, float w)
        {
            switch (goal)
            {
                case AvatarIKGoal.LeftFoot:
                    w_lf = w;
                    break;
                case AvatarIKGoal.RightFoot:
                    w_rf = w;
                    break;
                case AvatarIKGoal.LeftHand:
                    w_lh = w;
                    break;
                case AvatarIKGoal.RightHand:
                    w_rh = w;
                    break;
                default:
                    break;
            }
        }

        void OnAnimatorIK()
        {
            delta = Time.deltaTime;

            setIKpos(AvatarIKGoal.LeftHand, lh, w_lh);
            setIKpos(AvatarIKGoal.RightHand, rh, w_rh);
            setIKpos(AvatarIKGoal.LeftFoot, lf, w_lf);
            setIKpos(AvatarIKGoal.RightFoot, rf, w_rf);
        
        }

        void setIKpos(AvatarIKGoal goal, Vector3 tp, float w)
        {
            IKStates iKState = GetIKStates(goal);
            if (iKState == null)
            {
                iKState = new IKStates();
                iKState.goal = goal;
                iKStates.Add(iKState);
            }

            if (w == 0)
            {
                iKState.isSet = false;
            }

            if (!iKState.isSet)
            {
                iKState.position = GoalBodyBones(goal).position;
                iKState.isSet = true;
            }

            iKState.positionWeight = w;
            iKState.position = Vector3.Lerp(iKState.position, tp, delta * lerp_speed);


            anime.SetIKPositionWeight(goal, iKState.positionWeight);
            anime.SetIKPosition(goal, iKState.position);

            Transform GoalBodyBones(AvatarIKGoal goal)
            {
                switch (goal)
                {
                    case AvatarIKGoal.LeftFoot:
                        return anime.GetBoneTransform(HumanBodyBones.LeftFoot);
                    case AvatarIKGoal.RightFoot:
                        return anime.GetBoneTransform(HumanBodyBones.RightFoot);
                    case AvatarIKGoal.LeftHand:
                        return anime.GetBoneTransform(HumanBodyBones.LeftHand);
                    default:
                    case AvatarIKGoal.RightHand:
                        return anime.GetBoneTransform(HumanBodyBones.RightHand);

                }
            }

            anime.SetIKPositionWeight(goal, w);
            anime.SetIKPosition(goal, tp);
        }

        IKStates GetIKStates(AvatarIKGoal goal)
        {
            IKStates r = null;
            foreach (IKStates i in iKStates)
            {
                if(i.goal == goal)
                {
                    r = i;
                    break;
                }
            }

            return r;
        } 

        List<IKStates> iKStates = new List<IKStates>();

        class IKStates
        {
            public AvatarIKGoal goal;
            public Vector3 position;
            public float positionWeight;
            public bool isSet = false;
        }

    }
    public class IKGoals
    {
        public bool rh;
        public bool lh;
        public bool lf;
        public bool rf;
    }

}
