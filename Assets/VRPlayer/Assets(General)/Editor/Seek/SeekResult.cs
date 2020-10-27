using UnityEngine;
using System.Collections.Generic;
using System.IO;

namespace dlobo.Seek
{
	public abstract class Result : Selectable
	{
		public string GUID;
		public string Path;
		public string FullPath;

		public string Representation;
		public long FileSize;
		public Texture Icon;
		public string TypeString;
		public string FileName;

		public bool HasLoadedFileInfo;
		public System.DateTime CreationTime;
		public System.DateTime LastWriteTime;

		// needed for Selectable interface
		public bool IsSelected { get; set; }

		public Rect Rect;
	}

	public class SimpleResult : Result
	{
		public int Index;
		public int Length;
	}

	public class ScatteredResult : Result
	{
		public List<Slice> Slices;
	}

	public class Slice
	{
		public int Index;
		public int EndIndex;

		public Slice() {}

		public Slice(Slice other)
		{
			Index = other.Index;
			EndIndex = other.EndIndex;
		}
	}
}
