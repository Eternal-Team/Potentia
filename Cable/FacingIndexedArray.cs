using System;
using System.Collections;
using System.Collections.Generic;
using TheOneLibrary.Base;

namespace Potentia.Cable
{
	public class FacingIndexedArray<T> : IEnumerable
	{
		public FacingIndexedArray()
		{
			Values = new T[0];
		}

		internal T[] Values;

		public T this[Facing index]
		{
			get { return Values[Convert.ToInt32(index)]; }
			set { Values[Convert.ToInt32(index)] = value; }
		}

		public T this[int index]
		{
			get { return Values[index]; }
			set { Values[index] = value; }
		}

		public void Add(T value)
		{
			Array.Resize(ref Values, Values.Length + 1);
			Values[Values.Length - 1] = value;
		}

		private IEnumerable<T> CreateEnumerable() => Values;

		public IEnumerator<T> GetEnumerator() => CreateEnumerable().GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}