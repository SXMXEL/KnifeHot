using DG.Tweening;
using Managers;
using Pages;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class MenuPage : MonoBehaviour
    {
        public ShopPage ShopPage => shopPage;

        [Header("Items managers")] 
        [SerializeField] private ShopPage shopPage;
        [SerializeField] private RewardUI _rewardUI;
        [SerializeField] private RewardTimeManager rewardTimeTimeManager;

        [Header("UI")] 
        [SerializeField] private Image _selectedKnife;
        [SerializeField] private Image _appleImage;
        [SerializeField] private GameObject _soundOn;
        [SerializeField] private GameObject _soundOff;
        [SerializeField] private GameObject _vibrateOn;
        [SerializeField] private GameObject _vibrateOff;
        [SerializeField] private TextMeshProUGUI _totalApplesText;

        [Header("Text")] 
        [SerializeField] private TextMeshProUGUI _highStage;
        [SerializeField] private TextMeshProUGUI _highScore;
        [SerializeField] private TextMeshProUGUI _knifeText;
        [SerializeField] private TextMeshProUGUI _hotText;

        [Header("Buttons")] 
        [SerializeField] private Button _rateButton;
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _ShopBackToMenuButton;
        [SerializeField] private Button _SettingsBackToMenuButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _soundButton;
        [SerializeField] private Button _vibrateButton;
        [SerializeField] private Button _shopButton;

        [SerializeField] private Transform _bottomButtons;
        [SerializeField] private GameObject _lunarConsole;
        [SerializeField] private GameObject _startAnimationBlocker;
        
        private SoundManager _soundManager;
        private DataManager _dataManager;
        private PageManager _pageManager;
        private LevelManager _levelManager;
        private Sequence _startAnimation;
        private Sequence _hotTextSequence;

        public float ScreenHeight => Camera.main.orthographicSize * 2;

        public void Init(
            LevelManager levelManager,
            DataManager dataManager,
            SoundManager soundManager,
            PageManager pageManager)
        {
            _levelManager = levelManager;
            _dataManager = dataManager;
            _soundManager = soundManager;
            _pageManager = pageManager;
            _rewardUI.Init(
                _soundManager,
                _dataManager,
                rewardTimeTimeManager);
            shopPage.Init(
                _selectedKnife,
                _soundManager,
                _dataManager);
            _rateButton.onClick.RemoveAllListeners();
            _rateButton.onClick.AddListener(() => { Application.OpenURL("https://github.com/SXMXEL/KnifeHot"); });
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
                _levelManager.StartGame(true);
                _playButton.interactable = false;
                new DelayWrappedCommand(() =>
                {
                    _playButton.interactable = true;
                    _pageManager.PageState = PageState.GamePage;
                }, 0.55f).Started();
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
            var devModeButton = _knifeText.GetComponent<Button>();
            devModeButton.onClick.RemoveAllListeners();
            devModeButton.onClick.AddListener(() => { _lunarConsole.SetActive(!_lunarConsole.activeSelf); });
            UpdateSoundsUI();
            UpdateVibrationUI();
            StartAnimation();
        }

        private void StartAnimation()
        {
            var delay = 0.5f;
            var knifeTextPosition = _knifeText.transform.position;
            var hotTextPosition = _hotText.transform.position;
            var bottomButtonsPosition = _bottomButtons.transform.position;
            var selectedKnifePosition = _selectedKnife.transform.position;
            var highScorePosition = _highScore.transform.position;
            var highStagePosition = _highStage.transform.position;
            _startAnimationBlocker.SetActive(true);
            _settingsButton.gameObject.SetActive(false);
            _totalApplesText.gameObject.SetActive(false);
            _highScore.gameObject.SetActive(false);
            _highStage.gameObject.SetActive(false);
            _selectedKnife.gameObject.SetActive(false);
            _appleImage.gameObject.SetActive(false);
            _playButton.gameObject.SetActive(false);
            _bottomButtons.gameObject.SetActive(false);
            _knifeText.gameObject.SetActive(false);
            _hotText.gameObject.SetActive(false);
            _startAnimation?.Kill();
            _startAnimation = DOTween.Sequence();
            _startAnimation.Append(_selectedKnife.transform
                .DOMove(new Vector3(0, -7.5f, 0), 0.1f));
            _startAnimation.AppendCallback(() => _selectedKnife.gameObject.SetActive(true));
            _startAnimation.Append(_selectedKnife.transform
                .DOMove(selectedKnifePosition, delay).SetEase(Ease.OutBack));
            _startAnimation.Append(_knifeText.transform
                .DOMove(new Vector3(-10f, knifeTextPosition.y, knifeTextPosition.z), 0.1f));
            _startAnimation.Join(_hotText.transform
                .DOMove(new Vector3(10f, hotTextPosition.y, hotTextPosition.z), 0.1f));
            _startAnimation.AppendCallback(() =>
            {
                _knifeText.gameObject.SetActive(true);
                _hotText.gameObject.SetActive(true);
            });
            _startAnimation.Append(_knifeText.transform
                .DOMove(new Vector3(0, knifeTextPosition.y, knifeTextPosition.z), delay)
                .SetEase(Ease.OutBack));
            _startAnimation.Join(_hotText.transform
                .DOMove(new Vector3(0, hotTextPosition.y, hotTextPosition.z), delay)
                .SetEase(Ease.OutBack));
            _startAnimation.Append(_playButton.transform.DOScale(Vector3.zero, 0.1f));
            _startAnimation.AppendCallback(() => _playButton.gameObject.SetActive(true));
            _startAnimation.Append(_playButton.transform
                .DOScale(Vector3.one, 0.9f)
                .SetEase(Ease.OutBack));
            _startAnimation.Append(_bottomButtons.transform
                .DOMove(new Vector3(0, -12.5f, 0), 0.1f)
                .SetEase(Ease.OutBack));
            _startAnimation.AppendCallback(() => _bottomButtons.gameObject.SetActive(true));
            _startAnimation.Append(_bottomButtons.transform
                .DOMove(bottomButtonsPosition, delay)
                .SetEase(Ease.OutBack));
            
            _startAnimation.Append(_settingsButton.transform.DOScale(Vector3.zero, 0.1f));
            _startAnimation.Join(_appleImage.transform.DOScale(Vector3.zero, 0.1f));
            _startAnimation.Join(_highScore.transform
                .DOMove(new Vector3(-10f, highScorePosition.y, highScorePosition.z), 0.1f));
            _startAnimation.Join(_highStage.transform
                .DOMove(new Vector3(10f, highStagePosition.y, highStagePosition.z), 0.1f));
            _startAnimation.AppendCallback(() =>
            {
                _appleImage.gameObject.SetActive(true);
                _settingsButton.gameObject.SetActive(true);
                _totalApplesText.gameObject.SetActive(true);
                _highScore.gameObject.SetActive(true);
                _highStage.gameObject.SetActive(true);
            });
            _startAnimation.Join(_highScore.transform
                .DOMove(new Vector3(highScorePosition.x, highScorePosition.y, highScorePosition.z), delay)
                .SetEase(Ease.OutBack));
            _startAnimation.Join(_highStage.transform
                .DOMove(new Vector3(highStagePosition.x, highStagePosition.y, highStagePosition.z), delay)
                .SetEase(Ease.OutBack));
            _startAnimation.Join(_settingsButton.transform
                .DOScale(Vector3.one, 0.9f)
                .SetEase(Ease.OutBack));
            _startAnimation.Join(_appleImage.transform
                .DOScale(Vector3.one, 0.9f)
                .SetEase(Ease.OutBack));
            _startAnimation.OnComplete(()=> _startAnimationBlocker.SetActive(false));
            _startAnimation.Play();
        }

        private void Update()
        {
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