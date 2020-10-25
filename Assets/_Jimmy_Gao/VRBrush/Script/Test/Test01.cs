using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test01 : MonoBehaviour {


	public GameObject TestObj01;

	public GameObject helpTemp;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void DoAddCollider()
	{
		JimmyUtility.GenerateInteractiveBound(TestObj01,helpTemp);

	}
}
