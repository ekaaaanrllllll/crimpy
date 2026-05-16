using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // Wajib untuk handling swipe/drag
using System.Collections;

public class CableSwipeDown : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Coordinate Settings (DARI INPUT BOS)")]
    // Koordinat yang kamu kasih tadi
    public float posY_Ready = 406.02f;   // Posisi Start (Atas)
    public float posY_Locked = 308f;     // Posisi End/Tancap (Bawah)

    [Header("Fine Tuning Feel")]
    [Tooltip("Radius magnet: seberapa dekat dengan target bawah sebelum dia otomatis SNAP")]
    public float jarakMagnet = 20f; 

    private RectTransform rectTransform;
    private Canvas canvas;
    private bool isLocked = false;     // State: Sudah tertancap sempurna?
    
    // (Biar script ini bisa manggil popup sukses di SlideManager nanti)
    private SlideManager slideManager; 

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        
        // PENTING: Saat Slide 6 dinyalakan, visual kabel HARUS di posisi Ready
        // (Bahasan transisi fade tadi yang mengurus ini)
        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, posY_Ready);
    }

    void Start()
    {
        slideManager = FindObjectOfType<SlideManager>();
    }

    // --- INTERAKSI DRAG (SWIPE) ---
    
    // Dipanggil saat jari/mouse PERTAMA KALI nempel dan gerak
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isLocked) return; // Kalau udah dicolok, ga usah drag lagi
        Debug.Log("Mulai swipe kabel ke bawah...");
    }

    // Dipanggil TERUS-MENERUS selama jari/mouse gerak
    public void OnDrag(PointerEventData eventData)
    {
        if (isLocked) return;

        // --- INI KUNCINYA (KUNCI VERTIKAL KE BAWAH) ---
        // Hitung perubahan posisi jari di sumbu Y (dibagi scaleFactor canvas biar akurat)
        float deltaY = eventData.delta.y / canvas.scaleFactor;
        
        // Hitung posisi Y baru
        Vector2 currentAnchoredPos = rectTransform.anchoredPosition;
        float newY = currentAnchoredPos.y + deltaY;

        // --- PAKSA KUNCI ---
        // 1. Kunci Sumbu X (Jangan biarkan kabel gerak kiri kanan)
        float kunciX = currentAnchoredPos.x;

        // 2. Kunci Sumbu Y (PENTOKIN Atas & Bawah)
        // newY tidak boleh > posY_Ready (biar ga tembus atas)
        // newY tidak boleh < posY_Locked (biar ga tembus bawah/lubang)
        newY = Mathf.Clamp(newY, posY_Locked, posY_Ready);

        // TERAPKAN POSISI BARU
        rectTransform.anchoredPosition = new Vector2(kunciX, newY);
    }

    // Dipanggil saat jari/mouse DILEPAS
    public void OnEndDrag(PointerEventData eventData)
    {
        if (isLocked) return;

        // Cek seberapa dekat posisi sekarang dengan target bawah (Locked)
        float currentY = rectTransform.anchoredPosition.y;
        float jarakKeTarget = Mathf.Abs(currentY - posY_Locked);

        if (jarakKeTarget <= jarakMagnet)
        {
            // Bener! Masuk area magnet target. SNAP & LOCK!
            LakukanSnapNLockTuntas();
        }
        else
        {
            // Terlalu jauh dilepasnya. SNAP BACK balik ke atas (Ready)
            LakukanSnapBackKeReady();
        }
    }

    void LakukanSnapNLockTuntas()
    {
        isLocked = true;
        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, posY_Locked);
        Debug.Log("KABEL BERHASIL DITANCAPKAN & DIKUNCI!!");
        
        GetComponent<Image>().raycastTarget = false;
        
        // --- TAMBAHKAN KODE INI BOS ---
        if(FindObjectOfType<LanTesterPower>() != null)
        {
            FindObjectOfType<LanTesterPower>().AktifkanSwitchInput();
        }
        // -----------------------------
        
        // if(slideManager != null)
        // {
        //     Invoke("TampilkanPopupManager", 0.5f);
        // }
    }

    // Fungsi Snap balik ke atas karena pemain lepas di tengah jalan
    void LakukanSnapBackKeReady()
    {
        Debug.Log("Kabel kurang masuk bos! Ulangi swipe.");
        // Smooth snap back menggunakan Coroutine agar tidak teleport
        StartCoroutine(AnimateSnapBack());
    }

    IEnumerator AnimateSnapBack()
    {
        Vector2 currentPos = rectTransform.anchoredPosition;
        Vector2 targetPos = new Vector2(currentPos.x, posY_Ready);
        float elapsed = 0;
        float duration = 0.15f; // Cepat

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            rectTransform.anchoredPosition = Vector2.Lerp(currentPos, targetPos, elapsed / duration);
            yield return null;
        }
        rectTransform.anchoredPosition = targetPos;
    }
}