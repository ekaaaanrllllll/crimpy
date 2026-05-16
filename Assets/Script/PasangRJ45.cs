using UnityEngine;
using UnityEngine.EventSystems; // Wajib ditambahkan untuk fungsi Drag UI

public class PasangRJ45 : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    [Header("Target & Jarak")]
    public RectTransform targetAdapterRJ45; // Tempat konektor RJ45-nya
    public float jarakSnap = 50f; // Seberapa dekat sampai kabel otomatis masuk

    private RectTransform rectKabel;
    private Canvas canvas;
    private Vector2 posisiAwal;
    private bool sudahTerpasang = false;

    void Start()
    {
        rectKabel = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        posisiAwal = rectKabel.anchoredPosition; // Simpan posisi awal kabel
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Kalau sudah terpasang, kabel gak bisa ditarik lagi
        if (sudahTerpasang) return; 
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (sudahTerpasang) return;
        // Menggeser kabel mengikuti mouse/jari
        rectKabel.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (sudahTerpasang) return;

        // UBAH BAGIAN INI: Gunakan .position (posisi global/layar) bukan .anchoredPosition
        float jarak = Vector2.Distance(rectKabel.position, targetAdapterRJ45.position);

        if (jarak < jarakSnap)
        {
            // Jika sudah cukup dekat, kunci posisinya
            rectKabel.position = targetAdapterRJ45.position; 
            sudahTerpasang = true;
            Debug.Log("Kabel berhasil masuk ke RJ45!");
            
            // ==========================================
            // --- KODE POPUP DITAMBAHKAN DI SINI BOS ---
            // ==========================================
            if (FindFirstObjectByType<SlideManager>() != null)
            {
                FindFirstObjectByType<SlideManager>().TampilkanPopupSelesai();
            }
            // ==========================================
        }
        else
        {
            // Jika dilepas tapi masih jauh, kembalikan kabel ke posisi awal
            rectKabel.anchoredPosition = posisiAwal;
        }
    }
}