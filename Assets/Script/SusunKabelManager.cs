using UnityEngine;
using UnityEngine.UI;
using System; // WAJIB DITAMBAHKAN UNTUK FUNGSI SORTING

public class SusunKabelManager : MonoBehaviour
{
    [Header("Panel")]
    public GameObject panelUtama;
    public GameObject panelZoomKiri;
    public GameObject networkZoomKanan; // Menyesuaikan nama panel zoom kanan Anda

    [Header("Preview Panel Utama")]
    public Image[] previewKiri;
    public Image[] previewKanan;

    [Header("Sprite Kabel")]
    public Sprite[] spriteKabel;

    [Header("Parent Zoom")]
    public Transform parentKabelKiri;
    public Transform parentKabelKanan;

    private readonly int[] urutanBenar =
    {
        0, // putih oren
        1, // oren
        2, // putih hijau
        3, // biru
        4, // putih biru
        5, // hijau
        6, // putih coklat
        7  // coklat
    };

    // =====================================
    // FUNGSI UTAMA: SORTING BERDASARKAN POSISI X
    // =====================================
   GeserKabel[] DapatkanKabelBerurutan(Transform parentZoom)
    {
        // Mengambil semua komponen GeserKabel yang ada di dalam parent zoom
        GeserKabel[] daftarKabel = parentZoom.GetComponentsInChildren<GeserKabel>();

        // SEBELUMNYA: kabelA.GetComponent<RectTransform>().anchoredPosition.x
        // SEKARANG: Ganti ke transform.position.x agar 100% akurat dari kiri ke kanan 
        // tanpa memedulikan perbedaan Anchor atau Pivot di UI
        Array.Sort(daftarKabel, (kabelA, kabelB) => 
            kabelA.transform.position.x.CompareTo(kabelB.transform.position.x)
        );

        return daftarKabel;
    }

    // =========================
    // PANEL NAVIGATION
    // =========================

    public void BukaZoomKiri()
    {
        panelUtama.SetActive(false);
        panelZoomKiri.SetActive(true);
    }

    public void BukaZoomKanan()
    {
        panelUtama.SetActive(false);
        if (networkZoomKanan != null) networkZoomKanan.SetActive(true);
    }

    public void SelesaiKiri()
    {
        SimpanPreview(parentKabelKiri, previewKiri);
        panelZoomKiri.SetActive(false);
        panelUtama.SetActive(true);
    }

    public void SelesaiKanan()
    {
        SimpanPreview(parentKabelKanan, previewKanan);
        if (networkZoomKanan != null) networkZoomKanan.SetActive(false);
        panelUtama.SetActive(true);
    }

    // =========================
    // SAVE PREVIEW (SINKRONISASI WARNA)
    // =========================

    void SimpanPreview(Transform parentZoom, Image[] preview)
    {
        // Ambil list kabel yang sudah berurutan dari kiri ke kanan
        GeserKabel[] kabelTerurut = DapatkanKabelBerurutan(parentZoom);

        for (int i = 0; i < kabelTerurut.Length; i++)
        {
            if (i >= preview.Length)
            {
                Debug.LogWarning("Preview kurang dari jumlah kabel!");
                return;
            }

            GeserKabel kabel = kabelTerurut[i];

            // 🔥 LOG DETEKTIF: MENGECEK APAKAH ELEMEN DI INSPECTOR SUDAH SESUAI
            Debug.Log($"[SINKRON] Kabel Zoom urutan ke-{i} (Nama: {kabel.gameObject.name}) " +
                      $"dimasukkan ke Preview Utama Element ke-{i} (Nama UI di Inspector: {preview[i].gameObject.name})");

            // Validasi ID Sprite Kabel
            if (kabel.idKabel < 0 || kabel.idKabel >= spriteKabel.Length)
            {
                Debug.LogWarning("ID kabel invalid!");
                continue;
            }

            // Ubah gambar preview di panel utama
            preview[i].sprite = spriteKabel[kabel.idKabel];
        }
    }

    // =========================
    // CEK SUSUNAN AKHIR
    // =========================

    public void TombolCekSusunan()
    {
        bool kiriBenar = Validasi(parentKabelKiri);
        bool kananBenar = Validasi(parentKabelKanan);

        if (kiriBenar && kananBenar)
        {
            Debug.Log("BENAR - STRAIGHT THROUGH");
            
            // Panggil popup selesai dari SlideManager kamu di sini jika berhasil
            SlideManager sm = FindFirstObjectByType<SlideManager>();
            if (sm != null) sm.TampilkanPopupSelesai();
        }
        else
        {
            Debug.Log("SALAH - Silakan periksa kembali susunan kabel Anda!");
        }
    }

    bool Validasi(Transform parent)
    {
        // Ambil list kabel terurut dari kiri ke kanan
        GeserKabel[] kabelTerurut = DapatkanKabelBerurutan(parent);

        if (kabelTerurut.Length == 0) return false;

        for (int i = 0; i < kabelTerurut.Length; i++)
        {
            // Jika ada satu saja ID kabel yang tidak sesuai dengan urutanBenar, kembalikan salah
            if (kabelTerurut[i].idKabel != urutanBenar[i])
            {
                return false;
            }
        }

        return true;
    }
}