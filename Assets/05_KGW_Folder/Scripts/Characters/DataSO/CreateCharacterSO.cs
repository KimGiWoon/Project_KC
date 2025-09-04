#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using SDW;

public class CreateCharacterSO : Editor
{
    [MenuItem("Tools/DataCSV/Characters/CreateCharacterSO")]

    public static void ImportCharacters()
    {
        // CSV 데이터 읽기

        // 캐릭터 베이스 데이터
        string[] baseLines = HandleCSV.LoadFromCsv("Character/CharacterBaseData");
        var baseList = HandleCSV.ReadDataFromLines<CharacterBaseDataFileData>(baseLines);

        // 캐릭터 타입 데이터
        string[] typeLines = HandleCSV.LoadFromCsv("Character/CharacterType");
        var typeList = HandleCSV.ReadDataFromLines<CharacterTypeFileData>(typeLines);

        // 캐릭터별 ScriptableObject 생성
        foreach (var baseData in baseList)
        {
            var typeData = typeList.Find(t => t.ChaRole == baseData.ChaRole);

            // ScriptableObject 저장 경로
            string soFolder = "Assets/05_KGW_Folder/Prefabs/00_Characters/CharactersSOData";

            // 파일 이름 설정
            string soPath = $"{soFolder}/{baseData.ChaEnName}_Data.asset";

            // ScriptableObject 생성/로드
            CharacterDataSO so = AssetDatabase.LoadAssetAtPath<CharacterDataSO>(soPath);
            if (so == null)
            {
                so = ScriptableObject.CreateInstance<CharacterDataSO>();
                AssetDatabase.CreateAsset(so, soPath);
                Debug.Log($"[SO 생성] {baseData.ChaEnName} → {soPath}");
            }

            // 데이터 덮어쓰기
            so.DataApply(baseData, typeData);
            EditorUtility.SetDirty(so);

        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
#endif