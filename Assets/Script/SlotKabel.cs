using UnityEngine;
using UnityEngine.UI;

public class SlotKabel : MonoBehaviour
{
    [Header("Data Kabel")]
    public int kabelIDSaatIni = -1;

    [Header("Komponen UI")]
    public Image imageSlot;

    [Header("Database Sprite Kabel")]
    public Sprite[] daftarSpriteKabel;

    void Awake()
    {
        if (imageSlot == null)
        {
            imageSlot = GetComponent<Image>();
        }
    }

    // =========================================
    // SET KABEL KE SLOT
    // =========================================
    public void SetKabel(int idKabel)
    {
        kabelIDSaatIni = idKabel;

        if (idKabel < 0)
        {
            imageSlot.sprite = null;
            imageSlot.color = Color.clear;
            return;
        }

        if (idKabel >= daftarSpriteKabel.Length)
        {
            Debug.LogWarning("ID kabel melebihi jumlah sprite!");
            return;
        }

        imageSlot.sprite = daftarSpriteKabel[idKabel];
        imageSlot.color = Color.white;
    }

    // =========================================
    // AMBIL SPRITE SEKARANG
    // =========================================
    public Sprite GetSpriteSekarang()
    {
        return imageSlot.sprite;
    }

    // =========================================
    // RESET SLOT
    // =========================================
    public void ResetSlot()
    {
        kabelIDSaatIni = -1;

        imageSlot.sprite = null;
        imageSlot.color = Color.clear;
    }
}