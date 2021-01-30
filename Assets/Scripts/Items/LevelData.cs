using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Items
{ 
    [Serializable]
    public class RotationElement
    {
        public float SpeedAndDirection;
        public Ease Ease;
        public float Duration;
    }
    
    [Serializable]
    public class LevelData
    {
        public SimpleLevelData SimpleLevelData => _simpleLevelData;
        [SerializeField] private SimpleLevelData _simpleLevelData;
        public BossLevelData BossLevelData => _bossLevelData;
        [SerializeField] private BossLevelData _bossLevelData;
        
        public bool HasBoss => !string.IsNullOrEmpty(_bossLevelData.Name);
    }
    
    [Serializable]
    public class LevelBaseData
    {
        public int AvailableKnives => _availableKnives;
        public float AppleChance => _appleChance;
        // public List<float> AppleAngleFromWheel => _appleAngleFromWheel;
        // public List<float> KnifeAngleFromWheel => _knifeAngleFromWheel;
        public RotationElement[] RotationPattern => _rotationPattern;
        
        [SerializeField] private int _availableKnives;
        [SerializeField] [Range(0, 1)] private float _appleChance;
        [SerializeField] private RotationElement[] _rotationPattern;
        // [SerializeField] private List<float> _appleAngleFromWheel;
        // [SerializeField] private List<float> _knifeAngleFromWheel;
    }

    [Serializable]
    public class SimpleLevelData : LevelBaseData
    {
        
    }
    
    [Serializable]
    public class BossLevelData : LevelBaseData
    {
        public string Name => _name;
        [SerializeField] private string _name;
    }
}