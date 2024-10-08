﻿using Microsoft.AspNetCore.Mvc;
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
using Microsoft.AspNetCore.Authorization;

namespace PSWM_backend_project.Controllers
{ 
    
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        
        private readonly IConfiguration _configuration;
        private readonly IadditionalService _service;
        private readonly IGetSetSPI _getSPI;
        public LoginController(IConfiguration configuration, IadditionalService addService, IGetSetSPI getSPI)
        {
            _configuration = configuration;
            _service = addService;
            _getSPI = getSPI;
        }

        // Additional Functions

        //API Functions !!!!


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
            string result;
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
                    result= _service.tokenAuthentication(refToken.id);
                }
                else
                {
                    result = JsonConvert.SerializeObject(new Refreshtok { id = "null", token = "null", response = 1 });
                   

                }
                
               
                sqlcon.Close();
                dr.Close();
                return result;
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



        [Route("ChangeUserPassword()")]
        [HttpPost, Authorize]

        public IActionResult ChangeUserPassword([FromBody] ChangePassword changep)
        {
            try { return Ok(_getSPI.PostSpAllItem<ChangePassword>("ChangeUserPassword", changep.id, changep.password)); }
            catch (Exception ex) { return BadRequest(ex.Message); } 
            


        }





    }
    

}
