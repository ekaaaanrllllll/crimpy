using UnityEngine;
using UnityEngine.UI;

// Nama class diubah menjadi SlideManagerOld agar tidak bentrok di Unity Editor
public class SlideManagerOld : MonoBehaviour
{
    [Header("1. Daftar Slide Materi")]
    public GameObject[] slides; 

    [Header("2. Objek Navigasi")]
    public GameObject playButton;  
    public Button nextButton;      
    public Button prevButton;

    private int currentSlide = 0;

    void Start()
    {
        currentSlide = 0;
        ShowSlide(currentSlide); 
    }

    public void StartMateri() 
    { 
        currentSlide = 1; 
        ShowSlide(currentSlide); 
    }

    public void NextSlide() 
    { 
        if (currentSlide < slides.Length - 1) 
        { 
            currentSlide++; 
            ShowSlide(currentSlide); 
        } 
    }

    public void PrevSlide() 
    { 
        if (currentSlide > 0) 
        { 
            currentSlide--; 
            ShowSlide(currentSlide); 
        } 
    }

    // ==========================================
    // LOGIKA TAMPILAN SLIDE & NAVIGASI BIASA
    // ==========================================
    void ShowSlide(int index)
{
    // Aktifkan slide yang terpilih dan matikan sisanya
    for (int i = 0; i < slides.Length; i++)
    {
        if (slides[i] != null) slides[i].SetActive(i == index);
    }

    // 1. Atur Play Button (Hanya muncul di Slide Awal / Index 0)
    if (playButton != null) 
    {
        playButton.SetActive(index == 0);
    }

    // 2. Atur Tombol Next (Hanya muncul jika BUKAN di slide awal, dan belum mentok di slide terakhir)
    if (nextButton != null)
    {
        // Syarat: index harus di atas 0 DAN index harus lebih kecil dari slide terakhir
        nextButton.gameObject.SetActive(index > 0 && index < slides.Length - 1);
    }

    // 3. Atur Tombol Prev (Sembunyikan jika berada di Slide Awal / Index 0)
    if (prevButton != null)
    {
        prevButton.gameObject.SetActive(index > 0);
    }
}
}