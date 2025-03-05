using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using GlobalGameDataSpace;

public static partial class MathUtility
{
    /// <summary>
    /// 제한 범위 안에 들어오는지 판단합니다. 
    /// 범위를 벗어나면 True, 포함되면 False를 반환합니다.
    /// </summary>
    /// <param name="a">A의 좌표</param>
    /// <param name="b">B의 좌표</param>
    /// <param name="_restrict">제한 범위</param>
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
        // 1. 회전된 점을 사각형의 로컬 좌표계로 변환 (반대 방향 회전 적용)
        float radians = -_rotateAngle * Mathf.Deg2Rad;
        float cos = Mathf.Cos(radians);
        float sin = Mathf.Sin(radians);

        // 점을 사각형 중심 기준으로 이동 후 회전 변환
        Vector2 localPoint = _point - _centerPos;
        float rotatedX = localPoint.x * cos - localPoint.y * sin;
        float rotatedY = localPoint.x * sin + localPoint.y * cos;

        // 2. 회전된 점이 AABB(축 정렬 사각형) 내부에 있는지 확인
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
                // 둔각이라면 외곽
            }
        }
        // 각 방향의 방향벡터 생성
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