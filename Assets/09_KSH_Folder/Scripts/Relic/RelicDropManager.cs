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
        [SerializeField] private RelicResultUI relicResultUI;
        [SerializeField] private BuffRelicManager buffRelicManager;
        public List<InventoryItem> acquiredRelicLists = new List<InventoryItem>();
        
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
            RarityPick(RelicKind.Buf,3); //임시로 해둠
        }

        //테스트용
        private void Update()
        {
        //if (Input.GetKeyDown(KeyCode.R))
        //{
        //    OnRelicSkill?.Invoke();
        //}
        }

        public void RarityPick(RelicKind relicKind, int amount)
        { 
            RelicGrade relicGrade;
            
            if (relicKind == RelicKind.Debuff)
                 relicGrade = RelicGrade.Debuff;
            else
                relicGrade = relicRarityPicker.GetRandom();
            
            List<RelicDatas> getRelicList = relics
                .Where(relic => relic.relicGrade == relicGrade && !acquiredRelicLists.Any(r => r.relic == relic))
                .ToList();

            for (int i = 0; i < getRelicList.Count; i++)
            {
                RelicDatas relic = getRelicList[i];
                int index = Random.Range(i, getRelicList.Count);
                getRelicList[i] = getRelicList[index];
                getRelicList[index] = relic;
            }
            
            List<RelicDatas> result = getRelicList.Take(amount).ToList();
            
            relicResultUI.ShowRelic(result, relicGrade);
        }

        public void GetRelic(RelicDatas relic)
        {
            //리스트에 같은 유물이 있는지 Bool값
            bool alreadyAcquired = acquiredRelicLists.Any(r => r.relic == relic);

            if (!alreadyAcquired) //만약 없다면
            {
                var item = new InventoryItem(relic);
                acquiredRelicLists.Add(item); //아이템 추가
            
                Debug.Log($"{relic.relicName} 획득");
                buffRelicManager.ApplyRelicEffect(relic); //아이템 효과적용
            }
        }
    }    
}
