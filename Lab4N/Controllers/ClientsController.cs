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
    public class ClientsController : Controller
    {
        private readonly MarketDbContext _context;

        public ClientsController(MarketDbContext context)
        {
            _context = context;
        }

        // GET: Clients
        public async Task<IActionResult> Index(int ID)
        {
            BrokeragesViewModel clientViewModel = new BrokeragesViewModel();

            clientViewModel.Clients = await _context.Clients
                .Include(i => i.Subscriptions)
                .ThenInclude(i => i.Brokerages)
                .AsNoTracking()
                .ToListAsync()
            ;

            if (ID != 0)
            {
                ViewData["ClientID"] = ID;
                clientViewModel.Subscriptions = clientViewModel.Clients.Where(i => i.Id == ID).Single().Subscriptions;
            }

            return View(clientViewModel);
        }

        // GET: Clients/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Clients
                .FirstOrDefaultAsync(m => m.Id == id);
            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // GET: Clients/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Clients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,LastName,FirstName,BirthDate")] Client client)
        {
            if (ModelState.IsValid)
            {
                _context.Add(client);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(client);
        }

        // GET: Clients/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Clients.FindAsync(id);
            if (client == null)
            {
                return NotFound();
            }
            return View(client);
        }

        // POST: Clients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,LastName,FirstName,BirthDate")] Client client)
        {
            if (id != client.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(client);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClientExists(client.Id))
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
            return View(client);
        }

        // GET: Clients/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Clients
                .FirstOrDefaultAsync(m => m.Id == id);
            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // POST: Clients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClientExists(int id)
        {
            return _context.Clients.Any(e => e.Id == id);
        }
        // GET: Clients/EditSubscriptions/6
        public async Task<IActionResult> EditSubscriptions(int id)
        {
            BrokeragesViewModel ViewModel = new BrokeragesViewModel();

            ViewModel.Subscriptions = await _context.Subscriptions.Where(i => i.ClientId == id).ToListAsync();
            ViewModel.Clients = await _context.Clients.Where(i => i.Id == id).ToListAsync();
            ViewModel.Brokerages = await _context.Brokerages.OrderBy(n => n.Title).ToListAsync();

            return View(ViewModel);
        }

        public async Task<IActionResult> AddSubscriptions(int clientid, string brokerageId)
        {
            Subscription addMember = new Subscription();

            addMember.BrokerageId = brokerageId;
            addMember.ClientId = clientid;
            _context.Subscriptions.Add(addMember);

            await _context.SaveChangesAsync();

            return RedirectToAction("EditSubscriptions", new { id = clientid });
        }

        public async Task<IActionResult> RemoveSubscriptions(int clientid, string brokerageId)
        {
            Subscription removeMember = new Subscription();

            removeMember.BrokerageId = brokerageId;
            removeMember.ClientId = clientid;
            _context.Subscriptions.Remove(removeMember);

            await _context.SaveChangesAsync();

            return RedirectToAction("EditSubscriptions", new { id = clientid });

        }



    }
}
