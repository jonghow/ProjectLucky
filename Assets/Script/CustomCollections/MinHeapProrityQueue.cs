using Cysharp.Threading.Tasks;
using DTR_Extension;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using System;
using System.Linq;

public class MinHeapPriorityQueue<T>
{
    private List<(T Element, int Priority)> heap = new List<(T, int)>();

    // ť�� ��Ҹ� �߰�
    public void Enqueue(T element, int priority)
    {
        heap.Add((element, priority));
        HeapifyUp(heap.Count - 1);
    }

    // ť���� �켱������ ���� ���� ��Ҹ� ������
    public T Dequeue()
    {
        if (heap.Count == 0)
            throw new InvalidOperationException("Queue is empty.");

        // ���� �켱������ ���� ��Ҹ� ������
        var root = heap[0];
        var lastIndex = heap.Count - 1;
        heap[0] = heap[lastIndex];
        heap.RemoveAt(lastIndex);

        HeapifyDown(0);
        return root.Element;
    }

    // ť�� ������� Ȯ��
    public bool IsEmpty() => heap.Count == 0;

    // ���� ���� �ø��� ���� (�켱������ �� ���� ���ϼ��� ���� �ö󰡵���)
    private void HeapifyUp(int index)
    {
        while (index > 0)
        {
            int parentIndex = (index - 1) / 2;
            if (heap[index].Priority >= heap[parentIndex].Priority)
                break;

            // �θ�� �ڽ� ��� ��ȯ
            Swap(index, parentIndex);
            index = parentIndex;
        }
    }

    // ���� �Ʒ��� �������� ����
    private void HeapifyDown(int index)
    {
        int leftChildIndex = 2 * index + 1;
        int rightChildIndex = 2 * index + 2;
        int smallest = index;

        if (leftChildIndex < heap.Count && heap[leftChildIndex].Priority < heap[smallest].Priority)
            smallest = leftChildIndex;

        if (rightChildIndex < heap.Count && heap[rightChildIndex].Priority < heap[smallest].Priority)
            smallest = rightChildIndex;

        if (smallest != index)
        {
            Swap(index, smallest);
            HeapifyDown(smallest);
        }
    }

    // �� ��Ҹ� ��ȯ
    private void Swap(int i, int j)
    {
        var temp = heap[i];
        heap[i] = heap[j];
        heap[j] = temp;
    }

    public void Clear()
    {
        heap.Clear();
    }
}
