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
        private List<Relic> acquiredRelicLists = new List<Relic>();
        
        private WeightedRandom<RelicRarity> relicRarityPicker;
        
        private CharacterState characterState;

        protected override void Awake()
        {
            base.Awake();
            relicRarityPicker = new WeightedRandom<RelicRarity>();
            //TODO : 확률 정해지면 다시 넣기
            relicRarityPicker.Add(RelicRarity.Normal, 80);
            relicRarityPicker.Add(RelicRarity.Rare, 20);
        }

        private void Start()
        {
            BattleStageClear();
        }

        public void BattleStageClear() //전투스테이지 클리어 시 버프 유물 중 3중 1택
        {
            RelicRarity relicRarity = relicRarityPicker.GetRandom();
            
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
            
            List<Relic> result = getRelicList.Take(3).ToList();
            
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
                buffRelicManager.ApplyRelicEffect(characterState, relic.relicEffectValues); // 유물 효과 적용
                
            }
        }
    }    
}
