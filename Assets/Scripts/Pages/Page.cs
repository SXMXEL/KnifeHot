using UnityEngine;

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

    public void Show()
    {
        gameObject.SetActive(true);
        OnShow();
    }

    protected virtual void OnShow()
    {
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        OnHide();
    }

    protected virtual void OnHide()
    {
    }
}