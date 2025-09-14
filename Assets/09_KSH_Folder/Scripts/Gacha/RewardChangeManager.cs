using System.Collections.Generic;
using UnityEngine;
using System;
using SDW;

namespace KSH
{
    // public class RewardChangeManager : SingletonManager<RewardChangeManager>
    public class RewardChangeManager : MonoBehaviour
    {
        // public Dictionary<string, bool> ownedCharacters = new Dictionary<string, bool>();
        // public Dictionary<string, int> beadsInventory = new Dictionary<string, int>();

        private CharacterDataManager _charData;

        private int starCandy;

        private void Start()
        {
            starCandy = GameManager.Instance.Coin.starCandy;
            _charData = GameManager.Instance.CharacterData;
        }

        public int StarCandy
        {
            get
            {
                starCandy = GameManager.Instance.Coin.starCandy;
                return starCandy;
            }
            private set
            {
                starCandy = value;
                GameManager.Instance.Coin.SetStarCandy(starCandy);
                OnStarCandyChange?.Invoke(starCandy);
            }
        }

        private int beadMax = 6; //구슬 최대 갯수

        private int normalReward = 30;
        private int RareReward = 2000;

        public int gainedStarCandy = 0;
        public int gainedBead = 0;

        // public bool isStarCandy = false;
        public Dictionary<string, bool> isStarCandy = new Dictionary<string, bool>();

        public event Action<int> OnStarCandyChange;
        public event Action<int> OnStarCandyGained;
        public event Action<int> OnBeadGained;

        public (int starCandy, int bead, int currentBead) ProcessCharacter(CharacterDataSO character)
        {
            int currentBead = 0;
            if (_charData.OwnedCharacters.ContainsKey(character._chaBaseData.ChaEnName))
            {
                if (!_charData.BeadsInventory.ContainsKey(character._chaBaseData.ChaEnName))
                    _charData.SetBead(character._chaBaseData.ChaEnName, 1);

                _charData.SetBead(character._chaBaseData.ChaEnName,
                    _charData.BeadsInventory[character._chaBaseData.ChaEnName] + 1);
                character.Beads++;
                currentBead = character.Beads;

                if (currentBead >= 7)
                {
                    gainedStarCandy = character._chaBaseData.ChaGrade == CharacterGrade.Rare ? RareReward : normalReward;
                    gainedBead = 0;
                    _charData.SetBead(character._chaBaseData.ChaEnName, beadMax);
                    character.Beads = beadMax;
                    StarCandy += gainedStarCandy;

                    if (OnStarCandyGained != null)
                    {
                        Debug.Log("별사탕 이벤트");
                        Debug.Log($"{character._chaBaseData.ChaName} 구슬 6개 초과하였으므로 별사탕 {gainedStarCandy}개 획득!");
                        isStarCandy[character._chaBaseData.ChaName] = true;
                        // OnStarCandyGained?.Invoke(gainedStarCandy);
                    }
                    else
                    {
                        Debug.Log("별사탕 이벤트 호출안됨.");
                    }
                }
                else
                {
                    gainedBead = 1;
                    isStarCandy[character._chaBaseData.ChaName] = false;
                    Debug.Log($"{character._chaBaseData.ChaName}이 중복이므로 구슬 1개 획득!");
                }
            }
            else
            {
                currentBead = AddFirstCharacter(character);
            }
            return (gainedStarCandy, gainedBead, currentBead);
        }

        public int AddFirstCharacter(CharacterDataSO character, int beads = 0)
        {
            int currentBead;
            _charData.SetOwnedCharacter(character._chaBaseData.ChaEnName, true);
            _charData.AllOwnedCharacters.Add(character);
            // ownedCharacters.Add(character._chaBaseData.ChaName, false);

            if (beads > beadMax) currentBead = beadMax;
            else currentBead = beads;

            _charData.SetBead(character._chaBaseData.ChaEnName, currentBead);
            isStarCandy[character._chaBaseData.ChaName] = false;

            character.Beads = beads;
            Debug.Log($"{character._chaBaseData.ChaName} 획득!");
            return currentBead;
        }

        public void AddStarCandy(int count)
        {
            StarCandy += count;
            OnStarCandyChange?.Invoke(StarCandy);
        }
    }
}