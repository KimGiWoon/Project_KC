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

        /// <summary>
        /// 지정된 CSV 파일의 줄들을 읽어들여 지정된 구분자로 분리된 데이터를 구조체 배열로 변환
        /// </summary>
        /// <param name="lines">CSV 파일의 각 줄을 담은 문자열 배열</param>
        /// <param name="separator">필드를 구분하는 구분자 (기본값은 탭 문자 '\t')</param>
        /// <typeparam name="T">변환하고자 하는 데이터 구조체 타입으로, 기본 생성자를 가져야 함</typeparam>
        /// <returns>구조체 배열, 입력이 유효하지 않거나 타입 생성에 실패한 경우 빈 리스트를 반환</returns>
        public static List<T> ReadDataFromLines<T>(string[] lines, char separator = '\t') where T : struct
        {
            var dataList = new List<T>();

            foreach (string line in lines)
            {
                // string[] fields = line.Split(',');
                string[] fields = line.Split(separator);

                // 생성자(string[] fields)를 이용해 객체 생성
                var data = (T)Activator.CreateInstance(typeof(T), new object[] { fields });
                dataList.Add(data);
            }

            return dataList;
        }
    }
}