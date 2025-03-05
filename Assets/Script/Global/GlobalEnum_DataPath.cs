using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GlobalGameDataSpace
{
    /// <summary>
    /// ĳ���Ϳ� ���� Animation Data�� �ε��մϴ�. 
    /// �ε��ؾ��� ĳ���� AnimData�� �Ʒ� �߰� �մϴ�.
    /// * ��ҹ��ڴ� �������մϴ�. ��� �ҹ��ڷ� ó���� ��
    /// </summary>
    public enum CHARACTER_ACT_DATA
    {
        Novice,
        RiceCakeMercenary,
        ChiliPepperPasteMercenary,
        GreenOnionMercenary,

        // =========== ĳ���� ���� ==================

        EggMercenary,
        FishCakeMercenary,
        KonjacMercenary,
        SeaCrabMercenary,

        // =========================================

        // =========== ĳ���� ���� ==================

        WaterMercenary,
        FireMercenary,

        // =========================================

        // =========== ĳ���� ���� ==================
        CeleryMercenary,
        SalmonMercenary,
        CabbageMercenary,
        CornMercenary,
        MeatMercenary,
        MushroomMercenary,
        NapaCabbageMercenary,

        // =========================================

        RedRiceCakeMercenary,

        // =========== ĳ���� ���� ==================

        BoiledCrabMercenary,
        RoastedEggMercenary,
        ScallionMercenary,
        BulgogiMercenary,
        SmokedSalmonMercenary,

        // =========================================

        // =========== ĳ���� ���� ==================
        TteokbokkiMercenary,
        // =========================================


        // =========== ĳ���� ���� ==================
        FishCakeSoupMercenary,
        // =========================================

        MAX
    }
    public enum ENEMY_ACT_DATA
    { 
        None = 1000,
        SpoiledSlime ,
        SpoiledSlime2,

        MAX
    }

    public enum MEALFACTORY_ACT_DATA
    {
        None,
        CookMealFactory,
        CombineMealFactory,
        StoreMealFactory,
        MAX
    }
/// <summary>
/// ��ǲ �Ŵ����� State Ÿ�� �Դϴ�. 
/// ���¿� ���� � ��ǲ�� ������ ������, ������Ʈ �������� ���� �����Դϴ�. 
/// UI ��ǲ ���� �ΰ��� ��ǲ(Ű �Է�, ������Ʈ ���콺 �Է�) �����մϴ�.
/// </summary>
    public enum InputState
    {
        None,
        NormalState,
        StructureBuildState, // �ǹ� ���� ���� => �ǹ� ���� ��
        SelectEntityState, // �뺴 ����(Ŭ��)�� ���� => ���� �����ֱ��
        SelectStructureState, // �ǹ� ����(Ŭ��)�� ���� => ���� �����ֱ��
        SelectCookCardState, // �丮�� ī�带 ������ ���� => ��� �ܿ� ������ üũ

        SelectSpawnCardState, // ���� ��ƼƼ�� ������ ���� => ��� ���鿡 �������� üũ

        UIOpenState, // UI�� ���� ����, �̶��� ��ǲ �ý����� �ȳ־���ؼ� State�� �����Ͽ� UI ���¿����� Ű �Է��� �����ϵ��� ó�� �� ����
        StateMax
    }
    /// <summary>
    /// ������ Ÿ�� �Դϴ�.
    /// � Ÿ���� ������ ������ �����մϴ�.
    /// </summary>
    public enum ItemType
    {
        Equip,
        Consumable,
        Material
    }
    public enum DB_Enum_String
    {
        Common,
        Max
    }
    public enum DB_MealRecipe
    {
        Info,
        Max
    }
    public enum DB_MealKit
    {
        Info,
        Max
    }
}
