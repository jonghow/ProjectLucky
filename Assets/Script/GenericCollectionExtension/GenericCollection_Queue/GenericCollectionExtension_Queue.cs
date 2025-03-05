using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

/// <summary>
/// Generic Extension Queue
/// </summary>
public static class GenericCollectionExtension_Queue 
{
    /// <summary>
    /// Queue�� null�̰ų� ����ִ��� Ȯ��
    /// </summary>
    public static bool IsNullOrEmpty<T>(this Queue<T> queue)
    {
        return queue == null || queue.Count == 0;
    }
    public static bool IsNullOrEmpty(this Queue queue)
    {
        return queue == null || queue.Count == 0;
    }
}

