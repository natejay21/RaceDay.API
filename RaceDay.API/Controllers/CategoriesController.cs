using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.Models;

namespace RaceDay.API.Controllers
{
    [Route("api/events/{eventId}/[controller]")] // Notice the nested route!
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public CategoriesController(RaceDayDbContext context)
        {
            _context = context;
        }

        // GET: api/events/1/categories (Public)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories(int eventId)
        {
            // Check if event exists
            var eventExists = await _context.Events.AnyAsync(e => e.EventId == eventId);
            if (!eventExists) return NotFound("Event not found.");

            return await _context.Categories
                .Where(c => c.EventId == eventId)
                .ToListAsync();
        }

        // POST: api/events/1/categories (Organiser ONLY)
        [HttpPost]
        [Authorize(Roles = "Organiser")]
        public async Task<ActionResult<Category>> PostCategory(int eventId, Category category)
        {
            var eventExists = await _context.Events.AnyAsync(e => e.EventId == eventId);
            if (!eventExists) return NotFound("Event not found.");

            // Force the category to belong to this event
            category.EventId = eventId;

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCategories), new { eventId = eventId }, category);
        }
    }
}