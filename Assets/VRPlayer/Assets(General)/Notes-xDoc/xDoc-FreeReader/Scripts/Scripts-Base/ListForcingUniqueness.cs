using System;
using System.Collections.Generic;
using System.Linq;


namespace xDocBase.Extensions {

	public interface INamedObject
	{
		string Name { get; set; }
	}

	public interface IIdedObject<T> 
	where T : IId<T>
	{
		T Id { get; set; }
	}

	public interface IId<T> : IEquatable<T>, IDeepCopy<T>
	{
		void InitializeAsNewId();
	}

	public interface IDeepCopy<T>
	{
		void CopyFrom(
			T src
		);
	}

	public interface IMAsterAttributes<T>
	{
		bool HasSameMasterAttributes(
			T other
		);

		void CopyMasterAttributesFrom(
			T src
		);

		void CopyValueAttributesFrom(
			T src
		);
	}


	public static class ListForcingUniqueness
	{
#region INamedObject

		const string defaultUniqueName = "Xox";

		/// <summary>
		/// Takes a list of Named Objects and an index to one of them and then compares
		/// This objects name with all other and makes it unique by adding the prefixer and 
		/// a number. Whitespaces are always stripped of before.
		/// If the element is unique already only the whitespaces are stripped.
		/// </summary>
		/// <param name="list">List.</param>
		/// <param name="index">Index.</param>
		/// <param name = "defaultName">The default name to be assigned to the named object, if current 
		/// name is empty.
		/// </param>
		/// <param name="prefixer">Prefixer.</param>
		/// <typeparam name="T">The type of the list elements. Must implement the
		/// Interface INamedObject
		/// </typeparam>
		public static void UniquifyName<T>(
			this List<T> list,
			int index,
			string defaultName = defaultUniqueName,
			string prefixer = " "
		)
		where T : class, INamedObject
		{
			// _tidyup --- keep it, for propable later debug
//		string dl = "-------------------------------------------------\n";
//		dl += "index: " + index + ", defName: " + defaultUniqueName + ", pref: " + prefixer + "\n";
//		for (int i = 0; i < list.Count; i++) {
//			dl += i + ": " + list[i].Name + "\n";
//		}
//		Debug.Log(dl);

			// Check, if the index is pointing towards a valid entry.
			if (index < 0) {
				return;
			}
			if (index >= list.Count) {
				return;
			}
			
			// first strip whitespaces
			list[index].Name = list[index].Name.Trim();
			// check: empty string?
			if (list[index].Name.Length == 0) {
				list[index].Name = defaultName;
			}
			int v = 2;
			string origName = list[index].Name;
			// now compare with all the other entries in the list
			for (int i = 0; i < list.Count; i++) {
				if (i == index) {
					// skip, bc. that is the element itself
					continue;
				}
				if (list[i].Name.Equals(list[index].Name)) {
					list[index].Name = origName + prefixer + (v++);
//				Debug.Log("Return to 0 with " + list[index].Name);
					// this will initialize to 0, bc the i++ will count up next turn
					i = -1;
				}
			}
		}

		/// <summary>
		/// Returns a unique name by taking the baseName and adding a unique postfix to it.
		/// The baseName is compared against all entries in the list - except if a valid index
		/// is given. This element is not considered in the name uniquification - assuming it
		/// is the element, which has been edited and needs to be re-checked.
		/// </summary>
		/// <param name="list">List.</param>
		/// <param name="baseName">Base name.</param>
		/// <param name = "index">see function description</param>
		/// <param name="prefixer">Prefixer for the postfix.</param>
		/// <typeparam name="T">The 1st type parameter - type of the list elements.</typeparam>
		public static string GetUniqueName<T>(
			this List<T> list,
			string baseName = defaultUniqueName,
			int index = -1,
			string prefixer = " "
		)
		where T : class, INamedObject
		{
			string name;
			if (index >= 0 && index < list.Count) {
				name = list[index].Name;
				name = name.Trim();
				if (name.Length == 0) {
					name = baseName;
					name = name.Trim();
				}
			} else {
				name = baseName;
				name = name.Trim();
			}
			// check: empty string?
			if (name.Length == 0) {
				name = defaultUniqueName;
			}
			int v = 2;
			string origName = name;
			// now compare with all the entries in the list
			for (int i = 0; i < list.Count; i++) {
				if (i == index) {
					continue;
				}
				if (list[i].Name == name) {
					name = origName + prefixer + (v++);
					// start comparing the list elements from the beginning
					i = 0;
					continue;
				}
			}
			return name;
		}

		public static T GetItemWithName<T>(
			this List<T> list, 
			string aName
		)
		where T : class, INamedObject
		{
			if (list == null)
				return null;
			for (int i = 0; i < list.Count; i++) {
				if (list[i].Name == aName)
					return list[i];
			}
			return null;
		}

#endregion


#region IIdedObject

		//	public static void UniquifyId<T>(
		//		this List<T> list,
		//		int index,
		//		int minId = 1
		//	)
		//		where T : class, IIdedObject
		//	{
		//		// Check, if the index is pointing towards a valid entry.
		//		if (index < 0) {
		//			return;
		//		}
		//		if (index >= list.Count) {
		//			return;
		//		}
		//
		//		if (list[index].Id < minId) {
		//			list[index].Id = minId;
		//		}
		//
		//		bool idIsUnique = true;
		//		int maxId = minId;
		//		int origId = list[index].Id;
		//		// now compare with all the other entries in the list
		//		for (int i = 0; i < list.Count; i++) {
		//			if (i == index) {
		//				// skip, bc. that is the element itself
		//				continue;
		//			}
		//			if (list[i].Id == origId) {
		//				idIsUnique = false;
		//			}
		//			maxId = Mathf.Max(maxId, list[i].Id);
		//		}
		//		// if the id is already unique, there is no need to change it
		//		if (!idIsUnique) {
		//			list[index].Id = maxId + 1;
		//		}
		//	}

		//	public static int GetUniqueId<T,M>(
		//		this List<T> list,
		//		int minId = 1
		//	)
		//		where T : class, IIdedObject<M>
		//		where M : IId<M>
		//	{
		//		if (list == null) {
		//			return minId;
		//		}
		//		if (list.Count == 0) {
		//			return minId;
		//		}
		//
		//		// get the maxId in the list
		//		var maxId = list[0].Id;
		//		for (int i = 1; i < list.Count; i++) {
		//			maxId = Mathf.Max(maxId, list[i].Id);
		//		}
		//		// new maxId
		//		maxId++;
		//
		//		// check against required minId
		//		return Mathf.Max(maxId, minId);
		//	}

		public static T GetItemWithId<T,M>(
			this List<T> list, 
			M id
		)
		where T : class, IIdedObject<M>
		where M : class, IId<M>
		{
			if (list == null)
				return null;
			for (int i = 0; i < list.Count; i++) {
				if (list[i].Id == id)
					return list[i];
			}
			return null;
		}

#endregion


		//	public static List<T> GetIdSyncedList<T>(
		//		this List<T> list,
		//		List<T> other
		//	)
		//		where T : class, IIdedObject, IDeepCopy<T>, new()
		//	{
		//		List<T> retList = new List<T>();
		//
		//		T item;
		//		T refItem;
		//		for (int refIndex = 0; refIndex < list.Count; refIndex++) {
		//			refItem = list[refIndex];
		//			item = other.GetItemWithId(refItem.Id);
		//			if (item == null) {
		//				item = new T();
		//				item.CopyFrom(refItem);
		//			}
		//			retList.Add(item);
		//		}
		//		return retList;
		//	}

		public static List<T> GetMasterAttributeAndIdAndNameSyncedList<T,M>(
			this List<T> list,
			List<T> otherList
		)
		where T : class, IIdedObject<M>, IMAsterAttributes<T>, INamedObject, new()
		where M: class, IId<M>
		{
			List<T> retList = new List<T>();

			T item;
			T refItem;
			if (otherList == null) {
				for (int refIndex = 0; refIndex < list.Count; refIndex++) {
					refItem = list[refIndex];
					item = new T();
					item.CopyMasterAttributesFrom(refItem);
					item.CopyValueAttributesFrom(refItem);
					retList.Add(item);
				}
			} else {
				T otherItem;
				for (int refIndex = 0; refIndex < list.Count; refIndex++) {
					refItem = list[refIndex];
					item = new T();

					otherItem = otherList.GetItemWithId(refItem.Id);
					if (otherItem == null) {
						otherItem = otherList.GetItemWithName(refItem.Name);
					}

					item.CopyMasterAttributesFrom(refItem);
					if (otherItem == null) {
						item.CopyValueAttributesFrom(refItem);
					} else {
						item.CopyValueAttributesFrom(otherItem);
					}
					retList.Add(item);
				}
			}
			return retList;
		}

		//	public static bool IsIdSyncedWithOtherList<T>(
		//		this List<T> list,
		//		List<T> other
		//	)
		//		where T : class, IIdedObject
		//	{
		//		if (list.Count != other.Count) {
		//			return false;
		//		}
		//		for (int i = 0; i < list.Count; i++) {
		//			if (list[i].Id != other[i].Id)
		//				return false;
		//		}
		//		return true;
		//	}

		public static bool IsMasterAttributeSyncedWithOtherList<T>(
			this List<T> list,
			List<T> other
		)
		where T : class, IMAsterAttributes<T>
		{
			if (other == null) {
				return false;
			}
			if (list.Count != other.Count) {
				return false;
			}
			for (int i = 0; i < list.Count; i++) {
				if (!list[i].HasSameMasterAttributes(other[i]))
					return false;
			}
			return true;
		}

		public static List<T> Clone<T>(
			this List<T> listToClone
		) 
		where T: ICloneable
		{
			return listToClone.Select(
				item => (T)item.Clone()
		).ToList();
	}

}
}
