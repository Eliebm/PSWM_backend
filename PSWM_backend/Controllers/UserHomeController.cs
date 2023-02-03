using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PSWM_backend.Model;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using System.Collections.Generic;
using Microsoft.VisualBasic;

namespace PSWM_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserHomeController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IMappers _mapperservice;
        private readonly IGetSetSPI _GetSetSPI;
        private readonly IadditionalService _additionService;
        
        public UserHomeController(IConfiguration configuration, IMappers mapperService ,IGetSetSPI getSetSPI,IadditionalService addition)
        {
            _configuration = configuration;
            _mapperservice = mapperService;
            _GetSetSPI = getSetSPI;
            _additionService= addition;
        }


        [Route("GetAllProvinces()")]
        [HttpPost,Authorize]
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
        [HttpPost,Authorize]

        public IActionResult AddNewDevice([FromBody] Device device)
        {
            string id;
            var dateTimeNow = DateTime.Now; 
            

            id = Guid.NewGuid().ToString()[..5];
            string mac = "null";
            int idleday = 30;
            var dateto = dateTimeNow.AddMonths(1).ToShortDateString();
            int quant = 100;
            string userstatus = "false";
            string adminstatus = "true";
            var datefrom = dateTimeNow.ToShortDateString();
            int quantused = 0;



            try { return Ok(_GetSetSPI.PostSpAllItem<Device>("addNewDevice",id,mac,device.name,device.city,device.street,device.building, idleday, dateto, quant, userstatus, adminstatus,device.id, datefrom, quantused)); }
            catch (Exception ex) { return BadRequest(ex.Message); }



        }
    


        [Route("FetchUserDevices()")]
        [HttpPost]
        public IActionResult FetchUserDevices([FromBody] User user)
         {
            var date = DateTime.Now;
        try
            {
                string logDbConnectionString = _configuration.GetValue<string>("ConnectionStrings:dbconnection");
                SqlConnection con = new(logDbConnectionString);
                con.Open();
                SqlCommand cmd = new("GetDevicesid", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.Add("@userid", SqlDbType.NVarChar).Value = user.id;
                SqlDataReader dr = cmd.ExecuteReader();
                 var listd= new List<Device>();
                

                while (dr.Read())
                {
                    Device device = new Device();
                    device.id = dr["deviceId"].ToString();
                    listd.Add(device);

                }
                cmd.Cancel();
                con.Close();
                List<string> branches = listd.Select(q => q.id).Distinct().ToList();
                con.Open();
                foreach (string b in branches)
                {   
                    long amount = 0;
                    string year = date.Year.ToString();
                    string month = date.Month.ToString();
                    
                    SqlCommand cmd1 = new("GetUsedAmountForEachDevice", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd1.Parameters.Add("@deviceid", SqlDbType.NVarChar).Value = b;
                    cmd1.Parameters.Add("@setyear", SqlDbType.NVarChar).Value = year;
                    cmd1.Parameters.Add("@setmonth", SqlDbType.NVarChar).Value = month;

                    SqlDataReader dr1 = cmd1.ExecuteReader();

                   while(dr1.Read())
                    {
                        amount += (long)dr1["usedamount"];
                        
                       
                    }
                    SqlCommand cmd2 = new("UpdateDeviceQuantityUsed", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd2.Parameters.Add("@deviceid", SqlDbType.NVarChar).Value = b;
                    cmd2.Parameters.Add("@quant", SqlDbType.BigInt).Value = amount;
                    dr1 = cmd2.ExecuteReader();

                    dr1.Close();
                }con.Close();

                

                return Ok();
              //  return Ok(JsonConvert.SerializeObject(_GetSetSPI.GetSpAllItem<Device>("FetchUserDevices", _mapperservice.FetchAllDevices, user.id)));
                
            }
        catch (Exception ex) { return BadRequest(ex.Message); }


        }
      

    }
}
