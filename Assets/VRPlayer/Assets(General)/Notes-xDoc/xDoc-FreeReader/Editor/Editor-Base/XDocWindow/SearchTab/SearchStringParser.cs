using System.Collections.Generic;


namespace xDocEditorBase.Search {

	public class SearchStringParser
	{
		readonly List<string> searchStringList;
		string currentSearchString;
		string unparsedSearchString;
		bool quotedMode;
		bool isMasked;
		bool skipSpaces;

		public SearchStringParser()
		{
			searchStringList = new List<string>();
		}

		void CommitCurrentSeachString()
		{
			if (currentSearchString.Length > 0) {
				searchStringList.Add(currentSearchString);
			}
			currentSearchString = "";
		}

		public void SetSearchString(
			string aUnparsedSearchString
		)
		{
			quotedMode = false;
			skipSpaces = false;
			isMasked = false;

			searchStringList.Clear();
			currentSearchString = "";

			unparsedSearchString = aUnparsedSearchString.ToLower();

			for (int i = 0; i < unparsedSearchString.Length; i++) {
				var c = unparsedSearchString[i];
				if (isMasked) {
					currentSearchString += c;
					isMasked = false;
					continue;
				}
				if (c == ' ') {
					if (skipSpaces) {
						continue;
					}
				} else {
					skipSpaces = false;
				}
				if (quotedMode) {
					ParseQuoted(c);
				} else {
					ParseNormal(c);
				}
			}

			CommitCurrentSeachString();

		}

		public bool IsMatching(
			string stringToSearch
		)
		{
			foreach ( var searchString in searchStringList ) {
				if (!stringToSearch.Contains(searchString)) {
					return false;
				}
			}
			return true;
		}

		void ParseQuoted(
			char c
		)
		{
			switch (c) {
			case '"':
				quotedMode = false;
				break;
			case '\\':
				isMasked = true;
				break;
			default:
				currentSearchString += c;
				break;
			}
		}

		void ParseNormal(
			char c
		)
		{
			switch (c) {
			case '"':
				quotedMode = true;
				break;
			case '\\':
				isMasked = true;
				break;
			case ':':
				currentSearchString += c;
				currentSearchString += ' ';
				skipSpaces = true;
				break;
			case ' ':
				CommitCurrentSeachString();
				break;
			default:
				currentSearchString += c;
				break;
			}
		}
	}
}

