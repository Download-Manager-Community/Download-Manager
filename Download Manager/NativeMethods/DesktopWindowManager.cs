using System.Runtime.InteropServices;
using System.Security;

namespace DownloadManager.NativeMethods;

public static class DesktopWindowManager
{
    [StructLayout(LayoutKind.Sequential)]
    public struct MARGINS
    {
        /// <summary>Width of the left border that retains its size.</summary>
        public int cxLeftWidth;

        /// <summary>Width of the right border that retains its size.</summary>
        public int cxRightWidth;

        /// <summary>Height of the top border that retains its size.</summary>
        public int cyTopHeight;

        /// <summary>Height of the bottom border that retains its size.</summary>
        public int cyBottomHeight;

        public MARGINS(int allMargins) => cxLeftWidth = cxRightWidth = cyTopHeight = cyBottomHeight = allMargins;
    }

    [SecurityCritical]
    [DllImport("dwmapi.dll", SetLastError = false, ExactSpelling = true)]
    public static extern int DwmSetWindowAttribute(IntPtr hwnd, uint dwAttribute, [In] IntPtr pvAttribute, int cbAttribute);

    [SecurityCritical]
    [DllImport("dwmapi.dll", SetLastError = false, ExactSpelling = true)]
    public static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, in MARGINS pMarInset);

    private const uint DWMWA_MICA = 1029;
    private const uint DWMWA_IMMERSIVE_DARK_MODE = 20;
    private const uint DWMWA_SYSTEMBACKDROP_TYPE = 38;

    /// <summary>
    /// Check whether Windows build is 22000 or higher, that supports <see cref="SetMica(IntPtr, bool)"/>.
    /// </summary>
    public static bool IsUndocumentedMicaSupported { get; } =
        Environment.OSVersion.Version.Build >= 22000;

    /// <summary>
    /// Check whether Windows Windows build is 22523 or higher, that supports <see cref="SetBackdropType(IntPtr, uint)"/>
    /// </summary>
    public static bool IsBackdropTypeSupported { get; } =
        Environment.OSVersion.Version.Build >= 22523;

    /// <summary>
    /// Enable Mica on target window with <see cref="SetMica(IntPtr, bool)"/> or <see cref="SetBackdropType(IntPtr, uint)"/> if supported.
    /// </summary>
    public static void EnableMicaIfSupported(IntPtr hWnd)
    {
        if (IsBackdropTypeSupported)
        {
            SetBackdropType(hWnd, 2);
        }
        else if (IsUndocumentedMicaSupported)
        {
            SetMica(hWnd, true);
        }
    }

    /// <summary>
    /// Enable or Disable Mica on target window
    /// Supported on Windows builds from 22000 to 22523. It doesn't work on 22523, use <see cref="SetBackdropType(IntPtr, uint)"/> instead.
    /// </summary>
    public static void SetMica(IntPtr hWnd, bool state)
    {
        var value = GCHandle.Alloc(state ? 1 : 0, GCHandleType.Pinned);
        var result = DwmSetWindowAttribute(hWnd, DWMWA_MICA, value.AddrOfPinnedObject(), sizeof(int));
        value.Free();
        if (result != 0)
        {
            throw Marshal.GetExceptionForHR(result);
        }
    }

    /// <summary>
    /// Set backdrop type on target window
    /// Requires Windows build 22523 or higher.
    /// </summary>
    public static void SetBackdropType(IntPtr hWnd, uint backdropType)
    {
        var value = GCHandle.Alloc(backdropType, GCHandleType.Pinned);
        var result = DwmSetWindowAttribute(hWnd, DWMWA_SYSTEMBACKDROP_TYPE, value.AddrOfPinnedObject(), sizeof(uint));
        value.Free();
        if (result != 0)
        {
            throw Marshal.GetExceptionForHR(result);
        }

    }

    /// <summary>
    /// Enable or disable immersive dark mode.
    /// Requires Windows build 19041 or higher.
    /// </summary>
    public static void SetImmersiveDarkMode(IntPtr target, bool state)
    {
        var value = GCHandle.Alloc(state ? 1 : 0, GCHandleType.Pinned);
        var result = DwmSetWindowAttribute(target, DWMWA_IMMERSIVE_DARK_MODE, value.AddrOfPinnedObject(), sizeof(int));
        value.Free();
        if (result != 0)
        {
            throw Marshal.GetExceptionForHR(result);
        }
    }

    /// <summary>
    /// Extends the window frame into the client area.
    /// </summary>
    public static void ExtendFrameIntoClientArea(IntPtr target)
    {
        var margins = new MARGINS(-1);
        var result = DwmExtendFrameIntoClientArea(target, margins);
        if (result != 0)
        {
            throw Marshal.GetExceptionForHR(result);
        }
    }
}