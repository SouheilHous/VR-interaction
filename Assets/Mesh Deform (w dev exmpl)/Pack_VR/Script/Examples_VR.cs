using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MD_Plugin;
using Valve.VR;

public class Examples_VR : MonoBehaviour {

    public Transform CenterToInstantiate;

    public Transform Controller;
    public GameObject Head;

    [Space]

    public SteamVR_Action_Boolean @Move;
    public SteamVR_Action_Boolean @Trigger;

    public bool enableMovement;
    public Vector2 touchpad;

    private void Update()
    {
        if(enableMovement)
        {
            Rigidbody g = GetComponent<Rigidbody>();
            GetComponent<CapsuleCollider>().center = new Vector3(Head.transform.localPosition.x, 0.6f, Head.transform.localPosition.z);
            if (@Move.GetState(SteamVR_Input_Sources.RightHand))
                g.transform.position += Head.transform.forward * 0.15f;
        }
        else
            _VRGame();

    }

    public void _AddObject(int index)
    {
        if (index == 0)
        {
            GameObject newGm = CubeMesh_Generator.Generate();
            newGm.AddComponent<Rigidbody>();
            if (CenterToInstantiate != null)
                newGm.transform.position = CenterToInstantiate.transform.position;
            else
                newGm.transform.position = Vector3.zero;
            newGm.AddComponent<MD_MeshProEditor>();
            newGm.AddComponent<MD_MeshColliderRefresher>().Convex_MeshCollider = true;
            newGm.GetComponent<MD_MeshColliderRefresher>().IgnoreRaycast = true;
        }
        if (index == 1)
        {
            GameObject newGm = TriangleMesh_Generator.Generate();
            newGm.AddComponent<Rigidbody>();
            if (CenterToInstantiate != null)
                newGm.transform.position = CenterToInstantiate.transform.position;
            else
                newGm.transform.position = Vector3.zero;
            newGm.AddComponent<MD_MeshProEditor>();
            newGm.AddComponent<MD_MeshColliderRefresher>().Convex_MeshCollider = true;
            newGm.GetComponent<MD_MeshColliderRefresher>().IgnoreRaycast = true;

        }
        if (index == 2)
        {
            GameObject newGm = PlaneMesh_Generator.Generate();
            newGm.AddComponent<Rigidbody>();
            if (CenterToInstantiate != null)
                newGm.transform.position = CenterToInstantiate.transform.position;
            else
                newGm.transform.position = Vector3.zero;
            newGm.AddComponent<MD_MeshProEditor>();
            newGm.AddComponent<MD_MeshColliderRefresher>().Convex_MeshCollider = true;
            newGm.GetComponent<MD_MeshColliderRefresher>().IgnoreRaycast = true;

        }
        if (index == 3)
        {
            GameObject newGm = GameObject.CreatePrimitive(PrimitiveType.Sphere);

            if (CenterToInstantiate != null)
                newGm.transform.position = CenterToInstantiate.transform.position + Vector3.up * 3f;
            else
                newGm.transform.position = Vector3.zero + Vector3.up * 3f;

            newGm.AddComponent<Rigidbody>();

        }
    }


    bool physics;
    bool automovement;
    private void _VRGame()
    {
        RaycastHit hit;

        if (Physics.Raycast(Controller.transform.position, Controller.transform.forward, out hit) && @Trigger.GetStateDown(SteamVR_Input_Sources.RightHand))
        {
            if (hit.collider.name == "Physics_")
            {
                physics = !physics;
            }
            if (hit.collider.name == "AutoMove_")
            {
                automovement = !automovement;
            }

            if (hit.collider.name == "Cube_")
            {
                _AddObject(0);
            }
            if (hit.collider.name == "Triangle_")
            {
                _AddObject(1);
            }
            if (hit.collider.name == "Plane_")
            {
                _AddObject(2);
            }
            if (hit.collider.name == "Spher_")
            {
                _AddObject(3);
            }
        }

        if (physics)
        {
            foreach (GameObject gm in GameObject.FindObjectsOfType(typeof(GameObject)))
            {
                if (!gm.GetComponent<Rigidbody>() && gm.GetComponent<MD_MeshProEditor>())
                {
                    gm.AddComponent<Rigidbody>();

                    gm.GetComponent<Rigidbody>().useGravity = true;
                    gm.GetComponent<Rigidbody>().isKinematic = false;

                    if (gm.GetComponent<MD_MeshProEditor>().ppVerticesRoot != null)
                        gm.GetComponent<MD_MeshProEditor>().MeshEditor_ClearVerticeEditor();
                }
                if (automovement && gm.GetComponent<Rigidbody>())
                {
                    gm.GetComponent<Rigidbody>().AddTorque(Vector3.right * 200);
                }
            }
        }
        else
        {
            foreach (GameObject gm in GameObject.FindObjectsOfType(typeof(GameObject)))
            {
                if (gm.GetComponent<Rigidbody>() && gm.GetComponent<MD_MeshProEditor>())
                {
                    Destroy(gm.GetComponent<Rigidbody>());

                    if (gm.GetComponent<MD_MeshProEditor>().ppVerticesRoot == null)
                        gm.GetComponent<MD_MeshProEditor>().MeshEditor_CreateVerticeEditor();
                }
            }
        }

    }
}
