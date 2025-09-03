using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace SDW
{
    public class DownloadUI : BaseUI
    {
        [Header("UI Components")]
        [SerializeField] private TextMeshProUGUI _mainHeaderText;
        [SerializeField] private TextMeshProUGUI _sizeInfoText;
        [SerializeField] private Button _downloadButton;
        [SerializeField] private Slider _downloadSlider;
        [SerializeField] private TextMeshProUGUI _downloadValueText;

        [Header("Addressable Label")]
        [SerializeField] private AssetLabelReference[] _spriteLabel;

        private long _patchSize;
        private Dictionary<string, long> _patchMap = new Dictionary<string, long>();

        public Action<UIName> OnUIOpenRequested;
        public Action<UIName> OnUICloseRequested;

        private void Awake()
        {
            _panelContainer.SetActive(false);

            _downloadButton.onClick.AddListener(DownloadButtonClicked);
        }

        private void OnEnable()
        {
            _mainHeaderText.text = "업데이트를 확인 중입니다.";
            _sizeInfoText.gameObject.SetActive(false);
            _downloadButton.gameObject.SetActive(false);
            _downloadSlider.gameObject.SetActive(false);
            _downloadSlider.value = 0f;
            _downloadValueText.text = "0%";
        }

        private void OnDisable()
        {
            _mainHeaderText.text = "업데이트를 확인 중입니다.";
            _sizeInfoText.gameObject.SetActive(false);
            _downloadButton.gameObject.SetActive(false);
            _downloadSlider.gameObject.SetActive(false);
            _downloadSlider.value = 0f;
            _downloadValueText.text = "0%";
        }

        public void OnCheckUpdate()
        {
            StartCoroutine(InitializeAddressable());
            StartCoroutine(CheckUpdateFiles());
        }

        private IEnumerator InitializeAddressable()
        {
            var init = Addressables.InitializeAsync(true);
            yield return init;

            // Remote 카탈로그 업데이트
            var catalogHandle = Addressables.UpdateCatalogs();
            yield return catalogHandle;
        }

        #region Check Download

        private IEnumerator CheckUpdateFiles()
        {
            var labels = new List<string>();

            foreach (var label in _spriteLabel)
            {
                if (!LabelExists(label.labelString)) continue;
                labels.Add(label.labelString);
                Debug.Log($"Label : {label.labelString}");
            }

            _patchSize = default;

            foreach (string label in labels)
            {
                var handle = Addressables.GetDownloadSizeAsync(label);
                yield return handle;
                Debug.Log($"{label} : {handle.Result}");

                _patchSize += handle.Result;
            }

            if (_patchSize > decimal.Zero)
            {
                _mainHeaderText.text = "업데이트가 있습니다.";
                _sizeInfoText.gameObject.SetActive(true);
                _downloadButton.gameObject.SetActive(true);
                _downloadSlider.gameObject.SetActive(true);

                _sizeInfoText.text = "" + GetFileSize(_patchSize);
            }
            else
            {
                _downloadValueText.text = "100%";
                _downloadSlider.value = 1f;
                GameManager.Instance.SetCompleteDownload(true);
                yield return new WaitForSeconds(0.5f);

                OnUIOpenRequested?.Invoke(UIName.SignInUI);
                OnUICloseRequested?.Invoke(UIName.DownloadUI);
            }
        }

        private bool LabelExists(string label)
        {
            var handle = Addressables.LoadResourceLocationsAsync(label);

            //# 동기 대기
            handle.WaitForCompletion();

            bool exists = handle.Status == AsyncOperationStatus.Succeeded && handle.Result.Count > 0;

            Addressables.Release(handle);
            return exists;
        }

        private string GetFileSize(long fileSize)
        {
            string size = "0 Byte";

            if (fileSize >= 10737418240.0)
                size = string.Format("{0:##.##}", fileSize / 10737418240.0) + " GB";
            else if (fileSize >= 1048576.0)
                size = string.Format("{0:##.##}", fileSize / 1048576.0) + " MB";
            else if (fileSize >= 1024.0)
                size = string.Format("{0:##.##}", fileSize / 1024.0) + " KB";
            else if (fileSize > 0)
                size = string.Format("{0:##.##}", fileSize) + " Bytes";

            return size;
        }

        #endregion

        #region Download

        private void DownloadButtonClicked() => StartCoroutine(PatchFiles());

        private IEnumerator PatchFiles()
        {
            var labels = new List<string>();

            foreach (var label in _spriteLabel)
            {
                labels.Add(label.labelString);
            }

            foreach (string label in labels)
            {
                // var handle = Addressables.GetDownloadSizeAsync(label);
                // yield return handle;
                //
                // if (handle.Result != decimal.Zero)
                // {
                StartCoroutine(DownloadLabel(label));
                // }
            }

            yield return CheckDownload();
        }

        private IEnumerator DownloadLabel(string label)
        {
            Debug.Log($"Download : {label}");
            _patchMap.Add(label, 0);

            var handle = Addressables.DownloadDependenciesAsync(label, false);

            while (!handle.IsDone)
            {
                _patchMap[label] = handle.GetDownloadStatus().DownloadedBytes;
                yield return new WaitForEndOfFrame();
            }

            _patchMap[label] = handle.GetDownloadStatus().TotalBytes;
            Addressables.Release(handle);
        }

        // private IEnumerator CheckDownload()
        // {
        //     float total = 0f;
        //     _downloadValueText.text = "0 %";
        //
        //     while (true)
        //     {
        //         total += _patchMap.Sum(tmp => tmp.Value);
        //
        //         _downloadSlider.value = total / _patchSize;
        //         _downloadValueText.text = (int)(_downloadSlider.value * 100) + "%";
        //
        //         if (total == _patchSize) break;
        //
        //         total = 0f;
        //         yield return new WaitForEndOfFrame();
        //     }
        //
        //     OnUIOpenRequested?.Invoke(UIName.SignInUI);
        //     OnUICloseRequested?.Invoke(UIName.DownloadUI);
        // }

        private IEnumerator CheckDownload()
        {
            _downloadValueText.text = "0%";

            while (true)
            {
                // 매 프레임마다 합계 새로 계산
                long downloaded = _patchMap.Sum(tmp => tmp.Value);

                float progress = (float)downloaded / _patchSize;
                _downloadSlider.value = progress;
                _downloadValueText.text = (int)(progress * 100) + "%";

                if (downloaded >= _patchSize)
                    break;

                yield return null; // 매 프레임 갱신
            }

            GameManager.Instance.SetCompleteDownload(true);
            // 다운로드 완료 시 다음 UI로
            OnUIOpenRequested?.Invoke(UIName.SignInUI);
            OnUICloseRequested?.Invoke(UIName.DownloadUI);
        }

        #endregion
    }
}