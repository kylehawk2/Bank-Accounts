using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Bank_Accounts.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace Bank_Accounts.Controllers
{
    public class HomeController : Controller
    {
        private BankAccountContext dbContext;

        public HomeController (BankAccountContext context)
        {
            dbContext = context;
        }
        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("Login")]
        public IActionResult Login()
        {
            return View("Login");
        }

        [HttpPost]
        [Route("Register")]
        public IActionResult Register(User user)
        {
            if(ModelState.IsValid)
            {
                if(dbContext.users.Any(u => u.Email == user.Email))
                {
                    ModelState.AddModelError("Email", "Email is already in use!");
                }
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                user.Password = Hasher.HashPassword(user, user.Password);
                // Save your user to the database
                User NewUser = new User
                {
                    First_Name = @user.First_Name,
                    Last_Name = @user.Last_Name, 
                    Email = @user.Email,
                    Password = @user.Password,
                };
                dbContext.Add(NewUser);
                dbContext.SaveChanges();
                HttpContext.Session.SetInt32("logged_in_userID", NewUser.UserId); //Store logged in User's ID
                HttpContext.Session.SetString("logged_in_username", NewUser.First_Name); //Store logged in Users First name
                
                int? logged_in_user = HttpContext.Session.GetInt32("logged_in_userID");
                return Redirect($"Account/{logged_in_user}");
            }
            return View("");
        }

        [HttpPost]
        [Route("login")]
        public IActionResult login(User user)
        {
            if(ModelState.IsValid)
            {
                var userindb = dbContext.users.FirstOrDefault(u => u.Email == user.Email);

                if(userindb == null)
                {
                    ModelState.AddModelError("Email", "Invalid Email/Password");
                    return View("Login");
                }
                var hasher = new PasswordHasher<User>();
                var result = hasher.VerifyHashedPassword(user, userindb.Password, user.Password);

                if(result == 0)
                {
                    ModelState.AddModelError("Password", "Invalid Email/Password");
                    return View("Login");
                }
            
                HttpContext.Session.SetInt32("logged_in_userID", userindb.UserId); 
                HttpContext.Session.SetString("logged_in_username", userindb.First_Name); 
                
                int? logged_in_user = HttpContext.Session.GetInt32("logged_in_userID");

            return Redirect($"Account/{logged_in_user}");
            }
            else
            {
                return View("Login");
            }
        }

        [HttpPost]
        [Route("Account/{id}")]
        public IActionResult ShowAccount(int id)
        {
            if(HttpContext.Session.GetInt32("logged_in_userID") == null)
            {
                return RedirectToAction("Login");
            }
            List<Transaction> alltransactions = dbContext.transactions.Include(user => user.Creator).ToList();
            int logged_in_user = (int)HttpContext.Session.GetInt32("logged_in_userID");
            var current_user = dbContext.users.Include(user => user.Transactions).FirstOrDefault(user => user.UserId == logged_in_user);

            var transactions = current_user.Transactions;
            transactions.Reverse();
            decimal sum = 0;

            foreach(var i in transactions)
            {
                sum += i.Amount;
            }
            
            ViewBag.Balance = sum;

            
            ViewBag.AllTransactions = transactions;

            return View("AccountPage");
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        [Route("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
