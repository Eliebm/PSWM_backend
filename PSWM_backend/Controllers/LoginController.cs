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
using System.Data.Common;
using Microsoft.Extensions.Configuration;

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

        public IActionResult Login([FromBody] Login user)
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
        public string RefreshToken([FromBody] Refreshtok refToken) {
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
                    string msg = JsonConvert.SerializeObject(new Refreshtok { id = "null", token = "null", response = 1 });
                    return msg;

                }
                
                dr.Close();
                sqlcon.Close();
            } catch (Exception ex) { return ex.Message; }
            
        }

        [Route("SignUp()")]
        [HttpPost]
        public IActionResult SignUp([FromBody] Signup sign)
        {
            string id;

           id = Guid.NewGuid().ToString()[..13];

            string dbConnection = _configuration.GetValue<string>("ConnectionStrings:dbconnection");
            try
            {
                SqlConnection sqlcon = new(dbConnection);
                SqlCommand cmd = new("checkUserId", sqlcon)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.Add("@id", SqlDbType.NVarChar).Value = id;
                sqlcon.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read()) {
                    id = Guid.NewGuid().ToString()[..13];
                    cmd.Cancel();
                }
                
                    cmd = new("Signup", sqlcon)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.Add("@id", SqlDbType.NVarChar).Value = id;
                    cmd.Parameters.Add("@name", SqlDbType.NVarChar).Value = sign.name;
                    cmd.Parameters.Add("@lname", SqlDbType.NVarChar).Value = sign.lname;
                    cmd.Parameters.Add("@phone", SqlDbType.NVarChar).Value = sign.phone;
                    cmd.Parameters.Add("@email", SqlDbType.NVarChar).Value = sign.email;
                    cmd.Parameters.Add("@accountname", SqlDbType.NVarChar).Value = sign.account;
                    cmd.Parameters.Add("@pass", SqlDbType.NVarChar).Value = sign.pass;
                    cmd.Parameters.Add("@token", SqlDbType.NVarChar).Value = "null";
                    dr = cmd.ExecuteReader();

                



                dr.Close();
                sqlcon.Close();
                return Ok(1);

            }
            catch (Exception ex) {   return BadRequest(ex.Message); }

            




        }

        [Route("AdminLogin()")]
        [HttpPost]
        public IActionResult AdminLogin([FromBody] Admin admin)
        {

            string dbConnection = _configuration.GetValue<string>("ConnectionStrings:dbconnection");
            try
            {
                SqlConnection sqlcon = new(dbConnection);
                SqlCommand cmd = new("AdminLogin", sqlcon)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.Add("@id", SqlDbType.NVarChar).Value = admin.adminid;
                cmd.Parameters.Add("@name", SqlDbType.NVarChar).Value = admin.adminName;
                cmd.Parameters.Add("@pass", SqlDbType.NVarChar).Value = admin.adminPass;
                sqlcon.Open();
                SqlDataReader dr = cmd.ExecuteReader();


                if (dr.Read())
                {
                    
                    return Ok(1);
                }

                dr.Close();
                sqlcon.Close();
                return Unauthorized();
            }
            catch (Exception ex) { return BadRequest(ex.Message); }



        }
    }
    

}
