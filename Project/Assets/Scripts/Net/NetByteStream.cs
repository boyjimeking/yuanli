using UnityEngine;
using System.Collections;

public class NetByteStream
{
    //定义包头固定长度
    public const int header_len = 4;
    //定义消息体长度
    public const int max_body_len = 512;
    //字节长度为1
    public const int BYTE_LEN = 1;
    //int长度为4
    public const int INT_LEN = 4;
    //short长度为2
    public const int SHORT_LEN = 2;
    //字节流
    public byte[] byteStream;
    //操作索引(读和写)
    private int operIndex = 0;
    public NetByteStream()
    {
        operIndex = 0;
        byteStream = new byte[header_len+max_body_len];
    }
    /**
     * 整型写入字节流
     * */
    public void WriteInt(int value)
    {
        if ((operIndex + INT_LEN) > max_body_len) return;
        byte[] bs = System.BitConverter.GetBytes(value);
        bs.CopyTo(byteStream, operIndex);
        operIndex += INT_LEN;
    }
    /**
     * bool值写入
     * */
    public void WriteBool(bool value)
    {
        if ((operIndex + BYTE_LEN) > max_body_len) return;
        byte b = (byte)'1';
        if (!value)
            b = (byte)'0';
        byteStream[operIndex] = b;
        operIndex += BYTE_LEN;
    }
    /**
     * 字节写入
     * */
    public void WriteByte(byte value)
    {
        if ((operIndex + BYTE_LEN) > max_body_len) return;
        byteStream[operIndex] = value;
        operIndex += BYTE_LEN;
    }
    /**
     * 字符串写入
     * */
    public void WriteString(string value)
    {
        int len = System.Text.Encoding.UTF8.GetByteCount(value);
        if ((operIndex + len) > max_body_len) return;
        this.WriteInt(len);
        System.Text.Encoding.UTF8.GetBytes(value, 0, value.Length, byteStream, operIndex);
        operIndex += len;
    }
    /**
     * 字节流写入
     * */
    public void WriteBytes(byte[] bytes)
    {
        if ((operIndex + bytes.Length) > max_body_len) return;
        int len = bytes.Length;
        this.WriteInt(len);
        foreach (byte b in bytes)
        {
            byteStream[operIndex] = b;
            operIndex += BYTE_LEN;
        }
    }
    /**
     * 读int
     * */
    public int ReadInt()
    {
        if ((operIndex + INT_LEN) > max_body_len) return -1;
        int number = System.BitConverter.ToInt32(byteStream,operIndex);
        operIndex += INT_LEN;
        return number;
    }
    /**
     * 读bool值
     * */
    public bool ReadBool()
    {
        if ((operIndex + BYTE_LEN) > max_body_len) return false;
        byte b = byteStream[operIndex];
        if (b == (byte)'1')
            return true;
        else
            return false;
    }
    /**
     * 读取字节数组
     * */
    public byte[] ReadBytes(int len)
    {
        if ((operIndex + len*BYTE_LEN) > max_body_len) return null;
        byte [] returnBytes = new byte[len];
        for(int i=0;i<len;i++)
        {
            returnBytes[i] = byteStream[operIndex];
            operIndex += BYTE_LEN;
        }
        return returnBytes;
    }
    /**
     * 读取字符串
     * */
    public string ReadString(int len)
    {
        if ((operIndex + len) > max_body_len) return "";
        string str = System.Text.Encoding.UTF8.GetString(byteStream, operIndex, len);
        operIndex += len;
        return str;
    }
}
