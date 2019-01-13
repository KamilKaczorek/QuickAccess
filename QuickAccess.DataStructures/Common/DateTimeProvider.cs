using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace QuickAccess.DataStructures.Common
{
	public static class DateTimeProvider
	{
		public static DateTime UtcNow => DateTime.UtcNow;
		public static DateTime Now => DateTime.Now;

		public static Stopwatch CreateStopwatch()
		{
			return new Stopwatch();
		}
	}
}
