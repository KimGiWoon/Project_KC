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
            BattleStageGetRelic();
        }

        public Relic BattleStageClear() //전투스테이지 클리어 시 버프 유물 중 3중 1택
        {
            //RelicRarity relicRarity = relicRarityPicker.GetRandom();
            RelicRarity relicRarity = RelicRarity.Normal;
            
            List<Relic> getRelicList = relics
                .Where(relic => relic.relicRarity == relicRarity)
                .ToList();
            
            Relic selectRelic = getRelicList[Random.Range(0, getRelicList.Count)];
            return selectRelic;
        }

        public void BattleStageGetRelic() //3중 택 1
        {
            List<Relic> result = new List<Relic>();

            for (int i = 0; i < 3; i++)
            {
                Relic relic = BattleStageClear();
                result.Add(relic);
            }
            // UI연결
            relicResultUI.ShowRelic(result);
        }
    }    
}
