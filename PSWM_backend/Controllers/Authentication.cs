using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PSWM_backend.Controllers;
using PSWM_backend.Model;
using System;
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
                issuer: "https://localhost:5000",
                audience: "https://localhost:5000",
                claims: new List<Claim>(),
                expires: DateTime.UtcNow.AddHours(5),
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


        public void CheckDateValidation(string id)
        {
            
            int idle = 0;
            DateTime date = DateTime.Now;
            int newidleday= 30;
            int NrOfDays = 0;

            DateTime? dateto = DateTime.Now;



            try
            {
                string logDbConnectionString = _configuration.GetValue<string>("ConnectionStrings:dbconnection");
                SqlConnection con = new(logDbConnectionString);
                con.Open();
                SqlCommand cmd = new("CheckIdleDays", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.Add("@deviceid", SqlDbType.NVarChar).Value = id;
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    idle = (int)dr["idleDays"];
                    dateto = (DateTime)dr["cycleTo"];
                }
                dr.Close();
                con.Close();
                int res = date.Date.CompareTo(dateto);


                if (res == 1)
                { 

                    TimeSpan t = (TimeSpan)(date - dateto);
                    NrOfDays = t.Days;
                    

                }
                if(newidleday - NrOfDays != idle)
                {
                    if (newidleday - NrOfDays < 0)
                    {
                        newidleday = 0;
                        con.Open();
                        SqlCommand cmd2 = new("UpDateAuthority", con)
                        {
                            CommandType = CommandType.StoredProcedure
                        };
                        cmd2.Parameters.Add("@deviceid", SqlDbType.NVarChar).Value = id;
                        cmd2.Parameters.Add("@auth_status", SqlDbType.NVarChar).Value = "false";
                        SqlDataReader dr2 = cmd2.ExecuteReader();
                        dr2.Close();
                        con.Close();

                    }
                    else { newidleday = newidleday - NrOfDays; }
                    
                    con.Open();
                    SqlCommand cmd1 = new("UpdateIdleDays", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd1.Parameters.Add("@deviceid", SqlDbType.NVarChar).Value = id;
                    cmd1.Parameters.Add("@idle", SqlDbType.NVarChar).Value = newidleday;
                    SqlDataReader dr1 = cmd1.ExecuteReader();
                    dr1.Close();
                    con.Close();
                }
                

               

            }
            catch (Exception ex) { }

        }

        public void CheckRemainingQuantity(string id)
        {

            var date = DateTime.Now;
            try
            {
                string logDbConnectionString = _configuration.GetValue<string>("ConnectionStrings:dbconnection");
                SqlConnection con = new(logDbConnectionString);
                con.Open();



                long amount = 0;
                string year = date.Year.ToString();
                string month = date.Month.ToString();

                SqlCommand cmd1 = new("GetUsedAmountForEachDevice", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd1.Parameters.Add("@deviceid", SqlDbType.NVarChar).Value = id;
                cmd1.Parameters.Add("@setyear", SqlDbType.NVarChar).Value = year;
                cmd1.Parameters.Add("@setmonth", SqlDbType.NVarChar).Value = month;

                SqlDataReader dr1 = cmd1.ExecuteReader();

                while (dr1.Read())
                {
                    amount += (long)dr1["usedamount"];


                }
                SqlCommand cmd2 = new("UpdateDeviceQuantityUsed", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd2.Parameters.Add("@deviceid", SqlDbType.NVarChar).Value = id;
                cmd2.Parameters.Add("@quant", SqlDbType.BigInt).Value = amount;
                dr1 = cmd2.ExecuteReader();

                dr1.Close();

                con.Close();


            }
            catch (Exception ex) {  }

        }



        public void NewMonthUpdateReamingQuantity(string id)
        {

            var date = DateTime.Now;
           
            long amount = 0;
            string year = date.Year.ToString();
            long quantity = 0;
            long remain = 0;
            long remaining = 0;
            string? flag = "";
            try
            {
                string logDbConnectionString = _configuration.GetValue<string>("ConnectionStrings:dbconnection");
                SqlConnection con = new(logDbConnectionString);
                con.Open();



                int monthnumb = date.Month;

                int firstday = date.Day;

                monthnumb = monthnumb - 1;
                if (monthnumb == 0)
                {
                    monthnumb = 12;
                }

                SqlCommand cmd2 = new("[fetchDeviceDetails]", con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd2.Parameters.Add("@deviceid", SqlDbType.NVarChar).Value = id;
                SqlDataReader dr2 = cmd2.ExecuteReader();
                while (dr2.Read())
                {
                    quantity = (long)dr2["recharge_quantity"];
                    remain = (long)dr2["remainingquant"];

                }
                dr2.Close();

                SqlCommand cmdgetflag = new("[fetchDeviceDetails]", con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmdgetflag.Parameters.Add("@deviceid", SqlDbType.NVarChar).Value = id;
                SqlDataReader drgetflag = cmdgetflag.ExecuteReader();
                while (drgetflag.Read())
                {
                    flag = drgetflag["isupdated_endmonth_amount"].ToString();
                   

                }
                drgetflag.Close();


                if (firstday == 1)
                {
                    
                    SqlCommand cmd1 = new("GetUsedAmountForEachDevice", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd1.Parameters.Add("@deviceid", SqlDbType.NVarChar).Value = id;
                    cmd1.Parameters.Add("@setyear", SqlDbType.NVarChar).Value = year;
                    cmd1.Parameters.Add("@setmonth", SqlDbType.NVarChar).Value = monthnumb;

                    SqlDataReader dr1 = cmd1.ExecuteReader();

                    while (dr1.Read())
                    {
                        amount += (long)dr1["usedamount"];


                    }
                    dr1.Close();

                    
                        remaining = quantity - amount;

                    if (flag!="true") { 
                    
                    SqlCommand cmd = new("NewMonthUpdateTotalReamaingAmount", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.Add("@deviceid",SqlDbType.NVarChar).Value=id;
                    cmd.Parameters.Add("@remaing", SqlDbType.BigInt).Value = remaining;
                    SqlDataReader dr= cmd.ExecuteReader();
                    dr.Close();
                    

                    }
                }
                else if (firstday != 1)
                {
                    if (flag == "true")
                    {
                        SqlCommand cmd = new("update_isUpdatedmonth_flag", con)
                        {
                            CommandType = CommandType.StoredProcedure
                        };

                        cmd.Parameters.Add("@deviceid", SqlDbType.NVarChar).Value = id;
                        cmd.Parameters.Add("@flag", SqlDbType.BigInt).Value = false;
                        SqlDataReader dr = cmd.ExecuteReader();
                        dr.Close();
                       

                    }
                    
                }



                con.Close();

            }
            catch (Exception ex) { }

        }

    }
}
