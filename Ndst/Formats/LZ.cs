using System;
using System.IO;

namespace Ndst.Formats {

    // An LZ compressed file.
    public class LZFile : IFormat {
        public bool HasHeader;
        public byte[] CompressedData;

        public bool IsType(byte[] data) {
            if (data.Length > 4) {
                if (data[0] == 'L' && data[1] == 'Z' && data[2] == '7' && data[3] == '7') {
                    HasHeader = true;
                    return true;
                } else if (data[0] == 0x10) {
                    HasHeader = false;
                    try {
                        LZ77_Decompress(data, false);
                        return true;
                    } catch {}
                }
            }
            return false;
        }

        public void Read(BinaryReader r, byte[] rawData) {
            CompressedData = rawData;
        }

        public void Write(BinaryWriter w) {
            w.Write(CompressedData);
        }

        public void Extract(string path) {

            // Decompress data.
            System.IO.File.WriteAllBytes(path, LZ77_Decompress(CompressedData, HasHeader));

        }

        public void Pack(string path) {

            // Compress data.
            CompressedData = LZ77_Compress(System.IO.File.ReadAllBytes(path), HasHeader);

        }

        public string GetFormat() {
            return HasHeader ? "LZH" : "LZ";
        }

        public bool IsOfFormat(string str) {
            if (str.Equals("LZ")) {
                HasHeader = false;
                return true;
            } else if (str.Equals("LZH")) {
                HasHeader = true;
                return true;
            }
            return false;
        }

        public byte[] ContainedFile() {
            return LZ77_Decompress(CompressedData, HasHeader);
        }

        public string GetPathExtension() => "";

        public static void LZ77_Compress_Search(byte[] data, int pos, out int match, out int length) {
            int maxMatchDiff = 4096;
            int maxMatchLen = 18;
            match = 0;
            length = 0;

            int start = pos - maxMatchDiff;
            if (start < 0) start = 0;

            for (int thisMatch = start; thisMatch < pos; thisMatch++)
            {
                int thisLength = 0;
                while(thisLength < maxMatchLen
                    && thisMatch + thisLength < pos 
                    && pos + thisLength < data.Length
                    && data[pos+thisLength] == data[thisMatch+thisLength])
                    thisLength++;

                if(thisLength > length)
                {
                    match = thisMatch;
                    length = thisLength;
                }

                //We can't improve the max match length again...
                if(length == maxMatchLen)
                    return;
            }
        }

        public static byte[] LZ77_Compress(byte[] data, bool header = false) {
            ByteArrayOutputStream res = new ByteArrayOutputStream();
            if (header)
            {
                res.writeUInt(0x37375A4C); //LZ77
            }
            
            res.writeInt((data.Length << 8) | 0x10);

            byte[] tempBuffer = new byte[16];

            //Current byte to compress.
            int current = 0;

            while (current < data.Length)
            {
                int tempBufferCursor = 0;
                byte blockFlags = 0;
                for (int i = 0; i < 8; i++)

                {
                    //Not sure if this is needed. The DS probably ignores this data.
                    if (current >= data.Length)
                    {
                        tempBuffer[tempBufferCursor++] = 0;
                        continue;
                    }

                    int searchPos = 0;
                    int searchLen = 0;
                    LZ77_Compress_Search(data, current, out searchPos, out searchLen);
                    int searchDisp = current - searchPos - 1;
                    if (searchLen > 2) //We found a big match, let's write a compressed block.
                    {
                        blockFlags |= (byte)(1 << (7 - i));
                        tempBuffer[tempBufferCursor++] = (byte)((((searchLen - 3) & 0xF) << 4) + ((searchDisp >> 8) & 0xF));
                        tempBuffer[tempBufferCursor++] = (byte)(searchDisp & 0xFF);
                        current += searchLen;
                    }
                    else
                    {
                        tempBuffer[tempBufferCursor++] = data[current++];
                    }
                }

                res.writeByte(blockFlags);
                for (int i = 0; i < tempBufferCursor; i++)
                    res.writeByte(tempBuffer[i]);
            }

            return res.getArray();
        }


        public static byte[] LZ77_FastCompress(byte[] source) {
            int DataLen = 4;
            DataLen += source.Length;
            DataLen += (int)Math.Ceiling((double)source.Length / 8);
            byte[] dest = new byte[DataLen];

            dest[0] = 0;
            dest[1] = (byte)(source.Length & 0xFF);
            dest[2] = (byte)((source.Length >> 8) & 0xFF);
            dest[3] = (byte)((source.Length >> 16) & 0xFF);

            int FilePos = 4;
            int UntilNext = 0;

            for (int SrcPos = 0; SrcPos < source.Length; SrcPos++)
            {
                if (UntilNext == 0)
                {
                    dest[FilePos] = 0;
                    FilePos++;
                    UntilNext = 8;
                }
                dest[FilePos] = source[SrcPos];
                FilePos++;
                UntilNext -= 1;
            }

            return dest;
        }

        public static int LZ77_GetDecompressedSize(byte[] source, bool WithHeader) {
            // This code converted from Elitemap 
            int DataLen;
            if(!WithHeader)
                DataLen = source[1] | (source[2] << 8) | (source[3] << 16);
            else
                DataLen = source[5] | (source[6] << 8) | (source[7] << 16);
            return DataLen;
        }
        
        public static byte[] LZ77_Decompress(byte[] source, bool WithHeader) {
            // This code converted from Elitemap 
            int DataLen;
            DataLen = source[1] | (source[2] << 8) | (source[3] << 16);
            if(WithHeader)
                DataLen = source[5] | (source[6] << 8) | (source[7] << 16);
            byte[] dest = new byte[DataLen];
            int i, j, xin, xout;
            xin = 4;
            if (WithHeader)
                xin = 8;
            xout = 0;
            int length, offset, windowOffset, data;
            byte d;
            while (DataLen > 0)
            {
                d = source[xin++];
                if (d != 0)
                {
                    for (i = 0; i < 8; i++)
                    {
                        if ((d & 0x80) != 0)
                        {
                            data = ((source[xin] << 8) | source[xin + 1]);
                            xin += 2;
                            length = (data >> 12) + 3;
                            offset = data & 0xFFF;
                            windowOffset = xout - offset - 1;
                            for (j = 0; j < length; j++)
                            {
                                dest[xout++] = dest[windowOffset++];
                                DataLen--;
                                if (DataLen == 0)
                                {
                                    return dest;
                                }
                            }
                        }
                        else
                        {
                            dest[xout++] = source[xin++];
                            DataLen--;
                            if (DataLen == 0)
                            {
                                return dest;
                            }
                        }
                        d <<= 1;
                    }
                }
                else
                {
                    for (i = 0; i < 8; i++)
                    {
                        dest[xout++] = source[xin++];
                        DataLen--;
                        if (DataLen == 0)
                        {
                            return dest;
                        }
                    }
                }
            }
            return dest;
        }

    }

    public class ByteArrayOutputStream {
        //implements an unbonded array to store unlimited data.
        //writes in amortized constant time.

        private byte[] buf = new byte[16];
        private int pos = 0;

        public ByteArrayOutputStream()
        {
        }

        public int getPos()
        {
            return pos;
        }

        public byte[] getArray()
        {
            byte[] ret = new byte[pos];
            Array.Copy(buf, ret, pos);
            return ret;
        }

        public void writeByte(byte b)
        {
            if (buf.Length <= pos)
                grow();

            buf[pos] = b;
            pos++;
        }

        public void writeUShort(ushort u)
        {
            writeByte((byte)u);
            writeByte((byte)(u >> 8));
        }
        public void writeUInt(uint u)
        {
            writeByte((byte)u);
            writeByte((byte)(u >> 8));
            writeByte((byte)(u >> 16));
            writeByte((byte)(u >> 24));
        }

        public void writeInt(int i)
        {
            writeUInt((uint)i);

        }
        public void writeLong(long u)
        {
            writeByte((byte)u);
            writeByte((byte)(u >> 8));
            writeByte((byte)(u >> 16));
            writeByte((byte)(u >> 24));
            writeByte((byte)(u >> 32));
            writeByte((byte)(u >> 40));
            writeByte((byte)(u >> 48));
            writeByte((byte)(u >> 56));
        }

        public void align(int m)
        {
            while (pos % m != 0)
                writeByte(0);
        }

        private void grow()
        {
            byte[] nbuf = new byte[buf.Length * 2];
            Array.Copy(buf, nbuf, buf.Length);
            buf = nbuf;
        }

        public void write(byte[] ar)
        {
            for (int i = 0; i < ar.Length; i++)
                writeByte(ar[i]);
        }
    }

}