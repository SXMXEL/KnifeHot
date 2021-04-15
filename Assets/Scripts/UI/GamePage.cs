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

        [SerializeField] private Button _knifeFireButton;

        [Header("Effects")] 
        [SerializeField] private GameObject[] _bossFightEffects;
        [SerializeField] private GameObject[] _bossDefeatedEffects;

        private Sequence _fightSequence;
        private Sequence _defeatedSequence;
        private ScoreManager _scoreManager;
        private LevelManager _levelManager;

        public void Init(
            ScoreManager scoreManager,
            LevelManager levelManager)
        {
            _scoreManager = scoreManager;
            _levelManager = levelManager;
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