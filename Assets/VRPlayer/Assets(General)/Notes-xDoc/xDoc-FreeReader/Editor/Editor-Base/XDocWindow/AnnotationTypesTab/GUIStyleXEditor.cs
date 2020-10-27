using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using xDocBase.UI;
using xDocEditorBase.Extensions;
using xDocEditorBase.UI;
using System;
using System.ComponentModel;
using System.Text.RegularExpressions;


namespace xDocEditorBase.UI
{

	public class GUIStyleXEditor : PropertyEditor
	{

		// These are the master serialized GUIStyleX property itself and its two
		// data members / field 'bgColor' and 'style'
		public SerializedProperty spBgColor;
		public SerializedProperty spStyle;

		// These serialized properties in the contained GUI style are referenced and used frequently.
		// So we provide hooks to them, for easy access
		public SerializedProperty spFgColor;
		public SerializedProperty spBgTexture;

		public SerializedProperty spFontStyle;
		public SerializedProperty spFontSize;

		public SerializedProperty spBorder;
		public SerializedProperty spPadding;

		public SerializedProperty spFixedWidth;
		public SerializedProperty spFixedHeight;

		public GUIStyleXEditor (
			SerializedProperty serializedProperty
		)
			: base (
				serializedProperty
			)
		{

			spStyle = serializedProperty.FindPropertyRelative ("style");
			spBgColor = serializedProperty.FindPropertyRelative ("bgColor");

			spFgColor = spStyle.FindPropertyRelative ("m_Normal.m_TextColor");
			spBgTexture = spStyle.FindPropertyRelative ("m_Normal.m_Background");

			spFontStyle = spStyle.FindPropertyRelative ("m_FontStyle");
			spFontSize = spStyle.FindPropertyRelative ("m_FontSize");

			spBorder = spStyle.FindPropertyRelative ("m_Border");
			spPadding = spStyle.FindPropertyRelative ("m_Padding");

			spFixedWidth = spStyle.FindPropertyRelative ("m_FixedWidth");
			spFixedHeight = spStyle.FindPropertyRelative ("m_FixedHeight");

		}

		static bool showVerboseStyle = false;

		public void DrawStyleVerbose (
			XoxGUIRect currentRect
		)
		{
//		serializedObject.Update();
			using ( new XoxEditorGUI.ChangeCheck (ApplyProps) ) {
			
				using ( var indent = new EditorStateSaver.Indent () ) {

					currentRect.SetToLineHeight (1);

					indent.Set (0);
					showVerboseStyle = EditorGUI.Foldout (currentRect.rect, showVerboseStyle, "Show Advanced Style Settings");
					currentRect.MoveDown ();

					if ( showVerboseStyle ) {
						indent.Incr ();

						currentRect.SetToPropertyHeight (spBgColor);
						EditorGUI.PropertyField (currentRect.rect, spBgColor);
						currentRect.MoveDown ();

						currentRect.SetToPropertyHeight (spStyle);
						EditorGUI.PropertyField (currentRect.rect, spStyle, true);
					}
				}

			}
		}

		public float GetHeightVerbose (
			GUIStyleXEditor stxe
		)
		{
			if ( showVerboseStyle ) {
				return XoxGUIRect.GetHeightOfLines (1) +
				XoxGUIRect.GetHeightOfMoveDownSpace () +
				XoxGUIRect.GetHeightOfProperty (stxe.spBgColor) +
				XoxGUIRect.GetHeightOfMoveDownSpace () +
				XoxGUIRect.GetHeightOfProperty (stxe.spStyle);
			} else {
				return XoxGUIRect.GetHeightOfLines (1);
			}
		}


		public void DrawStyleBrief (
			XoxGUIRect currentRect
		)
		{
			serializedObject.Update ();

			// HACK MEGA HACK!!!
//			using ( new XoxEditorGUI.ChangeCheck (ApplyProps) ) {
			
			currentRect.SetToLineHeight (1);
			EditorGUI.LabelField (currentRect.rect, "Style Settings", EditorStyles.boldLabel);
			currentRect.MoveDown ();

			EditorGUI.PropertyField (currentRect.rect, spFgColor, new GUIContent ("Foreground Color"));
			currentRect.MoveDown ();

			EditorGUI.PropertyField (currentRect.rect, spBgColor, new GUIContent ("Background Color"));
			currentRect.MoveDown ();

			EditorGUI.PropertyField (currentRect.rect, spBgTexture, new GUIContent ("Background Texture"));
			currentRect.MoveDown ();

			currentRect.SetToPropertyHeight (spBorder);
			EditorGUI.PropertyField (currentRect.rect, spBorder, new GUIContent ("Texture Border"), true);
			currentRect.MoveDown ();

			currentRect.SetToPropertyHeight (spPadding);
			EditorGUI.PropertyField (currentRect.rect, spPadding, new GUIContent ("Content Padding"), true);
			currentRect.MoveDown ();

			currentRect.SetToLineHeight (1);
			EditorGUI.PropertyField (currentRect.rect, spFontStyle, true);
			currentRect.MoveDown ();

			EditorGUI.PropertyField (currentRect.rect, spFontSize, true);
//			}
		}

		public float GetHeightBrief (
			GUIStyleXEditor stxe
		)
		{
			return XoxGUIRect.GetHeightOfLines (5) +
			XoxGUIRect.GetHeightOfProperty (stxe.spBorder) +
			XoxGUIRect.GetHeightOfMoveDownSpace () +
			XoxGUIRect.GetHeightOfProperty (stxe.spPadding) +
			XoxGUIRect.GetHeightOfMoveDownSpace ();
		}


		void ApplyProps ()
		{
			ApplyModifiedProperties ();
			EditorApplication.RepaintHierarchyWindow ();
			SceneView.RepaintAll ();
		}


		#region Synchronizing

		List<GUIStyleXEditor> GUIStyleXEditorSyncList;
		List<string> GUIStyleXNameList;
		static bool showSyncUtility;
		static bool[] stylesToSync;
		static bool[] fieldsToSync;

		static readonly string[] syncFieldArray = {
			"ForegroundColor",
			"BackgroundColor",
			"BackgoundTexture",
			"Border",
			"Padding"
		};

		public void SetSyncList (
			List<GUIStyleXEditor> GUIStyleXEditorSyncList,
			List<string> GUIStyleXNameList
		)
		{
			this.GUIStyleXEditorSyncList = GUIStyleXEditorSyncList;
			this.GUIStyleXNameList = GUIStyleXNameList;

			if ( stylesToSync == null ) {
				showSyncUtility = false;
				fieldsToSync = new bool[syncFieldArray.Length];
				stylesToSync = new bool[GUIStyleXEditorSyncList.Count];
				for ( int i = 0 ; i < stylesToSync.Length ; i++ ) {
					stylesToSync[i] = true;
				}
			}
		}

		void SyncNow ()
		{
			for ( int syncField = 0 ; syncField < fieldsToSync.Length ; syncField++ ) {
				if ( !fieldsToSync[syncField] ) {
					// this field is not selected for syncing
					continue;
				}
				var destList = new List<GUIStyleXEditor> ();
			
				for ( int i = 0 ; i < stylesToSync.Length ; i++ ) {
					// dont sync not selected 
					if ( !stylesToSync[i] )
						continue;
					// dont sync with itself
					if ( GUIStyleXEditorSyncList[i] == this ) {
						continue;
					}
					destList.Add (GUIStyleXEditorSyncList[i]);
				}
				SyncStyles (syncField, destList);
				EditorApplication.RepaintHierarchyWindow ();
				SceneView.RepaintAll ();
			}

		}

		public void DrawSyncUtility (
			XoxGUIRect currentRect
		)
		{
			using ( var indent = new EditorStateSaver.Indent () ) {

				currentRect.SetToLineHeight (1);

				indent.Set (0);
				showSyncUtility = EditorGUI.Foldout (currentRect.rect, showSyncUtility, "Synchronize Styles");

				if ( showSyncUtility ) {
					currentRect.MoveDown ();
					indent.Set (1);
					EditorGUI.LabelField (currentRect.rect, "Apply the following field from this style");
					currentRect.MoveDown ();

					indent.Set (2);
					for ( int i = 0 ; i < syncFieldArray.Length ; i++ ) {
						fieldsToSync[i] = EditorGUI.Toggle (currentRect.rect, syncFieldArray[i], fieldsToSync[i]);
						currentRect.MoveDown ();
					}

					indent.Set (1);
					EditorGUI.LabelField (currentRect.rect, "to the following styles");
					currentRect.MoveDown ();

					indent.Set (2);
					for ( int i = 0 ; i < stylesToSync.Length ; i++ ) {
						if ( GUIStyleXEditorSyncList[i] == this ) {
							continue;
						}
						stylesToSync[i] = EditorGUI.Toggle (
							currentRect.rect,
							"Apply to " + GUIStyleXNameList[i] + " style",
							stylesToSync[i]);
						currentRect.MoveDown ();
					}

					if ( GUI.Button (currentRect.rect, "Sync now") ) {
						SyncNow ();
					}
				}
			}
		}

		public float GetHeightSyncUtility (
			GUIStyleXEditor stxe
		)
		{
			if ( showSyncUtility ) {
				return XoxGUIRect.GetHeightOfLines (13);
			} else {
				return XoxGUIRect.GetHeightOfLines (1);
			}
		}


		/// <summary>
		/// Syncs the styles. Takes one style element (like the bgColor e.g.) and copies the
		/// value / setting of this style element to all the GUIStyleX's, which are provided
		/// in the list as second argument.
		/// </summary>
		/// <param name="fieldId">Field element, whose settings will be copied over</param>
		/// <param name="GUIStyleXEditorList">a list of serialized properties, which will take over the settings 
		/// from this GUIStyleXProperty 
		/// </param>
		public void SyncStyles (
			int fieldId,
			List<GUIStyleXEditor> GUIStyleXEditorList
		)
		{
			// helper variable
			SerializedProperty hsp;
			foreach ( var GUIStyleXEditor in GUIStyleXEditorList ) {
				GUIStyleXEditor.serializedObject.Update ();

				switch ( fieldId ) {
				case 0:
					GUIStyleXEditor.spFgColor.colorValue = spFgColor.colorValue;
					break;
				case 1:
					GUIStyleXEditor.spBgColor.colorValue = spBgColor.colorValue;
					break;
				case 2:
					GUIStyleXEditor.spBgTexture.objectReferenceValue = spBgTexture.objectReferenceValue;
					break;
				case 3:
					hsp = GUIStyleXEditor.spBorder;
					hsp.FindPropertyRelative ("m_Bottom").intValue = spBorder.FindPropertyRelative ("m_Bottom").intValue;
					hsp.FindPropertyRelative ("m_Top").intValue = spBorder.FindPropertyRelative ("m_Top").intValue;
					hsp.FindPropertyRelative ("m_Left").intValue = spBorder.FindPropertyRelative ("m_Left").intValue;
					hsp.FindPropertyRelative ("m_Right").intValue = spBorder.FindPropertyRelative ("m_Right").intValue;
					break;
				case 4:
					hsp = GUIStyleXEditor.spPadding;
					hsp.FindPropertyRelative ("m_Bottom").intValue = spPadding.FindPropertyRelative ("m_Bottom").intValue;
					hsp.FindPropertyRelative ("m_Top").intValue = spPadding.FindPropertyRelative ("m_Top").intValue;
					hsp.FindPropertyRelative ("m_Left").intValue = spPadding.FindPropertyRelative ("m_Left").intValue;
					hsp.FindPropertyRelative ("m_Right").intValue = spPadding.FindPropertyRelative ("m_Right").intValue;
					break;
				}
				GUIStyleXEditor.serializedObject.ApplyModifiedProperties ();
			}	
		}

		#endregion
	}
}

