using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PSWM_backend.Model;
using System.Data;
using System.Data.SqlClient;
using PSWM_backend.Controllers;
using System.Security.AccessControl;
using System.IO.Pipelines;
using PSWM_backend;
using System.Text.Json;

namespace PSWM_backend_project.Controllers
{ 
    
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        
        private readonly IConfiguration _configuration;
        private readonly IadditionalService _service;
        public LoginController(IConfiguration configuration, IadditionalService addService)
        {
            _configuration = configuration;
            _service = addService;
            
        }
        
        // Additional Functions

        //API Functions !!!!



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

        [Route("login()")]
        [HttpPost]

        public IActionResult Login([FromBody] login user)
        { 
            string dbConnection = _configuration.GetValue<string>("ConnectionStrings:dbconnection");
            try
            {
                SqlConnection sqlcon = new(dbConnection);
                SqlCommand cmd = new("userLogin", sqlcon)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.Add("@accountname", SqlDbType.NVarChar).Value=user.userName;
                cmd.Parameters.Add("@password", SqlDbType.NVarChar).Value = user.password;
                sqlcon.Open();
                SqlDataReader dr = cmd.ExecuteReader();
            
            
            if (dr.Read()) {
                    var id = dr["user_id"].ToString();
                    return Ok(_service.tokenAuthentication(id));
            }
            
                dr.Close();
                sqlcon.Close();
                return Unauthorized();
            }
            catch (Exception ex) { return BadRequest(ex.Message); }


            
        }

        [Route("RefreshToken()")]
        [HttpPost]
        public string RefreshToken([FromBody] refreshtok refToken) {
            string dbConnection = _configuration.GetValue<string>("ConnectionStrings:dbconnection");
            try
            {
                SqlConnection sqlcon = new(dbConnection);
                SqlCommand cmd = new("refreshtoken", sqlcon)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.Add("@userid", SqlDbType.NVarChar).Value = refToken.id;
                cmd.Parameters.Add("@refresh_token", SqlDbType.NVarChar).Value = refToken.token;
                sqlcon.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    return _service.tokenAuthentication(refToken.id);
                }
                else
                {
                    string msg = JsonConvert.SerializeObject(new refreshtok { id = "null", token = "null", response = 1 });
                    return msg;

                }
            } catch (Exception ex) { return ex.Message; }
            
        }


        [HttpPost("SignUp()")]
        public string SignUp()
        {
            string id;

            id = Guid.NewGuid().ToString()[..13];
            return id;
            


            
        }


    }


}
