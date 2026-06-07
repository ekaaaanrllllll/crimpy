using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// Ubah nama class menjadi SlideManager23 agar sinkron dengan kebutuhan scene 2-3
public class SlideManager23 : MonoBehaviour
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
    
    [Header("4. Tambahan Animasi")]
    public Transform popupKonten; 
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

        if (playButton != null) playButton.SetActive(index == 0);

        // Aturan Scene 2 & 3: Tombol Next & Prev hanya muncul di slide 1 sampai 3
        if (index >= 1 && index <= 3) 
        {
            if (nextButton != null) 
            {
                nextButton.gameObject.SetActive(true);
                nextButton.interactable = (index != slides.Length - 1); 
            }
            if (prevButton != null) 
            {
                prevButton.gameObject.SetActive(true);
                prevButton.interactable = true;
            }
        }
        else 
        {
            if (nextButton != null) nextButton.gameObject.SetActive(false);
            if (prevButton != null) prevButton.gameObject.SetActive(false);
        }
    }

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
        if (popupKonten != null) popupKonten.localScale = Vector3.one * 0.7f; 

        float timer = 0;
        while (timer < durasiAnimasi)
        {
            timer += Time.deltaTime;
            float progress = timer / durasiAnimasi;
            popupCanvasGroup.alpha = progress;

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