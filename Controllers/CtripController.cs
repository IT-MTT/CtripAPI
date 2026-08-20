using CtripAPI.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using static System.Net.WebRequestMethods;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CtripAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CtripController : ControllerBase
    {
        [HttpPost]
        [Route("{Channel}/productquery/2.0/{timeStamp}/{sign}")]
        public async Task<JsonResult> QueryProductInterface(string Channel, string timeStamp, string sign)
        {
            try
            {
                string bodycontent;
                using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
                {
                    bodycontent = await reader.ReadToEndAsync();
                }
                string hexCipher = bodycontent;
                string key = "12345678";
                string decryptedText = DesEcbHexDecryptor.DecryptFromHex(hexCipher, key, PaddingMode.PKCS7);
                string file = AppContext.BaseDirectory + @"\LogFile.txt";
                System.IO.File.AppendAllText(file, decryptedText + Environment.NewLine);
                QueryProductRequestRoot obj = JsonSerializer.Deserialize<QueryProductRequestRoot>(decryptedText);
                //from database get the vehicle types which have thirdpartyvehicletypeid is not null and thirdpartyvehicletypeid!=0
                //getting fares using above request obj through your api of those above matched vehicles
                //getting fares response in List<QueryResultList>
                QueryProductResponseRoot response = new QueryProductResponseRoot();
                string guid = Guid.NewGuid().ToString("N").ToUpper().Substring(0, 16);
                response.msgCode = "OK";
                response.message = "success";
                response.priceMark = guid;
                response.currency = "GBP";
                //now showing only one vehicle response but may be multiple
                QueryResultList objQueryResultList = new QueryResultList();
                objQueryResultList.addServices = new List<AddService>{new AddService
                {
                    maxCount = 1,
                    vendorAddServiceCode = "DR-ZH"
                }};
                objQueryResultList.price = 100.0m;
                objQueryResultList.vehicleType = 117; //thirdpartyvehucletypeid against ctrip matched vehicle 
                response.queryResultList = new List<QueryResultList>();
                response.queryResultList.Add(objQueryResultList);
                //save the response of ctrip product query api in database table     
                System.IO.File.AppendAllText(file, JsonSerializer.Serialize(response) + Environment.NewLine);
                return new JsonResult(response);
            }
            catch (Exception ex)
            {
                var response = new QueryProductResponseRoot
                {
                    msgCode = "NO_TIMESERVICE",
                    message = "提前预定时间过短",
                    priceMark = "",
                    currency = "",
                    queryResultList = new List<QueryResultList>()
                };
                return new JsonResult(response);
            }
        }
        [HttpPost]
        [Route("{Channel}/ordercreate/2.0/{timeStamp}/{sign}")]
        public async Task<JsonResult> CreateOrderInterface(string Channel, string timeStamp, string sign)
        {
            try
            {
                string bodycontent = "";
                using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
                {
                    bodycontent = await reader.ReadToEndAsync();
                }
                string hexCipher = bodycontent;
                string key = "12345678";
                string decryptedText = DesEcbHexDecryptor.DecryptFromHex(hexCipher, key, PaddingMode.PKCS7);
                string file = AppContext.BaseDirectory + @"\LogFile.txt";
                System.IO.File.AppendAllText(file, decryptedText + Environment.NewLine);
                CreateOrderRequest obj = JsonSerializer.Deserialize<CreateOrderRequest>(decryptedText);
                //first check either ctripPurchaseOrderId exist
                //if exist then response with current jobid in the below object CreateOrderResponse with OK code and Success message 
                //get the categoryCode from request object
                //if categoryCode contains airport then fromLocType variable set to Airport else Address
                //get the VehicleType name from database table having vehicle details and ThirdPartyVehicleTypeId is equal to request object vehicleType
                //get the request object useTime and get in DateTime and get the formattedDate dd-MMM-yyyy in variable
                //In our dispatch we are updating priceMark in request object priceMark property  and ctripPurchaseOrderId in request object ctripPurchaseOrderId property and FromDoorNo equals (request object inside it flightInfo object and inside it flightNumber property) in booking table
                //getting response and set in response object like below sample
                //Call ConfirmOrder which is defined below to confirm this order to ctrip and get the response
                //ConfirmOrder object while calling ConfirmOrder which have different properties set vendorOrderID which is bookingid generated in our end and connectTelCode +86 and connectTel as CustomerMobileNo and pickUpLocationDesc as pickupaddress and ctripPurchaseOrderId as ctripPurchaseOrderId of booking
                //below sample to call ConfirmOrder
                ConfirmOrder objConfirmOrder = new ConfirmOrder();
                objConfirmOrder.connectTel = "3434245665";
                objConfirmOrder.connectTelCode = "+86";
                objConfirmOrder.ctripPurchaseOrderID = obj.ctripPurchaseOrderId;
                objConfirmOrder.pickUpLocationDesc = obj.duseLocation.address;
                objConfirmOrder.pickUpLocationImgUrl = "";
                objConfirmOrder.vendorOrderID = "424323432";
                objConfirmOrder.Channel = Channel;
                var result= await ConfirmOrder(objConfirmOrder);
                CreateOrderResponse response = new CreateOrderResponse();
                response.message = "Success";
                response.msgCode = "OK";
                response.vendorOrderId = "424323432";
                System.IO.File.AppendAllText(file, JsonSerializer.Serialize(response) + Environment.NewLine);
                return new JsonResult(response);
            }
            catch (Exception ex)
            {
                var response = new CreateOrderResponse { msgCode = "ERROR_NO_PRICEMARK", message = "价格标识符不存在" };
                return new JsonResult(response);
            }
        }
        [HttpPost]
        [Route("{Channel}/updatepassengerinfo/2.0/{timeStamp}/{sign}")]
        public async Task<JsonResult> UpdateOrderInterface(string Channel, string timeStamp, string sign)
        {
            try
            {
                string bodycontent = "";
                using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
                {
                    bodycontent = await reader.ReadToEndAsync();
                }
                string hexCipher = bodycontent;
                string key = "12345678";
                string decryptedText = DesEcbHexDecryptor.DecryptFromHex(hexCipher, key, PaddingMode.PKCS7);
                string file = AppContext.BaseDirectory + @"\LogFile.txt";
                System.IO.File.AppendAllText(file, decryptedText + Environment.NewLine);
                UpdateOrderRequest obj = JsonSerializer.Deserialize<UpdateOrderRequest>(decryptedText);
                UpdateOrderResponse response = new UpdateOrderResponse();
                //update the information in the database
                response.msgCode = "OK";
                response.message = "success";
                System.IO.File.AppendAllText(file, JsonSerializer.Serialize(response) + Environment.NewLine);
                return new JsonResult(response);
            }
            catch (Exception ex)
            {
                var response = new UpdateOrderResponse { msgCode = "ERROR_NO_ORDERID", message = "找不到订单" };
                return new JsonResult(response);
            }
        }
        [HttpPost]
        [Route("{Channel}/querydriverlocation/2.0/{timeStamp}/{sign}")]
        public async Task<JsonResult> QueryDriverLocationInterface(string Channel, string timeStamp, string sign)
        {
            try
            {
                string bodycontent = "";
                using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
                {
                    bodycontent = await reader.ReadToEndAsync();
                }
                string hexCipher = bodycontent;
                string key = "12345678";
                string decryptedText = DesEcbHexDecryptor.DecryptFromHex(hexCipher, key, PaddingMode.PKCS7);
                string file = AppContext.BaseDirectory + @"\LogFile.txt";
                System.IO.File.AppendAllText(file, decryptedText + Environment.NewLine);
                QueryDriverLocationRequest obj = JsonSerializer.Deserialize<QueryDriverLocationRequest>(decryptedText);
                QueryDriverLocationResponse response = new QueryDriverLocationResponse();
                var objDriverLocationInfo = new DriverLocationInfo() { coordinate = "WGS84", latitude = 24.9282936,longitude= 67.1565157,locationType=3, locationTime = "2026-07-27 10:07:24.743", direction = 511, speed = 0.07813829 };
                var list = new List<DriverLocationInfo>();
                list.Add(objDriverLocationInfo);
                response.orderDriverLocationInfo = new List<OrderDriverLocationInfo>();
                response.orderDriverLocationInfo.Add(new OrderDriverLocationInfo() { ctripPurchaseOrderId = obj.orderInfo[0].ctripPurchaseOrderId, message = "SUCCESS", msgCode = "OK", driverLocationInfo = list });
                response.msgCode = "OK";
                response.message = "success";
                System.IO.File.AppendAllText(file, JsonSerializer.Serialize(response) + Environment.NewLine);
                return new JsonResult(response);
            }
            catch (Exception ex)
            {
                var response = new QueryDriverLocationResponse
                {
                    msgCode = "ERROR_NO_DRIVER_LOCATION",
                    message = "",
                    orderDriverLocationInfo = new List<OrderDriverLocationInfo>()
                };
                return new JsonResult(response);
            }
        }
        [HttpPost]
        [Route("{Channel}/ordercancel/2.0/{timeStamp}/{sign}")]
        public async Task<JsonResult> CancelOrderInterface(string Channel, string timeStamp, string sign)
        {
            try
            {
                string bodycontent = "";
                using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
                {
                    bodycontent = await reader.ReadToEndAsync();
                }
                string hexCipher = bodycontent;
                string key = "12345678";
                string decryptedText = DesEcbHexDecryptor.DecryptFromHex(hexCipher, key, PaddingMode.PKCS7);
                string file = AppContext.BaseDirectory + @"\LogFile.txt";
                System.IO.File.AppendAllText(file, decryptedText + Environment.NewLine);
                CancelOrderRequest obj = JsonSerializer.Deserialize<CancelOrderRequest>(decryptedText);
                //Check first if job already cancelled response with error code
                //Check secondly if job already completed return response with error code
                //Else We are processing cancel job operations in dispatch.
                CancelOrderResponse response = new CancelOrderResponse();
                response.msgCode = "OK";
                response.message = "success";
                System.IO.File.AppendAllText(file, JsonSerializer.Serialize(response) + Environment.NewLine);
                return new JsonResult(response);
            }
            catch (Exception ex)
            {
                var response = new CancelOrderResponse { msgCode = "ERROR_NO_ORDERID", message = "can not find order" };
                return new JsonResult(response);
            }
        }
        [HttpPost]
        [Route("{Channel}/orderdetail/2.0/{timeStamp}/{sign}")]
        public async Task<JsonResult> OrderDetailInterface(string Channel, string timeStamp, string sign)
        {
            try
            {
                string bodycontent = "";
                using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
                {
                    bodycontent = await reader.ReadToEndAsync();
                }
                var hexCipher  = bodycontent;
                string key = "12345678";
                string decryptedText = DesEcbHexDecryptor.DecryptFromHex(hexCipher, key, PaddingMode.PKCS7);
                string file = AppContext.BaseDirectory + @"\LogFile.txt";
                System.IO.File.AppendAllText(file, decryptedText + Environment.NewLine);
                OrderDetailRequest obj = JsonSerializer.Deserialize<OrderDetailRequest>(decryptedText);
                OrderDetailResponse response = new OrderDetailResponse();
                response.msgCode = "OK";
                response.message = "SUCCESS";
                response.ctripPurchaseOrderId = obj.ctripPurchaseOrderId;
                response.vendorOrderId = obj.vendorOrderId;
                response.totalFee = 100;
                //fill details in the response object
                System.IO.File.AppendAllText(file, JsonSerializer.Serialize(response) + Environment.NewLine);
                return new JsonResult(response);
            }
            catch (Exception ex)
            {
                var response = new OrderDetailResponse { msgCode = "ERROR_NO_ORDERID", message = "找不到订单" };
                return new JsonResult(response);
            }
        }

        [HttpPost]
        [Route("JNT/createorderevent/2.0/{timeStamp}/{sign}")]
        public async Task<JsonResult> CreateReplyEventInterface(string timeStamp, string sign)
        {
            try
            {
                string bodycontent = "";
                using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
                {
                    bodycontent = await reader.ReadToEndAsync();
                }
                string hexCipher = bodycontent;
                string key = "12345678";
                string decryptedText = DesEcbHexDecryptor.DecryptFromHex(hexCipher, key, PaddingMode.PKCS7);
                string file = AppContext.BaseDirectory + @"\LogFile.txt";
                System.IO.File.AppendAllText(file, decryptedText + Environment.NewLine);
                CreateReplyEventRequest obj = JsonSerializer.Deserialize<CreateReplyEventRequest>(decryptedText);
                CreateReplyEventResponse response = new CreateReplyEventResponse();
                response.msgCode = "OK";
                response.message = "Success";
                response.vendorEventId = "21139262";
                System.IO.File.AppendAllText(file, decryptedText + Environment.NewLine);
                return new JsonResult(response);
            }
            catch (Exception ex)
            {
                var response = new CreateReplyEventResponse { msgCode = "Error", message = "" };
                return new JsonResult(response);
            }
        }

        [HttpPost]
        [Route("ConfirmOrder")]
        public async Task<IActionResult> ConfirmOrder(ConfirmOrder obj)
        {
            try
            {
                var ctripposturl = "https://testapi-car.ctrip.com/chvendormessagebus/sandbox";
                var channel = obj.Channel;//e.g JNT means airport/station - pick-up or drop-off services. 
                var vendorid = "1005327";//i have got this from dispatch system
                var ctrippurchaseorderid = obj.ctripPurchaseOrderID;
                var timeStamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
                string key = "12345678";
                var options = new JsonSerializerOptions
                {
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, // Prevents escaping +
                    WriteIndented = true
                };

                string json = JsonSerializer.Serialize(obj, options);
                var encrypted = DesEcbHexDecryptor.Encrypt(json, key);
                string sign = MD5Signature.ComputeMd5Hash(vendorid + "2.0" + channel + timeStamp + key + encrypted.Length.ToString()).ToLower();
                var url = string.Format("{0}/{1}-{2}/order/confirm/2.0/{3}/{4}/{5}", ctripposturl, channel, vendorid, ctrippurchaseorderid, timeStamp, sign);
                // 2. Initialize HttpClient (preferably reused in real applications)
                using HttpClient client = new HttpClient();
                // 3. Create the payload object
                var payload = encrypted;
                // 4. Send the POST request
                HttpResponseMessage response = await client.PostAsJsonAsync(url, payload);
                string responseBody = await response.Content.ReadAsStringAsync();
                string file = AppContext.BaseDirectory + @"\LogFile.txt";
                System.IO.File.AppendAllText(file, url + Environment.NewLine + responseBody + Environment.NewLine);
                return Content(responseBody, "application/json");
            }
            catch (Exception ex)
            {
                var error = new { msgCode = "Error", message = ex.Message };
                return new JsonResult(error);
            }
        }
        [HttpPost]
        [Route("DriverInfoPush")]
        public async Task<IActionResult> DriverInfoPush(DriverInfoPush obj)
        {
            try
            {
                var ctripposturl = "https://testapi-car.ctrip.com/chvendormessagebus/sandbox";
                var channel = obj.Channel;//e.g JNT means airport/station - pick-up or drop-off services, RTN means taxi.
                var vendorid = "1005327";//i have got this from dispatch system
                var ctrippurchaseorderid = obj.ctripPurchaseOrderID;
                var timeStamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
                string version = "2.0";
                string key = "12345678";
                var options = new JsonSerializerOptions
                {
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, // Prevents escaping +
                    WriteIndented = true
                };
                string json = JsonSerializer.Serialize(obj, options);
                string encrypted = DesEcbHexDecryptor.Encrypt(json, key);
                string sign = MD5Signature.ComputeMd5Hash(vendorid + version + channel + timeStamp + key + encrypted.Length.ToString()).ToLower();
                var url = string.Format("{0}/{1}-{2}/driver/push/2.0/{3}/{4}/{5}", ctripposturl, channel, vendorid, ctrippurchaseorderid, timeStamp, sign);
                // 2. Initialize HttpClient (preferably reused in real applications)
                using HttpClient client = new HttpClient();
                // 3. Create the payload object
                var payload = encrypted;
                // 4. Send the POST request
                HttpResponseMessage response = await client.PostAsJsonAsync(url, payload);
                string responseBody = await response.Content.ReadAsStringAsync();
                string file = AppContext.BaseDirectory + @"\LogFile.txt";
                System.IO.File.AppendAllText(file, url + Environment.NewLine + responseBody + Environment.NewLine);
                return Content(responseBody, "application/json");
            }
            catch (Exception ex)
            {
                var error = new { msgCode = "Error", message = ex.Message };
                return new JsonResult(error);
            }
        }

        [HttpPost]
        [Route("DriverStatusPush")]
        public async Task<IActionResult> DriverStatusPush(DriverStatusPush obj)
        {
            try
            {
                var ctripposturl = "https://testapi-car.ctrip.com/chvendormessagebus/sandbox";//ctrip test url
                var channel = obj.Channel;//e.g JNT means airport/station - pick-up or drop-off services, RTN means taxi.
                var vendorid = "1005327";//i have got this from dispatch system
                var ctrippurchaseorderid = obj.ctripPurchaseOrderID;
                var timeStamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
                string version = "2.0";
                string key = "12345678";
                var options = new JsonSerializerOptions
                {
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, // Prevents escaping +
                    WriteIndented = true
                };
                string json = JsonSerializer.Serialize(obj, options);
                string encrypted = DesEcbHexDecryptor.Encrypt(json, key);
                string sign = MD5Signature.ComputeMd5Hash(vendorid + version + channel + timeStamp + key + encrypted.Length.ToString()).ToLower();
                var url = string.Format("{0}/{1}-{2}/driver/inplace/2.0/{3}/{4}/{5}", ctripposturl, channel, vendorid, ctrippurchaseorderid, timeStamp, sign);
                // 2. Initialize HttpClient (preferably reused in real applications)
                using HttpClient client = new HttpClient();
                // 3. Create the payload object
                var payload = encrypted;
                // 4. Send the POST request
                HttpResponseMessage response = await client.PostAsJsonAsync(url, payload);
                string responseBody = await response.Content.ReadAsStringAsync();
                string file = AppContext.BaseDirectory + @"\LogFile.txt";
                System.IO.File.AppendAllText(file, url + Environment.NewLine + responseBody + Environment.NewLine);
                return Content(responseBody, "application/json");
            }
            catch (Exception ex)
            {
                var error = new { msgCode = "Error", message = ex.Message };
                return new JsonResult(error);
            }
        }
        [HttpPost]
        [Route("OrderComplete")]
        public async Task<IActionResult> OrderComplete(OrderCompleteRequest obj)
        {
            try
            {
                var ctripposturl = "https://testapi-car.ctrip.com/chvendormessagebus/sandbox";//ctrip test url
                var channel = obj.Channel;//e.g JNT means airport/station - pick-up or drop-off services, RTN means taxi.
                var vendorid = "1005327";//i have got this from dispatch system
                var ctrippurchaseorderid = obj.ctripPurchaseOrderID;
                var timeStamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
                string version = "2.0";
                string key = "12345678";
                var options = new JsonSerializerOptions
                {
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, // Prevents escaping +
                    WriteIndented = true
                };
                string json = JsonSerializer.Serialize(obj, options);
                string encrypted = DesEcbHexDecryptor.Encrypt(json, key);
                string sign = MD5Signature.ComputeMd5Hash(vendorid + version + channel + timeStamp + key + encrypted.Length.ToString()).ToLower();
                var url = string.Format("{0}/{1}-{2}/order/complete/2.0/{3}/{4}/{5}", ctripposturl, channel, vendorid, ctrippurchaseorderid, timeStamp, sign);
                // 2. Initialize HttpClient (preferably reused in real applications)
                using HttpClient client = new HttpClient();
                // 3. Create the payload object
                var payload = encrypted;
                // 4. Send the POST request
                HttpResponseMessage response = await client.PostAsJsonAsync(url, payload);
                string responseBody = await response.Content.ReadAsStringAsync();
                string file = AppContext.BaseDirectory + @"\LogFile.txt";
                System.IO.File.AppendAllText(file, url + Environment.NewLine + responseBody + Environment.NewLine);
                return Content(responseBody, "application/json");
            }
            catch (Exception ex)
            {
                var error = new { msgCode = "Error", message = ex.Message };
                return new JsonResult(error);
            }
        }

        [HttpPost]
        [Route("OrderDetails")]
        public async Task<IActionResult> OrderDetails(OrderDetailsRequest obj)
        {
            try
            {
                var ctripposturl = "https://testapi-car.ctrip.com/chvendormessagebus/sandbox";//ctrip test url
                var channel = obj.Channel;//e.g JNT means airport/station - pick-up or drop-off services, RTN means taxi.
                var vendorid = "1005327";//i have got this from dispatch system
                var ctrippurchaseorderid = obj.ctripPurchaseOrderID;
                var timeStamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
                string version = "2.0";
                string key = "12345678";
                var options = new JsonSerializerOptions
                {
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, // Prevents escaping +
                    WriteIndented = true
                };
                string json = JsonSerializer.Serialize(obj, options);
                string encrypted = DesEcbHexDecryptor.Encrypt(json, key);
                string sign = MD5Signature.ComputeMd5Hash(vendorid + version + channel + timeStamp + key + encrypted.Length.ToString()).ToLower();
                var url = string.Format("{0}/{1}-{2}/order/detail/2.0/{3}/{4}/{5}", ctripposturl, channel, vendorid, ctrippurchaseorderid, timeStamp, sign);
                // 2. Initialize HttpClient (preferably reused in real applications)
                using HttpClient client = new HttpClient();
                // 3. Create the payload object
                var payload = encrypted;
                // 4. Send the POST request
                HttpResponseMessage response = await client.PostAsJsonAsync(url, payload);
                string responseBody = await response.Content.ReadAsStringAsync();
                string file = AppContext.BaseDirectory + @"\LogFile.txt";
                System.IO.File.AppendAllText(file, url + Environment.NewLine + responseBody + Environment.NewLine);
                return Content(responseBody, "application/json");
            }
            catch (Exception ex)
            {
                var error = new { msgCode = "Error", message = ex.Message };
                return new JsonResult(error);
            }
        }

        [HttpPost]
        [Route("CannotService")]
        public async Task<IActionResult> CannotService(CannotServiceRequest obj)
        {
            try
            {
                var ctripposturl = "https://testapi-car.ctrip.com/chvendormessagebus/sandbox";//ctrip test url
                var channel = obj.Channel;//e.g JNT means airport/station - pick-up or drop-off services, RTN means taxi.
                var vendorid = "1005327";//i have got this from dispatch system
                var ctrippurchaseorderid = obj.ctripPurchaseOrderID;
                var timeStamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
                string version = "2.0";
                string key = "12345678";
                var options = new JsonSerializerOptions
                {
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, // Prevents escaping +
                    WriteIndented = true
                };
                string json = JsonSerializer.Serialize(obj, options);
                string encrypted = DesEcbHexDecryptor.Encrypt(json, key);
                string sign = MD5Signature.ComputeMd5Hash(vendorid + version + channel + timeStamp + key + encrypted.Length.ToString()).ToLower();
                var url = string.Format("{0}/{1}-{2}/order/cancel/2.0/{3}/{4}/{5}", ctripposturl, channel, vendorid, ctrippurchaseorderid, timeStamp, sign);
                // 2. Initialize HttpClient (preferably reused in real applications)
                using HttpClient client = new HttpClient();
                // 3. Create the payload object
                var payload = encrypted;
                // 4. Send the POST request
                HttpResponseMessage response = await client.PostAsJsonAsync(url, payload);
                string responseBody = await response.Content.ReadAsStringAsync();
                string file = AppContext.BaseDirectory + @"\LogFile.txt";
                System.IO.File.AppendAllText(file, url + Environment.NewLine + responseBody + Environment.NewLine);
                return Content(responseBody, "application/json");
            }
            catch (Exception ex)
            {
                var error = new { msgCode = "Error", message = ex.Message };
                return new JsonResult(error);
            }
        }

        [HttpPost]
        [Route("PartnerCreateReplyEvent")]
        public async Task<IActionResult> PartnerCreateReplyEvent(PartnerCreateReplyRequest obj)
        {
            try
            {
                var ctripposturl = "https://testapi-car.ctrip.com/chvendormessagebus/sandbox";//ctrip test url
                var channel = "JNT";//this endpoint's URL is hardcoded to the JNT channel below
                var vendorid = "1005327";//i have got this from dispatch system
                var ctrippurchaseorderid = obj.ctripPurchaseOrderId;
                var timeStamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
                string version = "2.0";
                string key = "12345678";
                var options = new JsonSerializerOptions
                {
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, // Prevents escaping +
                    WriteIndented = true
                };
                string json = JsonSerializer.Serialize(obj, options);
                string encrypted = DesEcbHexDecryptor.Encrypt(json, key);
                string sign = MD5Signature.ComputeMd5Hash(vendorid + version + channel + timeStamp + key + encrypted.Length.ToString()).ToLower();
                var url = string.Format("{0}/JNT-{1}/event/push/2.0/{2}/{3}/{4}", ctripposturl, vendorid, ctrippurchaseorderid, timeStamp, sign);
                // 2. Initialize HttpClient (preferably reused in real applications)
                using HttpClient client = new HttpClient();
                // 3. Create the payload object
                var payload = encrypted;
                // 4. Send the POST request
                HttpResponseMessage response = await client.PostAsJsonAsync(url, payload);
                string responseBody = await response.Content.ReadAsStringAsync();
                string file = AppContext.BaseDirectory + @"\LogFile.txt";
                System.IO.File.AppendAllText(file, url + Environment.NewLine + responseBody + Environment.NewLine);
                return Content(responseBody, "application/json");
            }
            catch (Exception ex)
            {
                var error = new { msgCode = "Error", message = ex.Message };
                return new JsonResult(error);
            }
        }

        [HttpPost]
        [Route("MeetingPoints")]
        public async Task<IActionResult> MeetingPoints(MeetingPointsRequest obj)
        {
            try
            {
                var ctripposturl = "https://testapi-car.ctrip.com/chvendormessagebus/sandbox";//ctrip test url
                var channel = "JNT";//this endpoint's URL is hardcoded to the JNT channel below
                var vendorid = "1005327";//i have got this from dispatch system
                var ctrippurchaseorderid = obj.ctripPurchaseOrderID;
                var timeStamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
                string version = "2.0";
                string key = "12345678";
                var options = new JsonSerializerOptions
                {
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, // Prevents escaping +
                    WriteIndented = true
                };
                string json = JsonSerializer.Serialize(obj, options);
                string encrypted = DesEcbHexDecryptor.Encrypt(json, key);
                string sign = MD5Signature.ComputeMd5Hash(vendorid + version + channel + timeStamp + key + encrypted.Length.ToString()).ToLower();
                var url = string.Format("{0}/JNT-{1}/order/meetingpoints/2.0/{2}/{3}/{4}", ctripposturl, vendorid, ctrippurchaseorderid, timeStamp, sign);
                // 2. Initialize HttpClient (preferably reused in real applications)
                using HttpClient client = new HttpClient();
                // 3. Create the payload object
                var payload = encrypted;
                // 4. Send the POST request
                HttpResponseMessage response = await client.PostAsJsonAsync(url, payload);
                string responseBody = await response.Content.ReadAsStringAsync();
                string file = AppContext.BaseDirectory + @"\LogFile.txt";
                System.IO.File.AppendAllText(file, url + Environment.NewLine + responseBody + Environment.NewLine);
                return Content(responseBody, "application/json");
            }
            catch (Exception ex)
            {
                var error = new { msgCode = "Error", message = ex.Message };
                return new JsonResult(error);
            }
        }

        [HttpPost]
        [Route("UploadAttachment")]
        public async Task<IActionResult> UploadAttachment(UploadAttachmentRequest obj)
        {
            try
            {
                var protocol = "http";//http or https http is mentioned in document in example
                var domain = "file.c-ctrip.com"; //file.c-ctrip.com menioned in dcoument in example
                var channel = obj.channel;
                var @public = obj.@public;
                var oversea = obj.oversea;
                var filename = obj.filename;
                byte[] textBytes = Encoding.UTF8.GetBytes(filename);
                string base64Encoded = Convert.ToBase64String(textBytes);
                var url = "{0}://{1}/file/v1/api/upload?channel={2}&public={3}&oversea={4}&filename={5}";
                url = string.Format(url, protocol, domain, channel, @public, oversea, base64Encoded);
                //Initialize HttpClient (preferably reused in real applications)
                using HttpClient client = new HttpClient();
                string file = AppContext.BaseDirectory + @"\taxiicon.jfif";
                byte[] fileBytes = System.IO.File.ReadAllBytes(file);//getting fileBytes
                client.DefaultRequestHeaders.Add("Crc", CrcUtility.GetCrc(fileBytes));
                client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Length", fileBytes.Length.ToString());
                client.DefaultRequestHeaders.TryAddWithoutValidation("Content-type", "image/jpeg");//can be set supported file type as mentioned in document
                //Create the payload object
                var payload = "{}";
                //Send the POST request
                HttpResponseMessage response = await client.PostAsJsonAsync(url, payload);
                string responseBody = await response.Content.ReadAsStringAsync();
                string logfile = AppContext.BaseDirectory + @"\LogFile.txt";
                System.IO.File.AppendAllText(logfile, url + Environment.NewLine + responseBody + Environment.NewLine);
                return Content(responseBody, "application/json");
            }
            catch (Exception ex)
            {
                var error = new { msgCode = "Error", message = ex.Message };
                return new JsonResult(error);
            }
        }
    }
}
