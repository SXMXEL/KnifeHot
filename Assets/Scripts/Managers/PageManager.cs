using UnityEngine;

public enum PageState
{
    MenuPage,
    GamePage,
    GameOverPage,
    SettingsPage,
    ShopPage
}

public class PageManager : MonoBehaviour
{
    [Header("Pages")] [SerializeField] private GameObject _menuPage;
    [SerializeField] private GameObject _gamePage;
    [SerializeField] private GameObject _game;
    [SerializeField] private GameObject _gameOverPage;
    [SerializeField] private GameObject _settingsPage;
    [SerializeField] private Page _shopPage;

    public PageState PageState
    {
        get => _pageState;
        set
        {
            _pageState = value;
            SetPageState();
        }
    }

    private PageState _pageState;
    
    private void SetPageState()
    {
        _menuPage.SetActive(_pageState == PageState.MenuPage);
        _gamePage.SetActive(_pageState == PageState.GamePage);
        _game.SetActive(_pageState == PageState.GamePage);
        _gameOverPage.SetActive(_pageState == PageState.GameOverPage);
        _settingsPage.SetActive(_pageState == PageState.SettingsPage);
        _shopPage.IsActive = (_pageState == PageState.ShopPage);
    }
}