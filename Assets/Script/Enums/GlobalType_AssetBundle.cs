using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AssetBundleType
{
    // 모델관련 에셋번들
    ModelCharacter,
    ModelSkin,
    ModelWeapon,

    // 맵 관련 에셋번들
    Map_Navigation, // 네비게이션 엘리먼트

    // UI 관련 에셋 번들
    UI_Main,
    UI_Scene,
    UI_Popup,

    // 이펙트 관련 에셋번들
    FX_Skill,
    FX_Particle,
    FX_Moving
}

