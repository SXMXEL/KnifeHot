using System;
using UnityEngine;

namespace Managers
{
    public class RewardTimeManager : MonoBehaviour
    {
        [SerializeField] private int _hoursToReward;
        [SerializeField] private int _minutesToReward;
        [SerializeField] private int _secondsToReward = 10;

        private int _minReward = 20;
        private int _maxReward = 60;

        private const string NEXT_REWARD = "RewardTime";

        private DateTime _nextRewardTime => GetNextRewardTime();
        public TimeSpan TimeToReward => _nextRewardTime.Subtract(DateTime.Now);

        public bool CanRewardNow()
        {
            return _nextRewardTime <= DateTime.Now;
        }

        public int GetRandomReward()
        {
            return UnityEngine.Random.Range(_minReward, _maxReward);
        }

        public void ResetRewardTime()
        {
            DateTime nextReward = DateTime.Now.Add(new TimeSpan(_hoursToReward, _minutesToReward, _secondsToReward));
            SaveNextRewardTime(nextReward);
        }

        private void SaveNextRewardTime(DateTime time)
        {
            PlayerPrefs.SetString(NEXT_REWARD, time.ToBinary().ToString());
            PlayerPrefs.Save();
        }

        private DateTime GetNextRewardTime()
        {
            string nextReward = PlayerPrefs.GetString(NEXT_REWARD, string.Empty);

            if (!string.IsNullOrEmpty(nextReward))
            {
                return DateTime.FromBinary(Convert.ToInt64(nextReward));
            }
            else
            {
                return DateTime.Now;
            }
        }
    }
}
