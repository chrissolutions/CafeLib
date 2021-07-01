using CafeLib.BsvSharp.Numerics;

namespace CafeLib.BsvSharp.Persistence
{
    public interface IDataWriter
    {
        IDataWriter Write(byte[] data);
        IDataWriter Write(byte v);
        IDataWriter Write(int v);
        IDataWriter Write(uint v);
        IDataWriter Write(long v);
        IDataWriter Write(ulong v);
        IDataWriter Write(UInt160 v);
        IDataWriter Write(UInt256 v);
        IDataWriter Write(UInt512 v);
    }
}
