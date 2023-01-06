
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PSWM_backend.Model;
using System.Data;

using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

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
        [HttpGet, Authorize]

        public string GetAllProvinces() {
        
        string msg = "hola amigosssssss  adasdasd !!!";
            return msg;
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
            var refreshtok = "";
            if (user is null)
            {
                return BadRequest("Invalid client request");
            }
            if (user.UserName == "elie" && user.Password == "elie@123")
            {
                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey@345543345"));
                var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
                var tokeOptions = new JwtSecurityToken(
                    issuer: "https://localhost:5000",
                    audience: "https://localhost:5000",
                    claims: new List<Claim>(),
                    expires: DateTime.UtcNow.AddHours(5),
                    signingCredentials: signinCredentials
                ) ;
                var issueDate = DateTime.UtcNow;
                var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
                var token = new JwtSecurityToken().InnerToken;
                var tokenTo = tokeOptions.ValidTo;

                var randomNumber = new byte[32];
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(randomNumber);
                    refreshtok = Convert.ToBase64String(randomNumber).ToString();
                }

                return Ok(new AuthenticatedResponse { accessToken = tokenString, refreshToken= refreshtok, issueDate=issueDate.ToString(),expireDate = tokenTo.ToString()});
            }
            return Unauthorized();
        }

        
        [HttpPost("SignUp()")]
        public string signUp()
        {
            string id = "";

            id = Guid.NewGuid().ToString().Substring(0, 13);
            return id;
            


            
        }


    }


}
