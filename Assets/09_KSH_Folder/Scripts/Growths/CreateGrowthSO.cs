#if UNITY_EDITOR
using SDW;
using UnityEditor;
using UnityEngine;

public class CreateGrowthSO : Editor
{
    [MenuItem("Tools/DataCSV/Growths/Create GrowthSO")]

    public static void ImportGrowths()
    {
        string[] growthDataLines = HandleCSV.LoadFromCsv("Contents/ChefsEchoData");
        var growthDataList = HandleCSV.ReadDataFromLines<ChefsEchoDataFileData>(growthDataLines);

        foreach (var data in growthDataList)
        {
            string soFolder = "Assets/09_KSH_Folder/Scripts/Growths/GrowthSOData";
            
            string soPath = $"{soFolder}/{data.EnName}_Dara.asset";
            
            GrowthDatas growthDatas = AssetDatabase.LoadAssetAtPath<GrowthDatas>(soPath);
            if (growthDatas == null)
            {
                growthDatas = ScriptableObject.CreateInstance<GrowthDatas>();
                AssetDatabase.CreateAsset(growthDatas, soPath);
                Debug.Log($"Create {data.EnName} - {soPath}");
            }

            growthDatas.DataApply(data);
            EditorUtility.SetDirty(growthDatas);
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
#endif
