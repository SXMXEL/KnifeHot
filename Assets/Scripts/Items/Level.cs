﻿using System;
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

        private int _availableKnives;
        private float _appleChance;
        private List<float> _appleAngleFromWheel = new List<float>();
        private List<float> _knifeAngleFromWheel = new List<float>();
        

        public List<Knife> Knives;

        private List<Knife> _obstacleKnives = new List<Knife>();
        private List<Apple> _apples = new List<Apple>();

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

            _availableKnives = _levelData.AvailableKnives;
            _appleChance = _levelData.AppleChance;
            _appleAngleFromWheel = _levelData.AppleAngleFromWheel;
            _knifeAngleFromWheel = _levelData.KnifeAngleFromWheel;
            
            if (_scoreManager.Stage % 5 == 0)
            {
                GetComponent<SpriteRenderer>().sprite = 
                    Resources.Load<Sprite>("BossSprites/" + (_levelData as BossLevelData)?.Name);
            }
            else if (_scoreManager.Stage < 5 || _scoreManager.Stage > 15)
            {
                GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("StageSprites/FirstStage");
            }
            else if (_scoreManager.Stage > 5 && _scoreManager.Stage < 10)
            {
                GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("StageSprites/SecondStage");

            }
            else if (_scoreManager.Stage > 10 && _scoreManager.Stage < 15)
            {
                GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("StageSprites/ThirdStage");
            }

        }

       

        public void SpawnKnives(ObstacleKnifeFactory obstacleKnifeFactory)
        {
            _obstacleKnifeFactory = obstacleKnifeFactory;
            foreach (var knifeAngle in _knifeAngleFromWheel)
            {
                var knifeTmp = _obstacleKnifeFactory.GetKnife();
                knifeTmp.SetAsObstacle();
                knifeTmp.ObstacleInit(_obstacleKnifeFactory.ReturnKnife);
                // var knifeTmp = Instantiate(_knifePrefab, transform, true);
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
                // knifeTmp.transform.SetParent(transform);
                
                SetRotationFromWheel(transform,
                    knifeTmp.transform, knifeAngle, 0.1f, 180f);
                _obstacleKnives.Add(knifeTmp);
            }
        }

        public void SpawnApple(DataManager dataManager,SoundManager soundManager, AppleFactory appleFactory)
        {
            _soundManager = soundManager;
            _appleFactory = appleFactory;
            _dataManager = dataManager;
            foreach (var appleAngle in _appleAngleFromWheel)
            {
                var appleTmp = _appleFactory.GetApple();
                appleTmp.Init(_dataManager,_soundManager, _appleFactory.ReturnApple);
                var tmpAppleTransform = appleTmp.transform;
                if (appleTmp.transform.parent != transform)
                {
                    var prefabPosition = _applePrefab.transform.localPosition;
                    tmpAppleTransform.localPosition = new Vector3(prefabPosition.x, prefabPosition.y, prefabPosition.z);
                    tmpAppleTransform.localRotation = Quaternion.identity;

                    appleTmp.transform.SetParent(transform);
                }
                
                SetRotationFromWheel(transform,
                    tmpAppleTransform, appleAngle, 0.5f, 0f);
                _apples.Add(appleTmp);
            }
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
                .DOLocalMoveY(  0.15f, 0.15f)
                .SetEase(Ease.InBounce));
            _bounceSequence.Append(transform
                .DOLocalMoveY(  0f, 0.15f)
                .SetEase(Ease.OutBounce));
            _bounceSequence.Play();
        }

        public void Dispose()
        {
            transform.rotation = Quaternion.identity;
            Debug.Log(gameObject.name + " Dispose");
            
            foreach (var knife in Knives)
            {
                knife.ReturnObject();
            }

            Knives.Clear();

            foreach (var knife in _obstacleKnives)
            {
                knife.ReturnObstacle();
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