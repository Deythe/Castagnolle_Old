using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using UnityEngine;

[XmlRoot("data")]
public class DataSerializer : MonoBehaviour
{
    public string path; 
    Encoding encoding = Encoding.UTF8;

    void SetPath()
    {
        path = Application.persistentDataPath;
    }

    public void SaveData<T>(T data)
    {
        SetPath();
        StreamWriter writer = new StreamWriter(Path.Combine(path, typeof(T).Name + ".xml"), false);
        XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
        xmlSerializer.Serialize(writer, data);
        writer.Close();
        Debug.Log("Save"+typeof(T).Name);
    }

    public T LoadData<T>()
    {
        SetPath();
        string completePath = Path.Combine(Path.Combine(path, typeof(T).Name + ".xml"));
        if (File.Exists(completePath))
        {
            FileStream fileStream = new FileStream(completePath, FileMode.Open);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));

            T data = (T) xmlSerializer.Deserialize(fileStream);
            fileStream.Close();
            Debug.Log("Data Loaded");
            return data;
        }
        
        Debug.Log("Path don't exist");
        return default;
    }
}
