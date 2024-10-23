using Microsoft.EntityFrameworkCore;
using ATMManagementApplication.Models;
using System.Security.Cryptography.X509Certificates;

namespace ATMManagementApplication.Data{

    public class ATMContext : DbContext{
        public ATMContext(DbContextOptions<ATMContext> options):base(options) {}

        public DbSet<Customer> customers{ get; set; }
        public DbSet<Transaction> transactions{ get; set; }
    }
}