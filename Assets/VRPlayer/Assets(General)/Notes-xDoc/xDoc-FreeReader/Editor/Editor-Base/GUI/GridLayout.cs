using UnityEngine;


namespace xDocEditorBase.UI {

	public class GridLayout
	{
		readonly Rect parentRect;
		readonly float sep;
		readonly float pwPlusSep;
		readonly float phPlusSep;

		readonly int[]	colLayoutWidths;
		readonly int[]	colLayoutPositions;
		readonly int	colLayoutTotalWidth;

		/// <summary>
		/// Initializes a new instance of the <see cref="xDocEditor.UI.GridLayout"/> class.
		/// 
		/// This class is a helper class for GUI element placement. It takes a Rect
		/// and slices the Rect into a table of cells (also Rects) cols x rows, separated
		/// by sep.
		/// </summary>
		/// <param name="parentRect">The parent rect, which will be sliced.</param>
		/// <param name="cols">Number of colums</param>
		/// <param name="rows">Number of rows</param>
		/// <param name="sep">Amount of separation btw the cells, same for x and y</param>
		public GridLayout(
			Rect parentRect,
			int cols = 1,
			int rows = 1,
			float sep = 0
		)
		{
			this.parentRect = parentRect;
			this.sep = sep;
	
			float pw = (parentRect.width - (cols - 1) * sep) / cols;
			pwPlusSep = pw + sep;
			float ph = (parentRect.height - (rows - 1) * sep) / rows;
			phPlusSep = ph + sep;
		}

		public GridLayout(
			Rect parentRect,
			int[] colLayoutWidths,
			float sep = 0
		)
		{
			this.colLayoutWidths = colLayoutWidths;
			colLayoutPositions = new int[colLayoutWidths.Length];
			colLayoutTotalWidth += colLayoutWidths[0];
			for (int i = 1, li = 0; i < colLayoutWidths.Length; i++, li++) {
				colLayoutTotalWidth += colLayoutWidths[i];
				colLayoutPositions[i] = colLayoutPositions[li] + colLayoutWidths[li];
			}

			this.parentRect = parentRect;
			this.sep = sep;

			int cols = colLayoutTotalWidth;
			const int rows = 1;
			float pw = (parentRect.width - (cols - 1) * sep) / cols;
			pwPlusSep = pw + sep;
			float ph = (parentRect.height - (rows - 1) * sep) / rows;
			phPlusSep = ph + sep;
		}

		public Rect GetRectSingleRow(
			int colNum
		)
		{
			return GetRect(colLayoutPositions[colNum], 0, colLayoutWidths[colNum]);
		}

		public Rect GetRectSingleRow(
			int colNum,
			int manualWidth
		)
		{
			return GetRect(colLayoutPositions[colNum], 0, manualWidth);
		}


		/// <summary>
		/// Gets a rect - the cell at the indexed position x,y.
		/// If optional parameter width and height are provided, it returns a
		/// Rect, which is the result of the merged cells.
		/// </summary>
		/// <returns>The rect.</returns>
		/// <param name="x">The x index coordinate.</param>
		/// <param name="y">The y index coordinate.</param>
		/// <param name="width">Width in cells</param>
		/// <param name="height">Heigh in cellst.</param>
		public Rect GetRect(
			int x,
			int y,
			int width = 1,
			int height = 1
		)
		{
			Rect r = new Rect(parentRect);
			r.x += x * pwPlusSep;
			r.y += y * phPlusSep;
			r.width = width * pwPlusSep - sep;
			r.height = height * phPlusSep - sep;
			return r;
		}
	}
}
