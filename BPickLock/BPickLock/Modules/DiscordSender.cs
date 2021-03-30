using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Text;

namespace BPickLock.Modules
{
    public class DiscordSender
    {
        public static void PostMessage(string WebhookURL, WebhookMessage message)
        {
            HttpWebRequest request = WebRequest.CreateHttp(WebhookURL);
            request.Method = "POST";
            request.ContentType = "application/json";

            string Payload = JsonConvert.SerializeObject(message);
            byte[] Buffer = Encoding.UTF8.GetBytes(Payload);

            request.ContentLength = Buffer.Length;

            using (Stream write = request.GetRequestStream())
            {
                write.Write(Buffer, 0, Buffer.Length);
                write.Flush();
            }

            var resp = (HttpWebResponse)request.GetResponse();
        }
    }
}
