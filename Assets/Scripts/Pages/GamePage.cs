using DG.Tweening;
using Managers;
using UnityEngine;

namespace Pages
{
    public class GamePage : Page
    {
        
        private LevelManager _levelManager;

        public void Init(LevelManager levelManager)
        {
            _levelManager = levelManager;
        }

        protected override void OnShow()
        {
            
        }
    }
}
