using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class InfoAlatManager : MonoBehaviour
{
    [System.Serializable]
    public struct DataAlat
    {
        public string namaAlat;              
        public GameObject panelPopupUtama;   
        public GameObject halamanPenjelasan; 
        public GameObject halamanVideo;      
        public Button tombolNext;            
        public Button tombolPrev;            
        public VideoPlayer videoPlayer;      
    }

    [Header("Daftar Informasi Alat")]
    public DataAlat[] daftarAlat; 

    [Header("Tombol Global / Slide Utama (Untuk Disembunyikan)")]
    public GameObject tombolNextUtama;  // Tarik objek tombol Next bawaan Canvas/Slide ke sini
    public GameObject tombolPrevUtama;  // Tarik objek tombol Prev bawaan Canvas/Slide ke sini
    public GameObject tombolMenuUtama;  // Tarik objek tombol Menu/Play bawaan Canvas/Slide ke sini

    private int indexAlatAktif = -1; 

    void Start()
    {
        // Pas awal game mulai, pastikan semua popup tersembunyi
        for (int i = 0; i < daftarAlat.Length; i++)
        {
            if (daftarAlat[i].panelPopupUtama != null)
            {
                daftarAlat[i].panelPopupUtama.SetActive(false);
            }
        }
    }

    public void BukaInfoAlat(int index)
    {
        if (index < 0 || index >= daftarAlat.Length) return;

        indexAlatAktif = index;

        if (daftarAlat[indexAlatAktif].panelPopupUtama != null)
        {
            daftarAlat[indexAlatAktif].panelPopupUtama.SetActive(true);
        }

        // --- 1. Sembunyikan Tombol Navigasi Utama ---
        SetStatusTombolUtama(false);

        SetupHalaman(1);
    }

    public void KlikNext()
    {
        SetupHalaman(2); 
    }

    public void KlikPrev()
    {
        SetupHalaman(1); 
    }

    public void TutupInfoAlat()
    {
        if (indexAlatAktif == -1) return;

        if (daftarAlat[indexAlatAktif].videoPlayer != null)
        {
            daftarAlat[indexAlatAktif].videoPlayer.Stop();
        }

        if (daftarAlat[indexAlatAktif].panelPopupUtama != null)
        {
            daftarAlat[indexAlatAktif].panelPopupUtama.SetActive(false);
        }

        // --- 2. Munculkan Kembali Tombol Navigasi Utama ---
        SetStatusTombolUtama(true);

        indexAlatAktif = -1;
    }

    private void SetupHalaman(int nomorHalaman)
    {
        if (indexAlatAktif == -1) return;

        DataAlat alat = daftarAlat[indexAlatAktif];

        if (nomorHalaman == 1)
        {
            if (alat.halamanPenjelasan != null) alat.halamanPenjelasan.SetActive(true);
            if (alat.halamanVideo != null) alat.halamanVideo.SetActive(false);

            if (alat.tombolNext != null) alat.tombolNext.gameObject.SetActive(true);
            if (alat.tombolPrev != null) alat.tombolPrev.gameObject.SetActive(false);

            if (alat.videoPlayer != null) alat.videoPlayer.Stop();
        }
        else if (nomorHalaman == 2)
        {
            if (alat.halamanPenjelasan != null) alat.halamanPenjelasan.SetActive(false);
            if (alat.halamanVideo != null) alat.halamanVideo.SetActive(true);

            if (alat.tombolNext != null) alat.tombolNext.gameObject.SetActive(false);
            if (alat.tombolPrev != null) alat.tombolPrev.gameObject.SetActive(true);

            if (alat.videoPlayer != null)
            {
                alat.videoPlayer.Stop(); 
                alat.videoPlayer.Play();
            }
        }
    }

    // Fungsi pembantu untuk mematikan/menyalakan semua tombol utama sekaligus
    private void SetStatusTombolUtama(bool status)
    {
        if (tombolNextUtama != null) tombolNextUtama.SetActive(status);
        if (tombolPrevUtama != null) tombolPrevUtama.SetActive(status);
        if (tombolMenuUtama != null) tombolMenuUtama.SetActive(status);
    }
}