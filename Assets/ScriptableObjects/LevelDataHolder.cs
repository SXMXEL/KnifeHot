using System;
using System.Collections.Generic;
using Items;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "LevelsData", menuName = "ScriptableObjects/LevelDataManager", order = 1)]
    public class LevelDataHolder : ScriptableObject
    {
        [SerializeField] private List<LevelData> _levels;
        
        public LevelData GetLevelData(int indexOfLevel)
        {
            var data = _levels[indexOfLevel];
            return data;
        }
    }
}
