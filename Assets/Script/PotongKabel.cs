using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using TMPro;

public class PotongKabelGeser : MonoBehaviour, IDragHandler, IEndDragHandler
{
    [Header("Objek Visual")]
    public RectTransform gunting;      
    public RectTransform areaTarget;   
    public GameObject kabelUtuh;       
    public GameObject kabelKupas;      
    public TMP_Text teksPetunjuk;      
    public Canvas canvasUtama;         

    [Header("Objek UI Petunjuk & Navigasi")]
    public GameObject panahPetunjuk; 
    public GameObject garisPutusPetunjuk; 
    
    // 1. KITA GANTI TOMBOL NEXT JADI SLIDE MANAGER UTAMA
    [Header("Hubungkan ke Game Manager")]
    public SlideManager slideManagerUtama;       

    [Header("Pengaturan Potong")]
    public int butuhBerapaAyunan = 6;
    public float sensitivitasPosisi = 3.0f; 
    public float sensitivitasRotasi = 2.0f; 
    public float batasGerakY = 120f; 
    public float sensitivitasGunting = 0.3f; 
    
    [Header("Pengaturan Rotasi Gunting")]
    public float sudutNukikMotong = 35f;   
    public float sudutWaggleAyunan = 10f;  

    private int tahapSaatIni = 0; 
    private int jumlahAyunanSaatIni = 0;
    private bool keAtas = true; 
    
    private Vector2 posisiAwalMeja;
    private Quaternion rotasiAwalMeja;
    private Quaternion rotasiBaseMotong; 

    void Awake()
    {
        if (gunting != null)
        {
            posisiAwalMeja = gunting.anchoredPosition;
            rotasiAwalMeja = gunting.localRotation;
        }
    }

    void OnEnable()
    {
        SetupAwal();
    }

    public void SetupAwal()
    {
        tahapSaatIni = 0; 
        jumlahAyunanSaatIni = 0;
        keAtas = true; 

        if (gunting != null)
        {
            gunting.anchoredPosition = posisiAwalMeja;
            gunting.localRotation = rotasiAwalMeja;
        }
        
        if(kabelUtuh != null) kabelUtuh.SetActive(true);
        if(kabelKupas != null) kabelKupas.SetActive(true); 
        
        if(teksPetunjuk != null) teksPetunjuk.text = "Tarik gunting ke pangkal kabel!";
        if(garisPutusPetunjuk != null) garisPutusPetunjuk.SetActive(true);
        if(panahPetunjuk != null) panahPetunjuk.SetActive(false);
        
        // 2. BAGIAN MATIIN TOMBOL DIHAPUS KARENA SUDAH DIURUS SLIDEMANAGER
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (tahapSaatIni == 0)
        {
            gunting.anchoredPosition += eventData.delta / canvasUtama.scaleFactor;
        }
        else if (tahapSaatIni == 1)
        {
            if(panahPetunjuk != null) panahPetunjuk.SetActive(false);

            float gerakanY = eventData.delta.y;

            if (keAtas && gerakanY < -10f) 
            {
                keAtas = false;
                jumlahAyunanSaatIni++;
            }
            else if (!keAtas && gerakanY > 10f) 
            {
                keAtas = true;
                jumlahAyunanSaatIni++;
            }

            float kemiringanOffset = Mathf.Clamp(gerakanY * sensitivitasGunting, -sudutWaggleAyunan, sudutWaggleAyunan); 
            Quaternion targetRotasi = rotasiBaseMotong * Quaternion.Euler(0, 0, kemiringanOffset);
            
            gunting.localRotation = Quaternion.Lerp(gunting.localRotation, targetRotasi, 0.5f);

            if (jumlahAyunanSaatIni >= butuhBerapaAyunan)
            {
                tahapSaatIni = 2; 
                StartCoroutine(ProsesKabelPutus());
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (tahapSaatIni == 0)
        {
            float jarak = Vector2.Distance(gunting.anchoredPosition, areaTarget.anchoredPosition);
            
            if (jarak < 100f) 
            {
                gunting.anchoredPosition = areaTarget.anchoredPosition;
                tahapSaatIni = 1; 
                
                rotasiBaseMotong = Quaternion.Euler(0, 0, sudutNukikMotong);
                gunting.localRotation = rotasiBaseMotong; 

                if(teksPetunjuk != null) teksPetunjuk.text = "Geser ke ATAS dan BAWAH untuk memotong!";
                if(garisPutusPetunjuk != null) garisPutusPetunjuk.SetActive(false); 
                if(panahPetunjuk != null) panahPetunjuk.SetActive(true); 
            }
            else
            {
                gunting.anchoredPosition = posisiAwalMeja;
                gunting.localRotation = rotasiAwalMeja; 
            }
        }
    }

    IEnumerator ProsesKabelPutus()
    {
        gunting.localRotation = rotasiBaseMotong;
        yield return new WaitForSeconds(0.2f);
        
        kabelUtuh.SetActive(false);
        kabelKupas.SetActive(true);

        if(teksPetunjuk != null) teksPetunjuk.text = "Berhasil dipotong!";
        if(panahPetunjuk != null) panahPetunjuk.SetActive(false);

        // 3. LAPOR KE SLIDE MANAGER KALAU PUZZLE BERES!
        if(slideManagerUtama != null) 
        {
            slideManagerUtama.BukaGembokSlideIni();
        }

        yield return new WaitForSeconds(0.5f);
        
        gunting.anchoredPosition = posisiAwalMeja;
        gunting.localRotation = rotasiAwalMeja; 
    }
}