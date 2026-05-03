using UnityEngine;
using UnityEngine.UI;

public class SlideManager : MonoBehaviour
{
    [Header("1. Daftar Slide Materi")]
    public GameObject[] slides; 

    [Header("2. Objek Navigasi (Tarik dari Hierarchy)")]
    public GameObject playButton;  
    public Button nextButton;      
    public Button prevButton;

    private int currentSlide = 0;

    void Start()
    {
        currentSlide = 0; // Mulai dari halaman judul
        ShowSlide(currentSlide); 
    }

    // Fungsi untuk tombol Play
    public void StartMateri()
    {
        currentSlide = 1; // Lompat ke slide 1 (Materi pertama)
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

    void ShowSlide(int index)
    {
        // 1. Nyala/matikan visual slide
        for (int i = 0; i < slides.Length; i++)
        {
            if (slides[i] != null)
                slides[i].SetActive(i == index);
        }

        // 2. LOGIKA MUNCUL/HILANG TOMBOL
        if (index == 0) 
        {
            // Jika di Slide 0 (Halaman Judul), munculkan Play, sembunyikan panah
            if (playButton != null) playButton.SetActive(true);
            if (nextButton != null) nextButton.gameObject.SetActive(false);
            if (prevButton != null) prevButton.gameObject.SetActive(false);
        }
        else 
        {
            // Jika masuk materi, sembunyikan Play, munculkan panah
            if (playButton != null) playButton.SetActive(false);
            if (nextButton != null) nextButton.gameObject.SetActive(true);
            if (prevButton != null) prevButton.gameObject.SetActive(true);

            // 3. LOGIKA TOMBOL PANAH BEBAS (Tanpa Gembok)
            if (prevButton != null) 
                prevButton.interactable = true; // Prev selalu bisa diklik di materi

            if (nextButton != null) 
            {
                // Tombol Next HANYA mati kalau sudah di slide paling mentok akhir
                if (index == slides.Length - 1) 
                    nextButton.interactable = false; 
                else 
                    nextButton.interactable = true; // Sisanya bebas diklik
            }
        }
    }

    // FUNGSI INI DIBIARKAN KOSONG SAJA AGAR TIDAK ERROR
    // Jadi kalau script puzzle (Urai Kabel / Potong) iseng memanggil fungsi ini,
    // Unity tidak akan ngambek karena tidak ada gembok yang perlu dibuka lagi.
    public void BukaGembokSlideIni()
    {
        // Kosongkan saja karena semua slide sudah terbuka bebas
    }
}