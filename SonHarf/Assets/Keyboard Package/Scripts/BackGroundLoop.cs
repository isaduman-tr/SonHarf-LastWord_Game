using UnityEngine;
using UnityEngine.UI;

public class BackGroundLoop : MonoBehaviour
{
    [SerializeField] private RawImage _img;
    [SerializeField] private float _speedMultiplier = 0.00005f;  // Swipe hassasiyeti
    [SerializeField] private float _momentumDamping = 0.99f;   // Momentum azaltma oranı
    [SerializeField] private float _minSpeed = 0.0005f;        // Sürekli olacak minimum hız (duraksama olmaması için)
    [SerializeField] private float _maxSpeed = 0.002f;         // Maksimum hız (Swipe sonrası en yüksek hız)
    [SerializeField] private float _swipeThreshold = 10f;      // **Swipe algılama eşiği** (10 piksel hareket etmeden swipe başlamaz)

    private Vector2 _velocity = Vector2.zero;
    private Vector2 _startTouchPosition;
    private bool _isSwiping = false;
    private bool _validSwipe = false; // **Gerçek bir swipe olup olmadığını belirler**

    void Start()
    {
        // Oyun başladığında rastgele bir yön seç ve başlangıç hızını ata
        Vector2 randomDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        _velocity = randomDirection * _minSpeed; // Sürekli hareket eden yavaş başlangıç hızı
    }

    void Update()
    {
        // Kullanıcı ekrana dokunduğunda
        if (Input.GetMouseButtonDown(0))
        {
            _startTouchPosition = Input.mousePosition;
            _isSwiping = true;
            _validSwipe = false; // **İlk dokunuşta henüz swipe başlamadı**
        }

        // Kullanıcı parmağını kaydırırken
        if (Input.GetMouseButton(0) && _isSwiping)
        {
            Vector2 currentTouchPosition = Input.mousePosition;
            Vector2 deltaPosition = currentTouchPosition - _startTouchPosition;

            // **Swipe hareketinin belirli bir mesafeyi geçmesini bekle**
            if (!_validSwipe && deltaPosition.magnitude > _swipeThreshold)
            {
                _validSwipe = true; // **Artık swipe geçerli**
            }

            if (_validSwipe) // **Sadece geçerli swipe hareketlerini uygula**
            {
                _velocity = new Vector2(-deltaPosition.x, -deltaPosition.y) * _speedMultiplier;

                // Hızın maksimum sınırını aşmasını engelle
                if (_velocity.magnitude > _maxSpeed)
                    _velocity = _velocity.normalized * _maxSpeed;

                _startTouchPosition = currentTouchPosition;
            }
        }

        // Kullanıcı parmağını kaldırdığında
        if (Input.GetMouseButtonUp(0))
        {
            _isSwiping = false;
        }

        // Momentum efekti (Swipe sonrası hız azalarak sabit minimum hıza ulaşsın)
        if (!_isSwiping)
        {
            _velocity *= _momentumDamping; // Hız yavaşça düşer

            // Eğer hız minimum hareket hızından daha düşükse, minimum hıza sabitle
            if (_velocity.magnitude < _minSpeed)
                _velocity = _velocity.normalized * _minSpeed;
        }

        // Sürekli hareketi uygula
        _img.uvRect = new Rect(_img.uvRect.position + _velocity, _img.uvRect.size);
    }
}



