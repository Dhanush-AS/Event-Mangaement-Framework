using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UI.Models;

namespace UI.Controllers
{
    [Route("Event")]

    public class EventController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly string _baseUrl;

        public EventController(IConfiguration configuration)
        {
            _configuration = configuration;
            _baseUrl = $"{_configuration.GetValue<string>("WebAPIBaseUrl")}Event";
        }


        // GET: /Event/GetAll
        [HttpGet("getallevents", Name = "GetAllEvents")]
        public async Task<IActionResult> GetAllEvents()
        {
            var events = new List<EventDetails>();

            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync($"{_baseUrl}/getallevents");
                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    events = JsonConvert.DeserializeObject<List<EventDetails>>(apiResponse);
                }
                else
                {
                    return BadRequest();
                }
            }
            return View(events);
        }

        // GET: /Event/Add
        [HttpGet]
        [Route("AddNewEvent")]
        public IActionResult AddEvent()
        {
            return View();
        }

        // POST: /Event/Add
        [HttpPost("Add", Name = "SaveEvent")]
        public async Task<IActionResult> SaveEvent(EventDetails eventDetails)
        {
            using (var httpClient = new HttpClient())
            {
                var content = new StringContent(JsonConvert.SerializeObject(eventDetails), Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync($"{_baseUrl}/addevent", content);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToRoute("GetAllEvents");
                }
                else
                {
                    return BadRequest();
                }
            }
        }

        // GET: /Event/Edit/{id}
        [HttpGet("Edit/{id:guid}", Name = "EditEvent")]
        public async Task<IActionResult> EditEvent(Guid id)
        {
            EventDetails eventDetails = null;

            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync($"{_baseUrl}/getevent/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    eventDetails = JsonConvert.DeserializeObject<EventDetails>(apiResponse);
                }
                else
                {
                    return BadRequest();
                }
            }
            return View(eventDetails);
        }

        // POST: /Event/Update
        [HttpPost("Update", Name = "UpdateEvent")]
        public async Task<IActionResult> UpdateEvent(EventDetails eventDetails)
        {
            if (!ModelState.IsValid)
            {
                return View(eventDetails); 
            }

            using (var httpClient = new HttpClient())
            {
                var content = new StringContent(JsonConvert.SerializeObject(eventDetails), Encoding.UTF8, "application/json");
                var response = await httpClient.PutAsync($"{_baseUrl}/updateevent/{eventDetails.eventId}", content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToRoute("GetAllEvents"); 
                }
                else
                {
                    return BadRequest(); 
                }
            }
        }   
        // GET: /Event/Delete/{id}
        [HttpGet("Delete/{id:guid}", Name = "DeleteEvent")]
        public async Task<IActionResult> DeleteEvent(Guid id)
        {
            EventDetails eventDetails = null;

            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync($"{_baseUrl}/getevent/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    eventDetails = JsonConvert.DeserializeObject<EventDetails>(apiResponse);
                }
                else
                {
                    return BadRequest();
                }
            }
            return View(eventDetails);
        }

        // POST: /Event/Delete/{id}
        [HttpPost("Delete/{id:guid}", Name = "DeleteEventPost")]
        public async Task<IActionResult> DeleteEventById(Guid id)
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.DeleteAsync($"{_baseUrl}/deleteevent/{id}");
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToRoute("GetAllEvents");
                }
                else
                {
                    return BadRequest();
                }
            }
        }
    }
}
