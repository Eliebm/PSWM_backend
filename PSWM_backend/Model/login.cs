namespace PSWM_backend.Model
{
    public class login
    {
        public string? UserName { get; set; }
        public string? Password { get; set; }
    }
    public class AuthenticatedResponse
    {
        public int ID { get; set; }
        public string? accessToken { get; set; }
        public string? refreshToken { get; set; }
        public string? issueDate { get; set; }
        public string? expireDate { get; set; }

    }
}
