using EMS2.Models;
using EMS2.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
namespace EMS2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventController(IEventService eventService)
        {
            _eventService = eventService;
        }

        [HttpPost("addevent")]
        public async Task<IActionResult> AddEvent([FromBody] EventDetails eventDetails)
        {
            await _eventService.AddEventAsync(eventDetails);
            return Ok();
        }

        [HttpGet("getevent/{eventId}")]
        public async Task<IActionResult> GetEventById(Guid eventId)
        {
            var eventDetails = await _eventService.GetEventByIdAsync(eventId);
            if (eventDetails == null)
            {
                return NotFound();
            }

            return Ok(eventDetails);
        }

        [HttpGet("getallevents")]
        public async Task<IActionResult> GetAllEvents()
        {
            var events = await _eventService.GetAllEventsAsync();
            return Ok(events);
        }

        // PUT: /updateevent/{id}
        [HttpPut("updateevent/{id:guid}")]
        public async Task<IActionResult> UpdateEvent(Guid id, [FromBody] EventDetails eventDetails)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existingEvent = await _eventService.GetEventByIdAsync(id);
            if (existingEvent == null) return NotFound();

            existingEvent.EventName = eventDetails.EventName;
            existingEvent.EventCategory = eventDetails.EventCategory;
            existingEvent.EventDate = eventDetails.EventDate;
            existingEvent.Description = eventDetails.Description;
            existingEvent.Status = eventDetails.Status;

            await _eventService.UpdateEventAsync(existingEvent);
            return NoContent();
        }


        [HttpDelete("deleteevent/{eventId}")]
        public async Task<IActionResult> DeleteEvent(Guid eventId)
        {
            await _eventService.DeleteEventAsync(eventId);
            return Ok();
        }
    }
}