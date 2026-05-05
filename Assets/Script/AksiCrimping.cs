using UnityEngine;
using UnityEngine.UI; // Wajib untuk mengakses komponen Image dan Sprite
using UnityEngine.EventSystems; // Wajib untuk deteksi klik
using System.Collections;

public class AksiCrimping : MonoBehaviour, IPointerClickHandler // Interface untuk klik
{
    [Header("Scripts & Objects References")]
    [Tooltip("Tarik komponen Image TangCrimpingZoom dari dirinya sendiri ke sini")]
    public Image imageTangUtama; 
    [Tooltip("Tarik komponen Image KabelZoom (RJ45) ke sini (Opsional)")]
    public Image imageKabelRJ45; 

    [Header("Sprites (Tarik Assets Gambar ke Sini)")]
    [Tooltip("Asset gambar tang posisi KEBuka (seperti yang sekarang)")]
    public Sprite spriteTangKebuka;  
    [Tooltip("Asset gambar tang posisi KETUTUP (yang baru kamu punya)")]
    public Sprite spriteTangKetutup; 
    [Tooltip("Asset gambar RJ45 yang sudah penyok dicrimp (jika ada, kalau gak ada kosongkan)")]
    public Sprite spriteRJ45CrimpAfter; 

    [Header("Crimping Mechanics")]
    [Tooltip("Berapa kali double-click krak (kiri-kanan)")]
    public int totalKrakMekanisme = 3; 
    private int krakCounter = 0;
    
    // (Opsional) Public AudioSource suaraKrak; // Kalau ada suara "Krak!"

    void Awake()
    {
        // Pastikan saat game mulai, gambarnya adalah tang kebuka
        if (imageTangUtama != null && spriteTangKebuka != null)
        {
            imageTangUtama.sprite = spriteTangKebuka;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Mendeteksi DOUBLE CLICK
        if (eventData.clickCount == 2) 
        {
            ProsesKlikJepitSempurna();
        }
    }

    void ProsesKlikJepitSempurna()
    {
        if (krakCounter < totalKrakMekanisme)
        {
            krakCounter++;
            Debug.Log("Krak! ke-" + krakCounter + "/" + totalKrakMekanisme);

            // if (suaraKrak != null) suaraKrak.Play(); // Mainkan suara

            // Jalankan animasi pergantian sprite yang super realistis
            // Kita matikan dulu animasi lama (kalau user klik cepet banget) biar gak tumpang tindih
            StopAllCoroutines(); 
            StartCoroutine(AnimasiJepitSnapPergantianSprite());

            if (krakCounter == totalKrakMekanisme)
            {
                CrimpingSelesaiTuntas();
            }
        }
    }

    IEnumerator AnimasiJepitSnapPergantianSprite()
    {
        // --- FASE 1: JEpit (Hold) ---
        // TUKAR LANGSUNG JADI GAMBAR KETUTUP (Dalam waktu 0 detik)
        imageTangUtama.sprite = spriteTangKetutup;

        // TAHAN GAYA JEPIT (Di sinilah kuncinya biar kerasa "nahan beban")
        // Semakin lama ditahan, semakin kuat feel jepitnya. Coba di antara 0.1s - 0.2s.
        yield return new WaitForSeconds(0.18f); 

        // --- FASE 2: LEPAS (Snap) ---
        // KEMBALIKAN LANGSUNG JADI GAMBAR KEBuka (Snap back)
        imageTangUtama.sprite = spriteTangKebuka;
    }

    void CrimpingSelesaiTuntas()
    {
        // MATIKAN script ini biar nggak bisa diklik ganda lagi
        this.enabled = false;
        
        Debug.Log("KABEL BERHASIL DICRIMP SEMPURNA!");

        // GANTI GAMBAR KABEL JADI "KABEL SUDAH DI-CRIMP"
        if(imageKabelRJ45 != null && spriteRJ45CrimpAfter != null)
        {
            // 1. Ganti gambarnya
            imageKabelRJ45.sprite = spriteRJ45CrimpAfter;

            // 2. Kunci Width dan Height sesuai screenshot kamu
            imageKabelRJ45.rectTransform.sizeDelta = new Vector2(255.1784f, 143.54f);

            // 3. Kunci Posisi X dan Y biar posisinya nggak geser dari lubang tang
            imageKabelRJ45.rectTransform.anchoredPosition = new Vector2(-379.4397f, 4.7002f);
        }

        // DI SINI BISA BUKA GEMBOK TOMBOL "NEXT" DI SLIDE MANAGER
    }
}