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
        public ArduinoController(IConfiguration configuration, IMappers mapperService, IGetSetSPI getSetSPI)
        {
            _configuration = configuration;
            _mapperservice = mapperService;
            _GetSetSPI = getSetSPI;

        }


        [Route("FetchDeviceInfo()")]
        [HttpPost]

        public string FetchDeviceInfo([FromBody] Arduino ard)
        {
            string replymsg = "";
            replymsg = JsonConvert.SerializeObject(_GetSetSPI.GetSpAllItem<Arduinoinfo>("FetchArduinoInfos", _mapperservice.ArdFetchDeviceInfo,ard.id ,ard.useraccount));
            return replymsg;
        }



    }
}
