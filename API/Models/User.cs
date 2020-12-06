namespace API.Models
{
    public class User
    {
        public string UserName { get; set; }
        public string Token { get; set; }
        public string PhotoUrl { get; internal set; }
        public string KnownAs { get; internal set; }
    }
}