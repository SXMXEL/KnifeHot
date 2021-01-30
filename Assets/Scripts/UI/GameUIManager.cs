using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Managers;
using Pages;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class GameUIManager : MonoBehaviour
    {
        [Header("UI Settings")] [SerializeField]
        private TextMeshProUGUI _scoreText;

        [SerializeField] private TextMeshProUGUI _stageText;
        [SerializeField] private Color _stageCompletedColor;
        [SerializeField] private Color _stageNormalColor;
        [SerializeField] private Color _bossfightColor;
        [SerializeField] private List<Image> _stageIcons;
        [SerializeField] private GamePage _gamePage;

        [Header("UI Boss")] public GameObject BossFightObject;
        public GameObject BossDefeatedObject;

        [Header("GameOver UI")] [SerializeField]
        private TextMeshProUGUI _gameOverScore;

        [SerializeField] private TextMeshProUGUI _gameOverStage;

        [Header("UI buttons")] [SerializeField]
        private Button _homeButton;

        [SerializeField] private Button _knifeFireButton;
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _optionsButton;
        [SerializeField] private Button _shopButton;

        [Header("Effects")] [SerializeField] private GameObject[] _bossFightEffects;
        [SerializeField] private GameObject[] _bossDefeatedEffects;

        public Sequence FightSequence;
        public Sequence DefeatedSequence;
        private SoundManager _soundManager;
        private ScoreManager _scoreManager;
        private LevelManager _levelManager;
        private PageManager _pageManager;

        public void Init(
            SoundManager soundManager,
            ScoreManager scoreManager,
            LevelManager levelManager,
            PageManager pageManager)
        {
            _soundManager = soundManager;
            _scoreManager = scoreManager;
            _levelManager = levelManager;
            _pageManager = pageManager;
            _gamePage.Init(_levelManager);
            _homeButton.onClick.RemoveAllListeners();
            _homeButton.onClick.AddListener(() =>
            {
                _soundManager.PlayButton();
                _pageManager.PageState = PageState.MenuPage;
            });
            _restartButton.onClick.RemoveAllListeners();
            _restartButton.onClick.AddListener(Restart);
            _optionsButton.onClick.RemoveAllListeners();
            _optionsButton.onClick.AddListener(() =>
            {
                _soundManager.PlayButton();
                _pageManager.PageState = PageState.SettingsPage;
            });
            _shopButton.onClick.RemoveAllListeners();
            _shopButton.onClick.AddListener(() =>
            {
                _soundManager.PlayButton();
                _pageManager.PageState = PageState.ShopPage;
            });
            _knifeFireButton.onClick.RemoveAllListeners();
            _knifeFireButton.onClick.AddListener(_levelManager.ThrowKnife);
        }


        public void UpdateScore()
        {
            _scoreText.text = _scoreManager.Score.ToString();
        }

        public void BossFightRotate()
        {
            Rotate(FightSequence, _bossFightEffects, BossFightObject);
        }

        public void BossDefeatedRotate()
        {
            Rotate(DefeatedSequence, _bossDefeatedEffects, BossDefeatedObject);
        }

        private void Rotate(Sequence sequence, GameObject[] effects, GameObject bossObject)
        {
            bossObject.SetActive(true);
            sequence?.Kill();
            sequence = DOTween.Sequence();
            for (var i = 0; i < effects.Length; i++)
            {
                var effect = effects[i];
                effect.transform.localRotation = Quaternion.identity;
                sequence.Join(
                        effect.transform.DORotate(
                            Vector3.forward * (90 * ((i + 1) % 2 == 0 ? 1 : -1)),
                            3f))
                    .SetLoops(1, LoopType.Incremental)
                    .SetEase(Ease.Linear);
            }

            sequence.OnComplete(() => bossObject.SetActive(false));
            sequence.Play();
        }

        public void GameOver()
        {
            _levelManager.IsInitialized = false;
            _pageManager.PageState = PageState.GameOverPage;
            _gameOverScore.text = _scoreManager.Score.ToString();
            _gameOverStage.text = "Stage " + _scoreManager.Stage;
            Debug.Log("game over");
        }


        private void Restart()
        {
            if (_levelManager.IsInitialized == false)
            {
                _levelManager.RestartUI();
            }

            _pageManager.PageState = PageState.GamePage;
        }


        public void UpdateUI()
        {
            if (_scoreManager.Stage % 5 != 0)
            {
                if (_stageText.color != Color.white)
                {
                    _stageText.color = Color.white;
                }

                _stageText.text = "Stage " + _scoreManager.Stage;
            }

            if (_scoreManager.Stage % 5 == 0)
            {
                foreach (var icon in _stageIcons)
                {
                    icon.gameObject.SetActive(false);
                }
                
                _stageIcons[_stageIcons.Count - 1].color = _stageNormalColor;
                _stageText.text = _levelManager.BossName.ToUpper();
                _stageText.color = Color.red;
                var bossFightIcon = _stageIcons.Last();
                bossFightIcon.gameObject.SetActive(true);
                bossFightIcon.color = _bossfightColor;
            }
            else
            {
                for (int i = 0; i < _stageIcons.Count; i++)
                {
                    if (_stageIcons[i].gameObject.activeSelf == false)
                    {
                        _stageIcons[i].gameObject.SetActive(true);
                    }

                    _stageIcons[i].color = _scoreManager.Stage % 5 <= i ? _stageNormalColor : _stageCompletedColor;
                    
                }
            }
        }
    }
}