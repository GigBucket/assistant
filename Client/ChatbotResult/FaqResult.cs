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

namespace Client.ChatbotResult
{
    class FaqResult
    {
        private string id;
        private float score;
        private string post;
        private string reply;
        private List<Reply> replies;

        public string Id { get => id; set => id = value; }
        public float Score { get => score; set => score = value; }
        public string Post { get => post; set => post = value; }
        public List<Reply> Replies { get => replies; set => replies = value; }
        public string Reply { get => reply; set => reply = value; }
    }
}
