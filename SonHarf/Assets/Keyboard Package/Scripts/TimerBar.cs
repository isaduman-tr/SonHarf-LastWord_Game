using UnityEngine;
using UnityEngine.UI;

public class TimerBar : MonoBehaviour, IStartable, IStoppable
{
    public Image timerImage;
    private float totalTime = 20f;
    private float currentTime;
    private bool isRunning = false;

    public delegate void TimerEndedHandler();
    public event TimerEndedHandler OnTimerEnd; // Süre bitince çağrılacak event

    private Color startColor;  // Başlangıç rengi (Yeşil - #62FF81)
    private Color middleColor; // Orta renk (Sarı - #FFDB51)
    private Color endColor;    // Bitiş rengi (Kırmızı - #FF5151)

    public void Begin()
    {
        InitializeColors();
        Debug.Log("TimerBar başlatıldı");
    }

    void Update()
    {
        if (isRunning && currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            timerImage.fillAmount = currentTime / totalTime;

            // Zamanlayıcının rengini güncelle
            UpdateTimerColor();

            if (currentTime <= 0)
            {
                isRunning = false;
                OnTimerEnd?.Invoke(); // Süre bitince event çalıştır
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
        float halfTime = totalTime / 2; // 10 saniye (orta nokta)
        
        if (currentTime > halfTime)
        {
            // Yeşilden sarıya doğru
            float lerpValue = Mathf.InverseLerp(totalTime, halfTime, currentTime);
            timerImage.color = Color.Lerp(startColor, middleColor, lerpValue);
        }
        else
        {
            // Sarıdan kırmızıya doğru
            float lerpValue = Mathf.InverseLerp(halfTime, 0, currentTime);
            timerImage.color = Color.Lerp(middleColor, endColor, lerpValue);
        }
    }

    public void AddExtraTime(float extraTime)
{
    currentTime += extraTime;
    totalTime += extraTime;
    
    // Güncel fillAmount hesaplaması
    timerImage.fillAmount = currentTime / totalTime;
    
    Debug.Log("Extra time eklendi. Yeni süre: " + currentTime + " / " + totalTime);
}

    public void SetDefaultTime(float defaultTime)
{
    // Yeni default süreyi ayarla
    totalTime = defaultTime;
    // Mevcut zamanı da default değere eşitle
    currentTime = defaultTime;
    timerImage.fillAmount = 1f;
}
    public void StartTimer()
    {
        currentTime = totalTime;
        isRunning = true;
        timerImage.fillAmount = 1f;
        timerImage.color = startColor; // Başlangıç rengini ayarla
        Debug.Log("Timer BAŞLADI: " + currentTime);
    }

    public void StopTimer()
    {
        isRunning = false;
        currentTime = 0; // Zamanı sıfırla
        timerImage.fillAmount = 0;
    }

    public void ResetTimer()
    {
        currentTime = totalTime;
        timerImage.fillAmount = 1f;
        timerImage.color = startColor; // Başlangıç rengine sıfırla
        isRunning = false;
    }

    public float GetRemainingTime()
    {
        return currentTime;
    }

    public void StopGame()
    {
        StopTimer();
        ResetTimer();
    }
}
