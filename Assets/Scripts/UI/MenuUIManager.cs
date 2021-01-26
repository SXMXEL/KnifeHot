using System;
using Managers;
using Pages;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class MenuUIManager : MonoBehaviour
    {
        public ShopPage ShopPage => shopPage;

        [Header("UI items managers")] [SerializeField]
        private ShopPage shopPage;

        [SerializeField] private RewardUI _rewardUI;
        [SerializeField] private RewardTimeManager rewardTimeTimeManager;

        [Header("UI")] [SerializeField] private Image _selectedKnife;
        [SerializeField] private GameObject _soundOn;
        [SerializeField] private GameObject _soundOff;
        [SerializeField] private GameObject _vibrateOn;
        [SerializeField] private GameObject _vibrateOff;
        [SerializeField] private TextMeshProUGUI _totalApplesText;
        [SerializeField] private GameObject[] _effects;

        [Header("Text")] [SerializeField] private TextMeshProUGUI _highStage;
        [SerializeField] private TextMeshProUGUI _highScore;

        [Header("Buttons")] [SerializeField] private Button _playButton;
        [SerializeField] private Button _ShopBackToMenuButton;
        [SerializeField] private Button _SettingsBackToMenuButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _soundButton;
        [SerializeField] private Button _vibrateButton;
        [SerializeField] private Button _shopButton;


        private SoundManager _soundManager;
        private ScoreManager _scoreManager;
        private DataManager _dataManager;
        private PageManager _pageManager;
        private LevelManager _levelManager;

        public float ScreenHeight => Camera.main.orthographicSize * 2;
        public float ScreenWidth => ScreenHeight / Screen.height * Screen.width;

        public void Init(
            LevelManager levelManager,
            DataManager dataManager,
            SoundManager soundManager,
            ScoreManager scoreManager,
            PageManager pageManager)
        {
            _levelManager = levelManager;
            _dataManager = dataManager;
            _soundManager = soundManager;
            _scoreManager = scoreManager;
            _pageManager = pageManager;
            _rewardUI.Init(
                _soundManager,
                _dataManager,
                rewardTimeTimeManager);
            shopPage.Init(
                _selectedKnife,
                _soundManager,
                _dataManager);
            _settingsButton.onClick.RemoveAllListeners();
            _settingsButton.onClick.AddListener(() =>
            {
                _soundManager.PlayButton();
                _pageManager.PageState = PageState.SettingsPage;
            });
            _ShopBackToMenuButton.onClick.RemoveAllListeners();
            _ShopBackToMenuButton.onClick.AddListener(() =>
            {
                _soundManager.PlayButton();
                _pageManager.PageState = PageState.MenuPage;
            });
            _SettingsBackToMenuButton.onClick.RemoveAllListeners();
            _SettingsBackToMenuButton.onClick.AddListener(() =>
            {
                _soundManager.PlayButton();
                _pageManager.PageState = PageState.MenuPage;
            });
            _playButton.onClick.RemoveAllListeners();
            _playButton.onClick.AddListener(() =>
            {
                _soundManager.PlayButton();
                _pageManager.PageState = PageState.GamePage;
                new DelayWrappedCommand(_levelManager.InitializeGame, 0.5f).Started();
            });
            _soundButton.onClick.RemoveAllListeners();
            _soundButton.onClick.AddListener(() =>
            {
                _soundManager.PlayButton();
                SoundOnOff();
                UpdateSoundsUI();
            });
            _vibrateButton.onClick.RemoveAllListeners();
            _vibrateButton.onClick.AddListener(() =>
            {
                _soundManager.PlayButton();
                VibrateOnOff();
                UpdateVibrationUI();
            });
            _shopButton.onClick.RemoveAllListeners();
            _shopButton.onClick.AddListener(() =>
            {
                _soundManager.PlayButton();
                _pageManager.PageState = PageState.ShopPage;
            });
            UpdateSoundsUI();
            UpdateVibrationUI();
        }

        private void Update()
        {
            if (shopPage.gameObject.activeInHierarchy)
            {
                _effects[0].transform.Rotate(0, 0, 40 * Time.deltaTime);
                _effects[1].transform.Rotate(0, 0, -40 * Time.deltaTime);
            }

            _totalApplesText.text = _dataManager.TotalApples.ToString();
            _highScore.text = "SCORE " + _dataManager.HighScore;
            _highStage.text = "STAGE " + _dataManager.HighStage;
        }

        private void SoundOnOff()
        {
            _soundManager.PlayButton();
            _dataManager.SoundsSettings = !_dataManager.SoundsSettings;
        }

        private void VibrateOnOff()
        {
            _soundManager.PlayButton();
            _dataManager.VibrationSettings = !_dataManager.VibrationSettings;
        }

        private void UpdateSoundsUI()
        {
            if (_dataManager.SoundsSettings)
            {
                _soundOn.SetActive(true);
                _soundOff.SetActive(false);
            }
            else
            {
                _soundOn.SetActive(false);
                _soundOff.SetActive(true);
            }
        }

        private void UpdateVibrationUI()
        {
            if (_dataManager.VibrationSettings)
            {
                _vibrateOn.SetActive(true);
                _vibrateOff.SetActive(false);
            }
            else
            {
                _vibrateOn.SetActive(false);
                _vibrateOff.SetActive(true);
            }
        }
    }
}