﻿using System.Net.Http.Headers;
using System.Text;

namespace Parcel.Services
{
    public static class ChatGPT
    {
        #region Enums
        public enum ChatGPTModel
        {
            GPT4Turbo
        }
        #endregion

        #region Defaults
        private const string OpenAIChatCompletionEndpoint = "https://api.openai.com/v1/chat/completions";
        private const int DefaultTokenSizeLimit = 4000;

        private const string GPT4TurboModel = "gpt-4-turbo-preview";
        #endregion

        #region Global Configurations
        public static void ConfigureChatGPT(string aipToken, int defaultTokenSizeLimit, ChatGPTModel model)
        {
            APIToken = aipToken; ;
            TokenSizeLimit = defaultTokenSizeLimit;
            Model = model switch
            {
                ChatGPTModel.GPT4Turbo => GPT4TurboModel,
                _ => throw new ArgumentOutOfRangeException($"Invalid model: {model}")
            };
        }
        public static string APIToken { get; set; }
        public static int TokenSizeLimit { get; set; } = DefaultTokenSizeLimit;
        public static string Model { get; set; } = GPT4TurboModel;
        #endregion

        #region Methods
        public static string Complete(string system, string query, string model, string apiToken, int sizeLimit)
        {
            using HttpClient httpClient = new()
            {
                BaseAddress = new Uri(OpenAIChatCompletionEndpoint)
            };
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiToken);

            using StringContent jsonContent = new(
                $$"""
                {
                    "messages": [
                        {
                            "role": "system",
                            "content": "{{system}}"
                        },
                        {
                            "role": "user",
                            "content": "{{query}}"
                        }
                    ],
                    "model": "{{model}}",
                    "max_tokens": {{sizeLimit}}
                }
                """, Encoding.UTF8, "application/json");

            using HttpResponseMessage response = httpClient.PostAsync(string.Empty, jsonContent).Result;
            HttpResponseMessage message = response.EnsureSuccessStatusCode();

            string jsonResponse = response.Content.ReadAsStringAsync().Result;
            return jsonResponse;
        }
        public static string Complete(string query, string token)
            => Complete(string.Empty, query, GPT4TurboModel, token, DefaultTokenSizeLimit);
        public static string AskChatGPTAboutData(string question, string dataCSV, string apiToken)
        => throw new NotImplementedException();
        public static string AskChatGPTAboutImage(string question, string imageReference, string apiToken)
            => throw new NotImplementedException();
        #endregion
    }
}
