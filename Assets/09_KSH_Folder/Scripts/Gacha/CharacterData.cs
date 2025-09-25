using UnityEngine;

namespace KSH
{
    [CreateAssetMenu(fileName = "CharacterData", menuName = "Gacha/Character Data")]
    public class CharacterData : ScriptableObject
    {
        public string characterName; //캐릭터 이름
        public Sprite onePullCharacterImage; //1뽑 캐릭터 사진
        public Sprite tenPullCharacterImage; // 10뽑 캐릭터 사진
        public Rarity rarity; //캐릭터 등급
        public PullType PullType; //뽑기 타입
        public int beads;
    }

    public enum Rarity
    {
        Common,
        Rare
    }
    
    public enum PullType
    {
        One,
        Ten
    }
}