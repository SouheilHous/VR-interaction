using UnityEngine;
using UnityEditor;
using System;

namespace dlobo.Seek
{
	public enum ClickType {
		None, Click, DoubleClick
	}

	public class ListSelectionsInput
	{
		public Func<float> GetDoubleClickMaxTime;
		public Func<float> GetListItemHeight;
		public Func<float> GetListHeight;
		public Func<float> GetListYPosition;
		public Action<float> SetListYPosition;

		public bool CanChangeListPosition = true;

		private ListSelections selections;
		private int lastSelectedIndex;
		private double lastClickTime;

		public ListSelectionsInput(ListSelections selections)
		{
			this.selections = selections;
			lastSelectedIndex = -1;
			lastClickTime = 0;
		}

		public bool ProcessKeyboardInput(Event ev)
		{
			bool didSomething = false;

			if (ev.type == EventType.KeyDown)
			{
				if (ev.keyCode == KeyCode.UpArrow || ev.keyCode == KeyCode.DownArrow
				 || ev.keyCode == KeyCode.PageUp || ev.keyCode == KeyCode.PageDown
				 || ev.keyCode == KeyCode.Home || ev.keyCode == KeyCode.End)
				 {
					#if UNITY_EDITOR_OSX
					bool isControlPressed = Event.current.command;
					#else
					bool isControlPressed = Event.current.control;
					#endif

					bool isShiftPressed = Event.current.shift;

					if (lastSelectedIndex == -1) {
						selections.Select(0, false, false);
					} else if (ev.keyCode == KeyCode.UpArrow) {
						selections.SelectPrevious(isControlPressed, isShiftPressed);
					} else if (ev.keyCode == KeyCode.DownArrow) {
						selections.SelectNext(isControlPressed, isShiftPressed);
					} else if (ev.keyCode == KeyCode.PageUp) {
						int nItems = (int) (GetListHeight() / GetListItemHeight());
						for (int i = 0; i < nItems; i++) {
							selections.SelectPrevious(isControlPressed, isShiftPressed);
						}
					} else if (ev.keyCode == KeyCode.PageDown) {
						int nItems = (int) (GetListHeight() / GetListItemHeight());
						for (int i = 0; i < nItems; i++) {
							selections.SelectNext(isControlPressed, isShiftPressed);
						}
					} else if (ev.keyCode == KeyCode.Home) {
						selections.Select(0, false, isShiftPressed, false);
					} else if (ev.keyCode == KeyCode.End) {
						selections.Select(selections.GetNumberOfSelectables()-1, false, isShiftPressed, false);
					}

					int index = selections.GetSingleSelectionIndex();
					if (index != -1) {
						lastSelectedIndex = index;
					} else {
						lastSelectedIndex = selections.GetLastSelected();
					}
					setListPositionToShowItem(lastSelectedIndex);
					didSomething = true;
				}
			}

			// see https://docs.unity3d.com/ScriptReference/Event-commandName.html
			if (ev.type == EventType.ValidateCommand) {
				if (ev.commandName == "SelectAll") {
					ev.Use();
				}
			} else if (ev.type == EventType.ExecuteCommand) {
				if (ev.commandName == "SelectAll") {
					selections.SelectAll();
					didSomething = true;
				}
			}

			return didSomething;
		}

		public ClickType Click(int index, bool doSelect = true)
		{
			if (doSelect) {
				Select(index);
			}

			ClickType clickType;

			if (index == lastSelectedIndex && lastClickTime + GetDoubleClickMaxTime() > EditorApplication.timeSinceStartup) {
				clickType = ClickType.DoubleClick;
			} else {
				clickType = ClickType.Click;
				lastClickTime = EditorApplication.timeSinceStartup;
			}

			lastSelectedIndex = index;
			if (CanChangeListPosition) {
				setListPositionToShowItem(lastSelectedIndex);
			}

			return clickType;
		}

		public void Select(int index)
		{
			#if UNITY_EDITOR_OSX
			bool isControlPressed = Event.current.command;
			#else
			bool isControlPressed = Event.current.control;
			#endif

			bool isShiftPressed = Event.current.shift;

			selections.Select(index, isControlPressed, isShiftPressed);
		}

		public bool AreModifiersPressed()
		{
			#if UNITY_EDITOR_OSX
			bool isControlPressed = Event.current.command;
			#else
			bool isControlPressed = Event.current.control;
			#endif

			bool isShiftPressed = Event.current.shift;

			return isControlPressed || isShiftPressed;
		}


		private void setListPositionToShowItem(int index)
		{
			float listYPos = GetListYPosition();
			float listHeight = GetListHeight();
			float itemHeight = GetListItemHeight();

			float itemYPos = index * itemHeight;
			if (itemYPos < listYPos + 2*itemHeight) {
				SetListYPosition(itemYPos - 2*itemHeight);
			} else if (itemYPos > listYPos + listHeight - 3*itemHeight) {
				SetListYPosition(itemYPos - listHeight + 3*itemHeight);
			}
		}
	}
}
