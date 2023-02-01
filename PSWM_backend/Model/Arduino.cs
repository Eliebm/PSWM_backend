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
}
