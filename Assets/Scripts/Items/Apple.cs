using System;
using System.Collections;
using Managers;
using UnityEngine;

namespace Items
{
    public class Apple : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _appleParticle;
        private BoxCollider2D _boxCollider;
        private SpriteRenderer _spriteRenderer;
        private SoundManager _soundManager;
        private Action<Apple> _returnApple;
        private DataManager _dataManager;

        private void Awake()
        {
            _boxCollider = GetComponent<BoxCollider2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            
        }

        public void Init(
            DataManager dataManager,
            SoundManager soundManager,
            Action<Apple> returnApple)
        {
            _dataManager = dataManager;
            _returnApple = returnApple;
            _soundManager = soundManager;
        }

        public void Dispose()
        {
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.GetComponent<Knife>() == null)
            {
                return;
            }

            _dataManager.TotalApples++;
            _boxCollider.enabled = false;
            _spriteRenderer.enabled = false;
            transform.parent = null;
            _soundManager.PlayAppleHit();
            _appleParticle.Play();
            
            StartCoroutine(ReturnApple());
        }

        private IEnumerator ReturnApple()
        {
            yield return new WaitForSeconds(1.3f);
            gameObject.SetActive(false);
        }

        public void ReturnObject()
        {
            Debug.Log("apple returned");
            // gameObject.SetActive(true);
            _boxCollider.enabled = true;
            _spriteRenderer.enabled = true;
            _returnApple.Invoke(this);
        }
    }
}