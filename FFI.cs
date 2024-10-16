using System.Runtime.InteropServices;

namespace StudioEnhancementSuite.FFI;

using HWND = nint;

internal static class User32 {
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool ShowWindow(HWND hWnd, ShowWindowCmd nCmdShow);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetWindowPlacement(HWND hWnd, ref WindowPlacement lpwndpl);
}

internal enum ShowWindowCmd : int {
    Hide = 0,
    ShowNormal = 1,
    ShowMinimized = 2,
    ShowMaximized = 3,
    ShowNoActivate = 4,
    Show = 5,
    Minimize = 6,
    ShowMinNoActive = 7,
    ShowNA = 8,
    Restore = 9,
    ShowDefault = 10,
    ForceMinimize = 11,
}

[StructLayout(LayoutKind.Sequential)]
internal record struct WindowPlacement {
    public WindowPlacement() {
        this.length = Marshal.SizeOf<WindowPlacement>();
    }

    public int length, flags;
    public ShowWindowCmd showCmd;
    public Point ptMinPosition, ptMaxPosition;
}

[StructLayout(LayoutKind.Sequential)]
internal record struct Point {
    public int x, y;
}

[StructLayout(LayoutKind.Sequential)]
internal record struct Rect {
    public int left, top, right, bottom;
}
