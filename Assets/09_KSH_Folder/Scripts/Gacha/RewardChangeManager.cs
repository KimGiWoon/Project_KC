using System.Collections.Generic;
using UnityEngine;
using System;
using SDW;

namespace KSH
{
    public class RewardChangeManager : SingletonManager<RewardChangeManager>
    {
        public Dictionary<string, bool> ownedCharacters = new Dictionary<string, bool>();
        public Dictionary<string, int> beadsInventory = new Dictionary<string, int>();

        private int starCandy;

        private void Start()
        {
            starCandy = GameManager.Instance.StarCandy;
        }

        public int StarCandy
        {
            get => starCandy;
            private set
            {
                starCandy = value;
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

        public (int starCandy, int bead, int currentBead) ProcessCharacter(CharacterData character)
        {
            int currentBead = 0;
            if (ownedCharacters.ContainsKey(character.characterName))
            {
                if (!beadsInventory.ContainsKey(character.characterName))
                    beadsInventory[character.characterName] = 1;

                beadsInventory[character.characterName]++;
                character.beads++;
                currentBead = character.beads;

                if (beadsInventory[character.characterName] > beadMax)
                {
                    beadsInventory[character.characterName] = beadMax;
                    character.beads = beadMax;
                    gainedStarCandy = character.rarity == Rarity.Rare ? RareReward : normalReward;
                    StarCandy += gainedStarCandy;

                    if (OnStarCandyGained != null)
                    {
                        Debug.Log("별사탕 이벤트");
                        Debug.Log($"{character.characterName} 구슬 6개 초과하였으므로 별사탕 {gainedStarCandy}개 획득!");
                        isStarCandy[character.characterName] = true;
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
                    isStarCandy[character.characterName] = false;
                    Debug.Log($"{character.characterName}이 중복이므로 구슬 1개 획득!");
                }
            }
            else
            {
                //ownedCharacters[character.characterName] = true;
                ownedCharacters.Add(character.characterName, false);
                beadsInventory.Add(character.characterName, 0);
                isStarCandy[character.characterName] = false;
                character.beads = 0;
                currentBead = 0;
                Debug.Log($"{character.characterName} 획득!");
            }
            return (gainedStarCandy, gainedBead, currentBead);
        }

        public void AddStarCandy(int count)
        {
            StarCandy += count;
        }
    }
}