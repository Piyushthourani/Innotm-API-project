namespace Innotm_API_project.DTO
{
    public class  CustomDto
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string PhoneNumber { get; set; }
    }
    public class UserResponse
    {
        public List<CustomDto> Result { get; set; }
        public string Response { get; set; }
        public string ResponseCode { get; set; }

    }
}
