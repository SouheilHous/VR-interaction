using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MD_Plugin;
using UnityEngine.SceneManagement;

public class ExampleScenes_Manager : MonoBehaviour {



    //--------------------------                                                                                        --------------------------------------
    //--------------------------                                                                                        --------------------------------------
    //--------------------------                                                                                        --------------------------------------
    //--------------------------                                                                                        --------------------------------------
    //--------------------------                                                                                        --------------------------------------
    //--------------------------                                                                                        --------------------------------------
    //--------------------------                                                                                        --------------------------------------
	//--------------------------This script is off-topic of the plugin... you can delete it if you don't use example scenes...--------------------------------
    //--------------------------                                                                                        --------------------------------------
    //--------------------------                                                                                        --------------------------------------
    //--------------------------                                                                                        --------------------------------------
    //--------------------------                                                                                        --------------------------------------
    //--------------------------                                                                                        --------------------------------------
    //--------------------------                                                                                        --------------------------------------
    //--------------------------                                                                                        --------------------------------------
    //--------------------------                                                                                        --------------------------------------

    public enum _ManagerType {BasicShapesDeformation, ExampleGame, Destructible, DefaultScene};

    public _ManagerType ManagerType;

    [Multiline(5)]
    public string SceneDestription;
    public Color DescColor = Color.white;
    public float YOffset,XOffset = 0;

    //------Basic Shapes Deform------
    public Transform CenterToInstantiate;

    public Toggle _EnablePhysics;
    public Toggle _EnableAutoMove;
   

    //----Example Game---
    public ParticleSystem ParticleSystem;

	void Update () 
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            SceneManager.LoadScene("Scene_INTRO");
        if (Input.GetKeyDown(KeyCode.R))
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);



        if (ManagerType == _ManagerType.BasicShapesDeformation)
            _BasicShapesSceneUpdater();


        if (ManagerType == _ManagerType.ExampleGame)
            _ExampleGameSceneUpdater();


        if (ManagerType == _ManagerType.Destructible)
            _ExampleDestructible();
    }

    private void Awake()
    {
        string desc = SceneDestription;
        SceneDestription = "Scene: <b>" + SceneManager.GetActiveScene().name + "</b>\n" + desc;
    }


    //------Basic Shapes Scene------

    GameObject SelectedPoint;
    private void _BasicShapesSceneUpdater()
    {
        if(_EnablePhysics)
        if (_EnablePhysics.isOn)
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
                if (_EnableAutoMove.isOn && gm.GetComponent<Rigidbody>() )
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
                newGm.transform.position = CenterToInstantiate.transform.position+Vector3.up*3f;
            else
                newGm.transform.position = Vector3.zero+Vector3.up*3f;

            newGm.AddComponent<Rigidbody>();
          
        }
    }


    //------Example Game Scene------

    private void _ExampleGameSceneUpdater()
    {
        if (Input.GetMouseButton(0))
        {
            GetComponent<MD_MeshEditorRuntime>().ppSpecialTag = "Finish";
            GetComponent<MD_MeshEditorRuntime>().ppInput = KeyCode.Mouse0;
            GetComponent<MD_MeshEditorRuntime>().NON_AXIS_SwitchControlMode(0);
        }
        else if (Input.GetMouseButton(1))
        {
            GetComponent<MD_MeshEditorRuntime>().NON_AXIS_SwitchControlMode(1);
            GetComponent<MD_MeshEditorRuntime>().ppInput = KeyCode.Mouse1;
            GetComponent<MD_MeshEditorRuntime>().ppSpecialTag = "Respawn";
        }

        if (Input.GetMouseButton(1))
            ParticleSystem.Play();
        else
            ParticleSystem.Stop();
    }


    //------Destructible Scene------

    private void _ExampleDestructible()
    {
        Ray r = new Ray(transform.position, transform.forward);
        RaycastHit hit = new RaycastHit();

        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(r, out hit))
            {
                if (hit.collider && hit.collider.GetComponent<MDM_MeshDamage>())
                    hit.collider.GetComponent<MDM_MeshDamage>().MeshDamage_ModifyMesh(hit.point, 0.6f, 0.3f);
            }
        }
    }





    private void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = 25;
        style.richText = true;
        style.normal.textColor = Color.gray;
        GUI.Label(new Rect(50, Screen.height - 80, 200, 100), "Press <b>ESC</b> to Menu\nPress <b>R</b> to Restart",style);
        style.fontSize = 20;
        style.normal.textColor = DescColor;
        GUI.Label(new Rect(Screen.width-550 - XOffset, Screen.height - 170 - YOffset, 200, 100), SceneDestription, style);
    }
}
