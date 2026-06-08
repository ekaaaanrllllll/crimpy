using UnityEngine;

public class AlurPertemuan4 : MonoBehaviour
{
    [System.Serializable]
    public enum StateMenu
    {
        MateriUtama,
        Pencegahan,
        AnalisisMasalah,
        Perbaikan,
        Quiz
    }

    [Header("Panel Utama")]
    public GameObject panelMateriUtama;
    public GameObject panelPencegahan;
    public GameObject panelAnalisisMasalah;
    public GameObject panelPerbaikan;
    public GameObject panelQuiz;

    [Header("Sub-Slide Analisis Masalah (1-4)")]
    public GameObject[] slideAnalisis; // Masukkan 4 GameObject slide di sini
    private int currentSlideAnalisis = 0;

    private StateMenu currentState;

    void Start()
    {
        // Kondisi awal game: buka Menu Materi Utama
        BukaMateriUtama();
    }

    // --- ALUR MASUK PANEL ---

    public void BukaMateriUtama()
    {
        MatikanSemuaPanel();
        panelMateriUtama.SetActive(true);
        currentState = StateMenu.MateriUtama;
    }

    public void BukaPencegahan()
    {
        MatikanSemuaPanel();
        panelPencegahan.SetActive(true);
        currentState = StateMenu.Pencegahan;
    }

    public void BukaAnalisisMasalah()
    {
        MatikanSemuaPanel();
        panelAnalisisMasalah.SetActive(true);
        currentState = StateMenu.AnalisisMasalah;
        
        // Reset ke slide pertama (Masalah 1)
        currentSlideAnalisis = 0;
        UpdateSlideAnalisis();
    }

    public void BukaPerbaikan()
    {
        MatikanSemuaPanel();
        panelPerbaikan.SetActive(true);
        currentState = StateMenu.Perbaikan;
    }

    public void BukaQuiz()
    {
        MatikanSemuaPanel();
        panelQuiz.SetActive(true);
        currentState = StateMenu.Quiz;
    }

    // --- NAVIGASI SLIDE ANALISIS MASALAH (1-4) ---

    public void NextSlideAnalisis()
    {
        if (currentSlideAnalisis < slideAnalisis.Length - 1)
        {
            currentSlideAnalisis++;
            UpdateSlideAnalisis();
        }
        else
        {
            // Jika sudah di slide terakhir (Slide 4), tombol Next akan membawa ke Quiz
            BukaQuiz();
        }
    }

    private void UpdateSlideAnalisis()
    {
        // Matikan semua slide analisis, lalu nyalakan yang aktif saja
        for (int i = 0; i < slideAnalisis.Length; i++)
        {
            slideAnalisis[i].SetActive(i == currentSlideAnalisis);
        }
    }

    // --- LOGIKA TOMBOL BACK GLOBAL ---

    public void TombolBack()
    {
        switch (currentState)
        {
            case StateMenu.MateriUtama:
                // Jika di Materi Utama, sesuaikan mau back ke Homepage game kamu
                Debug.Log("Kembali ke Homepage Game");
                break;

            case StateMenu.Pencegahan:
            case StateMenu.Perbaikan:
                // Dari Pencegahan atau Perbaikan, Back akan kembali ke Materi Utama
                BukaMateriUtama();
                break;

            case StateMenu.AnalisisMasalah:
                // Jika di dalam Analisis Masalah, tombol Back mundur per slide dulu
                if (currentSlideAnalisis > 0)
                {
                    currentSlideAnalisis--;
                    UpdateSlideAnalisis();
                }
                else
                {
                    // Jika sudah di slide 1, Back baru kembali ke Materi Utama
                    BukaMateriUtama();
                }
                break;

            case StateMenu.Quiz:
                // Dari Quiz, Back kembali ke menu pilihan (Materi Utama)
                BukaMateriUtama();
                break;
        }
    }

    // --- HELPER ---
    private void MatikanSemuaPanel()
    {
        panelMateriUtama.SetActive(false);
        panelPencegahan.SetActive(false);
        panelAnalisisMasalah.SetActive(false);
        panelPerbaikan.SetActive(false);
        panelQuiz.SetActive(false);
    }
}