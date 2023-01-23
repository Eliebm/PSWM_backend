namespace PSWM_backend.Model
{
    public class login
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
}
