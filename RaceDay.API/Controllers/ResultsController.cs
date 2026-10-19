using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.Models;
using System.Security.Claims;

namespace RaceDay.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResultsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public ResultsController(RaceDayDbContext context)
        {
            _context = context;
        }

        // POST: api/results (Organiser ONLY - Record finish times)
        [HttpPost]
        [Authorize(Roles = "Organiser")]
        public async Task<ActionResult<Result>> PostResult([FromBody] Result result)
        {
            // Verify the enrolment actually exists before adding a result
            var enrolment = await _context.Enrolments.FindAsync(result.EnrolmentId);
            if (enrolment == null)
                return NotFound("Enrolment not found.");

            _context.Results.Add(result);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMyResults), new { }, result);
        }

        // GET: api/results/my-results (Participant ONLY - View own results)
        [HttpGet("my-results")]
        [Authorize(Roles = "Participant")]
        public async Task<ActionResult<IEnumerable<Result>>> GetMyResults()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId))
                return Unauthorized("Invalid token.");

            // Fetch results and include the Event and Category names so the frontend can display them
            var results = await _context.Results
                .Where(r => r.Enrolment.UserId == userId)
                .Include(r => r.Enrolment)
                    .ThenInclude(e => e.Category)
                    .ThenInclude(c => c.RaceEvent)
                .ToListAsync();

            return Ok(results);
        }

        // GET: api/results/event/{eventId} (Public - View Leaderboard)
        [HttpGet("event/{eventId}")]
        public async Task<ActionResult<IEnumerable<Result>>> GetEventResults(int eventId)
        {
            // Check if event exists
            var eventExists = await _context.Events.AnyAsync(e => e.EventId == eventId);
            if (!eventExists) return NotFound("Event not found.");

            // Fetch all results for this event, ordered by finish time (Leaderboard style)
            var results = await _context.Results
                .Where(r => r.Enrolment.Category.EventId == eventId)
                .Include(r => r.Enrolment)
                    .ThenInclude(e => e.User) // To get participant name
                .Include(r => r.Enrolment)
                    .ThenInclude(e => e.Category) // To get category name
                .OrderBy(r => r.FinishTime)
                .ToListAsync();

            return Ok(results);
        }
    }
}