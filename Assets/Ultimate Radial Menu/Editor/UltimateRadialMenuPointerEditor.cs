/* UltimateRadialMenuPointerEditor.cs */
/* Written by Kaz Crowe */
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor.AnimatedValues;

[CustomEditor( typeof( UltimateRadialMenuPointer ) )]
public class UltimateRadialMenuPointerEditor : Editor
{
	UltimateRadialMenuPointer targ;

	// Properties //
	SerializedProperty ultimateRadialMenu;
	SerializedProperty radialMenuPointer, pointerSize;
	SerializedProperty targetingSpeed, snappingOption;
	SerializedProperty alwaysOn, colorChange, changeOverTime;
	SerializedProperty fadeInDuration, fadeOutDuration;
	SerializedProperty colorChangeImage, rotationOffset;
	SerializedProperty normalColor, activeColor;
	SerializedProperty spriteSwap, spriteSwapImage;
	SerializedProperty normalSprite, activeSprite;
	
	// Sections //
	AnimBool UltimateRadialMenuAssigned, UltimateRadialMenuUnassigned;
	AnimBool AlwaysOnDisabled, ColorChangeOptions;
	AnimBool ColorChangeOverTimeOptions, RadialMenuPointerAssigned;
	AnimBool SpriteSwapOptions, RadialMenuPointerUnassigned;
	AnimBool InstantRotationDisabled;


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
	void DisplayHeader ( string headerName )
	{
		EditorGUILayout.BeginVertical( "Toolbar" );
		EditorGUILayout.LabelField( headerName, EditorStyles.boldLabel );
		EditorGUILayout.EndVertical();
	}

	public override void OnInspectorGUI ()
	{
		serializedObject.Update();

		EditorGUILayout.Space();

		EditorGUI.BeginChangeCheck();
		EditorGUILayout.PropertyField( ultimateRadialMenu, new GUIContent( "Ultimate Radial Menu", "The Ultimate Radial Menu to use for this pointer." ) );
		if( EditorGUI.EndChangeCheck() )
		{
			serializedObject.ApplyModifiedProperties();

			UltimateRadialMenuUnassigned.target = targ.ultimateRadialMenu == null;
			UltimateRadialMenuAssigned.target = targ.ultimateRadialMenu != null;
		}

		if( EditorGUILayout.BeginFadeGroup( UltimateRadialMenuUnassigned.faded ) )
		{
			EditorGUILayout.BeginVertical( "Box" );
			EditorGUILayout.HelpBox( "Please assign the targeted Ultimate Radial Menu before continuing.", MessageType.Warning );
			if( GUILayout.Button( "Find Radial Menu" ) )
			{
				ultimateRadialMenu.objectReferenceValue = targ.GetComponentInParent<UltimateRadialMenu>();
				radialMenuPointer.objectReferenceValue = targ.GetComponent<RectTransform>();
				serializedObject.ApplyModifiedProperties();

				UltimateRadialMenuAssigned.target = targ.ultimateRadialMenu == null ? false : true;
				UltimateRadialMenuUnassigned.target = targ.ultimateRadialMenu != null ? false : true;

				RadialMenuPointerUnassigned = new AnimBool( targ.radialMenuPointer == null );
				RadialMenuPointerAssigned = new AnimBool( targ.radialMenuPointer != null );

				if( targ.ultimateRadialMenu == null )
					Debug.LogWarning( "Ultimate Radial Menu Pointer - Could not find an Ultimate Radial Menu component in any parent GameObjects." );
			}
			EditorGUILayout.EndVertical();
		}
		EditorGUILayout.EndFadeGroup();

		if( EditorGUILayout.BeginFadeGroup( UltimateRadialMenuAssigned.faded ) )
		{
			EditorGUILayout.Space();
			DisplayHeader( "Pointer Positioning" );
			EditorGUILayout.Space();

			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( radialMenuPointer, new GUIContent( "Pointer Transform", "The transform to use for the pointer." ) );
			if( EditorGUI.EndChangeCheck() )
			{
				serializedObject.ApplyModifiedProperties();

				RadialMenuPointerUnassigned.target = targ.radialMenuPointer == null;
				RadialMenuPointerAssigned.target = targ.radialMenuPointer != null;
			}

			if( EditorGUILayout.BeginFadeGroup( RadialMenuPointerUnassigned.faded ) )
			{
				EditorGUILayout.BeginVertical( "Box" );
				EditorGUILayout.HelpBox( "The Pointer Transform component needs to be assigned.", MessageType.Error );
				if( GUILayout.Button( "Find", EditorStyles.miniButton ) )
				{
					radialMenuPointer.objectReferenceValue = targ.GetComponent<RectTransform>();
					serializedObject.ApplyModifiedProperties();
					
					RadialMenuPointerUnassigned.target = targ.radialMenuPointer == null;
					RadialMenuPointerAssigned.target = targ.radialMenuPointer != null;
				}
				EditorGUILayout.EndVertical();
			}
			if( UltimateRadialMenuAssigned.target == true )
				EditorGUILayout.EndFadeGroup();

			if( EditorGUILayout.BeginFadeGroup( RadialMenuPointerAssigned.faded ) )
			{
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.Slider( pointerSize, 0.0f, 1.0f, new GUIContent( "Pointer Size", "The overall size of the pointer image." ) );
				if( EditorGUI.EndChangeCheck() )
					serializedObject.ApplyModifiedProperties();

				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField( snappingOption, new GUIContent( "Snapping Option", "Determines how the pointer will snap to the current radial button." ) );
				if( EditorGUI.EndChangeCheck() )
				{
					serializedObject.ApplyModifiedProperties();

					InstantRotationDisabled.target = targ.snappingOption != UltimateRadialMenuPointer.SnappingOption.Instant;
				}

				if( EditorGUILayout.BeginFadeGroup( InstantRotationDisabled.faded ) )
				{
					EditorGUI.BeginChangeCheck();
					EditorGUILayout.Slider( targetingSpeed, 2.0f, 10.0f, new GUIContent( "Targeting Speed", "The speed at which the pointer will target the radial button." ) );
					if( EditorGUI.EndChangeCheck() )
						serializedObject.ApplyModifiedProperties();
				}
				if( RadialMenuPointerAssigned.target == true && UltimateRadialMenuAssigned.target == true )
					EditorGUILayout.EndFadeGroup();

				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField( rotationOffset, new GUIContent( "Rotation Offset", "The offset to apply to the pointer transform." ) );
				if( EditorGUI.EndChangeCheck() )
					serializedObject.ApplyModifiedProperties();
			}
			if( UltimateRadialMenuAssigned.target == true )
				EditorGUILayout.EndFadeGroup();

			EditorGUILayout.Space();
			DisplayHeader( "Visual Options" );
			EditorGUILayout.Space();

			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( alwaysOn, new GUIContent( "Always On", "Should the pointer image be on regardless of the radial menu state?" ) );
			if( EditorGUI.EndChangeCheck() )
			{
				serializedObject.ApplyModifiedProperties();

				AlwaysOnDisabled.target = !targ.alwaysOn;
			}

			if( EditorGUILayout.BeginFadeGroup( AlwaysOnDisabled.faded ) )
			{
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField( colorChange, new GUIContent( "Color Change", "Determines if the pointer image will change color." ) );
				if( EditorGUI.EndChangeCheck() )
				{
					serializedObject.ApplyModifiedProperties();

					ColorChangeOptions.target = targ.colorChange;
				}

				if( EditorGUILayout.BeginFadeGroup( ColorChangeOptions.faded ) )
				{
					EditorGUI.indentLevel = 1;

					EditorGUI.BeginChangeCheck();
					EditorGUILayout.PropertyField( colorChangeImage, new GUIContent( "Image", "The image to change the color of." ) );
					if( colorChangeImage.objectReferenceValue == null )
					{
						if( GUILayout.Button( "Find Image", EditorStyles.miniButton ) )
						{
							colorChangeImage.objectReferenceValue = targ.GetComponent<Image>();
							serializedObject.ApplyModifiedProperties();
						}
					}
					EditorGUILayout.PropertyField( normalColor, new GUIContent( "Normal Color", "The normal color of the pointer image." ) );
					if( EditorGUI.EndChangeCheck() )
					{
						serializedObject.ApplyModifiedProperties();

						if( targ.colorChangeImage != null )
						{
							targ.colorChangeImage.color = targ.normalColor;
							EditorUtility.SetDirty( targ.colorChangeImage );
						}
					}

					EditorGUI.BeginChangeCheck();
					EditorGUILayout.PropertyField( activeColor, new GUIContent( "Active Color", "The active color of the pointer image." ) );
					if( EditorGUI.EndChangeCheck() )
						serializedObject.ApplyModifiedProperties();

					EditorGUI.BeginChangeCheck();
					EditorGUILayout.PropertyField( changeOverTime, new GUIContent( "Change Over Time", "Determines if the pointer image should change color over time or not." ) );
					if( EditorGUI.EndChangeCheck() )
					{
						serializedObject.ApplyModifiedProperties();

						ColorChangeOverTimeOptions.target = targ.changeOverTime;
					}

					if( EditorGUILayout.BeginFadeGroup( ColorChangeOverTimeOptions.faded ) )
					{
						EditorGUI.BeginChangeCheck();
						EditorGUILayout.PropertyField( fadeInDuration, new GUIContent( "Fade In Duration", "The time is seconds for the pointer image to fade in." ) );
						EditorGUILayout.PropertyField( fadeOutDuration, new GUIContent( "Fade Out Duration", "The time in seconds for the pointer image to fade out." ) );
						if( EditorGUI.EndChangeCheck() )
							serializedObject.ApplyModifiedProperties();
					}
					if( ColorChangeOptions.target == true && AlwaysOnDisabled.target == true && UltimateRadialMenuAssigned.target == true )
						EditorGUILayout.EndFadeGroup();

					EditorGUI.indentLevel = 0;
					EditorGUILayout.Space();
				}
				if( AlwaysOnDisabled.target == true && UltimateRadialMenuAssigned.target == true )
					EditorGUILayout.EndFadeGroup();

				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField( spriteSwap, new GUIContent( "Sprite Swap", "Determines of the pointer image should swap sprites or not." ) );
				if( EditorGUI.EndChangeCheck() )
				{
					serializedObject.ApplyModifiedProperties();

					SpriteSwapOptions.target = targ.spriteSwap;
				}

				if( EditorGUILayout.BeginFadeGroup( SpriteSwapOptions.faded ) )
				{
					EditorGUI.indentLevel = 1;
					EditorGUI.BeginChangeCheck();
					EditorGUILayout.PropertyField( spriteSwapImage, new GUIContent( "Image", "The image component to swap the sprites of." ) );
					if( spriteSwapImage.objectReferenceValue == null )
					{
						if( GUILayout.Button( "Find Image", EditorStyles.miniButton ) )
						{
							spriteSwapImage.objectReferenceValue = targ.GetComponent<Image>();
							serializedObject.ApplyModifiedProperties();
						}
					}
					EditorGUILayout.PropertyField( normalSprite, new GUIContent( "Normal Sprite", "The normal sprite to apply to the image." ) );
					if( EditorGUI.EndChangeCheck() )
					{
						serializedObject.ApplyModifiedProperties();

						if( targ.spriteSwapImage != null && targ.normalSprite != null )
							targ.spriteSwapImage.sprite = targ.normalSprite;
					}

					EditorGUI.BeginChangeCheck();
					EditorGUILayout.PropertyField( activeSprite, new GUIContent( "Active Sprite", "The active sprite to apply to the image." ) );
					if( EditorGUI.EndChangeCheck() )
						serializedObject.ApplyModifiedProperties();

					EditorGUI.indentLevel = 0;
				}
				if( AlwaysOnDisabled.target == true && UltimateRadialMenuAssigned.target == true )
					EditorGUILayout.EndFadeGroup();
			}
			if( UltimateRadialMenuAssigned.target == true )
				EditorGUILayout.EndFadeGroup();
		}
		EditorGUILayout.EndFadeGroup();

		Repaint();
	}

	void StoreReferences ()
	{
		targ = ( UltimateRadialMenuPointer )target;

		ultimateRadialMenu = serializedObject.FindProperty( "ultimateRadialMenu" );
		radialMenuPointer = serializedObject.FindProperty( "radialMenuPointer" );
		pointerSize = serializedObject.FindProperty( "pointerSize" );
		targetingSpeed = serializedObject.FindProperty( "targetingSpeed" );
		snappingOption = serializedObject.FindProperty( "snappingOption" );
		rotationOffset = serializedObject.FindProperty( "rotationOffset" );
		alwaysOn = serializedObject.FindProperty( "alwaysOn" );
		colorChange = serializedObject.FindProperty( "colorChange" );
		colorChangeImage = serializedObject.FindProperty( "colorChangeImage" );
		changeOverTime = serializedObject.FindProperty( "changeOverTime" );
		fadeInDuration = serializedObject.FindProperty( "fadeInDuration" );
		fadeOutDuration = serializedObject.FindProperty( "fadeOutDuration" );
		normalColor = serializedObject.FindProperty( "normalColor" );
		activeColor = serializedObject.FindProperty( "activeColor" );
		spriteSwap = serializedObject.FindProperty( "spriteSwap" );
		spriteSwapImage = serializedObject.FindProperty( "spriteSwapImage" );
		normalSprite = serializedObject.FindProperty( "normalSprite" );
		activeSprite = serializedObject.FindProperty( "activeSprite" );

		UltimateRadialMenuUnassigned = new AnimBool( targ.ultimateRadialMenu == null );
		UltimateRadialMenuAssigned = new AnimBool( targ.ultimateRadialMenu != null );
		AlwaysOnDisabled = new AnimBool( !targ.alwaysOn );
		ColorChangeOptions = new AnimBool( targ.colorChange );
		ColorChangeOverTimeOptions = new AnimBool( targ.changeOverTime );
		SpriteSwapOptions = new AnimBool( targ.spriteSwap );
		RadialMenuPointerUnassigned = new AnimBool( targ.radialMenuPointer == null );
		RadialMenuPointerAssigned = new AnimBool( targ.radialMenuPointer != null );
		InstantRotationDisabled = new AnimBool( targ.snappingOption != UltimateRadialMenuPointer.SnappingOption.Instant );
	}
}