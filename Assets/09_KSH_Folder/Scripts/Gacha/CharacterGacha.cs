using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SDW;

namespace KSH
{
    // public class CharacterGacha : SingletonManager<CharacterGacha>
    public class CharacterGacha : MonoBehaviour
    {
        [Header("캐릭터들")]
        // [SerializeField] private List<CharacterData> characterLists; //캐릭터 리스트
        // [SerializeField] private List<CharacterDataSO> characterLists; //캐릭터 리스트
        [Header("UI")]
        [SerializeField] private GachaResultUI gachaResultUI; //캐릭터 결과 UI

        [SerializeField] private GachaUI gachaUI;

        private WeightedRandom<CharacterGrade> rarityPicker; //가중치 랜덤

        private int getCount = 0; //누적 뽑기 횟수
        private const int pityStart = 43; // 천장 뽑기
        private const float pityIncrease = 0.14f; //확률 증가

        private bool _isSingleGacha = false;

        // protected override void Awake()
        private void Awake()
        {
            rarityPicker = new WeightedRandom<CharacterGrade>();
            rarityPicker.Add(CharacterGrade.Normal, 98); //일반 등급은 가중치 98
            rarityPicker.Add(CharacterGrade.Rare, 2); //레어 등급은 가중치 2
        }

        // private void Start()
        // {
        //     getCount = ;
        // }
        public CharacterDataSO GetRandomCharacter() //캐릭터 랜덤 뽑기
        {
            // getCount++; //횟수 누적
            GameManager.Instance.AddGachaCount(1);
            // Debug.Log($"누적 {GameManager.Instance.GachaCount}회");

            var getRarity = rarityPicker.GetRandom(); //가중치 랜덤 뽑기로 등급 뽑기

            if (GameManager.Instance.GachaCount > pityStart &&
                getRarity == CharacterGrade.Normal) //만약 누적 횟수가 43회 초과이고 등급이 기본만 얻었으면
            {
                float pity = pityIncrease * (GameManager.Instance.GachaCount - pityStart) * 100f; //43뽑 이후 누적 횟수당 14%씩 레어 확률 높임
                float roll = Random.Range(0f, 100f); //확률 랜덤 돌리기
                if (roll < pity) //만약 레어 확률이 랜덤확률보다 높다면
                {
                    getRarity = CharacterGrade.Rare; //레어 캐릭터 나옴
                }
            }

            if (getRarity == CharacterGrade.Rare) //만약 레어 캐릭터가 나왔다면
                // getCount = 0; //누적 횟수 초기화
                GameManager.Instance.ClearGachaCount();

            //랜덤으로 뽑힌 등급의 캐릭터들을 리스트로 모은다.
            var getCharacterList = GameManager.Instance.CharacterData.CharacterLists
                .Where(c => c._chaBaseData.ChaGrade == getRarity)
                .ToList();

            //뽑힌 등급의 캐릭터들을 랜덤으로 돌린다.
            var selectChracter = getCharacterList[Random.Range(0, getCharacterList.Count)];

            // Debug.Log($"가챠 결과 → {selectChracter._chaBaseData.ChaName} (등급: {selectChracter._chaBaseData.ChaGrade})");
            return selectChracter;
        }

        private ResultData SingleGacha() //1회 뽑기
        {
            var result = GetRandomCharacter();
            (int gainedStarCandy, int gainedBead, int currentBead) = GameManager.Instance.Reward.ProcessCharacter(result); //중복 처리

            return new ResultData
            {
                Result = new List<CharacterDataSO> { result },
                GainedStarCandy = new List<int> { gainedStarCandy },
                GainedBead = new List<int> { gainedBead },
                CurrentBead = new List<int> { currentBead }
            };
        }

        private ResultData TenGacha() //10회 뽑기
        {
            var result = new List<CharacterDataSO>();
            var gainedStarCandy = new List<int>();
            var gainedBead = new List<int>();
            var currentBeads = new List<int>();

            for (int i = 0; i < 10; i++)
            {
                var character = GetRandomCharacter();
                (int starCandy, int bead, int currentBead) = GameManager.Instance.Reward.ProcessCharacter(character);
                result.Add(character);
                gainedStarCandy.Add(starCandy);
                gainedBead.Add(bead);
                currentBeads.Add(currentBead);
            }
            return new ResultData
            {
                Result = result,
                GainedStarCandy = gainedStarCandy,
                GainedBead = gainedBead,
                CurrentBead = currentBeads
            };
        }

        public ResultData GetGacha()
        {
            if (_isSingleGacha)
                return SingleGacha();

            return TenGacha();
        }

        public void SetGachaType(bool isSingle) => _isSingleGacha = isSingle;

        public void SetGachaResultUI(GachaResultUI ui) => gachaResultUI = ui;
    }
}