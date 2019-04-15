using System;

namespace TextFinder.Model
{
	public class DateFilterCriteria
	{
		public DateTime? CreatedAfterDate
		{
			get;
			set;
		}

		public DateTime? CreatedBeforeDate
		{
			get;
			set;
		}

		public DateTime? LastAccessedAfterDate
		{
			get;
			set;
		}

		public DateTime? LastAccessedBeforeDate
		{
			get;
			set;
		}

		public DateTime? ModifiedAfterDate
		{
			get;
			set;
		}

		public DateTime? ModifiedBeforeDate
		{
			get;
			set;
		}
	}
}