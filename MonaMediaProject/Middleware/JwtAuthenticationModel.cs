namespace MonaMediaProject.Middleware
{
    public class JwtAuthenticationModel
    {
        public class LoginRequest
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }
    }
}
