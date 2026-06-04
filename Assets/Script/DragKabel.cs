using UnityEngine;
using UnityEngine.EventSystems;

public class DragKabel : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Pengaturan Target")]
    public RectTransform areaTarget; 
    public float jarakMagnet = 70f; 

    private RectTransform rectTransform;
    private Canvas canvas;
    private Vector2 posisiAwal;
    private Vector2 posisiSemulaSaatStart; 

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>(); 
        posisiSemulaSaatStart = rectTransform.anchoredPosition; 
    }

    public void ResetDragKabel()
    {
        this.enabled = true; 
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = posisiSemulaSaatStart; 
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        posisiAwal = rectTransform.anchoredPosition; 
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        float jarak = Vector2.Distance(rectTransform.anchoredPosition, areaTarget.anchoredPosition);

        if (jarak <= jarakMagnet)
        {
            Debug.Log("Kabel berhasil masuk ke lubang!");
            this.enabled = false; 

            // Gunakan FindFirstObjectByType versi standar yang aman untuk Unity 6
            AksiCrimping scriptTang = Object.FindFirstObjectByType<AksiCrimping>();
            if(scriptTang != null)
            {
                scriptTang.SetKabelMasukTang(); 
                scriptTang.enabled = true; 
            }
        }
        else
        {
            rectTransform.anchoredPosition = posisiAwal;
        }
    }
}