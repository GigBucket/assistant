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
using System.ComponentModel;
using Client.ChatbotResult;

namespace Client
{
    class MainWindowView : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string name)
        {
            var onProperty = PropertyChanged;
            if (onProperty != null)
                onProperty(this, new PropertyChangedEventArgs(name));
        }

        private string _email;
        private string _appId;
        private string _secret;
        private bool _enabled = true;
        private string _loading;
        private string _query;
        private List<ChatbotResult.FaqResult> _faq;

        public string Email
        {
            get => _email; set
            {
                _email = value;
                OnPropertyChanged("Email");
            }
        }
        public string AppId
        {
            get => _appId; set
            {
                _appId = value;
                OnPropertyChanged("AppId");
            }
        }
        public string Secret
        {
            get => _secret; set
            {
                _secret = value;
                OnPropertyChanged("Secret");
            }
        }

        public bool Enabled
        {
            get => _enabled; set
            {
                _enabled = value;
                OnPropertyChanged("Enabled");
            }
        }

        public string Loading
        {
            get => _loading; set
            {
                _loading = value;
                OnPropertyChanged("Loading");
            }
        }

        public List<FaqResult> Faq
        {
            get => _faq; set
            {
                _faq = value;
                OnPropertyChanged("Faq");
            }
        }

        public string Query
        {
            get => _query; set
            {
                _query = value;
                OnPropertyChanged("Query");
            }
        }
    }
}
