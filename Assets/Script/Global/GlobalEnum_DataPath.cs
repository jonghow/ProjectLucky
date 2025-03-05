using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GlobalGameDataSpace
{
    /// <summary>
    /// 캐릭터에 관한 Animation Data를 로드합니다. 
    /// 로드해야할 캐릭터 AnimData는 아래 추가 합니다.
    /// * 대소문자는 구별안합니다. 모두 소문자로 처리할 것
    /// </summary>
    public enum CHARACTER_ACT_DATA
    {
        Novice,
        RiceCakeMercenary,
        ChiliPepperPasteMercenary,
        GreenOnionMercenary,

        // =========== 캐릭터 없음 ==================

        EggMercenary,
        FishCakeMercenary,
        KonjacMercenary,
        SeaCrabMercenary,

        // =========================================

        // =========== 캐릭터 있음 ==================

        WaterMercenary,
        FireMercenary,

        // =========================================

        // =========== 캐릭터 없음 ==================
        CeleryMercenary,
        SalmonMercenary,
        CabbageMercenary,
        CornMercenary,
        MeatMercenary,
        MushroomMercenary,
        NapaCabbageMercenary,

        // =========================================

        RedRiceCakeMercenary,

        // =========== 캐릭터 없음 ==================

        BoiledCrabMercenary,
        RoastedEggMercenary,
        ScallionMercenary,
        BulgogiMercenary,
        SmokedSalmonMercenary,

        // =========================================

        // =========== 캐릭터 있음 ==================
        TteokbokkiMercenary,
        // =========================================


        // =========== 캐릭터 없음 ==================
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
/// 인풋 매니저의 State 타입 입니다. 
/// 상태에 따라 어떤 인풋을 감지할 것인지, 컴포넌트 패턴으로 넣을 예정입니다. 
/// UI 인풋 제외 인게임 인풋(키 입력, 오브젝트 마우스 입력) 관리합니다.
/// </summary>
    public enum InputState
    {
        None,
        NormalState,
        StructureBuildState, // 건물 짓는 상태 => 건물 짓는 용
        SelectEntityState, // 용병 선택(클릭)한 상태 => 정보 보여주기용
        SelectStructureState, // 건물 선택(클릭)한 상태 => 정보 보여주기용
        SelectCookCardState, // 요리할 카드를 선택한 상태 => 어느 솥에 부을지 체크

        SelectSpawnCardState, // 직접 엔티티를 생성할 상태 => 어느 지면에 생성할지 체크

        UIOpenState, // UI가 열린 상태, 이때는 인풋 시스템을 안넣어야해서 State를 변경하여 UI 상태에서만 키 입력이 가능하도록 처리 할 예정
        StateMax
    }
    /// <summary>
    /// 아이템 타입 입니다.
    /// 어떤 타입의 아이템 인지를 결정합니다.
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
