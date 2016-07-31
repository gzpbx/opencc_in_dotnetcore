using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var str = "龙，头发 发现";
            Console.WriteLine(str);
            Console.WriteLine(BasicToTraditional(str));

            Console.WriteLine("Press any key to exit !!");
            Console.ReadKey();
        }

        private static string BasicToTraditional(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                throw new ArgumentNullException("input");
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                IntPtr o_win = opencc_open_win(@"../../opencc/s2t.json");

                if (o_win != IntPtr.Zero && o_win != new IntPtr(-1))
                {
                    IntPtr output = opencc_convert_utf8_win(o_win, Encoding.UTF8.GetBytes(input), -1);
                    var result = PtrToStringUtf8(output);
                    opencc_convert_utf8_free_win(output);
                    return result;                }
                else
                {
                    Console.WriteLine("something is wrong !! ");
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                // make sure execute the command below first -
                // sudo ln -s /usr/lib/libopencc.so.2 /usr/lib64/libopencc.so.2
                IntPtr o_win = opencc_open_linux("s2t.json");

                if (o_win != IntPtr.Zero && o_win != new IntPtr(-1))
                {
                    IntPtr output = opencc_convert_utf8_linux(o_win, Encoding.UTF8.GetBytes(input), -1);
                    var result = PtrToStringUtf8(output);
                    opencc_convert_utf8_free_linux(output);
                    return result;
                }
                else
                {
                    Console.WriteLine("something is wrong !! ");
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return input;
            }

            return input;
        }

        private static string PtrToStringUtf8(IntPtr ptr) // aPtr is nul-terminated
        {
            if (ptr == IntPtr.Zero)
                return "";
            int len = 0;
            while (Marshal.ReadByte(ptr, len) != 0)
                len++;
            if (len == 0)
                return "";
            byte[] array = new byte[len];
            Marshal.Copy(ptr, array, 0, len);
            return System.Text.Encoding.UTF8.GetString(array);
        }

        const string DLL_NAME_64_Win = "../../opencc/opencc.dll";

        [DllImport(DLL_NAME_64_Win, EntryPoint = "opencc_open", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr opencc_open_win(string configFileName);

        [DllImport(DLL_NAME_64_Win, EntryPoint = "opencc_convert_utf8", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr opencc_convert_utf8_win(IntPtr opencc, [MarshalAs(UnmanagedType.LPArray)]byte[] input, long length);

        [DllImport(DLL_NAME_64_Win, EntryPoint = "opencc_convert_utf8_free", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void opencc_convert_utf8_free_win(IntPtr str);


        const string DLL_NAME_64_Linux = "libopencc.so.2";

        [DllImport(DLL_NAME_64_Linux, EntryPoint = "opencc_open", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr opencc_open_linux(string configFileName);

        [DllImport(DLL_NAME_64_Linux, EntryPoint = "opencc_convert_utf8", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr opencc_convert_utf8_linux(IntPtr opencc, [MarshalAs(UnmanagedType.LPArray)]byte[] input, long length);

        [DllImport(DLL_NAME_64_Linux, EntryPoint = "opencc_convert_utf8_free", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void opencc_convert_utf8_free_linux(IntPtr str);

    }
}
