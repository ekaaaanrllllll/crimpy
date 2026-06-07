using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LanTesterCrossoverManager : MonoBehaviour
{
    [Header("Pengaturan LED Master (Isi 8 Lampu)")]
    public Image[] masterLEDs; 

    [Header("Pengaturan LED Remote (Isi 8 Lampu)")]
    public Image[] remoteLEDs; 

    [Header("Kecepatan & Visual Lampu")]
    public float kecepatanPindah = 0.5f; // Waktu jeda per lampu (detik)
    
    // Efek Transparansi (Alpha) untuk membedakan mati/nyala
    public float alphaMati = 0.2f;  // Redup
    public float alphaNyala = 1.0f; // Terang benderang

    [Header("Pengaturan Popup (Looping)")]
    public int targetPutaran = 6; // Berapa balikan sebelum popup muncul

    private Coroutine sequenceCoroutine;
    private bool popupSudahMuncul = false; // Biar popup ga kepanggil berkali-kali

    // 🔥 KUNCI URUTAN REMOTE UNTUK CROSSOVER (Konversi ke Index Array 0-7)
    // Master [0, 1, 2, 3, 4, 5, 6, 7] akan menyalakan Remote sesuai isi array ini:
    private readonly int[] urutanRemoteCrossover = { 2, 5, 0, 3, 4, 1, 6, 7 };

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
        int jumlahPutaran = 0; // Variabel untuk menghitung balikan
        
        // Looping terus-menerus selama alat menyala
        while (true) 
        {
            // 1. Matikan semua lampu dulu di awal siklus
            MatikanSemuaLED();

            // 2. Nyalakan lampu Master sesuai urutan (index)
            if (index < masterLEDs.Length) SetAlpha(masterLEDs[index], alphaNyala);
            
            // 3. 🔥 LOGIKA BARU: Nyalakan lampu Remote berdasarkan Mapping Crossover
            if (index < urutanRemoteCrossover.Length)
            {
                int remoteTargetIndex = urutanRemoteCrossover[index];
                if (remoteTargetIndex < remoteLEDs.Length)
                {
                    SetAlpha(remoteLEDs[remoteTargetIndex], alphaNyala);
                }
            }

            // 4. Tunggu sebentar (Jeda)
            yield return new WaitForSeconds(kecepatanPindah);

            // 5. Lanjut ke lampu berikutnya
            index++;
            
            // 6. Kalau index udah sampai 8, balik lagi ke 0 (Satu putaran selesai!)
            if (index >= 8) 
            {
                index = 0; // Balik ke lampu 1
                jumlahPutaran++; // Tambah hitungan putaran

                // CEK APAKAH SUDAH PUTARAN TARGET & POPUP BELUM MUNCUL
                if (jumlahPutaran >= targetPutaran && !popupSudahMuncul)
                {
                    popupSudahMuncul = true; // Kunci biar ga manggil popup lagi di putaran berikutnya
                    
                    // ==================================================
                    // --- MEMANGGIL POPUP SUKSES SCENE 2 & 3 DI SINI ---
                    // ==================================================
                    SlideManager23 sm = FindFirstObjectByType<SlideManager23>(FindObjectsInactive.Include);
                    if (sm != null)
                    {
                        sm.TampilkanPopupSelesai();
                    }
                    // ==================================================
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

    // 🔥 FUNGSI RESET UNTUK RETRY (PENTING: Harus didaftarkan di SlideManager23)
    public void ResetLanTesterCrossoverManager()
    {
        // Hentikan putaran sequence lampu jika sedang berjalan
        if (sequenceCoroutine != null) 
        {
            StopCoroutine(sequenceCoroutine);
        }
        
        popupSudahMuncul = false; // Reset kunci popup
        MatikanSemuaLED();        // Redupkan semua LED Master & Remote
        Debug.Log("CONSOLE: LanTesterCrossoverManager berhasil di-reset!");
    }
}