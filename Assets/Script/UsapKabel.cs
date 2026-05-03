using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class UsapKabel : MonoBehaviour, IDragHandler
{
    [Header("Referensi Objek")]
    public Image kabelPilin; // Tarik objek Pilin (contoh: PilinOren) ke sini
    public Image kabelLurus; // Tarik objek Lurus (contoh: LurusOren) ke sini
    public ManagerUraikanKabel managerUtama; // Tarik objek Slide 2 ke sini

    [Header("Pengaturan Usap")]
    public float butuhBerapaUsapan = 1500f; // Semakin besar, semakin lama usapnya
    private float usapanSaatIni = 0f;
    private bool sudahLurus = false;

    void Start()
    {
        // Pastikan di awal game: kabel lurus ada tapi transparan (Alpha = 0)
        if (kabelLurus != null)
        {
            Color c = kabelLurus.color;
            c.a = 0f;
            kabelLurus.color = c;
            kabelLurus.gameObject.SetActive(true); 
        }
        
        // Pastikan kabel pilin terlihat utuh (Alpha = 1)
        if (kabelPilin != null)
        {
            Color c = kabelPilin.color;
            c.a = 1f;
            kabelPilin.color = c;
            kabelPilin.gameObject.SetActive(true);
        }
    }

    // Fungsi ini otomatis jalan pas area ini digesek/diusap mouse/jari
    public void OnDrag(PointerEventData eventData)
    {
        if (sudahLurus) return; // Kalau udah lurus, ga usah hitung usapan lagi

        // Tambahkan jarak usapan (magnitude dari pergerakan mouse/jari)
        usapanSaatIni += eventData.delta.magnitude;

        if (usapanSaatIni >= butuhBerapaUsapan)
        {
            sudahLurus = true;
            StartCoroutine(ProsesFadeKabel());
            
            // Lapor ke manajer kalau kabel warna ini udah beres!
            if (managerUtama != null)
            {
                managerUtama.TambahKabelLurus();
            }

            // Matikan area usap ini biar ga menghalangi kabel di bawahnya
            if (GetComponent<Image>() != null)
            {
                GetComponent<Image>().raycastTarget = false; 
            }
        }
    }

    // Efek Transisi Menyilang (Pilin menghilang, Lurus Muncul)
    IEnumerator ProsesFadeKabel()
    {
        float durasiFade = 0.5f; // Berapa detik proses perubahannya
        float waktuBerjalan = 0f;

        Color warnaPilin = kabelPilin.color;
        Color warnaLurus = kabelLurus.color;

        while (waktuBerjalan < durasiFade)
        {
            waktuBerjalan += Time.deltaTime;
            float alpha = waktuBerjalan / durasiFade;

            // Pilin perlahan hilang (1 ke 0)
            warnaPilin.a = Mathf.Lerp(1f, 0f, alpha);
            kabelPilin.color = warnaPilin;

            // Lurus perlahan muncul (0 ke 1)
            warnaLurus.a = Mathf.Lerp(0f, 1f, alpha);
            kabelLurus.color = warnaLurus;

            yield return null;
        }

        // Pastikan nilai akhir sempurna (biar ga nge-bug transparan setengah)
        warnaPilin.a = 0f;
        kabelPilin.color = warnaPilin;
        kabelPilin.gameObject.SetActive(false); // Matikan objek pilin selamanya

        warnaLurus.a = 1f;
        kabelLurus.color = warnaLurus;
    }
}