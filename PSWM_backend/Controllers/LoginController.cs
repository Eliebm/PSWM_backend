
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Nancy.Json;
using System.Data.SqlClient;

namespace PSWM_backend_project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {

        private readonly IConfiguration _configuration;

        public LoginController(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        //API Functions !!!!

        [Route("GetHello()")]
        [HttpGet]

        public string GetAllProvinces() {
            string msg = "hola amigosssssss  adasdasd !!!";
            return msg;
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

            returnmsg = new JavaScriptSerializer().Serialize(listProv);



            dr.Close();
            con.Close();

            return returnmsg;
        }
    

      [Route("DeleteProvince()")]
       [HttpPost]

       public string DeleteProvince(int id)
       {
            string msg;
        string logDbConnectionString = _configuration.GetValue<string>("ConnectionStrings:dbconnection");
        SqlConnection con = new(logDbConnectionString);
        con.Open();
            SqlCommand cmd = new("deleteprovince", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;
            SqlDataReader dr = cmd.ExecuteReader();

            if (dr.NextResult()) {
                msg = "Province succefully deleted";
            }else { msg = "error"; }
            dr.Close();
            con.Close();
        return msg;

        }
     }
}
