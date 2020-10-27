using System.Collections.Generic;

namespace dlobo.Seek
{
	public interface Selectable
	{
		bool IsSelected { get; set; }
	}

	public class ListSelections
	{
		public System.Func<int, Selectable> GetSelectable;
		public System.Func<int> GetNumberOfSelectables;

		private int indexOfLastSelected;
		private int indexOfLastItemThatWasRangeSelected;
		private HashSet<int> selectedIndexes = new HashSet<int>();

		public void ResetSelections()
		{
			indexOfLastSelected = 0;
			indexOfLastItemThatWasRangeSelected = 0;
			selectedIndexes.Clear();
		}

		public int GetLastSelected()
		{
			return indexOfLastSelected;
		}

		public int GetSingleSelectionIndex()
		{
			if (selectedIndexes.Count == 1) {
				IEnumerator<int> enumerator = selectedIndexes.GetEnumerator();
				enumerator.MoveNext();
				return enumerator.Current;
			} else {
				return -1;
			}
		}

		public HashSet<int> GetSelectedIndexes()
		{
			return selectedIndexes;
		}

		public void Select(int index)
		{
			setIsSelected(index, true);
		}

		public void Deselect(int index)
		{
			setIsSelected(index, false);
		}

		public void Select(int index, bool isAdditive, bool isRanged, bool softSelection = false)
		{
			if (index < 0 || index > GetNumberOfSelectables()-1) {
				return;
			}

			if (!isAdditive) {
				DeselectAll();
			}

			if (isRanged) {
				setIsSelectedForAllInRange(indexOfLastSelected, index, true);
				indexOfLastItemThatWasRangeSelected = index;
			} else {
				if (isAdditive) {
					if (!softSelection) {
						toggleIsSelected(index);
					}
				} else {
					setIsSelected(index, true);
				}
				indexOfLastSelected = index;
				indexOfLastItemThatWasRangeSelected = index;
			}
		}

		public void SelectPrevious(bool isAdditive, bool isRanged)
		{
			if (isRanged) {
				if (indexOfLastItemThatWasRangeSelected > 0) {
					Select(indexOfLastItemThatWasRangeSelected-1, isAdditive, isRanged, softSelection: true);
				}
			} else {
				if (indexOfLastSelected > 0) {
					Select(indexOfLastSelected-1, isAdditive, isRanged, softSelection: true);
				}
			}
		}

		public void SelectNext(bool isAdditive, bool isRanged)
		{
			if (isRanged) {
				if (indexOfLastItemThatWasRangeSelected < GetNumberOfSelectables()-1) {
					Select(indexOfLastItemThatWasRangeSelected+1, isAdditive, isRanged, softSelection: true);
				}
			} else {
				if (indexOfLastSelected < GetNumberOfSelectables()-1) {
					Select(indexOfLastSelected+1, isAdditive, isRanged, softSelection: true);
				}
			}
		}

		public void SelectAll()
		{
			setIsSelectedForAllInRange(0, GetNumberOfSelectables()-1, true);
		}

		public void DeselectAll()
		{
			setIsSelectedForAllInRange(0, GetNumberOfSelectables()-1, false);
		}

		public void InvertSelection()
		{
			invertRange(0, GetNumberOfSelectables()-1);
		}

		private void invertRange(int start, int end)
		{
			if (start > end) {
				int tmp = end;
				end = start;
				start = tmp;
			}

			if (start < 0) {
				return;
			}

			for (int i = start; i <= end; i++) {
				toggleIsSelected(i);
			}
		}

		private void setIsSelectedForAllInRange(int start, int end, bool state)
		{
			if (start > end) {
				int tmp = end;
				end = start;
				start = tmp;
			}

			if (start < 0) {
				return;
			}

			for (int i = start; i <= end; i++) {
				setIsSelected(i, state);
			}
		}

		private void setIsSelected(int index, bool state)
		{
			GetSelectable(index).IsSelected = state;

			if (state) {
				selectedIndexes.Add(index);
			} else {
				selectedIndexes.Remove(index);
			}
		}

		private void toggleIsSelected(int index)
		{
			Selectable selectable = GetSelectable(index);
			selectable.IsSelected = !selectable.IsSelected;

			if (selectable.IsSelected) {
				selectedIndexes.Add(index);
			} else {
				selectedIndexes.Remove(index);
			}
		}
	}
}
