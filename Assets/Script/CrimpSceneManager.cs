using UnityEngine;
using System.Collections;

public class CrimpSceneManager : MonoBehaviour
{
    [Header("Panel Canvas Groups")]
    [Tooltip("Tarik objek OverviewView ke sini")]
    public CanvasGroup overviewGroup; 
    
    [Tooltip("Tarik objek CloseUpView ke sini")]
    public CanvasGroup closeUpGroup;  

    [Header("Pengaturan Zoom & Fade")]
    public float durasiTransisi = 0.6f;
    [Tooltip("Seberapa besar gambar lama nge-zoom sebelum menghilang")]
    public float scaleZoomMaksimal = 1.5f; 

    private bool isZoomed = false;

    void Start()
    {
        ResetTampilanAwal();
    }

    // 🔥 OTOMATIS JALAN SAAT TOMBOL RETRY DIKLIK (KARENA SLIDE DI SETACTIVE TRUE)
    void OnEnable()
    {
        ResetTampilanAwal();

        // Cari dan reset script DragKabel yang ada di slide ini
        DragKabel scriptDrag = GetComponentInChildren<DragKabel>(true);
        if (scriptDrag != null) scriptDrag.ResetDragKabel();

        // Cari dan reset script AksiCrimping yang ada di slide ini
        AksiCrimping scriptTang = GetComponentInChildren<AksiCrimping>(true);
        if (scriptTang != null) scriptTang.ResetAksiCrimping();
    }

    void ResetTampilanAwal()
    {
        isZoomed = false;
        StopAllCoroutines(); // Hentikan coroutine transisi jika masih berjalan

        if (overviewGroup != null)
        {
            overviewGroup.gameObject.SetActive(true);
            overviewGroup.alpha = 1f;
            overviewGroup.transform.localScale = Vector3.one; 
        }

        if (closeUpGroup != null)
        {
            closeUpGroup.gameObject.SetActive(false);
            closeUpGroup.alpha = 0f;
        }
    }

    public void RequestZoomToCrimp()
    {
        if (isZoomed) return;
        isZoomed = true;
        StartCoroutine(AnimasiZoomDanMorph());
    }

    IEnumerator AnimasiZoomDanMorph()
    {
        closeUpGroup.gameObject.SetActive(true);
        closeUpGroup.alpha = 0f;

        Vector3 awalScaleOverview = Vector3.one;
        Vector3 targetScaleOverview = new Vector3(scaleZoomMaksimal, scaleZoomMaksimal, 1f);

        float waktu = 0f;

        while (waktu < durasiTransisi)
        {
            waktu += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, waktu / durasiTransisi); 

            overviewGroup.transform.localScale = Vector3.Lerp(awalScaleOverview, targetScaleOverview, t);
            overviewGroup.alpha = Mathf.Lerp(1f, 0f, t);
            closeUpGroup.alpha = Mathf.Lerp(0f, 1f, t);

            yield return null;
        }

        overviewGroup.gameObject.SetActive(false); 
        overviewGroup.alpha = 1f; 
        overviewGroup.transform.localScale = Vector3.one; 
        closeUpGroup.alpha = 1f; 
        
        Debug.Log("Zoom selesai! Sekarang di tampilan Close-Up.");
    }
}