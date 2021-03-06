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

        [HideInInspector] public string BossName;
        private int _totalSpawnKnives { get; set; }
        [SerializeField] private KnifeCounter _knifeCounter;
        [SerializeField] private LevelDataHolder _levelsDataHolder;
        [SerializeField] private ObstacleKnifeFactory _obstacleKnifeFactory;
        [SerializeField] private AppleFactory _appleFactory;
        [SerializeField] private KnifeFactory _knifeFactory;
        [SerializeField] private Knife _knifePrefab;
        [SerializeField] private Level _levelPrefab;
        [SerializeField] private GameObject _blocker;
        [SerializeField] private GameObject _fireBlocker;

        [Header("Wheel settings")] [SerializeField]
        private Transform _wheelSpawnPosition;

        [Header("Knife settings")] [SerializeField]
        private Transform _knifeSpawnPosition;

        [Range(0, 1)] [SerializeField] private float _knifeScale;
        [SerializeField] private float _knifeFireDelay;

        private LevelData _currentLevelData;
        private bool _isNextLevelInit;
        private Level _currentLevel;
        private Knife _currentKnife;
        private Sequence _levelScaleSequence;
        private Sequence _bossFightSequence;
        private Sequence _bossDefeatedSequence;
        private Sequence _knifeScaleSequence;

        private float _screenHeight => _menuPage.ScreenHeight;
        private Knife _selectedKnifePrefab => _menuPage.ShopPage.SelectedKnifePrefab;

        private bool _isRestart;
        private DataManager _dataManager;
        private SoundManager _soundManager;
        private VibrationManager _vibrationManager;
        private ScoreManager _scoreManager;
        private MenuPage _menuPage;
        private GamePage _gamePage;

        public void Init(
            DataManager dataManager,
            SoundManager soundManager,
            VibrationManager vibrationManager,
            ScoreManager scoreManager,
            MenuPage menuPage,
            GamePage gamePage)
        {
            _dataManager = dataManager;
            _soundManager = soundManager;
            _vibrationManager = vibrationManager;
            _scoreManager = scoreManager;
            _menuPage = menuPage;
            _gamePage = gamePage;

            if (_isRestart)
            {
                _isRestart = false;
            }

            _knifePrefab.GetComponent<Knife>()
                .Init(
                    _scoreManager,
                    _soundManager,
                    _vibrationManager,
                    _gamePage,
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

                if (_totalSpawnKnives >= _currentLevel.Knives.Count)
                {
                    _isNextLevelInit = false;
                }
            }
        }


        public void ThrowKnife()
        {
            if (_currentKnife.gameObject.activeInHierarchy && !_currentKnife.IsReleased)
            {
                _fireBlocker.SetActive(true);
                new DelayWrappedCommand(() => _fireBlocker.SetActive(false), _knifeFireDelay).Started();

                _currentKnife.Init(
                    _scoreManager,
                    _soundManager,
                    _vibrationManager,
                    _gamePage,
                    _knifeFactory.ReturnKnife);

                _currentKnife.FireKnife();
                if (_currentKnife.IsReleased)
                {
                    _knifeCounter.KnifeHit(_totalSpawnKnives);

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

            SetupGame();
            new DelayWrappedCommand(() => StartCoroutine(GenerateKnife()), 0.2f).Started();
        }

        private void SetupGame()
        {
            _totalSpawnKnives = 0;
            Debug.Log("Element " + (_scoreManager.Stage - 1));
            _currentLevelData = _levelsDataHolder.GetLevelData(_scoreManager.Stage - 1);

            LevelBaseData levelBaseData = _currentLevelData.SimpleLevelData;
            if (_currentLevelData.HasBoss)
            {
                levelBaseData = _currentLevelData.BossLevelData;
                var bossLevelData = (BossLevelData) levelBaseData;
                BossName = "BOSS: " + bossLevelData.Name;
            }

            _gamePage.UpdateUI();
            GenerateLevel(levelBaseData);
        }


        public void StartGame(bool isFirstStart)
        {
            StopCoroutine(GenerateKnife());
            
            if (!isFirstStart)
            {
                _isRestart = true;
            }

            InitializeGame();
        }


        private IEnumerator GenerateKnife()
        {
            yield return new WaitUntil(() =>
                _knifeSpawnPosition.childCount == 0
                && _currentLevel.AvailableKnives > _totalSpawnKnives
                && !_scoreManager.IsGameOver);


            _knifeFactory.Init(_knifeSpawnPosition);
            Knife knife;

            if (_selectedKnifePrefab == null)
            {
                knife = _knifeFactory.GetKnife();
                knife.GetComponent<SpriteRenderer>().sprite =
                    _knifePrefab.GetComponent<SpriteRenderer>().sprite;
                var velocity = _knifePrefab.Rigidbody.velocity;
                knife.Rigidbody.velocity = new Vector2(velocity.x, velocity.y);
                knife.Rigidbody.gravityScale = 0;
                knife.Collider.offset = new Vector2(knife.Collider.offset.x, _knifePrefab.Collider.offset.y);
                knife.Collider.size = new Vector2(knife.Collider.size.x, _knifePrefab.Collider.size.y);
            }
            else
            {
                knife = _knifeFactory.GetKnife();
                knife.GetComponent<SpriteRenderer>().sprite =
                    _selectedKnifePrefab.GetComponent<SpriteRenderer>().sprite;
                var velocity = _selectedKnifePrefab.Rigidbody.velocity;
                knife.Rigidbody.velocity = new Vector2(velocity.x, velocity.y);
                knife.Rigidbody.gravityScale = 0;
                knife.Collider.offset =
                    new Vector2(knife.Collider.offset.x, _selectedKnifePrefab.Collider.offset.y);
                knife.Collider.size = new Vector2(knife.Collider.size.x, _selectedKnifePrefab.Collider.size.y);
            }

            if (knife.Rigidbody.bodyType != RigidbodyType2D.Dynamic)
            {
                knife.Rigidbody.bodyType = RigidbodyType2D.Dynamic;
            }

            if (knife.Hit)
            {
                knife.Hit = false;
            }

            var delay = 0.12f;
            var knifeTransform = knife.transform;
            var knifeSprite = knife.GetComponent<SpriteRenderer>();
            knife.gameObject.SetActive(false);
            _knifeScaleSequence?.Kill();
            _knifeScaleSequence = DOTween.Sequence();
            _knifeScaleSequence.Append(knifeTransform.DOScale(Vector3.zero, 0.1f));
            _knifeScaleSequence.Join(knifeSprite.DOFade(1, 0.1f));
            _knifeScaleSequence.AppendCallback(() =>
            {
                knifeTransform.SetParent(_knifeSpawnPosition);
                knifeTransform.position = _knifeSpawnPosition.position;
                knife.gameObject.SetActive(true);
            });
            _knifeScaleSequence.Append(knifeTransform.DOScale(Vector3.one, delay).SetEase(Ease.OutBack));
            _knifeScaleSequence.Join(knifeSprite.DOFade(255, delay));
            _knifeScaleSequence.Play();
            _knifeScaleSequence.OnComplete(() => _totalSpawnKnives++);
            float knifeScaleInTheScreen = _screenHeight * _knifeScale /
                                          knife.GetComponent<SpriteRenderer>().bounds.size.x;
            knife.transform.localScale = Vector3.one * knifeScaleInTheScreen;
            knife.IsReleased = false;
            _currentKnife = knife;
        }

        private void NextLevel()
        {
            _blocker.SetActive(true);
            new DelayWrappedCommand(() => _blocker.SetActive(false), 3.2f).Started();
            if (_isNextLevelInit)
            {
                return;
            }

            _vibrationManager.CustomVibrate(VibrationSettings.Medium);
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
            _gamePage.BossFightObject.SetActive(true);
            _soundManager.PlayBossFightStart();
            _bossFightSequence?.Kill();
            _bossFightSequence = DOTween.Sequence();
            _bossFightSequence.AppendCallback(() => _gamePage.BossFightRotate());
            _bossFightSequence.OnComplete(() =>
            {
                _currentLevel.gameObject.SetActive(true);
                SetupGame();
            });
            _bossFightSequence.Play();
            _gamePage.BossFightObject.SetActive(false);
        }

        private void BossDefeated()
        {
            _menuPage.ShopPage.ShopItems[_currentLevelData.BossLevelData.BossKnifeIndex
                                         + _menuPage.ShopPage.AppleKnivesCount].IsUnlocked = true;
            _currentLevel.gameObject.SetActive(false);
            _gamePage.BossDefeatedObject.SetActive(true);
            _soundManager.PlayBossFightEnd();
            _bossDefeatedSequence?.Kill();
            _bossDefeatedSequence = DOTween.Sequence();
            _bossDefeatedSequence.AppendCallback(() => _gamePage.BossDefeatedRotate());
            _bossDefeatedSequence.OnComplete(() =>
            {
                _currentLevel.gameObject.SetActive(true);
                SetupGame();
            });
            _gamePage.BossDefeatedObject.SetActive(false);
            _bossDefeatedSequence.Play();
        }

        public void GenerateLevel(LevelBaseData levelBaseData)
        {
            _totalSpawnKnives = 0;
            var delay = 3f;
            if (_currentLevel == null)
            {
                Debug.Log("First level init");
                _currentLevel = Instantiate(_levelPrefab, _wheelSpawnPosition);
            }
            else
            {
                if (_scoreManager.Stage % 5 == 0)
                {
                    levelBaseData = (BossLevelData) levelBaseData;
                }

                _scoreManager.IsGameOver = true;
                new DelayWrappedCommand(() => _scoreManager.IsGameOver = false, 1.5f).Started();
                _currentLevel.Dispose(_isRestart);

                if (_isRestart)
                {
                    _isRestart = false;
                }

                if (_knifeCounter.gameObject.activeSelf)
                {
                    _knifeCounter.gameObject.SetActive(false);
                }

                Debug.Log("Current level: " + _scoreManager.Stage);
            }


            if (_scoreManager.Stage == 1)
            {
                delay = 0.1f;
            }

            new DelayWrappedCommand(() =>
            {
                _currentLevel.Init(_scoreManager, levelBaseData);
                _currentLevel.gameObject.SetActive(false);
                _levelScaleSequence?.Kill();
                _levelScaleSequence = DOTween.Sequence();
                _levelScaleSequence.Append(_currentLevel.transform
                    .DOScale(Vector3.zero, 0.1f));
                _levelScaleSequence.AppendCallback(() => _currentLevel.gameObject.SetActive(true));
                _levelScaleSequence.Append(_currentLevel.transform
                    .DOScale(Vector3.one, 0.3f)
                    .SetEase(Ease.OutBack));
                _levelScaleSequence.AppendCallback(() => _knifeCounter.gameObject.SetActive(true));
                _levelScaleSequence.Play();

                if (_currentLevel.AppleChance >= Random.value)
                {
                    _appleFactory.Init(_currentLevel.transform);
                    new DelayWrappedCommand(
                            () => _currentLevel.SpawnApple(_dataManager, _soundManager, _appleFactory), 0.95f)
                        .Started();
                }

                new DelayWrappedCommand(() => _knifeCounter.SetupKnife(_currentLevel.AvailableKnives), 0.5f).Started();
                _knifeFactory.Init(_currentLevel.transform);
                _currentLevel.SpawnKnives(_obstacleKnifeFactory);
                _currentLevel.name = "Level " + _scoreManager.Stage;
            }, delay).Started();
        }
    }
}