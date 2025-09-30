namespace SDW
{
    public enum AudioClipName
    {
        // 배경 오디오의 클립
        MemoryHSR,
        MemoryBCA,
        MemoryGSE,
        MemorySIL,

        // 버튼 클릭의 오디오 클립
        UIButton_1,
        UIButton_2,
        ToggleUI,

        // 스크롤 오디오 클립
        ScrollView_1,
        ScrollView_2,

        // 팝업&불가능 효과음 오디오 클립
        LevelUp,
        LevelUpCheer,
        UIPopUp_1,
        UIPopUp_2,
        UIPopUp_3,

        // 상호작용에 대한 오디오 클립
        BattleVictory,
        BattleLose,
        BuySuccess,

        // 캐릭터의 패시브 스킬의 오디오 클립
        MeleePassiveSkill,
        DebufferPassiveSkill,
        HealerPassiveSkill,
        RangedPassiveSkill,

        // 캐릭터의 액티브 스킬의 오디오 클립
        BCAActiveSkill,
        SILActiveSkill,
        BWActiveSkill,
        HSRActiveSkill,
        GSEActiveSkill,

        // 엘리트 몬스터의 스킬 오디오 클립
        ShadowScarecrowSkill,
        CorruptedFlowerSkill,
        RockGolemSkill,

        // 보스 몬스터의 스킬 오디오 클립
        IronBullSkill_1,
        IronBullSkill_2,
        GiantBearSkill_1,
        GiantBearSkill_2,
        BladeMonkeySkill_1,
        BladeMonkeySkill_2,

        // 캐릭터와 몬스터의 피격 오디오 클립
        HitSound_1,
        HitSound_2,
        HitSound_3,
        HitSound_4,
        HitSound_5,

        // 음식 섭취에 대한 오디오 클립 이름
        Buff,
        Revive,
        MP_Heal,
        ShieldCreate,
        ShieldImpact,
        HP_Heal,

        // 게임 로그라이크 오디오 클립 이름
        NagativeEncounter,
        PositiveEncounter,
        NeutralEncounter,
        MapMoving,

        //# 로그라이크 BGM
        Stage1BGM,
        Stage2BGM,
        Stage3BGM

        // TODO : 스토리 사운드는 미정 -> 정해지면 추가 예정
        // 게임 스토리 오디오 클립 이름
    }
}