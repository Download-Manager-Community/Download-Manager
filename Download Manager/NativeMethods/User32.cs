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

        public const int SC_CLOSE = 0xF060;
        public const int MF_BYCOMMAND = 0;
        public const int MF_ENABLED = 0;
        public const int MF_GRAYED = 1;

        [DllImport("user32.dll")]
        public static extern IntPtr GetSystemMenu(IntPtr hWnd, bool revert);

        [DllImport("user32.dll")]
        public static extern int EnableMenuItem(IntPtr hMenu, int IDEnableItem, int enable);

        /// <summary>
        /// Toggles the close button on the window.
        /// </summary>
        /// <param name="hWnd">The handle of the form.</param>
        /// <param name="enabled">If the close button should be enabled or not.</param>
        public static void ToggleCloseButton(IntPtr hWnd, bool enabled)
        {
            IntPtr hMenu = NativeMethods.User32.GetSystemMenu(hWnd, false);
            if (hMenu != IntPtr.Zero)
            {
                NativeMethods.User32.EnableMenuItem(hMenu,
                                             NativeMethods.User32.SC_CLOSE,
                                             NativeMethods.User32.MF_BYCOMMAND | (enabled ? NativeMethods.User32.MF_ENABLED : NativeMethods.User32.MF_GRAYED));
            }
        }
    }
}
