using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class MateriAlatMove : MonoBehaviour
{
    [Header("1. Objek UI (Tarik dari Hierarchy)")]
    public GameObject kotakPenjelasan; // Tarik 'Info_Tang' / 'Info_Stripper'
    public RectTransform gambarAlat;   // Tarik 'Button' (yang isinya gambar alat)

    [Header("2. Isi Materi")]
    public TextMeshProUGUI komponenTeks; // Tarik 'Penjelasan'
    [TextArea(3, 5)] 
    public string[] daftarHalaman;
    
    [Header("3. Tombol Navigasi")]
    public Button tombolPrev; // Tarik 'Prev'
    public Button tombolNext; // Tarik 'Next'

    [Header("4. Titik Posisi Animasi")]
    public float posisiY_Tengah = 0f;  // Posisi Y saat teks tertutup
    public float posisiY_Atas = 100f;  // Posisi Y saat teks terbuka
    public float durasiGerak = 0.3f;   // Kecepatan animasi

    private int indexHalaman = 0;
    private Coroutine movementCoroutine;
    private Vector2 posisiAwalX; // Untuk menyimpan posisi X agar tidak berubah

    void Start()
    {
        if (gambarAlat != null) 
        {
            posisiAwalX = gambarAlat.anchoredPosition; // Simpan posisi X bawaan
            gambarAlat.anchoredPosition = new Vector2(posisiAwalX.x, posisiY_Tengah);
        }
        if (kotakPenjelasan != null) kotakPenjelasan.SetActive(false);
    }

    public void TogglePenjelasan()
    {
        if (daftarHalaman.Length == 0) return;

        bool kondisiSaatIni = kotakPenjelasan.activeSelf;
        
        if (movementCoroutine != null) StopCoroutine(movementCoroutine);

        if (!kondisiSaatIni) // Saat diklik: Buka Kotak -> Gambar naik
        {
            indexHalaman = 0;
            UpdateTampilan();
            kotakPenjelasan.SetActive(true);
            movementCoroutine = StartCoroutine(MoveImageY(posisiY_Atas));
        }
        else // Saat diklik lagi: Tutup Kotak -> Gambar turun ke tengah
        {
            kotakPenjelasan.SetActive(false);
            movementCoroutine = StartCoroutine(MoveImageY(posisiY_Tengah));
        }
    }

    // Fungsi Animasi Halus (Naik Turun)
    IEnumerator MoveImageY(float targetY)
    {
        Vector2 startPos = gambarAlat.anchoredPosition;
        Vector2 targetPos = new Vector2(posisiAwalX.x, targetY);
        float elapsedTime = 0f;

        while (elapsedTime < durasiGerak)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsedTime / durasiGerak));
            gambarAlat.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            yield return null;
        }
        gambarAlat.anchoredPosition = targetPos; 
    }

    public void HalamanSelanjutnya()
    {
        if (indexHalaman < daftarHalaman.Length - 1) { indexHalaman++; UpdateTampilan(); }
    }

    public void HalamanSebelumnya()
    {
        if (indexHalaman > 0) { indexHalaman--; UpdateTampilan(); }
    }

    void UpdateTampilan()
    {
        komponenTeks.text = daftarHalaman[indexHalaman];
        if (tombolPrev != null) tombolPrev.interactable = (indexHalaman > 0);
        if (tombolNext != null) tombolNext.interactable = (indexHalaman < daftarHalaman.Length - 1);
    }
}