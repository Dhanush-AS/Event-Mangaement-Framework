using UI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Linq;

namespace UI.Controllers
{
    [Route("Registration")]
    public class RegistrationController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly string _baseUrl;

        public RegistrationController(IConfiguration configuration)
        {
            _configuration = configuration;
            _baseUrl = $"{_configuration.GetValue<string>("WebAPIBaseUrl")}Registration";
        }


        // GET: /Registration/GetAll
        [HttpGet("GetAll", Name = "GetAllRegistrations")]

        public async Task<IActionResult> GetAllRegistrations()
        {
            var registrations = new List<Registration>();

            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync($"{_baseUrl}/getall");
                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    registrations = JsonConvert.DeserializeObject<List<Registration>>(apiResponse);
                }
                else
                {
                    return BadRequest();
                }
            }
            return View(registrations);
        }


        // GET: /Registration/Edit/{id}
        [HttpGet("Edit/{id:guid}", Name = "EditRegistration")]
        public async Task<IActionResult> EditRegistration(Guid id)
        {
            Registration registration = null;

            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync($"{_baseUrl}/getbyid/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    registration = JsonConvert.DeserializeObject<Registration>(apiResponse);
                }
                else
                {
                    return BadRequest();
                }
            }
            return View(registration);
        }

        // POST: /Registration/Update
        [HttpPost("Update/{id:guid}", Name = "UpdateRegistration")]
        public async Task<IActionResult> UpdateRegistration(Guid id, Registration registration)
        {
            if (!ModelState.IsValid)
            {
                return View(registration); // Return the view with validation errors
            }

            using (var httpClient = new HttpClient())
            {
                registration.regId = id;

                var content = new StringContent(JsonConvert.SerializeObject(registration), Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync($"{_baseUrl}/updateregistration/{id}", content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToRoute("GetAllRegistrations");
                }
                else
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    return BadRequest(errorResponse);
                }
            }
        }


        // GET: /Registration/Delete/{id}
        [HttpGet("Delete/{id:guid}", Name = "DeleteRegistration")]
        public async Task<IActionResult> DeleteRegistration(Guid id)
        {
            Registration registration = null;

            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync($"{_baseUrl}/getbyid/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    registration = JsonConvert.DeserializeObject<Registration>(apiResponse);
                }
                else
                {
                    return BadRequest();
                }
            }
            return View(registration);
        }

        // POST: /Registration/Delete/{id}
        [HttpPost("Delete/{id:guid}", Name = "DeleteRegistrationPost")]
        public async Task<IActionResult> DeleteRegistrationById(Guid id)
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.DeleteAsync($"{_baseUrl}/delete/{id}");
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToRoute("GetAllRegistrations");
                }
                else
                {
                    return BadRequest();
                }
            }
        }

        //grp by count
        public async Task<IActionResult> RegistrationCounts()
        {
            var registrations = new List<Registration>();
            var events = new List<EventDetails>(); // Assuming Eventdetails is the model for the event data

            using (var httpClient = new HttpClient())
            {
                // Fetch registrations
                var regResponse = await httpClient.GetAsync($"{_baseUrl}/getall");
                if (regResponse.IsSuccessStatusCode)
                {
                    var regApiResponse = await regResponse.Content.ReadAsStringAsync();
                    registrations = JsonConvert.DeserializeObject<List<Registration>>(regApiResponse);
                }
                else
                {
                    return BadRequest("Could not retrieve registration data.");
                }

                // Fetch events
                var eventResponse = await httpClient.GetAsync($"{_configuration.GetValue<string>("WebAPIBaseUrl")}Event/getallevents");
                if (eventResponse.IsSuccessStatusCode)
                {
                    var eventApiResponse = await eventResponse.Content.ReadAsStringAsync();
                    events = JsonConvert.DeserializeObject<List<EventDetails>>(eventApiResponse);
                }
                else
                {
                    return BadRequest("Could not retrieve event data.");
                }
            }

            // Group registrations by EventId and join with event names
            var registrationCounts = registrations
                .GroupBy(r => r.EventId)
                .Select(g => new
                {
                    EventId = g.Key,
                    Count = g.Count(),
                    EventName = events.FirstOrDefault(e => e.eventId.ToString() == g.Key)?.EventName // Match EventId with EventName
                })
                .ToList();

            return View(registrationCounts);
        }
    }
    }
