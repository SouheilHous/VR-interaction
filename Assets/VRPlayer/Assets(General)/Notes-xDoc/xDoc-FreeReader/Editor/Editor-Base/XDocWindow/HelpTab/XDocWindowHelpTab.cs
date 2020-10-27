using UnityEditor;
using UnityEngine;
using xDocBase.AssetManagement;
using xDocBase.UI;
using xDocEditorBase.UI;


namespace xDocEditorBase.Windows
{

	public class XDocWindowHelpTab : XoxEditorGUI.SinglePanelLayout
	{
		Vector2 scrollPosHelp;
		Vector2 scrollPosHelp2;

		public override string name { get { return "Help"; } }

		readonly GUIContent manualButtonContentA4;
		readonly GUIContent manualButtonContentLetter;
		readonly GUIContent supportForumButtonContent;
		readonly GUIContent tutorialsButtonContent;
		readonly GUIContent xoxInteractiveButtonContent;
		readonly GUIContent assetStoreButtonContent;

		public XDocWindowHelpTab (
			XoxEditorGUI.EditorWindowX parent
		)
			: base (
				parent
			)
		{
			manualButtonContentA4 = new GUIContent (AssetManager.settings.xDocManualA4Button);
			manualButtonContentLetter = new GUIContent (AssetManager.settings.xDocManualLetterButton);
			supportForumButtonContent = new GUIContent (AssetManager.settings.xDocSupportForumButton);
			tutorialsButtonContent = new GUIContent (AssetManager.settings.xDocTutorialsButton);
			xoxInteractiveButtonContent = new GUIContent (AssetManager.settings.xDocXoxInteractiveButton);
			assetStoreButtonContent = new GUIContent (AssetManager.settings.xDocAssetStoreButton);
		}

		protected override void DrawPanel (
			Rect rect
		)
		{
			using (new GUILayout.AreaScope (rect)) {
			
				using (var sv = new EditorGUILayout.ScrollViewScope (scrollPosHelp))
				using (new StateSaver.BgColor (AssetManager.settings.styleHelpText.bgColor)) {
					scrollPosHelp = sv.scrollPosition; 

					EditorGUILayout.LabelField (
						AssetManager.settings.helpText.text,
						AssetManager.settings.styleHelpText.style
					);

					using (var sv2 = new GUILayout.ScrollViewScope (scrollPosHelp2, GUILayout.ExpandHeight (false))) {
						scrollPosHelp2 = sv2.scrollPosition;
						using (new EditorGUILayout.HorizontalScope (GUILayout.ExpandHeight (false))) {					 
							if (GUILayout.Button (xoxInteractiveButtonContent)) {
								Application.OpenURL (AssetManager.companySiteURL);	
							}
							if (GUILayout.Button (tutorialsButtonContent)) {
								Application.OpenURL (AssetManager.tutorialsURL);	
							}
							if (GUILayout.Button (supportForumButtonContent)) {
								Application.OpenURL (AssetManager.supportForumURL);	
							}
//							using (new GUILayout.VerticalScope()) {					 
							if (GUILayout.Button (manualButtonContentA4)) {
								AssetDatabase.OpenAsset (AssetManager.GetXDocManualID ());
							}
							if (GUILayout.Button (manualButtonContentLetter)) {
								AssetDatabase.OpenAsset (AssetManager.GetXDocManualIDLetter ());
							}
//							}
							if (GUILayout.Button (assetStoreButtonContent)) {
								Application.OpenURL (AssetManager.assetStoreURL);	
							}
						}				 
					}				 
				}
			}
		}

	}

}
