using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Managers
{
    public class ScrollManager : MonoBehaviour
    {
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private GameObject _shopPlate;

        public void Init()
        {
            var eventTrigger = _scrollRect.GetComponent<EventTrigger>();
            eventTrigger.triggers[0].callback.RemoveAllListeners();
            eventTrigger.triggers[0].callback.AddListener(args => { OnPointerDown(); });

            eventTrigger.triggers[1].callback.RemoveAllListeners();
            eventTrigger.triggers[1].callback.AddListener(args =>
            {
                OnPointerUp();
            });
        }
    
        private Vector2 _touchStartPosition;

        private void OnPointerDown()
        {
            _touchStartPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        private void OnPointerUp()
        {
            var currentMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var moveDistance = Vector2.Distance(_touchStartPosition, currentMousePosition)
                               * 2000;
            var isMoveRight = currentMousePosition.x < _touchStartPosition.x;
            var cellSize = _shopPlate.GetComponent<RectTransform>().rect.width;
            
            if (moveDistance < cellSize * 0.8f)
            {
                return;
            }
            
            if (isMoveRight)
            {
                _scrollRect.content
                    .DOMove(_scrollRect.content.position + new Vector3(-300f, 0, 0), 0.3f);
            }
            else
            {
                _scrollRect.content
                    .DOMove(_scrollRect.content.position + new Vector3(300f, 0, 0), 0.3f);
            }
        }
    }
}