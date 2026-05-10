using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LanTesterManager : MonoBehaviour
{
    [Header("Pengaturan LED Master (Isi 8 Lampu)")]
    public Image[] masterLEDs; 

    [Header("Pengaturan LED Remote (Isi 8 Lampu)")]
    public Image[] remoteLEDs; 

    [Header("Kecepatan & Visual Lampu")]
    public float kecepatanPindah = 0.5f; // Waktu jeda per lampu (detik)
    
    // Kita pakai efek Transparansi (Alpha) untuk membedakan mati/nyala
    public float alphaMati = 0.2f;  // Redup
    public float alphaNyala = 1.0f; // Terang benderang

    [Header("Pengaturan Popup (Looping)")]
    public int targetPutaran = 6; // Berapa balikan sebelum popup muncul

    private Coroutine sequenceCoroutine;
    private bool popupSudahMuncul = false; // Biar popup ga kepanggil berkali-kali

    void Start()
    {
        // Pastikan pas game mulai, semua lampu mati
        MatikanSemuaLED();
    }

    // Fungsi ini yang akan dipanggil oleh tombol Power
    public void MulaiSequence(bool isMulai)
    {
        if (isMulai)
        {
            popupSudahMuncul = false; // Reset status popup kalau alat dinyalakan ulang
            if (sequenceCoroutine != null) StopCoroutine(sequenceCoroutine);
            sequenceCoroutine = StartCoroutine(JalankanSequenceLampu());
        }
        else
        {
            if (sequenceCoroutine != null) StopCoroutine(sequenceCoroutine);
            MatikanSemuaLED(); // Kalau dimatikan, reset semua lampu jadi redup
        }
    }

    IEnumerator JalankanSequenceLampu()
    {
        int index = 0;
        int jumlahPutaran = 0; // Variabel baru untuk menghitung balikan
        
        // Looping terus-menerus selama alat menyala
        while (true) 
        {
            // 1. Matikan semua lampu dulu di awal siklus
            MatikanSemuaLED();

            // 2. Nyalakan lampu Master dan Remote sesuai urutan (index)
            if (index < masterLEDs.Length) SetAlpha(masterLEDs[index], alphaNyala);
            if (index < remoteLEDs.Length) SetAlpha(remoteLEDs[index], alphaNyala);

            // 3. Tunggu sebentar (Jeda)
            yield return new WaitForSeconds(kecepatanPindah);

            // 4. Lanjut ke lampu berikutnya
            index++;
            
            // 5. Kalau index udah sampai 8, balik lagi ke 0 (Satu putaran selesai!)
            if (index >= 8) 
            {
                index = 0; // Balik ke lampu 1
                jumlahPutaran++; // Tambah hitungan putaran

                // CEK APAKAH SUDAH 6 PUTARAN & POPUP BELUM MUNCUL
                if (jumlahPutaran >= targetPutaran && !popupSudahMuncul)
                {
                    popupSudahMuncul = true; // Kunci biar ga manggil popup lagi di putaran ke 7, 8, dst.
                    
                    // ==========================================
                    // --- MEMANGGIL POPUP SUKSES DARI SINI ---
                    // ==========================================
                    if (FindFirstObjectByType<SlideManager>() != null)
                    {
                        FindFirstObjectByType<SlideManager>().TampilkanPopupSelesai();
                    }
                    // ==========================================
                }
            }
        }
    }

    void MatikanSemuaLED()
    {
        // Looping untuk meredupkan semua lampu Master
        for (int i = 0; i < masterLEDs.Length; i++)
        {
            if (masterLEDs[i] != null) SetAlpha(masterLEDs[i], alphaMati);
        }
        // Looping untuk meredupkan semua lampu Remote
        for (int i = 0; i < remoteLEDs.Length; i++)
        {
            if (remoteLEDs[i] != null) SetAlpha(remoteLEDs[i], alphaMati);
        }
    }

    void SetAlpha(Image led, float alphaValue)
    {
        if (led == null) return;
        Color c = led.color;
        c.a = alphaValue; 
        led.color = c;
    }
}