using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Lab4N.Data;
using Lab4N.Models;
using Lab4N.Models.ViewModels;

namespace Lab4N.Controllers
{
    public class BrokeragesController : Controller
    {
        private readonly MarketDbContext _context;

        public BrokeragesController(MarketDbContext context)
        {
            _context = context;
        }

        // GET: Brokerages
        public async Task<IActionResult> Index(String ID)
        {
            BrokeragesViewModel ViewModel = new BrokeragesViewModel();

            ViewModel.Brokerages = await _context.Brokerages
                   .Include(i => i.Subscriptions)
                        .ThenInclude(i => i.Clients)
                  .AsNoTracking()
                  .OrderBy(i => i.Title)
                  .ToListAsync();

            if (ID != null)
            {
                ViewData["BrokerageID"] = ID;
                ViewModel.Subscriptions = ViewModel.Brokerages.Where(i => i.Id == ID).Single().Subscriptions;
            }

            return View(ViewModel);
        }
        // GET: Brokerages/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brokerage = await _context.Brokerages
                .FirstOrDefaultAsync(m => m.Id == id);
            if (brokerage == null)
            {
                return NotFound();
            }

            return View(brokerage);
        }

        // GET: Brokerages/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Brokerages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Fee")] Brokerage brokerage)
        {
            if (ModelState.IsValid)
            {
                _context.Add(brokerage);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(brokerage);
        }

        // GET: Brokerages/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brokerage = await _context.Brokerages.FindAsync(id);
            if (brokerage == null)
            {
                return NotFound();
            }
            return View(brokerage);
        }

        // POST: Brokerages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Title,Fee")] Brokerage brokerage)
        {
            if (id != brokerage.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(brokerage);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BrokerageExists(brokerage.Id))
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
            return View(brokerage);
        }

        // GET: Brokerages/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brokerage = await _context.Brokerages
                .FirstOrDefaultAsync(m => m.Id == id);
            if (brokerage == null)
            {
                return NotFound();
            }
            ViewBag.advertisementsExist = _context.AdvertisementBrokerage.Where(m => m.BrokerageId == id).Count() != 0;

            return View(brokerage);
        }

        // POST: Brokerages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var brokerage = await _context.Brokerages.FindAsync(id);
            _context.Brokerages.Remove(brokerage);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BrokerageExists(string id)
        {
            return _context.Brokerages.Any(e => e.Id == id);
        }
    }
}