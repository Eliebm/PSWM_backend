using PSWM_backend.Model;
using System.Data;

namespace PSWM_backend.Controllers
{
    public class Mappers : IMappers
    {
        public District GetAllDistrict(IDataReader dataread)
        {
            District dis = new();
            IDataReader dr = dataread;

            dis.Id = (int)dr["district_id"];
            dis.Name = dr["district_name"].ToString();

            return dis;

        }

        public City GetCity(IDataReader dataread)
        {
            City city = new();
            IDataReader dr = dataread;

            city.Id = (int)dr["city_id"];
            city.Name = dr["city_name"].ToString();

            return city;

        }
        public User GetUser(IDataReader dataread)
        {
            User user = new();
            IDataReader dr = dataread;
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
            device.quantityused = (Int64)dr["quantityused"] * 100 / (Int64)dr["recharge_quantity"];
            device.rechargequantity = (Int64)dr["recharge_quantity"];
            device.userstatus = dr["user_status"].ToString();
            device.adminstatus = dr["admin_status"].ToString();

            return device;

        }


        public DeviceDetails Fetchdevicedetails(IDataReader dataReader)
        {
            DeviceDetails device = new();
            IDataReader dr = dataReader;
            device.name = dr["name"].ToString();
            device.macaddres = dr["user_name"].ToString() + " " + dr["user_lastname"].ToString();
            device.idleday = (int)dr["idleDays"];
            DateTime dateto = (DateTime)dr["cycleTo"];
            device.cycleto = dateto.ToShortDateString();
            DateTime datefrom = (DateTime)dr["cycleFrom"];
            device.cyclefrom = datefrom.ToShortDateString();
            if (dr["admin_status"].ToString() == "true")
            {
                device.adminstatus = "Authorized";
            }
            else { device.adminstatus = "UnAuthorized"; }
            device.userstatus = dr["user_status"].ToString();
            device.quantityused = (Int64)dr["quantityused"];
            device.rechargequantity = (Int64)dr["recharge_quantity"];
            device.remainingquantity = (Int64)dr["remainingquant"];



            return device;
        }

        public PostDailyChart FetchDailyWaterData(IDataReader dataReader)
        {
            PostDailyChart device = new();
            IDataReader dr = dataReader;

            device.Time = dr["time"].ToString();
            device.watervalue = (long)dr["value"];

            return device;
        }
        public PostDailyChart FetchDailyTurbidityData(IDataReader dataReader)
        {
            PostDailyChart device = new();
            IDataReader dr = dataReader;

            device.Time = dr["time"].ToString();
            device.turbidityvalue = (Double)dr["value"];

            return device;
        }

        public PostNotification notificationcount(IDataReader dataread)
        {
            PostNotification postnotif = new();
            IDataReader dr = dataread;
            postnotif.id = (int)dr["notification_number"];

            return postnotif;
        }

        public Notification fetchNotification(IDataReader dataread)
        {
            Notification notification = new Notification();
            IDataReader dr = dataread;

            notification.id = (int)dr["id"];
            notification.date = dr["notif_date"].ToString();
            notification.iread = dr["notif_isread"].ToString();
            notification.text = dr["notif_text"].ToString() + " " + dr["notif_value"].ToString();

            return notification;
        }


        public Arduinoinfo ArdFetchDeviceInfo(IDataReader dataread)
        {
            Arduinoinfo ard = new();
            IDataReader dr = dataread;
            ard.id = dr["deviceId"].ToString();
            ard.remainingquant = (Int64)dr["remainingquant"];
            ard.userstatus = dr["user_status"].ToString();
            ard.adminstatus = dr["admin_status"].ToString();

            return ard;

        }

        public adminDeviceDetails AdminFetchdevicedetails(IDataReader dataReader)
        {
            adminDeviceDetails device = new();
            IDataReader dr = dataReader;
            device.id = dr["deviceId"].ToString();
            device.name = dr["name"].ToString();
            device.macaddres = dr["user_name"].ToString() + " " + dr["user_lastname"].ToString();
            device.cityname = dr["city_name"].ToString();
            device.adminstatus = dr["admin_status"].ToString();
            device.userstatus = dr["user_status"].ToString();
            device.quantityused = (Int64)dr["quantityused"] * 100 / (Int64)dr["recharge_quantity"];
            device.rechargequantity = (Int64)dr["recharge_quantity"];
            device.remainingquantity = (Int64)dr["remainingquant"];



            return device;
        }

       


    }
}
