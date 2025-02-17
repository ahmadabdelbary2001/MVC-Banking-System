using BlinkBank.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BlinkBank.Controllers
{
    [Authorize]
    public class TransactionsController : Controller
    {
        private readonly BankDBContext _context;
        private const decimal MaxDepositAmount = 10000; // Max deposit limit
        private const decimal MinBalanceForWithdrawal = 50; // Min balance after withdrawal

        public TransactionsController(BankDBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Accounts account = await _context.Accounts.FirstOrDefaultAsync(a => a.ApplicationUserId == userId);

            if (account == null)
            {
                return RedirectToAction("CreateAccount", "AccountManagement");
            }

            return View(account);
        }

        // GET: Withdraw
        [HttpGet]
        public async Task<IActionResult> Withdraw(int accountId)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == accountId);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        [HttpGet]
        public async Task<IActionResult> Deposit(int accountId)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == accountId);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        // POST: Withdraw or Deposit
        [HttpPost]
        public async Task<IActionResult> Withdraw(int accountId, decimal amount)
        {
            if (amount <= 0) // Check for negative amount
            {
                TempData["ErrorMessage"] = "You should not put a negative amount.";
                return View(); // Return the view with the error message
            }

            if (ModelState.IsValid)
            {
                var account = await _context.Accounts.Include(a => a.Transactions).FirstOrDefaultAsync(a => a.Id == accountId);
                if (account == null)
                {
                    TempData["ErrorMessage"] = "Account not found.";
                    return NotFound();
                }

                if (account.InitialBalance < amount)
                {
                    TempData["ErrorMessage"] = "Insufficient funds. You cannot withdraw more than your current balance.";
                    return View();
                }

                if (account.InitialBalance - amount < MinBalanceForWithdrawal)
                {
                    TempData["ErrorMessage"] = $"Insufficient funds. Minimum balance of {MinBalanceForWithdrawal} must remain.";
                    return View();
                }

                account.InitialBalance -= amount;

                // Log the transaction
                var transaction = new Transactions
                {
                    AccountId = account.Id,
                    TransactionType = "withdraw",
                    Amount = amount,
                    Date = DateTime.Now
                };
                _context.Update(account);
                _context.Transactions.Add(transaction);

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Transaction successful.";
                return RedirectToAction("Index", "Transactions", new { accountId });
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Deposit(int accountId, decimal amount)
        {
            if (amount <= 0) // Check for negative amount
            {
                TempData["ErrorMessage"] = "You should not put a negative amount.";
                return View(); // Return the view with the error message
            }

            if (ModelState.IsValid)
            {
                var account = await _context.Accounts.Include(a => a.Transactions).FirstOrDefaultAsync(a => a.Id == accountId);
                if (account == null)
                {
                    TempData["ErrorMessage"] = "Account not found.";
                    return NotFound();
                }

                if (amount > MaxDepositAmount)
                {
                    TempData["ErrorMessage"] = $"Deposit cannot exceed {MaxDepositAmount}.";
                    return View();
                }

                account.InitialBalance += amount;

                // Log the transaction
                var transaction = new Transactions
                {
                    AccountId = account.Id,
                    TransactionType = "deposit",
                    Amount = amount,
                    Date = DateTime.Now
                };
                _context.Transactions.Add(transaction);

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Transaction successful.";
                return RedirectToAction("Index", "Transactions", new { accountId });
            }

            return View();
        }


        // GET: Transfer
        [HttpGet]
        public async Task<IActionResult> Transfer(int accountId)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == accountId);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        // POST: Transfer
        [HttpPost]
        public async Task<IActionResult> Transfer(int accountId, string targetAccountNumber, decimal amount)
        {
            if (amount <= 0) // Check for negative amount
            {
                TempData["ErrorMessage"] = "You should not put a negative amount.";
                return View(); // Return the view with the error message
            }

            if (ModelState.IsValid)
            {
                var sourceAccount = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == accountId);
                var targetAccount = await _context.Accounts.FirstOrDefaultAsync(a => a.AccountNumber == targetAccountNumber);

                if (sourceAccount == null || targetAccount == null)
                {
                    TempData["ErrorMessage"] = "Source account or target account not found.";
                    return View();
                }

                // Check if the source account has sufficient balance
                if (sourceAccount.InitialBalance < amount)
                {
                    TempData["ErrorMessage"] = "Insufficient funds. You cannot transfer more than your current balance.";
                    return View();
                }

                // Ensure the minimum balance requirement is met after the transfer
                if (sourceAccount.InitialBalance - amount < MinBalanceForWithdrawal)
                {
                    TempData["ErrorMessage"] = $"Insufficient funds. Minimum balance of {MinBalanceForWithdrawal} must remain.";
                    return View();
                }

                sourceAccount.InitialBalance -= amount;
                targetAccount.InitialBalance += amount;

                // Log the transactions
                var sourceTransaction = new Transactions
                {
                    AccountId = sourceAccount.Id,
                    TransactionType = "transfer_out",
                    Amount = amount,
                    targetAccountNumber = targetAccount.AccountNumber,
                    Date = DateTime.Now
                };
                var targetTransaction = new Transactions
                {
                    AccountId = targetAccount.Id,
                    TransactionType = "transfer_in",
                    Amount = amount,
                    Date = DateTime.Now,
                    sourceAccountNumber = sourceAccount.AccountNumber
                };
                _context.Update(sourceAccount);
                _context.Update(targetAccount);
                _context.Transactions.Add(sourceTransaction);
                _context.Transactions.Add(targetTransaction);

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Transfer successful.";
                return RedirectToAction("Index", "Transactions", new { accountId = sourceAccount.Id });
            }

            return View();
        }

    }
}
