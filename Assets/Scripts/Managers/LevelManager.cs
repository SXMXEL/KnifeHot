﻿using System.Collections;
using System.Linq;
using DG.Tweening;
using Items;
using ScriptableObjects;
using UI;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Managers
{
    public class LevelManager : MonoBehaviour
    {
        public bool IsInitialized;
        public string BossName;
        private int _totalSpawnKnife { get; set; }

        [SerializeField] private KnifeCounter _knifeCounter;
        [SerializeField] private LevelDataHolder _levelsDataHolder;
        [SerializeField] private ObstacleKnifeFactory _obstacleKnifeFactory;
        [SerializeField] private AppleFactory _appleFactory;
        [SerializeField] private KnifeFactory _knifeFactory;
        [SerializeField] private Knife _knifePrefab;
        [SerializeField] private Level _levelPrefab;

        [Header("Wheel settings")] [SerializeField]
        private Transform _wheelSpawnPosition;

        [Range(0, 1)] [SerializeField] private float _wheelScale;

        [Header("Knife settings")] [SerializeField]
        private Transform _knifeSpawnPosition;

        [Range(0, 1)] [SerializeField] private float _knifeScale;

        private LevelData _currentLevelData;
        private bool _isNextLevelInit;
        private Level _currentLevel;
        private Knife _currentKnife;
        private Sequence _rotateSequence;
        private Sequence _bossFightSequence;
        private Sequence _bossDefeatedSequence;

        private float _screenWidth => _menuUIManager.ScreenWidth;
        private float _screenHeight => _menuUIManager.ScreenHeight;
        private Knife _selectedKnifePrefab => _menuUIManager.ShopPage.SelectedKnifePrefab;
        private DataManager _dataManager;
        private SoundManager _soundManager;
        private ScoreManager _scoreManager;
        private MenuUIManager _menuUIManager;
        private GameUIManager _gameUIManager;
        private PageManager _pageManager;

        public void Init(
            DataManager dataManager,
            SoundManager soundManager,
            ScoreManager scoreManager,
            MenuUIManager menuUIManager,
            GameUIManager gameUIManager,
            PageManager pageManager)
        {
            _dataManager = dataManager;
            _soundManager = soundManager;
            _scoreManager = scoreManager;
            _menuUIManager = menuUIManager;
            _gameUIManager = gameUIManager;
            _pageManager = pageManager;
            _knifePrefab.GetComponent<Knife>()
                .Init(
                    _scoreManager,
                    _soundManager,
                    _gameUIManager,
                    _knifeFactory.ReturnKnife);
        }


        private void Update()
        {
            if (_currentKnife == null)
            {
                return;
            }

            if (_currentLevel != null)
            {
                if (_currentLevel.Knives.Count >= _currentLevel.AvailableKnives &&
                    _currentLevel.Knives.All(knife => knife.Hit && _isNextLevelInit == false))
                {
                    NextLevel();
                }

                if (_totalSpawnKnife >= _currentLevel.Knives.Count)
                {
                    _isNextLevelInit = false;
                }
            }
        }


        public void ThrowKnife()
        {
            if (_currentKnife.gameObject.activeInHierarchy && !_currentKnife.IsReleased)
            {
                _currentKnife.Init(
                    _scoreManager,
                    _soundManager,
                    _gameUIManager,
                    _knifeFactory.ReturnKnife);

                _currentKnife.FireKnife();
                if (_currentKnife.IsReleased)
                {
                    _knifeCounter.KnifeHit(_totalSpawnKnife);

                    StartCoroutine(GenerateKnife());
                }
            }
        }

        public void InitializeGame()
        {
            _scoreManager.IsGameOver = false;
            _scoreManager.Score = 0;
            _scoreManager.Stage = 1;
            IsInitialized = true;
            StartCoroutine(GenerateKnife());
            SetupGame();
        }

        private void SetupGame()
        {
            _totalSpawnKnife = 0;
            Debug.Log("Element " + (_scoreManager.Stage - 1));
            _currentLevelData = _levelsDataHolder.GetLevelData(_scoreManager.Stage - 1);

            LevelBaseData levelBaseData = _currentLevelData.SimpleLevelData;
            if (_currentLevelData.HasBoss)
            {
                levelBaseData = _currentLevelData.BossLevelData;
                var bossLevelData = (BossLevelData) levelBaseData;
            }

            GenerateLevel(levelBaseData);
            _knifeCounter.SetupKnife(_currentLevel.AvailableKnives);
        }


        public void RestartUI()
        {
            _currentLevel.Dispose();
            InitializeGame();
        }


        private IEnumerator GenerateKnife()
        {
            yield return new WaitUntil(() => _knifeSpawnPosition.childCount == 0);
            if (_currentLevel.AvailableKnives > _totalSpawnKnife && !_scoreManager.IsGameOver)
            {
                _totalSpawnKnife++;
                Debug.Log("Generate knife");
                _knifeFactory.Init(_knifeSpawnPosition);
                Knife knife;
                if (_selectedKnifePrefab == null)
                {
                    _knifeFactory.Prefab.GetComponent<SpriteRenderer>().sprite =
                        _knifePrefab.GetComponent<SpriteRenderer>().sprite;
                    knife = _knifeFactory.GetKnife();
                    var velocity = _knifePrefab.Rigidbody.velocity;
                    knife.Rigidbody.velocity = new Vector2(velocity.x, velocity.y);
                    knife.Rigidbody.gravityScale = 0;
                    knife.Collider.offset = new Vector2(knife.Collider.offset.x, _knifePrefab.Collider.offset.y);
                    knife.Collider.size = new Vector2(knife.Collider.size.x, _knifePrefab.Collider.size.y);
                }
                else
                {
                    _knifeFactory.Prefab.GetComponent<SpriteRenderer>().sprite =
                        _selectedKnifePrefab.GetComponent<SpriteRenderer>().sprite;
                    knife = _knifeFactory.GetKnife();
                    var velocity = _selectedKnifePrefab.Rigidbody.velocity;
                    knife.Rigidbody.velocity = new Vector2(velocity.x, velocity.y);
                    knife.Rigidbody.gravityScale = 0;
                    knife.Collider.offset =
                        new Vector2(knife.Collider.offset.x, _selectedKnifePrefab.Collider.offset.y);
                    knife.Collider.size = new Vector2(knife.Collider.size.x, _selectedKnifePrefab.Collider.size.y);
                }

                if (knife.Rigidbody.bodyType != RigidbodyType2D.Dynamic && knife.Hit)
                {
                    knife.Rigidbody.bodyType = RigidbodyType2D.Dynamic;
                    knife.Hit = false;
                }


                var knifeTransform = knife.transform;
                knifeTransform.SetParent(_knifeSpawnPosition);
                knifeTransform.position = _knifeSpawnPosition.position;
                knife.gameObject.SetActive(true);
                float knifeScaleInTheScreen = _screenHeight * _knifeScale /
                                              knife.GetComponent<SpriteRenderer>().bounds.size.x;
                knife.transform.localScale = Vector3.one * knifeScaleInTheScreen;
                knife.IsReleased = false;
                _currentKnife = knife;
            }
        }

        private void NextLevel()
        {
            if (_isNextLevelInit)
            {
                return;
            }

            _soundManager.VibrateVictory();
            Debug.Log("Next level");

            if (_scoreManager.Stage % 5 == 0)
            {
                _scoreManager.Stage++;
                _isNextLevelInit = true;
                BossDefeated();
            }
            else
            {
                _scoreManager.Stage++;
                if (_scoreManager.Stage % 5 == 0)
                {
                    _isNextLevelInit = true;
                    Debug.Log("BossFight");
                    BossFight();
                }
                else
                {
                    _isNextLevelInit = true;
                    SetupGame();
                }
            }
        }

        private void BossFight()
        {
            _currentLevel.gameObject.SetActive(false);
            _gameUIManager.BossFightObject.SetActive(true);
            _soundManager.PlayBossFightStart();
            _bossFightSequence?.Kill();
            _bossFightSequence = DOTween.Sequence();
            _bossFightSequence.AppendCallback(() => _gameUIManager.BossFightRotate());
            _bossFightSequence.OnComplete(() =>
            {
                _currentLevel.gameObject.SetActive(true);
                SetupGame();
            });
            _bossFightSequence.Play();
            _gameUIManager.BossFightObject.SetActive(false);
        }

        private void BossDefeated()
        {
            _currentLevel.gameObject.SetActive(false);
            _gameUIManager.BossDefeatedObject.SetActive(true);
            _soundManager.PlayBossFightEnd();
            _bossDefeatedSequence?.Kill();
            _bossDefeatedSequence = DOTween.Sequence();
            _bossDefeatedSequence.Append(_gameUIManager.DefeatedSequence);
            _bossDefeatedSequence.AppendCallback(() => _gameUIManager.BossDefeatedRotate());
            _bossDefeatedSequence.OnComplete(() =>
            {
                _currentLevel.gameObject.SetActive(true);
                SetupGame();
            });
            _gameUIManager.BossDefeatedObject.SetActive(false);
            _bossDefeatedSequence.Play();
        }

        private void GenerateLevel(LevelBaseData levelBaseData)
        {
            if (_currentLevel == null)
            {
                Debug.Log("First level");
                _currentLevel = Instantiate(_levelPrefab, _wheelSpawnPosition);
            }
            else
            {
                if (_scoreManager.Stage % 5 == 0)
                {
                    levelBaseData = (BossLevelData) levelBaseData;
                }

                _currentLevel.Dispose();
                Debug.Log("Current level: " + _scoreManager.Stage);
            }

            _currentLevel.Init(_scoreManager, levelBaseData);

            if (_currentLevel.AppleChance >= Random.value)
            {
                _appleFactory.Init(_currentLevel.transform);
                _currentLevel.SpawnApple(_dataManager, _soundManager, _appleFactory);
            }

            _knifeFactory.Init(_currentLevel.transform);
            _currentLevel.SpawnKnives(_obstacleKnifeFactory);
            _currentLevel.name = "Level " + _scoreManager.Stage;

            // while (_scoreManager.IsGameOver == false)
            // {
            //     
            //     var rotationIndex = 0;
            if (_currentLevel.gameObject.activeInHierarchy)
            {
                _rotateSequence?.Kill();
                _rotateSequence = DOTween.Sequence();
                _rotateSequence.Append(
                        _currentLevel.gameObject.transform.DORotate(
                            new Vector3(0, 0, 180) * levelBaseData.RotationPattern[0].SpeedAndDirection,
                            levelBaseData.RotationPattern[0].Duration, RotateMode.FastBeyond360))
                    .SetLoops(-1, LoopType.Incremental)
                    .SetEase(levelBaseData.RotationPattern[0].Ease);
                _rotateSequence.SetLoops(-1);
                _rotateSequence.Play();
            }

            // if (_)
            // {
            //     rotationIndex++;
            //     rotationIndex = rotationIndex < levelBaseData.RrotationPattern.Length ? rotationIndex : 0;
            // }
            // }
        }
    }
}