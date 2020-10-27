using UnityEngine;
using xDocBase.Extensions;


// This is our own debugmode for this file for verbose console output; dont mix it with the unity DEBUG define
//#define DEBUGMODE

namespace xDocBase.CustomData
{

	[System.Serializable]
	public class DataField : INamedObject, IIdedObject<QuasiUniqueId>, IDeepCopy<DataField>, IMAsterAttributes<DataField>
	{
		[System.Serializable]
		public enum DataFieldType
		{
			String,
			Date,
			DateSpan,
			Time,
			Bool,
			Int,
			Float,
			Link,
			GameObject,
			Object
		}


		#region Main

		public const string defaultName = "Custom Data";

		public string name;
		public DataFieldType dataFieldType;
		public QuasiUniqueId id;
		public bool showInViewMode;
		public bool showInEditMode;
		public bool showInSceneMode;

		public DataField ()
		{
			name = defaultName;
			dataFieldType = DataFieldType.String;
			showInViewMode = true;
			showInEditMode = true;
			showInSceneMode = false;

			InitValueHolders ();
		}

		#endregion


		#region value holders

		public string stringValue;

		public DataFieldTypeDate dateValue;
		public DataFieldTypeDateSpan dateSpanValue;
		public DataFieldTypeTime timeValue;
		public DataFieldTypeLink linkValue;

		public bool boolValue;
		public int intValue;
		public float floatValue;

		public GameObject gameObjectValue;
		public Object	objectValue;

		void InitValueHolders ()
		{
			// value types
			stringValue = "";
			boolValue = false;
			intValue = 0;
			floatValue = 0f;

			// reference types
			gameObjectValue = null;
			objectValue = null;

			// object types
			dateValue = new DataFieldTypeDate ();
			dateSpanValue = new DataFieldTypeDateSpan ();
			timeValue = new DataFieldTypeTime ();
			linkValue = new DataFieldTypeLink ();

		}

		public string GetNameValuePairAsString ()
		{
			return name + ": " + GetValueAsString ();
		}

		public string GetValueAsString ()
		{
			switch (dataFieldType) {
			case DataFieldType.String:
				if (stringValue == null)
					return "<empty>";
				if (stringValue.Equals (string.Empty))
					return "<empty>";
				return stringValue;
			case DataFieldType.Date:
				return dateValue.ToString ();
			case DataFieldType.DateSpan:
				return dateSpanValue.ToString ();
			case DataFieldType.Time:
				return timeValue.ToString ();
			case DataFieldType.Bool:
				return boolValue.ToString ();
			case DataFieldType.Int:
				return intValue.ToString ();
			case DataFieldType.Float:
				return floatValue.ToString ();
			case DataFieldType.Link:
				return linkValue.ToString ();
			case DataFieldType.GameObject:
				if (gameObjectValue == null)
					return "<null>";
				return gameObjectValue.name;
			case DataFieldType.Object:
				if (gameObjectValue == null)
					return "<null>";
				return objectValue.name;
			default:
				return "Custom Data Type not implemented!";
			}
		}


		#endregion


		#region IDeepCopy implementation

		public void CopyFrom (
			DataField src
		)
		{
			CopyMasterAttributesFrom (src);
			CopyValueAttributesFrom (src);
		}

		// old implementation
		//	public bool IsCopyFrom(
		//		DataField src
		//	)
		//	{
		//		return
		//			name == src.name &&
		//			dataFieldType == src.dataFieldType &&
		//			showInViewMode == src.showInViewMode &&
		//			showInEditMode == src.showInEditMode &&
		//			showInSceneMode == src.showInSceneMode &&
		//			id == src.id &&
		//			stringValue == src.stringValue &&
		//			dateValue == src.dateValue &&
		//			timeValue == src.timeValue &&
		//			boolValue == src.boolValue &&
		//			intValue == src.intValue &&
		//			floatValue == src.floatValue &&
		//			linkValue == src.linkValue &&
		//			gameObjectValue == src.gameObjectValue &&
		//			objectValue == src.objectValue;
		//	}

		public bool IsCopyFrom (
			DataField src
		)
		{
			if (name != src.name) {
#if DEBUGMODE
			Debug.Log(name + ": name");
#endif
				return false;
			}
			if (id != src.id) {
#if DEBUGMODE
			Debug.Log(name + ": id");
#endif
				return false;
			}
			if (dataFieldType != src.dataFieldType) {
#if DEBUGMODE
			Debug.Log(name + ": dataFieldType");
#endif
				return false;
			}
			if (showInViewMode != src.showInViewMode) {
#if DEBUGMODE
			Debug.Log(name + ": showInViewMode");
#endif
				return false;
			}
			if (showInEditMode != src.showInEditMode) {
#if DEBUGMODE
			Debug.Log(name + ": showInEditMode");
#endif
				return false;
			}
			if (showInSceneMode != src.showInSceneMode) {
#if DEBUGMODE
			Debug.Log(name + ": showInSceneMode");
#endif
				return false;
			}
			if (stringValue != src.stringValue) {
#if DEBUGMODE
			Debug.Log(name + ": stringValue");
#endif
				return false;
			}
			if (boolValue != src.boolValue) {
#if DEBUGMODE
			Debug.Log(name + ": boolValue");
#endif
				return false;
			}
			if (intValue != src.intValue) {
#if DEBUGMODE
			Debug.Log(name + ": intValue");
#endif
				return false;
			}
			if (floatValue != src.floatValue) {
#if DEBUGMODE
			Debug.Log(name + ": floatValue");
#endif
				return false;
			}
			if (gameObjectValue != src.gameObjectValue) {
#if DEBUGMODE
			Debug.Log(name + ": gameObjectValue");
#endif
				return false;
			}
			if (objectValue != src.objectValue) {
#if DEBUGMODE
			Debug.Log(name + ": objectValue");
#endif
				return false;
			}

			if (!dateValue.IsSameValue (src.dateValue)) {
#if DEBUGMODE
			Debug.Log(name + ": dateValue");
#endif
				return false;
			}
			if (!dateSpanValue.IsSameValue (src.dateSpanValue)) {
#if DEBUGMODE
			Debug.Log(name + ": dateSpanValue");
#endif
				return false;
			}
			if (!timeValue.IsSameValue (src.timeValue)) {
#if DEBUGMODE
			Debug.Log(name + ": timeValue");
#endif
				return false;
			}
			if (!linkValue.IsSameValue (src.linkValue)) {
#if DEBUGMODE
			Debug.Log(name + ": linkValue");
#endif
				return false;
			}

			return true;


		}

		#endregion


		#region IMAsterAttributes implementation

		/// <summary>
		/// Determines whether this instance has same master attributes the specified other.
		/// 
		/// If master attributes differ they have to copied over from the original to the 
		/// non-master. The non-master will still keep the value attributes.
		/// </summary>
		/// <returns><c>true</c> if this instance has same master attributes the specified other; otherwise, <c>false</c>.</returns>
		/// <param name="other">Other.</param>
		public bool HasSameMasterAttributes (
			DataField other
		)
		{
			if (id != other.id) {
				return false;
			}
			if (name != other.name) {
				return false;
			}
			if (dataFieldType != other.dataFieldType) {
				return false;
			}
			if (showInViewMode != other.showInViewMode) {
				return false;
			}
			if (showInEditMode != other.showInEditMode) {
				return false;
			}
			if (showInSceneMode != other.showInSceneMode) {
				return false;
			}
			return true;
		}

		public void CopyMasterAttributesFrom (
			DataField src
		)
		{
			name = src.name;
			id = src.id;
			dataFieldType = src.dataFieldType;

			showInViewMode = src.showInViewMode;
			showInEditMode = src.showInEditMode;
			showInSceneMode = src.showInSceneMode;
		}

		public void CopyValueAttributesFrom (
			DataField src
		)
		{
			// value types
			stringValue = src.stringValue;
			boolValue = src.boolValue;
			intValue = src.intValue;
			floatValue = src.floatValue;

			// reference types
			gameObjectValue = src.gameObjectValue;
			objectValue = src.objectValue;

			// object types
			dateValue = new DataFieldTypeDate (src.dateValue);
			dateSpanValue = new DataFieldTypeDateSpan (src.dateSpanValue);
			timeValue = new DataFieldTypeTime (src.timeValue);
			linkValue = new DataFieldTypeLink (src.linkValue);
		}

		#endregion


		#region INamedObject implementation

		public string Name {
			get { return name; }
			set { name = value; }
		}

		#endregion


		#region IIdedObject implementation

		public QuasiUniqueId Id {
			get { return id; }
			set { id = value; }
		}

		#endregion

	}
}
