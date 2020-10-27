using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SP_PrintText : MonoBehaviour {
	//Attach the gameObject this script is attached to, as a receiver of a button, 
	//so the press function can be called from the button once pressed

	public TextMesh target;
	void press(string message){
		if (message == "x") {//delete last character
			target.text = target.text.Substring(0, target.text.Length - 1);
		}else target.text += message;//add the message to the text of the text mesh
	}

}
