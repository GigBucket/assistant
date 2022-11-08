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
using System.Threading.Tasks;

namespace Client.ChatbotResult
{
    class Reply
    {
        private string rtype;
        private string content;
        private string thumbnail;
        private string title;
        private string url;
        private string disply;

        public string Rtype { get => rtype; set => rtype = value; }
        public string Content { get => content; set => content = value; }
        public string Thumbnail { get => thumbnail; set => thumbnail = value; }
        public string Title { get => title; set => title = value; }
        public string Url { get => url; set => url = value; }
        public string Disply { get => disply; set => disply = value; }
    }
}
