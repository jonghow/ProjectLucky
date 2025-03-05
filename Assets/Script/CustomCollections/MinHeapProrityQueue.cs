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

    // 큐에 요소를 추가
    public void Enqueue(T element, int priority)
    {
        heap.Add((element, priority));
        HeapifyUp(heap.Count - 1);
    }

    // 큐에서 우선순위가 가장 높은 요소를 꺼내기
    public T Dequeue()
    {
        if (heap.Count == 0)
            throw new InvalidOperationException("Queue is empty.");

        // 가장 우선순위가 높은 요소를 꺼내기
        var root = heap[0];
        var lastIndex = heap.Count - 1;
        heap[0] = heap[lastIndex];
        heap.RemoveAt(lastIndex);

        HeapifyDown(0);
        return root.Element;
    }

    // 큐가 비었는지 확인
    public bool IsEmpty() => heap.Count == 0;

    // 힙을 위로 올리며 정렬 (우선순위가 더 낮은 값일수록 위로 올라가도록)
    private void HeapifyUp(int index)
    {
        while (index > 0)
        {
            int parentIndex = (index - 1) / 2;
            if (heap[index].Priority >= heap[parentIndex].Priority)
                break;

            // 부모와 자식 노드 교환
            Swap(index, parentIndex);
            index = parentIndex;
        }
    }

    // 힙을 아래로 내려가며 정렬
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

    // 두 요소를 교환
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
