using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KSH
{
    public class RelicDropManager : SingletonManager<RelicDropManager>
    {
        [Header("유물 리스트")]
        [SerializeField] private List<Relic> relics;
        
        //유물 나오는 UI 있어야함
        
        private WeightedRandom<RelicRarity> relicRarityPicker;
        private WeightedRandom<RelicType> relicTypePicker;

        protected override void Awake()
        {
            base.Awake();
            relicRarityPicker = new WeightedRandom<RelicRarity>();
            relicTypePicker = new WeightedRandom<RelicType>();
            
        }

        

        private RelicRarity DecideRarity() //버프 유물 중 Normal, Rare 랜덤 뽑기
        {
            int random = Random.Range(0, 100);
            if (random < 70) //확률 정확하게 정해지면 넣기
                return RelicRarity.Normal;
            else
                return RelicRarity.Rare;
        }

        private RelicType DecideRelicType() // 유물 타입 중 Buff, Debuff 랜덤 뽑기
        {
            int random = Random.Range(0, 100);
            if (random < 70) //확률 정확하게 정해지면 넣기
                return RelicType.Debuff;
            else
                return RelicType.Buff;
        }
        
    }    
}
