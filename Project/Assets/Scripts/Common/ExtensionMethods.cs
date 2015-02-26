using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using ProtoBuf;
using UnityEngine;

public static class ExtensionMethods
{
    /// <summary>
    /// List浅拷贝
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static List<TValue> Clone<TValue>(this List<TValue> source)
    {
        List<TValue> retv = new List<TValue>();
        retv.AddRange(source);
        return retv;
    }

    public static bool Serialize<T>(this T obj, string filename) where T : IExtensible
    {
        FileStream fs = null;
        try
        {
            System.IO.MemoryStream stream = new System.IO.MemoryStream();
            ProtoBuf.Serializer.Serialize<ProtoBuf.IExtensible>(stream, obj);
            System.Byte[] bytes = stream.ToArray();
            fs = new FileStream(filename, FileMode.Create);
            using (BinaryWriter writer = new BinaryWriter(fs))
            {
                writer.Write(bytes);
            }
            return true;
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.LogError(e.Message);
            return false;
        }
        finally
        {
            if (fs != null) fs.Close();
        }
    }

    public static T DeSerialize<T>(string filename) where T : class,IExtensible
    {
        byte[] bytes = File.ReadAllBytes(filename);
        return DeSerialize<T>(bytes);
    }

    public static T DeSerialize<T>(byte[] bytes) where T : class, IExtensible
    {
        var stream = new MemoryStream(bytes);
        return ProtoBuf.Serializer.Deserialize<T>(stream);
    }

    public static void SetLayerRecursively(this GameObject go, int layerNumber)
    {
        foreach (Transform trans in go.GetComponentsInChildren<Transform>(true))
        {
            trans.gameObject.layer = layerNumber;
        }
    }
}