using Microsoft.AspNetCore.Mvc;
using ATMManagementApplication.Models;
using ATMManagementApplication.Data;
using System.Linq;

namespace ATMManagementApplication.Controllers{

    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase{
        private readonly ATMContext _context;

        public AuthController( ATMContext context ){
            _context = context;
        }


        [HttpPost("Login")]
        public IActionResult Login([FromBody] Customer login){
            var customer = _context.customers.FirstOrDefault(c => c.Name == login.Name && c.Password == login.Password);

            if (customer == null){
                return Unauthorized("Invalid credentials");
            }
            return Ok(new{message = "login successful", customerId = customer.CustomerId});

        }

        //dang ky nguoi dung
        [HttpPost("register")]
        public IActionResult Register([FromBody] Customer register){
            var existingCustomer = _context.customers.FirstOrDefault(c => c.Email == register.Email); // kiem tra xem email da ton tai chua
            if(existingCustomer != null) return BadRequest("email da duoc su dung");

            var newCustomer = new Customer {
                Name = register.Name,
                Email = register.Email,
                Password = register.Password,
                Balance = 0
                };
                _context.customers.Add(newCustomer);
                _context.SaveChanges();

                return Ok(new{Message = "registrantion successful"});
        }

    }
}