#if UNITY_EDITOR
using SDW;
using UnityEditor;
using UnityEngine;

public class CreateMonsterSkillSO : Editor
{
    [MenuItem("Tools/DataCSV/Skills/CreateMonsterSkillSO")]

    public static void ImportMonsterSkills()
    {
        // CSV 데이터 읽기

        // 캐릭터의 스킬 데이터
        string[] skillDataLines = HandleCSV.LoadFromCsv("Monster/MonsterSkill");
        var skillDataList = HandleCSV.ReadDataFromLines<MonsterSkillFileData>(skillDataLines);

        // 캐릭터 스킬의 ScriptableObject 생성
        foreach (var data in skillDataList)
        {
            // ScriptableObject 저장 경로
            string soFolder = "Assets/05_KGW_Folder/Prefabs/03_Skills/MonsterSkills/SkillsSOData";

            // 파일 이름 설정
            string soPath = $"{soFolder}/{data.MonSkillEnName}_Data.asset";

            // ScriptableObject 생성/로드
            MonsterSkillDataSO so = AssetDatabase.LoadAssetAtPath<MonsterSkillDataSO>(soPath);
            if (so == null)
            {
                so = ScriptableObject.CreateInstance<MonsterSkillDataSO>();
                AssetDatabase.CreateAsset(so, soPath);
                Debug.Log($"[SO 생성] {data.MonSkillEnName} → {soPath}");
            }

            // 데이터 덮어쓰기
            so.DataApply(data);
            EditorUtility.SetDirty(so);
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
#endif