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

    [Header("Perbaikan Posisi Tang (Fine Tuning)")]
    public Vector2 offsetTangKetutup = new Vector2(0f, 0f); 

    void Awake()
    {
        if (imageTangUtama != null && spriteTangKebuka != null)
        {
            imageTangUtama.sprite = spriteTangKebuka;
        }
    }

    // =========================================================================
    // 1. FUNGSI RETRY: Kembalikan kabel ke posisi paling awal (luar tang)
    // =========================================================================
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
            
            // 🔥 SOLUSI UTAMA: Kembalikan skala visual ke normal (1, 1, 1) agar proporsi Before pas murni asli
            imageKabelRJ45.transform.localScale = Vector3.one;

            // Pulangkan koordinat posisi awal luar tang kamu
            imageKabelRJ45.rectTransform.anchoredPosition = new Vector2(-612f, 84.646f);
        }
        
        Debug.Log("Retry Sukses: Sprite kembali ke Before dengan skala asli 1:1!");
    }

    // =========================================================================
    // 2. FUNGSI DRAG SUKSES: Ikut posisi target lubang, Skala tetap normal Before
    // =========================================================================
    public void SetKabelMasukTang()
    {
        if (imageKabelRJ45 != null)
        {
            if (spriteRJ45CrimpBefore != null)
            {
                imageKabelRJ45.sprite = spriteRJ45CrimpBefore;
            }

            // Pastikan skala visualnya tidak gepeng (tetap proporsi normal murni bawaan Editor)
            imageKabelRJ45.transform.localScale = Vector3.one;

            // Otomatis mengunci ke TitikTargetLobang kamu
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
            
            Debug.Log("Kabel masuk lubang tang dengan skala normal.");
        }
    }

    // =========================================================================
    // 3. MEKANISME KLIK JEPIT
    // =========================================================================
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
        imageTangUtama.rectTransform.anchoredPosition = posisiAsli;
    }

    // =========================================================================
    // 4. FUNGSI FINISH SUKSES: Ganti sprite After, sesuaikan posisi & ukuran stretch
    // =========================================================================
    void CrimpingSelesaiTuntas()
    {
        this.enabled = false;
        Debug.Log("KABEL BERHASIL DICRIMP SEMPURNA!");

        if(imageKabelRJ45 != null && spriteRJ45CrimpAfter != null)
        {
            imageKabelRJ45.sprite = spriteRJ45CrimpAfter;
            
            // Mengubah posisi khusus untuk tipe asset After agar presisi di tengah tang
            imageKabelRJ45.rectTransform.anchoredPosition = new Vector2(-154.7229f, 87.76799f);
            
            // 🔥 RE-ADJUST UNTUK AFTER: Karena kanvas After bawaan aslinya lebih melebar, kita paksa sedikit skalanya agar pas memenuhi lubang tang
            imageKabelRJ45.rectTransform.sizeDelta = new Vector2(402.2377f, 226.2587f);
        }

        if (FindFirstObjectByType<SlideManager>() != null)
        {
            FindFirstObjectByType<SlideManager>().TampilkanPopupSelesai();
        }
    }
}