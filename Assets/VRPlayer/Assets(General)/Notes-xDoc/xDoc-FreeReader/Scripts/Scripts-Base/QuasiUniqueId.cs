using System;
using UnityEngine;
using xDocBase.Extensions;


namespace xDocBase.CustomData
{


	[System.Serializable]
	public class QuasiUniqueId : IId<QuasiUniqueId>
	{
		#region Id fields

		public float realtimeSinceStartup;
		public int bigTimeOne;
		public int bigTimeTwo;
		public int randomInt;

		#endregion


		#region IId implementation

		public void InitializeAsNewId ()
		{
			realtimeSinceStartup = Time.realtimeSinceStartup;

			var bigTime = DateTime.Now.Ticks;
			bigTimeOne = (int)(bigTime & uint.MaxValue);
			bigTimeTwo = (int)(bigTime >> 32);

			UnityEngine.Random.InitState (bigTimeOne);
			randomInt = UnityEngine.Random.Range (int.MinValue, int.MaxValue);
		}

		#endregion


		#region IDeepCopy implementation

		public void CopyFrom (
			QuasiUniqueId src
		)
		{
			realtimeSinceStartup = src.realtimeSinceStartup;
			bigTimeOne = src.bigTimeOne;
			bigTimeTwo = src.bigTimeTwo;
			randomInt = src.randomInt;
		}

		#endregion


		#region IEquatable implementation

		/// <summary>
		/// Determines whether the specified <see cref="xDocBase.CustomData.QuasiUniqueId"/> is equal to the current <see cref="xDocBase.CustomData.QuasiUniqueId"/>.
		/// </summary>
		/// <param name="other">The <see cref="xDocBase.CustomData.QuasiUniqueId"/> to compare with the current <see cref="xDocBase.CustomData.QuasiUniqueId"/>.</param>
		/// <returns><c>true</c> if the specified <see cref="xDocBase.CustomData.QuasiUniqueId"/> is equal to the current
		/// <see cref="xDocBase.CustomData.QuasiUniqueId"/>; otherwise, <c>false</c>.</returns>
		public bool Equals (
			QuasiUniqueId other
		)
		{
			if (ReferenceEquals (null, other))
				return false;
			if (ReferenceEquals (this, other))
				return true;
			if (realtimeSinceStartup != other.realtimeSinceStartup)
				return false;
			if (bigTimeOne != other.bigTimeOne)
				return false;
			if (bigTimeTwo != other.bigTimeTwo)
				return false;
			if (randomInt != other.randomInt)
				return false;
			return true;
		}

		/// <summary>
		/// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="xDocBase.CustomData.QuasiUniqueId"/>.
		/// </summary>
		/// <param name="obj">The <see cref="System.Object"/> to compare with the current <see cref="xDocBase.CustomData.QuasiUniqueId"/>.</param>
		/// <returns><c>true</c> if the specified <see cref="System.Object"/> is equal to the current
		/// <see cref="xDocBase.CustomData.QuasiUniqueId"/>; otherwise, <c>false</c>.</returns>
		public override bool Equals (
			object obj
		)
		{
			// Since our other Equals() method already compares guys, we'll just call it.
			if (!(obj is QuasiUniqueId))
				return false;
			return Equals ((QuasiUniqueId)obj);
		}

		/// <summary>
		/// Serves as a hash function for a particular type.
		/// </summary>
		/// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a hash table.</returns>
		public override int GetHashCode ()
		{
			// non readonly fields should not be used in GetHashCode usually, but it is
			// OK in this situation, bc. these fields are quasi readonly fields
			// Analysis disable once NonReadonlyReferencedInGetHashCode
			return bigTimeOne;
		}

		public static bool operator == (
			QuasiUniqueId left,
			QuasiUniqueId right
		)
		{
			// If we used == to check for null instead of Object.ReferenceEquals(), we'd
			// get a StackOverflowException. Can you figure out why?
			if (System.Object.ReferenceEquals (left, null))
				return false;
			else
				return left.Equals (right);
		}

		public static bool operator != (
			QuasiUniqueId left,
			QuasiUniqueId right
		)
		{
			// Since we've already defined ==, we can just invert it for !=.
			return !(left == right);
		}

		#endregion


		#region System.Object override

		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		/// <returns>A string that represents the current object.</returns>
		/// <filterpriority>2</filterpriority>
		public override string ToString ()
		{
			return "ID: " +
			realtimeSinceStartup + "/" +
			bigTimeOne + "/" +
			bigTimeTwo + "/" +
			randomInt;
		}

		#endregion

	}
}

