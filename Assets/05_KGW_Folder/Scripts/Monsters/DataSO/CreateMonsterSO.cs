#if UNITY_EDITOR
using SDW;
using UnityEditor;
using UnityEngine;

public class CreateMonsterSO : Editor
{
    [MenuItem("Tools/DataCSV/Monsters/CreateMonsterSO")]

    public static void ImportMonsters()
    {
        // CSV 데이터 읽기

        // 몬스터 데이터
        string[] dataLines = HandleCSV.LoadFromCsv("Monster/MonsterData");
        var dataList = HandleCSV.ReadDataFromLines<MonsterDataFileData>(dataLines);

        // 몬스터 스탯 데이터
        string[] statLines = HandleCSV.LoadFromCsv("Monster/MonsterStat");
        var statList = HandleCSV.ReadDataFromLines<MonsterStatFileData>(statLines);

        // 캐릭터별 ScriptableObject 생성
        foreach (var data in dataList)
        {
            var statData = statList.Find(s => s.MomID == data.MonId);

            // ScriptableObject 저장 경로
            string soFolder = "Assets/05_KGW_Folder/Prefabs/01_Monsters/MonstersSOData";
            
            // 파일 이름 설정
            string soPath = $"{soFolder}/{data.MonEnName}_Data.asset";

            // ScriptableObject 생성/로드
            MonsterDataSO so = AssetDatabase.LoadAssetAtPath<MonsterDataSO>(soPath);
            if (so == null)
            {
                so = ScriptableObject.CreateInstance<MonsterDataSO>();
                AssetDatabase.CreateAsset(so, soPath);
                Debug.Log($"[SO 생성] {data.MonEnName} → {soPath}");
            }

            // 데이터 덮어쓰기
            so.DataApply(data, statData);
            EditorUtility.SetDirty(so);

        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
#endif