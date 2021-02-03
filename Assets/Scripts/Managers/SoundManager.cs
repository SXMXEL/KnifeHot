using UnityEngine;

namespace Managers
{
    public class SoundManager : MonoBehaviour
    {
        [SerializeField] private AudioClip _buttonClip;
        [SerializeField] private AudioClip _gameOverClip;
        [SerializeField] private AudioClip _appleHitClip;
        [SerializeField] private AudioClip _appleRewardClip;
        [SerializeField] private AudioClip _wheelClip;
        [SerializeField] private AudioClip _knifeClip;
        [SerializeField] private AudioClip _knifeFireClip;
        [SerializeField] private AudioClip _unlockedClip;
        [SerializeField] private AudioClip _fightStartClip;
        [SerializeField] private AudioClip _fightEndClip;
        
        private DataManager _dataManager;


        public void Init(DataManager dataManager)
        {
            _dataManager = dataManager;
        }

        private void PlaySound(AudioClip clip, float vol = 1)
        {
            if (_dataManager.SoundsSettings)
            {
                if (Camera.main != null)
                {
                    AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position, vol);
                }
            }
        }

        public void PlayButton()
        {
            PlaySound(_buttonClip);
        }

        public void PlayAppleHit()
        {
            PlaySound(_appleHitClip);
        }

        public void PlayWheelHit()
        {
            PlaySound(_wheelClip);
        }

        public void PlayKnifeHit()
        {
            PlaySound(_knifeClip);
        }

        public void PlayKnifeFire()
        {
            PlaySound(_knifeFireClip);
        }

        public void PlayUnlock()
        {
            PlaySound(_unlockedClip);
        }

        public void PlayBossFightStart()
        {
            PlaySound(_fightStartClip);
        }

        public void PlayBossFightEnd()
        {
            PlaySound(_fightEndClip);
        }

        public void PlayGameOver()
        {
            PlaySound(_gameOverClip);
        }

        public void PlayAppleReward()
        {
            PlaySound(_appleRewardClip);
        }

        public void Vibrate()
        {
            if (_dataManager.VibrationSettings)
            {
                Handheld.Vibrate();
                // Vibration.VibratePop();
            }
        }
        public void VibrateVictory()
        {
            if (_dataManager.VibrationSettings)
            {
                Handheld.Vibrate();
                // Vibration.VibratePeek();
            }
        }
    }
}