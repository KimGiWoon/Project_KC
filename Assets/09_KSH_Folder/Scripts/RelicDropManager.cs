using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KSH
{
    public class RelicDropManager : SingletonManager<RelicDropManager>
    {
        [Header("유물 리스트")]
        [SerializeField] private List<Relic> relics;
        
        //유물 나오는 UI 있어야함
        [SerializeField] private RelicResultUI relicResultUI;
        [SerializeField] private BuffRelicManager buffRelicManager;
        public List<Relic> acquiredRelicLists = new List<Relic>();
        
        private WeightedRandom<RelicRarity> relicRarityPicker;
        
        private CharacterState characterState;

        protected override void Awake()
        {
            base.Awake();
            relicRarityPicker = new WeightedRandom<RelicRarity>();
            //TODO : 확률 정해지면 다시 넣기 (임의로 노말 80 레어 20)
            relicRarityPicker.Add(RelicRarity.Normal, 50);
            relicRarityPicker.Add(RelicRarity.Rare, 50);
        }

        private void Start()
        {
            RarityPick(RelicEffectType.BuffType,3); //임시로 해둠
        }

        public void RarityPick(RelicEffectType relicEffectType, int amount)
        {
            RelicRarity relicRarity;
            
            if (relicEffectType == RelicEffectType.DeburffType)
                 relicRarity = RelicRarity.None;
            else
                relicRarity = relicRarityPicker.GetRandom();
            
            List<Relic> getRelicList = relics
                .Where(relic => relic.relicRarity == relicRarity && !acquiredRelicLists.Contains(relic))
                .ToList();

            for (int i = 0; i < getRelicList.Count; i++)
            {
                Relic relic = getRelicList[i];
                int index = Random.Range(i, getRelicList.Count);
                getRelicList[i] = getRelicList[index];
                getRelicList[index] = relic;
            }
            
            List<Relic> result = getRelicList.Take(amount).ToList();
            
            relicResultUI.ShowRelic(result, relicRarity);
        }

        public void GetRelic(Relic relic)
        {
            //ToDo : 인벤토리에 유물 추가해야함 (일단 임시로 해둠)
            if (!acquiredRelicLists.Contains(relic)) //얻은 유물이 습득된 유물 리스트에 없다면
            {
                acquiredRelicLists.Add(relic); //리스트에 추가
                Debug.Log($"{relic.relicName} 획득");
                //TODO: 플레이어 스탯 적용 및 효과 적용
                buffRelicManager.ApplyRelicEffect(relic); // 캐릭터 유물 효과 적용
            }
        }
    }    
}
