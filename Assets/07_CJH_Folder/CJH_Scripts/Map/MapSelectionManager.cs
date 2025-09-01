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

        // 마지막 선택 맵 자동 로드
        if (lastSelectedIndex >= 0 && lastSelectedIndex < maps.Count)
        {
            mapView.CreateMapView(availableMaps[lastSelectedIndex]);
        }
    }
}