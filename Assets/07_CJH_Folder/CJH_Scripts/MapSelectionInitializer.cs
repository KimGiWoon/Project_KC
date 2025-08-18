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
    public MapView mapView; // ¡ç Ãß°¡
    public int numberOfMaps = 3;

    private void Start()
    {
        List<MapData> maps = new();
        List<Sprite> previews = new();

        for (int i = 0; i < numberOfMaps; i++)
        {
            MapData map = generator.GenerateMap(config);
            maps.Add(map);

            mapView.CreateMapView(map);
            Sprite preview = previewRenderer.CapturePreview();
            previews.Add(preview);
        }

        selectionManager.Initialize(maps, previews);
    }
}