using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Aseprite;
using UnityEngine;

public class MapSelectionInitializer : MonoBehaviour
{
    public MapGenerator generator;
    public MapConfig config;
    public MapSelectionManager selectionManager;
    public MapPreviewRenderer previewRenderer;
    public MapView mapView;
    public int numberOfMaps = 3;

    private void Start()
    {
        List<MapData> maps = new();
        List<Sprite> previews = new();

        for (int i = 0; i < numberOfMaps; i++)
        {
            MapData map = generator.GenerateMap(config); // Mapdata 생성
            maps.Add(map);

            mapView.CreateMapView(map); // 맵 UI 생성
            Sprite preview = previewRenderer.CapturePreview(); // 미리보기 생성
            previews.Add(preview);
        }

        selectionManager.Initialize(maps, previews); // 선택을 UI에 반영
    }
}