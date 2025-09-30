namespace SDW
{
    public enum AudioType
    {
        // 게임 UI 오디오
        Background,
        Click,
        Scroll,
        PopUp,

        // 게임 상호작용 오디오
        Win,
        Lose,
        Buy,
        Impossibility,

        // 게임 배틀 오디오
        ChaSkill,
        MonSkill,
        Food,
        Hit,

        // 게임 로그라이크 오디오
        Map_Moving,
        Good_Encounter,
        Nautral_Encounter,
        Bad_Encounter

        // TODO : 스토리 사운드는 미정 -> 정해지면 추가 예정
        // 게임 스토리 오디오
    }
}