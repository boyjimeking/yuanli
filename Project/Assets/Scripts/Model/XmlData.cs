using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using UnityEngine;
using System.Collections;

public class XmlData
{
    public static void MakeSkeleton<T>() where T : new()
    {
        List<T> list = new List<T>();
        Type type = typeof (T);
        var obj = new T();
        FillNullString(type,obj);
        list.Add(obj);
        list.Add(obj);
        XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<T>));
        TextWriter writer = new StreamWriter(Application.streamingAssetsPath + "/XmlDataSkeletons/" + typeof(T).Name + ".xml");
        xmlSerializer.Serialize(writer, list);
        writer.Flush();
        writer.Close();
    }
    private static void FillNullString<T>(Type type,T inst)
    {
        var properties = type.GetProperties();
        Type stringType = typeof (string);
        foreach (var propertyInfo in properties)
        {
            if(stringType == propertyInfo.PropertyType)
            {
                propertyInfo.SetValue(inst,string.Empty,null);
            }
        }

        var fields = type.GetFields();
        foreach (var fieldInfo in fields)
        {
            if(stringType == fieldInfo.FieldType)
            {
                fieldInfo.SetValue(inst,string.Empty);
            }
        }
    }
    public static List<T> Deserialize<T>()
    {
        XmlSerializer deserializer = new XmlSerializer(typeof(List<T>));
        var xmlData = Resources.Load<TextAsset>("XmlData/" + typeof (T).Name).text;
        var textReader = new StringReader(xmlData);
        List<T> datas = (List<T>) deserializer.Deserialize(textReader);
        textReader.Close();
        return datas;
    }
}
