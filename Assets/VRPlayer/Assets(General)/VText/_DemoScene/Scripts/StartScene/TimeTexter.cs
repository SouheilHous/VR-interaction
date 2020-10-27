// 
//  $Id: TimeTexter.cs 207 2015-04-21 12:56:50Z dirk $
// 
//  Virtence VFont package
//  Copyright 2014 .. 2015 by Virtence GmbH
//  http://www.virtence.com
// 
// 
//

using UnityEngine;
using System.Collections;

namespace Virtence.VText.Demo {
	/// <summary>
	/// handle the vtext objects which shows the current time.
	/// </summary>
	public class TimeTexter : MonoBehaviour {
		private VTextInterface vti;
		private string timeString = "";



	    // Use this for initialization
		void Start () {
			vti = GetComponent<VTextInterface>();
		}
		
		// Update is called once per frame
		void LateUpdate () {
	        Loom.QueueOnMainThread(() => {
	            if (null != vti)
	            {
	                string tString = System.DateTime.Now.ToLongTimeString();
	                if (tString != timeString)
	                {
	                    timeString = tString;
	                    vti.RenderText = timeString;
	                }

	            }
	        });
		}
	}
}