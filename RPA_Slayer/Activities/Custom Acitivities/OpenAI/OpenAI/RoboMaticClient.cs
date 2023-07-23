using System;
using System.Activities;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace OpenAI
{
    public class RoboMaticClient : CodeActivity
    {
        [Category("Input")]
        [RequiredArgument]
        public InArgument<string> InputMsg { get; set; }

        [Category("Output")]
        public OutArgument<string> OutputMsg { get; set; }

        protected override async void Execute(CodeActivityContext context)
        {
            try
            {
                string input = InputMsg.Get(context);

                using (var client = new HttpClient())
                {
                    var request = new HttpRequestMessage
                    {
                        Method = HttpMethod.Post,
                        RequestUri = new Uri("https://robomatic-ai.p.rapidapi.com/api"),
                        Headers =
                        {
                            { "X-RapidAPI-Key", "45da2837a0msh926e0d31bda49a7p1ae50ejsn21d5b6c49434" },
                            { "X-RapidAPI-Host", "robomatic-ai.p.rapidapi.com" },
                        },
                        Content = new FormUrlEncodedContent(new Dictionary<string, string>
                        {
                            { "in", input },
                            { "op", "in" },
                            { "cbot", "1" },
                            { "SessionID", "RapidAPI1" },
                            { "cbid", "1" },
                            { "key", "RHMN5hnQ4wTYZBGCF3dfxzypt68rVP" },
                            { "ChatSource", "RapidAPI" },
                            { "duration", "1" },
                        }),
                    };

                    using (var response = await client.SendAsync(request))
                    {
                        response.EnsureSuccessStatusCode();
                        OutputMsg.Set(context, response);
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that may occur during the execution.
                Console.WriteLine(".......................................................................");
                Console.WriteLine(ex.Message);
                
            }
        }
    }
}
