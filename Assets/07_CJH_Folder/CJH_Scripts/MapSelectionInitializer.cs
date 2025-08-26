using System.Collections.Generic;
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
            // 1. MapData�� �����մϴ�.
            MapData map = generator.GenerateMap(config);
            maps.Add(map);

            // 2. MapView�� ���� �׷� �̸���������� �غ��մϴ�.
            mapView.CreateMapView(map);

            // 3. ���� �׷��� ���� �̸������ ĸó�մϴ�.
            Sprite preview = previewRenderer.CapturePreview();
            previews.Add(preview);
        }

        // 4. ��� �� ���� �� �̸����� ĸó�� ���� �� UI�� �ʱ�ȭ�մϴ�.
        selectionManager.Initialize(maps, previews);

    }
}