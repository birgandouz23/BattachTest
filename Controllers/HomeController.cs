using BattachApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MigraDoc.DocumentObjectModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QRCoder;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Drawing.Imaging;
using Microsoft.AspNetCore.Routing.Constraints;
using IronBarCode;
using BattachApp.Services;
using System.Reflection;

namespace BattachApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _host;
        private readonly IConfiguration _config;
        private readonly IEmployeesList _empfc;
        private readonly IFetchData _fetchData;
        private object qrCodeData;
        private readonly IHttpClientFactory _httpClientFactory;

        public object GeoPositionAccuracy { get; private set; }

        //public SqlConnection conn;
        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment host, IConfiguration config, IEmployeesList empfc, IHttpClientFactory httpClientfactory , IFetchData fetchData)
        {
            _logger = logger;
            _host = host;
            _config = config;
            _empfc = empfc;
            _fetchData = fetchData;
            _httpClientFactory = httpClientfactory;
            //conn = new SqlConnection(_config["ConnectionStrings:Sql"]/*.GetSection("ConnectionStrings").GetSection("Sql").Value*/);
        }

        public async Task<IActionResult> Create(Employees employee)
        {
            if (employee.file != null) {
                string uploads = Path.Combine(_host.WebRootPath , "Uploads");
                string imgName = employee.file.FileName;
                string fullPath = Path.Combine(uploads, imgName);
                employee.file.CopyTo(new FileStream(fullPath, FileMode.Create));
                TempData["msg"] = _empfc.AddEmployee(employee);
                //return RedirectToAction("Employees");
               
            }
            HttpContext.Session.SetString("name", "MySession");
            CookieOptions cookie = new CookieOptions();
            cookie.Expires = DateTime.Now.AddDays(30);
            cookie.Secure = true;
            Response.Cookies.Append("MyCookie" , "Battach" , cookie);
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                TempData["Connection"] = "Your Device Is Connected To Internet!!!";
            }
            //string imgUrl = "";
            //string uploadDir = "";
            //using (MemoryStream ms = new MemoryStream())
            //{
            //    QRCodeGenerator qr = new QRCodeGenerator();
            //    QRCodeData qrData = qr.CreateQrCode("Battach", QRCodeGenerator.ECCLevel.Q);
            //    QRCode qrCode = new QRCode(qrData);
            //    using (Bitmap bitmap = qrCode.GetGraphic(20))
            //    {
            //        bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            //        uploadDir = Path.Combine(_host.WebRootPath, "Uploaded_IMG");
            //        imgUrl = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray()) + "";
            //        qrData.SaveRawData(uploadDir + "/result.png", QRCodeData.Compression.Uncompressed);

            //    }
            //}

            //var myBarcode = QRCodeWriter.CreateQrCode("12345");
            ////var resultFromFile = BarcodeReader.Read(@"file/barcode.png"); // From a file
            ////var resultFromPdf = BarcodeReader.ReadPdf(@"file/mydocument.pdf"); // From PDF use ReadPdf
            //myBarcode.ResizeTo(500, 500);
            ////myBarcode.ChangeBarCodeColor(System.Drawing.Color.AliceBlue, true);
            ////myBarcode.SaveAsImage("myBarcodeResized.jpeg");
            //string uploadDir = Path.Combine(_host.WebRootPath, "Uploads");
            //string filePath = Path.Combine(_host.WebRootPath, "Uploads/barCode.png");
            //myBarcode.SaveAsPng(filePath);
            //string fileName = Path.GetFileName(filePath);
            //string imgUrl = $"{this.Request.Scheme}://"+$"{this.Request.Host}{this.Request.PathBase}"+"/Uploads/"+fileName;

            string texthtml = "<Hello Battach> this is )))";
            TempData["text"] = WebUtility.HtmlEncode(texthtml);
            TempData["textcode"] = WebUtility.HtmlDecode(TempData["text"].ToString());


            Test test = new Test();
            //TempData["imgUrl"] = imgUrl;
            List<Students> students = new List<Students>();
            HttpClient client = _httpClientFactory.CreateClient();
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "https://localhost:44395/Home/Student");
            var response = await client.SendAsync(requestMessage);
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            students = JsonConvert.DeserializeObject<List<Students>>(jsonResponse);

            TempData["resp"] = students[1].name.ToString();
            //TempData["ipAddress"] = Request.HttpContext.Connection.RemoteIpAddress.ToString();
            HttpClient webClient = _httpClientFactory.CreateClient();
            var requestMsg = new HttpRequestMessage(HttpMethod.Get, "https://ipv4.icanhazip.com/");
            var request = await webClient.SendAsync(requestMsg);
            request.EnsureSuccessStatusCode();
            string ipAddress = await request.Content.ReadAsStringAsync();
            string key = "7EA34C9664AFBD70476DC8FB94F0812C";
            HttpClient webClient2 = _httpClientFactory.CreateClient();
            var requestMsg2 = new HttpRequestMessage(HttpMethod.Get, $"https://api.ip2location.io/?key={key}&ip={ipAddress}");
            var request2 = await webClient2.SendAsync(requestMsg2);
            request2.EnsureSuccessStatusCode();
            string resp = await request2.Content.ReadAsStringAsync();
            geoLocation location = JsonConvert.DeserializeObject<geoLocation>(resp);
            TempData["lat"] = location.latitude;
            TempData["long"] = location.longitude;
            Uri url = new Uri("https://jsonplaceholder.typicode.com/posts");
            string Posts = await _fetchData.fetch(url);
            JsonNode jsonPosts = JsonNode.Parse(Posts);
            TempData["post"] = jsonPosts["body"].ToString();
           
            return View();
        }
        public IActionResult Employees(int pg = 1)
        {

            //DataTable tbl = new DataTable();
            //SqlConnection conn = new SqlConnection(_config["ConnectionStrings:Sql"]);
            //try
            //{
            //    using (conn)
            //    {
            //        conn.Open();
            //        SqlDataAdapter ada = new SqlDataAdapter("SELECT * FROM employees", conn);
            //        ada.Fill(tbl);
            //        TempData["count"] = Math.Ceiling((decimal)tbl.Rows.Count / 3);
            //    }
            //}
            //catch (Exception)
            //{
            //    throw;
            //}
            TempData["Session"] = HttpContext.Session.GetString("name");
            TempData["Cookie"] = Request.Cookies["MyCookie"];
            TempData["pg"] = pg;
            TempData["count"] = _empfc.GetCount();
            return View(_empfc.GetEmployee(pg));
        }
        [HttpGet]
        public ContentResult Student(int pg = 1)
        {
            List<Students> std = new List<Students>() { new Students() { id = 1, name = "Morad", school = "Abdellah Ben Yassine", age = 23 },
                                                        new Students() { id = 2, name = "Abdelkabir", school = "Almotanabi", age = 19 },
                                                     new Students() { id = 3, name = "Jamal", school = "Rahal Ben Ahmed", age = 16 },
                                                     new Students {id = 4 , name = "Hamid" , school = "El Farouk" , age = 20}
                                                     };
            //var nm="";
            List<string> ln = new List<string>();
            /*for (int i = 0; i < std.Count; i++)
            {
                nm += "<h1>" + std[i].name +"<h1>";
            }*/
            //EmployeesList model = new EmployeesList();
            List<Employees> emp = _empfc.GetEmployee(pg);
            for (int i = 0; i < emp.Count; i++)
            {
                ln.Add("<tr><td>" + emp[i].id + "</td><td>" + emp[i].fname + "</td><td>" + emp[i].lname + "</td><td>" + emp[i].email + "</td><td><img style='width: 100px' src='/Uploads/" + emp[i].image + "'/></td></tr>");
            }
            var json = JsonConvert.SerializeObject(std);
            return Content(json);
        }
        public IActionResult pdfSharp()
        {
            QuestPDF.Settings.License = LicenseType.Community;
            List<Students> std = new List<Students>() { new Students() { id = 1, name = "Morad", school = "Abdellah Ben Yassine", age = 23 },
                                                        new Students() { id = 2, name = "Abdelkabir", school = "Almotanabi", age = 19 },
                                                     new Students() { id = 3, name = "Jamal", school = "Rahal Ben Ahmed", age = 16 },
                                                     new Students {id = 4 , name = "Hamid" , school = "El Farouk" , age = 20},
                                                     new Students {id = 5 , name = "Khadija" , school = "Al Farabi School Groups" , age = 26}
                                                     };
            //Test pdf = new Test();
            //string uploads = Path.Combine(_host.WebRootPath, "Uploads/58f8bd900ed2bdaf7c12830c.png");
            //int age = 0;
            //foreach (var item in std){
            //    age += item.age;
            //}
            //var document = pdf.HtmlPdf(std,uploads,age);
            //Invoice invoice = new Invoice();
            //var pdf = invoice.questPdf();
            //var document = pdf.GeneratePdf();


            var model = InvoiceDocumentDataSource.GetInvoiceDetails();
            string uploadDir = Path.Combine(_host.WebRootPath, "Uploads/barCode.png");
            string uploadDir2 = Path.Combine(_host.WebRootPath, "Uploads/inwi.jpg");
            //byte[] imageData = File.ReadAllBytes("https://www.google.com/url?sa=i&url=https%3A%2F%2Fwww.base64-image.de%2F&psig=AOvVaw1xssdrX7cR3dGwkERY0XQF&ust=1741089927325000&source=images&cd=vfe&opi=89978449&ved=0CBQQjRxqFwoTCJjcmcfv7YsDFQAAAAAdAAAAABAE");
            var pdf = new InvoiceDocument(std, uploadDir);
            var pdf2 = new InvoicePDF(uploadDir2);
            var document = pdf2.GeneratePdf();

            //byte[]? response = null;
            //using (MemoryStream ms = new MemoryStream())
            //{
            ////    document.Save(ms);
            //    response = ms.ToArray();
            //}
            //string fileName = "FeesStructure.pdf";
            //return File(document, "application/pdf", fileName);

            //MemoryStream stream = new MemoryStream();
            //response = stream.ToArray();
            //document.Save(stream);
            //Response.ContentType = "application/pdf";
            //Response.Headers.Add("content-length" , stream.Length.ToString());
            //byte[] bytes = stream.ToArray();
            //stream.Position = 0;
            //stream.Close();
            Attachment file = new Attachment(new MemoryStream(document), "Battach.pdf", "application/pdf");


            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.EnableSsl = true;
            smtp.Credentials = new NetworkCredential("battachofficial@gmail.com", "tzww dmoq yxlq sbvz");

            MailMessage message = new MailMessage();
            message.To.Add("tindouf.tiff@gmail.com"); // Add Receiver mail Address  
            message.From = new MailAddress("battachofficial@gmail.com"); // Sender address  
            message.Subject = "Confirmation Email";
            message.Attachments.Add(file);

            message.IsBodyHtml = true; //HTML email  
            message.Body = "Congratulation!!!, <a href='http://localhost:5170/Home/HomePage'>your email has been confirmed succsessfully.</a>";
            smtp.Send(message);

            return View();
        }
        private async Task<string> AccessToken()
        {
            string token = "";
            using (var client = new HttpClient())
            {
                string credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes("AfbRXnSQKzl1z5Z0uQeSeP0PNw6rRHpaMFthljl8EvybRQOXsvaYV44iaQgmMlyZ1V8vuuWHtBwsXwQM:EII4xPx16UaJ2qqy9bMhOfbt0ORbP849j7m7r18OSNXA9-yV2EHxRSXoU02obwM4DYvPfBib6JCWyfQ9"));
                client.DefaultRequestHeaders.Add("Authorization" , "Basic " + credentials);
                var requestMessage = new HttpRequestMessage(HttpMethod.Post , "https://api-m.sandbox.paypal.com/v1/oauth2/token");
                requestMessage.Content = new StringContent("grant_type=client_credentials", null , "application/x-www-form-urlencoded");
                var httpResponse = await client.SendAsync(requestMessage);
                if (httpResponse.IsSuccessStatusCode)
                {
                    var strResponse = await httpResponse.Content.ReadAsStringAsync();
                    var jsonResponse = JsonNode.Parse(strResponse);
                    if (jsonResponse != null)
                    {
                        token = jsonResponse["access_token"]?.ToString() ?? ""; 
                    }
                }
            }
                return token;
        }
        [HttpPost]
        public async Task<JsonResult> createOrder([FromBody] JsonObject data)
        {
            var totalAmount = data?["amount"]?.ToString();
            if (totalAmount == null)
            {
                return Json(new { Id = "" });
            }

            JsonObject createOrder = new JsonObject();
            createOrder.Add("intent", "CAPTURE");
            JsonObject amount = new JsonObject();
            amount.Add("currency_code", "USD");
            amount.Add("value", totalAmount);
            JsonObject purchase = new JsonObject();
            purchase.Add("amount", amount);
            JsonArray purchaseUnits = new JsonArray();
            purchaseUnits.Add(purchase);
            createOrder.Add("purchase_units" , purchaseUnits);
            string token = await AccessToken();
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization" , "Bearer " + token);
                var requestMessage = new HttpRequestMessage(HttpMethod.Post , "https://api-m.sandbox.paypal.com/v2/checkout/orders");
                requestMessage.Content = new StringContent(createOrder.ToString(), null , "application/json");
                var httpResponse = await client.SendAsync(requestMessage);
                if (httpResponse.IsSuccessStatusCode)
                {
                    var strResponse = await httpResponse.Content.ReadAsStringAsync();
                    var jsonResponse = JsonNode.Parse(strResponse);
                    if (jsonResponse != null)
                    {
                        string orderId = jsonResponse["id"]?.ToString();
                        return Json(new {Id = orderId});
                    }
                }
            }

                return Json(new { Id = "" });
        }
        [HttpPost]
        public async Task<JsonResult> capturePayment([FromBody] JsonObject data)
        {
            var orderId = data?["orderId"]?.ToString();
            if (orderId == null)
            {
                return Json("error");
            }
            string token = await AccessToken();
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization" , "Bearer " + token);
                var requestMessage = new HttpRequestMessage(HttpMethod.Post , "https://api-m.sandbox.paypal.com/v2/checkout/orders/"+orderId+"/capture");
                requestMessage.Content = new StringContent("", null , "application/json");
                var httpResponse = await client.SendAsync(requestMessage);
                if (httpResponse.IsSuccessStatusCode)
                {
                    var strResponse = await httpResponse.Content.ReadAsStringAsync();
                    var jsonResponse = JsonNode.Parse(strResponse);
                    if (jsonResponse != null)
                    {
                        string paypalOrderId = jsonResponse["status"]?.ToString();

                        Payer payer = new Payer();
                        payer.Id = jsonResponse["purchase_units"][0]["payments"]["captures"][0]["id"].ToString(); ;
                        payer.FirstName = jsonResponse["payment_source"]["paypal"]["name"]["given_name"].ToString();
                        payer.LastName = jsonResponse["payment_source"]["paypal"]["name"]["surname"].ToString();
                        payer.Price = jsonResponse["purchase_units"][0]["payments"]["captures"][0]["amount"]["value"].ToString() +" "+ jsonResponse["purchase_units"][0]["payments"]["captures"][0]["amount"]["currency_code"].ToString();
                        payer.Status = jsonResponse["purchase_units"][0]["payments"]["captures"][0]["status"].ToString();
                        //payer.Address = jsonResponse["purchase_units"][0]["shipping"]["address"]["country_code"].ToString(); //+ " , " + jsonResponse["purchase_units"][0]["shipping"]["address"]["country_code"].ToString() + ", " + jsonResponse["purchase_units"][0]["shipping"]["address"]["postal_code"].ToString();
                        payer.Address = jsonResponse["purchase_units"][0]["reference_id"].ToString();
                        //Send PDF tO email

                        Test pdf = new Test();
                        var document = pdf.paypalPdf(payer);
                        MemoryStream stream = new MemoryStream();
                        document.Save(stream);
                        //Response.ContentType = "application/pdf";
                        //Response.Headers.Add("content-length" , stream.Length.ToString());
                        //byte[] bytes = stream.ToArray();
                        stream.Position = 0;
                        //stream.Close();
                        Attachment file = new Attachment(stream, "Battach.pdf", "application/pdf");


                        SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                        smtp.EnableSsl = true;
                        smtp.Credentials = new NetworkCredential("battachofficial@gmail.com", "tzww dmoq yxlq sbvz");

                        MailMessage message = new MailMessage();
                        message.To.Add("tindouf.tiff@gmail.com"); // Add Receiver mail Address  
                        message.From = new MailAddress("battachofficial@gmail.com"); // Sender address  
                        message.Subject = "Confirmation Email";
                        message.Attachments.Add(file);

                        message.IsBodyHtml = true; //HTML email  
                        message.Body = "Congratulation!!!, <a href='http://localhost:5170/Home/HomePage'>your email has been confirmed succsessfully.</a>";
                        smtp.Send(message);


                        if (paypalOrderId == "COMPLETED")
                        {
                            return Json("success");
                        }

                    }
                }
            }
                return Json("error");
        }
        public async Task<IActionResult> refundPayment()
        {
            string token = await AccessToken();
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                client.DefaultRequestHeaders.Add("Prefer", "return=representation");
                //client.DefaultRequestHeaders.Add("PayPal-Request-Id", "");
                var requestMessage = new HttpRequestMessage(HttpMethod.Post, "https://api-m.sandbox.paypal.com/v2/payments/captures/4TJ367953E343891U/refund");
                requestMessage.Content = new StringContent("", null, "application/json");
                var httpResponse = await client.SendAsync(requestMessage);
                if (httpResponse.IsSuccessStatusCode)
                {
                    var strResponse = await httpResponse.Content.ReadAsStringAsync();
                    var jsonResponse = JsonNode.Parse(strResponse);
                    if (jsonResponse != null)
                    {
                        TempData["id"] = jsonResponse["id"].ToString();
                    }
                }
            }
                    return View();
        }
        public async Task<IActionResult> dataFetch()
        {
            List<Students> dataList = new List<Students>();
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage response = await client.GetAsync("https://localhost:44395/Home/Student"))
                {
                    using (HttpContent content = response.Content)
                    {
                        string myContent = await content.ReadAsStringAsync();
                        dataList = JsonConvert.DeserializeObject<List<Students>>(myContent);

                    }
                }
            }
                return View(dataList);
        }
        public async Task<IActionResult> tokenApi()
        {
            JsonObject obj = new JsonObject();
            obj.Add("username", "Admin");
            obj.Add("password", "Admin123");
            HttpClient client = _httpClientFactory.CreateClient();
            var requestMsg = new HttpRequestMessage(HttpMethod.Post, "https://localhost:7031/Api/Auth/Login");
            requestMsg.Content = new StringContent(obj.ToString(), null, "application/json");
            var request = await client.SendAsync(requestMsg);
            request.EnsureSuccessStatusCode();
            string response = await request.Content.ReadAsStringAsync();
            token token = JsonConvert.DeserializeObject<token>(response);
            TempData["token"] = token.Token;
            return View();
        }
        public IActionResult Test()
        {
            return View();
        }
        public ContentResult Test2(Test test)
        {
            //string name = Request.Form["name"];
            //string type = Request.ContentType;
            string[] ext = new string[] {"jpg","jpeg"};
            FileInfo fInfo;
            string fileName = string.Empty;
            if (test.file_name != null)
            {
                fInfo = new FileInfo(test.file_name.FileName);
                string extension = fInfo.Extension.ToLower().Substring(1,fInfo.Extension.Length - 1);
                if (ext.Contains(extension))
                {
                    string uploadDir = Path.Combine(_host.WebRootPath, "Uploaded_IMG");
                    if (!Directory.Exists(uploadDir))
                    {
                        Directory.CreateDirectory(uploadDir);
                    }
                   string[] files = Directory.GetFiles(uploadDir);
                    string fName = Guid.NewGuid().ToString() + "_" + test.file_name.FileName;
                    string filePath = Path.Combine(uploadDir, fName);
                    for (int i = 0; i < files.Length; i++)
                    {
                        string fname = Path.GetFileName(files[i]);
                        fileName += "<img style='height: 200px; width: 200px; object-fit: cover' src='/Uploaded_IMG/" + fname + "'/>";
                    }
                    //fileName = "<p>Hello "+test.name+"<p/><img style=' height: 200px; width: 200px;' src='/Uploaded_IMG/" + fName + "'/>";
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        test.file_name.CopyTo(fileStream);
                    }
                }else
                {
                    fileName = "<p>Hello "+test.name+"<p/><p style='color: red;'>This Type Of File Doesn't Allowed On This Website !!!<p/>";
                }
            }
            return Content(fileName);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}