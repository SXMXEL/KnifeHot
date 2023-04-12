using System;
using Managers;
using UI;
using UnityEngine;


public class GameController : MonoBehaviour
{
    [Header("Managers")] 
    [SerializeField] private PageManager _pageManager;
    [SerializeField] private LevelManager _levelManager;
    [SerializeField] private SoundManager _soundManager;
    [SerializeField] private AdManager _adManager;
    [SerializeField] private MenuPage _menuPage;
    [SerializeField] private GamePage _gamePage;
    [SerializeField] private GameOverPage _gameOverPage;
    private NotificationsManager _notificationsManager;
    private DataManager _dataManager;
    private ScoreManager _scoreManager;
    private VibrationManager _vibrationManager;
    

    private void Awake()
    {
        Init();
        
    }
    
    private void Init()
    {
        _notificationsManager = new NotificationsManager();
        _dataManager = new DataManager();
        _scoreManager = new ScoreManager(_dataManager);
        _vibrationManager = new VibrationManager(_dataManager);
        _pageManager.PageState = PageState.MenuPage;
        _soundManager.Init(_dataManager);
        _adManager.Init(_dataManager);
        _menuPage.Init(
            _levelManager,
            _dataManager,
            _soundManager,
            _pageManager,
            _adManager
            );
        _gamePage.Init(
            _scoreManager,
            _levelManager);
        _gameOverPage.Init(
            _scoreManager,
            _soundManager,
            _levelManager,
            _pageManager,
            _menuPage.UpdateHighScore);
        _levelManager.Init(
            _dataManager,
            _soundManager,
            _vibrationManager,
            _scoreManager,
            _menuPage,
            _gamePage,
            _gameOverPage.GameOver);
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            _notificationsManager.ScheduleNotification(DateTime.Now + new TimeSpan(8,0,0));
        }
    }

    private void OnApplicationQuit()
    {
        _notificationsManager.ScheduleNotification(DateTime.Now + new TimeSpan(0,0,10));
    }
}