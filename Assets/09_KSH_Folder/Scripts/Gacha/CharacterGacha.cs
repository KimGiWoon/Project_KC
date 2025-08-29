using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SDW;

namespace KSH
{
    public class CharacterGacha : SingletonManager<CharacterGacha>
    {
        [Header("캐릭터들")]
        [SerializeField] private List<CharacterData> characterLists; //캐릭터 리스트
        [Header("UI")]
        [SerializeField] private GachaResultUI gachaResultUI; //캐릭터 결과 UI

        [SerializeField] private GachaUI gachaUI;

        private WeightedRandom<Rarity> rarityPicker; //가중치 랜덤

        private int getCount = 0; //누적 뽑기 횟수
        private const int pityStart = 43; // 천장 뽑기
        private const float pityIncrease = 0.14f; //확률 증가

        private bool _isSingleGacha = false;

        protected override void Awake()
        {
            base.Awake();
            rarityPicker = new WeightedRandom<Rarity>();
            rarityPicker.Add(Rarity.Common, 98); //일반 등급은 가중치 98
            rarityPicker.Add(Rarity.Rare, 2); //레어 등급은 가중치 2
        }

        private void Start()
        {
            getCount = GameManager.Instance.GachaCount;
        }
        public CharacterData GetRandomCharacter() //캐릭터 랜덤 뽑기
        {
            getCount++; //횟수 누적
            Debug.Log($"누적 {getCount}회");

            var getRarity = rarityPicker.GetRandom(); //가중치 랜덤 뽑기로 등급 뽑기

            if (getCount > pityStart && getRarity == Rarity.Common) //만약 누적 횟수가 43회 초과이고 등급이 기본만 얻었으면
            {
                float pity = pityIncrease * (getCount - pityStart) * 100f; //43뽑 이후 누적 횟수당 14%씩 레어 확률 높임
                float roll = Random.Range(0f, 100f); //확률 랜덤 돌리기
                if (roll < pity) //만약 레어 확률이 랜덤확률보다 높다면
                {
                    getRarity = Rarity.Rare; //레어 캐릭터 나옴
                }
            }

            if (getRarity == Rarity.Rare) //만약 레어 캐릭터가 나왔다면
                getCount = 0; //누적 횟수 초기화

            //랜덤으로 뽑힌 등급의 캐릭터들을 리스트로 모은다.
            var getChracterList = characterLists
                .Where(c => c.rarity == getRarity)
                .ToList();

            //뽑힌 등급의 캐릭터들을 랜덤으로 돌린다.
            var selectChracter = getChracterList[Random.Range(0, getChracterList.Count)];

            Debug.Log($"가챠 결과 → {selectChracter.characterName} (등급: {selectChracter.rarity})");
            return selectChracter;
        }

        private List<CharacterData> SingleGacha() //1회 뽑기
        {
            //todo Result UI가 열릴 때 호출해서 가져오도록 수정
            var result = GetRandomCharacter();
            RewardChangeManager.Instance.ProcessCharacter(result); //중복 처리
            return new List<CharacterData> { result };
        }

        private List<CharacterData> TenGacha() //10회 뽑기
        {
            //todo Result UI가 열릴 때 호출해서 가져오도록 수정
            var result = new List<CharacterData>();

            for (int i = 0; i < 10; i++)
            {
                var character = GetRandomCharacter();
                RewardChangeManager.Instance.ProcessCharacter(character);
                result.Add(character);
            }
            return result;
        }

        public List<CharacterData> GetGacha()
        {
            if (_isSingleGacha)
                return SingleGacha();

            return TenGacha();
        }

        public void SetGachaType(bool isSingle) => _isSingleGacha = isSingle;
    }
}