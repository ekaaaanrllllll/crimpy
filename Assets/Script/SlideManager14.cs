using UnityEngine;
using UnityEngine.UI;

// Ubah nama class menjadi SlideManager14 agar unik dan tidak bentrok
public class SlideManager14 : MonoBehaviour
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

        // Aturan Scene 1 & 4: Selalu muncul jika bukan di awal/akhir slide
        if (nextButton != null) nextButton.gameObject.SetActive(index > 0 && index < slides.Length - 1);
        if (prevButton != null) prevButton.gameObject.SetActive(index > 0);
    }
}