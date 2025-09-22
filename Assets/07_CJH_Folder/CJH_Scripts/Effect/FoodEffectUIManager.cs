using UnityEngine;
using System.Collections.Generic;
using JJY;

public class FoodEffectUIManager : MonoBehaviour
{
    public static FoodEffectUIManager Instance { get; private set; }

    [System.Serializable]
    public class EffectAssetMapping
    {
        public EffectType type;
        public Sprite sprite;
        public Sprite secondarySprite; // 보호막 파괴 등 추가 스프라이트
        public EffectIconUI.IconBehavior behavior;
    }

    [Header("UI Prefabs & Parents")]
    [SerializeField] private GameObject effectIconPrefab;
    // 캐릭터 UI에 미리 위치를 잡아둔 부모 Transform들을 연결
    // public Transform buffParent;      // 우상단
    // public Transform debuffParent;    // 좌상단
    // public Transform instantParent;   // 하단
    // public Transform barrierParent;   // 좌측

    [Header("Effect Assets")]
    [SerializeField] private List<EffectAssetMapping> effectAssets;

    // 활성화된 아이콘들을 관리하는 딕셔너리
    // Key: 캐릭터 ID, Value: 이펙트 타입과 아이콘 UI
    private Dictionary<int, Dictionary<EffectType, EffectIconUI>> activeIcons = new Dictionary<int, Dictionary<EffectType, EffectIconUI>>();
    private Dictionary<EffectType, EffectAssetMapping> assetLookup = new Dictionary<EffectType, EffectAssetMapping>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        foreach (var asset in effectAssets)
        {
            assetLookup[asset.type] = asset;
        }
    }

    /// <summary>
    /// 캐릭터에게 음식 효과 UI를 적용합니다.
    /// </summary>
    /// <param name="targetCharacter">효과 대상 캐릭터</param>
    /// <param name="uiParents">캐릭터의 UI 위치별 부모 Transform 컨테이너</param>
    /// <param name="effectData">적용할 효과 데이터</param>
    public void ApplyEffect(GameObject targetCharacter, CharacterUIParents uiParents, FoodEffectData effectData)
    {
        if (!assetLookup.ContainsKey(effectData.type))
        {
            Debug.LogWarning($"Asset for EffectType '{effectData.type}' not found.");
            return;
        }

        int characterId = targetCharacter.GetInstanceID();
        var asset = assetLookup[effectData.type];

        if (!activeIcons.ContainsKey(characterId))
        {
            activeIcons[characterId] = new Dictionary<EffectType, EffectIconUI>();
        }
        var characterActiveIcons = activeIcons[characterId];

        // 갱신이 필요한 효과
        if (asset.behavior == EffectIconUI.IconBehavior.DurationBased)
        {
            // 같은 타입의 버프가 이미 있다면 갱신
            if (characterActiveIcons.ContainsKey(effectData.type))
            {
                characterActiveIcons[effectData.type].Refresh(asset.sprite, effectData.duration);
            }
            else // 없다면 새로 생성
            {
                Transform parent = GetParentForEffect(uiParents, effectData.type);
                CreateNewIcon(characterId, parent, effectData, asset);
            }
        }
        // 중복 실행되는 효과 (회복, 부활, 보호막 등)
        else
        {
            Transform parent = GetParentForEffect(uiParents, effectData.type);
            CreateNewIcon(characterId, parent, effectData, asset, true); // 중복 허용
        }
    }

    private void CreateNewIcon(int ownerId, Transform parent, FoodEffectData data, EffectAssetMapping asset, bool allowDuplicates = false)
    {
        GameObject iconObj = Instantiate(effectIconPrefab, parent);
        EffectIconUI newIcon = iconObj.GetComponent<EffectIconUI>();

        EffectType type = data.type;

        newIcon.Initialize(asset.behavior, asset.sprite, data.duration, () => {
            if (activeIcons.ContainsKey(ownerId) && activeIcons[ownerId].ContainsKey(type))
            {
                // 중복 허용된 아이콘은 참조가 같을 때만 지움
                if (allowDuplicates)
                {
                    if (activeIcons[ownerId][type] == newIcon)
                        activeIcons[ownerId].Remove(type);
                }
                else
                {
                    activeIcons[ownerId].Remove(type);
                }
            }
        });

        // 중복 비허용 아이콘만 딕셔너리에 저장하여 갱신 관리
        if (!allowDuplicates)
        {
            activeIcons[ownerId][type] = newIcon;
        }
    }

    /// <summary>
    /// 보호막을 가진 캐릭터가 피격 당했을 때 호출
    /// </summary>
    public void NotifyBarrierHit(GameObject targetCharacter)
    {
        int characterId = targetCharacter.GetInstanceID();
        if (activeIcons.ContainsKey(characterId) && activeIcons[characterId].ContainsKey(EffectType.CreateBarrierForAll))
        {
            var icon = activeIcons[characterId][EffectType.CreateBarrierForAll];
            var asset = assetLookup[EffectType.CreateBarrierForAll];
            icon.OnBarrierBreak(asset.secondarySprite); // 깨진 이미지 전달
        }
    }

    public void OnCharacterDied(GameObject character)
    {
        int characterId = character.GetInstanceID();
        if (activeIcons.ContainsKey(characterId))
        {
            foreach (var icon in new List<EffectIconUI>(activeIcons[characterId].Values))
            {
                if (icon != null) icon.ForceRemove();
            }
            activeIcons.Remove(characterId);
        }
    }

    private Transform GetParentForEffect(CharacterUIParents parents, EffectType type)
    {
        switch (type)
        {
            case EffectType.AttackBuff:
            case EffectType.DefenseBuff:
            case EffectType.BonusDamageToGroggyMonsters:
                return parents.buffParent; // 우상단

            case EffectType.EnemyAttackDebuff:
            case EffectType.EnemyDefenseDebuff:
                return parents.debuffParent; // 좌상단 (몬스터의 경우 다르게 처리 필요)

            case EffectType.CreateBarrierForAll:
                return parents.barrierParent; // 좌측

            case EffectType.ReviveRandomAllyPercentHP:
            case EffectType.RestoreManaPercentAll:
            case EffectType.InstantHealAll:
                return parents.instantParent; // 하단

            default:
                return parents.buffParent; // 기본값
        }
    }
}

/// <summary>
/// 캐릭터별 UI 부모 Transform을 담는 간단한 데이터 클래스
/// </summary>
public class CharacterUIParents : MonoBehaviour
{
    public Transform buffParent;      // 우상단
    public Transform debuffParent;    // 좌상단
    public Transform instantParent;   // 하단
    public Transform barrierParent;   // 좌측
}