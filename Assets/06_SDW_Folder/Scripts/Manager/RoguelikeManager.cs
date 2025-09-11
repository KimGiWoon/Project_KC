using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDW
{
    public class RoguelikeManager : MonoBehaviour
    {
        public static RoguelikeManager Instance { get; private set; }

        private BattleEventType _isElite;
        public BattleEventType IsElite => _isElite;

        public Action OnBattleStart;
        public Action OnBattleEnd;

        private int _chapterNumber;
        public int ChapterNumber => _chapterNumber;
        private int _stageNumber;
        public int StageNumber => _stageNumber;

        //todo
        //# 1. 입장하기 버튼 클릭 시
        //~ 1-1. Roguelike UI 모두 Close -> Battle UI Open
        //@ 1-2. Roguelike -> Battle Data 연동(캐릭터 편성, 요리, 유물 - 버프 Off)
        //@ 1-3. 전투 노드 -> Normal, 사건에서의 전투(중립/부정) -> Elite
        //# 2. 전투 패배 또는 클리어
        //@ 2-1. 전투 관련 UI 모두 Close -> Roguelike UI Open(보스인 경우는 Main Lobby로 이동)
        //@ 2-2. 획득한 유물 정보
        //@ 2-3. 사용한 요리
        //@ 2-4. 경험 - 전역(Coin Manager)
        //@ 2-5. 유물 버프 Off
        //# 별건
        //@ 챕터 - Stage 정보(Stage 3의 보스는 BossFinal)

        private void Awake()
        {
            // 싱글톤 초기화: 이미 인스턴스가 있으면 자신을 파괴
            if (Instance == null) Instance = this;
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
        }

        private void Update()
        {
        }

        private void BattleStart()
        {
        }

        private void BattleEnd()
        {
        }

        public void SetBattleEventType(BattleEventType type) => _isElite = type;
    }
}