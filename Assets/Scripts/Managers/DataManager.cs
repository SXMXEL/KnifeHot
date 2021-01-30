using UnityEngine;

namespace Managers
{
    public class DataManager
    {
        private const string SELECTED_KNIFE = "Knife";
        private const string HIGH_SCORE = "Highscore";
        private const string HIGH_STAGE = "Highscore";
        private const string TOTAL_APPLES = "TotalApples";
        private const string SOUND_SETTINGS = "SoundSettings";
        private const string VIBRATION_SETTINGS = "VibrationSettings";

        public int SelectedKnifeIndex
        {
            get => PlayerPrefs.GetInt(SELECTED_KNIFE, 0);
            set => PlayerPrefs.SetInt(SELECTED_KNIFE, value);
        }

        public int HighScore
        {
            get => PlayerPrefs.GetInt(HIGH_SCORE, 0);
            set => PlayerPrefs.SetInt(HIGH_SCORE, value);
        }
        public int HighStage
        {
            get => PlayerPrefs.GetInt(HIGH_STAGE, 0);
            set => PlayerPrefs.SetInt(HIGH_STAGE, value);
        }

        public int TotalApples
        {
            get => PlayerPrefs.GetInt(TOTAL_APPLES, 0);
            set => PlayerPrefs.SetInt(TOTAL_APPLES, value);
        }

        public bool SoundsSettings
        {
            get => PlayerPrefs.GetInt(SOUND_SETTINGS, 1) == 1;
            set => PlayerPrefs.SetInt(SOUND_SETTINGS, value ? 1 : 0);
        }

        public bool VibrationSettings
        {
            get => PlayerPrefs.GetInt(VIBRATION_SETTINGS, 1) == 1;
            set => PlayerPrefs.SetInt(VIBRATION_SETTINGS, value ? 1 : 0);
        }
    }
}