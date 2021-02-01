﻿using Managers;
using UI;
using UnityEngine;



public class GameController : MonoBehaviour
{
    [Header("Managers")] 
    [SerializeField] private PageManager _pageManager;
    [SerializeField] private LevelManager _levelManager;
    [SerializeField] private SoundManager _soundManager;
    [SerializeField] private MenuPage menuPage;
    [SerializeField] private GamePage gamePage;

    private DataManager _dataManager;
    private ScoreManager _scoreManager;


    private void Awake()
    {
        Init();
    }
    
    private void Init()
    {
        _dataManager = new DataManager();
        _scoreManager = new ScoreManager(_dataManager);
        _pageManager.PageState = PageState.MenuPage;
        _soundManager.Init(_dataManager);
        menuPage.Init(
            _levelManager,
            _dataManager,
            _soundManager,
            _scoreManager,
            _pageManager);
        gamePage.Init(
            _soundManager,
            _scoreManager,
            _levelManager,
            _pageManager);
        _levelManager.Init(
            _dataManager,
            _soundManager,
            _scoreManager,
            menuPage,
            gamePage,
            _pageManager);
    }
}