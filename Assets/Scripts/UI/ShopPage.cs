using System.Collections.Generic;
using DG.Tweening;
using Items;
using Managers;
using Pages;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ShopPage : Page
    {
        public Knife SelectedKnifePrefab { get; private set; }
        public ShopKnife ShopKnifeSelected { get; set; }
        [HideInInspector] public List<ShopKnife> ShopItems => _shopItems;
        [HideInInspector] public int AppleKnivesCount;

        [SerializeField] private ScrollManager _scrollManager;
        [SerializeField] private ShopKnife shopKnifePrefab;
        [SerializeField] private RectTransform _appleKnivesContainer;
        [SerializeField] private RectTransform _bossKnivesContainer;

        [Header("Text")] [SerializeField] private TextMeshProUGUI _counter;
        [SerializeField] private TextMeshProUGUI _price;

        [Header("Knives")] [SerializeField] private Image _knifeUnlocked;
        [SerializeField] private Image _knifeLocked;
        [SerializeField] private Knife[] _appleKnives;
        [SerializeField] private Knife[] _bossKnives;

        [Header("UI")] [SerializeField] private Button _unlockKnifeButton;
        [SerializeField] private GameObject[] _effects;


        private List<ShopKnife> _shopItems;

        
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
            _scrollManager.Init();
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
                    .SetLoops(-1, LoopType.Incremental)
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
            for (int i = 0; i < _appleKnives.Length; i++)
            {
                var item = Instantiate(shopKnifePrefab, _appleKnivesContainer);
                item.Init(_dataManager, OnItemSelected);
                item.IsForBoss = false;
                item.Setup(_appleKnives, i, this);
                _shopItems.Add(item);
            }

            AppleKnivesCount = _appleKnives.Length;

            for (int i = _appleKnives.Length; i < _bossKnives.Length + _appleKnives.Length; i++)
            {
                var item = Instantiate(shopKnifePrefab, _bossKnivesContainer);
                item.Init(_dataManager, OnItemSelected);
                item.IsForBoss = true;
                item.Setup(_bossKnives, i, this);
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
            if (_selected.IsUnlocked)
            {
                return;
            }
            
            if (_dataManager.TotalApples > _selected.Price && _selected.IsForBoss == false)
            {
                _dataManager.TotalApples -= _selected.Price;
                _selected.IsUnlocked = true;
                _selected.UpdateUI();
                _dataManager.SelectedKnifeIndex = _selected.Index;
                UpdateShopUI();
                _soundManager.PlayUnlock();
            }
        }

        public void UpdateShopUI()
        {
            var sprite = _selected.KnifeImage.sprite;
            _knifeUnlocked.sprite = sprite;
            _knifeLocked.sprite = sprite;

            _knifeUnlocked.gameObject.SetActive(_selected.IsUnlocked);
            _knifeLocked.gameObject.SetActive(!_selected.IsUnlocked);

            var itemsUnlocked = _shopItems.FindAll(x => x.IsUnlocked).Count;

            _counter.text = itemsUnlocked + "/" + _appleKnives.Length + _bossKnives.Length;

            SelectedKnifePrefab = _selected.IsForBoss ?
                _bossKnives[_dataManager.SelectedKnifeIndex - _appleKnives.Length] 
                : _appleKnives[_dataManager.SelectedKnifeIndex];

            _selectedKnife.sprite =
                SelectedKnifePrefab.GetComponent<SpriteRenderer>().sprite;
        }
    }
}