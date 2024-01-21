using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace Passimo.Cryptography;

public class SecureStringAccessor : IDisposable
{
    private const int scrambleLength = 1024;
    private const byte scrambleByteMin = 65; // A
    private const byte scrambleByteMax = 122; // z

    private readonly IntPtr _valuePtr;
    private bool _disposed;

    public SecureStringAccessor(SecureString secureString)
    {
        _valuePtr = Marshal.SecureStringToGlobalAllocUnicode(secureString);
    }

    public void GetValue(ref string value)
    {
        if (_disposed) throw new ObjectDisposedException(value.GetType().FullName);
        value = Marshal.PtrToStringUni(_valuePtr)!;
    }

    public void Scramble(ref string value)
    {
        var scrambleBuffer = new byte[scrambleLength];
        var range = scrambleByteMax - scrambleByteMin;
        for (var i = 0; i < scrambleLength; ++i)
        {
            scrambleBuffer[i] = (byte)(scrambleByteMin + (scrambleByteMin + i) % range);
        }
        value = Encoding.UTF8.GetString(scrambleBuffer);
    }

    public void Dispose()
    {
        _disposed = true;
        Marshal.ZeroFreeGlobalAllocUnicode(_valuePtr);
    }
}
