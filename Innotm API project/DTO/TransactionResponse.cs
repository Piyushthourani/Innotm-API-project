namespace Innotm_API_project.DTO
{
    public class TransactionCustom
    {
        public int TransactionId { get; set; }
        public int UserId { get; set; }
        public int ReceiverId { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverPhoneNumber { get; set; }
        public string TransactionType { get; set; } 
        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
        public decimal InitialAmount { get; set; }
        public decimal TransferAmount { get; set; }
    }
    public class TransactionResponse
    {
        public List<TransactionCustom> Result { get; set; }
        public string Response { get; set; }
        public string ResponseCode { get; set; }
    }
}
