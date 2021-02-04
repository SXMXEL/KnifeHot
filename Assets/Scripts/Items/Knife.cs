using System;
using Managers;
using UI;
using UnityEngine;

namespace Items
{
    [RequireComponent(typeof(Rigidbody2D),
        typeof(ParticleSystem), typeof(BoxCollider2D))]
    public class Knife : MonoBehaviour
    {
        private Action<Knife> _returnKnife;
        public BoxCollider2D Collider;
        public Rigidbody2D Rigidbody;

        [SerializeField] private float _speed;
        private ParticleSystem _particle;
        public bool IsReleased { get; set; }
        public bool Hit { get; set; }

        private ScoreManager _scoreManager;
        private SoundManager _soundManager;
        private VibrationManager _vibrationManager;
        private GamePage _gamePage;
        private Action<Knife> _returnObstacle;
        private bool _isObstacle;

        public void Init(
            ScoreManager scoreManager,
            SoundManager soundManager,
            VibrationManager vibrationManager,
            GamePage gamePage,
            Action<Knife> returnKnife)
        {
            _scoreManager = scoreManager;
            _soundManager = soundManager;
            _vibrationManager = vibrationManager;
            _gamePage = gamePage;
            _returnKnife = returnKnife;
            
            _isObstacle = false;
        }

        public void ObstacleInit(Action<Knife> returnObstacle)
        {
            _returnObstacle = returnObstacle;
        }

        private void Awake()
        {
            _particle = GetComponent<ParticleSystem>();
            Collider = GetComponent<BoxCollider2D>();
        }

        public void FireKnife()
        {
            if (!IsReleased && !_scoreManager.IsGameOver && gameObject.activeInHierarchy)
            {
                IsReleased = true;
                Rigidbody.AddForce(new Vector2(0, _speed), ForceMode2D.Impulse);
                _soundManager.PlayKnifeFire();
                Rigidbody.gravityScale = 1;
            }
        }


        public void Dispose()
        {
            IsReleased = false;
            Hit = false;
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (_isObstacle)
            {
                return;
            }

            if (other.gameObject.GetComponent<Level>() != null
                && !Hit
                && !_scoreManager.IsGameOver
                && IsReleased)
            {
                _particle.Play();
                _scoreManager.Score++;
                Rigidbody.bodyType = RigidbodyType2D.Kinematic;
                Collider.offset = new Vector2(Collider.offset.x, -0.4f);
                Collider.size = new Vector2(Collider.size.x, 1.2f);
                _soundManager.PlayWheelHit();
                _vibrationManager.CustomVibrate(VibrationSettings.Low);
                _gamePage.UpdateScore();
                other.gameObject.GetComponent<Level>().KnifeHit(this);
            }
            else if (
                other.gameObject.GetComponent<Knife>() != null
                && !Hit
                && !_scoreManager.IsGameOver
            )
            {
                Rigidbody.velocity = new Vector2(Rigidbody.velocity.x, -2);
                // _soundManager.PlayKnifeHit();
                _soundManager.PlayKnifeHit();
                new DelayWrappedCommand(() => _soundManager.PlayGameOver(), 0.3f).Started();
                _vibrationManager.CustomVibrate(VibrationSettings.Heavy);
                new DelayWrappedCommand(ReturnObject, 1f).Started();
                _scoreManager.IsGameOver = true;
                new DelayWrappedCommand(_gamePage.GameOver, 1.5f).Started();
                
            }
        }

        public void SetAsObstacle()
        {
            _isObstacle = true;
            Rigidbody.bodyType = RigidbodyType2D.Kinematic;
            Rigidbody.gravityScale = 0;
        }

        public void ReturnObject()
        {
            _returnKnife.Invoke(this);
        }

        public void ReturnObstacle()
        {
            _returnObstacle.Invoke(this);
        }
    }
}