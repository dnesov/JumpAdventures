using System;
using System.Runtime.InteropServices;
using System.Text;
using FMOD;

using Godot;

namespace GodotFmod
{
    public static class FmodFileCallbacks
    {
        public static RESULT OpenFile(IntPtr name, ref uint filesize, ref IntPtr handle, IntPtr userdata)
        {
            string stringName = StringFromNativeUtf8(name);
            _logger.Info($"Opening {stringName}...");
            return RESULT.ERR_FILE_NOTFOUND;
        }

        public static RESULT CloseFile(IntPtr handle, IntPtr userdata)
        {
            return RESULT.ERR_FILE_NOTFOUND;
        }

        public static RESULT ReadFile(IntPtr handle, IntPtr buffer, uint sizebytes, ref uint bytesread, IntPtr userdata)
        {
            return RESULT.ERR_FILE_NOTFOUND;
        }

        public static RESULT SeekFile(IntPtr handle, uint pos, IntPtr userdata)
        {
            return RESULT.ERR_FILE_NOTFOUND;
        }

        private static string StringFromNativeUtf8(IntPtr nativeUtf8)
        {
            int len = 0;
            while (Marshal.ReadByte(nativeUtf8, len) != 0) ++len;
            byte[] buffer = new byte[len];
            Marshal.Copy(nativeUtf8, buffer, 0, buffer.Length);
            return Encoding.UTF8.GetString(buffer);
        }

        private static Logger _logger = new(nameof(FmodFileCallbacks));
    }
}