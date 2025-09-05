using UnityEngine;

[CreateAssetMenu(menuName = "SDW/Encounter Data", fileName = "Encounter")]
public class EncounterDataSO : ScriptableObject
{
    public SDW.EncounterTable Row;   // CSV 한 줄을 그대로 보관

    public void Apply(SDW.EncounterTable t)
    {
        Row = t;                     // 한 곳에서 갱신
    }
}