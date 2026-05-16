using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SlideManager : MonoBehaviour
{
    [Header("1. Daftar Slide Materi")]
    public GameObject[] slides; 

    [Header("2. Objek Navigasi")]
    public GameObject playButton;  
    public Button nextButton;      
    public Button prevButton;

    [Header("3. Pengaturan Popup Selesai")]
    public GameObject popupSelesai; 
    public CanvasGroup popupCanvasGroup; 
    
    // --- TAMBAHAN BARU DI SINI BOS ---
    public Transform popupKonten; // Ini khusus untuk membesarkan isi popup (Mascot + Tombol)
    
    public float durasiAnimasi = 0.5f;

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
        for (int i = 0; i < slides.Length; i++)
        {
            if (slides[i] != null) slides[i].SetActive(i == index);
        }

        if (index == 0) 
        {
            if (playButton != null) playButton.SetActive(true);
            if (nextButton != null) nextButton.gameObject.SetActive(false);
            if (prevButton != null) prevButton.gameObject.SetActive(false);
        }
        else 
        {
            if (playButton != null) playButton.SetActive(false);
            if (nextButton != null) nextButton.gameObject.SetActive(true);
            if (prevButton != null) prevButton.gameObject.SetActive(true);
            if (prevButton != null) prevButton.interactable = true;
            if (nextButton != null) nextButton.interactable = (index != slides.Length - 1);
        }
    }

    // ==========================================
    // --- ANIMASI POPUP YANG SUDAH DIPERBAIKI ---
    // ==========================================

    public void TampilkanPopupSelesai()
    {
        if (popupSelesai != null)
        {
            StopAllCoroutines(); 
            StartCoroutine(AnimasiMasukPopup());
        }
    }

    IEnumerator AnimasiMasukPopup()
    {
        popupSelesai.SetActive(true);
        popupCanvasGroup.alpha = 0;
        
        // Yang dikecilin di awal cuma Popup_Konten-nya saja
        if (popupKonten != null) popupKonten.localScale = Vector3.one * 0.7f; 

        float timer = 0;
        while (timer < durasiAnimasi)
        {
            timer += Time.deltaTime;
            float progress = timer / durasiAnimasi;

            // 1. Background Hitam (+seluruh layar) memudar masuk
            popupCanvasGroup.alpha = progress;

            // 2. HANYA Konten (Mascot+Tombol) yang membesar nge-pop
            if (popupKonten != null)
            {
                float scale = Mathf.Lerp(0.7f, 1f, Mathf.Sin(progress * Mathf.PI * 0.5f));
                popupKonten.localScale = Vector3.one * scale;
            }

            yield return null;
        }

        popupCanvasGroup.alpha = 1;
        if (popupKonten != null) popupKonten.localScale = Vector3.one;
    }

    public void PopupTombolNext()
    {
        popupSelesai.SetActive(false);
        NextSlide(); 
    }

    public void PopupTombolRetry()
    {
        popupSelesai.SetActive(false);
        if (slides[currentSlide] != null)
        {
            slides[currentSlide].SetActive(false);
            slides[currentSlide].SetActive(true);
        }
    }

    public void BukaGembokSlideIni() { }
}