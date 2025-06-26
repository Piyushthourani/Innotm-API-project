using Innotm_API_project.Data;
using Innotm_API_project.DTO;
using Microsoft.AspNetCore.Mvc;

namespace Innotm_API_project.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly AppDbContext _context;
        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("all")]
        public ApiResponse GetAllUsers(string phoneNumber)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.PhoneNumber == phoneNumber);
                if (user == null || user.IsAdmin==false)
                {
                    response.Result = null;
                    response.Response = "Only admins can access this";
                    response.ResponseCode = "401";
                    return response;
                }
                else
                {
                    response.Result = user;
                    response.Response = "Records fetched successfully";
                    response.ResponseCode = "200";
                }
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Response = "Bad Request!" + ex.Message;
                response.ResponseCode = "400";
            }
            return response;
        }

        /*
         * Hum yaha balance k liye alag se walletresponse class bana kr bhi kar sakte hai
         * Jisme sirf Wallet ka balance aaega 
         * yaha hum user ka pura data fetch kr rhe hai usme se amount frontend pr show krwa denge
          */

        [HttpGet("balance")]
        public ApiResponse GetBalance(string phoneNumber)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.PhoneNumber == phoneNumber);
                if (user == null)
                {
                    response.Result = null;
                    response.Response = "Only admins can access this";
                    response.ResponseCode = "401";
                    return response;
                }
                else
                {
                    response.Result = user;
                    response.Response = "Records fetched successfully";
                    response.ResponseCode = "200";
                }
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Response = "Bad Request!" + ex.Message;
                response.ResponseCode = "400";
            }
            return response;
        }
        
    }
}
