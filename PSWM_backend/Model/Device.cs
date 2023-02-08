using Nancy.Routing.Trie;
using System.Numerics;

namespace PSWM_backend.Model
{
    public class Device
    {
        public string? id { get; set; }
        public string? macaddres { get; set; }
        public string? name { get; set; }

        public int city { get; set; }
        public string? cityname { get; set; }
        public string? street { get; set; }
        public string? building { get; set; }
        public int idleday { get; set; }
        public Int64 rechargequantity { get; set; }

        public DateTime cyclefrom { get; set; }
        public DateTime cycleto { get; set; }

        public float quantityused { get; set; }
        public float remainingquantity { get; set; }
        public string? userstatus { get; set; }
        public string? adminstatus { get; set; }





    }

    public class DeviceDetails {
         public string? id { get; set; }
        public string? macaddres { get; set; }
        public string? name { get; set; }
        public int idleday { get; set; }
        public Int64 rechargequantity { get; set; }

        public string? cyclefrom { get; set; }
        public string? cycleto { get; set; }
        
        public float quantityused { get; set; }
        public float remainingquantity { get; set; }
        public string? adminstatus { get; set; }
        public string? userstatus { get; set; }
    }

}
