namespace Managers
{
    public class ScoreManager
    {
        public ScoreManager(DataManager dataManager)
        {
            _dataManager = dataManager;
        }
        public bool IsGameOver { get; set; }

        public int Score
        {
            get => _score;
            set
            {
                _score = value;
                if (_score > _dataManager.HighScore)
                {
                    _dataManager.HighScore = _score;
                }
            }
        }

        public int Stage
        {
            get => _stage;
            set
            {
                _stage = value;
                if (_stage > _dataManager.HighStage)
                {
                    _dataManager.HighStage = _stage;
                }
            }
        }

        private readonly DataManager _dataManager;
        private int _score;
        private int _stage;

    
    }
}