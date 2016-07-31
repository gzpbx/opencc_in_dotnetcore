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

            if (File.Exists(DLL_NAME_64))
            {
                IntPtr o = opencc_open(@"../../opencc/s2t.json");

                if (o != IntPtr.Zero && o != new IntPtr(-1))
                {
                    var str = "龙，头发 发现";
                    Console.WriteLine(str);
                    var input = Encoding.UTF8.GetBytes(str);
                    IntPtr output = opencc_convert_utf8(o, input, -1);
                    var result = PtrToStringUtf8(output);
                    opencc_convert_utf8_free(output);
                    Console.WriteLine(result);
                }
                else
                {
                    Console.WriteLine("something is wrong !! ");
                }
            }
            else
            {
                Console.WriteLine("opencc.dll doesn't exits !!, CurrentDirectory: " + Directory.GetCurrentDirectory());
            }

            Console.WriteLine("Press any key to exit !!");
            Console.ReadKey();
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

        const string DLL_NAME_64 = "../../opencc/opencc.dll";

        [DllImport(DLL_NAME_64, EntryPoint = "opencc_open", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr opencc_open(string configFileName);

        [DllImport(DLL_NAME_64, EntryPoint = "opencc_convert_utf8", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr opencc_convert_utf8(IntPtr opencc, [MarshalAs(UnmanagedType.LPArray)]byte[] input, long length);

        [DllImport(DLL_NAME_64, EntryPoint = "opencc_convert_utf8_free", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void opencc_convert_utf8_free(IntPtr str);

    }
}
