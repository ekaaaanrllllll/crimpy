using UnityEngine;
using UnityEngine.UI;

public class SlotKabel : MonoBehaviour
{
    // ID Kabel yang sedang nempel di slot ini sekarang.
    // -1 artinya kosong, 0 = Putih-Oren, 1 = Oren, dst (sesuaikan standar T568B kamu)
    public int kabelIDSaatIni = -1; 
    
    private Image imageSlot;

    void Awake()
    {
        imageSlot = GetComponent<Image>();
    }

    // Fungsi untuk mengambil warna slot saat ini (untuk di-copy ke layar utama)
    public Color GetWarnaSekarang()
    {
        if (imageSlot != null) return imageSlot.color;
        return Color.white;
    }
}