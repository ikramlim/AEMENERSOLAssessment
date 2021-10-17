using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using WebAppAssessmentPart1.Models;
using WebAppAssessmentPart1.Data;

namespace WebAppAssessmentPart1.Controllers
{
    public class PlatformController : Controller
    {
        private readonly IConfiguration configuration;

        private static string baseURL = "http://test-demo.aemenersol.com/api/PlatformWell/GetPlatformWellDummy";

        public PlatformController(IConfiguration config)
        {
            this.configuration = config;
        }

        //public async Task<IActionResult> Index()
        //{
        //    var Platform = await GetPlatform();
        //    return View();
        //}

        [HttpGet]
        public  ActionResult Index(Platform platform)
        {
            // use the access token to call a protected web API.
            var accessToken = HttpContext.Session.GetString("JWToken");
            var url = baseURL;
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var jsonStr =  httpClient.GetStringAsync(requestUri: url);

            List<PlatformModelObject> pformResult = JsonConvert.DeserializeObject<List<PlatformModelObject>>(jsonStr.Result);

            //var pformResulttest = JsonConvert.DeserializeObject<List<PlatformWell>>(jsonStr);
            //Platform pformResult = JsonConvert.DeserializeObject<Platform>(jsonStr);
            insertPlatform(pformResult);
            return View();


        }

        [HttpPost]
        private void insertPlatform(List<PlatformModelObject> pformResult)
        {
            try
            {
                using (var db = new testDBContext())
                {
                    Platform pForm = new Platform();


                    foreach (var item in pformResult)
                    {
                        pForm.Id = item.id;
                        pForm.UniqueName = item.uniqueName;
                        pForm.Latitude = item.latitude;
                        pForm.Longitude = item.longitude;
                        pForm.CreatedAt = item.createdAt;
                        pForm.UpdatedAt = item.updatedAt;

                        var retVal = false;
                        using (var context = new testDBContext())
                        {
                            retVal = db.Platforms.Count(a => a.Id == pForm.Id) > 0;
                        }
                        if (retVal == false)
                        {
                            db.Add(pForm);
                            db.SaveChanges();
                        }

                        

                        using (var db2 = new testDBContext())
                        {
                            Well allWell = new Well();
                            foreach (var childItem in item.Well)
                            {
                                var selected = new Well
                                {
                                    Id = childItem.id,
                                    PlatformId = childItem.platformId,
                                    UniqueName = childItem.uniqueName,
                                    Latitude = childItem.latitude,
                                    Longitude = childItem.longitude,
                                    CreatedAt = childItem.createdAt,
                                    UpdatedAt = childItem.updatedAt
                                };

                                var retVal2 = false;
                                using (var context2= new testDBContext())
                                {
                                    retVal2 = db.Wells.Count(a => a.Id == selected.Id) > 0;
                                }
                                if (retVal2 == false)
                                {
                                    db2.Add(selected);
                                    db2.SaveChanges();
                                }
                            }
                        }
                    }
                    //db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("{0} Exception caught.", e);
            }

        }

        private bool checkIfIdExist(int id)
        {
            var retVal = false;

            using (var db = new testDBContext())
            {
                retVal = db.Platforms.Count(a => a.Id == id) > 0;
            }
            return retVal;
        }

        public class PlatformModelObject
        {
            public int id { get; set; }
            public string uniqueName { get; set; }
            public decimal latitude { get; set; }
            public decimal longitude { get; set; }
            public DateTime createdAt { get; set; }
            public DateTime updatedAt { get; set; }

            public List<WellModelObject> Well { get; set; }
        }

        public class WellModelObject
        {
            public int id { get; set; }
            public int platformId { get; set; }
            public string uniqueName { get; set; }
            public decimal latitude { get; set; }
            public decimal longitude { get; set; }
            public DateTime createdAt { get; set; }
            public DateTime updatedAt { get; set; }
        }

    }
}
