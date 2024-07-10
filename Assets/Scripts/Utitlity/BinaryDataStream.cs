using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

public class BinaryDataStream
{
    public static void Save<T>(T serializedObject, string fileName)
    {
        string path = Application.persistentDataPath + "/saves/";
        Directory.CreateDirectory(path);

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path + fileName + ".dat", FileMode.Create);
        try
        {
            formatter.Serialize(stream, serializedObject);            
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to serialize object: " + e.Message);
        }
        finally
        {
            stream.Close();
        }
    }

    public static bool Exists(string fileName)
    {
        string path = Application.persistentDataPath + "/saves/";
        return File.Exists(path + fileName + ".dat");
    }

    public static T Read<T>(string fileName){
        string path = Application.persistentDataPath + "/saves/";
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path + fileName + ".dat", FileMode.Open);
        T returnObject = default(T);
        try
        {
            returnObject = (T)formatter.Deserialize(stream);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to deserialize object: " + e.Message);
        }
        finally
        {
            stream.Close();
        }
        return returnObject;
    }
}
