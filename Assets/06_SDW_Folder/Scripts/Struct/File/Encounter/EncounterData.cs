using UnityEngine;
using System.Collections.Generic;
using SDW; 

[CreateAssetMenu(fileName = "Encounter_", menuName = "CJH/Encounter Data", order = 0)]
public class EncounterData : ScriptableObject
{
    [Header("기본 정보")]
    public int EncounterID;
    public int EncounterStage;
    [TextArea(3, 5)]
    public string EncounterText;

    [Header("타입 정보")]
    public EncounterResultType ResultType;
    public EncounterType EnCounterType;
    public RandomType RandomType;

    [Header("선택지 정보")]
    public int ChoiceCount;
    public List<string> ChoiceTexts;
    public List<string> EncounterExitText;

    [Header("결과 상세")]
    public bool ResultOwned;
    public bool CanGetMoney;
    public int StartID;
    public int EndID;
    public int ResultMinCount;
    public int ResultMaxCount;
    public int ResultMoney;
    public int ResultNumber;
    public int ResultChoice;
}