using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class SusunKabelManager : MonoBehaviour
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

    private readonly int[] urutanBenar = { 0, 1, 2, 3, 4, 5, 6, 7 };

    void OnEnable()
    {
        if (panelUtama != null) panelUtama.SetActive(true);
        if (panelZoomKiri != null) panelZoomKiri.SetActive(false);
        if (networkZoomKanan != null) networkZoomKanan.SetActive(false);

        // Langsung acak warna kabelnya saja saat masuk slide
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
    // LOGIK BARU: HANYA MENGACAK KABEL BERWARNA (KABEL ATAS & BAWAH TIDAK IKUT)
    // =========================================================================
    void AcakUrutanKabelWarnaSaja(Transform parentZoom)
    {
        if (parentZoom == null) return;

        // 1. Ambil semua anak objek yang BENAR-BENAR merupakan komponen GeserKabel
        GeserKabel[] semuaKabel = parentZoom.GetComponentsInChildren<GeserKabel>(true);
        
        List<GeserKabel> listKabelWarna = new List<GeserKabel>();
        List<Vector2> listPosisiKabelWarnaAsli = new List<Vector2>(); // <-- Nama variabel yang benar

        // FILTER: Pilah objek. Pastikan KabelBawah / KabelAtas tidak ikut masuk hitungan acak
        foreach (GeserKabel k in semuaKabel)
        {
            // Deteksi berdasarkan nama objek bawaan di Hierarchy agar tidak salah saring
            if (k.name.Contains("KabelBawah") || k.name.Contains("KabelAtas"))
            {
                continue; // Skip, jangan sentuh objek pelindung ini!
            }
            listKabelWarna.Add(k);
        }

        if (listKabelWarna.Count == 0) return;

        // 2. Urutkan koordinat X asli kabel warna dari kiri ke kanan sebelum dikocok
        listKabelWarna.Sort((a, b) => a.GetComponent<RectTransform>().anchoredPosition.x.CompareTo(b.GetComponent<RectTransform>().anchoredPosition.x));
        
        foreach (GeserKabel kabel in listKabelWarna)
        {
            listPosisiKabelWarnaAsli.Add(kabel.GetComponent<RectTransform>().anchoredPosition);
        }

        // 3. Kocok urutan list kabel warna saja (Fisher-Yates Shuffle)
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

        // 4. Pasangkan kabel warna yang sudah acak ke koordinat posisi asli tadi
        for (int i = 0; i < listKabelWarna.Count; i++)
        {
            RectTransform rt = listKabelWarna[i].GetComponent<RectTransform>();
            if (rt != null)
            {
                // Berikan koordinat X acak, posisi Y tetap aman mengunci
                rt.anchoredPosition = listPosisiKabelWarnaAsli[i];
                
                // 🔥 FIX DI SINI: Sekarang nama variabel sudah disamakan agar tidak error lagi!
                listKabelWarna[i].PerbaruiPosisiAwalSaatIni(listPosisiKabelWarnaAsli[i]);
            }
        }

        // 5. Susun ulang Sibling Index khusus kabel warna agar urutan Hierarchy-nya sinkron pas di-Sort X position
        int indeksMulai = 1; // Mulai dari indeks 1 (asumsi urutan ke-0 diisi KabelBawah)
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
        // Ambil semua komponen GeserKabel, bersihkan dari objek KabelAtas/KabelBawah agar tidak merusak index validasi
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

    public void TombolCekSusunan()
    {
        bool kiriBenar = Validasi(parentKabelKiri);
        bool kananBenar = Validasi(parentKabelKanan);

        if (kiriBenar && kananBenar)
        {
            Debug.Log("BENAR - STRAIGHT THROUGH");
            SlideManager sm = FindFirstObjectByType<SlideManager>();
            if (sm != null) sm.TampilkanPopupSelesai();
        }
        else
        {
            Debug.Log("SALAH - Silakan periksa kembali susunan kabel Anda!");
        }
    }

    bool Validasi(Transform parent)
    {
        GeserKabel[] kabelTerurut = DapatkanKabelBerurutan(parent);
        if (kabelTerurut.Length == 0) return false;

        for (int i = 0; i < kabelTerurut.Length; i++)
        {
            if (kabelTerurut[i].idKabel != urutanBenar[i])
            {
                return false;
            }
        }
        return true;
    }
}