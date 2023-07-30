using System;
using System.Activities;
using System.Net.Http;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Xml.Linq;
using System.Linq;

namespace OpenAI
{
    public class ChatGPT_3 : CodeActivity
    {
        [Category("Input")]
        [RequiredArgument]
        public InArgument<string> InputText { get; set; }

        [Category("Output")]
        public OutArgument<string> OutputText { get; set; }

        // Replace with your GPT-3 API key
        private const string ApiKey = "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";

        // GPT-3 API endpoint
        private const string ApiEndpoint = "https://api.openai.com/v1/chat/completions";

        protected override void Execute(CodeActivityContext context)
        {
            // Get the input text from the activity property
            string input = InputText.Get(context);

            // Create the JSON request object
            string jsonRequest = @"
{
    ""model"": ""gpt-3.5-turbo"",
    ""messages"": [{""role"": ""user"", ""content"": """ + input + @"""}],
    ""temperature"": 0.7
}";

            // Call the GPT-3 API to get the completion
            string generatedResponse = GetGpt3Completion(jsonRequest).Result;

            // Extract "content" from the response
            string content = ExtractContentFromResponse(generatedResponse);

            // Set the generated text as the output
            OutputText.Set(context, content);
        }

        private async Task<string> GetGpt3Completion(string jsonRequest)
        {
            using (var httpClient = new System.Net.Http.HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", ApiKey);
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync(ApiEndpoint, content);
                return await response.Content.ReadAsStringAsync();
            }
        }

        private string ExtractContentFromResponse(string jsonResponse)
        {
            try
            {
                // Parse the JSON response and extract the "content" from "message"
                var responseObj = JObject.Parse(jsonResponse);
                var content = responseObj["choices"]?.FirstOrDefault()?["message"]?["content"]?.ToString();

                return content;
            }
            catch (Exception ex)
            {
                // In case of any errors, return an empty string
                return string.Empty;
            }
        }
    }
}
