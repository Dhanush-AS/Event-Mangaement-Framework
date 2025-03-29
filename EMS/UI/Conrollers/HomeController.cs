using Microsoft.AspNetCore.Mvc;

namespace UI.Conrollers
{
    public class HomeController : Controller
    {
        [HttpGet]
        [Route("/")]
        [Route("HomePage", Name = "Homepage")]

        public IActionResult HomePage()
        {
            return View();
        }

        [Route("AboutUs", Name = "AboutUs")]
        public IActionResult AboutUs()
        {
            return View();
        }

        [Route("ContactUs", Name = "ContactUs")]
        public IActionResult ContactUs()
        {
            return View();
        }

    }
}
