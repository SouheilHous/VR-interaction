// ----------------------------------------------------------------------
// File: 		VTextEditorHeader
// Organisation: 	Virtence GmbH
// Department:   	Simulation Development
// Copyright:    	© 2014 Virtence GmbH. All rights reserved
// Author:       	Silvio Lange (silvio.lange@virtence.com)
// ----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


/// <summary>
/// draw the toolbar
/// </summary>
public class VTextEditorToolbar : AbstractVTextEditorComponent
{	
	#region EVENTS

	#endregion // EVENTS


	#region CONSTANTS

	#endregion // CONSTANTS


	#region FIELDS
    private VTextEditorTools _currentToolbarValue;                                                  // the current selected item of the toolbar
    private GUIContent[] _content;
	#endregion // FIELDS


	#region PROPERTIES
    public VTextEditorTools CurrentToolbarValue {
        get { return _currentToolbarValue; }
        set { _currentToolbarValue = value; }
    }
	#endregion // PROPERTIES


	#region CONSTRUCTORS

    public VTextEditorToolbar(SerializedObject obj, Editor currentEditor) 
	{
        SetupGUIContent();
	}

	#endregion // CONSTRUCTORS


	#region METHODS
    /// <summary>
    /// Setups the text icon dictionary.
    /// </summary>
    private void SetupGUIContent() {
        Dictionary<string, Texture> textIconDict = new Dictionary<string, Texture>();          // this dictionary holds the icons and the texts for the toolbar buttons

        textIconDict.Add(VTextEditorTools.Style.ToString(), Resources.Load("Images/Icons/IconStyle") as Texture);
        textIconDict.Add(VTextEditorTools.Mesh.ToString(), Resources.Load("Images/Icons/IconMesh") as Texture);
        textIconDict.Add(VTextEditorTools.Layout.ToString(), Resources.Load("Images/Icons/IconLayout") as Texture);
        textIconDict.Add(VTextEditorTools.Physics.ToString(), Resources.Load("Images/Icons/IconPhysics") as Texture);
        textIconDict.Add(VTextEditorTools.Scripts.ToString(), Resources.Load("Images/Icons/IconScripts") as Texture);

        List<GUIContent> contentList = new List<GUIContent>();
        foreach (string contentName in Enum.GetNames(typeof(VTextEditorTools))) {
            if (textIconDict.ContainsKey(contentName)) {
                contentList.Add(new GUIContent(contentName, textIconDict[contentName]));
            } else {
                contentList.Add(new GUIContent(contentName));
            }
        }
        _content = contentList.ToArray();
    }

    /// <summary>
    /// draw the ui for this component
    /// 
    /// returns true if this aspect of the VText should be updated (mesh, layout, physics, etc)
    /// </summary>
    public override bool DrawUI()
    {        
        Rect lastRect = new Rect();
        GUILayout.BeginHorizontal("box");
        CurrentToolbarValue = (VTextEditorTools) (GUILayout.Toolbar((int) CurrentToolbarValue, _content, GUILayout.Height(30), GUILayout.MinWidth(lastRect.width)));
        GUILayout.EndHorizontal();
        lastRect = GUILayoutUtility.GetLastRect();

        return false;
    }

	#endregion // METHODS


	#region EVENT HANDLERS

	#endregion // EVENT HANDLERS
}
