using System;
using System.Runtime.InteropServices;

public class PacketUtils
{
    public static byte[] StructToByteArray (object structure)
    {
        int structSize = Marshal.SizeOf(structure);
        byte[] byteArray = new byte[structSize];

        IntPtr dataPointer = Marshal.AllocHGlobal(structSize);
        Marshal.StructureToPtr(structure, dataPointer, true);
        Marshal.Copy(dataPointer, byteArray, 0, structSize);
        Marshal.FreeHGlobal(dataPointer);
        return byteArray;
    }

    public static object ByteArrayToStruct (byte[] byteArray, object desiredObject)
    {
        int structSize = Marshal.SizeOf(desiredObject);
        
        IntPtr dataPointer = Marshal.AllocHGlobal(structSize);
        Marshal.Copy(byteArray, 0, dataPointer, structSize);

        object result = Marshal.PtrToStructure(dataPointer, desiredObject.GetType());
        Marshal.FreeHGlobal(dataPointer);
        return result;
    }

}
