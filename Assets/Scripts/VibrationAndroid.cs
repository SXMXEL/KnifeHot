using UnityEngine;

public class VibrationAndroid
{
#if UNITY_ANDROID && !UNITY_EDITOR
    public static AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
    public static AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
    public static AndroidJavaObject vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
#else
    public AndroidJavaClass unityPlayer;
    public AndroidJavaObject currentActivity;
    public AndroidJavaObject vibrator;
#endif

    public void Vibrate()
    {
        if (IsAndroid())
            vibrator.Call("vibrate");
        else
            Handheld.Vibrate();
    }


    public void Vibrate(long milliseconds)
    {
        if (IsAndroid())
            vibrator.Call("vibrate", milliseconds);
        else
            Handheld.Vibrate();
    }

    public void Vibrate(long[] pattern, int repeat)
    {
        if (IsAndroid())
        {
            vibrator.Call("vibrate", pattern, repeat);
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            Handheld.Vibrate();
        }
    }

    public bool HasVibrator()
    {
        return IsAndroid();
    }

    public void Cancel()
    {
        if (IsAndroid())
            vibrator.Call("cancel");
    }

    private static bool IsAndroid()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
	return true;
#else
        return false;
#endif
    }
}