using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace JOBSEC
{
    static class MyConsole 
    {
        // Constants definitions
        private const int       STD_OUTPUT_HANDLE       = -11;
        private const int       TMPF_TRUETYPE           = 4;
        private const int       LF_FACESIZE             = 32;
        public const int        FONT_SIZE_X             = 13;
        public const int        FONT_SIZE_Y             = 13;
        public const int        FONT_WEIGHT             = 20;
        public const int        WINDOWS_HEIGHT          = 60,
                                WINDOWS_LENGHT          = 60;
        private static IntPtr   INVALID_HANDLE_VALUE    = new IntPtr(-1);

        // Setting display options, title and font type
        public static void set()
        {
            SetConsoleFont();
            Console.OutputEncoding = Encoding.UTF8;
            Console.Title = "GRAS ALGO FOR JOB SCHEDULING WITH HF INTEGRATION";
        }



        // Setting the core console font.Native dll loading  
        [DllImport("kernel32", SetLastError = true)]
        public static extern bool SetConsoleFont(IntPtr consoleOutput, int index);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool SetCurrentConsoleFontEx( IntPtr  consoleOutput,
                                                    bool    maximumWindow,
                                                    ref     CONSOLE_FONT_INFO_EX consoleCurrentFontEx);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetStdHandle(int dwType);


        public static void SetConsoleFont(string fontName = "Lucida Console")
        {
            unsafe
            {
                IntPtr handle = GetStdHandle(STD_OUTPUT_HANDLE);                        // WINAPI : Getting a handle of current standard windows 
                                                                                        // output devise (active console in sceen buffer) 

                // If success : valid handle 
                if (handle != INVALID_HANDLE_VALUE)
                {
                    CONSOLE_FONT_INFO_EX info   = new CONSOLE_FONT_INFO_EX();             // Info structure for font definition 
                    info.cbSize                 = (uint)Marshal.SizeOf(info);             // Size of the info structure 

                    // Set console font to Lucida Console.
                    CONSOLE_FONT_INFO_EX newInfo= new CONSOLE_FONT_INFO_EX();
                    newInfo.cbSize              = (uint)Marshal.SizeOf(newInfo);
                    newInfo.FontFamily          = TMPF_TRUETYPE;
                    IntPtr ptr                  = new IntPtr(newInfo.FaceName);

                    Marshal.Copy(fontName.ToCharArray(), 0, ptr, fontName.Length);

                    //  All previous information regarding the previous font are at info
                    //  Setting new font using newInfo.................................
                    newInfo.dwFontSize = new COORD(FONT_SIZE_X, FONT_SIZE_X);
                    newInfo.FontWeight = FONT_WEIGHT;

                    SetCurrentConsoleFontEx(handl, false, ref newInfo);
                }
            }

        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal unsafe struct CONSOLE_FONT_INFO_EX
        {
            internal uint       cbSize;                         
            internal uint       nFont;
            internal COORD      dwFontSize;
            internal int        FontFamily;
            internal int        FontWeight;
            internal fixed char FaceName[LF_FACESIZE];
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct COORD
        {
            internal short X;
            internal short Y;

            internal COORD(short x, short y)
            {
                X = x;
                Y = y;
            }
        }

        //Displaying main messages such as the 
        //section names, or main steps........
        public static void displayMain(String message )
        {
            Console.ForegroundColor = ConsoleColor.Yellow ;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        //Displaying possible errors during....
        //..durinf execution 
        public static void displayError(String message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[ERROR]" + message);
            Console.ForegroundColor = ConsoleColor.Gray;
        }


        // Displaying main results 
        public static void displayResult(String message)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("[RESULT]" + message);
            Console.ForegroundColor = ConsoleColor.Gray;
        }


        //Content text, such as progression, values...
        //...etc. 
        public static void display(String message)
        {
            Console.WriteLine(message); 
        }

        //Getting input 
        public static String readInput()
        {
            return Console.ReadLine(); 
        }

        // Setting the console 
        public static void restCursorPosition(CursorPosition cp)
        {
            Console.SetCursorPosition(cp.x, cp.y); 
        }

      
    }

    // internal class for capturing the current  
    // position of the console. To be used when 
    // resetting the console position for a static 
    // display 
    public class CursorPosition
    {
        // x axis position 
        public int x;
        // y axis position 
        public int y;

        // classe constructor 
        public CursorPosition()
        {
            x = Console.CursorLeft;
            y = Console.CursorTop;
        }
    }
}
