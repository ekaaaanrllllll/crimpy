using UnityEngine;
using UnityEngine.UI;
using TMPro; // Wajib kalau teks popup pakai TextMeshPro

public class SusunKabelManager : MonoBehaviour
{
    [Header("1. Pengaturan Panel UI")]
    public GameObject panelUtama;
    public GameObject panelZoomKiri;
    public GameObject panelZoomKanan;

    [Header("2. Indikator Progress")]
    public GameObject centangKiri;  // Ikon centang hijau di panel utama jika kiri selesai
    public GameObject centangKanan; // Ikon centang hijau di panel utama jika kanan selesai

    [Header("3. Slot Hasil di Tampilan Utama")]
    public Image[] slotKiriUtama;  // 8 Image kecil di kabel kiri utama
    public Image[] slotKananUtama; // 8 Image kecil di kabel kanan utama

    [Header("4. Slot Interaktif di Panel Zoom")]
    // Skrip drag-drop kamu harus memasukkan ID warna ke komponen Slot ini
    public SlotKabel[] slotKiriZoom;  // 8 Slot besar di panel kiri
    public SlotKabel[] slotKananZoom; // 8 Slot besar di panel kanan

    [Header("5. UI Popup Evaluasi")]
    public GameObject popupEvaluasi;     // Panel popup (bisa pakai popup mascot kemarin)
    public TMP_Text teksPesanEvaluasi;   // Komponen teks di dalam popup untuk memberi tahu yang salah
    public Button tombolNextSlide;       // Tombol next yang hanya muncul kalau susunan BENAR

    // Standar Urutan Warna T568B:
    // 0 = Putih-Oren, 1 = Oren, 2 = Putih-Hijau, 3 = Biru
    // 4 = Putih-Biru, 5 = Hijau, 6 = Putih-Cokelat, 7 = Cokelat
    private int[] urutanBenarT568B = { 0, 1, 2, 3, 4, 5, 6, 7 };

    private bool kiriSudahDisusun = false;
    private bool kananSudahDisusun = false;

    void Start()
    {
        // Setup awal: Tampilkan panel utama, sembunyikan sisanya
        panelUtama.SetActive(true);
        panelZoomKiri.SetActive(false);
        panelZoomKanan.SetActive(false);
        if (popupEvaluasi != null) popupEvaluasi.SetActive(false);
        
        if (centangKiri != null) centangKiri.SetActive(false);
        if (centangKanan != null) centangKanan.SetActive(false);
    }

    // --- FUNGSI PINDAH PANEL ---
    public void BukaZoomKiri()
    {
        panelUtama.SetActive(false);
        panelZoomKiri.SetActive(true);
    }

    public void BukaZoomKanan()
    {
        panelUtama.SetActive(false);
        panelZoomKanan.SetActive(true);
    }

    // Dipanggil saat klik tombol "Kembali ke Tampilan Utama" di Panel Zoom Kiri
    public void SelesaiKiri()
    {
        kiriSudahDisusun = true;
        if (centangKiri != null) centangKiri.SetActive(true);
        
        // Copy warna visual dari zoom ke utama
        UpdateVisualUtama(slotKiriZoom, slotKiriUtama);
        
        panelZoomKiri.SetActive(false);
        panelUtama.SetActive(true);
    }

    // Dipanggil saat klik tombol "Kembali ke Tampilan Utama" di Panel Zoom Kanan
    public void SelesaiKanan()
    {
        kananSudahDisusun = true;
        if (centangKanan != null) centangKanan.SetActive(true);
        
        // Copy warna visual dari zoom ke utama
        UpdateVisualUtama(slotKananZoom, slotKananUtama);

        panelZoomKanan.SetActive(false);
        panelUtama.SetActive(true);
    }

    void UpdateVisualUtama(SlotKabel[] slotZoom, Image[] slotUtama)
    {
        for (int i = 0; i < slotZoom.Length; i++)
        {
            if (slotZoom[i] != null && slotUtama[i] != null)
            {
                // Ambil gambar/warna dari item yang sedang menempati slot tersebut
                slotUtama[i].color = slotZoom[i].GetWarnaSekarang();
            }
        }
    }

    // ==========================================
    // --- LOGIKA UTAMA: CEK SUSUNAN KABEL ---
    // ==========================================
    public void TombolCekSusunan()
    {
        // 1. Validasi apakah pemain malas langsung pencet CEK padahal belum nyusun
        if (!kiriSudahDisusun || !kananSudahDisusun)
        {
            TampilkanPopupPesan("Kamu harus menyusun KEDUA sisi kabel (Kiri dan Kanan) terlebih dahulu sebelum melakukan pengecekan!", false);
            return;
        }

        string namaWarnaT568B(int id) {
            string[] nama = { "Putih-Oren", "Oren", "Putih-Hijau", "Biru", "Putih-Biru", "Hijau", "Putih-Cokelat", "Cokelat" };
            return (id >= 0 && id < nama.Length) ? nama[id] : "Kosong/Salah";
        }

        // 2. CEK KABEL SISI KIRI
        for (int i = 0; i < slotKiriZoom.Length; i++)
        {
            if (slotKiriZoom[i].kabelIDSaatIni != urutanBenarT568B[i])
            {
                string pesanSalah = $"<color=red>SUSUNAN SALAH!</color>\n\nPeriksa kembali <b>Kabel Sisi KIRI</b> pada urutan ke-<b>{i + 1}</b>.\nHarusnya adalah warna <b>{namaWarnaT568B(urutanBenarT568B[i])}</b>.";
                TampilkanPopupPesan(pesanSalah, false);
                return; // Stop perulangan, kasih tahu salah pertama saja biar ga pusing
            }
        }

        // 3. CEK KABEL SISI KANAN
        for (int i = 0; i < slotKananZoom.Length; i++)
        {
            if (slotKananZoom[i].kabelIDSaatIni != urutanBenarT568B[i])
            {
                string pesanSalah = $"<color=red>SUSUNAN SALAH!</color>\n\nPeriksa kembali <b>Kabel Sisi KANAN</b> pada urutan ke-<b>{i + 1}</b>.\nHarusnya adalah warna <b>{namaWarnaT568B(urutanBenarT568B[i])}</b>.";
                TampilkanPopupPesan(pesanSalah, false);
                return; // Stop perulangan
            }
        }

        // 4. JIKA LOLOS SEMUA BERARTI BENAR!
        TampilkanPopupPesan("<color=green>LUAR BIASA PERFECT!</color>\n\nKedua sisi kabel telah disusun dengan standar T568B yang benar. Silakan lanjut ke tahap berikutnya!", true);
    }

    void TampilkanPopupPesan(string pesan, bool isBenar)
    {
        if (popupEvaluasi != null)
        {
            popupEvaluasi.SetActive(true);
            if (teksPesanEvaluasi != null) teksPesanEvaluasi.text = pesan;
            
            // Tombol Next Slide diatur aktif HANYA jika jawabannya benar sempurna
            if (tombolNextSlide != null) tombolNextSlide.gameObject.SetActive(isBenar);
        }
    }

    public void TutupPopupEvaluasi()
    {
        if (popupEvaluasi != null) popupEvaluasi.SetActive(false);
    }
}