using UnityEngine;
using System.Collections;

public class TriggerContoller : MonoBehaviour {




	//-----------------------------------------------------
	//-----------------------------------------------------
	//-----------------------------------------------------
	//------------------Triggers----------------------
	//-----------------------------------------------------
	//-----------------------------------------------------
	//-----------------------------------------------------




	public bool Trigger_Text1;
	public bool Trigger_Text2;
	public bool Trigger_Text3;
	public bool Trigger_Text4_End;

    public bool Trigger_Repair;

	void Start () 
	{
	 
	}
	

	void Update () 
	{
	
	}

	void OnTriggerStay(Collider Any)
	{
		if(Any.tag == "MainCamera")
		{
			if(Trigger_Text1)
			{
				GameObject.Find("Titles").transform.Find("Text1").gameObject.SetActive(true);
			}
			if(Trigger_Text2)
			{
				GameObject.Find("Titles").transform.Find("Text2").gameObject.SetActive(true);
			}
			if(Trigger_Text3)
			{
				GameObject.Find("Titles").transform.Find("Text3").gameObject.SetActive(true);
			}
			if(Trigger_Text4_End)
			{
				GameObject.Find("UI").transform.Find("MainCanvas").transform.Find("YouWon").gameObject.SetActive(true);
				Any.transform.position = new Vector3(2.2F,4F,-3F);
			}
		}

        if(Any.tag == "Player")
        {
            if(Trigger_Repair)
                Any.GetComponent<MD_Plugin.MDM_MeshDamage>().MeshDamage_RepairMesh();
        }
	}
	void OnTriggerExit(Collider col)
	{
        if (Trigger_Repair)
        {
            if(col.GetComponent<MD_Plugin.MD_MeshColliderRefresher>())
            col.GetComponent<MD_Plugin.MD_MeshColliderRefresher>().MeshCollider_UpdateMeshCollider();
            return;
        }

        GameObject.Find("Titles").transform.Find("Text1").gameObject.SetActive(false);
		GameObject.Find("Titles").transform.Find("Text2").gameObject.SetActive(false);
		GameObject.Find("Titles").transform.Find("Text3").gameObject.SetActive(false);
	}
}
