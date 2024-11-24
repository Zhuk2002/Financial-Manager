using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;

namespace FinancialManager.Controllers
{
    public class TransactionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;


        public TransactionsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult UpdateBalanceView()
        {
            ViewBag.showUpdateBalance = true;
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateBalance(decimal amount)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("User not found");
            }

            user.Balance = amount;
            await _userManager.UpdateAsync(user);
            ViewBag.showUpdateBalance = false;

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var transactions = await _context.Transactions
                .Where(t => t.UserId == userId)
                .ToListAsync();
            return View(transactions);
        }

        // GET: Transactions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // GET: Transactions/Create
        public IActionResult Create()
        {
            var transaction = new Transaction
            {
                Date = DateTime.Now,
                UserId = _userManager.GetUserId(User)
            };
            return View(transaction);
        }

        // POST: Transactions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Amount,Description,Date,IsIncome,UserId")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return NotFound("User not found");

                user.Balance = transaction.IsIncome ? user.Balance + transaction.Amount : user.Balance - transaction.Amount;

                _context.Add(transaction);
                await _userManager.UpdateAsync(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(transaction);
        }

        // GET: Transactions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions.FindAsync(id);
            Console.WriteLine();
            if (transaction == null)
            {
                return NotFound();
            }
            transaction.UserId = _userManager.GetUserId(User);
            return View(transaction);
        }

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Amount,Description,Date,IsIncome,UserId")] Transaction transaction)
        {
            if (id != transaction.Id)
            {
                return NotFound();
            }
            transaction.UserId = _userManager.GetUserId(User);
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound("User not found");

            if (ModelState.IsValid)
            {
                try
                {
                    var currentTransaction = await _context.Transactions.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);
                    if (currentTransaction != null)
                    {
                        decimal deltaAmount = (transaction.IsIncome == currentTransaction.IsIncome) ? transaction.Amount - currentTransaction.Amount : transaction.Amount + currentTransaction.Amount;
                        user.Balance = transaction.IsIncome ? user.Balance + deltaAmount : user.Balance - deltaAmount;
                    }
                    _context.Update(transaction);
                    await _userManager.UpdateAsync(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TransactionExists(transaction.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }
            return View(transaction);
        }

        // GET: Transactions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound("User not found");
            
            if (transaction != null)
            {
                user.Balance = transaction.IsIncome ? user.Balance - transaction.Amount : user.Balance + transaction.Amount;
                _context.Transactions.Remove(transaction);
            }

            await _userManager.UpdateAsync(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TransactionExists(int id)
        {
            return _context.Transactions.Any(e => e.Id == id);
        }
    }
}
