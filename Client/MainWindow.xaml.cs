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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Net;
using System.Threading;
using System.Net.Http;
using Newtonsoft.Json;

namespace Client
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private ClipboardInterop _clipboardInterop;
        private MainWindowView view = new MainWindowView() { Loading = "Chatopera 小助手" };
        private Chatbot chatbot;
        private string lastCopy;
        private bool lastEnabled = true;

        public MainWindow()
        {
            InitializeComponent();

            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            view.Email = Properties.Settings.Default.email;
            view.AppId = Properties.Settings.Default.appId;
            view.Secret = Properties.Settings.Default.secret;
            logLabel.Visibility = Visibility.Hidden;

            appPwd.Password = view.Secret;

            StartBot();

            this.DataContext = view;
        }

        public void StartBot()
        {
            if (!string.IsNullOrEmpty(view.Email) && !string.IsNullOrEmpty(view.AppId) && !string.IsNullOrEmpty(view.Secret))
            {
                chatbot = new Chatbot(view.AppId, view.Secret);
                bMain.Visibility = Visibility.Hidden;
                view.Enabled = lastEnabled;

                btnQuery.Visibility = Visibility.Visible;
                qBox.Visibility = Visibility.Visible;
                logLabel.Visibility = Visibility.Hidden;
                view.Loading = "Chatopera 小助手";

            }
            else
            {
                bMain.Visibility = Visibility.Visible;

                btnQuery.Visibility = Visibility.Hidden;
                qBox.Visibility = Visibility.Hidden;
                logLabel.Visibility = Visibility.Visible;
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.bMain.Visibility = Visibility.Visible;
        }

        private void alertMsg(string msg)
        {
            Task.Factory.StartNew(() =>
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    btnQuery.Visibility = Visibility.Hidden;
                    qBox.Visibility = Visibility.Hidden;
                    logLabel.Visibility = Visibility.Visible;
                }));

                view.Loading = msg;

                Thread.Sleep(2000);
                Dispatcher.Invoke(new Action(() =>
                {
                    qBox.Visibility = Visibility.Visible;
                    logLabel.Visibility = Visibility.Hidden;
                    btnQuery.Visibility = Visibility.Visible;
                }));

            });
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _clipboardInterop = ClipboardInterop.GetClipboardInterop(this);
            _clipboardInterop.StartViewingClipboard();

            _clipboardInterop.ClipboardContentChanged +=
            (o, args) =>
            {
                Debug.WriteLine(DateTime.Now.ToLongTimeString() + " Content changed");
                var txt = Clipboard.GetText();
                if (!string.IsNullOrWhiteSpace(txt))
                {
                    if (txt != lastCopy && chatbot != null && view.Enabled)
                    {
                        view.Query = txt.Trim();
                        QueryBot(txt);
                    }
                }
            };
        }

        private void QueryBot(string txt)
        {
            if (bMain.Visibility == Visibility.Visible || String.IsNullOrWhiteSpace(txt))
            {
                return;
            }

            var height = 0;

            Debug.WriteLine(txt);
            Task.Factory.StartNew(() =>
            {
                try
                {
                    alertMsg("正在查询中...");
                    var results = chatbot.Faq(view.Email, txt);

                    if (results.Count == 0)
                    {
                        alertMsg("未找到匹配结果");
                        return;
                    }


                    results.ForEach(p =>
                    {
                        height += 48;

                        foreach (var r in p.Replies)
                        {
                            var i = p.Replies.IndexOf(r) + 1;
                            height += 24;
                            if (r.Rtype != "plain")
                            {
                                r.Content = JsonConvert.SerializeObject(p.Replies.First(), new JsonSerializerSettings()
                                {
                                    NullValueHandling = NullValueHandling.Ignore
                                });
                            }

                            r.Disply = i + ". " + r.Content;
                        }
                    });

                    view.Faq = results.OrderByDescending(p => p.Score).Take(5).ToList();
                }
                catch (Exception err)
                {
                    if (err.InnerException is HttpRequestException)
                    {
                        alertMsg("网络错误");
                    }
                    else
                    {
                        alertMsg("ID或密钥错误");
                    }

                    return;
                }

                Dispatcher.Invoke(new Action(() =>
                {
                    list.Height = height + 20;
                    list.Visibility = Visibility.Visible;
                }));

                //Dispatcher.Invoke(new Action(() =>
                //SetClipboardText(results[0].Reply)));
            });
        }

        private void SetClipboardText(string msg)
        {
            lastCopy = msg;
            Clipboard.SetText(msg);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(view.Email, @"^([A-Za-z0-9_\-\.])+\@([A-Za-z0-9_\-\.])+\.([A-Za-z]{2,4})$"))
            {
                MessageBox.Show("邮箱格式不正确");
                return;
            }

            view.Secret = appPwd.Password;

            Properties.Settings.Default.email = view.Email;
            Properties.Settings.Default.appId = view.AppId;
            Properties.Settings.Default.secret = view.Secret;
            Properties.Settings.Default.Save();

            StartBot();
        }

        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            if (bMain.Visibility != Visibility.Visible)
            {
                lastEnabled = view.Enabled;
                view.Enabled = false;
                bMain.Visibility = Visibility.Visible;
                list.Visibility = Visibility.Hidden;
            }
            else
            {
                bMain.Visibility = Visibility.Hidden;
                view.Enabled = lastEnabled;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var btn = e.Source as Button;
            var item = btn.DataContext as ChatbotResult.Reply;

            SetClipboardText(item.Content);
            alertMsg("答案已复制");

            //list.Visibility = Visibility.Hidden;
        }

        private void Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Task.Factory.StartNew(() =>
            System.Diagnostics.Process.Start("https://docs.chatopera.com/products/chatbot-platform/howto-guides/faq-assistant.html"));
        }

        private void Feedback_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Task.Factory.StartNew(() =>
            System.Diagnostics.Process.Start("https://github.com/chatopera/cskefu/issues/new/choose"));
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            view.Query = null;
            list.Visibility = Visibility.Hidden;
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            string etr = e.Key.ToString();
            if (etr == "Return")
            {
                QueryBot(qBox.Text);
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            QueryBot(view.Query);
        }
    }
}
