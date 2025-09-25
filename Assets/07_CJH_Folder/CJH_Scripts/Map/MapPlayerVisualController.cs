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
            Debug.LogError("플레이어 프리팹에 SpriteRenderer가 없습니다!", gameObject);
        }
    }

    /// <summary>
    /// 표시할 캐릭터 데이터를 받아와 초기화하고 앞모습을 표시합니다.
    /// </summary>
    public void Initialize()
    {
        characterData = GameManager.Instance.CharacterData.MapPlayerCharacter;
        if (characterData == null)
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);
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
    public void SetMoving(Vector2 direction)
    {
        if (characterData != null && characterData.backViewSprite != null)
        {
            spriteRenderer.sprite = characterData.backViewSprite;

            // 왼쪽으로 이동 시 (direction.x < 0), 캐릭터 이미지를 좌우로 반전시킵니다.
            if (direction.x < 0)
            {
                spriteRenderer.flipX = true;
            }
            // 오른쪽 또는 위로 이동 시, 원래 이미지 방향을 유지합니다.
            else
            {
                spriteRenderer.flipX = false;
            }
        }
    }
}