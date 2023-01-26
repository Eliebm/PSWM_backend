using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PSWM_backend.Controllers;
using PSWM_backend.Model;
using System.Data;

using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace PSWM_backend.Controllers
{
    public class Authentication :IadditionalService
    {
        private readonly IConfiguration _configuration;

       

        public Authentication(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        public string tokenAuthentication(string id)
        {

            var refreshtok = "";
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey@345543345"));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            var tokeOptions = new JwtSecurityToken(
                issuer: "https://localhost:44229",
                audience: "https://localhost:44200",
                claims: new List<Claim>(),
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: signinCredentials
            );
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
            var unixdate = new DateTimeOffset(tokenTo).ToUnixTimeMilliseconds();

            string dbConnection = _configuration.GetValue<string>("ConnectionStrings:dbconnection");
            
            SqlConnection sqlcon = new(dbConnection);
            SqlCommand cmd = new("updateToken", sqlcon);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@id", SqlDbType.NVarChar).Value = id;
            cmd.Parameters.Add("@token", SqlDbType.NVarChar).Value = refreshtok;
            sqlcon.Open();
            SqlDataReader dr = cmd.ExecuteReader();
            
            
            dr.Close();
            sqlcon.Close();

            return JsonConvert.SerializeObject(new AuthenticatedResponse { ID = id, accessToken = tokenString, refreshToken = refreshtok, issueDate = issueDate.ToString(), expireDate = unixdate.ToString() });

        }
    }
}
