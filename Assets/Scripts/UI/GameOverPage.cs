using System;
using DG.Tweening;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class GameOverPage : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _gameOverScore;
        [SerializeField] private TextMeshProUGUI _gameOverStage;
        [SerializeField] private GameObject _scorePanel;
        [SerializeField] private GameObject _buttonsBar;
        [SerializeField] private GameObject _homeText;
        
        [Header("Buttons")] 
        [SerializeField] private Button _homeButton;
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _optionsButton;
        [SerializeField] private Button _shopButton;
        private Sequence _gameOverAnimation;
        private ScoreManager _scoreManager;
        private LevelManager _levelManager;
        private PageManager _pageManager;
        private Action _updateHighScore;
        private SoundManager _soundManager;


        public void Init(
            ScoreManager scoreManager,
            SoundManager soundManager,
            LevelManager levelManager,
            PageManager pageManager,
            Action updateHighScore)
        {
            _scoreManager = scoreManager;
            _soundManager = soundManager;
            _levelManager = levelManager;
            _pageManager = pageManager;
            _updateHighScore = updateHighScore;
            
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
        }
        
        public void GameOver()
        {
            _updateHighScore.Invoke();
            _gameOverScore.text = _scoreManager.Score.ToString();
            _gameOverStage.text = "Stage " + _scoreManager.Stage;
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
            _levelManager.StartGame(false);
            _pageManager.PageState = PageState.GamePage;
        }
    }
}
