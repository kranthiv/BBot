using BBot.Helpers;
using BBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;

namespace BBot.QNA
{
    public class QNAService
    {
        private const string knowledgebaseId = @"fb94d233-4239-41e2-9b78-fc98bc3f5e72";
        private const string qnamakerSubscriptionKey = "4398fe15638b42ee8af4aebbc8abddbb";
        public async Task<QNAResponse> GetAnswerAsync(string question)
        {
            string responseString = string.Empty;


            //Build the URI
            Uri qnamakerUriBase = new Uri("https://westus.api.cognitive.microsoft.com/qnamaker/v1.0");
            var builder = new UriBuilder($"{qnamakerUriBase}/knowledgebases/{knowledgebaseId}/generateAnswer");

            //Add the question as part of the body
            var postBody = $"{{\"question\": \"{question}\"}}";

            //Send the POST request
            using (WebClient client = new WebClient())
            {
                //Add the subscription key header
                client.Headers.Add("Ocp-Apim-Subscription-Key", qnamakerSubscriptionKey);
                client.Headers.Add("Content-Type", "application/json");
                responseString = await client.UploadStringTaskAsync(builder.Uri, postBody);
            }

            QNAResponse response = await JSONHelper.DeSerializeJSON<QNAResponse>(responseString);
            return response;
        }
    }
}