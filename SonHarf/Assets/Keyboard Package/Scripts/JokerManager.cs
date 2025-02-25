using System.Collections;
using UnityEngine;

public class JokerManager : MonoBehaviour
{
    public static JokerManager Instance;

    private JokerType _currentJoker = JokerType.None;
    private float _jokerDuration;
    private bool _isJokerActive = false;

    private void Start()
{
    // Oyun başladığında kaydedilmiş joker bilgilerini yükle
    int savedJokerType = PlayerPrefs.GetInt("SelectedJokerType", 0);
    float savedJokerDuration = PlayerPrefs.GetFloat("SelectedJokerDuration", 0f);
    
    SetCurrentJoker((JokerType)savedJokerType, savedJokerDuration);
}

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetCurrentJoker(JokerType type, float duration)
    {
        _currentJoker = type;
        _jokerDuration = duration;
    }

    public void ActivateDoubleScore()
    {
        // Aktif joker tipi DoubleScore, DoubleScore2 veya DoubleScore3 olmalı ve joker henüz aktif olmamalı.
        if ((_currentJoker != JokerType.DoubleScore && _currentJoker != JokerType.DoubleScore2 && _currentJoker != JokerType.DoubleScore3) 
            || _isJokerActive) 
        {
            return;
        }

        float fixedMultiplier = 1f;
        switch (_currentJoker)
        {
            case JokerType.DoubleScore:
                fixedMultiplier = 8f;
                break;
            case JokerType.DoubleScore2:
                fixedMultiplier = 10f;
                break;
            case JokerType.DoubleScore3:
                fixedMultiplier = 15f;
                break;
        }

        PuanManager.Instance.ApplyFixedMultiplier(fixedMultiplier);
        _isJokerActive = true;
        StartCoroutine(ResetMultiplierAfterDelay());
    }

    public void ActivateFreezeTime()
{
    // Joker tipi FreezeTime, FreezeTime2 veya FreezeTime3 değilse veya zaten aktifse çıkış yap.
    if ((_currentJoker != JokerType.FreezeTime && 
         _currentJoker != JokerType.FreezeTime2 && 
         _currentJoker != JokerType.FreezeTime3) || _isJokerActive)
    {
        return;
    }

    float extraTime = 0f;
    switch (_currentJoker)
    {
        case JokerType.FreezeTime:
            extraTime = 10f;
            break;
        case JokerType.FreezeTime2:
            extraTime = 15f;
            break;
        case JokerType.FreezeTime3:
            extraTime = 20f;
            break;
    }

    TimerBar timerBar = Object.FindFirstObjectByType<TimerBar>();
    if (timerBar != null)
    {
        timerBar.AddExtraTime(extraTime);
        Debug.Log($"FreezeTime aktif: {extraTime} saniye eklendi.");
    }
    
    _isJokerActive = true;
    StartCoroutine(ResetJokerStateAfterDelay());
}



public JokerType GetCurrentJoker()
{
    return _currentJoker;
}

private IEnumerator ResetJokerStateAfterDelay()
{
    yield return new WaitForSeconds(_jokerDuration);
    _isJokerActive = false;
}

    private IEnumerator ResetMultiplierAfterDelay()
    {
        yield return new WaitForSeconds(_jokerDuration);
        PuanManager.Instance.ResetMultiplier();
        _isJokerActive = false;
    }

    
}
