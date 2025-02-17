using BlinkBank.Models;
using BlinkBank.Constants;
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

        // POST: Withdraw
        [HttpPost]
        public async Task<IActionResult> Withdraw(int accountId, decimal amount)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Invalid input. Please check your values.";
                return View();
            }

            if (amount <= 0)
            {
                ModelState.AddModelError("amount", "Withdrawal amount must be greater than zero.");
                return View();
            }

            var account = await _context.Accounts
                .Include(a => a.Transactions)
                .FirstOrDefaultAsync(a => a.Id == accountId);

            if (account == null)
            {
                TempData["ErrorMessage"] = "Account not found.";
                return NotFound();
            }

            decimal minBalance = FinancialConstants.MIN_ACCOUNT_BALANCE;

            if (account.InitialBalance < amount)
            {
                ModelState.AddModelError("amount", "Insufficient funds. You cannot withdraw more than your current balance.");
                return View();
            }

            if (account.InitialBalance - amount < minBalance)
            {
                ModelState.AddModelError("amount", $"Insufficient funds. Your balance cannot go below the minimum required: {minBalance}.");
                return View();
            }

            // تنفيذ السحب
            account.InitialBalance -= amount;

            var transaction = new Transactions
            {
                AccountId = account.Id,
                TransactionType = "withdraw",
                Amount = amount,
                Date = DateTime.Now
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Withdrawal successful.";
            return RedirectToAction("Index", "Transactions", new { accountId });
        }

        [HttpPost]
        public async Task<IActionResult> Deposit(int accountId, decimal amount)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Invalid input. Please check your values.";
                return View();
            }

            if (amount <= 0)
            {
                ModelState.AddModelError("amount", "Deposit amount must be greater than zero.");
                return View();
            }

            var account = await _context.Accounts
                .Include(a => a.Transactions)
                .FirstOrDefaultAsync(a => a.Id == accountId);

            if (account == null)
            {
                TempData["ErrorMessage"] = "Account not found.";
                return NotFound();
            }

            decimal maxDeposit = FinancialConstants.MAX_DEPOSIT_AMOUNT;

            if (amount > maxDeposit)
            {
                ModelState.AddModelError("amount", $"Deposit amount cannot exceed {maxDeposit}.");
                return View();
            }

            // تنفيذ الإيداع
            account.InitialBalance += amount;

            var transaction = new Transactions
            {
                AccountId = account.Id,
                TransactionType = "deposit",
                Amount = amount,
                Date = DateTime.Now
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Deposit successful.";
            return RedirectToAction("Index", "Transactions", new { accountId });
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
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Invalid input. Please check your values.";
                return View();
            }

            if (amount <= 0)
            {
                ModelState.AddModelError("amount", "Transfer amount must be greater than zero.");
                return View();
            }

            var sourceAccount = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == accountId);
            var targetAccount = await _context.Accounts.FirstOrDefaultAsync(a => a.AccountNumber == targetAccountNumber);

            if (sourceAccount == null)
            {
                ModelState.AddModelError("", "Source account not found.");
                return View();
            }

            if (targetAccount == null)
            {
                ModelState.AddModelError("targetAccountNumber", "Target account not found.");
                return View();
            }

            decimal minBalance = BlinkBank.Constants.FinancialConstants.MIN_ACCOUNT_BALANCE;

            // التحقق من توفر الرصيد الكافي
            if (sourceAccount.InitialBalance < amount)
            {
                ModelState.AddModelError("amount", "Insufficient funds. You cannot transfer more than your current balance.");
                return View();
            }

            // التأكد من أن الرصيد المتبقي لا ينخفض عن الحد الأدنى بعد التحويل
            if (sourceAccount.InitialBalance - amount < minBalance)
            {
                ModelState.AddModelError("amount", $"Insufficient funds. Your balance cannot go below the minimum required: {minBalance}.");
                return View();
            }

            // تنفيذ التحويل
            sourceAccount.InitialBalance -= amount;
            targetAccount.InitialBalance += amount;

            // تسجيل العمليات المالية
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


    }
}
