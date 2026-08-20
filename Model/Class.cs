using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;

namespace CtripAPI.Model
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class AuseLocation
    {
        public string address { get; set; }
        public string detailAddress { get; set; }
        public double longitude { get; set; }
        public double latitude { get; set; }
        public string geoType { get; set; }
        public string cityId { get; set; }
        public string cityName { get; set; }
    }

    public class DuseLocation
    {
        public string address { get; set; }
        public string detailAddress { get; set; }
        public double longitude { get; set; }
        public double latitude { get; set; }
        public string geoType { get; set; }
        public string cityId { get; set; }
        public string cityName { get; set; }
    }

    public class FixedLocation
    {
        public string fixedCode { get; set; }
        public string fixedLocationName { get; set; }
        public string cityId { get; set; }
        public string cityName { get; set; }
    }

    public class QueryProductRequestRoot
    {
        public string categoryCode { get; set; }
        public DuseLocation duseLocation { get; set; }
        public AuseLocation auseLocation { get; set; }
        public string useTime { get; set; }
        public FixedLocation fixedLocation { get; set; }
        public int fromType { get; set; }
    }

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class AddService
    {
        public int maxCount { get; set; }
        public string vendorAddServiceCode { get; set; }
    }

    public class QueryResultList
    {
        public decimal price { get; set; }
        public int vehicleType { get; set; }
        public List<AddService> addServices { get; set; }
    }

    public class QueryProductResponseRoot
    {
        public string msgCode { get; set; }
        public string message { get; set; }
        public string priceMark { get; set; }
        public string currency { get; set; }
        public List<QueryResultList> queryResultList { get; set; }
    }


    public class MD5Signature
    {
        public static string ComputeMd5Hash(string rawData)
        {
            // Convert the input string to a byte array
            byte[] inputBytes = Encoding.UTF8.GetBytes(rawData);

            // Compute the hash bytes
            byte[] hashBytes = MD5.HashData(inputBytes);

            // Convert the byte array to a readable hexadecimal string
            return Convert.ToHexString(hashBytes);
        }

    }

    public class DesEcbHexDecryptor
    {
        /// <summary>
        /// Decrypts a HEX-encoded ciphertext using DES in ECB mode.
        /// </summary>
        /// <param name="hexCipherText">Cipher text in uppercase HEX format (e.g., "5F2A...").</param>
        /// <param name="key">8-byte DES key as a string (will be UTF8 encoded).</param>
        /// <param name="padding">Padding mode (e.g., PaddingMode.PKCS7).</param>
        /// <returns>Decrypted plaintext string.</returns>
        public static string DecryptFromHex(string hexCipherText, string key, PaddingMode padding)
        {
            if (string.IsNullOrWhiteSpace(hexCipherText))
                throw new ArgumentException("Cipher text cannot be null or empty.", nameof(hexCipherText));

            if (string.IsNullOrEmpty(key) || Encoding.UTF8.GetByteCount(key) != 8)
                throw new ArgumentException("Key must be exactly 8 bytes when UTF8 encoded.", nameof(key));

            byte[] cipherBytes = HexStringToBytes(hexCipherText);

            using (DES des = DES.Create())
            {
                des.Mode = CipherMode.ECB;
                des.Padding = padding;
                des.Key = Encoding.UTF8.GetBytes(key);

                using (ICryptoTransform decryptor = des.CreateDecryptor())
                {
                    byte[] plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
                    return Encoding.UTF8.GetString(plainBytes);
                }
            }
        }

        /// <summary>
        /// Converts an uppercase HEX string to a byte array.
        /// </summary>
        private static byte[] HexStringToBytes(string hex)
        {
            if (hex.Length % 2 != 0)
                throw new ArgumentException("Hex string must have an even length.", nameof(hex));

            byte[] bytes = new byte[hex.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                string byteValue = hex.Substring(i * 2, 2);
                bytes[i] = Convert.ToByte(byteValue, 16);
            }
            return bytes;
        }

        public static string Encrypt(string plainText, string secretKey)
        {
            using DES des = DES.Create();
            des.Mode = CipherMode.ECB;
            des.Padding = PaddingMode.PKCS7;
            des.Key = Encoding.UTF8.GetBytes(secretKey);

            using ICryptoTransform encryptor = des.CreateEncryptor();
            byte[] inputBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] cipherBytes = encryptor.TransformFinalBlock(inputBytes, 0, inputBytes.Length);

            // Convert byte array to uppercase hex string
            return BitConverter.ToString(cipherBytes).Replace("-", "").ToUpperInvariant();
        }


    }

    public class Agent
    {
        public string name { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string intlPhoneAreaCode { get; set; }
        public string intlPhone { get; set; }
    }


    public class FlightInfo
    {
        public int departDelayTime { get; set; }
        public string takeOffTime { get; set; }
        public string flightLandingTime { get; set; }
    }

    public class Passenger
    {
        public string name { get; set; }
        public string localPhoneAreaCode { get; set; }
        public string localPhone { get; set; }
        public string maskRealPhone { get; set; }
    }

    public class CreateOrderRequest
    {
        public long ctripPurchaseOrderId { get; set; }
        public string categoryCode { get; set; }
        public double totalPrice { get; set; }
        public string priceMark { get; set; }
        public FixedLocation fixedLocation { get; set; }
        public int vehicleType { get; set; }
        public string useTime { get; set; }
        public DuseLocation duseLocation { get; set; }
        public AuseLocation auseLocation { get; set; }
        public FlightInfo flightInfo { get; set; }
        public Passenger passenger { get; set; }
        public Agent agent { get; set; }
        public long masterOrderId { get; set; }
        public bool needLandingVisa { get; set; }
        public int adults { get; set; }
        public int children { get; set; }
        public int luggage { get; set; }
    }

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class CreateOrderResponse
    {
        public string msgCode { get; set; }
        public string message { get; set; }
        public string vendorOrderId { get; set; }
    }

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);

    public class UpdateOrderRequest
    {
        public long ctripPurchaseOrderId { get; set; }
        public Passenger passenger { get; set; }
        public Agent agent { get; set; }
        public FlightInfo flightInfo { get; set; }
    }

    public class UpdateOrderResponse
    {
        public string msgCode { get; set; }
        public string message { get; set; }
    }

    public class OrderInfo
    {
        public long ctripPurchaseOrderId { get; set; }
        public string vendorOrderId { get; set; }
        public string startTime { get; set; }
    }
    public class QueryDriverLocationRequest
    {
        public List<OrderInfo> orderInfo { get; set; }
    }
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class DriverLocationInfo
    {
        public double longitude { get; set; }
        public double latitude { get; set; }
        public string coordinate { get; set; }
        public string locationTime { get; set; }
        public double speed { get; set; }
        public int direction { get; set; }
        public int locationType { get; set; }
    }

    public class OrderDriverLocationInfo
    {
        public object ctripPurchaseOrderId { get; set; }
        public string msgCode { get; set; }
        public string message { get; set; }
        public List<DriverLocationInfo> driverLocationInfo { get; set; }
    }

    public class QueryDriverLocationResponse
    {
        public string msgCode { get; set; }
        public string message { get; set; }
        public List<OrderDriverLocationInfo> orderDriverLocationInfo { get; set; }
    }

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class CancelOrderRequest
    {
        public long ctripPurchaseOrderId { get; set; }
        public string vendorOrderId { get; set; }
        public string cancelReason { get; set; }
    }
    public class CancelOrderResponse
    {
        public string msgCode { get; set; }
        public string message { get; set; }
    }

    public class OrderDetailRequest
    {
        public long ctripPurchaseOrderId { get; set; }
        public string vendorOrderId { get; set; }
    }
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class OrderDetailAgent
    {
        public string name { get; set; }
        public string intlPhoneAreaCode { get; set; }
        public string intlPhone { get; set; }
    }

    public class OrderDetailAuseLocation
    {
        public string address { get; set; }
        public string detailAddress { get; set; }
        public double longitude { get; set; }
        public double latitude { get; set; }
        public string geoType { get; set; }
        public int cityId { get; set; }
        public string cityName { get; set; }
    }

    public class DriverInfo
    {
        public string driverId { get; set; }
        public string driverName { get; set; }
        public string driverPhone { get; set; }
        public string driverAvaterURL { get; set; }
        public string driverVirtualPhone { get; set; }
        public string driverAgentVirtualPhone { get; set; }
        public int vehicleType { get; set; }
        public string vehicleBrand { get; set; }
        public string vehicleColor { get; set; }
        public string vehicleNumber { get; set; }
        public double level { get; set; }
        public string driverPhoneLangauge { get; set; }
    }

    public class OrderDetailDuseLocation
    {
        public string address { get; set; }
        public string detailAddress { get; set; }
        public double longitude { get; set; }
        public double latitude { get; set; }
        public string geoType { get; set; }
        public int cityId { get; set; }
        public string cityName { get; set; }
    }

    public class FeeItem
    {
        public string code { get; set; }
        public string name { get; set; }
        public int amount { get; set; }
        public bool discount { get; set; }
    }

    public class OrderDetailPassenger
    {
        public string name { get; set; }
        public string localPhoneAreaCode { get; set; }
        public string localPhone { get; set; }
        public string maskRealPhone { get; set; }
        public string email { get; set; }
        public string intlPhoneAreaCode { get; set; }
        public string intlPhone { get; set; }
    }

    public class OrderDetailResponse
    {
        public string msgCode { get; set; }
        public string message { get; set; }
        public long ctripPurchaseOrderId { get; set; }
        public string vendorOrderId { get; set; }
        public string categoryCode { get; set; }
        public int totalFee { get; set; }
        public int actualDistance { get; set; }
        public int actualTime { get; set; }
        public int priceMode { get; set; }
        public int vehicleType { get; set; }
        public string useTime { get; set; }
        public OrderDetailDuseLocation duseLocation { get; set; }
        public OrderDetailAuseLocation auseLocation { get; set; }
        public OrderDetailPassenger passenger { get; set; }
        public OrderDetailAgent agent { get; set; }
        public int orderStatus { get; set; }
        public List<FeeItem> feeItems { get; set; }
        public string orderTime { get; set; }
        public string takenTime { get; set; }
        public string readyTime { get; set; }
        public string serviceStartTime { get; set; }
        public string serviceEndTime { get; set; }
        public DriverInfo driverInfo { get; set; }
    }

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class EventDetail
    {
        public int eventTypeId { get; set; }
        public string eventTypeName { get; set; }
        public string @operator { get; set; }
    }

    public class CreateReplyEventRequest
    {
        public long ctripPurchaseOrderId { get; set; }
        public long masterOrderId { get; set; }
        public string vendorOrderId { get; set; }
        public string parentCategoryCode { get; set; }
        public string serviceProviderId { get; set; }
        public int ctripEventId { get; set; }
        public int eventSource { get; set; }
        public string content { get; set; }
        public string deadLine { get; set; }
        public EventDetail eventDetail { get; set; }
        public string ctripParentEventId { get; set; }
    }
    public class CreateReplyEventResponse
    {
        public string msgCode { get; set; }
        public string message { get; set; }
        public string vendorEventId { get; set; }
    }
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class ConfirmOrder
    {
        public long ctripPurchaseOrderID { get; set; }
        public string vendorOrderID { get; set; }
        public string pickUpLocationDesc { get; set; }
        public string pickUpLocationImgUrl { get; set; }
        public string connectTel { get; set; }
        public string connectTelCode { get; set; }
        public string Channel { get; set; }

        
    }

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class DriverInfoPush
    {
        public long ctripPurchaseOrderID { get; set; }
        public string vendorOrderID { get; set; }
        public string driverName { get; set; }
        public string driverPhone { get; set; }
        public string driverPhoneCode { get; set; }
        public string driverPhoneLangauge { get; set; }
        public string vehicleNumber { get; set; }
        public string vehicleBrand { get; set; }
        public string vehicleBrandDetail { get; set; }
        public string vehicleColor { get; set; }
        public string Channel { get; set; }
    }

    public class DriverStatusPush
    {
        public long ctripPurchaseOrderID { get; set; }
        public string vendorOrderID { get; set; }
        public int vehicleStatus { get; set; }
        public string Channel { get; set; }
    }

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class OrderCompleteRequest
    {
        public long ctripPurchaseOrderID { get; set; }
        public string vendorOrderID { get; set; }
        public string Channel { get; set; }
    }

    public class OrderDetailsRequest
    {
        public long ctripPurchaseOrderID { get; set; }
        public string vendorOrderID { get; set; }
        public string Channel { get; set; }
    }

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class CannotServiceRequest
    {
        public long ctripPurchaseOrderID { get; set; }
        public string vendorOrderID { get; set; }
        public string Channel { get; set; }
    }

    public class PartnerCreateReplyRequest
    {
        public string content { get; set; }
        public string ctripEventId { get; set; }
        public string ctripPurchaseOrderId { get; set; }
        public int eventSource { get; set; }
        public string eventTypeId { get; set; }
        public string @operator { get; set; }
        public int pushType { get; set; }
        public int serviceProviderId { get; set; }
        public string vendorEventId { get; set; }
        public string vendorOrderId { get; set; }
    }

    public class PickupImage
    {
        public string imageUrl { get; set; }
        public string description { get; set; }
    }

    public class MeetingPointsRequest
    {
        public long ctripPurchaseOrderID { get; set; }
        public string vendorOrderID { get; set; }
        public string name { get; set; }
        public string location { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public List<PickupImage> pickupImages { get; set; }
    }

    public class CrcUtility
    {
        public static string GetCrc(byte[] fileBytes)
        {
            byte[] tmp;
            int size5M = 5 * 1024 * 1024;
            int size10M = 10 * 1024 * 1024;

            if (fileBytes.Length > size10M)
            {
                tmp = new byte[size10M];
                Array.Copy(fileBytes, 0, tmp, 0, size5M);
                Array.Copy(fileBytes, fileBytes.Length - size5M, tmp, size5M, size5M);
            }
            else
            {
                tmp = fileBytes;
            }

            using (MD5 md5 = MD5.Create())
            {
                byte[] digest = md5.ComputeHash(tmp);
                return Convert.ToHexString(digest).ToLowerInvariant();
            }
        }
    }
    public class UploadAttachmentRequest
    {
        public string channel { get; set; }
        public string @public { get; set; }

        public string oversea { get; set; }

        public string filename { get; set; }
    }
}