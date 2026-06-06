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

    // Variabel internal untuk mencatat batas gerak dinamis
    private float batasXMinLokal = -9999f;
    private float batasXMaxLokal = 9999f;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        imageComponent = GetComponent<Image>();

        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        
        posisiAwal = rectTransform.anchoredPosition;
    }

    void OnEnable()
    {
        // 🔥 LOGIKA DETEKSI OTOMATIS: Mengambil batas gerak dari manager mana pun yang sedang aktif di scene
        SusunKabelManager managerStraight = FindFirstObjectByType<SusunKabelManager>(FindObjectsInactive.Include);
        if (managerStraight != null)
        {
            batasXMinLokal = SusunKabelManager.BatasXMin;
            batasXMaxLokal = SusunKabelManager.BatasXMax;
            return;
        }

        SusunKabelCrossoverManager managerCross = FindFirstObjectByType<SusunKabelCrossoverManager>(FindObjectsInactive.Include);
        if (managerCross != null)
        {
            batasXMinLokal = SusunKabelCrossoverManager.BatasXMin;
            batasXMaxLokal = SusunKabelCrossoverManager.BatasXMax;
        }
    }

    public void PerbaruiPosisiAwalSaatIni(Vector2 posisiBaru)
    {
        posisiAwal = posisiBaru;
        if (imageComponent != null)
        {
            imageComponent.color = Color.white;
        }

        // Perbarui ulang batas setelah posisi diacak oleh manager
        PerbaruiBatasGerak();
    }

    void PerbaruiBatasGerak()
    {
        if (FindFirstObjectByType<SusunKabelManager>(FindObjectsInactive.Include) != null)
        {
            batasXMinLokal = SusunKabelManager.BatasXMin;
            batasXMaxLokal = SusunKabelManager.BatasXMax;
        }
        else if (FindFirstObjectByType<SusunKabelCrossoverManager>(FindObjectsInactive.Include) != null)
        {
            batasXMinLokal = SusunKabelCrossoverManager.BatasXMin;
            batasXMaxLokal = SusunKabelCrossoverManager.BatasXMax;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        posisiAwal = rectTransform.anchoredPosition;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 posisiBaru = rectTransform.anchoredPosition + (eventData.delta / canvasUtama.scaleFactor);

        // Jaga-jaga jika batas belum terisi, lakukan perbaruan instan
        if (batasXMinLokal == -9999f) PerbaruiBatasGerak();

        // 🔥 FIX: Menggunakan batas lokal yang sudah adaptif terhadap manager yang aktif
        posisiBaru.x = Mathf.Clamp(posisiBaru.x, batasXMinLokal, batasXMaxLokal);

        rectTransform.anchoredPosition = posisiBaru;
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

        rectTransform.anchoredPosition = posisiAwal;
    }

    void TukarPosisi(GeserKabel target)
    {
        Vector2 posisiTarget = target.rectTransform.anchoredPosition;
        
        target.rectTransform.anchoredPosition = posisiAwal;
        rectTransform.anchoredPosition = posisiTarget;

        Vector2 tempPosisiAwal = posisiAwal;
        this.posisiAwal = posisiTarget;
        target.posisiAwal = tempPosisiAwal;

        // 🔥 FIX: Cek secara dinamis manager mana yang ada di scene saat ini untuk memperbarui preview UI
        SusunKabelManager managerStraight = FindFirstObjectByType<SusunKabelManager>();
        if (managerStraight != null)
        {
            managerStraight.PerbaruiSemuaPreviewUtama();
            return;
        }

        SusunKabelCrossoverManager managerCross = FindFirstObjectByType<SusunKabelCrossoverManager>();
        if (managerCross != null)
        {
            managerCross.PerbaruiSemuaPreviewUtama();
        }
    }
}