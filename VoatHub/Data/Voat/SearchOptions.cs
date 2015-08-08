using System;
using System.Net;


namespace VoatHub.Data.Voat
{
    public class SearchOptions
    {
        public SortSpan? span { get; set; }
        public SortAlgorithm? sort { get; set; }
        public SortDirection? direction { get; set; }
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
        public int? count { get; set; }
        public int? index { get; set; }
        public int? page { get; set; }
        public string search { get; set; }
        public int? depth { get; set; }
    }

    public enum SortSpan
    {
        All, Hour, Day, Week, Month, Quarter, Year
    }

    public enum SortAlgorithm
    {
        New, Top, Hot, Active, Viewed, Discussed, Bottom
    }

    public enum SortDirection
    {
        Default, Reverse
    }
}
