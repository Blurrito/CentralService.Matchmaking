using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncTypeXTest
{
    public static class EncTypeX
    {
        private static byte Func5(byte[] EncTypeXKey, byte[] Validate, int Index, ref int N1, ref int N2)
        {
            int BitMask = 1;
            if (Index == 0)
                return 0;
            if (Index > 1)
                do
                {
                    BitMask = (BitMask << 1) + 1;
                }
                while (BitMask < Index);

            int i = 0;
            int ReturnValue;
            do
            {
                N1 = EncTypeXKey[N1 & 0xFF] + Validate[N2++];
                if (N2 >= Validate.Length)
                {
                    N2 = 0;
                    N1 += Validate.Length;
                }
                ReturnValue = N1 & BitMask;
                if (++i > 11)
                    ReturnValue %= Index;
            }
            while (ReturnValue > Index);
            return (byte)(ReturnValue & 0xFF);
        }

        private static void Func4(byte[] EncTypeXKey, byte[] Validate)
        {
            int N1 = 0;
            int N2 = 0;

            for (int i = 0; i < 256; i++)
                EncTypeXKey[i] = (byte)i;

            for (int i = 255; i >= 0; i--)
            {
                byte Tmp1 = Func5(EncTypeXKey, Validate, i, ref N1, ref N2);
                byte Tmp2 = EncTypeXKey[i];
                EncTypeXKey[i] = EncTypeXKey[Tmp1];
                EncTypeXKey[Tmp1] = Tmp2;
            }

            EncTypeXKey[256] = EncTypeXKey[1];
            EncTypeXKey[257] = EncTypeXKey[3];
            EncTypeXKey[258] = EncTypeXKey[5];
            EncTypeXKey[259] = EncTypeXKey[7];
            EncTypeXKey[260] = EncTypeXKey[N1 & 0xFF];
        }

        private static void FuncX(byte[] EncTypeXKey, byte[] Key, byte[] Validate, byte[] Data, int DataStart)
        {
            for (int i = 0; i < DataStart; i++)
                Validate[((Key[i % Key.Length] * i) & 7)] ^= (byte)(Validate[(i & 7)] ^ Data[i]);
            Func4(EncTypeXKey, Validate);
        }

        private static byte[] Initialize(byte[] EncTypeXKey, byte[] Key, byte[] Validate, byte[] Data)
        {
            if (Data.Length < 1)
                return null;
            int HeaderLength = (Data[0] ^ 0xEC) + 2;
            if (Data.Length < HeaderLength)
                return null;
            int DataStart = Data[HeaderLength - 1] ^ 0xEA;
            if (Data.Length < (HeaderLength + DataStart))
                return null;

            byte[] Buffer = new byte[Data.Length - HeaderLength];
            Array.Copy(Data, HeaderLength, Buffer, 0, Data.Length - HeaderLength);
            FuncX(EncTypeXKey, Key, Validate, Buffer, DataStart);

            byte[] DataBody = new byte[Data.Length - HeaderLength - DataStart];
            Array.Copy(Data, DataStart + HeaderLength, DataBody, 0, Data.Length - HeaderLength - DataStart);
            return DataBody;
        }

        public static byte[] Decrypt(string Key, string Validate, byte[] Data)
        {
            byte[] EncTypeXKey = new byte[261];

            byte[] KeyBytes = Encoding.UTF8.GetBytes(Key);
            byte[] ValidateBytes = Encoding.UTF8.GetBytes(Validate);
            if (ValidateBytes.Length != 8)
                return null;

            byte[] DataBody = Initialize(EncTypeXKey, KeyBytes, ValidateBytes, Data);
            if (Data == null)
                return null;
            Func6(EncTypeXKey, DataBody);
            return DataBody;
        }

        public static byte[] Encrypt(string Key, string Validate, byte[] Data)
        {
            byte[] EncTypeXKey = new byte[261];

            byte[] KeyBytes = Encoding.UTF8.GetBytes(Key);
            byte[] ValidateBytes = Encoding.UTF8.GetBytes(Validate);
            if (ValidateBytes.Length != 8)
                return null;

            byte[] DataBody = Initialize(EncTypeXKey, KeyBytes, ValidateBytes, Data);
            if (Data == null)
                return null;
            Func6e(EncTypeXKey, DataBody);
            return DataBody;
        }

        private static void Func6(byte[] EncTypeXKey, byte[] Data)
        {
            for (int i = 0; i < Data.Length; i++)
                Data[i] = Func7(EncTypeXKey, Data[i]);
        }

        private static byte Func7(byte[] EncTypeXKey, byte Input)
        {
            byte Tmp1 = EncTypeXKey[256];              //a = encxkey[256];
            byte Tmp2 = EncTypeXKey[257];              //b = encxkey[257];
            byte Tmp3 = EncTypeXKey[Tmp1];             //c = encxkey[a];
            EncTypeXKey[256] = (byte)(Tmp1 + 1);       //encxkey[256] = a + 1;
            EncTypeXKey[257] = (byte)(Tmp2 + Tmp3);    //encxkey[257] = b + c;
            Tmp1 = EncTypeXKey[260];                   //a = encxkey[260];
            Tmp2 = EncTypeXKey[EncTypeXKey[257]];     //b = encxkey[257]; b = encxkey[b];
            Tmp3 = EncTypeXKey[Tmp1];                  //c = encxkey[a];
            EncTypeXKey[Tmp1] = Tmp2;                  //encxkey[a] = b;
            Tmp1 = EncTypeXKey[EncTypeXKey[259]];     //a = encxkey[259]; a = encxkey[a];
            Tmp2 = EncTypeXKey[257];                   //b = encxkey[257];
            EncTypeXKey[Tmp2] = Tmp1;                  //encxkey[b] = a;
            Tmp1 = EncTypeXKey[EncTypeXKey[256]];     //a = encxkey[256]; a = encxkey[a];
            Tmp2 = EncTypeXKey[259];                   //b = encxkey[259];
            EncTypeXKey[Tmp2] = Tmp1;                  //encxkey[b] = a;
            Tmp1 = EncTypeXKey[256];                   //a = encxkey[256];
            EncTypeXKey[Tmp1] = Tmp3;                  //encxkey[a] = c;
            Tmp2 = EncTypeXKey[258];                   //b = encxkey[258];
            Tmp1 = EncTypeXKey[Tmp3];                  //a = encxkey[c];
            Tmp3 = EncTypeXKey[259];                   //c = encxkey[259];
            Tmp2 += Tmp1;                               //b += a;
            EncTypeXKey[258] = Tmp2;                   //encxkey[258] = b;
            Tmp1 = Tmp2;                                //a = b;
            Tmp3 = EncTypeXKey[Tmp3];                  //c = encxkey[c];
            Tmp2 = EncTypeXKey[EncTypeXKey[257]];     //b = encxkey[257]; b = encxkey[b];
            Tmp1 = EncTypeXKey[Tmp1];                  //a = encxkey[a];
            Tmp3 += Tmp2;                               //c += b;
            Tmp2 = EncTypeXKey[EncTypeXKey[260]];     //b = encxkey[260]; b = encxkey[b];
            Tmp3 += Tmp2;                               //c += b;
            Tmp2 = EncTypeXKey[Tmp3];                  //b = encxkey[c];
            Tmp3 = EncTypeXKey[EncTypeXKey[256]];     //c = encxkey[256]; c = encxkey[c];
            Tmp1 += Tmp3;                               //a += c;
            Tmp3 = EncTypeXKey[Tmp2];                  //c = encxkey[b];
            Tmp2 = EncTypeXKey[Tmp1];                  //b = encxkey[a];
            EncTypeXKey[260] = Input;                  //encxkey[260] = d;
            Tmp3 ^= (byte)(Tmp2 ^ Input);               //c ^= b ^ d;
            EncTypeXKey[259] = Tmp3;                   //encxkey[259] = c;
            return Tmp3;                                //return (c);
        }

        private static void Func6e(byte[] EncTypeXKey, byte[] Data)
        {
            for (int i = 0; i < Data.Length; i++)
                Data[i] = Func7e(EncTypeXKey, Data[i]);
        }

        private static byte Func7e(byte[] EncTypeXKey, byte Input)
        {
            byte Tmp1 = EncTypeXKey[256];              //a = encxkey[256];
            byte Tmp2 = EncTypeXKey[257];              //b = encxkey[257];
            byte Tmp3 = EncTypeXKey[Tmp1];             //c = encxkey[a];
            EncTypeXKey[256] = (byte)(Tmp1 + 1);       //encxkey[256] = a + 1;
            EncTypeXKey[257] = (byte)(Tmp2 + Tmp3);    //encxkey[257] = b + c;
            Tmp1 = EncTypeXKey[260];                   //a = encxkey[260];
            Tmp2 = EncTypeXKey[EncTypeXKey[257]];     //b = encxkey[257]; b = encxkey[b];
            Tmp3 = EncTypeXKey[Tmp1];                  //c = encxkey[a];
            EncTypeXKey[Tmp1] = Tmp2;                  //encxkey[a] = b;
            Tmp1 = EncTypeXKey[EncTypeXKey[259]];     //a = encxkey[259]; a = encxkey[a];
            Tmp2 = EncTypeXKey[257];                   //b = encxkey[257];
            EncTypeXKey[Tmp2] = Tmp1;                  //encxkey[b] = a;
            Tmp1 = EncTypeXKey[EncTypeXKey[256]];     //a = encxkey[256]; a = encxkey[a];
            Tmp2 = EncTypeXKey[259];                   //b = encxkey[259];
            EncTypeXKey[Tmp2] = Tmp1;                  //encxkey[b] = a;
            Tmp1 = EncTypeXKey[256];                   //a = encxkey[256];
            EncTypeXKey[Tmp1] = Tmp3;                  //encxkey[a] = c;
            Tmp2 = EncTypeXKey[258];                   //b = encxkey[258];
            Tmp1 = EncTypeXKey[Tmp3];                  //a = encxkey[c];
            Tmp3 = EncTypeXKey[259];                   //c = encxkey[259];
            Tmp2 += Tmp1;                               //b += a;
            EncTypeXKey[258] = Tmp2;                   //encxkey[258] = b;
            Tmp1 = Tmp2;                                //a = b;
            Tmp3 = EncTypeXKey[Tmp3];                  //c = encxkey[c];
            Tmp2 = EncTypeXKey[EncTypeXKey[257]];     //b = encxkey[257]; b = encxkey[b];
            Tmp1 = EncTypeXKey[Tmp1];                  //a = encxkey[a];
            Tmp3 += Tmp2;                               //c += b;
            Tmp2 = EncTypeXKey[EncTypeXKey[260]];     //b = encxkey[260]; b = encxkey[b];
            Tmp3 += Tmp2;                               //c += b;
            Tmp2 = EncTypeXKey[Tmp3];                  //b = encxkey[c];
            Tmp3 = EncTypeXKey[EncTypeXKey[256]];     //c = encxkey[256]; c = encxkey[c];
            Tmp1 += Tmp3;                               //a += c;
            Tmp3 = EncTypeXKey[Tmp2];                  //c = encxkey[b];
            Tmp2 = EncTypeXKey[Tmp1];                  //b = encxkey[a];
            Tmp3 ^= (byte)(Tmp2 ^ Input);               //c ^= b ^ d;
            EncTypeXKey[260] = Tmp3;                  //encxkey[260] = c;
            EncTypeXKey[259] = Input;                   //encxkey[259] = d;
            return Tmp3;                                //return (c);
        }
    }
}
