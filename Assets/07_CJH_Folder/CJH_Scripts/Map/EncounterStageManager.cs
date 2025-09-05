using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace CJH
{
    public class EncounterStageManager : MonoBehaviour
    {
        public static EncounterStageManager Instance;

        // 모든 EncounterDataSO 파일을 여기에 담아둡니다.
        private List<EncounterDataSO> allEncounterSOs;

        // 게임의 현재 스테이지를 관리합니다.
        public int currentStage = 1;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                LoadEncounterData();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Resources 폴더에서 모든 EncounterDataSO 애셋을 불러옵니다.
        /// </summary>
        private void LoadEncounterData()
        {
            // CSV 임포터가 생성한 SO 파일들이 있는 경로입니다.
            allEncounterSOs = Resources.LoadAll<EncounterDataSO>("Chart/EncounterData").ToList();
            Debug.Log($"[EncounterStageManager] 총 {allEncounterSOs.Count}개의 인카운터 SO 파일을 불러왔습니다.");
        }

        /// <summary>
        /// 지정된 스테이지에 맞는 인카운터 중 하나를 무작위로 반환합니다.
        /// </summary>
        public EncounterDataSO GetRandomEncounter(int stage)
        {
            // 모든 SO 데이터 중에서 현재 스테이지와 일치하는 것들만 추립니다.
            List<EncounterDataSO> stageEncounters = allEncounterSOs.Where(so => so.Row.EncounterStage == stage).ToList();

            if (stageEncounters.Count == 0)
            {
                Debug.LogWarning($"{stage} 스테이지에 해당하는 인카운터를 찾을 수 없습니다!");
                return null;
            }

            // 찾은 인카운터 중에서 무작위로 하나를 뽑아 반환합니다.
            int randomIndex = Random.Range(0, stageEncounters.Count);
            return stageEncounters[randomIndex];
        }
    }
}