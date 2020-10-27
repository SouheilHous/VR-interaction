namespace xDocEditorBase.UI
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	
	/// <summary>
	/// This class keeps track of opened and closed scopes in order to close
	/// them in case of an GUI Error: GUIClip bzw. 
	/// Exception of type 'UnityEngine.ExitGUIException'.
	/// </summary>
	public static class XoxEditorScopeAccountant
	{
		//		public enum BracketTypes
		//		{
		//			NONE,
		//			GUI_EndScrollView,
		//			GUI_EndGroup,
		//			GUILayout_EndArea,
		//			GUILayout_EndHorizontal,
		//			GUILayout_EndScrollView,
		//			GUILayout_EndVertical,
		//			EditorGUILayout_EndFadeGroup,
		//			EditorGUILayout_EndHorizontal,
		//			EditorGUILayout_EndScrollView,
		//			EditorGUILayout_EndToggleGroup,
		//			EditorGUILayout_EndVertical,
		//			EditorGUI_EndChangeCheck,
		//			EditorGUI_EndDisabledGroup,
		//			EditorGUI_EndProperty
		//		}

		static Stack<XoxEditorScope> openScopes = new Stack<XoxEditorScope> ();

		public static void Push (XoxEditorScope aScope)
		{
			openScopes.Push (aScope);
//			Debug.Log (ToMyString ());
		}

		public static void Pop ()
		{
			openScopes.Pop ();
		}

		// this var is in principle just for debugging
		public static bool emergencyOngoing = false;

		public static void ClearStack ()
		{
			emergencyOngoing = true;
			Debug.Log ("Emergency Scope-Closing: " + ToMyString ());
			foreach (var item in openScopes) {
				item.EmergencyDispose ();
			}	
			openScopes.Clear ();
			emergencyOngoing = false;
		}

		public static string ToMyString ()
		{
			string s = openScopes.Count.ToString () + ": ";
			foreach (var item in openScopes) {
				s += item.GetType ().ToString () + "//";
			}	
			return s;
		}
	
	}
}
