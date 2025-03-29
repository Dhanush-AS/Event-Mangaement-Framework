using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Configuration;
using UI.Models;

namespace UI.Conrollers
{
    public class AdminController : Controller
    {

        private readonly IConfiguration _configuration;
        private readonly string _baseUrl;

        public AdminController(IConfiguration configuration)
        {
            _configuration = configuration;
            _baseUrl = $"{_configuration.GetValue<string>("WebAPIBaseUrl")}Registration";
        }


        [Route("AdminHome")]
        public IActionResult AdminHome()
        {
            return View();
        }

        [HttpGet("Approve/{id:guid}", Name = "ApproveRegistrationPage")]
        public async Task<IActionResult> ApproveRegistrationPage(Guid id)
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
        [HttpPost("Approve/{id:guid}", Name = "ApproveRegistration")]
        public async Task<IActionResult> ApproveRegistration(Guid id, Registration registration)
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
    }
}
