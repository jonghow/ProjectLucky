using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Linq;

/// <summary>
/// XML Save&Load Utility.
/// </summary>
public static class XMLUtility
{
    public static void SaveNavMap(string _filePath, int _rowCount, int _colCount)
    {
        if (string.IsNullOrEmpty(_filePath))
        {
            //UnityLogger.GetInstance().Log($"[XMLUtility] SaveNavMap #_path# is NULL");
            return;
        }

        SaveNavXML(_filePath, _rowCount, _colCount);
    }
    private static void SaveNavXML(string _filePath, int _rowCount, int _colCount)
    {
        MapManager.GetInstance().GetNavigationElements(out var _retNav);

        // 2D 배열을 List<List<NavigationElement>>로 변환
        NavigationElementListWrap dataWrapper = new NavigationElementListWrap();

        dataWrapper._row = _rowCount;
        dataWrapper._col = _colCount;

        foreach (var nav in _retNav)
        {
            dataWrapper.List.Add(nav.Value);
        }

        XmlSerializer serializer = new XmlSerializer(typeof(NavigationElementListWrap));
        using (StreamWriter writer = new StreamWriter(_filePath))
        {
            serializer.Serialize(writer, dataWrapper);
        }
    }
    public static void Serialize<T>(string _filePath, T _data)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(T));
        using (StreamWriter writer = new StreamWriter(_filePath))
        {
            serializer.Serialize(writer, _data);
        }
    }
    public static T Deserialize<T>(string _xmlData) where T : class
    { 
        try
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (StringReader reader = new StringReader(_xmlData))
            {
                return (T)serializer.Deserialize(reader);
            }
        }
        catch (Exception e)
        {
            UnityLogger.GetInstance().Log($"[XMLUtility] LoadXML Failed : {_xmlData} ");
            return null;
        }
    }
}