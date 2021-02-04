using System;

namespace Managers
{
    public enum VibrationSettings
    {
        Low,
        Medium,
        Heavy
    }

    public class VibrationManager
    {
        private readonly DataManager _dataManager;
        private readonly VibrationAndroid _vibrationAndroid;
    
        public VibrationManager(DataManager dataManager)
        {
            _dataManager = dataManager;
            _vibrationAndroid = new VibrationAndroid();
        }

        public void CustomVibrate(VibrationSettings vibrationSettings)
        {
            if (!_dataManager.VibrationSettings)
            {
                return;
            }
        
            long vibrateMilliseconds;

            switch (vibrationSettings)
            {
                case VibrationSettings.Low:
                    vibrateMilliseconds = 10;
                    break;
                case VibrationSettings.Medium:
                    vibrateMilliseconds = 20;
                    break;
                case VibrationSettings.Heavy:
                    vibrateMilliseconds = 40;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(vibrationSettings), vibrationSettings, null);
            }
        
            _vibrationAndroid.Vibrate(vibrateMilliseconds);
        }
    }
}