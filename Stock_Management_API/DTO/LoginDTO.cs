namespace Stock_Management_API.DTO
{
    public class LoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }

        public LoginDto()
        {
            Email = "";
            Password = "";
        }
    }
}
