using UI.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace UI.Conrollers
{
    public class ParticipantController : Controller
    {
        [Route("ParticipantHome")]
        public IActionResult ParticipantHome()
        {
            return View();
        }
        [HttpGet]
        [Route("geteventforreg")]
        public async Task<IActionResult> Get()
        {
            List<EventDetails> events = new List<EventDetails>();

            // Create a new HttpClient instance
            using (var httpClient = new HttpClient())
            {
                // Making a GET request to retrieve all events
                var response = await httpClient.GetAsync("https://localhost:44374/api/event/getallevents");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    events = JsonConvert.DeserializeObject<List<EventDetails>>(json);
                }
                else
                {
                    // Handle error response (optional)
                    ViewBag.ErrorMessage = "Unable to load events. Please try again later.";
                }
            }

            return View(events);
        }

        [HttpGet("Register", Name = "AddRegisteration")]
        public IActionResult Register(string eventId)
        {
            var registration = new Registration
            {
                EventId = eventId,
                Email = HttpContext.Session.GetString("Email")
            };

            return View(registration);
        }
        // POST: Events/Register
        [HttpPost("Register", Name = "SaveRegistration")]
        public async Task<IActionResult> Registration(Registration registration)
        {
            // Serializing registration object to JSON
            var email = HttpContext.Session.GetString("Email");
            var jsonContent = JsonConvert.SerializeObject(registration);
            var contentString = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Create a new HttpClient instance
            using (var httpClient = new HttpClient())
            {
                // Making a POST request to register for the event
                var response = await httpClient.PostAsync("https://localhost:44374/api/registration/register", contentString);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("GetAllRegistrationsParticipant", "Participant");
                }

                // Handle unsuccessful registration (optional)
                ViewBag.ErrorMessage = "Registration failed. Please check your details and try again.";
            }

            return View(registration); // Return to the view if registration fails
        }

        // GET: /Registration/GetAll/
        [HttpGet("GetAllRegistrationsParticipant")]
        public async Task<IActionResult> GetAllRegistrationsParticipant()
        {
            var registrations = new List<Registration>();
            var email = HttpContext.Session.GetString("Email");

            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Email not found in session.");
            }

            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync($"https://localhost:44374/api/registration/getbyemail/{email}");
                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    registrations = JsonConvert.DeserializeObject<List<Registration>>(apiResponse);
                }
                else
                {
                    return BadRequest("Failed to retrieve registrations.");
                }
            }

            return View(registrations);
        }
    }
}
  