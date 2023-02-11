using System.Collections.Generic;

namespace PSWM_backend.Model
{
    public class Chart
    {
        public List<long> water { get; set; }
        public List<Double> turbidity { get; set; }
        public List<string> category { get; set; }
    }

    public class YearChart
    {
        public int year { get; set; }
        public string? deviceid { get; set; }
    }

    public class PercentageYear
    {
        public string name { get; set; }
        public float y { get; set; }
    }
    public class TurbidityChart
    {
        public List<Double> turbidity { get; set; }
        public List<string> category { get; set; }
    }
}
