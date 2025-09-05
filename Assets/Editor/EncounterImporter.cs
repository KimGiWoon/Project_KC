#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq; // Linq 네임스페이스 추가

public static class EncounterImporter
{
    const string kSaveDir = "Assets/Resources/Chart/EncounterData"; // 런타임에서 Resources.Load로 쓸 경로

    [MenuItem("Tools/DB/Encounter/Import CSV to SOs")]
    public static void Import()
    {
        // CSV 선택
        string csvPath = EditorUtility.OpenFilePanel("Select Encounter CSV", Application.dataPath, "csv");
        if (string.IsNullOrEmpty(csvPath)) return;

        EnsureFolders(kSaveDir);

        var lines = File.ReadAllLines(csvPath, new UTF8Encoding(true));
        if (lines.Length <= 1) { Debug.LogWarning("CSV가 비었도다."); return; }

        int created = 0, updated = 0;

        AssetDatabase.StartAssetEditing();
        try
        {
            for (int i = 1; i < lines.Length; i++) // 0: 헤더
            {
                if (string.IsNullOrWhiteSpace(lines[i])) continue;

                Debug.Log($"[EncounterImporter] 줄 {i} 내용: {lines[i]}");

                // 쉼표(,)가 아닌 탭(\t)을 기준으로 데이터를 분리하도록 수정합니다.
                var cols = lines[i].Split('\t').ToList();

                for (int j = 0; j < cols.Count; j++)
                {
                    Debug.Log($"  └ 필드 {j}: '{cols[j]}'");
                }
                var row = new SDW.EncounterTable(cols.ToArray());   // ← 네 구조체 생성자 활용

                string id = row.EncounterID.ToString("D4");
                string assetPath = $"{kSaveDir}/Encounter_{id}.asset";

                var so = AssetDatabase.LoadAssetAtPath<EncounterDataSO>(assetPath);
                bool isNew = so == null;
                if (isNew)
                {
                    so = ScriptableObject.CreateInstance<EncounterDataSO>();
                    AssetDatabase.CreateAsset(so, assetPath);
                    created++;
                }
                else updated++;

                so.Apply(row);
                EditorUtility.SetDirty(so);

                if ((i & 31) == 0)
                    EditorUtility.DisplayProgressBar("Import Encounters", $"ID {id}", (float)i / (lines.Length - 1));
            }
        }
        finally
        {
            EditorUtility.ClearProgressBar();
            AssetDatabase.StopAssetEditing();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        Debug.Log($"Encounter 임포트 완료 — 생성 {created}, 갱신 {updated}");
    }

    static void EnsureFolders(string full)
    {
        var parts = full.Split('/');
        string cur = parts[0];
        for (int i = 1; i < parts.Length; i++)
        {
            string next = $"{cur}/{parts[i]}";
            if (!AssetDatabase.IsValidFolder(next)) AssetDatabase.CreateFolder(cur, parts[i]);
            cur = next;
        }
    }
}
#endif