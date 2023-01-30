using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PSWM_backend.Model;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;



namespace PSWM_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserHomeController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IMappers _mapperservice;
        private readonly IGetSetSPI _GetSetSPI;
        public UserHomeController(IConfiguration configuration, IMappers mapperService ,IGetSetSPI getSetSPI)
        {
            _configuration = configuration;
            _mapperservice = mapperService;
            _GetSetSPI = getSetSPI;

        }


        [Route("GetAllProvinces()")]
        [HttpPost, Authorize]
        public string GetProvince()
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

        [Route("GetAllDistricts()")]
        [HttpPost, Authorize]

        public string GetAllDistricts([FromBody] Province prov)
        {
            string replymsg = "";
            replymsg = JsonConvert.SerializeObject(_GetSetSPI.GetSpAllItem<District>("GetAllDistrict",_mapperservice.GetAllDistrict,prov.Id));
            return replymsg;
        }


        [Route("GetAllCities()")]
        [HttpPost,Authorize]

        public string GetAllCities([FromBody] District dis)
        {
            string replymsg = "";
            replymsg = JsonConvert.SerializeObject(_GetSetSPI.GetSpAllItem<City>("GetAllCities", _mapperservice.GetCity, dis.Id));
            return replymsg;
        }

        [Route("FetchUserData()")]
        [HttpPost,Authorize]


        public IActionResult FetchUserData([FromBody] User user)
        {
            try {
                return Ok(JsonConvert.SerializeObject(_GetSetSPI.GetSpAllItem<User>("FetchUserInfo", _mapperservice.GetUser, user.account))); 
            } catch (Exception ex) { return BadRequest(ex.Message); }
            

        }
        [Route("AddNewDevice()")]
        [HttpPost]

        public IActionResult AddNewDevice([FromBody] Device device)
        {
            string id;
            var dateTimeNow = DateTime.Now; // Return 00/00/0000 00:00:00
            

            id = Guid.NewGuid().ToString()[..5];
            string mac = "null";
            int idleday = 10;
            var dateto = dateTimeNow.AddMonths(1).ToShortDateString();
            int quant = 100;
            string userstatus = "false";
            string adminstatus = "true";
            var datefrom = dateTimeNow.ToString();



            try { return Ok(_GetSetSPI.PostSpAllItem<Device>("addNewDevice",id,mac,device.name,device.city,device.street,device.building, idleday, dateto, quant, userstatus, adminstatus,device.id, datefrom)); }
            catch (Exception ex) { return BadRequest(ex.Message); }



        }
    }
}
