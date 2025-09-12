using System.Collections.Generic;
using System.Linq;
using JJY;
using UnityEngine;
using SDW;

namespace KSH
{
    public class RelicDropManager : SingletonManager<RelicDropManager>
    {
        [Header("유물 리스트")]
        [SerializeField] private List<RelicDatas> relics;

        //유물 나오는 UI 있어야함
        [SerializeField] private ClearStageUI _clearStageUI;
        [SerializeField] private BattleManager _battle;
        private GameManager _gameManager;
        //[SerializeField] private BuffRelicManager buffRelicManager;
        //public List<InventoryItem> acquiredRelicLists = new List<InventoryItem>();

        private WeightedRandom<RelicGrade> relicRarityPicker;

        private CharacterState characterState;

        //public System.Action OnRelicSkill;
        protected override void Awake()
        {
            base.Awake();
            relics = Resources.LoadAll<RelicDatas>("Relics").ToList();
            relicRarityPicker = new WeightedRandom<RelicGrade>();
            //TODO : 확률 정해지면 다시 넣기 (임의로 노말 80 레어 20)
            relicRarityPicker.Add(RelicGrade.Normal, 1);
            relicRarityPicker.Add(RelicGrade.Rare, 99);
        }

        private void Start()
        {
            _gameManager = GameManager.Instance;
        }

        private void OnEnable()
        {
            _battle.OnGameResult += GameCleared;
        }

        private void OnDisable()
        {
            _battle.OnGameResult -= GameCleared;
        }

        public void RarityPick(RelicKind relicKind, int amount)
        {
            RelicGrade relicGrade;

            if (relicKind == RelicKind.Debuff)
                relicGrade = RelicGrade.Debuff;
            else
                relicGrade = relicRarityPicker.GetRandom();

            var getRelicList = relics
                .Where(relic =>
                    relic.relicGrade == relicGrade && !GameManager.Instance.InGameItem.relicInventory.Any(r => r.relic == relic))
                .ToList();

            for (int i = 0; i < getRelicList.Count; i++)
            {
                var relic = getRelicList[i];
                int index = Random.Range(i, getRelicList.Count);
                getRelicList[i] = getRelicList[index];
                getRelicList[index] = relic;
            }

            var result = getRelicList.Take(amount).ToList();

            _clearStageUI.ShowRelic(result, relicGrade);
        }

        public void GetRelic(RelicDatas relic)
        {
            //리스트에 같은 유물이 있는지 Bool값
            bool alreadyAcquired = GameManager.Instance.InGameItem.relicInventory.Any(r => r.relic == relic);

            if (!alreadyAcquired) //만약 없다면
            {
                GameManager.Instance.InGameItem.AddItem(relic);

                Debug.Log($"{relic.relicName} 획득");
                //todo 추후 전투 Scene에 들어갈 때 한번에 유물들 효과를 적용하도록 수정 필요
                //BuffRelicManager.Instance.ApplyRelicEffect(relic); //아이템 효과적용
            }
        }

        private void GameCleared(bool isCleared)
        {
            if (!isCleared) return;
            RarityPick(RelicKind.Buf, 3);
        }
    }
}