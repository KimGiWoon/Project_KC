using UnityEngine;

namespace CJH
{
    public class EncounterStageManager : MonoBehaviour
    {
        public static EncounterStageManager Instance;

        // 게임의 현재 스테이지를 관리합니다. (예: 맵 이동 시 이 값을 변경)
        public int currentStage = 1;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}