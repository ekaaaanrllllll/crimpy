using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GeserKabel : MonoBehaviour,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler
{
    [Header("ID Kabel")]
    public int idKabel;

    [Header("Canvas")]
    public Canvas canvasUtama;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 posisiAwal;
    private Image imageComponent;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        imageComponent = GetComponent<Image>();

        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        
        // Catat posisi default awal game
        posisiAwal = rectTransform.anchoredPosition;
    }

    // FUNGSI BARU: Dipanggil oleh manager saat pengacakan posisi sukses dilakukan
    public void PerbaruiPosisiAwalSaatIni(Vector2 posisiBaru)
    {
        posisiAwal = posisiBaru;
        if (imageComponent != null)
        {
            imageComponent.color = Color.white;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Menyimpan posisi sebelum digeser (untuk kebutuhan swap/tukar)
        posisiAwal = rectTransform.anchoredPosition;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvasUtama.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        GameObject target = eventData.pointerCurrentRaycast.gameObject;

        if (target != null)
        {
            GeserKabel targetKabel = target.GetComponent<GeserKabel>();

            if (targetKabel != null && targetKabel != this)
            {
                TukarPosisi(targetKabel);
                return;
            }
        }

        // Balik ke posisi acak asalnya jika dilesir di tempat kosong
        rectTransform.anchoredPosition = posisiAwal;
    }

    void TukarPosisi(GeserKabel target)
    {
        Vector2 posisiTarget = target.rectTransform.anchoredPosition;
        
        // Tukar posisi UI
        target.rectTransform.anchoredPosition = posisiAwal;
        rectTransform.anchoredPosition = posisiTarget;

        // Sinkronisasi memori posisi asal masing-masing setelah ditukar bos
        Vector2 tempPosisiAwal = posisiAwal;
        this.posisiAwal = posisiTarget;
        target.posisiAwal = tempPosisiAwal;

        // Beritahu manager utama untuk langsung update preview di panel utama secara real-time
        SusunKabelManager manager = FindFirstObjectByType<SusunKabelManager>();
        if (manager != null)
        {
            manager.PerbaruiSemuaPreviewUtama();
        }
    }
}