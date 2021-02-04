using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    public class KnifeCounter : MonoBehaviour
    {

        [SerializeField] private GameObject _knifeSprite;
        [SerializeField] private Color _knifeReadyColor;
        [SerializeField] private Color _knifeWastedColor;

        public List<GameObject> Icons => _icons;
        private readonly List<GameObject> _icons = new List<GameObject>();
    

        public void SetupKnife(int amount)
        {
            foreach (var icon in _icons)
            {
                Destroy(icon);
            }
        
            _icons.Clear();

            for (var i = 0; i < amount; i++)
            {
                GameObject icon = Instantiate(_knifeSprite, transform);
                icon.GetComponent<Image>().color = _knifeReadyColor;
                _icons.Add(icon);
            }
        }
    
        public void KnifeHit(int amount)
        {
            for (var i = 0; i < _icons.Count; i++)
            {
                _icons[i].GetComponent<Image>().color = i >= amount ? _knifeReadyColor : _knifeWastedColor;
            }
        }
    }
}
