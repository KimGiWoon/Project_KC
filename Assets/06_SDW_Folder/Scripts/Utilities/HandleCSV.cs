using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SDW
{
    public static class HandleCSV
    {
        /// <summary>
        /// 읽어들인 CSV 파일의 지정된 줄 이후의 데이터를 배열로 반환
        /// </summary>
        /// <param name="fileName">리소스 경로상의 CSV 파일 이름</param>
        /// <param name="skipLine">건너뛰어야 할 초기 줄 번호 (기본값 - 3)</param>
        /// <returns>CSV 데이터 줄 배열, 파일을 찾지 못한 경우 null 반환</returns>
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

        public static List<T> ReadDataFromLines<T>(string[] lines) where T : struct
        {
            var dataList = new List<T>();

            foreach (string line in lines)
            {
                string[] fields = line.Split(',');

                // 생성자(string[] fields)를 이용해 객체 생성
                var data = (T)Activator.CreateInstance(typeof(T), new object[] { fields });
                dataList.Add(data);
            }

            return dataList;
        }
    }
}