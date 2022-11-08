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
using System.Runtime.InteropServices;
using System.Text;

namespace Client
{
    internal static class Win32
    {
        /// <summary>
        ///     The WM_DRAWCLIPBOARD message notifies a clipboard viewer window that
        ///     the content of the clipboard has changed.
        /// </summary>
        internal const int WM_DRAWCLIPBOARD = 0x0308;

        /// <summary>
        ///     A clipboard viewer window receives the WM_CHANGECBCHAIN message when
        ///     another window is removing itself from the clipboard viewer chain.
        /// </summary>
        internal const int WM_CHANGECBCHAIN = 0x030D;

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetClipboardViewer(
            IntPtr hWndNewViewer);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ChangeClipboardChain(
            IntPtr hWndRemove,
            IntPtr hWndNewNext);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SendMessage(
            IntPtr hWnd,
            UInt32 msg,
            IntPtr wParam,
            IntPtr lParam);

        [DllImport("kernel32.dll")]
        public static extern void SetLastError(
            UInt32 errorCode);

        [DllImport("kernel32.dll")]
        public static extern UInt32 GetLastError();
    }
}
