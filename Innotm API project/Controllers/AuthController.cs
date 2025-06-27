using Innotm_API_project.Data;
using Innotm_API_project.DTO;
using Innotm_API_project.Models;
using Microsoft.AspNetCore.Mvc;

namespace Innotm_API_project.Controllers
{
    [ApiController]
    [Route("api/[controller]")] 
    public class AuthController : Controller
    {
        private readonly AppDbContext _context;
        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("signup")]
        public ApiResponse Signup(SignupDto dto)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                if (_context.Users.Any(u => u.PhoneNumber == dto.PhoneNumber))
                {
                    response.Result = null;
                    response.Response = "This phone number is already registered";
                    response.ResponseCode = "200";
                    return response;
                }
                var user = new User
                {
                    Username = dto.Username,
                    Email = dto.Email,
                    PhoneNumber = dto.PhoneNumber,
                    Gender = dto.Gender,
                    Password = dto.Password,
                    ImageUrl = dto.ImageUrl,
                    Amount = 0, // Default amount set to 0
                    IsAdmin = dto.IsAdmin
                };
                try
                {
                    _context.Users.Add(user);
                    _context.SaveChanges();

                    response.Result = user;
                    response.Response = "Registered successfully";
                    response.ResponseCode = "200";
                }
                catch (Exception ex)
                {
                    response.Result = null;
                    response.Response = "Bad Request! " + ex.Message;
                    response.ResponseCode = "400";
                }
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Response = "Bad Request! " + ex.Message;
                response.ResponseCode = "400";
            }
            return response;
        }

        [HttpPost("login")]
        public ApiResponse Login(LoginDto dto)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.PhoneNumber == dto.PhoneNumber && u.Password == dto.Password);
                if (user == null)
                {
                    response.Result = null;
                    response.Response = "Invalid phone number or password";
                    response.ResponseCode = "401";
                    return response;
                }
                else
                {
                    response.Result = user;
                    response.Response = "Login Successfully!";
                    response.ResponseCode = "200";
                }
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Response = "Bad Request! " + ex.Message;
                response.ResponseCode = "400";
            }
            return response;
        }
    }
}
