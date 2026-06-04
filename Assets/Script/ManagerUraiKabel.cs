using UnityEngine;

public class ManagerUraikanKabel : MonoBehaviour
{
    [Header("Hubungkan ke Game Manager")]
    public SlideManager slideManagerUtama; 

    private int jumlahKabelLurus = 0;
    private int totalKabel = 4; // Oren, Hijau, Biru, Coklat

    // Direset tiap kali masuk Slide 2 / Saat ditekan Retry
    void OnEnable()
    {
        jumlahKabelLurus = 0; 
    }

    public void TambahKabelLurus()
    {
        jumlahKabelLurus++;
        
        if (jumlahKabelLurus >= totalKabel)
        {
            Debug.Log("Semua 4 Kabel Lurus! Buka gembok Next.");
            if (slideManagerUtama != null)
            {
                slideManagerUtama.TampilkanPopupSelesai();
            }
        }
    }
}