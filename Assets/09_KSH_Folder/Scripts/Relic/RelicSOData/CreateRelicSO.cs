#if UNITY_EDITOR
using SDW;
using UnityEditor;
using UnityEngine;

public class CreateRelicSO : Editor
{
    [MenuItem("Tools/DataCSV/Relics/Create RelicSO")]

    public static void ImportRelics()
    {
        string[] relicDataLines = HandleCSV.LoadFromCsv("Relic/RelicData");
        var relicDataList = HandleCSV.ReadDataFromLines<RelicDataFileData>(relicDataLines);

        foreach (var data in relicDataList)
        {
            string soFolder = "Assets/09_KSH_Folder/Scripts/Relic/RelicSOData";

            string soPath = $"{soFolder}/{data.RelicEnName}_Data.asset";
            
            RelicDatas relicDatas = AssetDatabase.LoadAssetAtPath<RelicDatas>(soPath);
            if (relicDatas == null)
            {
                relicDatas = ScriptableObject.CreateInstance<RelicDatas>();
                AssetDatabase.CreateAsset(relicDatas, soPath);
                Debug.Log($"Create {data.RelicEnName} - {soPath}");
            }
            
            relicDatas.DataApply(data);
            EditorUtility.SetDirty(relicDatas);
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
#endif
