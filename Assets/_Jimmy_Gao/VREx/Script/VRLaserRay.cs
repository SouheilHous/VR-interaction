using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace JimmyGao
{
	/// <summary>
	/// This class contains code that controls the mockup vive controller. 
	/// Its made to make demo sceen look better. Its not made to be used with actual vive controller.
	/// </summary>
	public class VRLaserRay : MonoBehaviour
	{
		[SerializeField]
		Transform LaserBeamTransform;
		[SerializeField]
		Transform LaserBeamDot;

		Ray myRay;


		public bool PointAt = false;
        
        public void Start()
		{

        }
		// Update is called once per frame
		protected void Update()
		{

			//get direction of the controller
			myRay = new Ray(this.transform.position, this.transform.forward);
            VRExInputModule.CustomControllerRay = myRay;


			//make laser beam hit stuff it points at.
			if(LaserBeamTransform && LaserBeamDot) {
				//change the laser's length depending on where it hits
				float length = 10000;

				RaycastHit hit;
                //print("HIT!!!");
				if (Physics.Raycast(myRay, out hit, length,1<<5)) {

                    
					length = Vector3.Distance (hit.point, this.transform.position);
					//if(hit.collider.gameObject)

					//print (hit.transform.gameObject);
					//if (hit.collider.GetComponent<Button>()!=null)
					//{
					//	print("gogogo");
					//}
					//If we hit a canvas, we only want transforms with graphics to block the pointer. (that are drawn by canvas => depth not -1)
                    
					if (hit.transform.GetComponent<GraphicRaycaster> () != null) {
                        PointAt = true;
                        LaserBeamTransform.gameObject.SetActive(true);
                        LaserBeamDot.gameObject.SetActive(true);
                        //::int SelectablesUnderPointer = hit.transform.GetComponent<GraphicRaycaster>().GetObjectsUnderPointer().FindAll(x => x.GetComponent<Graphic>() != null && x.GetComponent<Graphic>().depth != -1).Count;

                        //Debug.Log("found graphics: " + SelectablesUnderPointer);

                        length = Vector3.Distance (hit.point, this.transform.position);
					} 
                    else
                    {
                        PointAt = false;
                        LaserBeamTransform.gameObject.SetActive(false);
                        LaserBeamDot.gameObject.SetActive(false);
                    }

				} else {
					//length = 0;
					PointAt = false;
					LaserBeamTransform.gameObject.SetActive(false);
					LaserBeamDot.gameObject.SetActive (false);
				}
				LaserBeamTransform.localScale = LaserBeamTransform.localScale.ModifyZ(length);
			}


		}
	}
}
