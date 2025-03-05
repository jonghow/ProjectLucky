using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using GlobalGameDataSpace;

public static partial class MathUtility
{
    /// <summary>
    /// ���� ���� �ȿ� �������� �Ǵ��մϴ�. 
    /// ������ ����� True, ���ԵǸ� False�� ��ȯ�մϴ�.
    /// </summary>
    /// <param name="a">A�� ��ǥ</param>
    /// <param name="b">B�� ��ǥ</param>
    /// <param name="_restrict">���� ����</param>
    /// <returns></returns>
    public static bool CheckOverV2SqrMagnitudeDistance(Vector2 a, Vector2 b, float _restrict)
    {
        return Vector2.SqrMagnitude(b - a) > (_restrict * _restrict);
    }
    public static bool CheckOverV3MagnitudeDistance(Vector3 a, Vector3 b, float _restrict)
    {
        return Vector3.SqrMagnitude(b - a) > (_restrict * _restrict);
    }

    public static bool CheckInRectOBB(Vector2 _point, Vector2 _centerPos, Vector2 _rectSize, float _rotateAngle)
    {
        // 1. ȸ���� ���� �簢���� ���� ��ǥ��� ��ȯ (�ݴ� ���� ȸ�� ����)
        float radians = -_rotateAngle * Mathf.Deg2Rad;
        float cos = Mathf.Cos(radians);
        float sin = Mathf.Sin(radians);

        // ���� �簢�� �߽� �������� �̵� �� ȸ�� ��ȯ
        Vector2 localPoint = _point - _centerPos;
        float rotatedX = localPoint.x * cos - localPoint.y * sin;
        float rotatedY = localPoint.x * sin + localPoint.y * cos;

        // 2. ȸ���� ���� AABB(�� ���� �簢��) ���ο� �ִ��� Ȯ��
        float halfWidth = _rectSize.x * 0.5f;
        float halfHeight = _rectSize.y * 0.5f;

        return (rotatedX >= -halfWidth && rotatedX <= halfWidth &&
                rotatedY >= -halfHeight && rotatedY <= halfHeight);
    }

    public static bool CheckInVertice(Vector2 _point, List<Vector2> _vertices)
    {
        for (int i = 0; i < _vertices.Count; i++)
        {
            if (i + 1 == _vertices.Count)
            {
                Vector2 _dir = (_vertices[0] - _vertices[_vertices.Count-1]).normalized;
                Vector2 _vertexToPoint = (_point - _vertices[_vertices.Count - 1]).normalized;

                float angle = Mathf.Acos(Vector3.Dot(_dir, _vertexToPoint)) * Mathf.Rad2Deg;

                if (angle > 90)
                    return false;
            }
            else
            {
                Vector2 _dir = (_vertices[i + 1] - _vertices[i]).normalized;
                Vector2 _vertexToPoint = (_point - _vertices[i]).normalized;

                float angle = Mathf.Acos(Vector3.Dot(_dir, _vertexToPoint)) * Mathf.Rad2Deg;

                if (angle >= 90)
                    return false;
                // �а��̶�� �ܰ�
            }
        }
        // �� ������ ���⺤�� ����
        return true;
    }

    public static bool IsPointInAABB(Vector2 point, Vector2 rectCenter, Vector2 rectSize)
    {
        float left = rectCenter.x - rectSize.x;
        float right = rectCenter.x + rectSize.x;
        float top = rectCenter.y + rectSize.y;
        float bottom = rectCenter.y - rectSize.y;

        return (point.x >= left && point.x <= right && point.y >= bottom && point.y <= top);
    }


    public static void GetNavigationVertice(Vector2 _point, out List<Vector2> _lt_vertice)
    {
        _lt_vertice = new List<Vector2>();
        _lt_vertice.Add(new Vector2(_point.x - Defines.DefaultHalfScaleX, _point.y + Defines.DefaultHalfScaleY)); // LT
        _lt_vertice.Add(new Vector2(_point.x + Defines.DefaultHalfScaleX, _point.y + Defines.DefaultHalfScaleY)); // RT
        _lt_vertice.Add(new Vector2(_point.x + Defines.DefaultHalfScaleX, _point.y - Defines.DefaultHalfScaleY)); // RB
        _lt_vertice.Add(new Vector2(_point.x - Defines.DefaultHalfScaleX, _point.y - Defines.DefaultHalfScaleY)); // LB
    }
}