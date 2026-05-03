using UnityEngine;

public class EfekJudul : MonoBehaviour
{
    [Header("Pilih Tipe Animasi")]
    public bool modeMelayang = true;
    public bool modeDenyut = true; // Saya default true biar kelihatan

    [Header("Pengaturan")]
    public float kecepatan = 2f;
    public float kekuatan = 10f;

    private Vector3 posisiAwal;
    private Vector3 skalaAwal; // Kita simpan ukuran aslinya di sini
    private RectTransform rectTrans;

    void Start()
    {
        rectTrans = GetComponent<RectTransform>();
        posisiAwal = rectTrans.anchoredPosition;
        
        // SIMPAN UKURAN YANG KAMU SET DI UNITY
        skalaAwal = rectTrans.localScale; 
    }

    void Update()
    {
        float gerak = Mathf.Sin(Time.time * kecepatan);

        if (modeMelayang)
        {
            rectTrans.anchoredPosition = posisiAwal + new Vector3(0, gerak * kekuatan, 0);
        }

        if (modeDenyut)
        {
            // RUMUS BARU:
            // Ambil "skalaAwal" dikali dengan denyutannya.
            // Jadi kalau kamu set scale 5, dia akan denyut dari 5 ke 5.5, bukan reset ke 1.
            float faktorDenyut = 1 + (gerak * (kekuatan / 100f));
            
            rectTrans.localScale = new Vector3(skalaAwal.x * faktorDenyut, skalaAwal.y * faktorDenyut, 1);
        }
    }
}