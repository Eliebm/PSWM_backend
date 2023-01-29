using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PSWM_backend.Model;
using System.Data.SqlClient;
using System.Data;
using Microsoft.AspNetCore.Authorization;

namespace PSWM_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserHomeController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IMappers _mapperservice;
        public UserHomeController(IConfiguration configuration, IMappers mapperService)
        {
            _configuration = configuration;
            _mapperservice = mapperService;

        }
        [Route("GetAllProvinces()")]
        [HttpPost]
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

    }
}
