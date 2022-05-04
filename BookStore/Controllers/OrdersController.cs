#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookStore.Data;
using BookStore.Models;
using BookStore.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;

namespace BookStore.Controllers
{
    public class OrdersController : Controller
    {
        private readonly BookStoreContext _context;
        private readonly UserManager<StoreUser> _userManager;


        public OrdersController(BookStoreContext context, UserManager<StoreUser> userManager)
        {
            _context = context;
            _userManager = userManager;

        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {

            var bookStoreContext = _context.Order.Include(o => o.User);
            return View(await bookStoreContext.ToListAsync());
        }
        [Route("CreateOders/{Id:int}")]
        public IActionResult AddToCart([FromRoute] string id)
        {

            var book = _context.Book
                            .Where(p => p.Isbn == id)
                            .FirstOrDefault();
            if (book == null)
                return NotFound("Không có sản phẩm");

            // Xử lý đưa vào Cart ...


            return RedirectToAction(nameof(Order));
        }
        /// xóa item trong cart
        [Route("/DeleteOrders/{Id:int}", Name = "DeleteOrder")]
        public IActionResult RemoveCart([FromRoute] string id)
        {

            // Xử lý xóa một mục của Cart ...
            return RedirectToAction(nameof(Order));
        }

        /// Cập nhật
        [Route("/updateOrder", Name = "updateOrder")]
        [HttpPost]
        public IActionResult UpdateOrder([FromForm] string id, [FromForm] int quantity)
        {
            // Cập nhật Cart thay đổi số lượng quantity ...

            return RedirectToAction(nameof(Order));
        }
        // Hiện thị giỏ hàng
        [Route("/Order", Name = "Order")]
        public IActionResult Cart()
        {
            return View();
        }

        [Route("/checkout")]
        public IActionResult CheckOut()
        {
            // Xử lý khi đặt hàng
            return View();
        }
      
            // GET: Orders/Details/5
            public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .Include(o => o.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            ViewData["UId"] = new SelectList(_context.Store, "Id", "Id");
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UId,OrderDate,Total")] Order order)
        {
            StoreUser thisUser = await _userManager.GetUserAsync(HttpContext.User);
            Store thisStore = await _context.Store.FirstOrDefaultAsync(s => s.UId == thisUser.Id);
            Book thisBook = await _context.Book.FindAsync(order.Id);

            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UId"] = new SelectList(_context.Store, "Id", "Id", order.UId);
            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["UId"] = new SelectList(_context.Store, "Id", "Id", order.UId);
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UId,OrderDate,Total")] Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
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
            ViewData["UId"] = new SelectList(_context.Store, "Id", "Id", order.UId);
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .Include(o => o.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Order.FindAsync(id);
            _context.Order.Remove(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Order.Any(e => e.Id == id);
        }
    }
}
