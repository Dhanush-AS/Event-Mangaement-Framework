using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using UI.Models;

namespace UI.Conrollers
{
    public class UserController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly string _baseurl;
        public UserController(IConfiguration configuration)
        {
            _configuration = configuration;
            _baseurl = $"{_configuration.GetValue<string>("WebAPIBaseUrl")}User";
        }

        [Route("Login")]
        public IActionResult Login()
        {
            return View();
        }

        [Route("ParticipantLogin")]
        public IActionResult ParticipantLogin()
        {
            return View();
        }

        //participant login
        [HttpPost]
        [Route("LogIn", Name = "LogIn")]
        public async Task<IActionResult> LogInPageParticipant(Users user)
        {

            using (HttpClient client = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
                using (var response = await client.PostAsync($"{_baseurl}/ValidateUser", content))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var result = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());
                        string token = result.token;
                        string role = result.role;
                        HttpContext.Session.SetString("token", token);
                        HttpContext.Session.SetString("Email", user.Email);  // Save email in session

                        if (role == "Participant")
                        {
                            return RedirectToAction("ParticipantHome", "Participant");
                        }
                        else
                        {
                            return RedirectToAction("Login", "Home");
                        }
                    }
                    else
                    {
                        return RedirectToAction("ParticipantLogin", "User");
                    }
                }
            }
        }

            //admin
            [Route("AdminLogin")]
            public IActionResult AdminLogin()
            {
                return View();
            }
        //admin login
        [HttpPost]
        [Route("LogInAdmin", Name = "LogInAdmin")]
        public async Task<IActionResult> LogInPageAdmin(Users Users)
        {
            using (HttpClient client = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(Users), Encoding.UTF8, "application/json");

                // Send request to validate user credentials and get the token and role
                using (var response = await client.PostAsync($"{_baseurl}/ValidateUser", content))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        // Deserialize the response content into a dynamic object
                        var result = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());

                        // Extract the token and role
                        string token = result.token;
                        string role = result.role;

                        HttpContext.Session.SetString("token", token);

                        // Redirect based on the role
                        if (role == "Admin")
                        {
                            return RedirectToAction("AdminHome", "Admin");
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Only admins are allowed.";
                            return RedirectToAction("AdminLogin", "User");
                        }
                    }
                    else
                    {
                        // Credentials validation failed, redirect back to login
                        TempData["ErrorMessage"] = "Invalid login attempt.";
                        return RedirectToAction("AdminLogin", "User");
                    }
                }
            }
        }

        //reg user page
        [HttpGet]
        [Route("regpage")]
        public IActionResult RegPage()
        {
            return View();
        }

        //reg user 
        [HttpPost]
        [Route("RegisterUser")]
        public async Task<IActionResult> RegisterUser(Users user)
        {
            if (ModelState.IsValid)
            {
                using (var httpClient = new HttpClient()){
                    var response = await httpClient.PostAsJsonAsync($"{_baseurl}/adduser", user);
                    if(response.IsSuccessStatusCode) {

                        // Redirect to a success page or another action
                        return RedirectToAction("ParticipantLogin", "Participant");
                    }
                    else
                    {
                        // Handle the case where the API request fails
                        ModelState.AddModelError(string.Empty, "Error creating user. Please try again.");
                    }
                }
            }

            // If we got this far, something failed, redisplay the form
            return View(user);
        }

        [HttpGet("getallusers", Name = "GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = new List<Users>();

            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync($"{_baseurl}/getallusers");
                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    users = JsonConvert.DeserializeObject<List<Users>>(apiResponse);
                }
                else
                {
                    return BadRequest();
                }
            }
            return View(users);
        }
        // GET: /User/Edit/{id}
        [HttpGet("Edit/{id:guid}", Name = "EditUser")]
        public async Task<IActionResult> EditUser(Guid id)
        {
            Users user = null;

            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync($"{_baseurl}/Getuserbyid/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    user = JsonConvert.DeserializeObject<Users>(apiResponse);
                }
                else
                {
                    return BadRequest();
                }
            }
            return View(user);
        }
        // POST: /User/Update
        [HttpPost("Update/{id:guid}", Name = "UpdateUser")]
        public async Task<IActionResult> UpdateUser(Guid id, Users user)
        {
            if (!ModelState.IsValid)
            {
                return View(user); 
            }

            using (var httpClient = new HttpClient())
            {
                user.Userid = id;

                var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync($"{_baseurl}/updateuser/{id}", content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToRoute("GetAllUsers"); 
                }
                else
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    return BadRequest(errorResponse);
                }
            }
        }

        // GET: /User/Delete/{id}
        [HttpGet("Delete/{id:guid}", Name = "DeleteUser")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            Users user = null;

            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync($"{_baseurl}/GetUserById/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    user = JsonConvert.DeserializeObject<Users>(apiResponse);
                }
                else
                {
                    return BadRequest();
                }
            }
            return View(user); // Returns the user details to confirm deletion
        }

        // POST: /User/Delete/{id}
        [HttpPost("Delete/{id:guid}", Name = "DeleteUserById")]
        public async Task<IActionResult> DeleteUserById(Guid id)
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.DeleteAsync($"{_baseurl}/deleteuser/{id}");
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToRoute("GetAllUsers"); // Redirect to the list of users after deletion
                }
                else
                {
                    return BadRequest();
                }
            }
        }

    }
}

