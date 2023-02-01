using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Domain.Common
{
    public partial class Utils
    {
        public static string GetConfig(string code)
        {
            IConfigurationRoot configuration = ConfigCollection.Instance.GetConfiguration();
            var value = configuration[code];
            return value;
        }

        public static string GetConfig(string code, string defaultValue)
        {
            IConfigurationRoot configuration = ConfigCollection.Instance.GetConfiguration();
            var value = configuration[code];
            if (string.IsNullOrEmpty(value))
                return defaultValue;
            return value;
        }

        public static string GetConfig(IConfiguration configuration, string code)
        {
            var value = configuration[code];
            return value;
        }
        public static string CreateExceptionMessage(Exception ex)
        {
            string s = "Error: " + ex.Message;
            s = s + "\r\n" + "Stacktrace: " +
                ex.StackTrace;
            if (ex.InnerException != null)
            {
                s = s + "\r\n" + "InnerMessage: " +
                ex.InnerException.Message;
                s = s + "\r\n" + "InnerStacktrace: " +
                ex.InnerException.StackTrace;
            }
            return s;
        }
        public static ResponseError CreateExceptionResponseError(Exception ex)
        {
            Log.Error(ex, Utils.CreateExceptionFormat(), Utils.GetCurrentMethod());
            string s = "Error: " + ex.Message;
            s = s + "\r\n" + "Stacktrace: " +
                ex.StackTrace;
            if (ex.InnerException != null)
            {
                s = s + "\r\n" + "InnerMessage: " +
                ex.InnerException.Message;
                s = s + "\r\n" + "InnerStacktrace: " +
                ex.InnerException.StackTrace;
            }
            return new ResponseError(HttpStatusCode.InternalServerError, s);
        }
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string GetCurrentMethod()
        {
            var st = new StackTrace();
            var sf = st.GetFrame(1);
            return sf.GetMethod()?.Name;
        }
        public static string CreateExceptionFormat()
        {
            return "Method: {0}";
        }
        public static bool IsFileExist(string url)
        {
            var _host = Utils.GetConfig("StaticFiles:Host");
            return System.IO.File.Exists(Path.Combine(_host, url));
        }

        public static bool IsGuid(string inputString)
        {
            try
            {
                var guid = new Guid(inputString);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static string RandomString(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHJKMNPQRSTUVWXYZ123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public static string RandomInt(int length)
        {
            Random random = new Random();
            const string chars = "0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
    public class UpdateLog
    {
        public DateTime Date { get; set; }
        public string Message { get; set; }
        public string Data { get; set; }
    }
    public static class UpdateLogHelper
    {
        public static string AddUpdateLog(string original, string message, string data)
        {
            if (string.IsNullOrEmpty(original))
            {
                var listLog = new List<UpdateLog>();
                listLog.Add(new UpdateLog { Date = DateTime.Now, Message = message, Data = data });
                return JsonConvert.SerializeObject(listLog);
            }
            else
            {
                var listLog = new List<UpdateLog>();

                try
                {
                    listLog = JsonConvert.DeserializeObject<List<UpdateLog>>(original);
                }
                catch (Exception)
                { }
                // Limit number of logs
                if (listLog.Count == 10)
                    listLog.RemoveAt(9);

                // Add new log to the start
                listLog.Insert(0, new UpdateLog { Date = DateTime.Now, Message = message, Data = data });

                return JsonConvert.SerializeObject(listLog);
            }
        }
    }

}
