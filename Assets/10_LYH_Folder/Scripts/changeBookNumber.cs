using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class changeBookNumber : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private TextMeshProUGUI _sliderText;

    private void Start()
    {
        // 슬라이더 값을 가져와서 선택한 경험치 책의 개수를 표기
        _slider.onValueChanged.AddListener(v => _sliderText.text = v.ToString("0"));

        // TODO: 개수 뒤에 전체 책의 개수를 표기해야 함
        // ex. 20/85 ( => 85개 중에서 20개 사용)
    }
}
