using Microsoft.AspNetCore.Mvc;
using ATMManagementApplication.Models;
using ATMManagementApplication.Data;
using System.Linq;
using System.Net;
using System.Net.Mail;

namespace ATMManagementApplication.Controllers
{
    [ApiController]
    [Route("api/atm")]
    public class ATMController : Controller
    {
        private readonly ATMContext _context;
        public ATMController(ATMContext context)
        {
            _context = context;
        }

        [HttpGet("Balance/{customerId}")]
        public IActionResult GetBalance(int customerId)
        {
            var customer = _context.customers.Find(customerId);
            if (customer == null) return NotFound("Customer not found");

            return Ok(new { Balance = customer.Balance });
        }

        [HttpPost("withdraw")]
        public IActionResult Withdraw([FromBody] WithdrawRequest request)
        {
            var customer = _context.customers.Find(request.CustomerId);
            if (customer == null) return NotFound("Customer not found");

            if (customer.Balance < request.Amount) return BadRequest("Insufficient balance");

            // Thực hiện rút tiền
            customer.Balance -= request.Amount;

            // Tạo đối tượng giao dịch mới
            var transaction = new Transaction
            {
                CustomerId = request.CustomerId,
                Amount = request.Amount,
                Timestamp = DateTime.Now,
                IsSuccessful = true,
                Description = "rut tien thanh cong"
            };

            // Thêm giao dịch vào cơ sở dữ liệu
            _context.transactions.Add(transaction);
            _context.SaveChanges();

            return Ok(new { Message = "Withdrawal successful", Balance = customer.Balance });
        }

        [HttpPost("deposit")]
        public IActionResult Deposit([FromBody] DepositRequest request)
        {
            var customer = _context.customers.Find(request.CustomerId);
            if (customer == null) return NotFound("Customer not found");

            // Thực hiện gửi tiền
            customer.Balance += request.Amount;

            // Tạo đối tượng giao dịch mới
            var transaction = new Transaction
            {
                CustomerId = request.CustomerId,
                Amount = request.Amount,
                Timestamp = DateTime.Now,
                IsSuccessful = true,
                Description = "gui tien thanh cong"
            };

            // Thêm giao dịch vào cơ sở dữ liệu
            _context.transactions.Add(transaction);
            _context.SaveChanges();

            return Ok(new { Message = "Deposit successful", Balance = customer.Balance });
        }



        // lich su giao  dich
        [HttpGet("transaction-history/{customerId}")]
        public IActionResult Transactionhistory(int customerId)
        {
            var customer = _context.customers.Find(customerId);
            if (customer == null) return NotFound("customer not found");

            var transactions = _context.transactions
                .Where(t => t.CustomerId == customerId)
                .OrderByDescending(t => t.Timestamp).ToList();

            return Ok(transactions);
        }


        // chuyen tien giua cac tai khoan
        [HttpPost("transfer")]
        public IActionResult Transfer([FromBody] TransferRequest request)
        {
            var sender = _context.customers.Find(request.SenderId);
            var receiver = _context.customers.Find(request.ReceiverId);

            if (sender == null || receiver == null) return NotFound("Sender or Receiver not found");
            if (sender.Balance < request.Amount) return BadRequest("Insufficient balance");

            // thuc hien chuyen tien
            sender.Balance -= request.Amount;
            receiver.Balance += request.Amount;

            // tao giao dich nguoi gui
            var senderTransaction = new Transaction
            {
                CustomerId = request.SenderId,
                Amount = -request.Amount,
                Timestamp = DateTime.Now,
                IsSuccessful = true,
                Description = "Transfer to " + receiver.Name

            };
            _context.transactions.Add(senderTransaction);

            // tao giao dich nguoi gui
            var receiverTransaction = new Transaction
            {
                CustomerId = request.ReceiverId,
                Amount = request.Amount,
                Timestamp = DateTime.Now,
                IsSuccessful = true,
                Description = "Transfer from " + sender.Name

            };
            _context.transactions.Add(receiverTransaction);

            _context.SaveChanges();

            // Gửi email cho người gửi
            SendEmail(sender.Email, "Transfer Confirmation", $"You have successfully transferred {request.Amount} to {receiver.Name}.");

            // Gửi email cho người nhận
            SendEmail(receiver.Email, "Money Received", $"You have received {request.Amount} from {sender.Name}.");


            return Ok(new { Message = "Transfer successful", SenderBalance = sender.Balance, ReceiverBalance = receiver.Balance });

        }


        // gui email giao dich
        private void SendEmail(string toEmail, string subject, string body)
        {
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("hongphuc0835@gmail.com", "joia vkwu vppg pdvu"),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("hongphuc0835@gmail.com"),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };
            mailMessage.To.Add(toEmail);

            smtpClient.Send(mailMessage);
        }


    }

    public class WithdrawRequest
    {
        public int CustomerId { get; set; }
        public decimal Amount { get; set; }
    }

    public class DepositRequest
    {
        public int CustomerId { get; set; }
        public decimal Amount { get; set; }
    }

    public class TransferRequest
    {
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public decimal Amount { get; set; }
    }
}
