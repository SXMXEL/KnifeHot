using System;
using UnityEngine;
using UnityEngine.Advertisements;

namespace Managers
{
    public class AdManager : MonoBehaviour, IUnityAdsListener
    {
        
        public bool IsTargetPlayStore;
        public bool IsTestAd;
        
        [HideInInspector]
        public ShowResult Result;

        private DataManager _dataManager;
        
        private string _playStoreID = "4014903";
        private string _appStoreID = "4014902";

        private string _interstitialAd = "video";
        private string _rewardedVideoAd = "rewardedVideo";

        public void Init(DataManager dataManager)
        {
            _dataManager = dataManager;
            Advertisement.AddListener(this);
            InitializeAdvertisement();
        }

        private void InitializeAdvertisement()
        {
            if (IsTargetPlayStore)
            {
                Advertisement.Initialize(_playStoreID, IsTestAd);
            }
            else
            {
                Advertisement.Initialize(_appStoreID, IsTestAd);
            }
        }

        public void PlayInterstitialAd()
        {
            if (!Advertisement.IsReady(_interstitialAd))
            {
                return;
            }
            
            Advertisement.Show(_interstitialAd);
        }

        public void PlayRewardedVideoAd()
        {
            if (!Advertisement.IsReady(_rewardedVideoAd))
            {
                return;
            }
            
            Advertisement.Show(_rewardedVideoAd);
        }

        public void OnUnityAdsReady(string placementId)
        {
        }

        public void OnUnityAdsDidError(string message)
        {
            Debug.Log("Ad error");
        }

        public void OnUnityAdsDidStart(string placementId)
        {
            Debug.Log("Ad starts");
            _dataManager.SoundsSettings = false;
        }

        public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
        {
            _dataManager.SoundsSettings = true;
            Result = showResult;
            switch (showResult)
            {
                case ShowResult.Failed:
                    Debug.LogWarning("Ad failed");
                    break;
                case ShowResult.Skipped:
                    Debug.LogWarning("Ad skipped");
                    break;
                case ShowResult.Finished:
                    if (placementId == _rewardedVideoAd)
                    {
                        Debug.Log("Rewarded ad");
                    }
                    else if(placementId == _interstitialAd)
                    {
                        Debug.Log("Interstitial ad");
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(showResult), showResult, null);
            }
        }
    }
}
