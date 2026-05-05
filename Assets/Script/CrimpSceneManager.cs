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
        // Pastikan tampilan awal saat game jalan
        if (overviewGroup != null)
        {
            overviewGroup.gameObject.SetActive(true);
            overviewGroup.alpha = 1f;
            overviewGroup.transform.localScale = Vector3.one; // Ukuran normal (1,1,1)
        }

        if (closeUpGroup != null)
        {
            closeUpGroup.gameObject.SetActive(false);
            closeUpGroup.alpha = 0f;
        }
    }

    // Fungsi ini yang dipanggil oleh tombol area klik kamu
    public void RequestZoomToCrimp()
    {
        if (isZoomed) return;
        isZoomed = true;
        StartCoroutine(AnimasiZoomDanMorph());
    }

    IEnumerator AnimasiZoomDanMorph()
    {
        // 1. Nyalakan panel close-up (tapi masih transparan / alpha 0)
        closeUpGroup.gameObject.SetActive(true);
        closeUpGroup.alpha = 0f;

        Vector3 awalScaleOverview = Vector3.one;
        Vector3 targetScaleOverview = new Vector3(scaleZoomMaksimal, scaleZoomMaksimal, 1f);

        float waktu = 0f;

        // 2. Proses Transisi (Zoom + Fade)
        while (waktu < durasiTransisi)
        {
            waktu += Time.deltaTime;
            // SmoothStep bikin gerakan melambat di akhir, jadi keliatan lebih natural
            float t = Mathf.SmoothStep(0f, 1f, waktu / durasiTransisi); 

            // Efek Overview: Membesar (nge-zoom) dan pelan-pelan menghilang
            overviewGroup.transform.localScale = Vector3.Lerp(awalScaleOverview, targetScaleOverview, t);
            overviewGroup.alpha = Mathf.Lerp(1f, 0f, t);

            // Efek CloseUp: Pelan-pelan muncul
            closeUpGroup.alpha = Mathf.Lerp(0f, 1f, t);

            yield return null;
        }

        // 3. Rapikan setelah transisi selesai
        overviewGroup.gameObject.SetActive(false); // Matikan yang lama
        overviewGroup.alpha = 1f; // Kembalikan ke normal
        overviewGroup.transform.localScale = Vector3.one; // Kembalikan ukurannya

        closeUpGroup.alpha = 1f; // Pastikan yang baru muncul 100%
        
        Debug.Log("Zoom selesai! Sekarang di tampilan Close-Up.");
    }
}