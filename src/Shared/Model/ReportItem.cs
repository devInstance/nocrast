using System;

namespace NoCrast.Shared.Model
{
    public class ReportItem
    {
        public enum RIType
        {
            Unknown,
            Custom,
            Daily,
            Weekly,
            Monthly,
            Yearly,
            Total
        }

        public enum RIMode
        {
            Combined,
            ByTask
        }

        public class Cell
        {
            public string Details { get; set; }
            public float Value { get; set; }

        }
        public class Row
        {
            public TaskItem Task { get; set; }
            public Cell[] Data { get; set; }
        }

        public RIType RiType { get; set; }
        public RIMode RiMode { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime[] Columns { get; set; }
        public Row[] Rows { get; set; }
    }
}
