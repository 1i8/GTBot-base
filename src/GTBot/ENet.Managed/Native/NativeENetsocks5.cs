using System.Runtime.InteropServices;

namespace ENet.Managed.Native
{
    [StructLayout(LayoutKind.Sequential)]
    public struct NativeENetsocks5
    {
        public NativeENetAddress address;
        public char[] username;
        public char[] password;

        public NativeENetsocks5(NativeENetAddress address, char[] username, char[] password)
        {
            this.address = address;
            this.username = username;
            this.password = password;
        }
    }
}
