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
    }
}
