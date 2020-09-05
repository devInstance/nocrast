using System;

namespace NoCrast.Shared.Model
{
    public class ReportItem
    {
        public enum RIType
        {
            Unknown,
            Daily,
            Weekly,
            Monthly
        }

        public class Row
        {
            public string TaskTitle { get; set; }
            public float[] Data { get; set; }
        }

        public RIType RiType { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime[] Columns { get; set; }
        public Row[] Rows { get; set; }
    }
}
