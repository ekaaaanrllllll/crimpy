using UnityEngine;

public class ManagerUraikanKabel : MonoBehaviour
{
    [Header("Hubungkan ke Game Manager")]
    public SlideManager slideManagerUtama; 

    private int jumlahKabelLurus = 0;
    private int totalKabel = 4; // Oren, Hijau, Biru, Coklat

    // Direset tiap kali masuk Slide 2
    void OnEnable()
    {
        jumlahKabelLurus = 0; 
    }

    // Dipanggil oleh UsapKabel.cs tiap ada 1 warna yang beres
    public void TambahKabelLurus()
    {
        jumlahKabelLurus++;
        
        // Kalau 4 kabel sudah lurus semua, buka gembok tombol Next!
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