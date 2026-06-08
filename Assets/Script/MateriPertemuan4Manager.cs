using UnityEngine;
using UnityEngine.UI;

public class MateriPertemuan4Manager : MonoBehaviour
{
    [Header("Panel Awal / Main Menu")]
    public GameObject panelBackgroundMenu; // Panel awal dengan Start Button

    [Header("Panel Materi Utama")]
    public GameObject panelMateriUtama;

    [Header("Panel Pencegahan & Perbaikan")]
    public GameObject panelPencegahan;
    public GameObject panelPerbaikan;

    [Header("Panel Analisis Masalah (2 Slide)")]
    public GameObject panelAnalisis_Slide1; // Ini GameObject "1" (Ada 3 tombol area klik & Tombol Next)
    public GameObject panelAnalisis_Slide2; // Ini Slide 2 lanjutan dari Analisis Masalah

    [Header("Sub-Panel Cabang RJ45 (2 Slide)")]
    public GameObject panelRJ45_Slide1;  // Panel 1.1
    public GameObject panelRJ45_Slide2;  // Panel 1.1.2

    [Header("Sub-Panel Cabang 1.2 & 1.3 (1 Slide)")]
    public GameObject panelIntiKabel;     // Panel 1.2
    public GameObject panelSusunanKabel;  // Panel 1.3

    void Start()
    {
        // Saat pertama kali game dinyalakan, wajib hanya menampilkan Main Menu awal
        ResetSemuaPanel();
    }

    private void ResetSemuaPanel()
    {
        panelBackgroundMenu.SetActive(true);
        
        // Matikan semua panel lainnya
        panelMateriUtama.SetActive(false);
        panelPencegahan.SetActive(false);
        panelPerbaikan.SetActive(false);
        panelAnalisis_Slide1.SetActive(false);
        panelAnalisis_Slide2.SetActive(false);
        panelRJ45_Slide1.SetActive(false);
        panelRJ45_Slide2.SetActive(false);
        panelIntiKabel.SetActive(false);
        panelSusunanKabel.SetActive(false);
    }

    // ==========================================
    // NAVIGASI MAIN MENU INITIAL
    // ==========================================
    public void KlikStartButton()
    {
        panelBackgroundMenu.SetActive(false);
        panelMateriUtama.SetActive(true);
    }

    // ==========================================
    // NAVIGASI DARI PANEL MATERI UTAMA
    // ==========================================
    public void BukaPanelPencegahan()
    {
        panelMateriUtama.SetActive(false);
        panelPencegahan.SetActive(true);
    }

    public void BukaPanelPerbaikan()
    {
        panelMateriUtama.SetActive(false);
        panelPerbaikan.SetActive(true);
    }

    public void BukaPanelAnalisisMasalah()
    {
        panelMateriUtama.SetActive(false);
        panelAnalisis_Slide1.SetActive(true); // Membuka Slide 1 dari Analisis Masalah
    }

    // Tombol Prev Universal untuk kembali ke Menu Materi Utama
    public void KembaliKeMateriUtama()
    {
        // Matikan panel-panel yang sejajar materi utama
        panelPencegahan.SetActive(false);
        panelPerbaikan.SetActive(false);
        panelAnalisis_Slide1.SetActive(false);
        panelAnalisis_Slide2.SetActive(false);

        // Nyalakan materi utama
        panelMateriUtama.SetActive(true);
    }

    // ==========================================
    // NAVIGASI INTERNAL ANALISIS MASALAH (Slide 1 <-> Slide 2)
    // ==========================================
    public void NextSlideAnalisis()
    {
        panelAnalisis_Slide1.SetActive(false);
        panelAnalisis_Slide2.SetActive(true);
    }

    public void PrevSlideAnalisis()
    {
        panelAnalisis_Slide2.SetActive(false);
        panelAnalisis_Slide1.SetActive(true);
    }

    // ==========================================
    // NAVIGASI DI DALAM AREA KLIK CABANG (DARI SLIDE 1)
    // ==========================================
    public void BukaCabangRJ45()
    {
        panelAnalisis_Slide1.SetActive(false);
        panelRJ45_Slide1.SetActive(true); // Masuk ke 1.1
    }

    public void NextSlideRJ45()
    {
        panelRJ45_Slide1.SetActive(false);
        panelRJ45_Slide2.SetActive(true); // Masuk ke 1.1.2
    }

    public void PrevSlideRJ45()
    {
        panelRJ45_Slide2.SetActive(false);
        panelRJ45_Slide1.SetActive(true); // Balik ke 1.1
    }

    public void BukaCabangIntiKabel()
    {
        panelAnalisis_Slide1.SetActive(false);
        panelIntiKabel.SetActive(true); // Masuk ke 1.2
    }

    public void BukaCabangSusunanKabel()
    {
        panelAnalisis_Slide1.SetActive(false);
        panelSusunanKabel.SetActive(true); // Masuk ke 1.3
    }

    // Tombol Kembali khusus dari sub-cabang untuk balik ke Slide 1 Analisis Masalah
    public void KembaliKeAnalisisSlide1()
    {
        panelRJ45_Slide1.SetActive(false);
        panelRJ45_Slide2.SetActive(false);
        panelIntiKabel.SetActive(false);
        panelSusunanKabel.SetActive(false);

        panelAnalisis_Slide1.SetActive(true);
    }
}