using Nancy.Routing.Trie;

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
        public int rechargequantity { get; set; }

        public DateTime cyclefrom { get; set; }
        public DateTime cycleto { get; set; }

        public int quantityused { get; set; }
        public string? userstatus { get; set; }
        public string? adminstatus { get; set; }





    }
}
