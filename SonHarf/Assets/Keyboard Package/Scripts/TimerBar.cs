using UnityEngine;
using UnityEngine.UI;

public class TimerBar : MonoBehaviour, IStartable, IStoppable
{
    public Image timerImage;
    private float totalTime = 20f;
    private float currentTime;
    private bool isRunning = false;

    public delegate void TimerEndedHandler();
    public event TimerEndedHandler OnTimerEnd;

    private Color startColor;   // #62FF81
    private Color middleColor;  // #FFDB51
    private Color endColor;     // #FF5151

    public void Begin()
    {
        InitializeColors();
        Debug.Log("TimerBar hazırlandı (başlatılmadı)");
    }

    void Update()
    {
        if (isRunning && currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            timerImage.fillAmount = currentTime / totalTime;

            UpdateTimerColor();

            if (currentTime <= 0)
            {
                isRunning = false;
                OnTimerEnd?.Invoke();
            }
        }
    }

    private void InitializeColors()
    {
        ColorUtility.TryParseHtmlString("#62FF81", out startColor);
        ColorUtility.TryParseHtmlString("#FFDB51", out middleColor);
        ColorUtility.TryParseHtmlString("#FF5151", out endColor);
    }

    private void UpdateTimerColor()
    {
        float halfTime = totalTime / 2;

        if (currentTime > halfTime)
        {
            float lerpValue = Mathf.InverseLerp(totalTime, halfTime, currentTime);
            timerImage.color = Color.Lerp(startColor, middleColor, lerpValue);
        }
        else
        {
            float lerpValue = Mathf.InverseLerp(halfTime, 0, currentTime);
            timerImage.color = Color.Lerp(middleColor, endColor, lerpValue);
        }
    }

    public void AddExtraTime(float extraTime)
    {
        currentTime += extraTime;
        totalTime += extraTime;
        timerImage.fillAmount = currentTime / totalTime;

        Debug.Log("Extra time eklendi. Yeni süre: " + currentTime + " / " + totalTime);
    }

    public void SetDefaultTime(float defaultTime)
    {
        totalTime = defaultTime;
        currentTime = defaultTime;
        timerImage.fillAmount = 1f;
    }

    public void StartTimer()
    {
        if (isRunning) return; // Timer zaten çalışıyorsa tekrar başlatma

        currentTime = totalTime;
        isRunning = true;
        timerImage.fillAmount = 1f;
        timerImage.color = startColor;

        Debug.Log("Timer BAŞLADI: " + currentTime);  // Timer başladığında mesajı yazdır
    }

    public void StopTimer()
    {
        isRunning = false;
        currentTime = 0;
        timerImage.fillAmount = 0;
        Debug.Log("Timer DURDURULDU");
    }

    public void ResetTimer()
    {
        currentTime = totalTime;
        timerImage.fillAmount = 1f;
        timerImage.color = startColor;
        isRunning = false;
        Debug.Log("Timer RESETLENDİ");
    }

    public float GetRemainingTime()
    {
        return currentTime;
    }

    public void StopGame()
    {
        StopTimer();   // Süreyi durdur
        ResetTimer();  // Sıfırla ama tekrar başlamasın
        Debug.Log("Oyun durduruldu ve timer sıfırlandı");
    }
}
