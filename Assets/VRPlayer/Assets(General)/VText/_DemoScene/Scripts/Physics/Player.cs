// ----------------------------------------------------------------------
// File: 		Player
// Organisation: 	Virtence GmbH
// Department:   	Simulation Development
// Copyright:    	© 2014 Virtence GmbH. All rights reserved
// Author:       	Silvio Lange (silvio.lange@virtence.com)
// ----------------------------------------------------------------------

using UnityEngine;
using UnityEngine.EventSystems;

namespace Virtence.VText.Demo {
	/// <summary>
	/// spawn balls to kick the letters
	/// </summary>
	public class Player : MonoBehaviour 
	{	

		#region EXPOSED 
	    [Tooltip("the prefab of the ball")]
	    public GameObject BallPrefab;                       // the prefab of the ball 

	    [Tooltip("The offset of the ball from the camera center into the shooting direction when it is spawned")]
	    public float BallSpawnOffset = 3.0f;                // the offset of the ball from the camera center into the shooting direction when it is spawned

	    [Tooltip("The speed of the ball")]
	    public float BallSpeed = 1000.0f;                   // the speed of the ball

	    [Tooltip("The lifetime of the ball until it is destroyed")]
	    public float BallLifeTime = 5.0f;                   // the lifetime of the ball until it is destroyed
		#endregion // EXPOSED


		#region CONSTANTS

		#endregion // CONSTANTS


		#region FIELDS

		#endregion // FIELDS


		#region PROPERTIES

		#endregion // PROPERTIES


		#region METHODS
		
		// initialize
		void Start() 
		{
		}
		
	    void Update() {
	        if (Input.GetMouseButtonUp(0)) {
	            // mouse is over an UI element
	            if (EventSystem.current.IsPointerOverGameObject()) { 
	                return;
	            }

	            Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
	            GameObject ball = Instantiate(BallPrefab, transform.position + r.direction * BallSpawnOffset, transform.rotation) as GameObject;
	            Rigidbody rb = ball.GetComponent<Rigidbody>();
	            if (rb != null) {
	                rb.AddForce(r.direction * BallSpeed);
	            }

	            Destroy(ball, BallLifeTime);
	        }
	    }
		#endregion // METHODS

		#region EVENTHANDLERS

		#endregion // EVENTHANDLERS
	}
}