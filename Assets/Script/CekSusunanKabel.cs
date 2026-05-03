using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CekSusunanKabel : MonoBehaviour
{
    [Header("Masukkan 8 Kabel KIRI")]
    public List<GeserKabel> daftarKabelKiri;

    [Header("Masukkan 8 Kabel KANAN")]
    public List<GeserKabel> daftarKabelKanan;

    [Header("Hubungkan ke Slide Manager")]
    public SlideManager slideManagerUtama;

    // Kunci jawaban standar T568B (Tanpa Spasi/Garis Bawah)
    private string[] kunciJawabanT568B = {
        "PutihOren", "Oren", "PutihHijau", "Biru", 
        "PutihBiru", "Hijau", "PutihCoklat", "Coklat"
    };

    public void ValidasiSusunan()
    {
        // 1. Urutkan kabel berdasarkan posisi X terkecil (paling kiri) ke terbesar (kanan)
        var urutanKiri = daftarKabelKiri.OrderBy(k => k.transform.position.x).ToList();
        var urutanKanan = daftarKabelKanan.OrderBy(k => k.transform.position.x).ToList();

        // 2. Cek masing-masing sisi
        bool kiriBenar = CekSatuSisi(urutanKiri);
        bool kananBenar = CekSatuSisi(urutanKanan);

        // 3. Beri Keputusan
        if (kiriBenar && kananBenar)
        {
            Debug.Log("MANTAP! Kedua ujung kabel sudah benar (T568B).");
            if (slideManagerUtama != null)
            {
                slideManagerUtama.BukaGembokSlideIni(); // Buka tombol Next!
            }
        }
        else if (!kiriBenar && !kananBenar)
        {
            Debug.Log("Kiri dan Kanan masih salah. Ayo susun lagi!");
        }
        else if (!kiriBenar)
        {
            Debug.Log("Kanan sudah benar, tapi KIRI masih salah!");
        }
        else
        {
            Debug.Log("Kiri sudah benar, tapi KANAN masih salah!");
        }
    }

    // Fungsi bantuan untuk mencocokkan urutan
    private bool CekSatuSisi(List<GeserKabel> kabelSisiIni)
    {
        for (int i = 0; i < kunciJawabanT568B.Length; i++)
        {
            if (kabelSisiIni[i].namaWarna != kunciJawabanT568B[i])
            {
                return false; // Langsung ketahuan kalau ada 1 aja yang posisinya salah
            }
        }
        return true;
    }
}