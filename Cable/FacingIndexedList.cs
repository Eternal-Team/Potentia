using System;
using System.Collections.Generic;
using TheOneLibrary.Base;

namespace Potentia.Cable
{
	public class FacingIndexedList<T> : List<T>
	{
		public T this[Facing index]
		{
			get { return this[Convert.ToInt32(index)]; }
			set { this[Convert.ToInt32(index)] = value; }
		}
	}
}