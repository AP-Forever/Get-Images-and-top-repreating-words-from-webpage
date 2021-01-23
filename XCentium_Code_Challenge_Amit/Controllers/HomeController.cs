using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XCentium_Code_Challenge_Amit.Models;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Xml;
using HtmlAgilityPack;

namespace XCentium_Code_Challenge_Amit.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            UrlModel model = new UrlModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(UrlModel urlModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //Creating the HttpWebRequest to check if URL page exists.
                    HttpWebRequest request = WebRequest.Create(urlModel.url) as HttpWebRequest;
                    request.Method = "GET";
                    HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                    response.Close();
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        List<string> images = new List<string>();
                        WebRequest wr = WebRequest.Create(urlModel.url);
                        WebResponse res = wr.GetResponse();
                        string htmlString;
                        using (StreamReader reader = new StreamReader(res.GetResponseStream()))
                        {
                            htmlString = reader.ReadToEnd();
                        }

                        urlModel.images = HtmlUtility.GetAllImages(htmlString);

                        string plaintext = HtmlUtility.ConvertToDisplayText(htmlString);

                        urlModel.WordsFrequency = HtmlUtility.GetWordsFrequency(plaintext);

                        ModelState.Clear();
                        return View(urlModel);
                    }
                }
                catch (Exception ex)
                {
                    //Any exception will return Model as is.
                    ModelState.AddModelError("WebpageIssue", "Error. Please check the web page for the entered URL." + ex.Message);
                    return View(urlModel);
                }
            }
            return View(urlModel);
        }
    }
}