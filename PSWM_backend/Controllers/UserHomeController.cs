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
        [HttpPost,Authorize]
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

                

                
                return Ok(JsonConvert.SerializeObject(_GetSetSPI.GetSpAllItem<Device>("FetchUserDevices", _mapperservice.FetchAllDevices, user.id)));
                
            }
        catch (Exception ex) { return BadRequest(ex.Message); }


        }
        [Route("FetchDeviceDetails()")]
        [HttpPost,Authorize]

        public IActionResult FetchDeviceDetails([FromBody] PostDevice device)
        {
            _additionService.CheckDateValidation(device.id);

            _additionService.CheckRemainingQuantity(device.id); 

         return Ok(JsonConvert.SerializeObject(_GetSetSPI.GetSpAllItem<DeviceDetails>("fetchDeviceDetails", _mapperservice.Fetchdevicedetails, device.id)));


             }




        [Route("TurndeviceOnOff()")]
        [HttpPost,Authorize]

        public IActionResult TurndeviceOnOff([FromBody] PostDevice device)
        {
            _additionService.CheckRemainingQuantity(device.id);
            return Ok(JsonConvert.SerializeObject(_GetSetSPI.PostSpAllItem<DeviceDetails>("turndeviceOnOff", device.id,device.userstatus)));
        }

        [Route("FirstChart()")]
        [HttpPost]

        public IActionResult FirstChart([FromBody] Device device)
        {
            try
            {
                string logDbConnectionString = _configuration.GetValue<string>("ConnectionStrings:dbconnection");
                SqlConnection con = new(logDbConnectionString);
                con.Open();
                SqlCommand cmd = new("yearsChartWaterFlow", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.Add("@deviceid", SqlDbType.NVarChar).Value = device.id;
                SqlDataReader dr = cmd.ExecuteReader();
                var listchart = new List<Chart>();
                Chart chart = new Chart();
                var waterchart=new List<long>();
                var turbiditychart = new List<Double>();
                var categorychart = new List<string>();
               

                
                while (dr.Read())
                {
                    
                    categorychart.Add(dr["date"].ToString());
                    waterchart.Add((long)dr["wateramount"]);
                    
                
                }
                dr.Close();
                chart.water = waterchart;
                chart.category= categorychart;
                

                SqlCommand cmd1 = new("yearsCharturbidity", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd1.Parameters.Add("@deviceid", SqlDbType.NVarChar).Value = device.id;
                SqlDataReader dr1 = cmd1.ExecuteReader();

                while (dr1.Read())
                {
                    
                    turbiditychart.Add((Double)dr1["turbidityvalue"]);

                }
                chart.turbidity = turbiditychart;

                listchart.Add(chart);
                dr1.Close();
                con.Close();
                return Ok(JsonConvert.SerializeObject(listchart));
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
                    
        }

        [Route("ChartByYear()")]
        [HttpPost]

        public IActionResult ChartByYear([FromBody] YearChart ychart)
        {
            try
            {
                string logDbConnectionString = _configuration.GetValue<string>("ConnectionStrings:dbconnection");
                SqlConnection con = new(logDbConnectionString);
                con.Open();
                SqlCommand cmd = new("FetchYearChart", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.Add("@deviceid", SqlDbType.NVarChar).Value = ychart.deviceid;
                cmd.Parameters.Add("@yearid", SqlDbType.NVarChar).Value = ychart.year.ToString();
                SqlDataReader dr = cmd.ExecuteReader();
                var listchart = new List<Chart>();
                Chart chart = new Chart();
                var waterchart = new List<long>();
                var categorychart = new List<string>();

                while (dr.Read())
                {
                    string ycase = dr["monthdata"].ToString();
                    if (ycase == "1")
                    {
                        categorychart.Add("Jan");
                    }else if (ycase == "2")
                    {
                        categorychart.Add("Feb");
                    }
                    else if (ycase == "3")
                    {
                        categorychart.Add("Mar");
                    }
                    else if (ycase == "4")
                    {
                        categorychart.Add("Apr");
                    }
                    else if (ycase == "5")
                    {
                        categorychart.Add("May");

                    }
                    else if (ycase == "6")
                    {
                        categorychart.Add("Jun");
                    }
                    else if (ycase == "7")
                    {
                        categorychart.Add("Jul");
                    }
                    else if (ycase == "8")
                    {
                        categorychart.Add("Aug");
                    }
                    else if (ycase == "9")
                    {
                        categorychart.Add("Sep");
                    }
                    else if (ycase == "10")
                    {
                        categorychart.Add("Oct");
                    }
                    else if (ycase == "11")
                    {
                        categorychart.Add("Nov");
                    }
                    else if (ycase == "12")
                    {
                        categorychart.Add("Dec");
                    }

                    waterchart.Add((long)dr["wateramount"]); 

                }
                chart.category = categorychart;
                chart.water = waterchart;
                listchart.Add(chart);

                dr.Close();
                con.Close();

                return Ok(JsonConvert.SerializeObject(listchart));
            }
            catch (Exception ex) { return BadRequest(ex.Message); }


                
        }


        [Route("PercentageChartByYear()")]
        [HttpPost]

        public IActionResult PercentageChartByYear([FromBody] YearChart ychart)
        {
            long totalamount = 0;
            try
            {
                string logDbConnectionString = _configuration.GetValue<string>("ConnectionStrings:dbconnection");
                SqlConnection con = new(logDbConnectionString);
                con.Open();
                SqlCommand cmd = new("FetchYearChartpercentage", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.Add("@deviceid", SqlDbType.NVarChar).Value = ychart.deviceid;
                cmd.Parameters.Add("@yearid", SqlDbType.NVarChar).Value = ychart.year.ToString();
                SqlDataReader dr = cmd.ExecuteReader();
                

                if (dr.Read())
                {
                    totalamount = (long)dr["totalamount"];
                }
                dr.Close();
                con.Close();
                con.Open();

                SqlCommand cmd1 = new("FetchYearChart", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd1.Parameters.Add("@deviceid", SqlDbType.NVarChar).Value = ychart.deviceid;
                cmd1.Parameters.Add("@yearid", SqlDbType.NVarChar).Value = ychart.year.ToString();
                SqlDataReader dr1 = cmd1.ExecuteReader();
                var listchart = new List<PercentageYear>();
                

                while (dr1.Read())
                {
                    PercentageYear pyear = new();

                    string ycase = dr1["monthdata"].ToString();

                    if (ycase == "1")
                    {
                        pyear.name="Jan";
                    }
                    else if (ycase == "2")
                    {
                        pyear.name = "Feb";
                    }
                    else if (ycase == "3")
                    {
                        pyear.name = "Mar";
                    }
                    else if (ycase == "4")
                    {
                        pyear.name = "Apr";
                    }
                    else if (ycase == "5")
                    {
                        pyear.name = "May";

                    }
                    else if (ycase == "6")
                    {
                        pyear.name = "Jun";
                    }
                    else if (ycase == "7")
                    {
                        pyear.name = "Jul";
                    }
                    else if (ycase == "8")
                    {
                        pyear.name = "Aug";
                    }
                    else if (ycase == "9")
                    {
                        pyear.name = "Sep";
                    }
                    else if (ycase == "10")
                    {
                        pyear.name = "Oct";
                    }
                    else if (ycase == "11")
                    {
                        pyear.name = "Nov";
                    }
                    else if (ycase == "12")
                    {
                        pyear.name = "Dec";
                    }

                   pyear.y=(long)dr1["wateramount"] *100 / totalamount;
                   listchart.Add(pyear);
                }
                
                

                dr1.Close();
                con.Close();

                return Ok(JsonConvert.SerializeObject(listchart));
            }
            catch (Exception ex) { return BadRequest(ex.Message); }



        }

        [Route("TurbidityLineChart()")]
        [HttpPost]

        public IActionResult TurbidityLineChart([FromBody] YearChart ychart)
        {
            
            try
            {
                string logDbConnectionString = _configuration.GetValue<string>("ConnectionStrings:dbconnection");
                SqlConnection con = new(logDbConnectionString);
                con.Open();
                SqlCommand cmd = new("FetchYearTurbidityAverage", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.Add("@deviceid", SqlDbType.NVarChar).Value = ychart.deviceid;
                cmd.Parameters.Add("@yearid", SqlDbType.NVarChar).Value = ychart.year.ToString();
                SqlDataReader dr = cmd.ExecuteReader();
                var listchart = new List<TurbidityChart>();
                TurbidityChart chart = new TurbidityChart();
                var turbidity = new List<Double>();
                var categorychart = new List<string>();

                while (dr.Read())
                {
                    string ycase = dr["turbdate"].ToString();
                    if (ycase == "1")
                    {
                        categorychart.Add("Jan");
                    }
                    else if (ycase == "2")
                    {
                        categorychart.Add("Feb");
                    }
                    else if (ycase == "3")
                    {
                        categorychart.Add("Mar");
                    }
                    else if (ycase == "4")
                    {
                        categorychart.Add("Apr");
                    }
                    else if (ycase == "5")
                    {
                        categorychart.Add("May");

                    }
                    else if (ycase == "6")
                    {
                        categorychart.Add("Jun");
                    }
                    else if (ycase == "7")
                    {
                        categorychart.Add("Jul");
                    }
                    else if (ycase == "8")
                    {
                        categorychart.Add("Aug");
                    }
                    else if (ycase == "9")
                    {
                        categorychart.Add("Sep");
                    }
                    else if (ycase == "10")
                    {
                        categorychart.Add("Oct");
                    }
                    else if (ycase == "11")
                    {
                        categorychart.Add("Nov");
                    }
                    else if (ycase == "12")
                    {
                        categorychart.Add("Dec");
                    }

                    turbidity.Add((Double)dr["averagevalue"]);

                }
                chart.category = categorychart;
                chart.turbidity = turbidity;
                listchart.Add(chart);

                dr.Close();
                con.Close();

                return Ok(JsonConvert.SerializeObject(listchart));
            }
            
            catch (Exception ex) { return BadRequest(ex.Message); }



        }

        [Route("MonthWaterTurbidityChart()")]
        [HttpPost,Authorize]

        public IActionResult MonthWaterTurbidityChart( [FromBody] PostMonthChart monthChart )
        {

            try
            {
                string logDbConnectionString = _configuration.GetValue<string>("ConnectionStrings:dbconnection");
                SqlConnection con = new(logDbConnectionString);
                con.Open();
                SqlCommand cmd = new("monthChartWaterFlow", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.Add("@deviceid", SqlDbType.NVarChar).Value = monthChart.deviceid;
                cmd.Parameters.Add("@yearid", SqlDbType.NVarChar).Value = monthChart.year;
                cmd.Parameters.Add("@monthid", SqlDbType.NVarChar).Value = monthChart.month;
                SqlDataReader dr = cmd.ExecuteReader();
                var listchart = new List<Chart>();
                Chart chart = new Chart();
                var waterchart = new List<long>();
                var turbiditychart = new List<Double>();
                var categorychart = new List<string>();



                while (dr.Read())
                {

                    categorychart.Add(dr["date"].ToString());
                    waterchart.Add((long)dr["wateramount"]);


                }
                dr.Close();
                chart.water = waterchart;
                chart.category = categorychart;


                SqlCommand cmd1 = new("monthChartTurbidity", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd1.Parameters.Add("@deviceid", SqlDbType.NVarChar).Value = monthChart.deviceid;
                cmd1.Parameters.Add("@yearid", SqlDbType.NVarChar).Value = monthChart.year;
                cmd1.Parameters.Add("@monthid", SqlDbType.NVarChar).Value = monthChart.month;
                SqlDataReader dr1 = cmd1.ExecuteReader();

                while (dr1.Read())
                {

                    turbiditychart.Add((Double)dr1["turbidityvalue"]);

                }
                chart.turbidity = turbiditychart;

                listchart.Add(chart);
                dr1.Close();
                con.Close();
                return Ok(JsonConvert.SerializeObject(listchart));
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [Route("DailyWaterData()")]
        [HttpPost]

        public IActionResult DailyWaterData([FromBody] PostDailyChart dailyc)
        {
            return Ok(JsonConvert.SerializeObject(_GetSetSPI.GetSpAllItem<PostDailyChart>("dailyTableData", _mapperservice.FetchDailyWaterData, dailyc.deviceid,dailyc.year)));
        }

        [Route("DailyTurbidityData()")]
        [HttpPost]

        public IActionResult DailyTurbidityData([FromBody] PostDailyChart dailyc)
        {
            return Ok(JsonConvert.SerializeObject(_GetSetSPI.GetSpAllItem<PostDailyChart>("dailyTableTurbidityData", _mapperservice.FetchDailyTurbidityData, dailyc.deviceid, dailyc.year)));
        }

        [Route("NotificationCount()")]
        [HttpPost]

        public IActionResult NotificationCount([FromBody] PostNotification notif)
        {
            return Ok(JsonConvert.SerializeObject(_GetSetSPI.GetSpAllItem<PostNotification>("NotificationFaultCount", _mapperservice.notificationcount, notif.deviceid, notif.notiftype)));
        }

    }
}
