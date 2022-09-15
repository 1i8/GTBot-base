using System.Text;
namespace GTBot.Common;

public class Variant
{
    public static byte[] get_extended_data(byte[] data) => data.Skip(56).ToArray();

    public static byte[]? get_struct_data(byte[] data)
    {
        var dataLength = data.Length;
        if (dataLength >= 0x3c)
        {
            var structData = new byte[dataLength - 4];
            Array.Copy(data, 4, structData, 0, dataLength - 4);
            var p2Len = BitConverter.ToInt32(data, 56);
            if ((data[16] & 8) == 0)
                Array.Copy(BitConverter.GetBytes(0), 0, data, 56, 4);
            return structData;
        }
        return null;
    }

    public static variant_t GetCall(byte[] package)
    {
        var varList = new variant_t();
        var pos = 0;
        var argsTotal = package[pos++];
        if (argsTotal > 7) return varList;
        varList.functionArgs = new object[argsTotal];

        for (var i = 0; i < argsTotal; i++)
        {
            varList.functionArgs[i] = 0;
            var index = package[pos++];
            var type = package[pos++];
            switch (type)
            {
                case 1:
                    {
                        pos += 4;
                        break;
                    }
                case 2: // string
                    var strLen = BitConverter.ToInt32(package, pos);
                    pos += 4;
                    var v = string.Empty;
                    v = Encoding.ASCII.GetString(package, pos, strLen);
                    pos += strLen;

                    if (index == 0)
                        varList.FunctionName = v;

                    if (index > 0)
                    {
                        varList.functionArgs[index] = v;
                    }

                    break;
                case 5: // uint
                    var vUInt = BitConverter.ToUInt32(package, pos);
                    pos += 4;
                    varList.functionArgs[index] = vUInt;
                    break;
                case 9
                    :
                    var vInt = BitConverter.ToInt32(package, pos);
                    pos += 4;
                    varList.functionArgs[index] = vInt;
                    break;
            }
        }
        return varList;
    }

    public struct variant_t
    {
        public string FunctionName;
        public int netid;
        public uint delay;
        public object[] functionArgs;
    }
}
