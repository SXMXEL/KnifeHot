using System.Collections.Generic;
using DG.Tweening;
using Managers;
using UnityEngine;

namespace Items
{
    public class Level : MonoBehaviour
    {
        public int AvailableKnives => _availableKnives;
        public float AppleChance => _appleChance;

        [Header("Prefabs")] 
        [SerializeField] private Apple _applePrefab;
        [SerializeField] private Knife _knifePrefab;
        [SerializeField] private ParticleSystem _levelParticle, _levelTR, _levelTL;
        
        private Sequence _rotateSequence;
        private int _availableKnives;
        private float _appleChance;
        private readonly List<float> _appleAngleFromWheel = new List<float> {0, 60, 120, 180};
        private readonly List<float> _knifeAngleFromWheel = new List<float> {30, 90, 150, 210};
        private int _obstacleKnivesCount;


        public List<Knife> Knives;

        private List<Knife> _obstacleKnives = new List<Knife>();
        private List<Apple> _apples = new List<Apple>();

        private SpriteRenderer _renderer;

        private ParticleSystemRenderer _bottom;
        private ParticleSystemRenderer _topLeft;
        private ParticleSystemRenderer _topRight;
        private Sequence _bounceSequence;

        private ScoreManager _scoreManager;
        private LevelBaseData _levelData;
        private SoundManager _soundManager;
        private ObstacleKnifeFactory _obstacleKnifeFactory;
        private AppleFactory _appleFactory;
        private DataManager _dataManager;


        public void Init(
            ScoreManager scoreManager,
            LevelBaseData levelData)
        {
            _scoreManager = scoreManager;
            _levelData = levelData;
            _renderer = GetComponent<SpriteRenderer>();
            _bottom = _levelParticle.GetComponent<ParticleSystemRenderer>();
            _topLeft = _levelTL.GetComponent<ParticleSystemRenderer>();
            _topRight = _levelTR.GetComponent<ParticleSystemRenderer>();
            _availableKnives = _levelData.AvailableKnives;
            _appleChance = _levelData.AppleChance;

            // For custom apples and knives count
            // _appleAngleFromWheel = _levelData.AppleAngleFromWheel;
            // _knifeAngleFromWheel = _levelData.KnifeAngleFromWheel;

            _obstacleKnivesCount = Random.Range(0, 3);

            if (_scoreManager.Stage % 5 == 0)
            {
                _renderer.sprite =
                    Resources.Load<Sprite>("BossSprites/" + (_levelData as BossLevelData)?.Name);
            }
            else if (_scoreManager.Stage < 5 || _scoreManager.Stage > 15)
            {
                _renderer.sprite = Resources.Load<Sprite>("StageSprites/FirstStage");
                _bottom.material = Resources.Load<Material>("LevelsPieces/LI/LIBottom");
                _topLeft.material = Resources.Load<Material>("LevelsPieces/LI/LITL");
                _topRight.material = Resources.Load<Material>("LevelsPieces/LI/LITR");
            }
            else if (_scoreManager.Stage > 5 && _scoreManager.Stage < 10)
            {
                _renderer.sprite = Resources.Load<Sprite>("StageSprites/SecondStage");
                _bottom.material = Resources.Load<Material>("LevelsPieces/LII/LIIBottom");
                _topLeft.material = Resources.Load<Material>("LevelsPieces/LII/LIITL");
                _topRight.material = Resources.Load<Material>("LevelsPieces/LII/LIITR");
            }
            else if (_scoreManager.Stage > 10 && _scoreManager.Stage < 15)
            {
                _renderer.sprite = Resources.Load<Sprite>("StageSprites/ThirdStage");
                _bottom.material = Resources.Load<Material>("LevelsPieces/LIII/LIIIBottom");
                _topLeft.material = Resources.Load<Material>("LevelsPieces/LIII/LIIITL");
                _topRight.material = Resources.Load<Material>("LevelsPieces/LIII/LIIITR");
            }

            if (_renderer.enabled == false)
            {
                _renderer.enabled = true;
            }

            Rotation();
        }

        private void Rotation()
        {
            var rotationIndex = 0;
            RotationLoop();
            void RotationLoop()
            {
                _rotateSequence?.Kill();
                _rotateSequence = DOTween.Sequence();
                _rotateSequence.Append(
                        gameObject.transform.DORotate(
                            new Vector3(0, 0, 180) * 
                            _levelData.RotationPattern[rotationIndex].SpeedAndDirection,
                            _levelData.RotationPattern[rotationIndex].Duration,
                            RotateMode.FastBeyond360))
                    .SetEase(_levelData.RotationPattern[rotationIndex].Ease);
                _rotateSequence.AppendInterval(_levelData.RotationPattern[rotationIndex].HoldDelay);
                _rotateSequence.Play();
                _rotateSequence.OnComplete(() =>
                {
                    rotationIndex++;
                    rotationIndex = rotationIndex < _levelData.RotationPattern.Length ? rotationIndex : 0;
                    RotationLoop();
                });
            }
        }

        public void SpawnKnives(ObstacleKnifeFactory obstacleKnifeFactory)
        {
            _obstacleKnifeFactory = obstacleKnifeFactory;
            for (var index = 0; index < _obstacleKnivesCount; index++)
            {
                var knifeAngle = _knifeAngleFromWheel[index];
                var knifeTmp = _obstacleKnifeFactory.GetKnife();
                knifeTmp.SetAsObstacle();
                knifeTmp.ObstacleInit(_obstacleKnifeFactory.ReturnKnife);
                var knifeTransform = knifeTmp.transform;

                if (knifeTmp.transform.parent != transform)
                {
                    var prefabPosition = _applePrefab.transform.localPosition;
                    knifeTransform.localPosition = new Vector3(prefabPosition.x, prefabPosition.y, prefabPosition.z);
                    knifeTransform.localRotation = Quaternion.identity;

                    knifeTransform.SetParent(transform);
                }

                knifeTmp.Collider.offset = new Vector2(knifeTmp.Collider.offset.x, -0.4f);
                knifeTmp.Collider.size = new Vector2(knifeTmp.Collider.size.x, 1.2f);
                _knifePrefab.Rigidbody.velocity = new Vector2(_knifePrefab.Rigidbody.velocity.x, -2);

                SetRotationFromWheel(transform,
                    knifeTmp.transform, knifeAngle, 0.3f, 180f);
                _obstacleKnives.Add(knifeTmp);
            }
        }

        public void SpawnApple(DataManager dataManager, SoundManager soundManager, AppleFactory appleFactory)
        {
            _soundManager = soundManager;
            _appleFactory = appleFactory;
            _dataManager = dataManager;
            // foreach (var appleAngle in _appleAngleFromWheel)
            // {
            var appleAngle = _appleAngleFromWheel[Random.Range(0, 4)];
            var appleTmp = _appleFactory.GetApple();
            appleTmp.Init(_dataManager, _soundManager, _appleFactory.ReturnApple);
            var tmpAppleTransform = appleTmp.transform;
            if (appleTmp.transform.parent != transform)
            {
                var prefabPosition = _applePrefab.transform.localPosition;
                tmpAppleTransform.localPosition = new Vector3(prefabPosition.x, prefabPosition.y, prefabPosition.z);
                tmpAppleTransform.localRotation = Quaternion.identity;

                appleTmp.transform.SetParent(transform);
            }

            SetRotationFromWheel(transform,
                tmpAppleTransform, appleAngle, 0.4f, 0f);
            _apples.Add(appleTmp);
            // }
        }

        private void SetRotationFromWheel(
            Transform wheel,
            Transform objectToPlace,
            float angle,
            float spaceFromObject,
            float objectRotation)
        {
            Vector2 offset = new Vector2(Mathf.Sin(angle * Mathf.Deg2Rad), Mathf.Cos(angle * Mathf.Deg2Rad))
                             * (wheel.GetComponent<CircleCollider2D>().radius + spaceFromObject);
            objectToPlace.localPosition = (Vector2) wheel.localPosition + offset;
            objectToPlace.localRotation = Quaternion.Euler(0, 0, -angle + objectRotation);
        }

        public void KnifeHit(Knife knife)
        {
            knife.Rigidbody.isKinematic = true;
            knife.Rigidbody.velocity = Vector2.zero;
            knife.transform.SetParent(transform);
            knife.Hit = true;
            Knives.Add(knife);

            StartBounceAnimation();
        }

        private void StartBounceAnimation()
        {
            transform.localPosition = Vector3.zero;
            _bounceSequence?.Kill();
            _bounceSequence = DOTween.Sequence();
            _bounceSequence.Append(transform
                .DOLocalMoveY(0.1f, 0.1f)
                .SetEase(Ease.InBounce));
            _bounceSequence.Append(transform
                .DOLocalMoveY(0f, 0.1f)
                .SetEase(Ease.OutBounce));
            _bounceSequence.Play();
        }

        public void Dispose()
        {
            var pause = 1f;
            if (_scoreManager.Stage % 5 != 0)
            {
                _levelParticle.Play();
            }

            _rotateSequence?.Kill();
            _renderer.enabled = false;
            transform.rotation = Quaternion.identity;
            Debug.Log(gameObject.name + " Dispose");

            foreach (var knife in Knives)
            {
                knife.transform.SetParent(null);
                var bodyType = knife.Rigidbody.bodyType;
                var hitStatus = knife.Hit;
                knife.Rigidbody.bodyType = RigidbodyType2D.Dynamic;
                knife.Hit = false;
                new DelayWrappedCommand(() =>
                {
                    knife.Hit = hitStatus;
                    if (knife.Rigidbody.bodyType != RigidbodyType2D.Kinematic)
                    {
                        knife.Rigidbody.bodyType = RigidbodyType2D.Kinematic;
                    }
                }, pause).Started();
                new DelayWrappedCommand(() => knife.ReturnObject(), 1.1f).Started();
            }

            Knives.Clear();

            foreach (var knife in _obstacleKnives)
            {
                knife.transform.SetParent(null);
                var bodyType = knife.Rigidbody.bodyType;
                var gravity = knife.Rigidbody.gravityScale;
                knife.Rigidbody.bodyType = RigidbodyType2D.Dynamic;
                knife.Rigidbody.gravityScale = 1;
                new DelayWrappedCommand(() =>
                {
                    knife.Rigidbody.bodyType = bodyType;
                    knife.Rigidbody.gravityScale = gravity;
                },pause).Started();
                new DelayWrappedCommand(() => knife.ReturnObstacle(), 1.1f).Started();
            }

            _obstacleKnives.Clear();

            foreach (var apple in _apples)
            {
                apple.ReturnObject();
            }

            _apples.Clear();
            
        }
    }
}