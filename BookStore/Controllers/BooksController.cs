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
using System.IO;
using BookStore.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;

namespace BookStore.Controllers
{
    public class BooksController : Controller
    {
        private readonly BookStoreContext _context;
        private readonly UserManager<StoreUser> _userManager;
        private double _recordsPerPage = 6;


        public BooksController(BookStoreContext context, UserManager<StoreUser> userManager)
        {
            _context = context;
            _userManager = userManager;

        }

        // GET: Books
       
        public async Task<IActionResult> List(int id = 0)
        {
            

            int numberOfRecords = await _context.Book.CountAsync();     //Count SQL
            int numberOfPages = (int)Math.Ceiling((double)numberOfRecords / _recordsPerPage);
            ViewBag.numberOfPages = numberOfPages;
            ViewBag.currentPage = id;
            List<Book> Books = await _context.Book
                .Skip((int)(id * _recordsPerPage))  //Offset SQL
                .Take((int)_recordsPerPage)       //Top SQL
                .ToListAsync();
                 _context.Book.ToList();
            return View(Books);

        }
        
        // GET: Books/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .Include(s => s.StoreId)
                .FirstOrDefaultAsync(m => m.Isbn == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: Books/Create
        public IActionResult Create()
        {
            ViewData["StoreId"] = new SelectList(_context.Store, "Id", "Id");
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Isbn,Title,Pages,Author,Category,Price,Desc")]Book book, IFormFile image)
        {
            if (image != null)
            {
                string imgName = book.Isbn + Path.GetExtension(image.FileName);
                string savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", imgName);
                using (var stream = new FileStream(savePath, FileMode.Create))
                {
                    image.CopyTo(stream);
                }
                book.ImgUrl = "img/" + imgName;
                StoreUser thisUser = await _userManager.GetUserAsync(HttpContext.User);
                Store thisStore = await _context.Store.FirstOrDefaultAsync(s => s.UId == thisUser.Id);
                book.StoreId = thisStore.Id;
            }
            else
            {
                return View(book);
            }
                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(List));
            ViewData["StoreId"] = new SelectList(_context.Store, "Id", "Id", book.StoreId);

            return View(book);
        }

            // GET: Books/Edit/5
            public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Book.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            ViewData["StoreId"] = new SelectList(_context.Store, "Id", "Id", book.StoreId);
            return View(book);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Isbn,Title,Pages,Author,Category,Price,Desc,ImgUrl,StoreId")] Book book)
        {
            if (id != book.Isbn)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.Isbn))
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
            ViewData["StoreId"] = new SelectList(_context.Store, "Id", "Id", book.StoreId);
            return View(book);
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .Include(b => b.Store)
                .FirstOrDefaultAsync(m => m.Isbn == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var book = await _context.Book.FindAsync(id);
            _context.Book.Remove(book);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(string id)
        {
            return _context.Book.Any(e => e.Isbn == id);
        }
    }
}
