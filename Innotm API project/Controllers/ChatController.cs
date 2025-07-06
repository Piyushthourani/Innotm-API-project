using Innotm_API_project.Data;
using Innotm_API_project.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using OpenAI_API;
using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Innotm_API_project.Controllers
{
    [Microsoft.AspNetCore.Components.Route("api/[controller]")]
    [ApiController]
    public class ChatController:ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public ChatController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpGet("ask")]
        public async Task<IActionResult> Ask([FromQuery]string question, [FromQuery] string phoneNumber)
        {
            if(string.IsNullOrWhiteSpace(phoneNumber))
                return BadRequest("Phone number is required.");

            var user = _context.Users.FirstOrDefault(u => u.PhoneNumber == phoneNumber);
            if(user == null)
            {
                return NotFound("User not found.");
            }

            var originalAmount = user.Amount;
            string systemReply = "";

            //Step 1: Check if its a transfer request
            if(question.ToLower().Contains("transfer") || question.ToLower().Contains("send"))
            {
                var normalized = question.ToLower();

                //Match amount + phonenumber
                var phoneMatch = Regex.Match(normalized, @"(?:transfer|send|please\s*transfer|can you transfer|please send)?\D*(\d+)\D*(\d{10})");
                
                //Match amount + username
                var usernameMatch = Regex.Match(normalized, @"(?:transfer|send|please\s*transfer|can you transfer|please send)?\D*(\d+)\D*(?:to\s+)?([a-zA-Z0-9_]+)");

                decimal amountToTransfer = 0;
                string recipientPhone = null;
                string recipientUsername = null;

                if(phoneMatch.Success)
                {
                    amountToTransfer = decimal.Parse(phoneMatch.Groups[1].Value);
                    recipientPhone = phoneMatch.Groups[2].Value;
                }
                else if(usernameMatch.Success)
                {
                    amountToTransfer = decimal.Parse(usernameMatch.Groups[1].Value);
                    recipientUsername = usernameMatch.Groups[2].Value;
                }
                else
                {
                    return BadRequest("Invalid transfer request format. Try : 'Transfer 100 to 9876543210' or ' send 100 to ritesh'.");
                }

                if(amountToTransfer <= 0)
                {
                    return BadRequest("Amount to transfer must be greater than zero.");
                }
                if(user.Amount < amountToTransfer)
                {
                    return BadRequest("Insufficient balance for transfer.");
                }

                //Find recipient by username or phone number
                 User recipient = null;
                if(!string.IsNullOrEmpty(recipientPhone))
                {
                    recipient = _context.Users.FirstOrDefault(u => u.PhoneNumber == recipientPhone);
                }
                else if(!string.IsNullOrEmpty(recipientUsername))
                {
                    recipient = _context.Users.FirstOrDefault(u => u.Username.ToLower()==recipientUsername.ToLower());
                }
                if(recipient == null)
                {
                    return NotFound("Recipient not found.");
                }

                user.Amount -= amountToTransfer;
                recipient.Amount += amountToTransfer;
                var senderTransaction = new Transaction
                {
                    UserId = user.UserId,
                    ReceiverId = recipient.UserId,
                    TransactionType = "Debit",
                    TransactionDate = DateTime.UtcNow,
                    InitialAmount = user.Amount + amountToTransfer,
                    TransferAmount =amountToTransfer
                };
                var receiverTransaction = new Transaction
                {
                    UserId = recipient.UserId,
                    ReceiverId = user.UserId,
                    TransactionType = "Credit",
                    TransactionDate = DateTime.UtcNow,
                    InitialAmount = recipient.Amount - amountToTransfer,
                    TransferAmount = amountToTransfer
                };
                _context.Transactions.Add(senderTransaction);
                _context.Transactions.Add(receiverTransaction);
                _context.SaveChanges();

                systemReply = $"{amountToTransfer} has been successfully transferred to " + 
                              $"{(recipientPhone ?? recipient.Username)}. " +
                              $"Your new balance is {user.Amount}.";    

            }

            //Step 2: Prepare data for assistant reply
            var userInfoJson = JsonSerializer.Serialize(new
            {
                user.Username,
                user.PhoneNumber,
                AmountInRupees = $"{user.Amount} Rupees"
            });

            // Replace this line with your OpenRouter API key config key
            var openRouterApiKey = _config["OpenRouter:ApiKey"];
            if (string.IsNullOrWhiteSpace(openRouterApiKey))
            {
                return StatusCode(500, "OpenRouter API key not configured.");
            }

            // Create the request payload
            var messages = new[]
            {
                new { role = "system", content = "You are a helpful assistant. Answer the user's question based on their wallet balance and help transfer money to other users." },
                new { role = "user", content = $"User Asked: \"{question}\". The user info is: {userInfoJson}. {systemReply}" }
            };

            var payload = new
            {
                model = "deepseek/deepseek-r1-0528-qwen3-8b", // DeepSeek Qwen3 8B
                messages = messages,
                temperature = 0.7
            };

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", openRouterApiKey);
            httpClient.DefaultRequestHeaders.Add("HTTP-Referer", "localhost"); // Required by OpenRouter
            httpClient.DefaultRequestHeaders.Add("X-Title", "Innotm AI Assistant"); // Optional title for tracking

            var requestContent = new StringContent(JsonSerializer.Serialize(payload), System.Text.Encoding.UTF8, "application/json");

            var httpResponse = await httpClient.PostAsync("https://openrouter.ai/api/v1/chat/completions", requestContent);

            if (!httpResponse.IsSuccessStatusCode)
            {
                var errorContent = await httpResponse.Content.ReadAsStringAsync();
                return StatusCode((int)httpResponse.StatusCode, $"OpenRouter API error: {errorContent}");
            }

            var jsonResponse = await httpResponse.Content.ReadAsStringAsync();
            using var document = JsonDocument.Parse(jsonResponse);

            var reply = document.RootElement
                                .GetProperty("choices")[0]
                                .GetProperty("message")
                                .GetProperty("content")
                                .GetString();

            return Ok(new { reply = reply });




        }
    }
}
