using System.Collections.Generic;
using UnityEngine;
using System;
using SDW;

namespace KSH
{
    public class RewardChangeManager : SingletonManager<RewardChangeManager>
    {
        private Dictionary<string, bool> ownedCharacters = new Dictionary<string, bool>();
        private Dictionary<string, int> beadsInventory = new Dictionary<string, int>();

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
        
        public event Action<int> OnStarCandyChange;
        public event Action<int> OnStarCandyGained;
        public event Action<int> OnBeadGained;
        
        public (int starCandy, int bead) ProcessCharacter(CharacterData character)
        {
            int gainedStarCandy = 0;
            int gainedBead = 0;
            
            if (ownedCharacters.ContainsKey(character.characterName))
            {
                if(!beadsInventory.ContainsKey(character.characterName))
                    beadsInventory[character.characterName] = 0;
            
                beadsInventory[character.characterName]++;
            
                if (beadsInventory[character.characterName] > beadMax)
                {
                    beadsInventory[character.characterName] = beadMax;
                
                    gainedStarCandy = (character.rarity == Rarity.Rare) ? RareReward : normalReward;
                    StarCandy += gainedStarCandy;
                    OnStarCandyChange?.Invoke(gainedStarCandy);
                    Debug.Log($"{character.characterName} 구슬 6개 초과하였으므로 별사탕 {gainedStarCandy}개 획득!");
                }
                else
                {
                    gainedBead = 1;
                    OnBeadGained?.Invoke(gainedBead);
                    Debug.Log($"{character.characterName}이 중복이므로 구슬 1개 획득!");
                }
            }
            else
            {
                ownedCharacters[character.characterName] = true;
                Debug.Log($"{character.characterName} 획득!");
            }
            return (gainedStarCandy, gainedBead);
        }
        
        public void AddStarCandy(int count)
        {
            StarCandy += count;
        }
    }

}