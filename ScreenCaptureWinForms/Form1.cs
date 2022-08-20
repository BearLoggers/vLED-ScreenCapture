using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace ScreenCaptureWinForms;

public partial class Form1 : Form
{
    [DllImport("user32.dll")]
    private static extern bool GetCursorPos(ref Point lpPoint);

    [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
    private static extern int BitBlt(IntPtr hDC, int x, int y, int nWidth, int nHeight, IntPtr hSrcDC, int xSrc, int ySrc, int dwRop);
    
    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool AllocConsole();

    public Form1()
    {
        AllocConsole();
        
        InitializeComponent();

        MouseMoveTimer.Enabled = true;
        MouseMoveTimer.Interval = 17;
    }

    private void MouseMoveTimer_Tick(object sender, EventArgs e)
    {
        var cursor = new Point();
        GetCursorPos(ref cursor);
        
        Console.WriteLine($"X: {cursor.X}, Y: {cursor.Y}");

        var c = GetColorAt(cursor);
        BackColor = c;

        if (c.R == c.G && c.G < 64 && c.B > 128)
        {
            MessageBox.Show("Blue");
        }
    }

    private readonly Bitmap _screenPixel = new(1, 1, PixelFormat.Format32bppArgb);

    private Color GetColorAt(Point location)
    {
        using (var gdest = Graphics.FromImage(_screenPixel))
        {
            using (var gsrc = Graphics.FromHwnd(IntPtr.Zero))
            {
                var hSrcDC = gsrc.GetHdc();
                var hDC = gdest.GetHdc();
                var retval = BitBlt(hDC, 0, 0, 1, 1, hSrcDC, location.X, location.Y, (int)CopyPixelOperation.SourceCopy);
                gdest.ReleaseHdc();
                gsrc.ReleaseHdc();
            }
        }

        return _screenPixel.GetPixel(0, 0);
    }
}