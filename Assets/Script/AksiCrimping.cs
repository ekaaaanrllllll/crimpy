using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class AksiCrimping : MonoBehaviour, IPointerClickHandler
{
    [Header("Scripts & Objects References")]
    public Image imageTangUtama; 
    public Image imageKabelRJ45; 
    public AudioSource suaraKrak; 

    [Header("Sprites")]
    public Sprite spriteTangKebuka;  
    public Sprite spriteTangKetutup; 
    public Sprite spriteRJ45CrimpAfter; 

    [Header("Crimping Mechanics")]
    public int totalKrakMekanisme = 3; 
    private int krakCounter = 0;

    void Awake()
    {
        if (imageTangUtama != null && spriteTangKebuka != null)
        {
            imageTangUtama.sprite = spriteTangKebuka;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
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
            Debug.Log("Krak! ke-" + krakCounter);
            
            if (suaraKrak != null)
            {
                suaraKrak.Stop(); 
                suaraKrak.Play();
            }

            StopAllCoroutines(); 
            StartCoroutine(AnimasiJepitSnapPergantianSprite());

            if (krakCounter == totalKrakMekanisme)
            {
                CrimpingSelesaiTuntas();
            }
        }
    }

    [Header("Perbaikan Posisi (Fine Tuning)")]
    [Tooltip("Seberapa jauh tang bergeser (X, Y) saat posisi KETUTUP agar kabel pas")]
    public Vector2 offsetTangKetutup = new Vector2(0f, 0f); // default 0,0

    IEnumerator AnimasiJepitSnapPergantianSprite()
    {
        // --- 1. Simpan posisi asli tang kebuka ---
        Vector2 posisiAsli = imageTangUtama.rectTransform.anchoredPosition;

        // --- 2. Ganti ke gambar ketutup ---
        imageTangUtama.sprite = spriteTangKetutup;

        // --- 3. TERAPKAN OFFSET ---
        // Kita geser posisi tangnya sedikit agar visualnya pas dengan kabel
        imageTangUtama.rectTransform.anchoredPosition = posisiAsli + offsetTangKetutup;

        // Tahan sebentar biar kerasa ngejepit
        yield return new WaitForSeconds(0.18f); 
        
        // --- 4. Balik lagi ke gambar kebuka ---
        imageTangUtama.sprite = spriteTangKebuka;

        // --- 5. KEMBALIKAN KE POSISI ASLI ---
        imageTangUtama.rectTransform.anchoredPosition = posisiAsli;
    }

    void CrimpingSelesaiTuntas()
    {
        // Matikan script agar tidak bisa di-klik lagi
        this.enabled = false;
        Debug.Log("KABEL BERHASIL DICRIMP SEMPURNA!");

        // Update gambar kabel RJ45 beserta posisi dan ukurannya yang BENAR
        if(imageKabelRJ45 != null && spriteRJ45CrimpAfter != null)
        {
            imageKabelRJ45.sprite = spriteRJ45CrimpAfter;
            
            // Menggunakan koordinat terbaru dari screenshot kamu
            imageKabelRJ45.rectTransform.anchoredPosition = new Vector2(-154.7229f, 87.76799f);
            imageKabelRJ45.rectTransform.sizeDelta = new Vector2(402.2377f, 226.2587f);
        }

        // // Memanggil popup sukses dari SlideManager (Konsep baru yang tadi kita bahas)
        // SlideManager slideMgr = FindObjectOfType<SlideManager>();
        // if(slideMgr != null)
        // {
        //     slideMgr.TampilkanPopup();
        // }
        // else
        // {
        //     Debug.LogWarning("SlideManager tidak ditemukan, pastikan ada di scene!");
        // }
    }
}