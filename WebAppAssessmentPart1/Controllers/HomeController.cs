using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WebAppAssessmentPart1.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace WebAppAssessmentPart1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        //[HttpPost]
        //public ActionResult Index(User user)
        //{

        //    return View(user);
        //}

        public async Task<IActionResult> Login(User user)
        {
            //List<Reservation> reservationList = new List<Reservation>();
            //using (var httpClient = new HttpClient())
            //{
            //    using (var response = await httpClient.GetAsync("http://test-demo.aemenersol.com/api/Account/Login"))
            //    {
            //        string apiResponse = await response.Content.ReadAsStringAsync();
            //        reservationList = JsonConvert.DeserializeObject<List<Reservation>>(apiResponse);
            //    }
            //}
            //return View(reservationList);

            if ((user.Username == "" || user.Username == null) || (user.Password == "" || user.Password == null))
            {
                ViewBag.Message = "Please Enter Username and Password";
                return Redirect("~/Home/Index");
            }

            using (var httpClient = new HttpClient())
            {
                StringContent stringContent = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
                using (var response = await httpClient.PostAsync("http://test-demo.aemenersol.com/api/Account/Login", stringContent)) // call Rest API as provided 
                {
                    string token = await response.Content.ReadAsAsync<string>();
                    if (token == "Invalid credentials")
                    {
                        ViewBag.Message = "Incorrect UserId or Password!";
                        return Redirect("~/Home/Index"); //return back to main page
                    }
                    HttpContext.Session.SetString("JWToken", token);
                }
                return Redirect("~/Platform/Index"); // return to Dashboard page
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
