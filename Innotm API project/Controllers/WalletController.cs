using Innotm_API_project.Data;
using Innotm_API_project.DTO;
using Innotm_API_project.Models;
using Microsoft.AspNetCore.Mvc;

namespace Innotm_API_project.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WalletController : Controller
    {
        private readonly AppDbContext _context;
        public WalletController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("add")]
        public AddMoneyResponse AddMoney(AddMoneyDto dto)
        {
            AddMoneyResponse response = new AddMoneyResponse();
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.PhoneNumber == dto.phoneNumber);
                if(user == null)
                {
                    response.Response = "User not found";
                    response.ResponseCode = "200";
                    response.Amount = 0;
                    return response;
                }

                decimal initialAmount = user.Amount;
                user.Amount += dto.Amount;

                var transaction = new Transaction
                {
                    UserId = user.UserId,
                    ReceiverId = user.UserId,
                    TransactionType = "Wallet",
                    TransactionDate = DateTime.UtcNow,
                    InitialAmount = initialAmount,
                    TransferAmount = dto.Amount
                };
                try
                {
                    _context.Transactions.Add(transaction);
                    _context.SaveChanges();

                    response.Amount = user.Amount;
                    response.Response = "Amount added succesfully!";
                    response.ResponseCode = "200";
                }
                catch (Exception ex)
                {
                    response.Amount = 0;
                    response.Response = "Bad Request !"+ex.Message;
                    response.ResponseCode = "400";
                }
                
            }
            catch (Exception ex)
            {
                response.Amount = 0;
                response.Response = "An error occured " + ex.Message;
                response.ResponseCode = "400";
            }
            return response;

        }
    }
}
