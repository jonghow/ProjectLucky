using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GlobalGameDataSpace
{
    public static class Defines
    {
        public const float DefaultScaleX = 1; // 
        public const float DefaultScaleY = 0.95f; // 

        public const float DefaultHalfScaleX = DefaultScaleX * 0.5f; // 
        public const float DefaultHalfScaleY = DefaultScaleY * 0.5f; // 

        public const float GridPixelPerUnit = 0.25f; // 1Unit = 4Tile , 1Tile = 0.25Unit

        public const int NormalSingleGameEnemyAllowCount = 100; // �� �̱�, ��� ���ӿ��� ����� ������ ����. �Ѿ�� ���� ��.
        public const int NormalSingleGameSupplyMaxCount = 27; // �� �̱�, ��� ���ӿ��� ����� �α� ��.

        public const int DrawDiaPriceUncommon = 1; // � ��� ����
        public const int DrawDiaPriceHero = 1; // � ���� ����
        public const int DrawDiaPriceMyth = 2;// � ��ȭ ����

        public const int DrawDefaultGoldPrice = 20; // ��� �̱� ����

        public const float DefaultInitWaitTime = 3; // 3�� ��ٸ��� �������� ����
        public const float DefaultStageIntervalWaveTime = 20; // 20�ʰ� WaitTime
    }

    public static class BuildGridHelper
    {
        public static List<Vector2> GetBuildGrid(bool _ContainDiagonal)
        {
            List<Vector2> _ret = new List<Vector2>();

            // 0. ����Ʈ
            _ret.Add(new Vector2(0f, 0f));

            // 1. �»����
            _ret.Add(new Vector2(-1f, 0));
            _ret.Add(new Vector2(0, 1f));
            _ret.Add(new Vector2(1f, 0));
            _ret.Add(new Vector2(0,-1f));

            // 2. �»�,���,����,����
            if(_ContainDiagonal)
            {
                _ret.Add(new Vector2(-1f, 1f));
                _ret.Add(new Vector2(1f, 1f));
                _ret.Add(new Vector2(1f, -1f));
                _ret.Add(new Vector2(-1f, -1f));
            }

            return _ret;
        }

        public static Vector2Int ConvertDirToNavIndex(GridDirectionGroup _dirGroup)
        {
            Vector2Int _ret = new Vector2Int();

            switch (_dirGroup)
            {
                case GridDirectionGroup.O:
                    _ret = new Vector2Int(0, 0);
                    break;
                case GridDirectionGroup.L:
                    _ret = new Vector2Int(0, -1);
                    break;
                case GridDirectionGroup.T:
                    _ret = new Vector2Int(1, 0);
                    break;
                case GridDirectionGroup.R:
                    _ret = new Vector2Int(0, 1);
                    break;
                case GridDirectionGroup.B:
                    _ret = new Vector2Int(-1, 0);
                    break;
                case GridDirectionGroup.LT:
                    _ret = new Vector2Int(1, -1);
                    break;
                case GridDirectionGroup.RT:
                    _ret = new Vector2Int(1, 1);
                    break;
                case GridDirectionGroup.RB:
                    _ret = new Vector2Int(-1, 1);
                    break;
                case GridDirectionGroup.LB:
                    _ret = new Vector2Int(-1, -1);
                    break;
                default:
                    break;
            }

            return _ret;
        }
    }
}

