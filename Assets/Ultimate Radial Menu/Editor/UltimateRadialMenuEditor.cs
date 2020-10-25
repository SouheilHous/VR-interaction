/* UltimateRadialMenuEditor.cs */
/* Written by Kaz Crowe */
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor.AnimatedValues;
using System.Collections.Generic;

[CustomEditor( typeof( UltimateRadialMenu ) )]
public class UltimateRadialMenuEditor : Editor
{
	#region VARIABLES
	UltimateRadialMenu targ;

	AnimBool DefaultInspector, GenerateInspector;
	
	/* ----- > GENERATE RADIAL MENU OPTIONS < ----- */
	SerializedProperty menuButtonCount;

	/* ----- > RADIAL MENU POSITIONING < ----- */
	AnimBool RadialMenuPositioning;
	SerializedProperty scalingAxis, menuSize;
	SerializedProperty horizontalPosition, verticalPosition;
	SerializedProperty menuButtonSize, radialMenuButtonRadius;
	SerializedProperty angleOffset, followOrbitalRotation;
	SerializedProperty minRange, maxRange;
	SerializedProperty infiniteMaxRange, buttonInputAngle;
	
	/* ----- > RADIAL MENU SETTINGS < ----- */
	AnimBool RadialMenuSettings, ColorChangeOptions, ScaleTransformOptions, SpriteSwapOptions;
	SerializedProperty colorChange, disabledColor, normalColor, highlightedColor, pressedColor;
	SerializedProperty scaleTransform, highlightedScaleModifier, pressedScaleModifier, positionModifier;
	SerializedProperty spriteSwap, disabledSprite, normalSprite, highlightedSprite, pressedSprite;
	SerializedProperty radialMenuToggle, toggleInDuration, toggleOutDuration;
	// TEXT SETTINGS //
	AnimBool DisplaySelectionTextOptions, DisplayDescriptionTextOptions;
	SerializedProperty displayButtonName, nameText;
	SerializedProperty nameTextRatioX, nameTextRatioY, nameTextSize, nameTextHorizontalPosition, nameTextVerticalPosition;
	SerializedProperty displayButtonDescription, descriptionText;
	SerializedProperty descriptionTextRatioX, descriptionTextRatioY, descriptionTextSize, descriptionTextHorizontalPosition, descriptionTextVerticalPosition;
	SerializedProperty disableIcon, disableText;

	/* ----- > MENU BUTTON CUSTOMIZATION < ----- */
	AnimBool RadialButtonSettings;
	static int radialNameListIndex = 0;
	List<string> buttonNames = new List<string>();
	AnimBool RadialButtonUnityEvents = new AnimBool();
	List<AnimBool> MenuButtonAnimBools = new List<AnimBool>();
	List<AnimBool> RadialButtonNotDisabled = new List<AnimBool>();
	List<AnimBool> RadialButtonUseIcon = new List<AnimBool>();
	List<AnimBool> RadialButtonUseText = new List<AnimBool>();
	List<SerializedProperty> buttonTransform;
	List<SerializedProperty> buttonName, buttonKey, buttonId, description;
	List<SerializedProperty> useIcon, useText;
	List<SerializedProperty> icon, text;
	List<SerializedProperty> rmbIconSize, rmbIconRotation;
	List<SerializedProperty> rmbIconHorizontalPosition, rmbIconVerticalPosition;
	List<SerializedProperty> rmbUseIconUnique;
	List<SerializedProperty> buttonDisabled, unityEvent;
	// UNIFORM ICON AND TEXT SETTINGS //
	SerializedProperty iconSize, iconHorizontalPosition, iconVerticalPosition, iconRotation, iconLocalRotation;
	SerializedProperty textAreaRatioX, textAreaRatioY, textSize, textHorizontalPosition, textVerticalPosition;
	SerializedProperty textPositioningOption, displayNameOnButton;

	AnimBool IconColorChangeOptions, IconScaleTransformOptions;
	SerializedProperty iconColorChange, iconDisabledColor, iconNormalColor, iconHighlightedColor, iconPressedColor;
	SerializedProperty iconScaleTransform, iconHighlightedScaleModifier, iconPressedScaleModifier, iconPositionModifier;

	AnimBool TextColorChangeOptions, TextPositioningOptionLocal;
	SerializedProperty textColorChange, textDisabledColor, textNormalColor, textHighlightedColor, textPressedColor;
	SerializedProperty textLocalRotation;

	/* ----- > SCRIPT REFERENCE < ----- */
	AnimBool ScriptReference;
	AnimBool RadialMenuNameAssigned, RadialMenuNameDuplicate, RadialMenuNameUnassigned;
	SerializedProperty radialMenuName;
	// ----->>> EXAMPLE CODE //
	class ExampleCode
	{
		public string optionName = "";
		public string optionDescription = "";
		public string basicCode = "";
	}
	ExampleCode[] exampleCodes = new ExampleCode[]
	{
		new ExampleCode() { optionName = "UpdateRadialButton ( int index )", optionDescription = "Updates an existing radial button with new information on the targeted radial menu. This function should only be used on existing buttons.", basicCode = "UltimateRadialMenu.UpdateRadialButton( \"{0}\", {1}, YourCallbackFunction, newRadialButtonInfo );" },
		new ExampleCode() { optionName = "UpdateRadialButton ( string name )", optionDescription = "Updates an existing radial button with new information on the targeted radial menu. This function should only be used on existing buttons.", basicCode = "UltimateRadialMenu.UpdateRadialButton( \"{0}\", {1}, YourCallbackFunction, newRadialButtonInfo );" },
		new ExampleCode() { optionName = "AddRadialButton", optionDescription = "Adds a radial button to the end of the list.", basicCode = "UltimateRadialMenu.AddRadialButton( \"{0}\", YourCallbackFunction, newRadialButtonInfo );" },
		new ExampleCode() { optionName = "InsertRadialButton", optionDescription = "Creates a new radial menu button at the targeted index with the new information.", basicCode = "UltimateRadialMenu.InsertRadialButton( \"{0}\", {1}, YourCallbackFunction, newRadialButtonInfo );" },
		new ExampleCode() { optionName = "RemoveRadialButton", optionDescription = "Removes the radial button at the targeted index.", basicCode = "UltimateRadialMenu.RemoveRadialButton( \"{0}\", {1} );" },
		new ExampleCode() { optionName = "ClearRadialButtons", optionDescription = "Clears all of the radial menu buttons from the radial menu.", basicCode = "UltimateRadialMenu.ClearRadialButtons( \"{0}\" );" },
		new ExampleCode() { optionName = "EnableRadialMenu", optionDescription = "Enables the radial menu visually.", basicCode = "UltimateRadialMenu.EnableRadialMenu( \"{0}\" );" },
		new ExampleCode() { optionName = "DisableRadialMenu", optionDescription = "Disables the radial menu visually.", basicCode = "UltimateRadialMenu.DisableRadialMenu( \"{0}\" );" },
		new ExampleCode() { optionName = "DisableRadialMenuImmediate", optionDescription = "Disables the radial menu instantly, without fading/scaling.", basicCode = "UltimateRadialMenu.DisableRadialMenuImmediate( \"{0}\" );" },
		new ExampleCode() { optionName = "GetUltimateRadialMenu", optionDescription = "Returns the Ultimate Radial Menu component that is registered with the target name.", basicCode = "UltimateRadialMenu.GetUltimateRadialMenu( \"{0}\" );" },
	};
	List<string> exampleCodeOptions = new List<string>();
	int exampleCodeIndex = 0;

	// ERROR MESSAGES //
	AnimBool CanvasError;
	Canvas parentCanvas;

	/* ----- > MISC VARIABLES < ----- */
	int framesToWait = 0;
	Sprite iconPlaceholderSprite;
	Font placeholderFont;
	bool minRangeBeingChanged = false;
	bool maxRangeBeingChanged = false;
	#endregion


	void OnEnable ()
	{
		StoreReferences();

		// Register the UndoRedoCallback function to be called when an undo/redo is performed.
		Undo.undoRedoPerformed += UndoRedoCallback;
	}

	void OnDisable ()
	{
		// Remove the UndoRedoCallback from the Undo event.
		Undo.undoRedoPerformed -= UndoRedoCallback;
	}

	// Function called for Undo/Redo operations.
	void UndoRedoCallback ()
	{
		// Re-reference all variables on undo/redo.
		StoreReferences();
	}

	// Function called to display an interactive header.
	void DisplayHeaderDropdown ( string headerName, string editorPref, AnimBool targetAnim )
	{
		EditorGUILayout.BeginVertical( "Toolbar" );
		GUILayout.BeginHorizontal();
		EditorGUILayout.LabelField( headerName, EditorStyles.boldLabel );
		if( GUILayout.Button( EditorPrefs.GetBool( editorPref ) == true ? "Hide" : "Show", EditorStyles.miniButton, GUILayout.Width( 50 ), GUILayout.Height( 14f ) ) )
		{
			EditorPrefs.SetBool( editorPref, EditorPrefs.GetBool( editorPref ) == true ? false : true );
			targetAnim.target = EditorPrefs.GetBool( editorPref );
		}
		GUILayout.EndHorizontal();
		EditorGUILayout.EndVertical();
	}

	public override void OnInspectorGUI ()
	{
		if( Application.isPlaying && targ.UltimateRadialButtonList.Count == 0 )
		{
			EditorGUILayout.HelpBox( "The Radial Menu Button List has been cleared and there are no radial buttons present.", MessageType.Error );
			GenerateInspector.target = false;
			return;
		}

		serializedObject.Update();

		#region POSITIONING ERROR
		if( EditorGUILayout.BeginFadeGroup( CanvasError.faded ) )
		{
			EditorGUILayout.BeginVertical( "Box" );
			int fontSize = GUI.skin.font.fontSize;
			GUIStyle warningStyle = new GUIStyle( GUI.skin.label )
			{
				fontStyle = FontStyle.Bold,
				alignment = TextAnchor.MiddleCenter,
				wordWrap = true,
				fontSize = fontSize + 2
			};
			warningStyle.normal.textColor = Color.red;
			EditorGUILayout.LabelField( "WARNING", warningStyle );
			EditorGUILayout.LabelField( "  The Ultimate Radial Menu needs to have consistent and reliable calculations to make the radial menu function correctly.", EditorStyles.wordWrappedLabel );
			EditorGUILayout.LabelField( "Therefore, the parent Canvas needs to be set to 'Screen Space - Overlay' in order for the Ultimate Radial Menu to function properly.", EditorStyles.wordWrappedLabel );
			if( GUILayout.Button( "Fix Current Canvas", EditorStyles.miniButton ) )
			{
				parentCanvas.renderMode = RenderMode.ScreenSpaceOverlay;

				if( parentCanvas.GetComponent<CanvasScaler>() )
					parentCanvas.GetComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;

				CanvasError.target = GetCanvasError();
				EditorUtility.SetDirty( parentCanvas );
				DefaultInspector.target = CanvasError.target == false && targ.UltimateRadialButtonList.Count > 0;
				GenerateInspector.target = CanvasError.target == false && targ.UltimateRadialButtonList.Count == 0;
			}
			if( GUILayout.Button( "Create New Canvas", EditorStyles.miniButton ) )
			{
				UltimateRadialMenuCreator.RequestCanvas( Selection.activeGameObject );
				parentCanvas = GetParentCanvas();
				CanvasError.target = GetCanvasError();
				EditorUtility.SetDirty( parentCanvas );
				DefaultInspector.target = CanvasError.target == false && targ.UltimateRadialButtonList.Count > 0;
				GenerateInspector.target = CanvasError.target == false && targ.UltimateRadialButtonList.Count == 0;
			}
			EditorGUILayout.EndVertical();
		}
		EditorGUILayout.EndFadeGroup();
		#endregion

		#region RADIAL MENU GENERATOR
		if( EditorGUILayout.BeginFadeGroup( GenerateInspector.faded ) )
		{
			EditorGUILayout.BeginVertical( "Box" );

			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( menuButtonCount, new GUIContent( "Menu Button Count", "The amount of menu buttons in this radial menu." ) );
			if( EditorGUI.EndChangeCheck() )
			{
				if( menuButtonCount.intValue < 2 )
					menuButtonCount.intValue = 2;

				serializedObject.ApplyModifiedProperties();
			}

			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( normalSprite, new GUIContent( "Normal Sprite", "The sprite to be applied to each radial button." ) );
			
			EditorGUILayout.PropertyField( followOrbitalRotation, new GUIContent( "Follow Orbital Rotation", "Determines whether or not the buttons should follow the rotation of the menu." ) );
			if( EditorGUI.EndChangeCheck() )
				serializedObject.ApplyModifiedProperties();
			
			if( GUILayout.Button( "Generate" ) )
			{
				if( targ.normalSprite == null )
				{
					if( EditorUtility.DisplayDialog( "Ultimate Radial Menu", "You are about to create a radial menu with no assigned sprite.", "Continue", "Cancel" ) )
						GenerateRadialImages();
				}
				else
					GenerateRadialImages();
			}

			EditorGUILayout.EndVertical();
		}
		EditorGUILayout.EndFadeGroup();
		#endregion

		EditorGUILayout.Space();

		if( EditorGUILayout.BeginFadeGroup( DefaultInspector.faded ) )
		{
			#region RADIAL MENU POSITIONING
			DisplayHeaderDropdown( "Radial Menu Positioning", "URM_RadialMenuPositioning", RadialMenuPositioning );
			if( EditorGUILayout.BeginFadeGroup( RadialMenuPositioning.faded ) )
			{
				EditorGUILayout.Space();

				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField( scalingAxis, new GUIContent( "Scaling Axis", "Determines whether the Ultimate Radial Menu is sized according to Screen Height or Screen Width." ) );
				EditorGUILayout.Slider( menuSize, 0.0f, 10.0f, new GUIContent( "Menu Size", "Determines the overall size of the radial menu." ) );
				if( EditorGUI.EndChangeCheck() )
					serializedObject.ApplyModifiedProperties();
				
				// RADIAL MENU POSTION //
				EditorGUILayout.BeginVertical( "Box" );
				EditorGUILayout.LabelField( "Radial Menu Position", EditorStyles.boldLabel );
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.Slider( horizontalPosition, 0.0f, 100.0f, new GUIContent( "Horizontal Position", "The horizontal position of the radial menu." ) );
				EditorGUILayout.Slider( verticalPosition, 0.0f, 100.0f, new GUIContent( "Vertical Position", "The vertical position of the radial menu." ) );
				if( EditorGUI.EndChangeCheck() )
					serializedObject.ApplyModifiedProperties();
				GUILayout.Space( 1 );
				EditorGUILayout.EndVertical();

				EditorGUI.BeginChangeCheck();
				EditorGUILayout.Slider( radialMenuButtonRadius, 0.0f, 1.5f, new GUIContent( "Button Radius", "The distance that the buttons will be from the center of the menu." ) );
				EditorGUILayout.Slider( menuButtonSize, 0.0f, 1.0f, new GUIContent( "Button Size", "The size of the radial buttons." ) );
				if( EditorGUI.EndChangeCheck() )
					serializedObject.ApplyModifiedProperties();

				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField( angleOffset, new GUIContent( "Angle Offset", "Offsets the buttons position from neutral." ) );

				if( angleOffset.floatValue != targ.GetAnglePerButton / 2 )
				{
					if( GUILayout.Button( "Restore to Default", EditorStyles.miniButton ) )
						angleOffset.floatValue = targ.GetAnglePerButton / 2;
				}

				EditorGUILayout.PropertyField( followOrbitalRotation, new GUIContent( "Orbital Rotation", "Determines whether or not the buttons should follow the rotation of the menu." ) );
				if( EditorGUI.EndChangeCheck() )
					serializedObject.ApplyModifiedProperties();

				EditorGUILayout.Space();

				EditorGUILayout.LabelField( "Input Settings", EditorStyles.boldLabel );

				EditorGUI.BeginChangeCheck();
				EditorGUILayout.Slider( minRange, 0.0f, targ.maxRange, new GUIContent( "Minimum Range", "The minimum range that will affect the radial menu." ) );
				if( EditorGUI.EndChangeCheck() )
				{
					minRangeBeingChanged = true;
					maxRangeBeingChanged = false;
					framesToWait = 0;
					serializedObject.ApplyModifiedProperties();
				}
				
				EditorGUI.BeginDisabledGroup( targ.infiniteMaxRange );
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.Slider( maxRange, targ.minRange, 1.5f, new GUIContent( "Maximum Range", "The maximum range that will affect the radial menu." ) );
				if( EditorGUI.EndChangeCheck() )
				{
					maxRangeBeingChanged = true;
					minRangeBeingChanged = false;
					framesToWait = 0;
					serializedObject.ApplyModifiedProperties();
				}
				EditorGUI.EndDisabledGroup();

				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField( infiniteMaxRange, new GUIContent( "Infinite Max Range", "Determines whether or not the maximum range should be calculated as infinite." ) );
				if( EditorGUI.EndChangeCheck() )
					serializedObject.ApplyModifiedProperties();

				EditorGUI.BeginChangeCheck();
				EditorGUILayout.Slider( buttonInputAngle, 0.0f, 1.0f, new GUIContent( "Input Angle", "Determines how much of the angle should be used for input." ) );
				if( EditorGUI.EndChangeCheck() )
					serializedObject.ApplyModifiedProperties();

				if( GUILayout.Button( "Select Input Manager", EditorStyles.miniButton ) )
				{
					Selection.activeGameObject = FindObjectOfType<UltimateRadialMenuInputManager>().gameObject;
					EditorGUIUtility.PingObject( Selection.activeGameObject );
				}
			}
			if( DefaultInspector.faded == 1.0f )
				EditorGUILayout.EndFadeGroup();
			#endregion

			EditorGUILayout.Space();

			#region RADIAL MENU SETTINGS
			DisplayHeaderDropdown( "Radial Menu Settings", "URM_RadialMenuSettings", RadialMenuSettings );
			if( EditorGUILayout.BeginFadeGroup( RadialMenuSettings.faded ) )
			{
				EditorGUILayout.Space();
				
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField( normalSprite, new GUIContent( "Radial Button Sprite", "The default sprite to apply to the radial button image." ) );
				if( EditorGUI.EndChangeCheck() )
				{
					serializedObject.ApplyModifiedProperties();

					for( int i = 0; i < targ.UltimateRadialButtonList.Count; i++ )
					{
						targ.UltimateRadialButtonList[ i ].radialImage.sprite = targ.normalSprite;
						EditorUtility.SetDirty( targ.UltimateRadialButtonList[ i ].radialImage );
					}
				}

				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField( normalColor, new GUIContent( "Radial Button Color", "The default color to apply to the radial button image." ) );
				if( EditorGUI.EndChangeCheck() )
				{
					serializedObject.ApplyModifiedProperties();
					for( int i = 0; i < targ.UltimateRadialButtonList.Count; i++ )
					{
						if( targ.UltimateRadialButtonList[ i ].buttonDisabled )
							continue;

						targ.UltimateRadialButtonList[ i ].radialImage.color = normalColor.colorValue;
						EditorUtility.SetDirty( targ.UltimateRadialButtonList[ i ].radialImage );
					}
				}

				EditorGUILayout.Space();

				// ---------------------------------------------------- BUTTON INTERACTION SETTINGS ---------------------------------------------------- //
				EditorGUILayout.LabelField( "Button Interaction Settings", EditorStyles.boldLabel );

				// --------- SPRITE SWAP SETTINGS --------- //
				EditorGUILayout.BeginHorizontal();
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField( spriteSwap, new GUIContent( "Sprite Swap", "Determines whether or not the radial buttons will swap sprites when being interacted with." ) );
				if( EditorGUI.EndChangeCheck() )
				{
					serializedObject.ApplyModifiedProperties();
					ResetRadialMenuButtons();
					SpriteSwapOptions.target = spriteSwap.boolValue;
					EditorPrefs.SetBool( "URM_SpriteSwap", SpriteSwapOptions.target );
				}
				if( spriteSwap.boolValue == true )
				{
					GUILayout.FlexibleSpace();
					if( GUILayout.Button( SpriteSwapOptions.target == true ? "-" : "+", EditorStyles.miniButton, GUILayout.Width( 17 ) ) )
					{
						SpriteSwapOptions.target = !SpriteSwapOptions.target;
						EditorPrefs.SetBool( "URM_SpriteSwap", SpriteSwapOptions.target );
					}
				}
				EditorGUILayout.EndHorizontal();

				if( EditorGUILayout.BeginFadeGroup( SpriteSwapOptions.faded ) )
				{
					EditorGUI.indentLevel = 1;
					EditorGUI.BeginChangeCheck();
					EditorGUILayout.PropertyField( highlightedSprite, new GUIContent( "Highlighted Sprite", "The sprite to be applied to the radial button when highlighted." ) );
					EditorGUILayout.PropertyField( pressedSprite, new GUIContent( "Pressed Sprite", "The sprite to be applied to the radial button when pressed." ) );
					EditorGUILayout.PropertyField( disabledSprite, new GUIContent( "Disabled Sprite", "The sprite to be applied to the radial button when disabled." ) );
					if( EditorGUI.EndChangeCheck() )
						serializedObject.ApplyModifiedProperties();
					EditorGUI.indentLevel = 0;
					EditorGUILayout.Space();
				}
				if( DefaultInspector.target && RadialMenuSettings.faded == 1.0f )
					EditorGUILayout.EndFadeGroup();
				// --------- END SPRITE SWAP SETTINGS --------- //

				// --------- COLOR CHANGE SETTINGS --------- //
				EditorGUILayout.BeginHorizontal();
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField( colorChange, new GUIContent( "Color Change", "Determines whether or not the radial buttons will change color when being interacted with." ) );
				if( EditorGUI.EndChangeCheck() )
				{
					serializedObject.ApplyModifiedProperties();
					ResetRadialMenuButtons();
					ColorChangeOptions.target = colorChange.boolValue;
					EditorPrefs.SetBool( "URM_ColorChange", ColorChangeOptions.target );
				}
				if( colorChange.boolValue == true )
				{
					GUILayout.FlexibleSpace();
					if( GUILayout.Button( ColorChangeOptions.target == true ? "-" : "+", EditorStyles.miniButton, GUILayout.Width( 17 ) ) )
					{
						ColorChangeOptions.target = !ColorChangeOptions.target;
						EditorPrefs.SetBool( "URM_ColorChange", ColorChangeOptions.target );
					}
				}
				EditorGUILayout.EndHorizontal();
				
				if( EditorGUILayout.BeginFadeGroup( ColorChangeOptions.faded ) )
				{
					EditorGUI.indentLevel = 1;
					EditorGUI.BeginChangeCheck();
					EditorGUILayout.PropertyField( highlightedColor, new GUIContent( "Highlighted Color", "The color to be applied to the radial button when highlighted." ) );
					EditorGUILayout.PropertyField( pressedColor, new GUIContent( "Pressed Color", "The color to be applied to the radial button when pressed." ) );
					if( EditorGUI.EndChangeCheck() )
						serializedObject.ApplyModifiedProperties();

					EditorGUI.BeginChangeCheck();
					EditorGUILayout.PropertyField( disabledColor, new GUIContent( "Disabled Color", "The color to be applied to the radial button when disabled." ) );
					if( EditorGUI.EndChangeCheck() )
					{
						serializedObject.ApplyModifiedProperties();

						for( int i = 0; i < targ.UltimateRadialButtonList.Count; i++ )
						{
							if( targ.UltimateRadialButtonList[ i ].buttonDisabled )
							{
								targ.UltimateRadialButtonList[ i ].radialImage.color = disabledColor.colorValue;
								EditorUtility.SetDirty( targ.UltimateRadialButtonList[ i ].radialImage );
							}
						}
					}

					

					EditorGUI.indentLevel = 0;
					EditorGUILayout.Space();
				}
				if( DefaultInspector.target && RadialMenuSettings.faded == 1.0f )
					EditorGUILayout.EndFadeGroup();
				// --------- COLOR CHANGE SETTINGS --------- //

				// --------- SCALE TRANSFORM SETTINGS --------- //
				EditorGUILayout.BeginHorizontal();
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField( scaleTransform, new GUIContent( "Scale Transform", "Determines whether or not the radial buttons will scale when being interacted with." ) );
				if( EditorGUI.EndChangeCheck() )
				{
					serializedObject.ApplyModifiedProperties();
					ScaleTransformOptions.target = scaleTransform.boolValue;
					EditorPrefs.SetBool( "URM_ScaleTransform", ScaleTransformOptions.target );
				}
				if( scaleTransform.boolValue == true )
				{
					GUILayout.FlexibleSpace();
					if( GUILayout.Button( ScaleTransformOptions.target == true ? "-" : "+", EditorStyles.miniButton, GUILayout.Width( 17 ) ) )
					{
						ScaleTransformOptions.target = !ScaleTransformOptions.target;
						EditorPrefs.SetBool( "URM_ScaleTransform", ScaleTransformOptions.target );
					}
				}
				EditorGUILayout.EndHorizontal();

				if( EditorGUILayout.BeginFadeGroup( ScaleTransformOptions.faded ) )
				{
					EditorGUI.indentLevel = 1;
					EditorGUI.BeginChangeCheck();
					EditorGUILayout.Slider( highlightedScaleModifier, 0.5f, 1.5f, new GUIContent( "Highlighted Scale", "The scale modifier to be applied to the radial button transform when highlighted." ) );
					EditorGUILayout.Slider( pressedScaleModifier, 0.5f, 1.5f, new GUIContent( "Pressed Scale", "The scale modifier to be applied to the radial button transform when pressed." ) );
					EditorGUILayout.Slider( positionModifier, 0.0f, 0.2f, new GUIContent( "Position Modifier", "The position modifier for how much the radial button will expand from it's default position." ) );
					if( EditorGUI.EndChangeCheck() )
						serializedObject.ApplyModifiedProperties();
					EditorGUI.indentLevel = 0;
				}
				if( DefaultInspector.target && RadialMenuSettings.faded == 1.0f )
					EditorGUILayout.EndFadeGroup();
				// --------- END SCALE TRANSFORM SETTINGS --------- //
				// ---------------------------------------------------- END BUTTON INTERACTION SETTINGS ---------------------------------------------------- //

				EditorGUILayout.Space();

				// ---------------------------------------------------- MENU TOGGLE SETTINGS ---------------------------------------------------- //
				EditorGUILayout.LabelField( "Menu Toggle Settings", EditorStyles.boldLabel );

				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField( radialMenuToggle, new GUIContent( "Radial Menu Toggle", "Determines how the radial menu will toggle it's state, either by fading the alpha of a Canvas Group component or scaling the transform." ) );
				EditorGUILayout.PropertyField( toggleInDuration, new GUIContent( targ.radialMenuToggle == UltimateRadialMenu.RadialMenuToggle.FadeAlpha ? "Fade In Duration" : "Scale In Duration", "The time in seconds to enable the radial menu." ) );
				EditorGUILayout.PropertyField( toggleOutDuration, new GUIContent( targ.radialMenuToggle == UltimateRadialMenu.RadialMenuToggle.FadeAlpha ? "Fade Out Duration" : "Scale Out Duration", "The time in seconds to disable the radial menu." ) );
				if( EditorGUI.EndChangeCheck() )
					serializedObject.ApplyModifiedProperties();
				// ---------------------------------------------------- END MENU TOGGLE SETTINGS ---------------------------------------------------- //

				EditorGUILayout.Space();

				EditorGUILayout.LabelField( "Text Settings", EditorStyles.boldLabel );

				// ---------------------------------------------------- TEXT SETTINGS ---------------------------------------------------- //
				// --------- NAME TEXT --------- //
				EditorGUILayout.BeginHorizontal();
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField( displayButtonName, new GUIContent( "Display Name", "Determines if the radial menu should have a text component that will display the name of the currently selected button." ) );
				if( EditorGUI.EndChangeCheck() )
				{
					serializedObject.ApplyModifiedProperties();

					if( targ.nameText != null )
						targ.nameText.gameObject.SetActive( displayButtonName.boolValue );
					
					DisplaySelectionTextOptions.target = displayButtonName.boolValue;
					EditorPrefs.SetBool( "URM_DisplaySelectionTextOptions", DisplaySelectionTextOptions.target );
				}
				if( displayButtonName.boolValue == true && targ.nameText != null )
				{
					GUILayout.FlexibleSpace();
					if( GUILayout.Button( DisplaySelectionTextOptions.target == true ? "-" : "+", EditorStyles.miniButton, GUILayout.Width( 17 ) ) )
					{
						DisplaySelectionTextOptions.target = !DisplaySelectionTextOptions.target;
						EditorPrefs.SetBool( "URM_DisplaySelectionTextOptions", DisplaySelectionTextOptions.target );
					}
				}
				EditorGUILayout.EndHorizontal();

				if( EditorGUILayout.BeginFadeGroup( DisplaySelectionTextOptions.faded ) )
				{
					EditorGUI.BeginChangeCheck();
					EditorGUILayout.PropertyField( nameText, GUIContent.none );
					if( EditorGUI.EndChangeCheck() )
						serializedObject.ApplyModifiedProperties();
				
					if( targ.nameText == null )
					{
						EditorGUILayout.BeginVertical( "Box" );
						EditorGUILayout.HelpBox( "There is no text component assigned.", MessageType.Warning );
						placeholderFont = ( Font )EditorGUILayout.ObjectField( placeholderFont, typeof( Font ), false );
						if( GUILayout.Button( "Generate Text Object", EditorStyles.miniButton ) )
						{
							GameObject newText = new GameObject();
							newText.AddComponent<RectTransform>();
							newText.AddComponent<CanvasRenderer>();
							newText.AddComponent<Text>();

							newText.transform.SetParent( targ.transform );
							newText.GetComponent<RectTransform>().position = targ.GetComponent<RectTransform>().position;
							newText.GetComponent<RectTransform>().pivot = new Vector2( 0.5f, 0.5f );
							newText.name = "Name Text";
							newText.GetComponent<Text>().text = "Name Text";
							newText.GetComponent<Text>().resizeTextForBestFit = true;
							newText.GetComponent<Text>().resizeTextMinSize = 0;
							newText.GetComponent<Text>().resizeTextMaxSize = 300;
							newText.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
							nameText.objectReferenceValue = newText;
							serializedObject.ApplyModifiedProperties();

							Undo.RegisterCreatedObjectUndo( newText, "Create Text Object" );

							if( placeholderFont != null )
							{
								newText.GetComponent<Text>().font = placeholderFont;
								EditorUtility.SetDirty( newText );
							}
						}
						EditorGUILayout.EndVertical();
					}
					else if( targ.nameText != null )
					{
						EditorGUI.indentLevel = 1;
						EditorGUI.BeginChangeCheck();
						EditorGUILayout.Slider( nameTextRatioX, 0.0f, 1.0f, new GUIContent( "X Ratio", "The horizontal ratio of the text transform." ) );
						EditorGUILayout.Slider( nameTextRatioY, 0.0f, 1.0f, new GUIContent( "Y Ratio", "The vertical ratio of the text transform." ) );
						EditorGUILayout.Slider( nameTextSize, 0.0f, 1.0f, new GUIContent( "Overall Size", "The overall size of the text transform." ) );
						EditorGUILayout.Slider( nameTextHorizontalPosition, 0.0f, 100.0f, new GUIContent( "Horizontal Position", "The horizontal position of the text transform." ) );
						EditorGUILayout.Slider( nameTextVerticalPosition, 0.0f, 100.0f, new GUIContent( "Vertical Position", "The vertical position of the text transform." ) );
						if( EditorGUI.EndChangeCheck() )
							serializedObject.ApplyModifiedProperties();
						EditorGUI.indentLevel = 0;
					}
					EditorGUILayout.Space();
				}
				if( DefaultInspector.target && RadialMenuSettings.faded == 1.0f )
					EditorGUILayout.EndFadeGroup();
				// --------- END NAME TEXT --------- //

				// --------- DESCRIPTION TEXT --------- //
				EditorGUILayout.BeginHorizontal();
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField( displayButtonDescription, new GUIContent( "Display Description", "Determines if the radial menu should have a text component that will display the description of the currently selected button." ) );
				if( EditorGUI.EndChangeCheck() )
				{
					serializedObject.ApplyModifiedProperties();

					if( targ.descriptionText != null )
						targ.descriptionText.gameObject.SetActive( displayButtonDescription.boolValue );
					
					DisplayDescriptionTextOptions.target = displayButtonDescription.boolValue;	
					EditorPrefs.SetBool( "URM_DisplayDescriptionTextOptions", DisplayDescriptionTextOptions.target );
				}
				if( displayButtonDescription.boolValue == true && targ.descriptionText != null )
				{
					GUILayout.FlexibleSpace();
					if( GUILayout.Button( DisplayDescriptionTextOptions.target == true ? "-" : "+", EditorStyles.miniButton, GUILayout.Width( 17 ) ) )
					{
						DisplayDescriptionTextOptions.target = !DisplayDescriptionTextOptions.target;
						EditorPrefs.SetBool( "URM_DisplayDescriptionTextOptions", DisplayDescriptionTextOptions.target );
					}
				}
				EditorGUILayout.EndHorizontal();

				if( EditorGUILayout.BeginFadeGroup( DisplayDescriptionTextOptions.faded ) )
				{
					EditorGUI.BeginChangeCheck();
					EditorGUILayout.PropertyField( descriptionText, GUIContent.none );
					if( EditorGUI.EndChangeCheck() )
						serializedObject.ApplyModifiedProperties();

					if( targ.descriptionText == null )
					{
						EditorGUILayout.BeginVertical( "Box" );
						EditorGUILayout.HelpBox( "There is no text component assigned.", MessageType.Warning );
						placeholderFont = ( Font )EditorGUILayout.ObjectField( placeholderFont, typeof( Font ), false );
						if( GUILayout.Button( "Generate Text Object", EditorStyles.miniButton ) )
						{
							GameObject newText = new GameObject();
							newText.AddComponent<RectTransform>();
							newText.AddComponent<CanvasRenderer>();
							newText.AddComponent<Text>();

							newText.transform.SetParent( targ.transform );
							newText.GetComponent<RectTransform>().position = targ.GetComponent<RectTransform>().position;
							newText.GetComponent<RectTransform>().pivot = new Vector2( 0.5f, 0.5f );
							newText.name = "Description Text";
							newText.GetComponent<Text>().text = "Description Text";
							newText.GetComponent<Text>().resizeTextForBestFit = true;
							newText.GetComponent<Text>().resizeTextMinSize = 0;
							newText.GetComponent<Text>().resizeTextMaxSize = 300;
							newText.GetComponent<Text>().alignment = TextAnchor.UpperCenter;
							descriptionText.objectReferenceValue = newText;
							serializedObject.ApplyModifiedProperties();

							Undo.RegisterCreatedObjectUndo( newText, "Create Description Text Object" );

							if( placeholderFont != null )
							{
								newText.GetComponent<Text>().font = placeholderFont;
								EditorUtility.SetDirty( newText );
							}
						}
						EditorGUILayout.EndVertical();
					}
					else if( targ.descriptionText != null )
					{
						EditorGUI.indentLevel = 1;
						EditorGUI.BeginChangeCheck();
						EditorGUILayout.Slider( descriptionTextRatioX, 0.0f, 1.0f, new GUIContent( "X Ratio", "The horizontal ratio of the text transform." ) );
						EditorGUILayout.Slider( descriptionTextRatioY, 0.0f, 1.0f, new GUIContent( "Y Ratio", "The vertical ratio of the text transform." ) );
						EditorGUILayout.Slider( descriptionTextSize, 0.0f, 1.0f, new GUIContent( "Overall Size", "The overall size of the text transform." ) );
						EditorGUILayout.Slider( descriptionTextHorizontalPosition, 0.0f, 100.0f, new GUIContent( "Horizontal Position", "The horizontal position of the text transform." ) );
						EditorGUILayout.Slider( descriptionTextVerticalPosition, 0.0f, 100.0f, new GUIContent( "Vertical Position", "The vertical position of the text transform." ) );
						if( EditorGUI.EndChangeCheck() )
							serializedObject.ApplyModifiedProperties();
						EditorGUI.indentLevel = 0;
					}
				}
				if( DefaultInspector.target && RadialMenuSettings.faded == 1.0f )
					EditorGUILayout.EndFadeGroup();
				// --------- END DESCRIPTION TEXT --------- //
				// ---------------------------------------------------- END TEXT SETTINGS ---------------------------------------------------- //

				EditorGUILayout.Space();

				// ---------------------------------------------------- DISABLE SETTINGS ---------------------------------------------------- //
				EditorGUILayout.LabelField( "Disable Settings", EditorStyles.boldLabel );

				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField( disableIcon, new GUIContent( "Disable Icon", "Determines whether or not the icon associated with the button will be disabled when the button itself is disabled." ) );
				EditorGUILayout.PropertyField( disableText, new GUIContent( "Disable Text", "Determines whether or not the text associated with the button will be disabled when the button itself is disabled." ) );
				if( EditorGUI.EndChangeCheck() )
					serializedObject.ApplyModifiedProperties();
				// ---------------------------------------------------- END DISABLE SETTINGS ---------------------------------------------------- //
			}
			if( DefaultInspector.faded == 1.0f )
				EditorGUILayout.EndFadeGroup();
			#endregion

			EditorGUILayout.Space();

			#region RADIAL BUTTON SETTINGS
			DisplayHeaderDropdown( "Radial Button Settings", "URM_MenuButtonCustomization", RadialButtonSettings );
			if( EditorGUILayout.BeginFadeGroup( RadialButtonSettings.faded ) )
			{
				EditorGUILayout.Space();
				
				EditorGUILayout.BeginVertical( "Box" );
				GUILayout.Space( 1 );

				// ------------------------------------------------------------ RADIAL BUTTON TOOLBAR ------------------------------------------------------------ //
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.BeginHorizontal();
				if( GUILayout.Button( "<", EditorStyles.miniButton, GUILayout.Width( 25 ) ) )
				{
					radialNameListIndex = radialNameListIndex == 0 ? targ.UltimateRadialButtonList.Count - 1 : radialNameListIndex - 1;
					GUI.FocusControl( "" );
				}
				GUILayout.FlexibleSpace();
				GUIStyle headerStyle = new GUIStyle( GUI.skin.label )
				{
					fontStyle = FontStyle.Bold,
					alignment = TextAnchor.MiddleCenter,
					wordWrap = true
				};
				EditorGUILayout.LabelField( buttonNames.Count == 0 ? "" : ( buttonNames[ radialNameListIndex ] + ( radialNameListIndex == 0 ? " (Prefab)" : "" ) ), headerStyle );
				GUILayout.FlexibleSpace();
				if( GUILayout.Button( ">", EditorStyles.miniButton, GUILayout.Width( 25 ) ) )
				{
					radialNameListIndex = radialNameListIndex == targ.UltimateRadialButtonList.Count - 1 ? 0 : radialNameListIndex + 1;
					GUI.FocusControl( "" );
				}
				EditorGUILayout.EndHorizontal();
				if( EditorGUI.EndChangeCheck() )
				{
					UpdateMenuButtonAnimBools( radialNameListIndex );
					EditorGUIUtility.PingObject( targ.UltimateRadialButtonList[ radialNameListIndex ].buttonTransform );
				}
				EditorGUILayout.BeginHorizontal();
				EditorGUI.BeginDisabledGroup( Application.isPlaying );
				if( GUILayout.Button( "Insert", EditorStyles.miniButtonLeft ) )
					AddNewRadialButton( radialNameListIndex + 1 );
				EditorGUI.EndDisabledGroup();
				EditorGUI.BeginDisabledGroup( targ.UltimateRadialButtonList.Count == 1 );
				if( GUILayout.Button( "Remove", EditorStyles.miniButtonRight ) )
				{
					if( EditorUtility.DisplayDialog( "Ultimate Radial Menu", "Warning!\n\nAre you sure that you want to delete this radial button?", "Yes", "No" ) )
						RemoveRadialButton( radialNameListIndex );
				}
				EditorGUI.EndDisabledGroup();
				EditorGUILayout.EndHorizontal();
				// ------------------------------------------------------------ END RADIAL BUTTON TOOLBAR ------------------------------------------------------------ //

				EditorGUILayout.Space();

				// ------------------------------------------------------------ RADIAL BUTTON FOR LOOP ------------------------------------------------------------ //
				for( int i = 0; i < MenuButtonAnimBools.Count; i++ )
				{
					if( EditorGUILayout.BeginFadeGroup( MenuButtonAnimBools[ i ].faded ) )
					{
						EditorGUI.BeginChangeCheck();
						EditorGUILayout.PropertyField( buttonDisabled[ i ] );
						if( EditorGUI.EndChangeCheck() )
						{
							serializedObject.ApplyModifiedProperties();

							if( buttonDisabled[ i ].boolValue == true )
								targ.UltimateRadialButtonList[ i ].DisableButton();
							else
								targ.UltimateRadialButtonList[ i ].EnableButton();

							RadialButtonNotDisabled[ i ].target = !buttonDisabled[ i ].boolValue;
						}

						if( EditorGUILayout.BeginFadeGroup( RadialButtonNotDisabled[ i ].faded ) )
						{
							EditorGUI.BeginChangeCheck();
							EditorGUILayout.PropertyField( buttonName[ i ], new GUIContent( "Name", "The name to be displayed on the radial button." ) );
							if( EditorGUI.EndChangeCheck() )
							{
								serializedObject.ApplyModifiedProperties();
								if( displayNameOnButton.boolValue && targ.UltimateRadialButtonList[ i ].text != null )
								{
									
									targ.UltimateRadialButtonList[ i ].text.text = buttonName[ i ].stringValue;
									EditorUtility.SetDirty( targ.UltimateRadialButtonList[ i ].text );
								}
							}

							EditorGUI.BeginChangeCheck();
							EditorGUILayout.PropertyField( buttonKey[ i ], new GUIContent( "Key", "The string key to send when the radial button is interacted with." ) );
							EditorGUILayout.PropertyField( buttonId[ i ], new GUIContent( "ID", "The integer ID to send when the radial button is interacted with." ) );
							if( EditorGUI.EndChangeCheck() )
								serializedObject.ApplyModifiedProperties();

							if( description[ i ].stringValue == string.Empty && Event.current.type == EventType.Repaint )
							{
								GUIStyle style = new GUIStyle( GUI.skin.textField );
								style.normal.textColor = new Color( 0.5f, 0.5f, 0.5f, 0.75f );
								style.wordWrap = true;
								EditorGUILayout.TextField( GUIContent.none, "Description", style );
							}
							else
							{
								Event mEvent = Event.current;

								if( mEvent.type == EventType.KeyDown && mEvent.keyCode == KeyCode.Return )
								{
									GUI.SetNextControlName( "DescriptionField" );
									if( GUI.GetNameOfFocusedControl() == "DescriptionField" )
										GUI.FocusControl( "" );
								}

								GUIStyle style = new GUIStyle( GUI.skin.textField ) { wordWrap = true };

								EditorGUI.BeginChangeCheck();
								description[ i ].stringValue = EditorGUILayout.TextArea( description[ i ].stringValue, style );
								if( EditorGUI.EndChangeCheck() )
									serializedObject.ApplyModifiedProperties();
							}

							// ------------------------------------------- ICON SETTINGS ------------------------------------------- //

							EditorGUILayout.BeginHorizontal();
							EditorGUI.BeginChangeCheck();
							EditorGUILayout.PropertyField( useIcon[ i ], new GUIContent( "Use Icon", "Determines whether or not an icon should be used with this radial button." ) );
							if( EditorGUI.EndChangeCheck() )
							{
								serializedObject.ApplyModifiedProperties();
								ResetRadialMenuButtons();

								RadialButtonUseIcon[ i ].target = useIcon[ i ].boolValue;
								EditorPrefs.SetBool( "URM_UseIcon", RadialButtonUseIcon[ i ].target );
								UpdateRadialButtonUseIcon();

								if( useIcon[ i ].boolValue == false && targ.UltimateRadialButtonList[ i ].icon != null )
									targ.UltimateRadialButtonList[ i ].icon.gameObject.SetActive( false );
								else if( useIcon[ i ].boolValue == true && targ.UltimateRadialButtonList[ i ].icon != null )
									targ.UltimateRadialButtonList[ i ].icon.gameObject.SetActive( true );
							}
							if( useIcon[ i ].boolValue == true )
							{
								GUILayout.FlexibleSpace();
								if( GUILayout.Button( RadialButtonUseIcon[ i ].target == true ? "-" : "+", EditorStyles.miniButton, GUILayout.Width( 17 ) ) )
								{
									RadialButtonUseIcon[ i ].target = !RadialButtonUseIcon[ i ].target;
									EditorPrefs.SetBool( "URM_UseIcon", RadialButtonUseIcon[ i ].target );
									UpdateRadialButtonUseIcon();
								}
							}
							EditorGUILayout.EndHorizontal();

							if( EditorGUILayout.BeginFadeGroup( RadialButtonUseIcon[ i ].faded ) )
							{
								EditorGUI.indentLevel = 1;

								EditorGUI.BeginChangeCheck();
								EditorGUILayout.PropertyField( icon[ i ] );
								if( targ.UltimateRadialButtonList[ i ].icon != null )
								{
									targ.UltimateRadialButtonList[ i ].icon.sprite = ( Sprite )EditorGUILayout.ObjectField( targ.UltimateRadialButtonList[ i ].icon.sprite, typeof( Sprite ), true );
									
									EditorGUI.BeginChangeCheck();
									EditorGUILayout.PropertyField( iconNormalColor, GUIContent.none );
									if( EditorGUI.EndChangeCheck() )
									{
										serializedObject.ApplyModifiedProperties();

										for( int n = 0; n < targ.UltimateRadialButtonList.Count; n++ )
										{
											if( targ.UltimateRadialButtonList[ n ].buttonDisabled )
												continue;

											targ.UltimateRadialButtonList[ n ].icon.color = iconNormalColor.colorValue;
											EditorUtility.SetDirty( targ.UltimateRadialButtonList[ n ].icon );
										}
									}
								}
								if( EditorGUI.EndChangeCheck() )
								{
									serializedObject.ApplyModifiedProperties();

									serializedObject.FindProperty( string.Format( "UltimateRadialButtonList.Array.data[{0}].iconTransform", i ) ).objectReferenceValue = targ.UltimateRadialButtonList[ i ].icon.rectTransform;
									serializedObject.ApplyModifiedProperties();

									EditorUtility.SetDirty( targ.UltimateRadialButtonList[ i ].icon );
								}
								
								if( targ.UltimateRadialButtonList[ i ].icon == null )
								{
									EditorGUILayout.BeginVertical( "Box" );
									iconPlaceholderSprite = ( Sprite )EditorGUILayout.ObjectField( iconPlaceholderSprite, typeof( Sprite ), false );
									EditorGUILayout.BeginHorizontal();
									if( GUILayout.Button( "Generate One", EditorStyles.miniButtonLeft ) )
									{
										GameObject newIcon = new GameObject();
										newIcon.AddComponent<RectTransform>();
										newIcon.AddComponent<CanvasRenderer>();
										newIcon.AddComponent<Image>();

										newIcon.transform.SetParent( targ.UltimateRadialButtonList[ i ].buttonTransform );
										newIcon.GetComponent<RectTransform>().pivot = new Vector2( 0.5f, 0.5f );
										newIcon.name = "Icon";
										icon[ i ].objectReferenceValue = newIcon;
										serializedObject.ApplyModifiedProperties();
										Undo.RegisterCreatedObjectUndo( newIcon, "Create Icon Object" );

										if( iconPlaceholderSprite != null )
										{
											newIcon.GetComponent<Image>().sprite = iconPlaceholderSprite;
											EditorUtility.SetDirty( newIcon );
										}
									}
									if( GUILayout.Button( "Generate All", EditorStyles.miniButtonRight ) )
									{
										for( int t = 0; t < targ.UltimateRadialButtonList.Count; t++ )
										{
											if( targ.UltimateRadialButtonList[ t ].useIcon == false )
											{
												useIcon[ t ].boolValue = true;
												serializedObject.ApplyModifiedProperties();
											}

											if( targ.UltimateRadialButtonList[ t ].icon == null )
											{
												GameObject newIcon = new GameObject();
												newIcon.AddComponent<RectTransform>();
												newIcon.AddComponent<CanvasRenderer>();
												newIcon.AddComponent<Image>();

												newIcon.transform.SetParent( targ.UltimateRadialButtonList[ t ].buttonTransform );
												newIcon.GetComponent<RectTransform>().pivot = new Vector2( 0.5f, 0.5f );
												newIcon.name = "Icon";
												icon[ t ].objectReferenceValue = newIcon;
												serializedObject.ApplyModifiedProperties();

												Undo.RegisterCreatedObjectUndo( newIcon, "Create Icon Objects" );

												if( iconPlaceholderSprite != null )
												{
													newIcon.GetComponent<Image>().sprite = iconPlaceholderSprite;
													EditorUtility.SetDirty( newIcon );
												}
											}
										}
									}
									EditorGUILayout.EndHorizontal();
									EditorGUILayout.EndVertical();
								}
								else
								{
									if( i == 0 )
									{
										EditorGUI.BeginChangeCheck();
										EditorGUILayout.Slider( iconSize, 0.0f, 1.0f, new GUIContent( "Icon Size", "The size of the icon image transform." ) );
										EditorGUILayout.Slider( iconHorizontalPosition, 0.0f, 100.0f, new GUIContent( "Horizontal Position", "The horizontal position in relation to the radial button transform." ) );
										EditorGUILayout.Slider( iconVerticalPosition, 0.0f, 100.0f, new GUIContent( "Vertical Position", "The vertical position in relation to the radial button transform." ) );
										EditorGUILayout.PropertyField( iconRotation, new GUIContent( "Rotation Offset", "The rotation offset to apply to the icon transform." ) );
										EditorGUILayout.PropertyField( iconLocalRotation, new GUIContent( "Local Rotation", "Determines if the icon transform will use local or global rotation." ) );
										if( EditorGUI.EndChangeCheck() )
											serializedObject.ApplyModifiedProperties();

										// --------- COLOR CHANGE SETTINGS --------- //
										EditorGUILayout.BeginHorizontal();
										EditorGUI.BeginChangeCheck();
										EditorGUILayout.PropertyField( iconColorChange, new GUIContent( "Color Change", "Determines whether or not the icon will change color when being interacted with." ) );
										if( EditorGUI.EndChangeCheck() )
										{
											serializedObject.ApplyModifiedProperties();
											ResetRadialMenuButtons();
											IconColorChangeOptions.target = iconColorChange.boolValue;
											EditorPrefs.SetBool( "URM_IconColorChange", IconColorChangeOptions.target );
										}
										if( iconColorChange.boolValue == true )
										{
											GUILayout.FlexibleSpace();
											if( GUILayout.Button( IconColorChangeOptions.target == true ? "-" : "+", EditorStyles.miniButton, GUILayout.Width( 17 ) ) )
											{
												IconColorChangeOptions.target = !IconColorChangeOptions.target;
												EditorPrefs.SetBool( "URM_IconColorChange", IconColorChangeOptions.target );
											}
										}
										EditorGUILayout.EndHorizontal();

										if( EditorGUILayout.BeginFadeGroup( IconColorChangeOptions.faded ) )
										{
											EditorGUI.indentLevel = 2;
											EditorGUI.BeginChangeCheck();
											EditorGUILayout.PropertyField( iconHighlightedColor, new GUIContent( "Highlighted Color", "The color to be applied to the icon when highlighted." ) );
											EditorGUILayout.PropertyField( iconPressedColor, new GUIContent( "Pressed Color", "The color to be applied to the icon when pressed." ) );
											if( EditorGUI.EndChangeCheck() )
												serializedObject.ApplyModifiedProperties();

											EditorGUI.BeginChangeCheck();
											EditorGUILayout.PropertyField( iconDisabledColor, new GUIContent( "Disabled Color", "The color to be applied to the icon when disabled." ) );
											if( EditorGUI.EndChangeCheck() )
											{
												serializedObject.ApplyModifiedProperties();

												for( int n = 0; n < targ.UltimateRadialButtonList.Count; n++ )
												{
													if( targ.UltimateRadialButtonList[ n ].buttonDisabled && targ.UltimateRadialButtonList[ n ].useIcon && targ.UltimateRadialButtonList[ n ].icon != null )
													{
														targ.UltimateRadialButtonList[ n ].icon.color = iconDisabledColor.colorValue;
														EditorUtility.SetDirty( targ.UltimateRadialButtonList[ n ].icon );
													}
												}
											}
											EditorGUI.indentLevel = 1;
											EditorGUILayout.Space();
										}
										if( DefaultInspector.target && RadialButtonSettings.faded == 1.0f && MenuButtonAnimBools[ i ].faded == 1.0f && RadialButtonNotDisabled[ i ].faded == 1.0f && RadialButtonUseIcon[ i ].faded == 1.0f )
											EditorGUILayout.EndFadeGroup();
										// --------- COLOR CHANGE SETTINGS --------- //

										// --------- SCALE TRANSFORM SETTINGS --------- //
										EditorGUILayout.BeginHorizontal();
										EditorGUI.BeginChangeCheck();
										EditorGUILayout.PropertyField( iconScaleTransform, new GUIContent( "Scale Transform", "Determines whether or not the icon will scale when being interacted with." ) );
										if( EditorGUI.EndChangeCheck() )
										{
											serializedObject.ApplyModifiedProperties();
											IconScaleTransformOptions.target = iconScaleTransform.boolValue;
											EditorPrefs.SetBool( "URM_IconScaleTransform", IconScaleTransformOptions.target );
										}
										if( iconScaleTransform.boolValue == true )
										{
											GUILayout.FlexibleSpace();
											if( GUILayout.Button( IconScaleTransformOptions.target == true ? "-" : "+", EditorStyles.miniButton, GUILayout.Width( 17 ) ) )
											{
												IconScaleTransformOptions.target = !IconScaleTransformOptions.target;
												EditorPrefs.SetBool( "URM_IconScaleTransform", IconScaleTransformOptions.target );
											}
										}
										EditorGUILayout.EndHorizontal();

										if( EditorGUILayout.BeginFadeGroup( IconScaleTransformOptions.faded ) )
										{
											EditorGUI.indentLevel = 2;
											EditorGUI.BeginChangeCheck();
											EditorGUILayout.Slider( iconHighlightedScaleModifier, 0.5f, 1.5f, new GUIContent( "Highlighted Scale", "The scale modifier to be applied to the icon transform when highlighted." ) );
											EditorGUILayout.Slider( iconPressedScaleModifier, 0.5f, 1.5f, new GUIContent( "Pressed Scale", "The scale modifier to be applied to the icon transform when pressed." ) );
											if( EditorGUI.EndChangeCheck() )
												serializedObject.ApplyModifiedProperties();
											EditorGUI.indentLevel = 1;
										}
										if( DefaultInspector.target && RadialButtonSettings.faded == 1.0f && MenuButtonAnimBools[ i ].faded == 1.0f && RadialButtonNotDisabled[ i ].faded == 1.0f && RadialButtonUseIcon[ i ].faded == 1.0f )
											EditorGUILayout.EndFadeGroup();
										// --------- END SCALE TRANSFORM SETTINGS --------- //
									}
									else
									{
										if( iconLocalRotation.boolValue )
										{
											if( GUILayout.Button( "Invert Y Scale", EditorStyles.miniButton ) )
											{
												Vector3 originalScale = targ.UltimateRadialButtonList[ i ].icon.rectTransform.localScale;
												originalScale.y *= -1;
												targ.UltimateRadialButtonList[ i ].icon.rectTransform.localScale = originalScale;
												EditorUtility.SetDirty( targ.UltimateRadialButtonList[ i ].icon );
											}
										}

										EditorGUI.BeginChangeCheck();
										EditorGUILayout.PropertyField( rmbUseIconUnique[ i ], new GUIContent( "Unique Positioning", "Determines if the icon should use positioning different from the prefab radial button or not." ) );
										if( EditorGUI.EndChangeCheck() )
										{
											if( rmbUseIconUnique[ i ].boolValue == true )
											{
												if( rmbIconSize[ i ].floatValue == 0.0f )
													rmbIconSize[ i ].floatValue = iconSize.floatValue;
												if( rmbIconHorizontalPosition[ i ].floatValue == 0.0f )
													rmbIconHorizontalPosition[ i ].floatValue = iconHorizontalPosition.floatValue;
												if( rmbIconVerticalPosition[ i ].floatValue == 0.0f )
													rmbIconVerticalPosition[ i ].floatValue = iconVerticalPosition.floatValue;
												if( rmbIconRotation[ i ].floatValue == 0.0f )
													rmbIconRotation[ i ].floatValue = iconRotation.floatValue;
											}
											serializedObject.ApplyModifiedProperties();
										}
										if( rmbUseIconUnique[ i ].boolValue )
										{
											EditorGUI.BeginChangeCheck();
											EditorGUILayout.Slider( rmbIconSize[ i ], 0.0f, 1.0f, new GUIContent( "Icon Size", "The size of the icon image transform." ) );
											EditorGUILayout.Slider( rmbIconHorizontalPosition[ i ], 0.0f, 100.0f, new GUIContent( "Horizontal Position", "The horizontal position in relation to the radial button transform." ) );
											EditorGUILayout.Slider( rmbIconVerticalPosition[ i ], 0.0f, 100.0f, new GUIContent( "Vertical Position", "The vertical position in relation to the radial button transform." ) );
											EditorGUILayout.PropertyField( rmbIconRotation[ i ], new GUIContent( "Rotation Offset", "The rotation offset to apply to the icon transform." ) );
											if( EditorGUI.EndChangeCheck() )
												serializedObject.ApplyModifiedProperties();
										}
									}
								}
								EditorGUI.indentLevel = 0;

								EditorGUILayout.Space();
							}
							if( DefaultInspector.target && RadialButtonSettings.faded == 1.0f && MenuButtonAnimBools[ i ].faded == 1.0f && RadialButtonNotDisabled[ i ].faded == 1.0f )
								EditorGUILayout.EndFadeGroup();

							// ------------------------------------------- TEXT SETTINGS ------------------------------------------- //

							EditorGUILayout.BeginHorizontal();
							EditorGUI.BeginChangeCheck();
							EditorGUILayout.PropertyField( useText[ i ], new GUIContent( "Use Text", "Determines whether or not an text component should be used  to display information for this radial button." ) );
							if( EditorGUI.EndChangeCheck() )
							{
								serializedObject.ApplyModifiedProperties();
								ResetRadialMenuButtons();

								RadialButtonUseText[ i ].target = useText[ i ].boolValue;
								EditorPrefs.SetBool( "URM_UseText", RadialButtonUseText[ i ].target );
								UpdateRadialButtonUseText();

								if( useText[ i ].boolValue == false && targ.UltimateRadialButtonList[ i ].text != null )
									targ.UltimateRadialButtonList[ i ].text.gameObject.SetActive( false );
								else if( useText[ i ].boolValue == true && targ.UltimateRadialButtonList[ i ].text != null )
									targ.UltimateRadialButtonList[ i ].text.gameObject.SetActive( true );
							}
							if( useText[ i ].boolValue == true )
							{
								GUILayout.FlexibleSpace();
								if( GUILayout.Button( RadialButtonUseText[ i ].target == true ? "-" : "+", EditorStyles.miniButton, GUILayout.Width( 17 ) ) )
								{
									RadialButtonUseText[ i ].target = !RadialButtonUseText[ i ].target;
									EditorPrefs.SetBool( "URM_UseText", RadialButtonUseText[ i ].target );
									UpdateRadialButtonUseText();
								}
							}
							EditorGUILayout.EndHorizontal();

							if( EditorGUILayout.BeginFadeGroup( RadialButtonUseText[ i ].faded ) )
							{
								EditorGUI.indentLevel = 1;

								EditorGUI.BeginChangeCheck();
								EditorGUILayout.PropertyField( text[ i ] );
								if( EditorGUI.EndChangeCheck() )
									serializedObject.ApplyModifiedProperties();

								if( targ.UltimateRadialButtonList[ i ].text == null )
								{
									EditorGUILayout.BeginVertical( "Box" );
									placeholderFont = ( Font )EditorGUILayout.ObjectField( placeholderFont, typeof( Font ), false );
									EditorGUILayout.BeginHorizontal();
									if( GUILayout.Button( "Generate One", EditorStyles.miniButtonLeft ) )
									{
										GameObject newText = new GameObject();
										newText.AddComponent<RectTransform>();
										newText.AddComponent<CanvasRenderer>();
										newText.AddComponent<Text>();

										newText.transform.SetParent( targ.UltimateRadialButtonList[ i ].buttonTransform );
										newText.transform.SetAsLastSibling();
										newText.GetComponent<RectTransform>().position = targ.UltimateRadialButtonList[ radialNameListIndex ].buttonTransform.position;
										newText.GetComponent<RectTransform>().pivot = new Vector2( 0.5f, 0.5f );
										newText.name = "Text";
										newText.GetComponent<Text>().text = "Text";
										newText.GetComponent<Text>().resizeTextForBestFit = true;
										newText.GetComponent<Text>().resizeTextMinSize = 0;
										newText.GetComponent<Text>().resizeTextMaxSize = 300;
										newText.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
										text[ i ].objectReferenceValue = newText;
										serializedObject.ApplyModifiedProperties();

										Undo.RegisterCreatedObjectUndo( newText, "Create Text Object" );

										if( placeholderFont != null )
										{
											newText.GetComponent<Text>().font = placeholderFont;
											EditorUtility.SetDirty( newText );
										}
									}
									if( GUILayout.Button( "Generate All", EditorStyles.miniButtonRight ) )
									{
										for( int n = 0; n < targ.UltimateRadialButtonList.Count; n++ )
										{
											if( targ.UltimateRadialButtonList[ n ].useText == false )
											{
												useText[ n ].boolValue = true;
												serializedObject.ApplyModifiedProperties();
											}

											if( targ.UltimateRadialButtonList[ n ].text == null )
											{
												GameObject newText = new GameObject();
												newText.AddComponent<RectTransform>();
												newText.AddComponent<CanvasRenderer>();
												newText.AddComponent<Text>();

												newText.transform.SetParent( targ.UltimateRadialButtonList[ n ].buttonTransform );
												newText.transform.SetAsLastSibling();
												newText.GetComponent<RectTransform>().position = targ.UltimateRadialButtonList[ radialNameListIndex ].buttonTransform.position;
												newText.GetComponent<RectTransform>().pivot = new Vector2( 0.5f, 0.5f );
												newText.name = "Text";
												newText.GetComponent<Text>().text = "Text";
												newText.GetComponent<Text>().resizeTextForBestFit = true;
												newText.GetComponent<Text>().resizeTextMinSize = 0;
												newText.GetComponent<Text>().resizeTextMaxSize = 300;
												newText.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
												text[ n ].objectReferenceValue = newText;

												Undo.RegisterCreatedObjectUndo( newText, "Create Text Objects" );

												if( placeholderFont != null )
												{
													newText.GetComponent<Text>().font = placeholderFont;
													EditorUtility.SetDirty( newText );
												}
											}
											serializedObject.ApplyModifiedProperties();
										}
									}
									EditorGUILayout.EndHorizontal();
									EditorGUILayout.EndVertical();
								}
								else
								{
									EditorGUI.BeginChangeCheck();
									EditorGUILayout.PropertyField( textNormalColor, GUIContent.none );
									if( EditorGUI.EndChangeCheck() )
									{
										serializedObject.ApplyModifiedProperties();

										for( int n = 0; n < targ.UltimateRadialButtonList.Count; n++ )
										{
											if( targ.UltimateRadialButtonList[ n ].buttonDisabled )
												continue;

											targ.UltimateRadialButtonList[ n ].text.color = textNormalColor.colorValue;
											EditorUtility.SetDirty( targ.UltimateRadialButtonList[ n ].text );
										}
									}

									EditorGUI.BeginChangeCheck();
									EditorGUILayout.PropertyField( displayNameOnButton, new GUIContent( "Display Name", "Determines if the name of the button should be applied to the text or not." ) );
									if( EditorGUI.EndChangeCheck() )
									{
										serializedObject.ApplyModifiedProperties();

										if( displayNameOnButton.boolValue && targ.UltimateRadialButtonList[ i ].text != null )
										{
											targ.UltimateRadialButtonList[ i ].text.text = buttonName[ i ].stringValue;
											EditorUtility.SetDirty( targ.UltimateRadialButtonList[ i ].text );
										}
									}
									if( !displayNameOnButton.boolValue )
									{
										EditorGUI.BeginChangeCheck();
										targ.UltimateRadialButtonList[ i ].text.text = EditorGUILayout.TextField( targ.UltimateRadialButtonList[ i ].text.text );
										if( EditorGUI.EndChangeCheck() )
											EditorUtility.SetDirty( targ.UltimateRadialButtonList[ i ].text );
									}

									if( i == 0 )
									{
										EditorGUI.BeginChangeCheck();
										EditorGUILayout.Slider( textAreaRatioX, 0.0f, 1.0f, new GUIContent( "Text Ratio X", "The horizontal ratio of the text transform." ) );
										EditorGUILayout.Slider( textAreaRatioY, 0.0f, 1.0f, new GUIContent( "Text Ratio Y", "The vertical ratio of the text transform." ) );
										EditorGUILayout.Slider( textSize, 0.0f, 0.5f, new GUIContent( "Text Size", "The overall size of the text transform." ) );
										EditorGUILayout.Slider( textHorizontalPosition, 0.0f, 100.0f, new GUIContent( "Horizontal Position", "The horizontal position of the text transform." ) );
										EditorGUILayout.Slider( textVerticalPosition, 0.0f, 100.0f, new GUIContent( "Vertical Position", "The horizontal position of the text transform." ) );
										if( EditorGUI.EndChangeCheck() )
											serializedObject.ApplyModifiedProperties();

										EditorGUI.BeginChangeCheck();
										EditorGUILayout.PropertyField( textPositioningOption, new GUIContent( "Positioning Option", "Determines how the text will position itself. Global uses global positioning, Local uses local position and rotation and Relative to Icon uses the icon transform as center." ) );
										if( EditorGUI.EndChangeCheck() )
										{
											serializedObject.ApplyModifiedProperties();

											TextPositioningOptionLocal.target = targ.textPositioningOption == UltimateRadialMenu.TextPositioningOption.Local;
										}

										if( EditorGUILayout.BeginFadeGroup( TextPositioningOptionLocal.faded ) )
										{
											EditorGUI.indentLevel = 2;
											EditorGUI.BeginChangeCheck();
											EditorGUILayout.PropertyField( textLocalRotation, new GUIContent( "Local Rotation", "Determines if the text should follow the local rotation of the button or not." ) );
											if( EditorGUI.EndChangeCheck() )
												serializedObject.ApplyModifiedProperties();
											EditorGUI.indentLevel = 1;
										}
										if( DefaultInspector.target && RadialButtonSettings.faded == 1.0f && MenuButtonAnimBools[ i ].faded == 1.0f && RadialButtonNotDisabled[ i ].faded == 1.0f && RadialButtonUseText[ i ].faded == 1.0f )
											EditorGUILayout.EndFadeGroup();

										// --------- COLOR CHANGE SETTINGS --------- //
										EditorGUILayout.BeginHorizontal();
										EditorGUI.BeginChangeCheck();
										EditorGUILayout.PropertyField( textColorChange, new GUIContent( "Color Change", "Determines whether or not the text will change color when being interacted with." ) );
										if( EditorGUI.EndChangeCheck() )
										{
											serializedObject.ApplyModifiedProperties();
											ResetRadialMenuButtons();
											TextColorChangeOptions.target = textColorChange.boolValue;
											EditorPrefs.SetBool( "URM_TextColorChange", TextColorChangeOptions.target );
										}
										if( textColorChange.boolValue == true )
										{
											GUILayout.FlexibleSpace();
											if( GUILayout.Button( TextColorChangeOptions.target == true ? "-" : "+", EditorStyles.miniButton, GUILayout.Width( 17 ) ) )
											{
												TextColorChangeOptions.target = !TextColorChangeOptions.target;
												EditorPrefs.SetBool( "URM_TextColorChange", TextColorChangeOptions.target );
											}
										}
										EditorGUILayout.EndHorizontal();

										if( EditorGUILayout.BeginFadeGroup( TextColorChangeOptions.faded ) )
										{
											EditorGUI.indentLevel = 2;
											EditorGUI.BeginChangeCheck();
											EditorGUILayout.PropertyField( textHighlightedColor, new GUIContent( "Highlighted Color", "The color to be applied to the icon when highlighted." ) );
											EditorGUILayout.PropertyField( textPressedColor, new GUIContent( "Pressed Color", "The color to be applied to the icon when pressed." ) );
											if( EditorGUI.EndChangeCheck() )
												serializedObject.ApplyModifiedProperties();

											EditorGUI.BeginChangeCheck();
											EditorGUILayout.PropertyField( textDisabledColor, new GUIContent( "Disabled Color", "The color to be applied to the icon when disabled." ) );
											if( EditorGUI.EndChangeCheck() )
											{
												serializedObject.ApplyModifiedProperties();

												for( int n = 0; n < targ.UltimateRadialButtonList.Count; n++ )
												{
													if( targ.UltimateRadialButtonList[ n ].buttonDisabled && targ.UltimateRadialButtonList[ n ].useIcon && targ.UltimateRadialButtonList[ n ].icon != null )
													{
														targ.UltimateRadialButtonList[ n ].icon.color = iconDisabledColor.colorValue;
														EditorUtility.SetDirty( targ.UltimateRadialButtonList[ n ].icon );
													}
												}
											}
											EditorGUI.indentLevel = 1;
										}
										if( DefaultInspector.target && RadialButtonSettings.faded == 1.0f && MenuButtonAnimBools[ i ].faded == 1.0f && RadialButtonNotDisabled[ i ].faded == 1.0f && RadialButtonUseText[ i ].faded == 1.0f )
											EditorGUILayout.EndFadeGroup();
										// --------- COLOR CHANGE SETTINGS --------- //
									}
								}
								EditorGUILayout.Space();
								EditorGUI.indentLevel = 0;
							}
							if( DefaultInspector.target && RadialButtonSettings.faded == 1.0f && MenuButtonAnimBools[ i ].faded == 1.0f && RadialButtonNotDisabled[ i ].faded == 1.0f )
								EditorGUILayout.EndFadeGroup();

							EditorGUILayout.BeginHorizontal();
							GUIStyle unityEventLabelStyle = new GUIStyle( GUI.skin.label );

							if( targ.UltimateRadialButtonList[ i ].unityEvent.GetPersistentEventCount() > 0 )
								unityEventLabelStyle.fontStyle = FontStyle.Bold;

							EditorGUILayout.LabelField( "Unity Events", unityEventLabelStyle );
							GUILayout.FlexibleSpace();
							if( GUILayout.Button( RadialButtonUnityEvents.target == true ? "-" : "+", EditorStyles.miniButton, GUILayout.Width( 17 ) ) )
							{
								RadialButtonUnityEvents.target = !RadialButtonUnityEvents.target;
								EditorPrefs.SetBool( "URM_RadialButtonUnityEvents", RadialButtonUnityEvents.target );
							}
							EditorGUILayout.EndHorizontal();
							if( EditorGUILayout.BeginFadeGroup( RadialButtonUnityEvents.faded ) )
							{
								EditorGUI.BeginChangeCheck();
								EditorGUILayout.PropertyField( unityEvent[ i ] );
								if( EditorGUI.EndChangeCheck() )
									serializedObject.ApplyModifiedProperties();
							}
							if( DefaultInspector.target && RadialButtonSettings.faded == 1.0f && MenuButtonAnimBools[ i ].faded == 1.0f && RadialButtonNotDisabled[ i ].faded == 1.0f )
								EditorGUILayout.EndFadeGroup();
						}
						if( DefaultInspector.target && RadialButtonSettings.faded == 1.0f && MenuButtonAnimBools[ i ].faded == 1.0f )
							EditorGUILayout.EndFadeGroup();
					}
					if( DefaultInspector.faded == 1.0f && RadialButtonSettings.faded == 1.0f )
						EditorGUILayout.EndFadeGroup();
				}
				// ------------------------------------------------------------ END RADIAL BUTTON FOR LOOP ------------------------------------------------------------ //
				GUILayout.Space( 1 );
				EditorGUILayout.EndVertical();

				if( GUILayout.Button( "Clear Radial Buttons", EditorStyles.miniButton ) )
				{
					if( EditorUtility.DisplayDialog( "Ultimate Radial Menu", "Warning!\n\nAre you sure that you want to delete all of the radial buttons?", "Yes", "No" ) )
					{
						DeleteRadialImages();
						StoreReferences();
					}
				}
			}
			if( DefaultInspector.faded == 1.0f )
				EditorGUILayout.EndFadeGroup();
			#endregion
			
			EditorGUILayout.Space();

			if( PrefabUtility.GetPrefabType( Selection.activeGameObject ) == PrefabType.Prefab )
			{
				GUISkin defaultSkin = GUI.skin;
				int defaultFontSize = defaultSkin.FindStyle( "Button" ).fontSize;
				defaultSkin.FindStyle( "Button" ).fontSize = 12;
				EditorGUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				if( GUILayout.Button( "Add to Scene", GUILayout.Height( 25 ), GUILayout.Width( 200 ) ) )
					UltimateRadialMenuCreator.CreateNewUltimateRadialMenu( targ.gameObject );
				GUILayout.FlexibleSpace();
				GUILayout.Space( 10 );
				EditorGUILayout.EndHorizontal();
				defaultSkin.FindStyle( "Button" ).fontSize = defaultFontSize;
			}
			#region SCRIPT REFERENCE
			else
			{
				DisplayHeaderDropdown( "Script Reference", "UUI_ScriptReference", ScriptReference );
				if( EditorGUILayout.BeginFadeGroup( ScriptReference.faded ) )
				{
					EditorGUILayout.Space();

					EditorGUI.BeginChangeCheck();
					EditorGUILayout.PropertyField( radialMenuName, new GUIContent( "Radial Menu Name", "The name to be used for reference from scripts." ) );
					if( EditorGUI.EndChangeCheck() )
					{
						serializedObject.ApplyModifiedProperties();

						RadialMenuNameDuplicate.target = DuplicateRadialMenuName();
						RadialMenuNameUnassigned.target = targ.radialMenuName == string.Empty ? true : false;
						RadialMenuNameAssigned.target = RadialMenuNameDuplicate.target == false && targ.radialMenuName != string.Empty ? true : false;
					}

					if( EditorGUILayout.BeginFadeGroup( RadialMenuNameUnassigned.faded ) )
					{
						EditorGUILayout.HelpBox( "Please make sure to assign a name so that this radial menu can be referenced from your scripts.", MessageType.Warning );
					}
					if( DefaultInspector.faded == 1.0f && ScriptReference.faded == 1.0f )
						EditorGUILayout.EndFadeGroup();

					if( EditorGUILayout.BeginFadeGroup( RadialMenuNameDuplicate.faded ) )
					{
						EditorGUILayout.HelpBox( "This name has already been used in your scene. Please make sure to make the Radial Menu Name unique.", MessageType.Error );
					}
					if( DefaultInspector.faded == 1.0f && ScriptReference.faded == 1.0f )
						EditorGUILayout.EndFadeGroup();

					if( EditorGUILayout.BeginFadeGroup( RadialMenuNameAssigned.faded ) )
					{
						EditorGUILayout.BeginVertical( "Box" );
						GUILayout.Space( 1 );
						EditorGUILayout.LabelField( "Example Code Generator", EditorStyles.boldLabel );

						exampleCodeIndex = EditorGUILayout.Popup( "Function", exampleCodeIndex, exampleCodeOptions.ToArray() );

						EditorGUILayout.LabelField( "Function Description", EditorStyles.boldLabel );
						GUIStyle wordWrappedLabel = new GUIStyle( GUI.skin.label ) { wordWrap = true };
						EditorGUILayout.LabelField( exampleCodes[ exampleCodeIndex ].optionDescription, wordWrappedLabel );

						EditorGUILayout.LabelField( "Example Code", EditorStyles.boldLabel );
						GUIStyle wordWrappedTextArea = new GUIStyle( GUI.skin.textArea ) { wordWrap = true };
						EditorGUILayout.TextArea( string.Format( exampleCodes[ exampleCodeIndex ].basicCode, radialMenuName.stringValue, exampleCodeIndex == 1 ? "\"" + targ.UltimateRadialButtonList[ radialNameListIndex ].name + "\"" : radialNameListIndex.ToString() ), wordWrappedTextArea );

						if( exampleCodeIndex <= 3 )
						{
							EditorGUILayout.LabelField( "Needed Variable", EditorStyles.boldLabel );
							EditorGUILayout.TextArea( "UltimateRadialButtonInfo radialButtonInfo;", wordWrappedTextArea );
						}

						GUILayout.Space( 1 );
						EditorGUILayout.EndVertical();
					}
					if( DefaultInspector.faded == 1.0f && ScriptReference.faded == 1.0f )
						EditorGUILayout.EndFadeGroup();

					if( GUILayout.Button( "Open Online Documentation" ) )
					{
						Debug.Log( "Ultimate Radial Menu\nOpening Online Documentation" );
						Application.OpenURL( "https://www.tankandhealerstudio.com/ultimate-radial-menu_documentation.html" );
					}
				}
				if( DefaultInspector.faded == 1.0f )
					EditorGUILayout.EndFadeGroup();

				EditorGUILayout.Space();
			}
			#endregion
		}
		EditorGUILayout.EndFadeGroup();

		Repaint();
	}

	bool GetCanvasError ()
	{
		// If the selection is currently empty, then return false.
		if( Selection.activeGameObject == null )
			return false;

		// If the selection is actually the prefab within the Project window, then return no errors.
		if( PrefabUtility.GetPrefabType( Selection.activeGameObject ) == PrefabType.Prefab )
			return false;

		// If parentCanvas is unassigned, then get a new canvas and return no errors.
		if( parentCanvas == null )
		{
			parentCanvas = GetParentCanvas();
			
			if( parentCanvas == null )
				return false;
		}

		// If the parentCanvas is not enabled, then return true for errors.
		if( parentCanvas.enabled == false )
			return true;

		// If the canvas' renderMode is not the needed one, then return true for errors.
		if( parentCanvas.renderMode != RenderMode.ScreenSpaceOverlay )
			return true;

		// If the canvas has a CanvasScaler component and it is not the correct option.
		if( parentCanvas.GetComponent<CanvasScaler>() && parentCanvas.GetComponent<CanvasScaler>().uiScaleMode != CanvasScaler.ScaleMode.ConstantPixelSize )
			return true;

		return false;
	}

	Canvas GetParentCanvas ()
	{
		if( Selection.activeGameObject == null )
			return null;

		// Store the current parent.
		Transform parent = Selection.activeGameObject.transform.parent;

		// Loop through parents as long as there is one.
		while( parent != null )
		{
			// If there is a Canvas component, return that gameObject.
			if( parent.transform.GetComponent<Canvas>() && parent.transform.GetComponent<Canvas>().enabled == true )
				return parent.transform.GetComponent<Canvas>();

			// Else, shift to the next parent.
			parent = parent.transform.parent;
		}
		if( parent == null && PrefabUtility.GetPrefabType( Selection.activeGameObject ) != PrefabType.Prefab )
			UltimateRadialMenuCreator.RequestCanvas( Selection.activeGameObject );
		return null;
	}

	bool DuplicateRadialMenuName ()
	{
		UltimateRadialMenu[] ultimateRadialMenus = FindObjectsOfType<UltimateRadialMenu>();

		for( int i = 0; i < ultimateRadialMenus.Length; i++ )
		{
			if( ultimateRadialMenus[ i ].radialMenuName == string.Empty )
				continue;

			if( ultimateRadialMenus[ i ] != targ && ultimateRadialMenus[ i ].radialMenuName == targ.radialMenuName )
				return true;
		}

		return false;
	}

	void ResetRadialMenuButtons ()
	{
		for( int i = 0; i < targ.UltimateRadialButtonList.Count; i++ )
		{
			targ.UltimateRadialButtonList[ i ].OnExit();

			if( targ.UltimateRadialButtonList[ i ].buttonDisabled )
			{
				targ.UltimateRadialButtonList[ i ].DisableButton();

				if( targ.UltimateRadialButtonList[ i ].useIcon && targ.UltimateRadialButtonList[ i ].icon != null )
				{
					if( targ.iconColorChange )
						targ.UltimateRadialButtonList[ i ].icon.color = iconDisabledColor.colorValue;
					else
						targ.UltimateRadialButtonList[ i ].icon.color = iconNormalColor.colorValue;

					EditorUtility.SetDirty( targ.UltimateRadialButtonList[ i ].icon );
				}
			}
		}
	}

	void UpdateMenuButtonAnimBools ( int index )
	{
		for( int i = 0; i < targ.UltimateRadialButtonList.Count; i++ )
		{
			if( radialNameListIndex == i )
				MenuButtonAnimBools[ i ].target = true;
			else
				MenuButtonAnimBools[ i ].target = false;
		}
	}

	void UpdateRadialButtonUseIcon ()
	{
		for( int i = 0; i < targ.UltimateRadialButtonList.Count; i++ )
		{
			RadialButtonUseIcon[ i ].target = EditorPrefs.GetBool( "URM_UseIcon" ) && targ.UltimateRadialButtonList[ i ].useIcon;
		}
	}

	void UpdateRadialButtonUseText ()
	{
		for( int i = 0; i < targ.UltimateRadialButtonList.Count; i++ )
		{
			RadialButtonUseText[ i ].target = EditorPrefs.GetBool( "URM_UseText" ) && targ.UltimateRadialButtonList[ i ].useText;
		}
	}

	void AddNewRadialButton ( int index )
	{
		serializedObject.FindProperty( "UltimateRadialButtonList" ).InsertArrayElementAtIndex( index );
		serializedObject.ApplyModifiedProperties();

		GameObject newGameObject = targ.UltimateRadialButtonList[ 0 ].buttonTransform.gameObject;

		if( newGameObject == null )
		{
			newGameObject = new GameObject();
			newGameObject.AddComponent<RectTransform>();
			newGameObject.AddComponent<CanvasRenderer>();
			newGameObject.AddComponent<Image>();
			newGameObject.GetComponent<Image>().sprite = targ.normalSprite;
		}

		GameObject image = Instantiate( newGameObject.gameObject, Vector3.zero, Quaternion.identity );
		image.transform.SetParent( targ.transform );
		image.transform.SetSiblingIndex( targ.UltimateRadialButtonList[ targ.UltimateRadialButtonList.Count - 1 ].buttonTransform.GetSiblingIndex() + 1 );

		image.gameObject.name = "Radial Image " + ( targ.UltimateRadialButtonList.Count ).ToString( "00" );

		RectTransform trans = image.GetComponent<RectTransform>();
		
		trans.anchorMin = new Vector2( 0.5f, 0.5f );
		trans.anchorMax = new Vector2( 0.5f, 0.5f );
		trans.pivot = new Vector2( 0.5f, 0.5f );

		serializedObject.FindProperty( "menuButtonCount" ).intValue++;
		serializedObject.FindProperty( string.Format( "UltimateRadialButtonList.Array.data[{0}].radialMenu", index ) ).objectReferenceValue = targ;
		serializedObject.FindProperty( string.Format( "UltimateRadialButtonList.Array.data[{0}].buttonTransform", index ) ).objectReferenceValue = trans;
		serializedObject.FindProperty( string.Format( "UltimateRadialButtonList.Array.data[{0}].radialImage", index ) ).objectReferenceValue = trans.GetComponent<Image>();

		serializedObject.FindProperty( string.Format( "UltimateRadialButtonList.Array.data[{0}].useIcon", index ) ).boolValue = targ.UltimateRadialButtonList[ 0 ].useIcon;
		serializedObject.FindProperty( string.Format( "UltimateRadialButtonList.Array.data[{0}].useText", index ) ).boolValue = targ.UltimateRadialButtonList[ 0 ].useText;

		if( serializedObject.FindProperty( string.Format( "UltimateRadialButtonList.Array.data[{0}].useIcon", index ) ).boolValue == true )
		{
			Image[] images = trans.GetComponentsInChildren<Image>();
			for( int i = 0; i < images.Length; i++ )
			{
				if( images[ i ] != trans.GetComponent<Image>() )
				{
					serializedObject.FindProperty( string.Format( "UltimateRadialButtonList.Array.data[{0}].icon", index ) ).objectReferenceValue = images[ i ];
					serializedObject.FindProperty( string.Format( "UltimateRadialButtonList.Array.data[{0}].iconTransform", index ) ).objectReferenceValue = images[ i ].rectTransform;
				}
			}
		}
		if( serializedObject.FindProperty( string.Format( "UltimateRadialButtonList.Array.data[{0}].useText", index ) ).boolValue == true )
			serializedObject.FindProperty( string.Format( "UltimateRadialButtonList.Array.data[{0}].text", index ) ).objectReferenceValue = trans.GetComponentInChildren<Text>();

		serializedObject.FindProperty( string.Format( "UltimateRadialButtonList.Array.data[{0}].useIconUnique", index ) ).boolValue = false;
		serializedObject.FindProperty( string.Format( "UltimateRadialButtonList.Array.data[{0}].buttonDisabled", index ) ).boolValue = targ.UltimateRadialButtonList[ 0 ].buttonDisabled;
		serializedObject.FindProperty( string.Format( "UltimateRadialButtonList.Array.data[{0}].name", index ) ).stringValue = string.Empty;
		serializedObject.FindProperty( string.Format( "UltimateRadialButtonList.Array.data[{0}].description", index ) ).stringValue = string.Empty;
		serializedObject.FindProperty( string.Format( "UltimateRadialButtonList.Array.data[{0}].iconSize", index ) ).floatValue = 0.0f;
		serializedObject.FindProperty( string.Format( "UltimateRadialButtonList.Array.data[{0}].iconHorizontalPosition", index ) ).floatValue = 0.0f;
		serializedObject.FindProperty( string.Format( "UltimateRadialButtonList.Array.data[{0}].iconVerticalPosition", index ) ).floatValue = 0.0f;
		serializedObject.FindProperty( string.Format( "UltimateRadialButtonList.Array.data[{0}].iconRotation", index ) ).floatValue = 0.0f;
		serializedObject.FindProperty( string.Format( "UltimateRadialButtonList.Array.data[{0}].id", index ) ).intValue = 0;
		serializedObject.FindProperty( string.Format( "UltimateRadialButtonList.Array.data[{0}].key", index ) ).stringValue = string.Empty;
		serializedObject.ApplyModifiedProperties();
		
		float angle = targ.GetAnglePerButton;
		angleOffset.floatValue = angle / 2;
		buttonInputAngle.floatValue = 1.0f;
		serializedObject.ApplyModifiedProperties();

		Undo.RegisterCreatedObjectUndo( image, "Create Radial Button Object" );

		radialNameListIndex = index;

		StoreReferences();

		if( newGameObject != targ.UltimateRadialButtonList[ 0 ].buttonTransform.gameObject )
			DestroyImmediate( newGameObject );
	}

	void RemoveRadialButton ( int index )
	{
		Undo.DestroyObjectImmediate( targ.UltimateRadialButtonList[ index ].radialImage.gameObject );
		menuButtonCount.intValue = menuButtonCount.intValue - 1;
		serializedObject.FindProperty( "UltimateRadialButtonList" ).DeleteArrayElementAtIndex( index );
		serializedObject.ApplyModifiedProperties();

		float angle = targ.GetAnglePerButton;
		angleOffset.floatValue = angle / 2;
		buttonInputAngle.floatValue = 1.0f;
		serializedObject.ApplyModifiedProperties();

		if( index == targ.UltimateRadialButtonList.Count )
		{
			radialNameListIndex = targ.UltimateRadialButtonList.Count - 1;
		}
		
		EditorUtility.SetDirty( targ );
		StoreReferences();
	}

	void UpdateRadialButtonPrefab ()
	{
		if( targ.UltimateRadialButtonList.Count == 0 )
			return;
		
		serializedObject.FindProperty( "radialButtonPrefab.prefab" ).objectReferenceValue = targ.UltimateRadialButtonList[ 0 ].buttonTransform.gameObject;
		serializedObject.FindProperty( "radialButtonPrefab.radialButton.radialMenu" ).objectReferenceValue = targ.UltimateRadialButtonList[ 0 ].radialMenu;
		serializedObject.FindProperty( "radialButtonPrefab.radialButton.radialImage" ).objectReferenceValue = targ.UltimateRadialButtonList[ 0 ].radialImage;
		serializedObject.FindProperty( "radialButtonPrefab.radialButton.buttonTransform" ).objectReferenceValue = targ.UltimateRadialButtonList[ 0 ].buttonTransform;
		serializedObject.FindProperty( "radialButtonPrefab.radialButton.useIcon" ).boolValue = targ.UltimateRadialButtonList[ 0 ].useIcon;
		serializedObject.FindProperty( "radialButtonPrefab.radialButton.useText" ).boolValue = targ.UltimateRadialButtonList[ 0 ].useText;
		serializedObject.FindProperty( "radialButtonPrefab.radialButton.icon" ).objectReferenceValue = targ.UltimateRadialButtonList[ 0 ].icon;
		serializedObject.FindProperty( "radialButtonPrefab.radialButton.text" ).objectReferenceValue = targ.UltimateRadialButtonList[ 0 ].text;
		serializedObject.FindProperty( "radialButtonPrefab.radialButton.useIconUnique" ).boolValue = false;
		serializedObject.FindProperty( "radialButtonPrefab.radialButton.iconSize" ).floatValue = 0.0f;
		serializedObject.FindProperty( "radialButtonPrefab.radialButton.iconHorizontalPosition" ).floatValue = 0.0f;
		serializedObject.FindProperty( "radialButtonPrefab.radialButton.iconVerticalPosition" ).floatValue = 0.0f;
		serializedObject.FindProperty( "radialButtonPrefab.radialButton.iconRotation" ).floatValue = 0.0f;
		serializedObject.FindProperty( "radialButtonPrefab.radialButton.name" ).stringValue = "";
		serializedObject.FindProperty( "radialButtonPrefab.radialButton.key" ).stringValue = "";
		serializedObject.FindProperty( "radialButtonPrefab.radialButton.description" ).stringValue = "";
		serializedObject.FindProperty( "radialButtonPrefab.radialButton.id" ).intValue = 0;
		serializedObject.FindProperty( "radialButtonPrefab.radialButton.buttonDisabled" ).boolValue = false;

		serializedObject.ApplyModifiedProperties();
	}

	void StoreReferences ()
	{
		targ = ( UltimateRadialMenu )target;

		if( targ == null )
			return;

		if( radialNameListIndex >= targ.UltimateRadialButtonList.Count )
			radialNameListIndex = 0;

		CanvasError = new AnimBool( GetCanvasError() );

		DefaultInspector = new AnimBool( CanvasError.target == false && targ.UltimateRadialButtonList.Count > 0 );
		GenerateInspector = new AnimBool( CanvasError.target == false && targ.UltimateRadialButtonList.Count == 0 );

		RadialMenuPositioning = new AnimBool( EditorPrefs.GetBool( "URM_RadialMenuPositioning" ) );
		RadialMenuSettings = new AnimBool( EditorPrefs.GetBool( "URM_RadialMenuSettings" ) );
		RadialButtonSettings = new AnimBool( EditorPrefs.GetBool( "URM_MenuButtonCustomization" ) );
		ScriptReference = new AnimBool( EditorPrefs.GetBool( "UUI_ScriptReference" ) );

		SpriteSwapOptions = new AnimBool( EditorPrefs.GetBool( "URM_SpriteSwap" ) && targ.spriteSwap );
		ColorChangeOptions = new AnimBool( EditorPrefs.GetBool( "URM_ColorChange" ) && targ.colorChange );
		ScaleTransformOptions = new AnimBool( EditorPrefs.GetBool( "URM_ScaleTransform" ) && targ.scaleTransform );
		DisplaySelectionTextOptions = new AnimBool( EditorPrefs.GetBool( "URM_DisplaySelectionTextOptions" ) && targ.displayButtonName );
		if( targ.displayButtonName && targ.nameText == null )
		{
			DisplaySelectionTextOptions = new AnimBool( true );
			EditorPrefs.SetBool( "URM_DisplaySelectionTextOptions", true );
		}

		DisplayDescriptionTextOptions = new AnimBool( EditorPrefs.GetBool( "URM_DisplayDescriptionTextOptions" ) && targ.displayButtonDescription );
		if( targ.displayButtonDescription && targ.descriptionText == null )
		{
			DisplayDescriptionTextOptions = new AnimBool( true );
			EditorPrefs.SetBool( "URM_DisplayDescriptionTextOptions", true );
		}

		/* ----- > GENERATE RADIAL MENU OPTIONS < ----- */
		menuButtonCount = serializedObject.FindProperty( "menuButtonCount" );
		followOrbitalRotation = serializedObject.FindProperty( "followOrbitalRotation" );

		/* ----- > RADIAL MENU POSITIONING < ----- */
		scalingAxis = serializedObject.FindProperty( "scalingAxis" );
		menuSize = serializedObject.FindProperty( "menuSize" );
		horizontalPosition = serializedObject.FindProperty( "horizontalPosition" );
		verticalPosition = serializedObject.FindProperty( "verticalPosition" );
		menuButtonSize = serializedObject.FindProperty( "menuButtonSize" );
		infiniteMaxRange = serializedObject.FindProperty( "infiniteMaxRange" );
		radialMenuButtonRadius = serializedObject.FindProperty( "radialMenuButtonRadius" );
		angleOffset = serializedObject.FindProperty( "angleOffset" );
		minRange = serializedObject.FindProperty( "minRange" );
		maxRange = serializedObject.FindProperty( "maxRange" );
		buttonInputAngle = serializedObject.FindProperty( "buttonInputAngle" );

		/* ----- > RADIAL MENU SETTINGS < ----- */
		// COLOR CHANGE //
		colorChange = serializedObject.FindProperty( "colorChange" );
		normalColor = serializedObject.FindProperty( "normalColor" );
		highlightedColor = serializedObject.FindProperty( "highlightedColor" );
		pressedColor = serializedObject.FindProperty( "pressedColor" );
		disabledColor = serializedObject.FindProperty( "disabledColor" );
		// SPRITE SWAP //
		spriteSwap = serializedObject.FindProperty( "spriteSwap" );
		normalSprite = serializedObject.FindProperty( "normalSprite" );
		highlightedSprite = serializedObject.FindProperty( "highlightedSprite" );
		pressedSprite = serializedObject.FindProperty( "pressedSprite" );
		disabledSprite = serializedObject.FindProperty( "disabledSprite" );
		// SCALE TRANSFORM //
		scaleTransform = serializedObject.FindProperty( "scaleTransform" );
		highlightedScaleModifier = serializedObject.FindProperty( "highlightedScaleModifier" );
		pressedScaleModifier = serializedObject.FindProperty( "pressedScaleModifier" );
		positionModifier = serializedObject.FindProperty( "positionModifier" );
		// TEXT SETTINGS //
		displayButtonName = serializedObject.FindProperty( "displayButtonName" );
		nameText = serializedObject.FindProperty( "nameText" );
		nameTextRatioX = serializedObject.FindProperty( "nameTextRatioX" );
		nameTextRatioY = serializedObject.FindProperty( "nameTextRatioY" );
		nameTextSize = serializedObject.FindProperty( "nameTextSize" );
		nameTextHorizontalPosition = serializedObject.FindProperty( "nameTextHorizontalPosition" );
		nameTextVerticalPosition = serializedObject.FindProperty( "nameTextVerticalPosition" );
		displayButtonDescription = serializedObject.FindProperty( "displayButtonDescription" );
		descriptionText = serializedObject.FindProperty( "descriptionText" );
		descriptionTextRatioX = serializedObject.FindProperty( "descriptionTextRatioX" );
		descriptionTextRatioY = serializedObject.FindProperty( "descriptionTextRatioY" );
		descriptionTextSize = serializedObject.FindProperty( "descriptionTextSize" );
		descriptionTextHorizontalPosition = serializedObject.FindProperty( "descriptionTextHorizontalPosition" );
		descriptionTextVerticalPosition = serializedObject.FindProperty( "descriptionTextVerticalPosition" );
		// TOGGLE SETTINGS //
		radialMenuToggle = serializedObject.FindProperty( "radialMenuToggle" );
		toggleInDuration = serializedObject.FindProperty( "toggleInDuration" );
		toggleOutDuration = serializedObject.FindProperty( "toggleOutDuration" );
		// DISABLE SETTINGS //
		disableIcon = serializedObject.FindProperty( "disableIcon" );
		disableText = serializedObject.FindProperty( "disableText" );

		/* ----- > MENU BUTTON CUSTOMIZATION < ----- */
		// UNIFORM SETTINGS //
		iconSize = serializedObject.FindProperty( "iconSize" );
		iconHorizontalPosition = serializedObject.FindProperty( "iconHorizontalPosition" );
		iconVerticalPosition = serializedObject.FindProperty( "iconVerticalPosition" );
		iconRotation = serializedObject.FindProperty( "iconRotation" );
		iconLocalRotation = serializedObject.FindProperty( "iconLocalRotation" );
		
		IconColorChangeOptions = new AnimBool( EditorPrefs.GetBool( "URM_IconColorChange" ) && targ.iconColorChange );
		IconScaleTransformOptions = new AnimBool( EditorPrefs.GetBool( "URM_IconScaleTransform" ) && targ.iconScaleTransform );

		iconColorChange = serializedObject.FindProperty( "iconColorChange" );
		iconDisabledColor = serializedObject.FindProperty( "iconDisabledColor" );
		iconNormalColor = serializedObject.FindProperty( "iconNormalColor" );
		iconHighlightedColor = serializedObject.FindProperty( "iconHighlightedColor" );
		iconPressedColor = serializedObject.FindProperty( "iconPressedColor" );
		iconScaleTransform = serializedObject.FindProperty( "iconScaleTransform" );
		iconHighlightedScaleModifier = serializedObject.FindProperty( "iconHighlightedScaleModifier" );
		iconPressedScaleModifier = serializedObject.FindProperty( "iconPressedScaleModifier" );

		TextPositioningOptionLocal = new AnimBool( targ.textPositioningOption == UltimateRadialMenu.TextPositioningOption.Local );
		textLocalRotation = serializedObject.FindProperty( "textLocalRotation" );

		TextColorChangeOptions = new AnimBool( EditorPrefs.GetBool( "URM_TextColorChange" ) && targ.textColorChange );
		textColorChange = serializedObject.FindProperty( "textColorChange" );
		textDisabledColor = serializedObject.FindProperty( "textDisabledColor" );
		textNormalColor = serializedObject.FindProperty( "textNormalColor" );
		textHighlightedColor = serializedObject.FindProperty( "textHighlightedColor" );
		textPressedColor = serializedObject.FindProperty( "textPressedColor" );

		textSize = serializedObject.FindProperty( "textSize" );
		textHorizontalPosition = serializedObject.FindProperty( "textHorizontalPosition" );
		textVerticalPosition = serializedObject.FindProperty( "textVerticalPosition" );
		textAreaRatioX = serializedObject.FindProperty( "textAreaRatioX" );
		textAreaRatioY = serializedObject.FindProperty( "textAreaRatioY" );
		textPositioningOption = serializedObject.FindProperty( "textPositioningOption" );
		displayNameOnButton = serializedObject.FindProperty( "displayNameOnButton" );

		buttonTransform = new List<SerializedProperty>();
		buttonName = new List<SerializedProperty>();
		buttonKey = new List<SerializedProperty>();
		buttonId = new List<SerializedProperty>();
		description = new List<SerializedProperty>();
		useIcon = new List<SerializedProperty>();
		useText = new List<SerializedProperty>();
		icon = new List<SerializedProperty>();
		rmbIconSize = new List<SerializedProperty>();
		rmbIconHorizontalPosition = new List<SerializedProperty>();
		rmbIconVerticalPosition = new List<SerializedProperty>();
		rmbIconRotation = new List<SerializedProperty>();
		rmbUseIconUnique = new List<SerializedProperty>();
		text = new List<SerializedProperty>();
		buttonDisabled = new List<SerializedProperty>();
		unityEvent = new List<SerializedProperty>();

		MenuButtonAnimBools = new List<AnimBool>();
		RadialButtonNotDisabled = new List<AnimBool>();
		RadialButtonUnityEvents = new AnimBool( EditorPrefs.GetBool( "URM_RadialButtonUnityEvents" ) );
		RadialButtonUseIcon = new List<AnimBool>();
		RadialButtonUseText = new List<AnimBool>();

		buttonNames = new List<string>();
		for( int i = 0; i < targ.UltimateRadialButtonList.Count; i++ )
		{
			buttonNames.Add( "Radial Button " + i.ToString( "00" ) );
			if( targ.UltimateRadialButtonList[ i ].buttonTransform.name.Contains( "Radial" ) )
				targ.UltimateRadialButtonList[ i ].buttonTransform.name = "Radial Button " + i.ToString( "00" );

			if( i > 0 )
				targ.UltimateRadialButtonList[ i ].buttonTransform.SetSiblingIndex( targ.UltimateRadialButtonList[ i - 1 ].buttonTransform.GetSiblingIndex() + 1 );

			MenuButtonAnimBools.Add( i == radialNameListIndex ? new AnimBool( true ) : new AnimBool( false ) );
			RadialButtonNotDisabled.Add( new AnimBool( !targ.UltimateRadialButtonList[ i ].buttonDisabled ) );
			RadialButtonUseIcon.Add( new AnimBool( EditorPrefs.GetBool( "URM_UseIcon" ) && targ.UltimateRadialButtonList[ i ].useIcon ) );
			RadialButtonUseText.Add( new AnimBool( EditorPrefs.GetBool( "URM_UseText" ) && targ.UltimateRadialButtonList[ i ].useText ) );

			buttonTransform.Add( serializedObject.FindProperty( string.Format( "UltimateRadialButtonList.Array.data[{0}].buttonTransform", i ) ) );
			buttonName.Add( serializedObject.FindProperty( string.Format( "UltimateRadialButtonList.Array.data[{0}].name", i ) ) );
			buttonKey.Add( serializedObject.FindProperty( string.Format( "UltimateRadialButtonList.Array.data[{0}].key", i ) ) );
			buttonId.Add( serializedObject.FindProperty( string.Format( "UltimateRadialButtonList.Array.data[{0}].id", i ) ) );
			description.Add( serializedObject.FindProperty( string.Format( "UltimateRadialButtonList.Array.data[{0}].description", i ) ) );

			useIcon.Add( serializedObject.FindProperty( string.Format( "UltimateRadialButtonList.Array.data[{0}].useIcon", i ) ) );
			icon.Add( serializedObject.FindProperty( string.Format( "UltimateRadialButtonList.Array.data[{0}].icon", i ) ) );
			rmbIconSize.Add( serializedObject.FindProperty( string.Format( "UltimateRadialButtonList.Array.data[{0}].iconSize", i ) ) );
			rmbIconHorizontalPosition.Add( serializedObject.FindProperty( string.Format( "UltimateRadialButtonList.Array.data[{0}].iconHorizontalPosition", i ) ) );
			rmbUseIconUnique.Add( serializedObject.FindProperty( string.Format( "UltimateRadialButtonList.Array.data[{0}].useIconUnique", i ) ) );
			rmbIconVerticalPosition.Add( serializedObject.FindProperty( string.Format( "UltimateRadialButtonList.Array.data[{0}].iconVerticalPosition", i ) ) );
			rmbIconRotation.Add( serializedObject.FindProperty( string.Format( "UltimateRadialButtonList.Array.data[{0}].iconRotation", i ) ) );

			useText.Add( serializedObject.FindProperty( string.Format( "UltimateRadialButtonList.Array.data[{0}].useText", i ) ) );
			text.Add( serializedObject.FindProperty( string.Format( "UltimateRadialButtonList.Array.data[{0}].text", i ) ) );
			buttonDisabled.Add( serializedObject.FindProperty( string.Format( "UltimateRadialButtonList.Array.data[{0}].buttonDisabled", i ) ) );
			unityEvent.Add( serializedObject.FindProperty( string.Format( "UltimateRadialButtonList.Array.data[{0}].unityEvent", i ) ) );

			if( targ.UltimateRadialButtonList[ i ].useIcon && targ.UltimateRadialButtonList[ i ].icon != null )
			{
				targ.UltimateRadialButtonList[ i ].icon.color = iconNormalColor.colorValue;
				EditorUtility.SetDirty( targ.UltimateRadialButtonList[ i ].icon );
			}
		}

		/* ----- > SCRIPT REFERENCE < ----- */
		RadialMenuNameDuplicate = new AnimBool( DuplicateRadialMenuName() );
		RadialMenuNameUnassigned = new AnimBool( targ.radialMenuName == string.Empty );
		RadialMenuNameAssigned = new AnimBool( RadialMenuNameDuplicate.target == false && targ.radialMenuName != string.Empty );
		radialMenuName = serializedObject.FindProperty( "radialMenuName" );
		exampleCodeOptions = new List<string>();

		for( int i = 0; i < exampleCodes.Length; i++ )
			exampleCodeOptions.Add( exampleCodes[ i ].optionName );

		if( !Application.isPlaying )
			UpdateRadialButtonPrefab();
		
		UpdateColors();
	}

	void UpdateColors ()
	{
		for( int i = 0; i < targ.UltimateRadialButtonList.Count; i++ )
		{
			if( targ.UltimateRadialButtonList[ i ].radialImage != null )
			{
				if( targ.UltimateRadialButtonList[ i ].buttonDisabled )
					targ.UltimateRadialButtonList[ i ].radialImage.color = targ.disabledColor;
				else
					targ.UltimateRadialButtonList[ i ].radialImage.color = targ.normalColor;

				EditorUtility.SetDirty( targ.UltimateRadialButtonList[ i ].radialImage );
			}

			if( targ.UltimateRadialButtonList[ i ].useIcon && targ.UltimateRadialButtonList[ i ].icon != null )
			{
				targ.UltimateRadialButtonList[ i ].icon.color = targ.iconNormalColor;
				EditorUtility.SetDirty( targ.UltimateRadialButtonList[ i ].icon );
			}
		}
	}

	void GenerateRadialImages ()
	{
		GameObject newGameObject = new GameObject();
		newGameObject.AddComponent<RectTransform>();
		newGameObject.AddComponent<CanvasRenderer>();
		newGameObject.AddComponent<Image>();

		newGameObject.GetComponent<Image>().color = targ.normalColor;

		if( targ.normalSprite != null )
			newGameObject.GetComponent<Image>().sprite = targ.normalSprite;

		newGameObject.transform.SetParent( targ.transform );

		float angle = targ.GetAnglePerButton;
		for( int i = 0; i < targ.menuButtonCount; i++ )
		{
			GameObject image = Instantiate( newGameObject, Vector3.zero, Quaternion.identity );
			image.transform.SetParent( targ.transform );

			image.gameObject.name = "Radial Image " + i.ToString( "00" );

			RectTransform trans = image.GetComponent<RectTransform>();

			trans.anchorMin = new Vector2( 0.5f, 0.5f );
			trans.anchorMax = new Vector2( 0.5f, 0.5f );
			trans.pivot = new Vector2( 0.5f, 0.5f );

			serializedObject.FindProperty( "UltimateRadialButtonList" ).arraySize++;
			serializedObject.ApplyModifiedProperties();

			targ.UltimateRadialButtonList[ i ] = new UltimateRadialMenu.UltimateRadialButton();
			serializedObject.FindProperty( string.Format( "UltimateRadialButtonList.Array.data[{0}].radialMenu", i ) ).objectReferenceValue = targ;
			serializedObject.FindProperty( string.Format( "UltimateRadialButtonList.Array.data[{0}].buttonTransform", i ) ).objectReferenceValue = trans;
			serializedObject.FindProperty( string.Format( "UltimateRadialButtonList.Array.data[{0}].radialImage", i ) ).objectReferenceValue = trans.GetComponent<Image>();
			serializedObject.ApplyModifiedProperties();
			
			Undo.RegisterCreatedObjectUndo( image, "Create Radial Button Objects" );
		}

		angleOffset.floatValue = angle / 2;
		buttonInputAngle.floatValue = 1.0f;
		serializedObject.ApplyModifiedProperties();
		StoreReferences();

		DestroyImmediate( newGameObject );

		if( !FindObjectOfType<EventSystem>().GetComponent<UltimateRadialMenuInputManager>() )
			FindObjectOfType<EventSystem>().gameObject.AddComponent<UltimateRadialMenuInputManager>();
	}

	void DeleteRadialImages ()
	{
		for( int i = targ.UltimateRadialButtonList.Count - 1; i >= 0; i-- )
			DestroyImmediate( targ.UltimateRadialButtonList[ i ].buttonTransform.gameObject );

		serializedObject.FindProperty( "UltimateRadialButtonList" ).ClearArray();
		serializedObject.ApplyModifiedProperties();

		StoreReferences();
	}

	void OnSceneGUI ()
	{
		if( Selection.activeGameObject == null || Application.isPlaying || Selection.objects.Length > 1 )
			return;

		RectTransform trans = Selection.activeGameObject.transform.GetComponent<RectTransform>();
		Vector3 center = ( Vector2 )trans.position;

		Handles.color = Color.black;

		if( GenerateInspector.target == true )
		{
			Handles.color = Color.black;
			Handles.DrawWireDisc( center + ( Selection.activeGameObject.transform.forward ), Selection.activeGameObject.transform.forward, ( trans.sizeDelta.x / 2 ) );

			Handles.color = Color.red;
			float buttonRadius = trans.sizeDelta.x / 20;
			float angleInRadians = -( 360f / targ.menuButtonCount ) * Mathf.Deg2Rad;
			for( int i = 0; i < targ.menuButtonCount; i++ )
			{
				Vector3 newPosition = center;
				newPosition.x += ( Mathf.Cos( ( ( angleInRadians * i ) ) + ( 90 * Mathf.Deg2Rad ) ) * ( ( trans.sizeDelta.x / 2 ) - buttonRadius ) );
				newPosition.y += ( Mathf.Sin( ( ( angleInRadians * i ) ) + ( 90 * Mathf.Deg2Rad ) ) * ( ( trans.sizeDelta.x / 2 ) - buttonRadius ) );
				Handles.DrawWireDisc( newPosition, Selection.activeGameObject.transform.forward, buttonRadius );
			}
			SceneView.RepaintAll();
			return;
		}

		if( RadialMenuPositioning.target == true )
		{
			Handles.color = Color.black;
			if( minRangeBeingChanged && framesToWait < 200 )
				Handles.color = Color.green;

			Handles.DrawWireDisc( center + ( Selection.activeGameObject.transform.forward ), Selection.activeGameObject.transform.forward, ( trans.sizeDelta.x / 2 ) * targ.minRange );
			Handles.Label( center + ( Vector3.down * ( ( trans.sizeDelta.x / 2 ) * targ.minRange ) ), "Min Range: " + targ.minRange );

			Handles.color = Color.black;

			if( maxRangeBeingChanged && framesToWait < 200 )
				Handles.color = Color.green;

			if( !targ.infiniteMaxRange )
			{
				Handles.DrawWireDisc( center + ( Selection.activeGameObject.transform.forward ), Selection.activeGameObject.transform.forward, ( trans.sizeDelta.x / 2 ) * targ.maxRange );
				Handles.Label( center + ( Vector3.down * ( ( trans.sizeDelta.x / 2 ) * targ.maxRange ) ), "Max Range: " + ( targ.infiniteMaxRange ? "Infinite" : targ.maxRange.ToString() ) );
			}
			Handles.color = Color.black;

			framesToWait++;

			if( targ.UltimateRadialButtonList.Count > 0 )
			{
				Handles.color = Color.cyan;

				float maxRange = targ.maxRange;
				if( targ.infiniteMaxRange )
					maxRange = 1.5f;

				Vector3 lineStart = center;
				lineStart.x += ( Mathf.Cos( targ.UltimateRadialButtonList[ 0 ].minAngle * Mathf.Deg2Rad ) * ( ( trans.sizeDelta.x / 2 ) * targ.minRange ) );
				lineStart.y += ( Mathf.Sin( targ.UltimateRadialButtonList[ 0 ].minAngle * Mathf.Deg2Rad ) * ( ( trans.sizeDelta.x / 2 ) * targ.minRange ) );
				Vector3 lineEnd = center;
				lineEnd.x += ( Mathf.Cos( targ.UltimateRadialButtonList[ 0 ].minAngle * Mathf.Deg2Rad ) * ( ( trans.sizeDelta.x / 2 ) * maxRange ) );
				lineEnd.y += ( Mathf.Sin( targ.UltimateRadialButtonList[ 0 ].minAngle * Mathf.Deg2Rad ) * ( ( trans.sizeDelta.x / 2 ) * maxRange ) );
				Handles.DrawLine( lineStart, lineEnd );

				lineStart = center;
				lineStart.x += ( Mathf.Cos( targ.UltimateRadialButtonList[ 0 ].maxAngle * Mathf.Deg2Rad ) * ( ( trans.sizeDelta.x / 2 ) * targ.minRange ) );
				lineStart.y += ( Mathf.Sin( targ.UltimateRadialButtonList[ 0 ].maxAngle * Mathf.Deg2Rad ) * ( ( trans.sizeDelta.x / 2 ) * targ.minRange ) );
				lineEnd = center;
				lineEnd.x += ( Mathf.Cos( targ.UltimateRadialButtonList[ 0 ].maxAngle * Mathf.Deg2Rad ) * ( ( trans.sizeDelta.x / 2 ) * maxRange ) );
				lineEnd.y += ( Mathf.Sin( targ.UltimateRadialButtonList[ 0 ].maxAngle * Mathf.Deg2Rad ) * ( ( trans.sizeDelta.x / 2 ) * maxRange ) );
				Handles.DrawLine( lineStart, lineEnd );

				Handles.color = Color.white;
				for( int i = 0; i < targ.UltimateRadialButtonList.Count; i++ )
				{
					if( targ.UltimateRadialButtonList[ i ].radialImage.sprite == null )
					{
						Vector3 transformSize = new Vector3( targ.UltimateRadialButtonList[ i ].buttonTransform.sizeDelta.x, targ.UltimateRadialButtonList[ i ].buttonTransform.sizeDelta.y, 1 );
						Handles.DrawWireCube( targ.UltimateRadialButtonList[ i ].buttonTransform.position, transformSize );
					}
				}
			}
		}

		if( RadialMenuSettings.target == true )
		{
			Handles.color = Color.yellow;
			if( targ.displayButtonName && targ.nameText != null && DisplaySelectionTextOptions.target == true )
			{
				Vector3 textAreaSize = new Vector3( targ.nameText.rectTransform.sizeDelta.x, targ.nameText.rectTransform.sizeDelta.y, 1 );
				Handles.DrawWireCube( targ.nameText.rectTransform.position, textAreaSize );
			}
			if( targ.displayButtonDescription && targ.descriptionText != null && DisplayDescriptionTextOptions.target == true )
			{
				Vector3 textAreaSize = new Vector3( targ.descriptionText.rectTransform.sizeDelta.x, targ.descriptionText.rectTransform.sizeDelta.y, 1 );
				Handles.DrawWireCube( targ.descriptionText.rectTransform.position, textAreaSize );
			}
		}

		if( RadialButtonSettings.target == true )
		{
			if( radialNameListIndex == 0 && targ.UltimateRadialButtonList[ 0 ].useText && targ.UltimateRadialButtonList[ 0 ].text != null && EditorPrefs.GetBool( "URM_UseText" ) )
			{
				Handles.color = Color.yellow;

				Vector3 textAreaSize = new Vector3( targ.UltimateRadialButtonList[ 0 ].text.rectTransform.sizeDelta.x, targ.UltimateRadialButtonList[ 0 ].text.rectTransform.sizeDelta.y, 1 );
				Handles.DrawWireCube( targ.UltimateRadialButtonList[ 0 ].text.rectTransform.position, textAreaSize );
			}

			for( int i = 0; i < targ.UltimateRadialButtonList.Count; i++ )
			{
				if( radialNameListIndex == i )
					Handles.color = Color.yellow;
				else
					Handles.color = Color.white;

				float handleSize = trans.sizeDelta.x / 10;
				float distanceMod = ( ( trans.sizeDelta.x / 2 ) * ( targ.radialMenuButtonRadius ) ) + handleSize;
				
				Vector3 handlePos = center;
				handlePos.x += ( Mathf.Cos( targ.UltimateRadialButtonList[ i ].angle * Mathf.Deg2Rad ) * ( distanceMod ) );
				handlePos.y += ( Mathf.Sin( targ.UltimateRadialButtonList[ i ].angle * Mathf.Deg2Rad ) * ( distanceMod ) );
				if( Handles.Button( handlePos, Quaternion.identity, handleSize, trans.sizeDelta.x / 10, Handles.SphereHandleCap ) )
				{
					radialNameListIndex = i;
					UpdateMenuButtonAnimBools( i );
					EditorGUIUtility.PingObject( targ.UltimateRadialButtonList[ i ].buttonTransform );
				}
				GUIStyle labelStyle = new GUIStyle( GUI.skin.label )
				{
					alignment = TextAnchor.MiddleCenter,
					fontStyle = FontStyle.Bold,
				};
				Handles.Label( handlePos, i.ToString( "00" ), labelStyle );
			}
		}

		SceneView.RepaintAll();
	}
}

/* Written by Kaz Crowe */
/* UltimateRadialMenuCreator.cs */
public class UltimateRadialMenuCreator
{
	public static void CreateNewUltimateRadialMenu ( GameObject radialMenuPrefab )
	{
		GameObject prefab = ( GameObject )Object.Instantiate( radialMenuPrefab, Vector3.zero, Quaternion.identity );
		prefab.name = radialMenuPrefab.name;
		Selection.activeGameObject = prefab;
		RequestCanvas( prefab );
	}

	private static void CreateNewCanvas ( GameObject child )
	{
		GameObject root = new GameObject( "Canvas" );
		root.layer = LayerMask.NameToLayer( "UI" );
		Canvas canvas = root.AddComponent<Canvas>();
		canvas.renderMode = RenderMode.ScreenSpaceOverlay;
		root.AddComponent<GraphicRaycaster>();
		Undo.RegisterCreatedObjectUndo( root, "Create " + root.name );

		child.transform.SetParent( root.transform, false );

		CreateEventSystem();
	}

	private static void CreateEventSystem ()
	{
		Object esys = Object.FindObjectOfType<EventSystem>();
		if( esys == null )
		{
			GameObject eventSystem = new GameObject( "EventSystem" );
			esys = eventSystem.AddComponent<EventSystem>();
			eventSystem.AddComponent<StandaloneInputModule>();
			#if UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
			eventSystem.AddComponent<TouchInputModule>();
			#endif
			eventSystem.AddComponent<UltimateRadialMenuInputManager>();

			Undo.RegisterCreatedObjectUndo( eventSystem, "Create " + eventSystem.name );
		}
	}

	/* PUBLIC STATIC FUNCTIONS */
	public static void RequestCanvas ( GameObject child )
	{
		Canvas[] allCanvas = Object.FindObjectsOfType( typeof( Canvas ) ) as Canvas[];

		for( int i = 0; i < allCanvas.Length; i++ )
		{
			if( allCanvas[ i ].renderMode == RenderMode.ScreenSpaceOverlay && allCanvas[ i ].enabled == true && ValidateCanvasScalerComponent( allCanvas[ i ] ) )
			{
				child.transform.SetParent( allCanvas[ i ].transform, false );
				CreateEventSystem();
				return;
			}
		}
		CreateNewCanvas( child );
	}

	static bool ValidateCanvasScalerComponent ( Canvas canvas )
	{
		if( !canvas.GetComponent<CanvasScaler>() )
			return true;
		else if( canvas.GetComponent<CanvasScaler>().uiScaleMode == CanvasScaler.ScaleMode.ConstantPixelSize )
			return true;

		return false;
	}

	public static void RequestNewWorldSpaceCanvas ( GameObject child )
	{
		CreateNewCanvas( child );
	}
}