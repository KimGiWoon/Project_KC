using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SDW;

namespace CJH
{
    public class MapSelectionInitializer : MonoBehaviour
    {
        public MapGenerator generator;
        public MapConfig config;
        public MapSelectionManager selectionManager;
        public MapPreviewRenderer previewRenderer;
        public MapView mapView;
        public int numberOfMaps = 3;
        [SerializeField] private PrefabRandomizer _prefabRandomizer;

        private GameManager _gameManager;
        private bool _isDownloaded;

        private void Start()
        {
            _gameManager = GameManager.Instance;
            mapView.mapConfig = config;
        }

        private void Update()
        {
            if (!_gameManager.CompleteDownload || !_gameManager.ImageSpriteConnected || !_gameManager.PrefabAndSoConnected ||
                _isDownloaded || !_prefabRandomizer.IsDownloaded) return;
            _isDownloaded = true;
            Init();
        }

        private void Init()
        {
            var maps = new List<MapData>();
            var previews = new List<Sprite>();

            for (int i = 0; i < numberOfMaps; i++)
            {
                // 1. MapData를 생성합니다.
                var map = generator.GenerateMap(config);
                maps.Add(map);

                // 2. MapView에 맵을 그려 미리보기용으로 준비합니다.
                mapView.CreateMapView(map);

                // 3. 현재 그려진 맵을 미리보기로 캡처합니다.
                var preview = previewRenderer.CapturePreview();
                previews.Add(preview);
            }

            // 4. 모든 맵 생성 및 미리보기 캡처가 끝난 후 UI를 초기화합니다.
            selectionManager.Initialize(maps, previews);
        }
    }
}