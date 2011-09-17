using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using System.Windows;

namespace DevConnect.Container_Files
{
    class AccountDecryption
    {
        //The following is Haxalot's code with his, gabe_k's, and DJ Shepherd's research with slight edits done to the code
        public byte[] Decrypted = new byte[0x184];
        public static byte[] devkey = new byte[] { 0xda, 0xb6, 0x9a, 0xd9, 0x8e, 40, 0x76, 0x4f, 0x97, 0x7e, 0xe2, 0x48, 0x7e, 0x4f, 0x3f, 0x68 };
        public static byte[] retailkey = new byte[] { 0xe1, 0xbc, 0x15, 0x9c, 0x73, 0xb1, 0xea, 0xe9, 0xab, 0x31, 0x70, 0xf3, 0xad, 0x47, 0xeb, 0xf3 };
        public static byte[] CopyArrayPart(byte[] Data, int CopyStart, int Length)
        {
            byte[] buffer = new byte[Length];
            for (int i = 0; i <= (Length - 1); i++)
            {
                buffer[i] = Data[CopyStart + i];
            }
            return buffer;
        }
        public static void SampleMethod(int i) { }
        public static void SampleMethod(ushort s) { }

        public int Decrypt(System.IO.Stream input, int overridekey, bool Dev)
        {
            byte[] retailkey;
            BinaryReader reader = new BinaryReader(input);
            BinaryWriter writer = new BinaryWriter(input);
            reader.BaseStream.Position = 0L;
            byte[] buffer = reader.ReadBytes((int)input.Length);
            new HMACSHA1();
            if (!Dev)
            {
                retailkey = AccountDecryption.retailkey;
            }
            else
            {
                retailkey = devkey;
            }
            if (overridekey == 1)
            {
                retailkey = AccountDecryption.retailkey;
            }
            else if (overridekey == 2)
            {
                retailkey = devkey;
            }
            byte[] array = new HMACSHA1(retailkey).ComputeHash(buffer, 0, 0x10);
            Array.Resize<byte>(ref array, 0x10);
            RC4Session session = RC4CreateSession(array);
            RC4Encrypt(ref session, buffer, 0x10, 8);
            RC4Encrypt(ref session, buffer, 0x18, buffer.Length - 0x18);
            byte[] buffer4 = new HMACSHA1(retailkey).ComputeHash(buffer, 0x10, buffer.Length - 0x10);
            if (memcmp(buffer, buffer4, 0x10))
            {
                writer.BaseStream.Position = 0L;
                writer.Write(buffer, 0x18, buffer.Length - 0x18);
                writer.BaseStream.SetLength((long)(buffer.Length - 0x18));
                writer.Close();
                if (memcmp(retailkey, AccountDecryption.retailkey, 0x10))
                {
                    Dev = false;
                }
                else if (memcmp(retailkey, devkey, 0x10))
                {
                    Dev = true;
                }
                return 1;
            }
            else
            {
                writer.Close();
                if (overridekey == 0)
                {
                    if (memcmp(retailkey, AccountDecryption.retailkey, 0x10))
                    {
                        Decrypt(input, 2, Dev);
                    }
                    else if (memcmp(retailkey, devkey, 0x10))
                    {
                        Decrypt(input, 1, Dev);
                    }
                }
                else
                {
                    return -1; ;
                }
            }
            return 1;
        }
        public void Encrypt(Stream input, bool Dev)
        {
            byte[] retailkey;
            BinaryReader reader = new BinaryReader(input);
            BinaryWriter writer = new BinaryWriter(input);
            byte[] buffer = new byte[((int)input.Length) + 0x18];
            reader.BaseStream.Position = 0L;
            reader.Read(buffer, 0x18, (int)input.Length);
            Random random = new Random();
            for (int i = 0x10; i < 0x18; i++)
            {
                buffer[i] = (byte)random.Next(0, 0x100);
            }
            if (!Dev)
            {
                retailkey = AccountDecryption.retailkey;
            }
            else
            {
                retailkey = devkey;
            }
            byte[] sourceArray = new HMACSHA1(retailkey).ComputeHash(buffer, 0x10, buffer.Length - 0x10);
            Array.Copy(sourceArray, buffer, 0x10);
            byte[] array = new HMACSHA1(retailkey).ComputeHash(sourceArray, 0, 0x10);
            Array.Resize<byte>(ref array, 0x10);
            RC4Session session = RC4CreateSession(array);
            RC4Encrypt(ref session, buffer, 0x10, buffer.Length - 0x10);
            writer.BaseStream.Position = 0L;
            writer.Write(buffer);
            input.SetLength((long)buffer.Length);
            input.Close();
            MessageBox.Show("Encrypted successfully");
        }

        public bool memcmp(byte[] data1, byte[] data2, int length)
        {
            for (int i = 0; i < length; i++)
            {
                if (data1[i] != data2[i])
                {
                    return false;
                }
            }
            return true;
        }

        public RC4Session RC4CreateSession(byte[] key)
        {
            RC4Session session = new RC4Session();
            session.key = key;
            session.i = 0;
            session.j = 0;
            session.s_boxLen = 0x100;
            session.s_box = new byte[session.s_boxLen];
            for (int i = 0; i < session.s_boxLen; i++)
            {
                session.s_box[i] = (byte)i;
            }
            int index = 0;
            for (int j = 0; j < session.s_boxLen; j++)
            {
                index = ((index + session.s_box[j]) + key[j % key.Length]) % session.s_boxLen;
                byte num4 = session.s_box[index];
                session.s_box[index] = session.s_box[j];
                session.s_box[j] = num4;
            }
            return session;
        }

        public void RC4Decrypt(ref RC4Session Session, byte[] Data, int Index, int count)
        {
            RC4Encrypt(ref Session, Data, Index, count);
        }

        public void RC4Encrypt(ref RC4Session Session, byte[] Data, int Index, int count)
        {
            int index = Index;
            do
            {
                Session.i = (Session.i + 1) % 0x100;
                Session.j = (Session.j + Session.s_box[Session.i]) % 0x100;
                byte num2 = Session.s_box[Session.i];
                Session.s_box[Session.i] = Session.s_box[Session.j];
                Session.s_box[Session.j] = num2;
                byte num3 = Data[index];
                byte num4 = Session.s_box[(Session.s_box[Session.i] + Session.s_box[Session.j]) % 0x100];
                Data[index] = (byte)(num3 ^ num4);
                index++;
            }
            while (index != (Index + count));
        }

        public void ResetSession(ref RC4Session Session)
        {
            Session = RC4CreateSession(Session.key);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RC4Session
        {
            public byte[] key;
            public int s_boxLen;
            public byte[] s_box;
            public int i;
            public int j;
        }

        public byte[] ReadBytesFromArray(byte[] Array, int Position, int Count)
        {
            byte[] buffer = new byte[Count];
            for (int i = 0; i < Count; i++)
            {
                buffer[i] = Array[Position + i];
            }
            return buffer;
        }
    }
}
