/* UltimateRadialMenuWindow.cs */
/* Written by Kaz Crowe */
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class UltimateRadialMenuWindow : EditorWindow
{
	static string version = "v1.0.4";// ALWAYS UPDATE
	static int importantChanges = 0;
	static string menuTitle = "Ultimate Radial Menu";
	static Texture2D introVideo;
	static WWW img;

	GUILayoutOption[] docSize = new GUILayoutOption[] { GUILayout.Width( 405 ), GUILayout.Height( 290 ) };
	class PageInformation
	{
		public string pageName = "";
		public Vector2 scrollPosition = Vector2.zero;
		public delegate void TargetMethod ();
		public TargetMethod targetMethod;
	}
	static PageInformation mainMenu = new PageInformation() { pageName = "Ultimate Radial Menu" };
	static PageInformation majorUpdate = new PageInformation() { pageName = "Major Update" };
	static PageInformation thankYou = new PageInformation() { pageName = "Thank You!" };
	static PageInformation versionHistory = new PageInformation() { pageName = "Version History" };
	static List<PageInformation> pageHistory = new List<PageInformation>();
	static PageInformation currentPage = new PageInformation();


	[MenuItem( "Window/Tank and Healer Studio/Ultimate Radial Menu", false, 10 )]
	static void InitializeWindow ()
	{
		EditorWindow window = GetWindow<UltimateRadialMenuWindow>( true, "Tank and Healer Studio Asset Window", true );
		window.maxSize = new Vector2( 570, 400 );
		window.minSize = new Vector2( 570, 400 );
		window.Show();
	}

	void OnEnable ()
	{
		if( !pageHistory.Contains( mainMenu ) )
			pageHistory.Insert( 0, mainMenu );

		mainMenu.targetMethod = MainPage;
		majorUpdate.targetMethod = MajorUpdatePage;
		thankYou.targetMethod = ThankYouPage;
		versionHistory.targetMethod = VersionHistoryPage;

		if( pageHistory.Count == 1 )
			currentPage = mainMenu;
	}

	static void NavigateBack ()
	{
		pageHistory.RemoveAt( pageHistory.Count - 1 );
		menuTitle = pageHistory[ pageHistory.Count - 1 ].pageName;
		currentPage = pageHistory[ pageHistory.Count - 1 ];

		GUI.SetNextControlName( "DummyField" );
		GUI.FocusControl( "DummyField" );
	}

	static void NavigateForward ( PageInformation menu )
	{
		if( pageHistory.Contains( menu ) )
			return;

		pageHistory.Add( menu );
		menuTitle = menu.pageName;
		currentPage = menu;

		GUI.SetNextControlName( "DummyField" );
		GUI.FocusControl( "DummyField" );
	}

	void OnGUI ()
	{
		GUIStyle background = new GUIStyle( GUI.skin.box );
		background.normal.background = ( Texture2D )Resources.Load( "UltimateRadialMenuWindowBackground" );
		background.margin = new RectOffset();
		background.overflow = new RectOffset();
		background.padding = new RectOffset();
		background.border = new RectOffset();
		Texture2D tahLogo = ( Texture2D )Resources.Load( "TankAndHealerLogo" );

		EditorGUILayout.BeginVertical( background );
		{
			GUILayout.Space( 15 );
			EditorGUILayout.BeginHorizontal();
			{
				GUILayout.Space( 15 );

				Color origColor = GUI.backgroundColor;
				GUI.backgroundColor = Color.clear;
				if( GUILayout.Button( new GUIContent( tahLogo, "Tank & Healer Studio Website" ), GUILayout.Width( 75 ), GUILayout.Height( 75 ) ) )
				{
					Debug.Log( "Ultimate Radial Menu Window\nOpening Tank & Healer Studio Website" );
					Application.OpenURL( "https://www.tankandhealerstudio.com/" );
				}
				EditorGUIUtility.AddCursorRect( GUILayoutUtility.GetLastRect(), MouseCursor.Link );
				GUI.backgroundColor = origColor;

				GUILayout.Space( 30 );
				EditorGUILayout.BeginVertical();
				{
					GUIStyle titleText = new GUIStyle( GUI.skin.label )
					{
						fontSize = 14,
						fontStyle = FontStyle.Bold,
						alignment = TextAnchor.MiddleLeft,
					};
					EditorGUILayout.LabelField( menuTitle, titleText, GUILayout.Height( 30 ) );

					if( currentPage.targetMethod != null )
						currentPage.targetMethod();
				}
				EditorGUILayout.EndVertical();
				GUILayout.Space( 30 );
			}
			EditorGUILayout.EndHorizontal();
			GUILayout.Space( 15 );
			GUIStyle versionNumber = new GUIStyle( GUI.skin.label )
			{
				alignment = TextAnchor.MiddleRight,
			};
			EditorGUILayout.BeginHorizontal();
			{
				GUILayout.FlexibleSpace();
				EditorGUILayout.LabelField( "Ultimate Radial Menu " + version, versionNumber );
				var rect = GUILayoutUtility.GetLastRect();
				EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );
				if( Event.current.type == EventType.MouseDown && rect.Contains( Event.current.mousePosition ) )
					NavigateForward( versionHistory );
			}
			GUILayout.Space( 5 );
			EditorGUILayout.EndHorizontal();
			GUILayout.Space( 5 );
		}
		EditorGUILayout.EndVertical();

		Repaint ();
	}

	void MainPage ()
	{
		StartPage( mainMenu );

		EditorGUILayout.LabelField( "We hope that you are enjoying using the Ultimate Radial Menu in your project!", EditorStyles.wordWrappedLabel );
		EditorGUILayout.Space();
		EditorGUILayout.LabelField( "As with any package, you may be having some trouble understanding how to get the Ultimate Radial Menu working in your project. If so, have no fear, Tank & Healer Studio is here! Here is a few things that can help you get started:", EditorStyles.wordWrappedLabel );

		EditorGUILayout.Space();

		GUIStyle style = new GUIStyle( GUI.skin.label ) { richText = true, wordWrap = true };
		EditorGUILayout.LabelField( "  • Check out the <b><color=blue>Online Documentation</color></b>!", style );
		var rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );
		if( Event.current.type == EventType.MouseDown && rect.Contains( Event.current.mousePosition ) )
			OpenOnlineDocumentation();

		EditorGUILayout.Space();

		EditorGUILayout.LabelField( "  • Watch our <b><color=blue>YouTube Tutorials</color></b> on the Ultimate Radial Menu!", style );
		rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );
		if( Event.current.type == EventType.MouseDown && rect.Contains( Event.current.mousePosition ) )
			OpenYouTubeTutorials();

		EditorGUILayout.Space();

		EditorGUILayout.LabelField( "  • <b><color=blue>Contact Us</color></b> directly with your issue! We'll try to help you out as much as we can.", style );
		rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );
		if( Event.current.type == EventType.MouseDown && rect.Contains( Event.current.mousePosition ) )
			OpenContactForm();

		EditorGUILayout.Space();

		EditorGUILayout.LabelField( "Now you have the tools you need to get the Ultimate Radial Menu working in your project. Now get out there and make your awesome game!", EditorStyles.wordWrappedLabel );

		EditorGUILayout.Space();

		EditorGUILayout.LabelField( "Happy Game Making,\n" + Indent + "Tank & Healer Studio", EditorStyles.wordWrappedLabel );

		EndPage();

		GUILayout.FlexibleSpace();
		EditorGUILayout.BeginHorizontal();
		if( GUILayout.Button( "Online Documentation", GUILayout.Width( 200 ) ) )
			OpenOnlineDocumentation();
		if( GUILayout.Button( "Contact Us", GUILayout.Width( 200 ) ) )
			OpenContactForm();
		EditorGUILayout.EndHorizontal();
	}

	void ThankYouPage ()
	{
		StartPage( thankYou );
		GUIStyle style = new GUIStyle( GUI.skin.label ) { richText = true, wordWrap = true };
		EditorGUILayout.LabelField( "Thank you for downloading the <b>Ultimate Radial Menu</b> UnityPackage from the Unity Asset Store!", style );

		EditorGUILayout.Space();

		EditorGUILayout.LabelField( "We know you are ready and eager to get the Ultimate Radial Menu implemented into your project as fast as you can.", style );

		EditorGUILayout.Space();

		EditorGUILayout.LabelField( "Please take just a few minutes to watch the <b>Ultimate Radial Menu Introduction</b>.", style );

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();

		if( introVideo != null )
		{
			Color origColor = GUI.backgroundColor;
			GUI.backgroundColor = Color.clear;

			if( GUILayout.Button( introVideo, GUILayout.Width( 250 ), GUILayout.Height( 140 ) ) )
				Application.OpenURL( "https://www.youtube.com/watch?v=n9Mt9vCfmf4&list=PL7crd9xMJ9TltHWPVuj-GLs9ZBd4tYMmu" );

			EditorGUIUtility.AddCursorRect( GUILayoutUtility.GetLastRect(), MouseCursor.Link );

			GUI.backgroundColor = origColor;
		}
		else
		{
			if( GUILayout.Button( "Intro Video" ) )
				Application.OpenURL( "https://youtu.be/n9Mt9vCfmf4" );
		}

		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.LabelField( "You can return to this window at any time:\n" + Indent + "<i>Window / Tank and Healer Studio / Ultimate Radial Menu</i>.", style );

		EndPage();

		GUILayout.FlexibleSpace();
		DisplayBackToMainMenuButton();
	}

	void VersionHistoryPage ()
	{
		StartPage( versionHistory );
		GUIStyle style = new GUIStyle( GUI.skin.label ) { richText = true, wordWrap = true };
		EditorGUILayout.LabelField( "<b>Version 1.0.4</b>", style );
		EditorGUILayout.LabelField( "  • Updated the Ultimate Radial Menu editor to display a few more functions in the generated example code.", style );
		EditorGUILayout.LabelField( "  • Modified the Input Manager script to have virtual functions, allowing custom input scripts to be implemented.", style );
		EditorGUILayout.LabelField( "  • Added new public function: <b>SetPosition</b> to allow for easily changing the position of the radial menu on the screen.", style );
		EditorGUILayout.Space();
		EditorGUILayout.LabelField( "<b>Version 1.0.3</b>", style );
		EditorGUILayout.LabelField( "  • Fixed the Ultimate Radial Menu Editor to display the correct script reference code.", style );
		EditorGUILayout.LabelField( "  • Overall bug fixes to the scripts.", style );
		EditorGUILayout.Space();
		EditorGUILayout.LabelField( "<b>Version 1.0.2</b>", style );
		EditorGUILayout.LabelField( "  • Modified Input Manager script to handle all of the input for the radial menu. It is also now placed on the EventSystem in the scene.", style );
		EditorGUILayout.LabelField( "  • Added a button to the Ultimate Radial Menu inspector to select the input manager.", style );
		EditorGUILayout.Space();
		EditorGUILayout.LabelField( "<b>Version 1.0.1</b>", style );
		EditorGUILayout.LabelField( "  • Modified Input Manager script to allow for no key to be used for enabling and disabling the menu.", style );
		EditorGUILayout.Space();
		EditorGUILayout.LabelField( "<b>Version 1.0.0</b>", style );
		EditorGUILayout.LabelField( "  • Initial Release", style );
		EndPage();

		GUILayout.FlexibleSpace();
		DisplayBackToMainMenuButton();
	}

	void MajorUpdatePage ()
	{
		StartPage( majorUpdate );
		GUIStyle style = new GUIStyle( GUI.skin.label ) { richText = true, wordWrap = true };
		EditorGUILayout.LabelField( "There have not been any major updates yet.", style );
		EndPage();

		GUILayout.FlexibleSpace();
		DisplayBackToMainMenuButton();
	}

	void DisplayBackToMainMenuButton ()
	{
		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if( GUILayout.Button( "Main Page", GUILayout.Width( 200 ) ) )
			NavigateBack();
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();
	}

	void StartPage ( PageInformation pageInfo )
	{
		pageInfo.scrollPosition = EditorGUILayout.BeginScrollView( pageInfo.scrollPosition, false, false, docSize );
	}

	void EndPage ()
	{
		EditorGUILayout.EndScrollView();
	}

	void OpenOnlineDocumentation ()
	{
		Debug.Log( "Ultimate Radial Menu Window\nOpening Online Documentation" );
		Application.OpenURL( "https://www.tankandhealerstudio.com/ultimate-radial-menu_documentation.html" );
	}

	void OpenContactForm ()
	{
		Debug.Log( "Ultimate Radial Menu Window\nOpening Online Contact Form" );
		Application.OpenURL( "https://www.tankandhealerstudio.com/contact-us.html" );
	}

	void OpenYouTubeTutorials ()
	{
		Debug.Log( "Ultimate Radial Menu Window\nOpening YouTube Tutorials" );
		Application.OpenURL( "https://www.youtube.com/playlist?list=PL7crd9xMJ9TltHWPVuj-GLs9ZBd4tYMmu" );
	}

	string Indent
	{
		get
		{
			return "    ";
		}
	}

	[InitializeOnLoad]
	class UltimateRadialMenuInitialLoad
	{
		static UltimateRadialMenuInitialLoad ()
		{
			// If this is the first time that the user has downloaded the Ultimate Status Bar...
			if( !EditorPrefs.HasKey( "UltimateRadialMenuVersion" ) )
			{
				// Set the current menu to the thank you page.
				NavigateForward( thankYou );

				// Set the version to current so they won't see these version changes.
				EditorPrefs.SetInt( "UltimateRadialMenuVersion", importantChanges );

				// Load intro video image.
				img = new WWW( "https://www.tankandhealerstudio.com/uploads/7/7/4/9/77490188/urm-introductionsmall_orig.png" );

				EditorApplication.update += WaitForDownload;
			}
			else if( EditorPrefs.GetInt( "UltimateRadialMenuVersion" ) < importantChanges )
			{
				// Set the current menu to the version changes page.
				NavigateForward( majorUpdate );

				// Set the version to current so they won't see this page again.
				EditorPrefs.SetInt( "UltimateRadialMenuVersion", importantChanges );

				EditorApplication.update += WaitForCompile;
			}
		}

		static void WaitForDownload ()
		{
			if( !img.isDone || EditorApplication.isCompiling )
				return;

			introVideo = img.texture;

			EditorApplication.update -= WaitForDownload;

			InitializeWindow();
		}

		static void WaitForCompile ()
		{
			if( EditorApplication.isCompiling )
				return;

			EditorApplication.update -= WaitForCompile;

			InitializeWindow();
		}
	}
}