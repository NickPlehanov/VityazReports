namespace VityazReports.Models.AnalyzeServicemans {
    public class IntervalsModel {
        public IntervalsModel(string range, string duration) {
            Range = range;
            Duration = duration;
        }

        public string Range { get; set; }
        public string Duration { get; set; }
    }
}
