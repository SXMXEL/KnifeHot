using UnityEngine;

namespace Pages
{
    public abstract class Page : MonoBehaviour
    {
        public bool IsActive
        {
            set
            {
                if (value)
                {
                    Show();
                }
                else
                {
                    Hide();
                }
            }
        }

        private void Show()
        {
            gameObject.SetActive(true);
            OnShow();
        }

        protected virtual void OnShow()
        {
        }

        private void Hide()
        {
            gameObject.SetActive(false);
            OnHide();
        }

        protected virtual void OnHide()
        {
        }
    }
}