using UnityEngine;
using UnityEngine.EventSystems; // Wajib untuk Drag & Drop UI

public class DragKabel : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Pengaturan Target")]
    [Tooltip("Tarik objek TitikTargetLobang ke sini")]
    public RectTransform areaTarget; 
    
    [Tooltip("Berapa dekat jaraknya sampai kabel otomatis nempel (magnet)")]
    public float jarakMagnet = 70f; 

    private RectTransform rectTransform;
    private Canvas canvas;
    private Vector2 posisiAwal;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        // Mencari komponen Canvas teratas agar geseran mouse akurat dengan resolusi layar
        canvas = GetComponentInParent<Canvas>(); 
    }

    // 1. Saat kabel PERTAMA KALI diklik dan ditarik
    public void OnBeginDrag(PointerEventData eventData)
    {
        posisiAwal = rectTransform.anchoredPosition; // Simpan posisi awal buat jaga-jaga
    }

    // 2. Saat kabel SEDANG ditarik (mengikuti mouse)
    public void OnDrag(PointerEventData eventData)
    {
        // Posisi UI kabel ditambah dengan gerakan mouse (dibagi scaleFactor biar gak meleset ukurannya)
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    // 3. Saat klik DILEPAS
    public void OnEndDrag(PointerEventData eventData)
    {
        // Hitung jarak antara kabel dan titik target
        float jarak = Vector2.Distance(rectTransform.anchoredPosition, areaTarget.anchoredPosition);

        if (jarak <= jarakMagnet)
        {
            // Bener! Posisinya udah deket target. Tempelkan!
            rectTransform.anchoredPosition = areaTarget.anchoredPosition;
            Debug.Log("Kabel berhasil masuk ke lubang!");
            
            // Matikan script ini biar kabel gak bisa ditarik-tarik lagi
            this.enabled = false; 

            AksiCrimping scriptTang = FindObjectOfType<AksiCrimping>(true);
            if(scriptTang != null)
            {
                scriptTang.enabled = true; // Nyalakan script tang
            }

            // (Nanti di sini kamu bisa panggil fungsi untuk lanjut, misal memunculkan instruksi klik tang)
        }
        else
        {
            // Salah! Terlalu jauh dari target. Balikin ke tempat semula.
            rectTransform.anchoredPosition = posisiAwal;
        }
    }
}