using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapSelectionManager : MonoBehaviour
{
    [Header("Dependencies")]
    public MapView mapView;
    public Transform mapListContainer;
    public GameObject mapButtonPrefab;

    private List<MapData> availableMaps = new();
    private List<Sprite> previewImages = new();

    private int lastSelectedIndex = -1;

    public void Initialize(List<MapData> maps, List<Sprite> previews)
    {
        availableMaps = maps;
        previewImages = previews;
        lastSelectedIndex = MapPrefs.LoadSelectedMapIndex();

        GenerateUI();

        // ������ ���� �� �ڵ� �ε�
        if (lastSelectedIndex >= 0 && lastSelectedIndex < maps.Count)
        {
            mapView.CreateMapView(availableMaps[lastSelectedIndex]);
        }
    }

    private void GenerateUI()
    {
        foreach (Transform child in mapListContainer)
            Destroy(child.gameObject);

        for (int i = 0; i < availableMaps.Count; i++)
        {
            int index = i;
            GameObject buttonObj = Instantiate(mapButtonPrefab, mapListContainer);
            Button button = buttonObj.GetComponent<Button>();
            Image image = buttonObj.GetComponentInChildren<Image>();

            if (image != null && i < previewImages.Count)
                image.sprite = previewImages[i];

            // ����: �������� ������ ��ư�̸� ���� ǥ��
            if (index == lastSelectedIndex)
            {
                Image btnImg = button.GetComponent<Image>();
                if (btnImg != null)
                    btnImg.color = Color.yellow;
            }

            button.onClick.AddListener(() => SelectMap(index));
        }
    }

    private void SelectMap(int index)
    {
        MapPrefs.SaveSelectedMapIndex(index);
        mapView.CreateMapView(availableMaps[index]);
    }
}