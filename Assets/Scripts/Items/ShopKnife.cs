﻿using System;
using Managers;
using Pages;
using UnityEngine;
using UnityEngine.UI;

namespace Items
{
    public class ShopKnife : MonoBehaviour
    {
        public int Price => _price;
        public Image KnifeImage => _knifeImage;
        public int Index { get; private set; }

        [Header("Images")] [SerializeField] private Image _backgroundImage;
        [SerializeField] private Image _knifeImage;
        [SerializeField] private Image _unlockImage;


        [Header("Colors")] [SerializeField] private Color _knifeUnlockColor;
        [SerializeField] private Color _knifeLockColor;
        [SerializeField] private Color _knifeUnlockBackgroundColor;
        [SerializeField] private Color _knifeLockBackgroundColor;

        [SerializeField] private int _price;
        [SerializeField] private Button _innerButton;
        [SerializeField] private GameObject _selectedImage;

        private DataManager _dataManager;
        private ShopPage _shopPage;
        private Knife _knife;
        private Action<ShopKnife> _onItemSelected;
        private const string KNIFE_UNLOCKED = "KnifeUnlocked_";

        public bool IsUnlocked
        {
            get
            {
                if (Index == 0)
                {
                    return true;
                }

                return PlayerPrefs.GetInt(KNIFE_UNLOCKED + Index, 0) == 1;
            }
            set => PlayerPrefs.SetInt(KNIFE_UNLOCKED + Index, value ? 1 : 0);
        }

        public bool IsSelected
        {
            get => _selectedImage.activeSelf;
            private set
            {
                if (value)
                {
                    if (_shopPage.ShopKnifeSelected != null)
                    {
                        _shopPage.ShopKnifeSelected.IsSelected = false;
                    }

                    _shopPage.ShopKnifeSelected = this;
                }

                _selectedImage.SetActive(value);
            }
        }


        public void Init(DataManager dataManager, Action<ShopKnife> onItemSelected)
        {
            _dataManager = dataManager;
            _onItemSelected = onItemSelected;
            _innerButton.onClick.RemoveAllListeners();
            _innerButton.onClick.AddListener(OnItemClick);
        }

        public void Setup(int index, ShopPage shopPage)
        {
            _shopPage = shopPage;
            Index = index;
            _knife = _shopPage.Knives[Index];
            _knifeImage.sprite = _knife.GetComponent<SpriteRenderer>().sprite;
            UpdateUI();
        }

        public void OnItemClick()
        {
            if (!IsSelected)
            {
                IsSelected = true;
            }

            if (IsUnlocked)
            {
                _dataManager.SelectedKnifeIndex = Index;
            }

            _onItemSelected.Invoke(this);
        }

        public void UpdateUI()
        {
            _backgroundImage.color = IsUnlocked ? _knifeUnlockBackgroundColor : _knifeLockBackgroundColor;
            _unlockImage.gameObject.SetActive(!IsUnlocked);

            _knifeImage.transform.GetChild(0).GetComponent<Image>().color =
                IsUnlocked ? _knifeUnlockColor : _knifeLockColor;
            _knifeImage.transform.GetChild(0).gameObject.SetActive(!IsUnlocked);
        }
    }
}