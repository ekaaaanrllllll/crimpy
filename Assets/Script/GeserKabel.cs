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

    void Awake()
    {
        rectTransform =
            GetComponent<RectTransform>();

        canvasGroup =
            GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup =
                gameObject.AddComponent<CanvasGroup>();
        }
    }

    // =====================================
    // BEGIN DRAG
    // =====================================

    public void OnBeginDrag(
        PointerEventData eventData)
    {
        posisiAwal =
            rectTransform.anchoredPosition;

        canvasGroup.blocksRaycasts = false;

        // reset warna
        Image image =
            GetComponent<Image>();

        if (image != null)
        {
            image.color = Color.white;
        }
    }

    // =====================================
    // DRAG
    // =====================================

    public void OnDrag(
        PointerEventData eventData)
    {
        rectTransform.anchoredPosition +=
            eventData.delta /
            canvasUtama.scaleFactor;
    }

    // =====================================
    // END DRAG
    // =====================================

    public void OnEndDrag(
        PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        GameObject target =
            eventData.pointerCurrentRaycast.gameObject;

        if (target != null)
        {
            GeserKabel targetKabel =
                target.GetComponent<GeserKabel>();

            if (targetKabel != null &&
                targetKabel != this)
            {
                TukarPosisi(targetKabel);
                return;
            }
        }

        // balik kalau gagal
        rectTransform.anchoredPosition =
            posisiAwal;
    }

    // =====================================
    // TUKAR POSISI
    // =====================================

    void TukarPosisi(
        GeserKabel target)
    {
        Vector2 posisiTarget =
            target.rectTransform.anchoredPosition;

        target.rectTransform.anchoredPosition =
            posisiAwal;

        rectTransform.anchoredPosition =
            posisiTarget;
    }
}