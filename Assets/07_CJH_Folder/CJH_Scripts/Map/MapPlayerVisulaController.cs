using UnityEngine;
using SDW;

public class MapPlayerVisualController : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private CharacterDataSO characterData;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("플레이어 프리팹에 SpriteRenderer가 없습니다!", this.gameObject);
        }
    }

    /// <summary>
    /// 표시할 캐릭터 데이터를 받아와 초기화하고 앞모습을 표시합니다.
    /// </summary>
    public void Initialize()
    {
        characterData = GameManager.Instance.MapPlayerCharacter;
        if (characterData == null)
        {
            gameObject.SetActive(false);
            return;
        }
        SetIdle();
    }

    /// <summary>
    /// 멈춤 상태 (앞모습)로 변경
    /// </summary>
    public void SetIdle()
    {
        if (characterData != null && characterData.frontViewSprite != null)
        {
            spriteRenderer.sprite = characterData.frontViewSprite;
        }
    }

    /// <summary>
    /// 이동 상태 (뒷모습)로 변경
    /// </summary>
    public void SetMoving()
    {
        if (characterData != null && characterData.backViewSprite != null)
        {
            spriteRenderer.sprite = characterData.backViewSprite;
        }
    }
}