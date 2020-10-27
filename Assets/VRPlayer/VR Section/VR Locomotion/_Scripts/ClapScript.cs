using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ClapScript : MonoBehaviour
{
    // for clapping
    public GameObject VfxPrefab;

    ////pick and drop objects
    //public SteamVR_Action_Boolean m_ActionGrab = null;

    //public SteamVR_Behaviour_Pose m_Pose = null;
    //private FixedJoint joint = null;
    //private Interactable m_CurrInteractable = null;

    //public List<Interactable> m_ContactInteractable = new List<Interactable>();

    private void Awake()
    {
        //m_Pose = 
        //joint = gameObject.GetComponent<FixedJoint>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    //void Update()
    //{
    //    if (m_ActionGrab.GetStateDown(m_Pose.inputSource))
    //    {
    //        print("pickig up");
    //        PickUp();
    //    }

    //    if (m_ActionGrab.GetStateUp(m_Pose.inputSource))
    //    {
    //        print("Droping..");
    //        Drop();
    //    }
    //}

    private void OnTriggerEnter(Collider other)
    {
        print("clap 1");
        if(other.gameObject.tag == "ClappingHands")
        {
            print("clap 2");
            //gameObject.GetComponent<Rigidbody>().isKinematic = true;
            VfxPrefab.SetActive(true);
            
            Invoke("Reactivate", 5f);
        }

        //if (!other.gameObject.CompareTag("Interactable"))
        //{
        //    return;
        //}
        //else
        //{
        //    m_ContactInteractable.Add(other.gameObject.GetComponent<Interactable>());
        //}
    }

    //private void OnTriggerExit(Collider other)
    //{
    //    if (!other.gameObject.CompareTag("Interactable"))
    //    {
    //        return;
    //    }
    //    else
    //    {
    //        m_ContactInteractable.Remove(other.gameObject.GetComponent<Interactable>());
    //    }
    //}

    void Reactivate()
    {
        //gameObject.GetComponent<Rigidbody>().isKinematic = false;
        VfxPrefab.SetActive(false);
    }

    //public void PickUp()
    //{
    //    //get nearest
    //    m_CurrInteractable = GetNearestInteractable();

    //    //null check
    //    if (!m_CurrInteractable)
    //        return;
    //    //already held check
    //    if(m_CurrInteractable.m_ActiveHand != null)
    //    {
    //        m_CurrInteractable.m_ActiveHand.Drop();
    //    }

    //    //position
    //    m_CurrInteractable.transform.position = transform.position;




    //    //attach
    //    Rigidbody Target = m_CurrInteractable.GetComponent<Rigidbody>();
    //    joint.connectedBody = Target;

    //    //set active hand
    //    m_CurrInteractable.m_ActiveHand = this;
    //}

    //public void Drop()
    //{
    //    //null check 
    //    if (!m_CurrInteractable)
    //        return;

    //    //Apply Velocity 
    //    Rigidbody Target = m_CurrInteractable.GetComponent<Rigidbody>();
    //    Target.velocity = m_Pose.GetVelocity();
    //    Target.angularVelocity = m_Pose.GetAngularVelocity();

    //    //Detach
    //    joint.connectedBody = null;


    //    //Clear
    //    m_CurrInteractable.m_ActiveHand = null;
    //    m_CurrInteractable = null;

    //}

    //private Interactable GetNearestInteractable()
    //{
    //    Interactable nearest = null;
    //    float minDist = float.MaxValue;
    //    float distance = 0.0f;

    //    foreach (Interactable i in m_ContactInteractable)
    //    {
    //        distance = (i.transform.position - transform.position).sqrMagnitude;

    //        if(distance < minDist)
    //        {
    //            minDist = distance;
    //            nearest = i;
    //        }
    //    }

    //    return nearest;
    //}
}
