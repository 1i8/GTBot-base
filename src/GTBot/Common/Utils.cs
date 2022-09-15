using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTBot.Common
{
    public static class Utils
    {
        public static string CreateRID()
        {
            var rand = new Random();
            string str = "0";
            const string chars = "ABCDEF0123456789";
            str += new string(Enumerable.Repeat(chars, 31)
               .Select(s => s[rand.Next(s.Length)]).ToArray());
            return str;
        }

        public static string CreateWinKey()
        {
            var rand = new Random();
            string str = "7";
            const string chars = "ABCDEF0123456789";
            str += new string(Enumerable.Repeat(chars, 31)
               .Select(s => s[rand.Next(s.Length)]).ToArray());
            return str;
        }

        public static string CreateMAC()
        {
            var random = new Random();
            var buffer = new byte[6];
            random.NextBytes(buffer);
            var result = String.Concat(buffer.Select(x => string.Format("{0}:", x.ToString("X2"))).ToArray());
            return result.TrimEnd(':');
        }

        public static async Task<string> request_server_data()
        {
            using (var client = new HttpClient())
            {
                var values = new Dictionary<string, string>
                {
                    { "version", "3.99" },
                    { "platform", "0" },
                    { "protocol", "164" }
                };
                var content = new FormUrlEncodedContent(values);
                using (var request = new HttpRequestMessage(HttpMethod.Post,
                           "https://www.growtopia2.com/growtopia/server_data.php"))
                {
                    request.Headers.Host = "www.growtopia2.com";
                    request.Headers.UserAgent.ParseAdd("UbiServices_SDK_2019.Release.27_PC64_unicode_static");
                    var response = await client.SendAsync(request);
                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadAsStringAsync();
                }
            }
        }
    }
}
