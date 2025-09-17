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

        /// <summary>
        /// UI 컴포넌트 활성화 설정 및 이벤트 리스너 할당을 수행
        /// </summary>
        private void Awake()
        {
            _panelContainer.SetActive(false);
        }

        /// <summary>
        /// UI 요소가 활성화될 때 필요한 이벤트 연결 수행
        /// </summary>
        private void OnEnable()
        {
            _mainHeaderText.text = "업데이트를 확인 중입니다.";
            _sizeInfoText.gameObject.SetActive(false);
            _downloadButton.gameObject.SetActive(false);
            _downloadSlider.gameObject.SetActive(false);
            _downloadSlider.value = 0f;
            _downloadValueText.text = "0%";

            _downloadButton.onClick.AddListener(DownloadButtonClicked);
        }

        /// <summary>
        /// UI 요소가 비활성화될 때 이벤트 리스너 제거를 수행
        /// </summary>
        private void OnDisable()
        {
            _mainHeaderText.text = "업데이트를 확인 중입니다.";
            _sizeInfoText.gameObject.SetActive(false);
            _downloadButton.gameObject.SetActive(false);
            _downloadSlider.gameObject.SetActive(false);
            _downloadSlider.value = 0f;
            _downloadValueText.text = "0%";

            _downloadButton.onClick.RemoveListener(DownloadButtonClicked);
        }

        /// <summary>
        /// 업데이트 체크 요청을 처리하고 필요한 업데이트 파일의 크기를 확인하는 메서드
        /// </summary>
        public void OnCheckUpdate()
        {
            StartCoroutine(InitializeAddressable());
            StartCoroutine(CheckUpdateFiles());
        }

        /// <summary>
        /// Addressable Assets 초기화를 수행하며, 필요한 카탈로그 업데이트와 함께 진행
        /// </summary>
        private IEnumerator InitializeAddressable()
        {
            var init = Addressables.InitializeAsync(true);
            yield return init;

            // Remote 카탈로그 업데이트
            // var catalogHandle = Addressables.UpdateCatalogs();
            // yield return catalogHandle;
        }

        #region Check Download

        /// <summary>
        /// 업데이트 파일의 크기를 확인하고 업데이트 필요 여부를 판단하는 메서드
        /// </summary>
        private IEnumerator CheckUpdateFiles()
        {
            var labels = new List<string>();

            foreach (var label in _spriteLabel)
            {
                if (!LabelExists(label.labelString)) continue;
                labels.Add(label.labelString);
            }

            _patchSize = default;

            foreach (string label in labels)
            {
                var handle = Addressables.GetDownloadSizeAsync(label);
                yield return handle;

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
                GameManagerEvents.RaiseDownloadCompleted();
                yield return new WaitForSeconds(0.9f);
                //# 다운로드 완료 시 다음 UI로
                // OnUIOpenRequested?.Invoke(UIName.SignInUI);
                // yield return null;
                OnUICloseRequested?.Invoke(UIName.DownloadUI);
            }
        }

        /// <summary>
        /// 지정된 레이블이 존재하는지 확인하는 메서드
        /// </summary>
        /// <param name="label">확인할 레이블 문자열</param>
        /// <returns>레이블이 존재하면 true, 그렇지 않으면 false</returns>
        private bool LabelExists(string label)
        {
            var handle = Addressables.LoadResourceLocationsAsync(label);

            //# 동기 대기
            handle.WaitForCompletion();

            bool exists = handle.Status == AsyncOperationStatus.Succeeded && handle.Result.Count > 0;

            Addressables.Release(handle);
            return exists;
        }

        /// <summary>
        /// 지정된 파일 크기를 사람이 읽기 쉬운 형식(Bytes, KB, MB, GB)으로 변환하여 반환
        /// </summary>
        /// <param name="fileSize">변환할 파일 크기 (바이트 단위)</param>
        /// <returns>파일 크기를 문자열로 표현한 값 (예: "1.23 GB", "456 KB", "789 Bytes")</returns>
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

        /// <summary>
        /// 다운로드 버튼 클릭 이벤트 핸들러 메서드
        /// 다운로드 요청을 처리하고 파일 패치 코루틴을 시작
        /// </summary>
        private void DownloadButtonClicked() => StartCoroutine(PatchFiles());

        /// <summary>
        /// 지정된 레이블에 대한 파일 다운로드 및 패치 프로세스를 관리하는 메서드
        /// </summary>
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

        /// <summary>
        /// 지정된 레이블의 업데이트 파일 다운로드를 시작하고 진행 상황을 추적합니다.
        /// </summary>
        /// <param name="label">다운로드할 업데이트 파일의 레이블</param>
        private IEnumerator DownloadLabel(string label)
        {
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

        /// <summary>
        /// 다운로드 진행 상황을 확인하고 UI에 반영하는 비동기 코루틴 메서드
        /// </summary>
        private IEnumerator CheckDownload()
        {
            _downloadValueText.text = "0%";

            while (true)
            {
                //# 매 프레임마다 합계 새로 계산
                long downloaded = _patchMap.Sum(tmp => tmp.Value);

                float progress = (float)downloaded / _patchSize;
                _downloadSlider.value = progress;
                _downloadValueText.text = (int)(progress * 100) + "%";

                if (downloaded >= _patchSize)
                    break;

                yield return null;
            }

            GameManager.Instance.SetCompleteDownload(true);
            GameManagerEvents.RaiseDownloadCompleted();
            yield return new WaitForSeconds(0.9f);
            OnUICloseRequested?.Invoke(UIName.DownloadUI);
        }

        #endregion
    }
}