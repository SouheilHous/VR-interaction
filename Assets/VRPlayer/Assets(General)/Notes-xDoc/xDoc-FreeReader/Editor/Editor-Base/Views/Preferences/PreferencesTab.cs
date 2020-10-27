#define CONDENSED_FORMAT
#define EMAIL_IN_FIELDS

using UnityEditor;
using UnityEngine;
using xDocBase.AssetManagement;
using xDocEditorBase.Windows;


namespace xDocEditorBase.Preferences {

	public static class PreferencesTab
	{
		const string versionAndCopyrightText = 
			"Version " + AssetManager.versionLabel
			+ ", " + AssetManager.versionDate
			+ ", (c) by " + AssetManager.companyName;

		static Vector2 scrollPosition;

		[PreferenceItem(AssetManager.productName)]
		private static void CustomPreferencesTab()
		{
			GUIStyle style = GUI.skin.label;
			style.wordWrap = true;
			EditorGUILayout.LabelField(versionAndCopyrightText, style);

			EditorGUILayout.Space();

			scrollPosition = GUILayout.BeginScrollView(scrollPosition);

#if CONDENSED_FORMAT
			EditorGUIUtility.labelWidth = 100;
			var goContent = new GUIContent("GO");
			const float goWidth = 30;
			EditorGUILayout.LabelField("Links", EditorStyles.boldLabel);
#endif			
			// --------------------------------------------------------------------------
			// --- Home Page
			// --------------------------------------------------------------------------
#if CONDENSED_FORMAT
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.TextField("Home Page", AssetManager.companySiteURL);
			if (GUILayout.Button(goContent, GUILayout.Width(goWidth))) {
				Application.OpenURL(AssetManager.companySiteURL);	
			}
			EditorGUILayout.EndHorizontal();
#endif

#if VERBOSE_FORMAT
			EditorGUILayout.LabelField("xox interactive Home Page", EditorStyles.boldLabel);
			EditorGUILayout.TextField(AssetManager.companySiteURL);
			if (GUILayout.Button("Go to xox interactive")) {
			Application.OpenURL(AssetManager.companySiteURL);	
			}
#endif

			// --------------------------------------------------------------------------
			// --- Tutorials
			// --------------------------------------------------------------------------
#if CONDENSED_FORMAT
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.TextField("Tutorials", AssetManager.tutorialsURL);
			if (GUILayout.Button(goContent, GUILayout.Width(goWidth))) {
				Application.OpenURL(AssetManager.tutorialsURL);	
			}
			EditorGUILayout.EndHorizontal();
#endif

#if VERBOSE_FORMAT
			EditorGUILayout.LabelField("Tutorials Page", EditorStyles.boldLabel);
			EditorGUILayout.TextField(AssetManager.tutorialsURL);
			if (GUILayout.Button("Go to the Tutorials")) {
			Application.OpenURL(AssetManager.tutorialsURL);	
			}
#endif

			// --------------------------------------------------------------------------
			// --- Support Forum
			// --------------------------------------------------------------------------

#if CONDENSED_FORMAT
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.TextField("Support Forum", AssetManager.supportForumURL);
			if (GUILayout.Button(goContent, GUILayout.Width(goWidth))) {
				Application.OpenURL(AssetManager.supportForumURL);	
			}
			EditorGUILayout.EndHorizontal();
#endif
			
#if VERBOSE_FORMAT
			EditorGUILayout.LabelField("Support Forum", EditorStyles.boldLabel);
			EditorGUILayout.TextField(AssetManager.supportForumURL);

			if (GUILayout.Button("Go to the Support Forum")) {
				Application.OpenURL(AssetManager.supportForumURL);	
			}
#endif

			// --------------------------------------------------------------------------
			// --- Emails
			// --------------------------------------------------------------------------
#if EMAIL_IN_FIELDS
			EditorGUILayout.LabelField("Email", EditorStyles.boldLabel);
			EditorGUILayout.LabelField("Even though we prefer to use our support forum, " +
			"you can contact us via email as well. " +
			"Please use the email addresses below.", style);
			EditorGUILayout.TextField("Support Email", "support@xoxinteractive.com");
			EditorGUILayout.TextField("Contact Email", "contact@xoxinteractive.com");

#else
			EditorGUILayout.LabelField("Email", EditorStyles.boldLabel);
			EditorGUILayout.LabelField("You can contact us via email as well. " +
			"Please use the email addresses below.\n" +
			"Support: support@xoxinteractive.com\n" +
			"Contact: contact@xoxinteractive.com ", style);
#endif

			// --------------------------------------------------------------------------
			// --- Quick Help
			// --------------------------------------------------------------------------

//			EditorGUILayout.HelpBox("Quick and Simple Help\n\n" +
//				"1. Add 'xDoc Annotation' components to game objects to annotate them.\n\n" +
//				"2. Open the xDoc Window to access xDoc's further functionality: " +
//				"'Window' menu -> xDoc.\n\n" +
//				"You may want to visit the tutorials section; and as well give the manual a chance.", 
//				MessageType.Info);

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Quick and Simple Help", EditorStyles.boldLabel);
			EditorGUILayout.Space();
			EditorGUILayout.LabelField(
				"1. Add 'xDoc Annotation' components to game objects to annotate them.\n\n" +
				"2. Open the xDoc Window to access xDoc's further functionality: " +
				"'Window' menu -> xDoc.\n\n" +
				"You may want to visit the tutorials section; and as well give the manual a chance.",
				EditorStyles.textArea);
		
			// --------------------------------------------------------------------------
			// --- xDoc Window
			// --------------------------------------------------------------------------
#if CONDENSED_FORMAT
			EditorGUILayout.Space();
			if (GUILayout.Button("Open the xDoc Window")) {
				XDocWindow.ShowWindow();
			}
#endif

#if VERBOSE_FORMAT
			EditorGUILayout.LabelField("xDoc Window", EditorStyles.boldLabel);
			if (GUILayout.Button("Open the xDoc Window")) {
			XDocWindow.ShowWindow();
			}
#endif

			// --------------------------------------------------------------------------
			// --- Manual
			// --------------------------------------------------------------------------
#if VERBOSE_FORMAT
			EditorGUILayout.LabelField("xDoc Manual", EditorStyles.boldLabel);
#endif
			if (GUILayout.Button("Open the Manual - A4 Format")) {
				AssetDatabase.OpenAsset(AssetManager.GetXDocManualID());
			}
			EditorGUILayout.LabelField("The button above won't work, " +
			"if you have an error related to the xDoc AssetManager. " +
			"You can find the manual in the following folder: " +
			"'Assets\\Editor Default Resources\\xDoc-FreeReader\\Documentation'.\n" +
			"This folder also contains the manual in Letter format.", style);

			GUILayout.EndScrollView();

		}

	}
}