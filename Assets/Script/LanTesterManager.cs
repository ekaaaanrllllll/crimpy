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

    private Coroutine sequenceCoroutine;

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
            
            // 5. Kalau index udah sampai 8 (melewati batas), balik lagi ke 0 (Lampu 1)
            if (index >= 8) 
            {
                index = 0;
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
        // Asumsi warna gambar UI LED kamu sudah oranye/hijau, 
        // kita cuma ubah Alpha (A) supaya seolah-olah nyala/mati.
        c.a = alphaValue; 
        led.color = c;
    }
}