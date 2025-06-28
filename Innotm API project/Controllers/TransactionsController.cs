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
            List<TransactionCustom> retlist = new List<TransactionCustom>();
            TransactionCustom obj = new TransactionCustom();
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.PhoneNumber == phoneNumber);
                if (user == null)
                {
                    response.Result = null;
                    response.Response = "User not found";
                    response.ResponseCode = "200";
                    return response;
                }
                else
                {
                    var transactions = _context.Transactions.Where(t => t.UserId == user.UserId).OrderByDescending(t => t.TransactionDate).ToList();
                    foreach (var transaction in transactions)
                    {
                        obj = new TransactionCustom();
                        var Receiver = _context.Users.Where(rec => rec.UserId == transaction.ReceiverId).FirstOrDefault();
                        obj.TransactionId = transaction.TransactionId;
                        obj.UserId = transaction.UserId;
                        obj.ReceiverId = transaction.ReceiverId;
                        obj.ReceiverName = Receiver.Username;
                        obj.ReceiverPhoneNumber = Receiver.PhoneNumber;
                        obj.TransactionType = transaction.TransactionType;
                        obj.TransactionDate = transaction.TransactionDate;
                        obj.InitialAmount = transaction.InitialAmount;
                        obj.TransferAmount = transaction.TransferAmount;

                        retlist.Add(obj);

                    }
                    response.Result = retlist;
                    response.Response = "Successfully fetched history";
                    response.ResponseCode = "200";
                    return response;

                }
                   
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Response =  ex.Message;
                response.ResponseCode = "400";
            }
            return response;
        }

        [HttpPost("pay")]
        public ApiResponse MakePayment(PayMoneyDto dto)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                var sender = _context.Users.FirstOrDefault(u => u.PhoneNumber == dto.senderPhoneNumber);
                if (sender == null)
                {
                    response.Result = null;
                    response.Response = "User not found";
                    response.ResponseCode = "200";
                    return response;
                }
                var receiver = _context.Users.FirstOrDefault(u => u.PhoneNumber == dto.receiverPhoneNumber);
                if (receiver == null)
                {
                    response.Result = null;
                    response.Response = "Receiver not found";
                    response.ResponseCode = "200";
                    return response;
                }
                if (sender.Amount < dto.Amount)
                {
                    response.Result = null;
                    response.Response = "Insufficient balance";
                    response.ResponseCode = "200";
                    return response;
                }
                decimal senderinitial = sender.Amount;
                decimal receiverinitial = receiver.Amount;
                sender.Amount -= dto.Amount;
                receiver.Amount += dto.Amount;
                var senderTransaction = new Transaction
                {
                    UserId = sender.UserId,
                    ReceiverId = receiver.UserId,
                    TransactionType = "Debit",
                    TransactionDate = DateTime.UtcNow,
                    InitialAmount = senderinitial,
                    TransferAmount = dto.Amount
                };
                var receiverTransaction = new Transaction
                {
                    UserId = receiver.UserId,
                    ReceiverId = sender.UserId,
                    TransactionType = "Credit",
                    TransactionDate = DateTime.UtcNow,
                    InitialAmount = receiverinitial,
                    TransferAmount = dto.Amount
                };
                _context.Transactions.Add(senderTransaction);
                _context.Transactions.Add(receiverTransaction);
                _context.SaveChanges();
                response.Result = null;

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

        [HttpDelete("DeleteTransactionById")]

        public TransactionResponse DeleteTransactionById(int Tid)
        {
            TransactionResponse response = new TransactionResponse();
            try
            {
                var transaction = _context.Transactions.FirstOrDefault(t => t.TransactionId == Tid);
                _context.Transactions.RemoveRange(transaction);
                _context.SaveChanges();
                response.Result = null;
                response.Response = "Transaction deleted successfully";
                response.ResponseCode = "200";
                return response;
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Response = "An error occurred: " + ex.Message;
                response.ResponseCode = "400";
            }
            return response;
        }

        [HttpDelete("DeleteHistory")]

        public TransactionResponse DeleteHistory(string phoneNumber)
        {
            TransactionResponse response = new TransactionResponse();
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.PhoneNumber == phoneNumber);
                if(user== null)
                {
                    response.Result = null;
                    response.Response = "User not found";
                    response.ResponseCode = "200";
                    return response;
                }
                var transactions = _context.Transactions.Where(t => t.UserId == user.UserId);
                _context.Transactions.RemoveRange(transactions);
                _context.SaveChanges();
                response.Result = null;
                response.Response = "All transactions deleted successfully";
                response.ResponseCode = "200";
                return response;
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Response =  ex.Message;
                response.ResponseCode = "400";
            }
            return response;
        }
    }
}
