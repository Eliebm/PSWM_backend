using PSWM_backend.Model;
using System.Data;

namespace PSWM_backend
{
    public interface IMappers
    {
        District GetAllDistrict(IDataReader dataread);
        City GetCity(IDataReader dataread);
        User GetUser(IDataReader dataread);
        Device FetchAllDevices(IDataReader dataread);
        
        Arduinoinfo ArdFetchDeviceInfo(IDataReader dataread);
        DeviceDetails Fetchdevicedetails(IDataReader dataReader);
        PostDailyChart FetchDailyWaterData(IDataReader dataReader);
        PostDailyChart FetchDailyTurbidityData(IDataReader dataReader);

        PostNotification notificationcount(IDataReader dataread);
        public Notification fetchNotification(IDataReader dataread);
        public adminDeviceDetails AdminFetchdevicedetails(IDataReader dataread);
    }
}
