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
    public class EnrolmentsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public EnrolmentsController(RaceDayDbContext context)
        {
            _context = context;
        }

        // POST: api/enrolments (Participant ONLY)
        [HttpPost]
        [Authorize(Roles = "Participant")]
        public async Task<ActionResult<Enrolment>> EnrolInEvent(int categoryId)
        {
            // 1. Get the current user's ID from the JWT token
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId))
                return Unauthorized("Invalid token.");

            // 2. Check if the category exists
            var category = await _context.Categories.FindAsync(categoryId);
            if (category == null)
                return NotFound("Category not found.");

            // 3. Check if already enrolled
            var existingEnrolment = await _context.Enrolments
                .AnyAsync(e => e.UserId == userId && e.CategoryId == categoryId);

            if (existingEnrolment)
                return BadRequest("You are already enrolled in this category.");

            // 4. Create the enrolment
            var enrolment = new Enrolment
            {
                UserId = userId,
                CategoryId = categoryId,
                Status = "Paid", // Assuming instant payment for this project
                EnrolmentDate = DateTime.UtcNow
            };

            _context.Enrolments.Add(enrolment);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMyRaces), new { }, enrolment);
        }

        // GET: api/enrolments/my-races (Participant ONLY)
        [HttpGet("my-races")]
        [Authorize(Roles = "Participant")]
        public async Task<ActionResult<IEnumerable<Enrolment>>> GetMyRaces()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId))
                return Unauthorized("Invalid token.");

            // Include Category and the parent RaceEvent so the frontend has all the info
            var enrolments = await _context.Enrolments
                .Where(e => e.UserId == userId)
                .Include(e => e.Category)
                .ThenInclude(c => c.RaceEvent)
                .ToListAsync();

            return Ok(enrolments);
        }

        // GET: api/events/{eventId}/enrolments (Organiser ONLY)
        [HttpGet("event/{eventId}")]
        [Authorize(Roles = "Organiser")]
        public async Task<ActionResult<IEnumerable<Enrolment>>> GetEventEnrolments(int eventId)
        {
            // Check if event exists
            var eventExists = await _context.Events.AnyAsync(e => e.EventId == eventId);
            if (!eventExists) return NotFound("Event not found.");

            var enrolments = await _context.Enrolments
                .Where(e => e.Category.EventId == eventId)
                .Include(e => e.User)
                .Include(e => e.Category)
                .ToListAsync();

            return Ok(enrolments);
        }
    }
}