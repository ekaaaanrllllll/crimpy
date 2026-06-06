using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // Wajib untuk handling swipe/drag
using System.Collections;

public class CableSwipeDown : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Coordinate Settings (DARI INPUT BOS)")]
    public float posY_Ready = 406.02f;   // Posisi Start (Atas)
    public float posY_Locked = 308f;     // Posisi End/Tancap (Bawah)

    [Header("Fine Tuning Feel")]
    [Tooltip("Radius magnet: seberapa dekat dengan target bawah sebelum dia otomatis SNAP")]
    public float jarakMagnet = 20f; 

    private RectTransform rectTransform;
    private Canvas canvas;
    private bool isLocked = false;     // State: Sudah tertancap sempurna?
    
    // FIXED: Diubah ke SlideManager sesuai nama class asli kamu
    private SlideManager slideManager; 

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        
        // Saat Slide dinyalakan, visual kabel HARUS di posisi Ready
        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, posY_Ready);
    }

    void Start()
    {
        // FIXED: Mencari SlideManager asli
        slideManager = FindFirstObjectByType<SlideManager>();
    }

    // --- INTERAKSI DRAG (SWIPE) ---
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isLocked) return; // Kalau udah dicolok, ga usah drag lagi
        Debug.Log("Mulai swipe kabel ke bawah...");
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isLocked) return;

        // Hitung perubahan posisi jari di sumbu Y (dibagi scaleFactor canvas biar akurat)
        float deltaY = eventData.delta.y / canvas.scaleFactor;
        
        // Hitung posisi Y baru
        Vector2 currentAnchoredPos = rectTransform.anchoredPosition;
        float newY = currentAnchoredPos.y + deltaY;

        // --- PAKSA KUNCI ---
        float kunciX = currentAnchoredPos.x;
        newY = Mathf.Clamp(newY, posY_Locked, posY_Ready);

        // TERAPKAN POSISI BARU
        rectTransform.anchoredPosition = new Vector2(kunciX, newY);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isLocked) return;

        // Cek seberapa dekat posisi sekarang dengan target bawah (Locked)
        float currentY = rectTransform.anchoredPosition.y;
        float jarakKeTarget = Mathf.Abs(currentY - posY_Locked);

        if (jarakKeTarget <= jarakMagnet)
        {
            LakukanSnapNLockTuntas();
        }
        else
        {
            LakukanSnapBackKeReady();
        }
    }

    void LakukanSnapNLockTuntas()
    {
        isLocked = true;
        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, posY_Locked);
        Debug.Log("KABEL BERHASIL DITANCAPKAN & DIKUNCI!!");
        
        GetComponent<Image>().raycastTarget = false;
        
        // ====================================================================
        // 🔥 SOLUSI PINTAR: COCOKKAN SCRIPT POWER MANA YANG ADA DI SCENE INI
        // ====================================================================
        
        // 1. Coba cari apakah ini Scene Straight biasa?
        LanTesterPower normalPower = FindFirstObjectByType<LanTesterPower>();
        if (normalPower != null)
        {
            normalPower.AktifkanSwitchInput();
            Debug.Log("CONSOLE: Mengaktifkan Switch Input pada LanTesterPower Biasa.");
        }

        // 2. Coba cari apakah ini Scene Crossover baru?
        LanTesterCrossoverPower crossPower = FindFirstObjectByType<LanTesterCrossoverPower>();
        if (crossPower != null)
        {
            crossPower.AktifkanSwitchInput();
            Debug.Log("CONSOLE: Mengaktifkan Switch Input pada LanTesterCrossoverPower.");
        }
        
        // ====================================================================
    }

    void LakukanSnapBackKeReady()
    {
        Debug.Log("Kabel kurang masuk bos! Ulangi swipe.");
        StartCoroutine(AnimateSnapBack());
    }

    IEnumerator AnimateSnapBack()
    {
        Vector2 currentPos = rectTransform.anchoredPosition;
        Vector2 targetPos = new Vector2(currentPos.x, posY_Ready);
        float elapsed = 0;
        float duration = 0.15f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            rectTransform.anchoredPosition = Vector2.Lerp(currentPos, targetPos, elapsed / duration);
            yield return null;
        }
        rectTransform.anchoredPosition = targetPos;
    }
}