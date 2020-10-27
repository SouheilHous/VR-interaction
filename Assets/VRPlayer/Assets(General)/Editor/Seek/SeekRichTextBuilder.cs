using UnityEngine;
using System.Collections.Generic;

namespace dlobo.Seek
{
	public class RichTextBuilder
	{
		class RichTextSection
		{
			public RichTextSection Next;
			public int Index;
			public bool IsStart;
			public RichTextWrap Wrap;
		}

		// --------------------------------------------------------
		// --------------------------------------------------------

		abstract class RichTextWrap
		{
			public string Wrap(string text)
			{
				return Open() + text + Close();
			}

			public abstract string Open();
			public abstract string Close();
		}

		class ColorWrap : RichTextWrap
		{
			public string Color;

			public override string Open() { return "<color="+Color+">"; }
			public override string Close() { return "</color>";}
		}

		class BoldWrap : RichTextWrap
		{
			public override string Open() { return "<b>"; }
			public override string Close() { return "</b>";}
		}

		// --------------------------------------------------------
		// --------------------------------------------------------

		private RichTextSection head = new RichTextSection();

		public void AddColorSection(int index, int endIndex, string color)
		{
			var wrap = new ColorWrap { Color = color };
			insertSection(index, endIndex, wrap);
		}

		public void AddBoldSection(int index, int endIndex)
		{
			var wrap = new BoldWrap();
			insertSection(index, endIndex, wrap);
		}

		private void insertSection(int index, int endIndex, RichTextWrap wrap)
		{
			var iniSection = new RichTextSection { Index = index,    IsStart = true,  Wrap = wrap };
			var endSection = new RichTextSection { Index = endIndex, IsStart = false, Wrap = wrap };
			insertSection(iniSection);
			insertSection(endSection, iniSection);
		}

		private void insertSection(RichTextSection section, RichTextSection startSection = null)
		{
			RichTextSection node = (startSection != null ? startSection : head);

			while (node.Next != null)
			{
				if (section.Index <= node.Next.Index) {
					// insert in linked list
					section.Next = node.Next;
					node.Next = section;
					return;
				}
				node = node.Next;
			}

			node.Next = section;
		}

		public void PrintSections()
		{
			RichTextSection node = head.Next;

			while (node != null)
			{
				MonoBehaviour.print(""+node.Wrap.GetType() + " " + node.Index + " " + node.IsStart);
				node = node.Next;
			}
		}

		// utils.AddColorSection(1, 5, "#ff0");
		// utils.AddColorSection(2, 4, "#0f0");
		// utils.AddBoldSection(2, 7);
		// utils.PrintSections();
		// string res = utils.ProcessSections(...);
		// "abcdefghijklmnopqrstuvwxyz"
		// a, <color=#ff0>, <b>, <color=#0f0>, bcdef, </color>, , </b>, </color>, <b>, ghi, </b>, jklmnopqrstuvwxyz

		public string ProcessSections(string text)
		{
			var sb = new System.Text.StringBuilder();
			var openSections = new Stack<RichTextSection>();
			var tempSections = new Stack<RichTextSection>();

			RichTextSection section = head.Next;
			int lastIndex = 0;

			while (section != null)
			{
				sb.Append(text.Substring(lastIndex, section.Index - lastIndex));
				lastIndex = section.Index;

				if (section.IsStart)
				{
					while (openSections.Count > 0)
					{
						var topSection = openSections.Pop();
						sb.Append(topSection.Wrap.Close());
						tempSections.Push(topSection);
					}

					while (tempSections.Count > 0)
					{
						var tmpSection = tempSections.Pop();
						sb.Append(tmpSection.Wrap.Open());
						openSections.Push(tmpSection);
					}

					sb.Append(section.Wrap.Open());
					openSections.Push(section);
				}
				else
				{
					RichTextSection topSection = openSections.Pop();

					while (section.Wrap.GetType() != topSection.Wrap.GetType())
					{
						sb.Append(topSection.Wrap.Close());
						tempSections.Push(topSection);
						topSection = openSections.Pop();
					}

					sb.Append(topSection.Wrap.Close());

					while (tempSections.Count > 0)
					{
						var tmpSection = tempSections.Pop();
						sb.Append(tmpSection.Wrap.Open());
						openSections.Push(tmpSection);
					}
				}

				section = section.Next;
			}

			sb.Append(text.Substring(lastIndex, text.Length - lastIndex));

			return sb.ToString();
		}

		public void ClearSections()
		{
			head.Next = null;
		}
	}
}
