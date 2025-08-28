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
        
        private WeightedRandom<RelicRarity> relicRarityPicker;
        private WeightedRandom<RelicType> relicTypePicker;

        protected override void Awake()
        {
            base.Awake();
            relicRarityPicker = new WeightedRandom<RelicRarity>();
            relicTypePicker = new WeightedRandom<RelicType>();
            //임시로 정해둔 것
            relicRarityPicker.Add(RelicRarity.Normal, 90);
            relicRarityPicker.Add(RelicRarity.Rare, 10);
            relicTypePicker.Add(RelicType.Buff, 90);
            relicTypePicker.Add(RelicType.Debuff, 10);
        }

        private void Start()
        {
            BattleStageClear();
        }

        public void BattleStageClear() //전투스테이지 클리어 시 버프 유물 중 3중 1택
        {
            RelicRarity relicRarity = relicRarityPicker.GetRandom();
            
            List<Relic> getRelicList = relics
                .Where(relic => relic.relicRarity == relicRarity)
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
    }    
}
