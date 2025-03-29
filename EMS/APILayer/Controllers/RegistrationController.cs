using EMS2.Models;
using EMS2.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace EMS2.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationController : Controller
    {
        public readonly IParticipantService _participantService;
        public RegistrationController(IParticipantService participantService)
        {
            _participantService = participantService;
        }

        //add
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Registration registration)
        {
            await _participantService.AddRegistrationAsync(registration);
            return Ok();
        }

        //delete
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _participantService.DeleteRegistrationAsync(id);
            return Ok();
        }

        //update
        [HttpPost("updateregistration/{id:guid}")]
        public async Task<IActionResult> UpdateRegistration(Guid id, [FromBody] Registration registration)
        {
            // Validate the incoming model
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Check if the registration exists
            var existingRegistration = await _participantService.GetByIdAsync(id);
            if (existingRegistration == null) return NotFound();

            // Update the properties of the existing registration
            existingRegistration.FirstName = registration.FirstName;
            existingRegistration.LastName = registration.LastName;
            existingRegistration.Email = registration.Email;
            existingRegistration.CountryCode = registration.CountryCode;
            existingRegistration.PhoneNumber = registration.PhoneNumber;
            existingRegistration.RegStatus = registration.RegStatus;
            existingRegistration.EventId = registration.EventId;

            // Call the service to save the updated registration
            await _participantService.UpdateRegistrationAsync(existingRegistration);

            // Return a No Content response
            return NoContent();
        }


        //get all
        [HttpGet("getall")]
        public async Task<IActionResult> Getall()
        {
            var registration = await _participantService.GetAllRegistrationsAsync();
            return Ok(registration);
        }

        //getbyId
        [HttpGet("getbyid/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var registration =await _participantService.GetByIdAsync(id);
            return Ok(registration);
        }

        //getbyEmail
        [HttpGet("getbyemail/{email}")]
        public async Task<IActionResult> GetByEmail(string email)
        {
            var registration =await _participantService.GetByEmailAsync(email);
            return Ok(registration);
        }
    }
}