﻿/**
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
using System.Windows.Interop;

namespace Client
{
    public class ClipboardInterop : IDisposable
    {
        public event EventHandler ClipboardContentChanged;

        private void OnClipboardContentChanged()
        {
            var handlers = ClipboardContentChanged;
            if (handlers != null)
            {
                handlers(this, new EventArgs());
            }
        }

        public static ClipboardInterop GetClipboardInterop(Window window)
        {
            var wih = new WindowInteropHelper(window);
            var hwndSource = HwndSource.FromHwnd(wih.Handle);
            if (hwndSource == null)
            {
                return null;
            }

            return new ClipboardInterop(hwndSource);
        }

        private IntPtr _hWndNextViewer;
        private HwndSource _hWndSource;

        public bool IsViewing { get; private set; }

        private ClipboardInterop(HwndSource hwndSource)
        {
            _hWndSource = hwndSource;
            IsViewing = false;
        }

        public bool StartViewingClipboard()
        {
            Win32.SetLastError(0);
            _hWndNextViewer = Win32.SetClipboardViewer(_hWndSource.Handle);
            if (_hWndNextViewer == IntPtr.Zero)
            {
                UInt32 eCode = Win32.GetLastError();
                if (eCode != 0)
                {
                    return false;
                }
            }
            _hWndSource.AddHook(WinProc);
            IsViewing = true;
            return true;
        }


        public bool StopViewingClipboard()
        {
            Win32.SetLastError(0);
            Win32.ChangeClipboardChain(_hWndSource.Handle, _hWndNextViewer);
            _hWndNextViewer = IntPtr.Zero;
            _hWndSource.RemoveHook(WinProc);
            UInt32 eCode = Win32.GetLastError();
            IsViewing = false;
            return eCode == 0;
        }

        private IntPtr WinProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case Win32.WM_CHANGECBCHAIN:
                    if (wParam == _hWndNextViewer)
                    {
                        _hWndNextViewer = lParam;
                    }
                    else if (_hWndNextViewer != IntPtr.Zero)
                    {
                        Win32.SendMessage(_hWndNextViewer, Convert.ToUInt32(msg), wParam, lParam);
                    }
                    break;

                case Win32.WM_DRAWCLIPBOARD:
                    OnClipboardContentChanged();
                    Win32.SendMessage(_hWndNextViewer, Convert.ToUInt32(msg), wParam, lParam);
                    break;

            }
            return IntPtr.Zero;
        }

        public void Dispose()
        {
            if (IsViewing)
                StopViewingClipboard();
            _hWndSource = null;
            _hWndNextViewer = IntPtr.Zero;
        }
    }
}
