namespace PSWM_backend.Model
{
    public class Arduinoinfo
    {
        public string? id { get; set; } 
        public Int64 remainingquant { get; set; }
        public string? userstatus { get; set; }
        public string? adminstatus { get; set; }
    }

    public class Arduino
    {
        public string? id { get; set; }
        public string? useraccount { get; set; }

    }

    public class ArduinoNotif
    {
        public string? deviceid { get; set; }
        public string? text { get; set; }

        public string? type { get; set; }

        public double value { get; set; }

    }
    public class ArduinoFlowTurb
    {
        public string? deviceid { get; set; }
        public long flowvalue { get; set; }

        public double turbidityvalue { get; set; }

    }

}
