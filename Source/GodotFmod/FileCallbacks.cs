using System;
using System.Runtime.InteropServices;
using FMOD;

using Godot;

namespace GodotFmod
{
    public class FileCallbacks
    {
        private static FileCallbacks _instance;
        public static FileCallbacks Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new FileCallbacks();

                return _instance;
            }
        }

        public RESULT OpenFile(IntPtr name, ref uint filesize, ref IntPtr handle, IntPtr userdata)
        {
            return RESULT.ERR_FILE_NOTFOUND;
        }

        public RESULT CloseFile(IntPtr handle, IntPtr userdata)
        {
            return RESULT.ERR_FILE_NOTFOUND;
        }

        public RESULT ReadFile(IntPtr handle, IntPtr buffer, uint sizebytes, ref uint bytesread, IntPtr userdata)
        {
            return RESULT.ERR_FILE_NOTFOUND;
        }

        public RESULT SeekFile(IntPtr handle, uint pos, IntPtr userdata)
        {
            return RESULT.ERR_FILE_NOTFOUND;
        }
    }
}