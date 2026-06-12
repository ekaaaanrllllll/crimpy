using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SlideManager23 : MonoBehaviour
{
    [Header("1. Daftar Slide Materi")]
    public GameObject[] slides; 

    [Header("2. Objek Navigasi")]
    public GameObject playButton;  
    public Button nextButton;      
    public Button prevButton;

    [Tooltip("Index slide terakhir yang MASIH boleh memunculkan tombol Next. Di slide setelah angka ini, tombol Next akan otomatis HILANG.")]
    [Header("⚠️ Batas Tombol Next (Setel di Inspector!)")]
    public int batasMaksimalNext = 3; 

    [Header("3. Pengaturan Popup Selesai")]
    public GameObject popupSelesai; 
    public CanvasGroup popupCanvasGroup; 
    
    [Header("4. Tambahan Animasi & Jeda")]
    public Transform popupKonten; 
    [Tooltip("Waktu tunggu setelah aktivitas selesai sebelum popup mulai muncul (Detik)")]
    public float jedaSebelumMuncul = 2.0f; 
    [Tooltip("Durasi proses pemudaran/fade-in popup (Detik)")]
    public float durasiAnimasi = 0.6f;     

    private int currentSlide = 0;

    void Start()
    {
        currentSlide = 0;
        ShowSlide(currentSlide); 
        
        if (popupSelesai != null) 
        {
            popupSelesai.SetActive(false);
            if(popupCanvasGroup != null) popupCanvasGroup.alpha = 0;
        }
    }

    public void StartMateri() { currentSlide = 1; ShowSlide(currentSlide); }
    public void NextSlide() { if (currentSlide < slides.Length - 1) { currentSlide++; ShowSlide(currentSlide); } }
    public void PrevSlide() { if (currentSlide > 0) { currentSlide--; ShowSlide(currentSlide); } }

    void ShowSlide(int index)
    {
        // 1. Aktifkan slide yang dipilih, matikan slide lainnya
        for (int i = 0; i < slides.Length; i++)
        {
            if (slides[i] != null) slides[i].SetActive(i == index);
        }

        // 2. Tombol Play Utama hanya muncul di Slide 0
        if (playButton != null) playButton.SetActive(index == 0);

        // 3. Logika Navigasi Berdasarkan Batas yang Diisi di Inspector
        if (index >= 1) 
        {
            // Tombol PREV selalu muncul dari slide 1 sampai akhir materi
            if (prevButton != null) 
            {
                prevButton.gameObject.SetActive(true);
                prevButton.interactable = true;
            }

            if (nextButton != null) 
            {
                // JIKA index saat ini sudah menyentuh atau melewati batas maksimal yang kamu tentukan
                if (index >= batasMaksimalNext)
                {
                    nextButton.gameObject.SetActive(false); // Sembunyikan tombol next!
                }
                else
                {
                    nextButton.gameObject.SetActive(true); // Munculkan tombol next
                    nextButton.interactable = true; 
                }
            }
        }
        else 
        {
            // Jika kembali ke halaman judul (index 0), matikan kedua navigasi
            if (nextButton != null) nextButton.gameObject.SetActive(false);
            if (prevButton != null) prevButton.gameObject.SetActive(false);
        }
    }

    public void TampilkanPopupSelesai()
    {
        if (popupSelesai != null)
        {
            StopAllCoroutines(); 
            StartCoroutine(AlurMunculPopupSmooth());
        }
    }

    IEnumerator AlurMunculPopupSmooth()
    {
        yield return new WaitForSeconds(jedaSebelumMuncul);

        popupSelesai.SetActive(true);
        if (popupCanvasGroup != null) popupCanvasGroup.alpha = 0;
        if (popupKonten != null) popupKonten.localScale = Vector3.one * 0.7f; 

        float timer = 0;
        while (timer < durasiAnimasi)
        {
            timer += Time.deltaTime;
            float progress = timer / durasiAnimasi;
            
            if (popupCanvasGroup != null) popupCanvasGroup.alpha = progress;

            if (popupKonten != null)
            {
                float scale = Mathf.Lerp(0.7f, 1f, Mathf.Sin(progress * Mathf.PI * 0.5f));
                popupKonten.localScale = Vector3.one * scale;
            }
            yield return null;
        }

        if (popupCanvasGroup != null) popupCanvasGroup.alpha = 1;
        if (popupKonten != null) popupKonten.localScale = Vector3.one;
    }

    public void PopupTombolNext()
    {
        popupSelesai.SetActive(false);
        NextSlide(); 
    }

    public void PopupTombolRetry()
    {
        if (popupSelesai != null) popupSelesai.SetActive(false);
        if (popupCanvasGroup != null) popupCanvasGroup.alpha = 0;
        StopAllCoroutines(); 

        LanTesterManager managerLampu = FindFirstObjectByType<LanTesterManager>(FindObjectsInactive.Include);
        if (managerLampu != null) managerLampu.ResetLanTesterManager();

        LanTesterCrossoverManager managerLampuCross = FindFirstObjectByType<LanTesterCrossoverManager>(FindObjectsInactive.Include);
        if (managerLampuCross != null) managerLampuCross.ResetLanTesterCrossoverManager();

        LanTesterPower saklarPower = FindFirstObjectByType<LanTesterPower>(FindObjectsInactive.Include);
        if (saklarPower != null) saklarPower.ResetLanTesterPower();

        TransisiLanTester transisiView = FindFirstObjectByType<TransisiLanTester>(FindObjectsInactive.Include);
        if (transisiView != null) transisiView.ResetTransisiLanTester();

        if (slides[currentSlide] != null)
        {
            slides[currentSlide].SetActive(false);
            slides[currentSlide].SetActive(true);
        }
    }
}