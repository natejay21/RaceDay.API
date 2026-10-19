using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.Models;

namespace RaceDay.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public EventsController(RaceDayDbContext context)
        {
            _context = context;
        }

        // GET: api/events (Public - Anyone can view events)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RaceEvent>>> GetEvents()
        {
            // .Include() tells EF Core to also grab the Categories for each event
            return await _context.Events.Include(e => e.Categories).ToListAsync();
        }

        // GET: api/events/5 (Public)
        [HttpGet("{id}")]
        public async Task<ActionResult<RaceEvent>> GetEvent(int id)
        {
            var raceEvent = await _context.Events
                .Include(e => e.Categories)
                .FirstOrDefaultAsync(e => e.EventId == id);

            if (raceEvent == null)
                return NotFound("Event not found.");

            return raceEvent;
        }

        // POST: api/events (Organiser ONLY)
        [HttpPost]
        [Authorize(Roles = "Organiser")]
        public async Task<ActionResult<RaceEvent>> PostEvent(RaceEvent raceEvent)
        {
            _context.Events.Add(raceEvent);
            await _context.SaveChangesAsync();

            // Returns 201 Created and the URL to get the new event
            return CreatedAtAction(nameof(GetEvent), new { id = raceEvent.EventId }, raceEvent);
        }

        // PUT: api/events/5 (Organiser ONLY)
        [HttpPut("{id}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> PutEvent(int id, RaceEvent raceEvent)
        {
            if (id != raceEvent.EventId)
                return BadRequest("Event ID mismatch.");

            _context.Entry(raceEvent).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EventExists(id))
                    return NotFound("Event not found.");
                else
                    throw;
            }

            return NoContent(); // 204 No Content is the REST standard for a successful update
        }

        // DELETE: api/events/5 (Organiser ONLY)
        [HttpDelete("{id}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var raceEvent = await _context.Events.FindAsync(id);
            if (raceEvent == null)
                return NotFound("Event not found.");

            _context.Events.Remove(raceEvent);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Helper method to check if an event exists
        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.EventId == id);
        }
    }
}