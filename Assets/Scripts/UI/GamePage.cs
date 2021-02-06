using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class GamePage : MonoBehaviour
    {
        [Header("Settings")] 
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private TextMeshProUGUI _stageText;
        [SerializeField] private Color _stageCompletedColor;
        [SerializeField] private Color _stageNormalColor;
        [SerializeField] private Color _bossFightColor;
        [SerializeField] private List<Image> _stageIcons;

        [Header("Boss")] 
        public GameObject BossFightObject;
        public GameObject BossDefeatedObject;

        [Header("GameOver")] 
        [SerializeField] private TextMeshProUGUI _gameOverScore;
        [SerializeField] private TextMeshProUGUI _gameOverStage;
        [SerializeField] private GameObject _scorePanel;
        [SerializeField] private GameObject _buttonsBar;
        [SerializeField] private GameObject _homeText;

        [Header("Buttons")] 
        [SerializeField] private Button _homeButton;
        [SerializeField] private Button _knifeFireButton;
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _optionsButton;
        [SerializeField] private Button _shopButton;

        [Header("Effects")] 
        [SerializeField] private GameObject[] _bossFightEffects;
        [SerializeField] private GameObject[] _bossDefeatedEffects;
        
        
        private Sequence _gameOverAnimation;
        private Sequence _fightSequence;
        private Sequence _defeatedSequence;
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
            Rotate(_fightSequence, _bossFightEffects, BossFightObject);
        }

        public void BossDefeatedRotate()
        {
            Rotate(_defeatedSequence, _bossDefeatedEffects, BossDefeatedObject);
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
            _gameOverScore.text = _scoreManager.Score.ToString();
            _gameOverStage.text = "Stage " + _scoreManager.Stage;
            _levelManager.RestartUI();
            Debug.Log("game over");
            var delay = 0.5f;
            var scorePanelPosition = _scorePanel.transform.position;
            var buttonsBarPosition = _buttonsBar.transform.position;
            _levelManager.IsInitialized = false;
            _homeButton.gameObject.SetActive(false);
            _homeText.SetActive(false);
            _scorePanel.SetActive(false);
            _restartButton.gameObject.SetActive(false);
            _buttonsBar.SetActive(false);
            _pageManager.PageState = PageState.GameOverPage;
            _gameOverAnimation?.Kill();
            _gameOverAnimation = DOTween.Sequence();
            _gameOverAnimation.Append(_scorePanel.transform
                .DOMove(new Vector3(0,12.5f,0), 0.1f));
            _gameOverAnimation.Join(_buttonsBar.transform
                .DOMove(new Vector3(0,-12.5f,0), 0.1f));
            _gameOverAnimation.AppendCallback(() =>
            {
                _scorePanel.SetActive(true);
                _buttonsBar.SetActive(true);
            });
            _gameOverAnimation.Append(_scorePanel.transform
                .DOMove(scorePanelPosition, delay)
                .SetEase(Ease.OutBack));
            _gameOverAnimation.Join(_buttonsBar.transform
                .DOMove(buttonsBarPosition, delay)
                .SetEase(Ease.OutBack));
            _gameOverAnimation.Append(_restartButton.transform
                .DOScale(Vector3.zero, 0.1f));
            _gameOverAnimation.AppendCallback(() => _restartButton.gameObject.SetActive(true));
            _gameOverAnimation.Append(_restartButton.transform
                .DOScale(Vector3.one, delay)
                .SetEase(Ease.OutBack));
            _gameOverAnimation.AppendCallback(() =>
            {
                _homeButton.gameObject.SetActive(true);
                _homeText.SetActive(true);
            });
            _gameOverAnimation.Play();
        }


        private void Restart()
        {
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
                bossFightIcon.color = _bossFightColor;
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