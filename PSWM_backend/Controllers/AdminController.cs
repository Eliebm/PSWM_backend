using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PSWM_backend.Model;
using System.Data.SqlClient;
using System.Data;
using System.Net;

namespace PSWM_backend.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IMappers _mapperservice;
        private readonly IGetSetSPI _GetSetSPI;
        private readonly IadditionalService _additionService;

        public AdminController(IConfiguration configuration, IMappers mapperService, IGetSetSPI getSetSPI, IadditionalService additionalService)
        {
            _configuration = configuration;
            _mapperservice = mapperService;
            _GetSetSPI = getSetSPI;
            _additionService = additionalService;

        }


        [Route("AdminGetAllProvinces()")]
        [HttpPost]
        public string AdminGetProvince()
        {
            string returnmsg;
            string logDbConnectionString = _configuration.GetValue<string>("ConnectionStrings:dbconnection");
            SqlConnection con = new(logDbConnectionString);
            con.Open();
            SqlCommand cmd = new("GetAllProvinces", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            SqlDataReader dr = cmd.ExecuteReader();
            var listProv = new List<Province>();

            while (dr.Read())
            {


                Province prov = new()
                {
                    Id = (int)dr["Province_Id"],
                    Name = dr["Province_Name"].ToString()
                };


                listProv.Add(prov);
            }

            returnmsg = JsonConvert.SerializeObject(listProv);



            dr.Close();
            con.Close();

            return returnmsg;
        }

        [Route("AdminGetAllDistricts()")]
        [HttpPost]

        public string AdminGetAllDistricts([FromBody] Province prov)
        {
            string replymsg = "";
            replymsg = JsonConvert.SerializeObject(_GetSetSPI.GetSpAllItem<District>("GetAllDistrict", _mapperservice.GetAllDistrict, prov.Id));
            return replymsg;
        }


        [Route("AdminGetAllCities()")]
        [HttpPost]

        public string AdminGetAllCities([FromBody] District dis)
        {
            string replymsg = "";
            replymsg = JsonConvert.SerializeObject(_GetSetSPI.GetSpAllItem<City>("GetAllCities", _mapperservice.GetCity, dis.Id));
            return replymsg;
        }

        [Route("AdminFetchAllUsers()")]
        [HttpPost]

        public IActionResult AdminFetchAllUsers([FromBody] PostFetchAdminUser admuser)
        {
            return Ok(JsonConvert.SerializeObject(_GetSetSPI.GetSpAllItem<adminDeviceDetails>("AdminFetchAllCityUsers", _mapperservice.AdminFetchdevicedetails, admuser.cityid)));
        }

        [Route("AdminFetchDeviceDetails()")]
        [HttpPost]

        public IActionResult AdminFetchDeviceDetails([FromBody] PostDevice device)
        {
         //   _additionService.CheckDateValidation(device.id);

         //   _additionService.CheckRemainingQuantity(device.id);
         //   _additionService.NewMonthUpdateReamingQuantity(device.id);

            return Ok(JsonConvert.SerializeObject(_GetSetSPI.GetSpAllItem<DeviceDetails>("fetchDeviceDetails", _mapperservice.Fetchdevicedetails, device.id)));


        }
        [Route("AdminTurndeviceOnOff()")]
        [HttpPost]

        public IActionResult AdminTurndeviceOnOff([FromBody] PostDevice device)
        {
            
            return Ok(JsonConvert.SerializeObject(_GetSetSPI.PostSpAllItem<DeviceDetails>("[UpDateAuthority]", device.id, device.userstatus)));
        }

    }
    
    
}
