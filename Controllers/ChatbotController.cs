using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CosmeticsStore.Controllers
{
    public class ChatbotController : Controller
    {
        // Thay YOUR_OPENAI_API_KEY bằng API key thực của bạn
        private static readonly string OpenAiApiKey = "sk-svcacct-GJAwcfYav15aH3fQs_4F2KNsm_aOw65pv25VPP-Hau8d89WGzBdAzlJEdyHT3BlbkFJwN7xFtnxtejP9h6F7Kp8M6fGdTGSEGKEcGFdTjUMfxi54dzRMRO9H850qzQA";
        private static readonly string OpenAiApiUrl = "https://api.openai.com/v1/chat/completions";

        // GET: Chatbot
        public ActionResult Index()
        {
            return View();
        }

        // POST: Chatbot/SendMessage
        [HttpPost]
        public async Task<ActionResult> SendMessage(string userMessage)
        {
            if (string.IsNullOrEmpty(userMessage))
            {
                return Json(new { error = "Message cannot be empty" });
            }

            // Gửi yêu cầu tới OpenAI API để nhận phản hồi
            string botResponse = await GetBotResponse(userMessage);

            return Json(new { userMessage, botResponse });
        }

        // Method to interact with OpenAI API to get the bot's response
        private async Task<string> GetBotResponse(string userMessage)
        {
            using (HttpClient client = new HttpClient())
            {
                // Thiết lập các headers cho yêu cầu API
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {OpenAiApiKey}");

                // Tạo payload để gửi lên API
                var requestData = new
                {
                    model = "gpt-3.5-turbo", // Bạn có thể thay bằng GPT-4 nếu có quyền sử dụng
                    messages = new List<object>
                    {
                        new { role = "system", content = "You are a helpful assistant." },
                        new { role = "user", content = userMessage }
                    },
                    max_tokens = 150
                };

                string jsonRequest = JsonConvert.SerializeObject(requestData);
                StringContent content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                // Gửi yêu cầu POST tới API
                HttpResponseMessage response = await client.PostAsync(OpenAiApiUrl, content);
                string responseString = await response.Content.ReadAsStringAsync();

                // Xử lý phản hồi JSON từ API
                if (response.IsSuccessStatusCode)
                {
                    dynamic responseJson = JsonConvert.DeserializeObject(responseString);
                    return responseJson.choices[0].message.content;
                }
                else
                {
                    return "Sorry, something went wrong. Please try again.";
                }
            }
        }
    }
}
