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
        // �����̴� ���� �����ͼ� ������ ����ġ å�� ������ ǥ��
        _slider.onValueChanged.AddListener(v => _sliderText.text = v.ToString("0"));

        // TODO: ���� �ڿ� ��ü å�� ������ ǥ���ؾ� ��
        // ex. 20/85 ( => 85�� �߿��� 20�� ���)
    }
}
