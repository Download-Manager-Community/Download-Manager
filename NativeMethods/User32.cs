using System.Runtime.InteropServices;

namespace DownloadManager.NativeMethods
{
    internal class User32
    {
        public const int WM_ACTIVATE = 0x006;
        public const int WM_ACTIVATEAPP = 0x01C;
        public const int WM_NCACTIVATE = 0x086;

        [DllImport("user32.dll", SetLastError = false)]
        internal static extern IntPtr SendMessageW(IntPtr hWnd, uint msg,
                                                   IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal extern static bool PostMessageW(IntPtr handle, uint msg,
                                                 IntPtr wParam, IntPtr lParam);
    }
}
