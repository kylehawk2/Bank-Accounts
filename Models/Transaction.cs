using System;
using System.ComponentModel.DataAnnotations;

namespace Bank_Accounts.Models
{
    public class Transaction
    {
        [Key]
        public int TransactionId {get;set;}
        [Required]
        public int Amount {get;set;}
        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public User Creator {get;set;}
    }
}