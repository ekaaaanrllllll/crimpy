using UnityEngine;
using UnityEngine.EventSystems;

public class PasangRJ45 : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    [Header("Target & Jarak")]
    public RectTransform targetAdapterRJ45; 
    public float jarakSnap = 50f; 

    [Header("Pengaturan Reset (Wajib Diisi)")]
    [SerializeField] private bool gunakanPosisiResetManual = true; // Kita buat default-nya true bos
    [SerializeField] private Vector2 posisiResetManual = new Vector2(-954f, -6.5f); // Sesuai koordinat gambar 2 abang

    private RectTransform rectKabel;
    private Canvas canvas;
    private Vector2 posisiAwal;
    private bool sudahTerpasang = false;

    void Awake()
    {
        rectKabel = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    // 🔥 KUNCI UTAMA: Berjalan otomatis setiap kali slide di-retry / diaktifkan kembali
    void OnEnable()
    {
        ResetKabel();
    }

    // ==========================================
    // --- FUNGSI RESET UNTUK TOMBOL RETRY ---
    // ==========================================
    public void ResetKabel()
    {
        sudahTerpasang = false;
        
        // Pastikan komponen rectKabel sudah terdefinisi
        if (rectKabel == null) rectKabel = GetComponent<RectTransform>();

        if (rectKabel != null)
        {
            // Paksa posisi kabel melompat ke posisi reset manual (kiri luar) tiap kali reset/enable
            if (gunakanPosisiResetManual)
            {
                rectKabel.anchoredPosition = posisiResetManual;
            }
            else
            {
                rectKabel.anchoredPosition = posisiAwal;
            }
        }
        
        Debug.Log("Sistem OnEnable/Retry mendeteksi: Kabel RJ45 berhasil dipulangkan ke kiri luar!");
    }
    // ==========================================

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (sudahTerpasang) return; 
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (sudahTerpasang) return;
        rectKabel.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (sudahTerpasang) return;

        float jarak = Vector2.Distance(rectKabel.position, targetAdapterRJ45.position);

        if (jarak < jarakSnap)
        {
            rectKabel.position = targetAdapterRJ45.position; 
            sudahTerpasang = true;
            Debug.Log("Kabel berhasil masuk ke RJ45!");
            
            if (FindFirstObjectByType<SlideManager23>() != null)
            {
                FindFirstObjectByType<SlideManager23>().TampilkanPopupSelesai();
            }
        }
        else
        {
            // Jika dilepas sembarangan, balikkan ke posisi reset manual
            rectKabel.anchoredPosition = gunakanPosisiResetManual ? posisiResetManual : posisiAwal;
        }
    }
}