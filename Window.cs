using System;
using System.Runtime.InteropServices;   // DllImport
using System.Drawing;

namespace PSO2Utility
{
    class Window
    {
        [Flags]
        public enum GetWindowLongFlags : int
        {
            GWL_WNDPROC     = (-4),
            GWL_HINSTANCE   = (-6),
            GWL_HWNDPARENT  = (-8),
            GWL_STYLE       = (-16),
            GWL_EXSTYLE     = (-20),
            GWL_USERDATA    = (-21),
            GWL_ID          = (-12)
        }

        [Flags]
        public enum WindowStyleFlags : uint
        {
            WS_OVERLAPPED       = 0x00000000,
            WS_POPUP            = 0x80000000,
            WS_CHILD            = 0x40000000,
            WS_MINIMIZE         = 0x20000000,
            WS_VISIBLE          = 0x10000000,
            WS_DISABLED         = 0x08000000,
            WS_CLIPSIBLINGS     = 0x04000000,
            WS_CLIPCHILDREN     = 0x02000000,
            WS_MAXIMIZE         = 0x01000000,
            WS_CAPTION          = 0x00C00000,
            WS_BORDER           = 0x00800000,
            WS_DLGFRAME         = 0x00400000,
            WS_VSCROLL          = 0x00200000,
            WS_HSCROLL          = 0x00100000,
            WS_SYSMENU          = 0x00080000,
            WS_THICKFRAME       = 0x00040000,
            WS_GROUP            = 0x00020000,
            WS_TABSTOP          = 0x00010000,

            WS_MINIMIZEBOX      = 0x00020000,
            WS_MAXIMIZEBOX      = 0x00010000,

            WS_TILED            = WS_OVERLAPPED,
            WS_ICONIC           = WS_MINIMIZE,
            WS_SIZEBOX          = WS_THICKFRAME,
            WS_TILEDWINDOW      = WS_OVERLAPPEDWINDOW,

            WS_OVERLAPPEDWINDOW = (WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX),
            WS_POPUPWINDOW      = (WS_POPUP | WS_BORDER | WS_SYSMENU),
            WS_CHILDWINDOW      = WS_CHILD
        }

        [Flags]
        public enum SetWindowPosFlags : uint
        {
            SWP_NOSIZE          = 0x0001,
            SWP_NOMOVE          = 0x0002,
            SWP_NOZORDER        = 0x0004,
            SWP_NOREDRAW        = 0x0008,
            SWP_NOACTIVATE      = 0x0010,
            SWP_FRAMECHANGED    = 0x0020,
            SWP_SHOWWINDOW      = 0x0040,
            SWP_HIDEWINDOW      = 0x0080,
            SWP_NOCOPYBITS      = 0x0100,
            SWP_NOOWNERZORDER   = 0x0200,
            SWP_NOSENDCHANGING  = 0x0400,

            SWP_DRAWFRAME       = SWP_FRAMECHANGED,
            SWP_NOREPOSITION    = SWP_NOOWNERZORDER,

            SWP_DEFERERASE      = 0x2000,
            SWP_ASYNCWINDOWPOS  = 0x4000
        }

        [Flags]
        public enum ShowWindowFlags : int
        {
            SW_HIDE = 0,
            SW_SHOWNORMAL = 1,
            SW_NORMAL = 1,
            SW_SHOWMINIMIZED = 2,
            SW_SHOWMAXIMIZED = 3,
            SW_MAXIMIZE = 3,
            SW_SHOWNOACTIVATE = 4,
            SW_SHOW = 5,
            SW_MINIMIZE = 6,
            SW_SHOWMINNOACTIVE = 7,
            SW_SHOWNA = 8,
            SW_RESTORE = 9,
            SW_SHOWDEFAULT = 10,
            SW_FORCEMINIMIZE = 11,
            SW_MAX = 11
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWindowVisible(IntPtr hWnd);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, GetWindowLongFlags nIndex);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int SetWindowLong(IntPtr hWnd, GetWindowLongFlags nIndex, int dwNewLong);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool AdjustWindowRectEx(ref RECT lpRect, WindowStyleFlags dwStyle, bool bMenu, uint dwExStyle);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, SetWindowPosFlags uFlags);
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, ShowWindowFlags nCmdShow);
        [DllImport("user32.dll")]
        private static extern bool IsIconic(IntPtr hWnd);

        /// <summary>
        /// ウィンドウが存在するか調べる
        /// </summary>
        public static bool Exists(string className)
        {
            return (FindWindow(className, null) != IntPtr.Zero);
        }

        /// <summary>
        /// システムボタンを追加
        /// </summary>
        public static void AddSystemButton(IntPtr hWnd)
        {
            WindowStyleFlags style = (WindowStyleFlags)GetWindowLong(hWnd, GetWindowLongFlags.GWL_STYLE);

            style |= (WindowStyleFlags.WS_SYSMENU | WindowStyleFlags.WS_MINIMIZEBOX);

            SetWindowLong(hWnd, GetWindowLongFlags.GWL_STYLE, (int)style);
        }

        /// <summary>
        /// システムボタンを追加
        /// </summary>
        public static void AddSystemButton(string className)
        {
            IntPtr hWnd = FindWindow(className, null);

            if (hWnd == IntPtr.Zero)
            {
                throw new WindowNotFoundException();
            }

            AddSystemButton(hWnd);
        }

        /// <summary>
        /// ウィンドウの位置を取得
        /// </summary>
        public static Point GetPosition(IntPtr hWnd)
        {
            RECT rect;

            if (!GetWindowRect(hWnd, out rect))
            {
                throw new WindowOperationException();
            }

            return new Point(rect.left, rect.top);
        }

        /// <summary>
        /// ウィンドウの位置を取得
        /// </summary>
        public static Point GetPosition(string className)
        {
            IntPtr hWnd = FindWindow(className, null);

            if (hWnd == IntPtr.Zero)
            {
                throw new WindowNotFoundException();
            }

            return GetPosition(hWnd);
        }

        /// <summary>
        /// ウィンドウのサイズを取得
        /// </summary>
        public static Size GetSize(IntPtr hWnd)
        {
            RECT rect;

            if (!GetClientRect(hWnd, out rect))
            {
                throw new WindowOperationException();
            }

            return new Size(rect.right, rect.bottom);
        }

        /// <summary>
        /// ウィンドウのサイズを取得
        /// </summary>
        public static Size GetSize(string className)
        {
            IntPtr hWnd = FindWindow(className, null);

            if (hWnd == IntPtr.Zero)
            {
                throw new WindowNotFoundException();
            }

            return GetSize(hWnd);
        }

        /// <summary>
        /// ウィンドウの位置とサイズを設定
        /// </summary>
        public static void SetPositionAndSize(IntPtr hWnd, Point position, Size size)
        {
            WindowStyleFlags style = (WindowStyleFlags)GetWindowLong(hWnd, GetWindowLongFlags.GWL_STYLE);
            int exStyle = GetWindowLong(hWnd, GetWindowLongFlags.GWL_EXSTYLE);
            RECT rect;

            rect.left = rect.top = 0;
            rect.right = size.Width;
            rect.bottom = size.Height;

            if (AdjustWindowRectEx(ref rect, style, false, (uint)exStyle))
            {
                SetWindowPos(hWnd, IntPtr.Zero, position.X, position.Y, rect.right - rect.left, rect.bottom - rect.top,
                    SetWindowPosFlags.SWP_NOACTIVATE |
                    SetWindowPosFlags.SWP_NOOWNERZORDER |
                    SetWindowPosFlags.SWP_NOZORDER);
            }
        }

        /// <summary>
        /// ウィンドウの位置とサイズを設定
        /// </summary>
        public static void SetPositionAndSize(string className, Point position, Size size)
        {
            IntPtr hWnd = FindWindow(className, null);

            if (hWnd == IntPtr.Zero)
            {
                throw new WindowNotFoundException();
            }

            SetPositionAndSize(hWnd, position, size);
        }

        /// <summary>
        /// ウィンドウの位置を設定
        /// </summary>
        public static void SetPosition(IntPtr hWnd, Point position)
        {
            SetWindowPos(hWnd, IntPtr.Zero, position.X, position.Y, 0, 0,
                SetWindowPosFlags.SWP_NOACTIVATE |
                SetWindowPosFlags.SWP_NOOWNERZORDER |
                SetWindowPosFlags.SWP_NOSIZE |
                SetWindowPosFlags.SWP_NOZORDER);
        }

        /// <summary>
        /// ウィンドウの位置を設定
        /// </summary>
        public static void SetPosition(string className, Point position)
        {
            IntPtr hWnd = FindWindow(className, null);

            if (hWnd == IntPtr.Zero)
            {
                throw new WindowNotFoundException();
            }

            SetPosition(hWnd, position);
        }

        /// <summary>
        /// ウィンドウサイズの変更を禁止
        /// </summary>
        public static void NoResize(IntPtr hWnd)
        {
            Point position = GetPosition(hWnd);
            Size size = GetSize(hWnd);

            WindowStyleFlags style = (WindowStyleFlags)GetWindowLong(hWnd, GetWindowLongFlags.GWL_STYLE);

            style &= ~WindowStyleFlags.WS_THICKFRAME;
            SetWindowLong(hWnd, GetWindowLongFlags.GWL_STYLE, (int)style);

            // ウィンドウの縁を変更するとクライアント領域のサイズが変わるので、変更前と同じにする
            SetPositionAndSize(hWnd, position, size);
        }

        /// <summary>
        /// ウィンドウサイズの変更を禁止
        /// </summary>
        public static void NoResize(string className)
        {
            IntPtr hWnd = FindWindow(className, null);

            if (hWnd == IntPtr.Zero)
            {
                throw new WindowNotFoundException();
            }

            NoResize(hWnd);
        }

        /// <summary>
        /// ウィンドウを前面にする
        /// </summary>
        public static void SetForeground(IntPtr hWnd)
        {
            // メイン・ウィンドウが最小化されていれば元に戻す
            if (IsIconic(hWnd)) ShowWindowAsync(hWnd, ShowWindowFlags.SW_RESTORE);

            // メイン・ウィンドウを最前面に表示する
            SetForegroundWindow(hWnd);
        }

        /// <summary>
        /// ウィンドウを前面にする
        /// </summary>
        public static void SetForeground(string className)
        {
            IntPtr hWnd = FindWindow(className, null);

            if (hWnd == IntPtr.Zero)
            {
                throw new WindowNotFoundException();
            }

            SetForeground(hWnd);
        }

        private string className = "";
        private IntPtr hWnd = IntPtr.Zero;

        public Window()
        {
        }

        public Window(string newClassName)
        {
            className = newClassName;
            UpdateHandle();
        }

        public string ClassName
        {
            get { return className; }
            set
            {
                className = value;
                UpdateHandle();
            }
        }

        public void UpdateHandle()
        {
            hWnd = FindWindow(className, null);
            if (hWnd == IntPtr.Zero) throw new WindowNotFoundException();
        }

        /// <summary>
        /// ウィンドウの表示状態を調べる
        /// </summary>
        public bool Visible
        {
            get { return IsWindowVisible(hWnd); }
        }

        /// <summary>
        /// システムボタンを追加
        /// </summary>
        public void AddSystemButton()
        {
            AddSystemButton(hWnd);
        }

        /// <summary>
        /// ウィンドウの位置
        /// </summary>
        public Point Position
        {
            get { return GetPosition(hWnd); }
            set { SetPosition(hWnd, value); }
        }

        /// <summary>
        /// ウィンドウのサイズ
        /// </summary>
        public Size Size
        {
            get { return GetSize(hWnd); }
        }

        /// <summary>
        /// ウィンドウの位置とサイズを設定
        /// </summary>
        public void SetPositionAndSize(Point position, Size size)
        {
            SetPositionAndSize(hWnd, position, size);
        }

        /// <summary>
        /// ウィンドウサイズの変更を禁止
        /// </summary>
        public void NoResize()
        {
            NoResize(hWnd);
        }

        /// <summary>
        /// ウィンドウを前面にする
        /// </summary>
        public void SetForeground()
        {
            SetForeground(hWnd);
        }
    }

    /// <summary>
    /// ウィンドウが見つからない場合の例外
    /// </summary>
    class WindowNotFoundException : ApplicationException
    {
        public WindowNotFoundException(string message = "Window Not Found")
            : base(message)
        {
        }
        protected WindowNotFoundException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }
        public WindowNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// ウィンドウの操作に失敗した場合の例外
    /// </summary>
    class WindowOperationException : ApplicationException
    {
        public WindowOperationException(string message = "Window Operation Failed")
            : base(message)
        {
        }
        protected WindowOperationException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }
        public WindowOperationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
