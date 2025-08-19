using System.Linq;
using UnityEngine;

namespace SDW
{
    public static class LoadCSV
    {
        public static string[] LoadFromCsv(string fileName, int skipLine = 3)
        {
            var csvFile = Resources.Load<TextAsset>($"CSVData/{fileName}");

            if (csvFile != null)
            {
                string[] lines = csvFile.text.Split('\n').Skip(skipLine).ToArray();
                return lines;
            }

            Debug.LogWarning($"CSV 파일을 찾지 못했습니다 : Resources/CSVData/{fileName}");
            return null;
        }
    }
}