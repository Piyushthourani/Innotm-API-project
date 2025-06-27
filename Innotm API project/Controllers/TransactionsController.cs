using Innotm_API_project.Data;
using Innotm_API_project.DTO;
using Innotm_API_project.Models;
using Microsoft.AspNetCore.Mvc;

namespace Innotm_API_project.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionsController : Controller
    {
        private readonly AppDbContext _context;
        public TransactionsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("history")]
        public TransactionResponse GetHistory(string phoneNumber)
        {
            TransactionResponse response = new TransactionResponse();
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.PhoneNumber == phoneNumber);
                if(user==null)
                {
                    response.Result = null;
                    response.Response = "User not found";
                    response.ResponseCode = "404";
                    return response;
                }
                var transactions = _context.Transactions.Where(t => t.UserId == user.UserId || t.ReceiverId == user.UserId).OrderByDescending(t => t.TransactionDate).ToList();
                response.Result = transactions.Select(t => new TransactionCustom
                {
                    TransactionId = t.TransactionId,
                    UserId = t.UserId,
                    ReceiverId = t.ReceiverId,
                    ReceiverName = user.Username,
                    ReceiverPhoneNumber = user.PhoneNumber,
                    TransactionType = t.TransactionType,
                    TransactionDate = t.TransactionDate,
                    InitialAmount = t.InitialAmount,
                    TransferAmount = t.TransferAmount,
                }).ToList();
                response.Response = "Successfully fetched history";
                response.ResponseCode = "200";
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Response = "An error occured " + ex.Message;
                response.ResponseCode = "400";
            }
            return response;
        }

        [HttpPost("pay")]
        public TransactionResponse MakePayment(PayMoneyDto dto)
        {
            TransactionResponse response = new TransactionResponse();
            try
            {
                var sender = _context.Users.FirstOrDefault(u => u.PhoneNumber == dto.senderPhoneNumber);
                if (sender == null)
                {
                    response.Result = null;
                    response.Response = "User not found";
                    response.ResponseCode = "404";
                    return response;
                }
                var receiver = _context.Users.FirstOrDefault(u => u.PhoneNumber == dto.receiverPhoneNumber);
                if (receiver == null)
                {
                    response.Result = null;
                    response.Response = "Receiver not found";
                    response.ResponseCode = "404";
                    return response;
                }
                if (sender.Amount < dto.Amount)
                {
                    response.Result = null;
                    response.Response = "Insufficient balance";
                    response.ResponseCode = "400";
                    return response;
                }
                sender.Amount -= dto.Amount;
                receiver.Amount += dto.Amount;
                var transaction = new Transaction
                {
                    UserId = sender.UserId,
                    ReceiverId = receiver.UserId,
                    TransactionType = "Debit",
                    TransactionDate = DateTime.UtcNow,
                    InitialAmount = sender.Amount + dto.Amount,
                    TransferAmount = dto.Amount
                };
                _context.Transactions.Add(transaction);
                _context.SaveChanges();
                response.Result = new List<TransactionCustom>
                {
                    new TransactionCustom
                    {
                        TransactionId = transaction.TransactionId,
                        UserId = transaction.UserId,
                        ReceiverId = transaction.ReceiverId,
                        ReceiverName = receiver.Username,
                        ReceiverPhoneNumber = receiver.PhoneNumber,
                        TransactionType = transaction.TransactionType,
                        TransactionDate = transaction.TransactionDate,
                        InitialAmount = transaction.InitialAmount,
                        TransferAmount = transaction.TransferAmount
                    }
                };
                
                response.Response = "Payment successful";
                response.ResponseCode = "200";
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Response = "An error occurred: " + ex.Message;
                response.ResponseCode = "400";
            }
            return response;
        }

    }
}
