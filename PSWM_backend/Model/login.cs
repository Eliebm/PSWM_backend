﻿namespace PSWM_backend.Model
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

    public class Notification
    {
        public int id { get; set; }
        public string? deviceid { get; set; }
        public string? text { get; set; }
        public string? notiftype { get; set; }
        public string? iread { get; set; }
        public string? date { get; set; }  
    }

    public class PostNotification
    {
        public int id { get; set; }
        public string? deviceid { get; set; }
        public string? notiftype { get; set; }
    }

    public class DeleteMessage
    {
        public int id { get; set; }
    }

    public class PostFetchAdminUser
    {
      public  int cityid { get; set; }
    }
    
}
