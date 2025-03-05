using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generic Extension stack
/// </summary>
public static class GenericCollectionExtension_Stack
{
    /// <summary>
    /// stack�� null�̰ų� ����ִ��� Ȯ��
    /// </summary>
    public static bool IsNullOrEmpty<T>(this Stack<T> stack)
    {
        return stack == null || stack.Count == 0;
    }

    public static bool IsNullOrEmpty(this Stack stack)
    {
        return stack == null || stack.Count == 0;
    }
    public static bool IsNull<T>(this Stack<T> stack)
    {
        return stack == null;
    }

    public static bool IsNull(this Stack stack)
    {
        return stack == null;
    }

    public static bool IsEmpty<T>(this Stack<T> stack)
    {
        return stack.Count == 0;
    }

    public static bool IsEmpty(this Stack stack)
    {
        return stack.Count == 0;
    }
}

