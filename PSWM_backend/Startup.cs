using PSWM_backend.Controllers;

namespace PSWM_backend_project
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        
        public Startup(IConfiguration configuration) 
        {
            _configuration = configuration;
           
        }
    }
}
