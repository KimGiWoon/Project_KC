using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace SDW
{
    public class ImagePrefabLoadingUI : BaseUI
    {
        [Header("UI Component")]
        [SerializeField] private TextMeshProUGUI _connectText;
        private Coroutine _coroutine;

        private void Awake()
        {
            _panelContainer.SetActive(false);
        }

        public override void Open()
        {
            base.Open();
            _coroutine = StartCoroutine(LoadProgress());
        }

        public override void Close()
        {
            if (_coroutine != null) StopCoroutine(_coroutine);
            base.Close();
        }

        private IEnumerator LoadProgress()
        {
            bool _canAdd = true;
            int count = 0;

            while (true)
            {
                yield return new WaitForSeconds(1f);
                if (_canAdd) _connectText.text += ".";
                else _connectText.text = _connectText.text.Substring(0, _connectText.text.Length - 1);

                count++;

                if (count >= 2)
                {
                    count = 0;
                    _canAdd = !_canAdd;
                }
            }
        }
    }
}