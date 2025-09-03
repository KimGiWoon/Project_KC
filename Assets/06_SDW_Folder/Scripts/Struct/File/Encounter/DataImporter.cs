using UnityEngine;
using UnityEditor;
using System.IO;
using System.Reflection;
using SDW;

public class DataImporter : EditorWindow
{
    [MenuItem("Tools/CJH/Smart Data Importer")]
    public static void ShowWindow()
    {
        GetWindow<DataImporter>("Smart Data Importer");
    }

    private string tsvFilePath = "Assets/Data/EncounterTable.tsv";

    void OnGUI()
    {
        GUILayout.Label("Smart Data TSV Importer", EditorStyles.boldLabel);
        tsvFilePath = EditorGUILayout.TextField("TSV File Path", tsvFilePath);

        if (GUILayout.Button("Import Encounter Data"))
        {
            // 제네릭 메소드를 호출하여 어떤 타입의 데이터를 임포트할지 지정
            ImportData<EncounterData, EncounterTable>("Encounter");
        }

    }

    /// <summary>
    /// TSV 파일로부터 데이터를 읽어 ScriptableObject 에셋을 생성하는 제네릭 메소드
    /// </summary>
    /// <typeparam name="TSO">생성할 ScriptableObject 타입</typeparam>
    /// <typeparam name="TStruct">데이터를 파싱할 구조체 타입</typeparam>
    /// <param name="assetPrefix">생성될 에셋 파일의 접두사 (예: "Encounter")</param>
    private void ImportData<TSO, TStruct>(string assetPrefix) where TSO : ScriptableObject
    {
        if (!File.Exists(tsvFilePath))
        {
            Debug.LogError($"파일을 찾을 수 없습니다: {tsvFilePath}");
            return;
        }

        string outputFolder = $"Assets/Resources/Data/{assetPrefix}Data";
        Directory.CreateDirectory(outputFolder);

        string[] lines = File.ReadAllLines(tsvFilePath);

        for (int i = 1; i < lines.Length; i++) // 헤더는 건너뛰기
        {
            string[] fields = lines[i].Split('\t');
            if (fields.Length == 0 || string.IsNullOrEmpty(fields[0])) continue; // 빈 라인 건너뛰기

            // 1. 구조체 인스턴스를 생성하고 TSV 데이터로 변환
            object structInstance = System.Activator.CreateInstance(typeof(TStruct), new object[] { fields });

            // 2. ScriptableObject 인스턴스를 생성
            TSO soInstance = CreateInstance<TSO>();

            // 3. 리플렉션을 사용하여 구조체의 데이터를 SO로 자동 복사
            CopyFields(structInstance, soInstance);

            // 4. ID 필드를 찾아 파일 이름을 결정
            FieldInfo idField = typeof(TStruct).GetField($"{assetPrefix}ID");
            if (idField == null)
            {
                Debug.LogError($"ID 필드('{assetPrefix}ID')를 {typeof(TStruct).Name}에서 찾을 수 없습니다.");
                continue;
            }
            int id = (int)idField.GetValue(structInstance);

            // 5. 에셋을 생성하고 저장
            string assetPath = $"{outputFolder}/{assetPrefix}_{id}.asset";
            AssetDatabase.CreateAsset(soInstance, assetPath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"{assetPrefix} 데이터 임포트 완료! 총 {lines.Length - 1}개의 에셋이 처리되었습니다.");
    }

    /// <summary>
    /// 리플렉션을 사용하여 한 객체의 필드 값을 다른 객체로 복사합니다.
    /// </summary>
    /// <param name="source">원본 객체 (예: EncounterTable 인스턴스)</param>
    /// <param name="destination">대상 객체 (예: EncounterData 인스턴스)</param>
    private static void CopyFields(object source, object destination)
    {
        // public 필드만
        FieldInfo[] sourceFields = source.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
        FieldInfo[] destinationFields = destination.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);

        foreach (var sourceField in sourceFields)
        {
            // 이름이 같은 필드를 대상 객체에서 할당
            FieldInfo destinationField = System.Array.Find(destinationFields, f => f.Name == sourceField.Name);

            // 필드를 찾았고, 타입이 서로 호환된다면 값을 복사
            if (destinationField != null && destinationField.FieldType.IsAssignableFrom(sourceField.FieldType))
            {
                destinationField.SetValue(destination, sourceField.GetValue(source));
            }
        }
    }
}