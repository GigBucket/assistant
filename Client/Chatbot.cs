/**
 * Copyright (C) 2019-2022 Chatopera Inc, <https://www.chatopera.com>
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Security.Cryptography;
using Newtonsoft.Json;

namespace Client
{
    class Chatbot
    {
        private string appId;
        private string secret;
        private HttpClient client = new HttpClient();

        public Chatbot(string appId, string secret)
        {
            this.appId = appId;
            this.secret = secret;
            this.client.BaseAddress = new Uri("https://bot.chatopera.com");
        }

        private string hmacSha1(string key, string input)
        {
            byte[] keyBytes = ASCIIEncoding.ASCII.GetBytes(key);
            byte[] inputBytes = ASCIIEncoding.ASCII.GetBytes(input);
            HMACSHA1 hmac = new HMACSHA1(keyBytes);
            byte[] hashBytes = hmac.ComputeHash(inputBytes);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }

        private string generate(string method, string path)
        {
            Int32 timestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            string random = Guid.NewGuid().ToString();
            string signature = hmacSha1(secret, appId + timestamp + random + method + path);
            var obj = new { appId, timestamp, random, signature };
            string json = JsonConvert.SerializeObject(obj);
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
        }

        public List<ChatbotResult.FaqResult> Faq(string userId, string query)
        {
            string path = "/api/v1/chatbot/" + appId + "/faq/query";
            string token = generate("POST", path);
            StringContent content = new StringContent(content: JsonConvert.SerializeObject(new { fromUserId = userId, query }), encoding: Encoding.UTF8, mediaType: "application/json");
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, path) { Content = content };
            requestMessage.Headers.Add("Authorization", token);
            string json = client.SendAsync(requestMessage).Result.Content.ReadAsStringAsync().Result;
            ChatbotResult.Response<List<ChatbotResult.FaqResult>> obj = JsonConvert.DeserializeObject<ChatbotResult.Response<List<ChatbotResult.FaqResult>>>(json);

            if (obj.Rc == 0)
            {
                return obj.Data;
            }
            else
            {
                throw new Exception(obj.Error);
            }
        }
    }
}
