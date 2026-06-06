using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class SusunKabelCrossoverManager : MonoBehaviour
{
    [Header("Panel")]
    public GameObject panelUtama;
    public GameObject panelZoomKiri;
    public GameObject networkZoomKanan; 

    [Header("Preview Panel Utama")]
    public Image[] previewKiri;
    public Image[] previewKanan;

    [Header("Sprite Kabel")]
    public Sprite[] spriteKabel;

    [Header("Parent Zoom")]
    public Transform parentKabelKiri;
    public Transform parentKabelKanan;

    // 🔥 KUNCI JAWABAN CROSSOVER (T568A di Kiri, T568B di Kanan)
    // Asumsi Urutan ID Kabel (Sama seperti Straight):
    // 0: Putih-Oranye, 1: Oranye, 2: Putih-Hijau, 3: Biru, 4: Putih-Biru, 5: Hijau, 6: Putih-Cokelat, 7: Cokelat
    private readonly int[] urutanBenarKiri = { 2, 5, 0, 3, 4, 1, 6, 7 }; // T568A (Putih-Hijau, Hijau, Putih-Oranye, ...)
    private readonly int[] urutanBenarKanan = { 0, 1, 2, 3, 4, 5, 6, 7 }; // T568B (Putih-Oranye, Oranye, Putih-Hijau, ...)

    public static float BatasXMin { get; private set; }
    public static float BatasXMax { get; private set; }

    void OnEnable()
    {
        if (panelUtama != null) panelUtama.SetActive(true);
        if (panelZoomKiri != null) panelZoomKiri.SetActive(false);
        if (networkZoomKanan != null) networkZoomKanan.SetActive(false);

        // Langsung acak warna kabelnya saat masuk slide
        AcakUrutanKabelWarnaSaja(parentKabelKiri);
        AcakUrutanKabelWarnaSaja(parentKabelKanan);

        PerbaruiSemuaPreviewUtama();
    }

    public void ResetGameKeSemula()
    {
        AcakUrutanKabelWarnaSaja(parentKabelKiri);
        AcakUrutanKabelWarnaSaja(parentKabelKanan);
        PerbaruiSemuaPreviewUtama();
    }

    // =========================================================================
    // LOGIK: HANYA MENGACAK KABEL BERWARNA (KABEL ATAS & BAWAH TIDAK IKUT)
    // =========================================================================
    void AcakUrutanKabelWarnaSaja(Transform parentZoom)
    {
        if (parentZoom == null) return;

        GeserKabel[] semuaKabel = parentZoom.GetComponentsInChildren<GeserKabel>(true);
        
        List<GeserKabel> listKabelWarna = new List<GeserKabel>();
        List<Vector2> listPosisiKabelWarnaAsli = new List<Vector2>();

        foreach (GeserKabel k in semuaKabel)
        {
            if (k.name.Contains("KabelBawah") || k.name.Contains("KabelAtas"))
            {
                continue; 
            }
            listKabelWarna.Add(k);
        }

        if (listKabelWarna.Count == 0) return;

        listKabelWarna.Sort((a, b) => a.GetComponent<RectTransform>().anchoredPosition.x.CompareTo(b.GetComponent<RectTransform>().anchoredPosition.x));
        
        foreach (GeserKabel kabel in listKabelWarna)
        {
            listPosisiKabelWarnaAsli.Add(kabel.GetComponent<RectTransform>().anchoredPosition);
        }

        BatasXMin = listPosisiKabelWarnaAsli[0].x - 30f;
        BatasXMax = listPosisiKabelWarnaAsli[listPosisiKabelWarnaAsli.Count - 1].x + 30f;

        System.Random rng = new System.Random();
        int n = listKabelWarna.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            GeserKabel value = listKabelWarna[k];
            listKabelWarna[k] = listKabelWarna[n];
            listKabelWarna[n] = value;
        }

        for (int i = 0; i < listKabelWarna.Count; i++)
        {
            RectTransform rt = listKabelWarna[i].GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.anchoredPosition = listPosisiKabelWarnaAsli[i];
                listKabelWarna[i].PerbaruiPosisiAwalSaatIni(listPosisiKabelWarnaAsli[i]);
            }
        }

        int indeksMulai = 1; 
        for (int i = 0; i < listKabelWarna.Count; i++)
        {
            listKabelWarna[i].transform.SetSiblingIndex(indeksMulai + i);
        }
    }

    public void PerbaruiSemuaPreviewUtama()
    {
        SimpanPreview(parentKabelKiri, previewKiri);
        SimpanPreview(parentKabelKanan, previewKanan);
    }

    GeserKabel[] DapatkanKabelBerurutan(Transform parentZoom)
    {
        GeserKabel[] semua = parentZoom.GetComponentsInChildren<GeserKabel>();
        List<GeserKabel> validKabel = new List<GeserKabel>();

        foreach (GeserKabel k in semua)
        {
            if (!k.name.Contains("KabelBawah") && !k.name.Contains("KabelAtas"))
            {
                validKabel.Add(k);
            }
        }

        validKabel.Sort((kabelA, kabelB) => 
            kabelA.transform.position.x.CompareTo(kabelB.transform.position.x)
        );
        return validKabel.ToArray();
    }

    public void BukaZoomKiri()
    {
        panelUtama.SetActive(false);
        panelZoomKiri.SetActive(true);
    }

    public void BukaZoomKanan()
    {
        panelUtama.SetActive(false);
        if (networkZoomKanan != null) networkZoomKanan.SetActive(true);
    }

    public void SelesaiKiri()
    {
        SimpanPreview(parentKabelKiri, previewKiri);
        panelZoomKiri.SetActive(false);
        panelUtama.SetActive(true);
    }

    public void SelesaiKanan()
    {
        SimpanPreview(parentKabelKanan, previewKanan);
        if (networkZoomKanan != null) networkZoomKanan.SetActive(false);
        panelUtama.SetActive(true);
    }

    void SimpanPreview(Transform parentZoom, Image[] preview)
    {
        if (parentZoom == null || preview == null) return;

        GeserKabel[] kabelTerurut = DapatkanKabelBerurutan(parentZoom);

        for (int i = 0; i < kabelTerurut.Length; i++)
        {
            if (i >= preview.Length) return;

            GeserKabel kabel = kabelTerurut[i];
            if (kabel.idKabel < 0 || kabel.idKabel >= spriteKabel.Length) continue;

            if (preview[i] != null)
            {
                preview[i].sprite = spriteKabel[kabel.idKabel];
                Color c = preview[i].color;
                c.a = 1f; 
                preview[i].color = c;
            }
        }
    }

    // =========================================================================
    // VALIDASI KLIK TOMBOL CEK SUSUNAN
    // =========================================================================
    public void TombolCekSusunan()
    {
        // Jalankan pengecekan dasar untuk setiap panel terlebih dahulu
        bool kiriCocokA = Validasi(parentKabelKiri, urutanBenarKiri);
        bool kiriCocokB = Validasi(parentKabelKiri, urutanBenarKanan);
        
        bool kananCocokA = Validasi(parentKabelKanan, urutanBenarKiri);
        bool kananCocokB = Validasi(parentKabelKanan, urutanBenarKanan);

        // 🔥 KONDISI 1: Kiri adalah T568A DAN Kanan adalah T568B
        bool kombinasiNormal = kiriCocokA && kananCocokB;

        // 🔥 KONDISI 2: Kiri adalah T568B DAN Kanan adalah T568A (Kondisi Tukar Posisi)
        bool kombinasiTerbalik = kiriCocokB && kananCocokA;

        // Jika salah satu dari kedua kombinasi di atas terpenuhi, maka dianggap BENAR!
        if (kombinasiNormal || kombinasiTerbalik)
        {
            Debug.Log("BENAR - KABEL CROSSOVER VALID (Polanya Saling Silang)");
            
            // Mencari SlideManager23 bawaan Scene 2 & 3 agar sinkron
            SlideManager23 sm = FindFirstObjectByType<SlideManager23>(FindObjectsInactive.Include);
            if (sm != null) sm.TampilkanPopupSelesai();
        }
        else
        {
            Debug.Log("SALAH - Silakan periksa kembali susunan kabel Crossover Anda! Pastikan kedua ujung memiliki standar berbeda (A dan B).");
        }
    }

    bool Validasi(Transform parent, int[] kunciJawaban)
    {
        GeserKabel[] kabelTerurut = DapatkanKabelBerurutan(parent);
        if (kabelTerurut.Length == 0 || kabelTerurut.Length != kunciJawaban.Length) return false;

        for (int i = 0; i < kabelTerurut.Length; i++)
        {
            if (kabelTerurut[i].idKabel != kunciJawaban[i])
            {
                return false;
            }
        }
        return true;
    }
}