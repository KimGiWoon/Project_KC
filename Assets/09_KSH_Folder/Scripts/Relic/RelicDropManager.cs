using System.Collections.Generic;
using System.Linq;
using JJY;
using UnityEngine;
using SDW;

namespace KSH
{
    public class RelicDropManager : MonoBehaviour
    {
        public static RelicDropManager Instance;

        [Header("유물 리스트")]
        [SerializeField] private List<RelicDatas> relics;

        [Header("스크립트")]
        [SerializeField] private ClearStageUI _clearStageUI;
        [SerializeField] private BattleManager _battle;

        private GameManager _gameManager;
        private WeightedRandom<RelicGrade> relicRarityPicker;
        private CharacterState characterState;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            relics = Resources.LoadAll<RelicDatas>("Relics").ToList(); //리소스에 있는 유물들 리스트에 넣기

            relicRarityPicker = new WeightedRandom<RelicGrade>();

            relicRarityPicker.Add(RelicGrade.Normal, 80); //노말 아이템 80
            relicRarityPicker.Add(RelicGrade.Rare, 20); //레어 아이템 20
        }

        private void Start()
        {
            _gameManager = GameManager.Instance;
        }

        private void Update() //테스트용
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                Debug.Log("R");
                GetRelicName("행운의 영수증");
            }
        }

        private void OnEnable()
        {
            _battle.OnGameResult += GameCleared;
        }

        private void OnDisable()
        {
            _battle.OnGameResult -= GameCleared;
        }

        public void RarityPick(RelicKind relicKind, int amount) //등급 뽑기
        {
            RelicGrade relicGrade;

            if (relicKind == RelicKind.Debuff) //만약 유물 종류가 디버프라면
                relicGrade = RelicGrade.Debuff; //유물 등급을 디버프로 정한다.
            else
                relicGrade = relicRarityPicker.GetRandom(); //아니라면 버프(노말, 레어) 유물 중 랜덤으로 뽑는다.

            var getRelicList = relics //유물 리스트에서 뽑힌 등급과 같고 유물 인벤토리에 없다면 가져온다.
                .Where(relic =>
                    relic.relicGrade == relicGrade && !GameManager.Instance.InGameItem.relicInventory.Any(r => r.relic == relic))
                .ToList();

            for (int i = 0; i < getRelicList.Count; i++) //가져온 유물들을 순회
            {
                var relic = getRelicList[i]; //유물[i]를 relic에 저장
                int index = Random.Range(i, getRelicList.Count); //i부터 리스트 끝까지 랜덤으로 하나 선택하여 저장
                getRelicList[i] = getRelicList[index]; //임시로 저장한 것과 자리를 바꿔준다.
                getRelicList[index] = relic;
            }

            var result = getRelicList.Take(amount).ToList(); //리스트에서 amount 갯수만큼 가져온다.

            _clearStageUI.ShowRelic(result, relicGrade);
        }

        public void GetRelic(RelicDatas relic) //유물을 인벤토리에 넣는 기능
        {
            //리스트에 같은 유물이 있는지 Bool값을 따진다.
            bool alreadyAcquired = GameManager.Instance.InGameItem.relicInventory.Any(r => r.relic == relic);

            if (!alreadyAcquired) //만약 없다면
            {
                GameManager.Instance.InGameItem.AddItem(relic); //인벤토리에 유물 아이템을 추가한다.
                int relicCount = GameManager.Instance.InGameItem.GetRelicCount;
                GameManager.Instance.DailyQuest.CompleteQuestInRoguelikeScene(QuestType.GetArtifact, relicCount);
            }
        }

        public void GetRelicName(string relicName) //테스트용
        {
            var relic = relics.Find(r => r.relicName == relicName);

            if (relic != null)
                GetRelic(relic);
        }

        private void GameCleared(bool isCleared) //게임이 클리어되었을 때 유물 뽑는 기능
        {
            if (!isCleared) return;
            RarityPick(RelicKind.Buf, 3);
        }
    }
}