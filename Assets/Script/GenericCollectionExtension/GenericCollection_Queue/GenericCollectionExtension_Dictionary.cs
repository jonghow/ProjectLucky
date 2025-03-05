using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generic Extension Dictionary
/// </summary>
public static class GenericCollectionExtension_Dictionary
{
    /// <summary>
    /// Dictionary가 null이거나 비어있는지 확인
    /// </summary>
    public static bool IsNullOrEmpty<K,V>(this Dictionary<K, V> dictionary)
    {
        return dictionary == null || dictionary.Count == 0;
    }
    public static bool IsNull<K, V>(this Dictionary<K, V> dictionary)
    {
        return dictionary == null;
    }
    public static bool IsEmpty<K, V>(this Dictionary<K, V> dictionary)
    {
        return dictionary.Count == 0;
    }
}

