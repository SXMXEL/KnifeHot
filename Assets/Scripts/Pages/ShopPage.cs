using System.Collections.Generic;
using DG.Tweening;
using Items;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Pages
{
    public class ShopPage : Page
    {
        public Knife SelectedKnifePrefab { get; private set; }
        [SerializeField] private ShopKnife shopKnifePrefab;
        [SerializeField] private GameObject _shopContainer;

        [Header("Text")] 
        [SerializeField] private TextMeshProUGUI _counter;
        [SerializeField] private TextMeshProUGUI _price;

        [Header("Knives")] 
        [SerializeField] private Image _knifeUnlocked;
        [SerializeField] private Image _knifeLocked;
        
        [Header("UI")]
        [SerializeField] private Button _unlockKnifeButton;
        [SerializeField] private GameObject[] _effects;


        private List<ShopKnife> _shopItems;

        public Knife[] Knives;
        public ShopKnife ShopKnifeSelected { get; set; }
        private Image _selectedKnife;
        private SoundManager _soundManager;
        private Sequence _knifeIdleSequence;
        private Sequence _effectsRotateSequence;
        private Knife _knifePrefab;
        private DataManager _dataManager;
        private ShopKnife _selected;

        public void Init(
            Image selectedKnife,
            SoundManager soundManager,
            DataManager dataManager)
        {
            _dataManager = dataManager;
            _selectedKnife = selectedKnife;
            _soundManager = soundManager;
            Setup();
            _unlockKnifeButton.onClick.RemoveAllListeners();
            _unlockKnifeButton.onClick.AddListener(() =>
            {
                _soundManager.PlayUnlock();
                UnlockKnife();
            });
        }

        protected override void OnShow()
        {
            base.OnShow();
            _knifeIdleSequence?.Kill();
            _knifeIdleSequence = DOTween.Sequence();

            _knifeLocked.transform.localScale = Vector3.one * 4.5f;
            _knifeIdleSequence.Append(_knifeLocked.transform.DOScale(Vector3.one * 5.5f, 0.8f)
                .SetEase(Ease.InOutSine)
                .SetLoops(int.MaxValue, LoopType.Yoyo));

            _knifeUnlocked.transform.localScale = Vector3.one * 4.5f;
            _knifeIdleSequence.Join(_knifeUnlocked.transform.DOScale(Vector3.one * 5.5f, 0.8f)
                .SetEase(Ease.InOutSine)
                .SetLoops(int.MaxValue, LoopType.Yoyo));
            _knifeIdleSequence.Play();
            _effectsRotateSequence?.Kill();
            _effectsRotateSequence = DOTween.Sequence();
            for (var i = 0; i < _effects.Length; i++)
            {
                var effect = _effects[i];
                effect.transform.localRotation = Quaternion.identity;
                _effectsRotateSequence.Join(
                        effect.transform.DORotate(
                            Vector3.forward * (90 * ((i + 1) % 2 == 0 ? 1 : -1)),
                            3f))
                    .SetLoops(1, LoopType.Incremental)
                    .SetEase(Ease.Linear);
            }

            _effectsRotateSequence.Play();
        }

        protected override void OnHide()
        {
            base.OnHide();
            _knifeIdleSequence.Kill();
        }

        private void Setup()
        {
            _shopItems = new List<ShopKnife>();
            for (int i = 0; i < Knives.Length; i++)
            {
                var item = Instantiate(shopKnifePrefab, _shopContainer.transform);
                item.Init(_dataManager, OnItemSelected);
                item.Setup(i, this);
                _shopItems.Add(item);
            }

            _shopItems[_dataManager.SelectedKnifeIndex].OnItemClick();
        }

        private void OnItemSelected(ShopKnife selectedKnife)
        {
            _selected = selectedKnife;
            _price.text = _selected.Price.ToString();
            UpdateShopUI();
        }

        private void UnlockKnife()
        {
            if (_dataManager.TotalApples > _selected.Price)
            {
                _dataManager.TotalApples -= _selected.Price;
                _selected.IsUnlocked = true;
                _selected.UpdateUI();
                _dataManager.SelectedKnifeIndex = _selected.Index;
                UpdateShopUI();
                _soundManager.PlayUnlock();
            }
        }

        private void UpdateShopUI()
        {
            var sprite = _selected.KnifeImage.sprite;
            _knifeUnlocked.sprite = sprite;
            _knifeLocked.sprite = sprite;

            _knifeUnlocked.gameObject.SetActive(_selected.IsUnlocked);
            _knifeLocked.gameObject.SetActive(!_selected.IsUnlocked);

            var itemsUnlocked = _shopItems.FindAll(x => x.IsUnlocked).Count;

            _counter.text = itemsUnlocked + "/" + Knives.Length;

            SelectedKnifePrefab = Knives[_dataManager.SelectedKnifeIndex];

            _selectedKnife.sprite =
                SelectedKnifePrefab.GetComponent<SpriteRenderer>().sprite;
        }
    }
}