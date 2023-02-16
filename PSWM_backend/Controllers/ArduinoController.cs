using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PSWM_backend.Model;

namespace PSWM_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArduinoController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IMappers _mapperservice;
        private readonly IGetSetSPI _GetSetSPI;
        private readonly IadditionalService _additionalService;
        public ArduinoController(IConfiguration configuration, IMappers mapperService, IGetSetSPI getSetSPI, IadditionalService additionalService)
        {
            _configuration = configuration;
            _mapperservice = mapperService;
            _GetSetSPI = getSetSPI;
            _additionalService = additionalService;
        }


        [Route("FetchDeviceInfo()")]
        [HttpPost]

        public string FetchDeviceInfo([FromBody] Arduino ard)
        {
            string replymsg = "";
            replymsg = JsonConvert.SerializeObject(_GetSetSPI.GetSpAllItem<Arduinoinfo>("FetchArduinoInfos", _mapperservice.ArdFetchDeviceInfo,ard.id ,ard.useraccount));
            return replymsg;
        }

        [Route("AddNotification()")]
        [HttpPost]

        public void AddNotification([FromBody] ArduinoNotif notif)
        {
            DateTime date = DateTime.Now;
            string read = "false";
            _GetSetSPI.PostSpAllItem<Arduinoinfo>("ADDNOTIFICATION", notif.deviceid, notif.text, notif.type,read,date,notif.value);
        }

        [Route("ADDFlowAndTurbidity()")]
        [HttpPost]


        public void ADDFlowAndTurbidity([FromBody] ArduinoFlowTurb flowturb)
         {
            _additionalService.CheckRemainingQuantity(flowturb.deviceid);
            DateTime date = DateTime.Now;
            string dateonly = date.ToShortDateString();
            string time = date.ToShortTimeString();

            _GetSetSPI.PostSpAllItem<Arduinoinfo>("ArdAddFlowTubidity", flowturb.deviceid,flowturb.flowvalue,flowturb.turbidityvalue,dateonly, time);

        }


    }
}
