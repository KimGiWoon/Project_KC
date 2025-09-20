using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace SDW
{
    public class CharInfoBottomUI : BaseUI
    {
        [Header("UI Components")]
        [SerializeField] private TweenAnimation _tweenAnimation;
        [SerializeField] private GameObject _contents;

        private List<LevelUpCharButton> _charButtonList;

        public bool fromMain;
        public Action<CharacterEnName> OnChaButtonClicked;

        private void Awake()
        {
            _panelContainer.SetActive(false);
        }

        public override void Open()
        {
            base.Open();
            if (fromMain)
            {
                fromMain = false;
                return;
            }

            _tweenAnimation.moveBack();
        }

        public override void Close()
        {
            if (fromMain)
            {
                base.Close();
                return;
            }

            _tweenAnimation.moveAway();
            StartCoroutine(DelayedClose());
        }

        private IEnumerator DelayedClose()
        {
            yield return new WaitForSeconds(_tweenAnimation.tweenTime);
            base.Close();
        }

        private void OnDisable()
        {
            if (_charButtonList == null) return;
            foreach (var levelUpCharButton in _charButtonList)
            {
                var button = levelUpCharButton.GetComponent<Button>();
                button.onClick.RemoveListener(() => { OnCharButtonClicked(levelUpCharButton.ChaEnName); });
            }
        }

        public void AddCharacter(LevelUpCharButton levelUpCharButton)
        {
            _charButtonList.Add(levelUpCharButton);

            var button = levelUpCharButton.GetComponent<Button>();
            button.onClick.AddListener(() => { OnCharButtonClicked(levelUpCharButton.ChaEnName); });
        }

        private void OnCharButtonClicked(CharacterEnName enName)
        {
            foreach (var charButton in _charButtonList)
            {
                if (charButton.ChaEnName == enName) charButton.SetSelected(true);
                else charButton.SetSelected(false);
            }

            OnChaButtonClicked?.Invoke(enName);
        }

        public void BottomMoveAway()
        {
            _tweenAnimation.moveAway();
        }

        public void BottomMoveBack()
        {
            _tweenAnimation.moveBack();
        }
    }
}