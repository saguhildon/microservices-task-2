using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieBookingAPI.Models;
using Newtonsoft.Json.Linq;

namespace MovieBookingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly BookingContext _context;

        public BooksController(BookingContext context)
        {
            _context = context;
        }

        // GET: api/Books
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetBookItems()
        {
            var book = await _context.BookItems.ToListAsync();

            foreach (var item in book.Where(w => w.Currency != "sgd"))
            {
                var value = await CurrencyConvertor(item.Currency);
                item.Amount = item.Amount * value;
            }

            return book;

        }

        // GET: api/Books/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetBook(int id)
        {
            var book = await _context.BookItems.FindAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            return book;
        }

        // PUT: api/Books/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBook(int id, Book book)
        {
            if (id != book.BookingId)
            {
                return BadRequest();
            }

            _context.Entry(book).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Books
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Book>> PostBook(Book book)
        {
            var value = await CurrencyConvertor(book.Currency);
            book.Amount = book.Amount * value;
            _context.BookItems.Add(book);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBook", new { id = book.BookingId }, book);
        }

        // DELETE: api/Books/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _context.BookItems.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            _context.BookItems.Remove(book);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private async Task<double> CurrencyConvertor(String currencyType)
        {
            double results = -1L;
            HttpClient client = null;
            HttpResponseMessage response = null;

            try
            {
                string apiCallURL =
                    String.Format(@"https://cdn.jsdelivr.net/gh/fawazahmed0/currency-api@1/latest/currencies/{0}/sgd.json", currencyType);

                client = new HttpClient();
                response = (HttpResponseMessage)await client.GetAsync(apiCallURL);

                var json_results = await response.Content.ReadAsStringAsync();
                Debug.WriteLine(results);

                dynamic json = JObject.Parse(json_results);
                Debug.WriteLine((String)json.sgd);

                results = Convert.ToDouble((String)json.sgd);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return results;
        }

        private bool BookExists(int id)
        {
            return _context.BookItems.Any(e => e.BookingId == id);
        }
    }
}
