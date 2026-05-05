using UnityEngine;
using TMPro;
using System.Collections;

public class TypewriterEffect : MonoBehaviour
{
    [Header("Pengaturan Teks")]
    [TextArea(3, 10)]
    public string teksPenuh; // Tulis isi petunjukmu di sini
    public TextMeshProUGUI komponenTeksTMP;
    public float jedaPerHuruf = 0.03f;

    void Awake()
    {
        // Memastikan teks kosong saat game baru mulai atau slide dimuat
        if (komponenTeksTMP != null)
        {
            komponenTeksTMP.text = "";
        }
    }

    // Fungsi ini akan dipanggil oleh UIPopUpAnim setelah animasi selesai
    public void MulaiNgetik()
    {
        StopAllCoroutines();
        komponenTeksTMP.text = ""; // Kosongkan lagi untuk memastikan bersih
        StartCoroutine(RoutineKetik());
    }

    IEnumerator RoutineKetik()
    {
        // Sembunyikan teks sebentar sebelum mulai (opsional)
        yield return new WaitForSeconds(0.1f);

        foreach (char huruf in teksPenuh)
        {
            komponenTeksTMP.text += huruf;
            yield return new WaitForSeconds(jedaPerHuruf);
        }
    }
}