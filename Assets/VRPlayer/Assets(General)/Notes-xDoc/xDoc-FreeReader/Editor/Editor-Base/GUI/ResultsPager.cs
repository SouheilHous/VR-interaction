using UnityEditor;
using UnityEngine;
using xDocBase.AssetManagement;
using xDocBase.UI;


namespace xDocEditorBase.Search
{

	// #################################################################################################################
	// ### ResultsPagerBase
	// #################################################################################################################

	abstract public class ResultsPagerBase
	{
		#region Main

		protected ResultsPagerBase (
			int currentMinDelta,
			int currentMaxDelta
		)
		{
			this.currentMinDelta = currentMinDelta;
			this.currentMaxDelta = currentMaxDelta;
			Initialize (0, 10);
		}

		public void Initialize (
			int totalNumberOfResults,
			int resultsPerPage
		)
		{
			this.totalNumberOfResults = totalNumberOfResults;

			this.resultsPerPage = resultsPerPage;

			maxPage = totalNumberOfResults / resultsPerPage;
			if (totalNumberOfResults % resultsPerPage != 0) {
				maxPage++;
			}
			maxPage = Mathf.Max (minPage, maxPage);
			currentPage = 1;
		}

		#endregion


		#region results

		protected int totalNumberOfResults;
		protected int resultsPerPage;

		#endregion


		#region pages

		/// The current page. Page Numbers start at 1 !!!!!!!	/// 
		protected const int minPage = 1;
		protected int maxPage;

		protected int _currentPage;
		protected int _startElement;
		protected int _endElement;
		protected int _startElementHR;
		protected int _numElements;

		protected int currentMinDelta;
		protected int currentMaxDelta;
		protected int _currentMinPage;
		protected int _currentMaxPage;

		public int currentPage {
			get { return _currentPage; }
			protected set { 
				_currentPage = Mathf.Clamp (value, minPage, maxPage); 
				// calculate elements shown in the current page 
				_startElement = (_currentPage - 1) * resultsPerPage;
				_endElement = Mathf.Min (_currentPage * resultsPerPage, totalNumberOfResults);
				_startElementHR = _startElement + 1;
				_numElements = _endElement - _startElement;
				// calculate pages to be shown
				_currentMinPage = _currentPage - currentMinDelta;
				_currentMaxPage = _currentPage + currentMaxDelta;
				if (_currentMinPage < minPage) {
					_currentMaxPage += minPage - _currentMinPage;
					_currentMinPage = minPage;
				}
				if (_currentMaxPage > maxPage) {
					_currentMinPage -= _currentMaxPage - maxPage;
					_currentMaxPage = maxPage;
				}
				_currentMinPage = Mathf.Max (_currentMinPage, minPage);
			}
		}

		public int startElement { get { return _startElement; } }

		public int endElement { get { return _endElement; } }

		public int numElements { get { return _numElements; } }

		public int startElementHumanReadable { get { return _startElementHR; } }

		public int endElementHumanReadable { get { return _endElement; } }

		protected bool hasPreviousPage { get { return currentPage != minPage; } }

		protected bool hasNextPage { get { return currentPage != maxPage; } }

		protected int currentMinPage { get { return _currentMinPage; } }

		protected int currentMaxPage { get { return _currentMaxPage; } }

		#endregion
	}

	// #################################################################################################################
	// ### ResultsPager
	// #################################################################################################################

	public class ResultsPager : ResultsPagerBase
	{
		#region style

		readonly GUIStyle scopeStyle;
		readonly GUIStyle pagerStyle;
		readonly Color textColor;
		const int spacer = 20;


		GUIContent contentXD;
		GUIContent contentPrevious;
		GUIContent contentCLE;
		GUIContent contentNext;

		#endregion


		#region main

		public ResultsPager ()
			: base (
				4,
				5
			)
		{
			scopeStyle = AssetManager.settings.styleSearchItemParent.style;
			pagerStyle = AssetManager.settings.styleSearchPager.style;
			textColor = AssetManager.settings.styleSearchOverviewLine.style.normal.textColor;
			contentXD = new GUIContent ("", AssetManager.settings.pagerXD);
			contentPrevious = new GUIContent ("Previous", AssetManager.settings.pagerPrevious);
			contentCLE = new GUIContent ("", AssetManager.settings.pagerCLE);
			contentNext = new GUIContent ("Next", AssetManager.settings.pagerNext);
		}

		/// <summary>
		/// Draw this instance.
		/// </summary>
		/// <returns>
		/// <c>true</c> if a new page has been selected - consider to scroll the page to the top; otherwise, 
		/// <c>false</c>.
		/// </returns>
		public bool Draw ()
		{
			GUILayout.Space (spacer * 2);

			// Draw a separator line of 1 pixel height
			using (new StateSaver.GuiColor (textColor)) {
				var r = EditorGUILayout.GetControlRect (false, 1, scopeStyle);
				r.xMin += scopeStyle.padding.left;
				r.xMax -= scopeStyle.padding.right;
				GUI.DrawTexture (r, AssetManager.settings.whitePixel, ScaleMode.StretchToFill);
			}

			GUILayout.Space (spacer);

			var contentRedO = new GUIContent (currentPage.ToString (), AssetManager.settings.pagerRedO);

			using (new EditorGUILayout.HorizontalScope (scopeStyle)) {
				GUILayout.Space (spacer);
				if (hasPreviousPage) {
					if (GUILayout.Button (contentPrevious, pagerStyle)) {
						currentPage--;
						return true;
					} 
					EditorGUIUtility.AddCursorRect (GUILayoutUtility.GetLastRect (), MouseCursor.Link);
				} else {
					GUILayout.Box (contentXD, pagerStyle);
				}

				for (int i = currentMinPage; i < currentPage; i++) {
					if (DrawJumpButton (i)) {
						return true;
					}
					EditorGUIUtility.AddCursorRect (GUILayoutUtility.GetLastRect (), MouseCursor.Link);
				}

				using (new StyleSaver.NormalTextColor (pagerStyle, Color.black)) {
					GUILayout.Box (contentRedO, pagerStyle);
				}

				for (int i = currentPage + 1; i <= currentMaxPage; i++) {
					if (DrawJumpButton (i)) {
						return true;
					}
					EditorGUIUtility.AddCursorRect (GUILayoutUtility.GetLastRect (), MouseCursor.Link);
				}
				
				if (hasNextPage) {
					if (GUILayout.Button (contentNext, pagerStyle)) {
						currentPage++;
						return true;
					}
					EditorGUIUtility.AddCursorRect (GUILayoutUtility.GetLastRect (), MouseCursor.Link);
				} else {
					GUILayout.Box (contentCLE, pagerStyle);
				}
			}

			GUILayout.Space (spacer * 3);
			return false;

		}

		bool DrawJumpButton (
			int page
		)
		{
			var content = new GUIContent (page.ToString (), AssetManager.settings.pagerYellowO);
			if (GUILayout.Button (content, pagerStyle)) {
				currentPage = page;
				return true;
			}
			EditorGUIUtility.AddCursorRect (GUILayoutUtility.GetLastRect (), MouseCursor.Link);
			return false;
		}

		#endregion
	
	}

	// #################################################################################################################
	// ### ResultsPagerSimple
	// #################################################################################################################

	public class ResultsPagerSimple : ResultsPagerBase
	{
		#region style

		readonly GUIStyle pagerStyle;
		const int spacer = 10;
		const int spacer2 = 5;

		GUIContent contentPrevious;
		GUIContent contentNext;

		#endregion


		#region main

		public ResultsPagerSimple ()
			: base (
				9,
				10
			)
		{
			pagerStyle = AssetManager.settings.styleBulkOperationsPager.style;
			contentPrevious = new GUIContent ("<");
			contentNext = new GUIContent (">");
		}

		Vector2 scrollPosition;

		/// <summary>
		/// Draw this instance.
		/// </summary>
		/// <returns>
		/// <c>true</c> if a new page has been selected - consider to scroll the page to the top; otherwise, 
		/// <c>false</c>.
		/// </returns>
		public bool Draw (
			Rect rect,
			string overviewString
		)
		{
			using (new GUILayout.AreaScope (rect)) {
				var contentCurrent = new GUIContent (currentPage.ToString ());

				// overview string
				GUILayout.Space (spacer);
				using (new EditorGUILayout.HorizontalScope ()) {
					GUILayout.Space (spacer);
					GUILayout.Label (overviewString);
				}
				GUILayout.Space (spacer2);

				// pager line
				using (var sv = new GUILayout.ScrollViewScope (scrollPosition)) {
					scrollPosition = sv.scrollPosition;

					using (new EditorGUILayout.HorizontalScope ()) {
						GUILayout.Space (spacer);
						if (hasPreviousPage) {
							if (GUILayout.Button (contentPrevious, pagerStyle)) {
								currentPage--;
								return true;
							} 
							EditorGUIUtility.AddCursorRect (GUILayoutUtility.GetLastRect (), MouseCursor.Link);
						} 

						for (int i = currentMinPage; i < currentPage; i++) {
							if (DrawJumpButton (i)) {
								return true;
							}
							EditorGUIUtility.AddCursorRect (GUILayoutUtility.GetLastRect (), MouseCursor.Link);
						}

						using (new StateSaver.BgColor (AssetManager.settings.styleBulkOperationsPager.bgColor))
						using (new StyleSaver.FontStyle (pagerStyle, FontStyle.Bold)) {
							GUILayout.Box (contentCurrent, pagerStyle);
						}

						for (int i = currentPage + 1; i <= currentMaxPage; i++) {
							if (DrawJumpButton (i)) {
								return true;
							}
							EditorGUIUtility.AddCursorRect (GUILayoutUtility.GetLastRect (), MouseCursor.Link);
						}

						if (hasNextPage) {
							if (GUILayout.Button (contentNext, pagerStyle)) {
								currentPage++;
								return true;
							}
							EditorGUIUtility.AddCursorRect (GUILayoutUtility.GetLastRect (), MouseCursor.Link);
						} 
					}
				}

				return false;
			}

		}

		bool DrawJumpButton (
			int page
		)
		{
			var content = new GUIContent (page.ToString ());
			if (GUILayout.Button (content, pagerStyle)) {
				currentPage = page;
				return true;
			}
			EditorGUIUtility.AddCursorRect (GUILayoutUtility.GetLastRect (), MouseCursor.Link);
			return false;
		}

		#endregion

	}



}

