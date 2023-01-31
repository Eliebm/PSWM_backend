using PSWM_backend.Model;
using System.Data;

namespace PSWM_backend.Controllers
{
    public class Mappers : IMappers
    {
        public District GetAllDistrict(IDataReader dataread) {
            District dis = new();
            IDataReader dr = dataread;

            dis.Id = (int)dr["district_id"];
            dis.Name = dr["district_name"].ToString();

            return dis;
        
        }

        public City GetCity(IDataReader dataread) {
            City city = new();
            IDataReader dr = dataread;

            city.Id = (int)dr["city_id"];
            city.Name = dr["city_name"].ToString();

            return city;
        
        }
        public User GetUser(IDataReader dataread) { 
          User user = new();
            IDataReader dr= dataread;
            user.account = dr["user_accountname"].ToString();
            user.name = dr["user_name"].ToString() + " " + dr["user_lastname"].ToString();
            user.phone = dr["user_phone"].ToString();
            user.email = dr["user_email"].ToString();

            return user;
        
        }

        public Device FetchAllDevices(IDataReader dataread)
        {


            Device device = new Device();
            IDataReader dr = dataread;
            device.id = dr["deviceId"].ToString();
            device.name = dr["name"].ToString();
            device.cityname = dr["city_name"].ToString();
            device.quantityused = (int)dr["quantityused"] * 100 / (int)dr["recharge_quantity"];
            device.rechargequantity = (int)dr["recharge_quantity"];
            device.userstatus = dr["user_status"].ToString();
            device.adminstatus = dr["admin_status"].ToString();

            return device;

        }

        }
}
