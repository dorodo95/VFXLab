using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class targetSystem : MonoBehaviour
{
    private Collider m_collider;
    private List<Collider> targetCollider;
    private float initialDist = 100000f;
    private Vector3 lockOnAway = new Vector3(10000f, 10000f, 10000f);

    [HideInInspector]
    public static Collider currentTarget;
    private Collider prevTarget;
    private bool lockOnLerp;
    private float lockOnLerpTime = 0;
    private Vector3 cachePos;
    [SerializeField]
    private GameObject lockOnObject;
    private Animator lockOnAnimator;
    private bool cacheLockOn = false;
    [SerializeField]
    private AnimationCurve lerpEase;
    [HideInInspector]
    public static bool isTargeting;

    void Start()
    {
        m_collider = GetComponent<Collider>();
        targetCollider = new List<Collider>();
        lockOnAnimator = lockOnObject.GetComponent<Animator>();
        lockOnObject.transform.position = lockOnAway;
    }

    void Update()
    {
        if (Input.GetButtonUp("LockOn"))
        {
            lockOnAnimator.SetBool("LockActive", false);
            isTargeting = false;
        }
    }

    void OnTriggerEnter(Collider element)
    {
        //Debug.Log(element.name);
        targetCollider.Add(element);
    }

    void OnTriggerStay()
    {
        float dist = initialDist;
        prevTarget = currentTarget;
        foreach (Collider target in targetCollider)
        {
            float curDist = Vector3.Distance(transform.position, target.transform.position);
            if (curDist < dist)
            {
                dist = curDist;

                if (!isTargeting)
                {
                    currentTarget = target;
                }
            }
        }

        if (currentTarget != null)
        {
            if (prevTarget != null && prevTarget != currentTarget)
            {
                cacheLockOn = true;
                lockOnLerp = true;
            }

            MoveLockOnPosition();
            lockOnAnimator.SetBool("LockOn", true);

            if (Input.GetButtonDown("LockOn"))
            {
                lockOnAnimator.SetBool("LockActive", true);
                isTargeting = true;
            }



        }

        //Debug.Log("Closest Target: " + currentTarget.name);
    }

    void MoveLockOnPosition()
    {
        if (prevTarget == null && !cacheLockOn)
        {
            lockOnObject.transform.position = currentTarget.transform.position;
        }

        else if (lockOnLerp)
        {


            if (cacheLockOn)
            {
                cachePos = prevTarget.transform.position;
                cacheLockOn = false;
            }

            if (prevTarget != currentTarget)
            {
                cachePos = lockOnObject.transform.position;
                lockOnLerpTime = 0;
            }

            float t = lerpEase.Evaluate(lockOnLerpTime += Time.deltaTime);
            lockOnObject.transform.position = Vector3.Lerp(cachePos, currentTarget.transform.position, t);
            lockOnLerpTime += Time.deltaTime;
            if (lockOnLerpTime >= 1)
            {
                lockOnLerpTime = 0;
                lockOnLerp = false;
            }
        }

        else if (!lockOnLerp && prevTarget == currentTarget)
        {
            lockOnObject.transform.position = currentTarget.transform.position;
        }

    }

    void OnTriggerExit(Collider element)
    {
        if(currentTarget == element)
        {
            lockOnAnimator.SetBool("LockActive", false);
            isTargeting = false;
            currentTarget = null;
        }


        targetCollider.Remove(element);
        

        if (targetCollider.Count == 0)
        {
            currentTarget = null;
            lockOnAnimator.SetBool("LockOn", false);
            lockOnAnimator.SetBool("LockActive", false);
            isTargeting = false;
            //lockOnObject.transform.position = lockOnAway;
        }
    }
}
