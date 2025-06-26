namespace Innotm_API_project.DTO
{
    public class SignupDto
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Gender { get; set; }
        public string Password { get; set; }
        public string ImageUrl { get; set; }
        public bool IsAdmin { get; set; } = false;

    }
}
