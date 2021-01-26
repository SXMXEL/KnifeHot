using System;
using System.Collections;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class RewardUI : MonoBehaviour
    {
        [SerializeField] private GameObject _rewardPanel;
        [SerializeField] private ParticleSystem _appleParticle;
        [SerializeField] private TextMeshProUGUI _rewardText;
        [SerializeField] private Button _rewardButton;

        private SoundManager _soundManager;

        private DataManager _dataManager;
        private RewardTimeManager _rewardTimeManager;

        public void Init(SoundManager soundManager, DataManager dataManager, RewardTimeManager rewardTimeManager)
        {
            _dataManager = dataManager;
            _soundManager = soundManager;
            _rewardTimeManager = rewardTimeManager;
            _rewardButton.onClick.RemoveAllListeners();
            _rewardButton.onClick.AddListener(RewardPlayer);
        }

        private void Update()
        {
            if (_rewardTimeManager.CanRewardNow())
            {
                _rewardText.text = "Get Reward!";
            }
            else
            {
                TimeSpan timeToReward = _rewardTimeManager.TimeToReward;
                _rewardText.text = $"{timeToReward.Hours:00}:{timeToReward.Minutes:00}:{timeToReward.Seconds:00}";
            }
        }

        private void RewardPlayer()
        {
            if (_rewardTimeManager.CanRewardNow())
            {
                int amount = _rewardTimeManager.GetRandomReward();
                StartCoroutine(Reward());
                _rewardTimeManager.ResetRewardTime();
                _dataManager.TotalApples += amount;
                _soundManager.PlayAppleReward();
            }
        }

        private IEnumerator Reward()
        {
            _rewardPanel.SetActive(true);
            yield return new WaitForSeconds(1);

            Instantiate(_appleParticle);
        
            yield return new WaitForSeconds(3);
            _rewardPanel.SetActive(false);
        }
    }
}
