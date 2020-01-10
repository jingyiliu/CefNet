using System;
using System.Collections.Generic;
using System.Text;

namespace CefNet
{
	public partial struct CefTime
	{
		public static CefTime FromDateTime(DateTime t)
		{
			t =  t.ToUniversalTime();
			return new CefTime
			{
				DayOfMonth = t.Day,
				DayOfWeek = (int)t.DayOfWeek,
				Hour = t.Hour,
				Millisecond = t.Millisecond,
				Minute = t.Minute,
				Month = t.Month,
				Second = t.Second,
				Year = t.Year
			};
		}

		public DateTime ToDateTime()
		{
			return new DateTime(Year, Month, DayOfMonth, Hour, Minute, Second, DateTimeKind.Utc);
		}
	}
}
