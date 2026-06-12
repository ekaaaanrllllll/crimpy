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

    [Header("Sprites Tang & Kabel")]
    public Sprite spriteTangKebuka;  
    public Sprite spriteTangKetutup; 
    public Sprite spriteRJ45CrimpBefore; 
    public Sprite spriteRJ45CrimpAfter; 

    [Header("Crimping Mechanics")]
    public int totalKrakMekanisme = 3; 
    private int krakCounter = 0;

    [Header("Android Double Tap Settings")]
    [Tooltip("Batas jeda waktu maksimal antar ketukan untuk dianggap sebagai double-tap (Detik)")]
    public float batasJedaDoubleTap = 0.3f; 
    private float waktuKetukanTerakhir = 0f;

    [Header("Perbaikan Posisi Tang (Fine Tuning)")]
    public Vector2 offsetTangKetutup = new Vector2(0f, 0f); 

    void Awake()
    {
        if (imageTangUtama != null && spriteTangKebuka != null)
        {
            imageTangUtama.sprite = spriteTangKebuka;
        }
    }

    public void ResetAksiCrimping()
    {
        this.enabled = true; 
        krakCounter = 0;    
        StopAllCoroutines();

        if (imageTangUtama != null && spriteTangKebuka != null)
        {
            imageTangUtama.sprite = spriteTangKebuka;
        }

        if (imageKabelRJ45 != null)
        {
            if (spriteRJ45CrimpBefore != null)
            {
                imageKabelRJ45.sprite = spriteRJ45CrimpBefore;
            }
            
            imageKabelRJ45.transform.localScale = Vector3.one;
            imageKabelRJ45.rectTransform.anchoredPosition = new Vector2(-612f, 84.646f);
        }
        
        Debug.Log("Retry Sukses: Sprite kembali ke Before!");
    }

    public void SetKabelMasukTang()
    {
        if (imageKabelRJ45 != null)
        {
            if (spriteRJ45CrimpBefore != null)
            {
                imageKabelRJ45.sprite = spriteRJ45CrimpBefore;
            }

            imageKabelRJ45.transform.localScale = Vector3.one;

            GameObject targetLobang = GameObject.Find("TitikTargetLobang");
            if (targetLobang != null)
            {
                RectTransform rectTarget = targetLobang.GetComponent<RectTransform>();
                imageKabelRJ45.rectTransform.anchoredPosition = rectTarget.anchoredPosition;
            }
            else
            {
                imageKabelRJ45.rectTransform.anchoredPosition = new Vector2(-289f, -1.99f);
            }
            
            Debug.Log("Kabel masuk lubang tang.");
        }
    }

    // =========================================================================
    // UTAMA: SISTEM DETEKSI DOUBLE TAP YANG AMAN UNTUK ANDROID
    // =========================================================================
    public void OnPointerClick(PointerEventData eventData)
    {
        // Hitung selisih waktu antara ketukan sekarang dengan ketukan sebelumnya
        float selisihWaktu = Time.time - waktuKetukanTerakhir;

        if (selisihWaktu <= batasJedaDoubleTap)
        {
            // Jika ketukan kedua masuk sebelum batas waktu habis, eksekusi jepit!
            ProsesKlikJepitSempurna();
            
            // Reset waktu agar tidak terjadi triple-tap yang tidak sengaja
            waktuKetukanTerakhir = 0f; 
        }
        else
        {
            // Jika terlalu lama, simpan waktu ketukan ini sebagai ketukan pertama
            waktuKetukanTerakhir = Time.time;
        }
    }

    void ProsesKlikJepitSempurna()
    {
        if (krakCounter < totalKrakMekanisme)
        {
            krakCounter++;
            
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

    IEnumerator AnimasiJepitSnapPergantianSprite()
    {
        Vector2 posisiAsli = imageTangUtama.rectTransform.anchoredPosition;
        imageTangUtama.sprite = spriteTangKetutup;
        imageTangUtama.rectTransform.anchoredPosition = posisiAsli + offsetTangKetutup;

        yield return new WaitForSeconds(0.18f); 
        
        imageTangUtama.sprite = spriteTangKebuka;
        // ↙️ HAPUS huruf 's' pada kata positionsAsli agar menjadi posisiAsli
        imageTangUtama.rectTransform.anchoredPosition = posisiAsli; 
    }

    void CrimpingSelesaiTuntas()
    {
        this.enabled = false;
        Debug.Log("KABEL BERHASIL DICRIMP SEMPURNA!");

        if (imageKabelRJ45 != null && spriteRJ45CrimpAfter != null)
        {
            // 1. Ganti gambarnya ke sprite After
            imageKabelRJ45.sprite = spriteRJ45CrimpAfter;
            
            // 2. PAKSA POSISI ASLI (Sesuai koreksi kamu: X = -143, Y = 110.82)
            imageKabelRJ45.rectTransform.anchoredPosition = new Vector2(-143f, 110.82f);

            // 3. PAKSA UKURAN ASLI (Sesuai koreksi kamu: W = 394.0376, H = 221.6438)
            imageKabelRJ45.rectTransform.sizeDelta = new Vector2(394.0376f, 221.6438f);
            
            // 4. Reset skala ke 1:1 dan matikan preserveAspect agar ukuran patuh total pada sizeDelta
            imageKabelRJ45.transform.localScale = Vector3.one;
            imageKabelRJ45.preserveAspect = false; 
        }

        if (FindFirstObjectByType<SlideManager23>() != null)
        {
            FindFirstObjectByType<SlideManager23>().TampilkanPopupSelesai();
        }
    }
}