namespace PSWM_backend.Model
{
    public class Login
    {   
        public string? userName { get; set; }
        public string? password { get; set; }
    }
    public class AuthenticatedResponse
    {
        public string? ID { get; set; }
        public string? accessToken { get; set; }
        public string? refreshToken { get; set; }
        public string? issueDate { get; set; }
        public string? expireDate { get; set; }

    }

    public class Refreshtok
    {
        public string? id { get; set; }
        public string? token { get; set; }

        public int response { get; set; }
    }

    public class Admin
    {
        public int adminid { get; set; }
        public string? adminName { get; set; }
        public string? adminPass { get; set; }

    }

    public class Signup
    {
        public string? account { get; set; }
        public string? name { get; set; }
        public string? lname { get; set; }
        public string? phone { get; set; }
        public string? email { get; set; }

        public string? pass { get; set; }


    }

    public class User
    {   public string? id { get; set; }  
        public string? account { get; set; }
        public string? name { get; set; }
        public string? password { get; set; }
        public string? phone { get; set; }
        public string? email { get; set; }



    }

    public class ChangePassword
    {
        public string? id { get; set; }
        public string? password { get; set; }

    }
}
