using UnityEngine;
using UnityEngine.EventSystems;

public class GeserKabel : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Identitas Kabel")]
    public string namaWarna; 

    // Variabel untuk mengingat asal mula kabel sebelum ditarik
    private Vector3 posisiAwal;
    private int urutanHierarchy;

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 1. Catat posisi X, Y, Z awal sebelum kabel mulai ditarik
        posisiAwal = transform.position;
        urutanHierarchy = transform.GetSiblingIndex();

        // 2. Tarik gambar kabel ini ke lapisan paling depan agar tidak tertutup kabel lain saat digeser
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 3. KUNCI SUMBU Y! Kabel hanya akan mengikuti mouse di sumbu X (kiri-kanan)
        transform.position = new Vector3(eventData.position.x, posisiAwal.y, posisiAwal.z);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        GeserKabel kabelTarget = null;
        float jarakTerdekat = Mathf.Infinity;

        // 4. Cari kabel mana yang posisinya paling dekat dengan tempat kita melepas klik
        foreach (Transform saudara in transform.parent)
        {
            GeserKabel scriptSaudara = saudara.GetComponent<GeserKabel>();
            
            // Cek hanya kabel lain (bukan kabel yang sedang kita pegang)
            if (scriptSaudara != null && scriptSaudara != this)
            {
                // Cek seberapa dekat jarak X kita dengan kabel saudara ini
                float jarak = Mathf.Abs(transform.position.x - saudara.position.x);
                
                // Jika jaraknya cukup dekat (toleransi 150 pixel, bisa kamu ubah kalau kurang sensitif)
                if (jarak < jarakTerdekat && jarak < 150f) 
                {
                    jarakTerdekat = jarak;
                    kabelTarget = scriptSaudara;
                }
            }
        }

        // 5. PROSES TUKAR POSISI (SWAP)
        if (kabelTarget != null)
        {
            // Ambil posisi kabel yang mau ditabrak
            Vector3 posisiTarget = kabelTarget.transform.position;

            // Lempar kabel yang ditabrak ke tempat asal kita
            kabelTarget.transform.position = new Vector3(posisiAwal.x, kabelTarget.transform.position.y, kabelTarget.transform.position.z);
            
            // Taruh kabel kita di tempat kabel yang ditabrak tadi
            transform.position = new Vector3(posisiTarget.x, posisiAwal.y, posisiAwal.z);
        }
        else
        {
            // 6. Kalau dilepas sembarangan (tidak kena kabel lain), kembalikan ke tempat asal (Snap back)
            transform.position = posisiAwal;
        }

        // Kembalikan urutan lapisan gambar seperti semula
        transform.SetSiblingIndex(urutanHierarchy);
    }
}