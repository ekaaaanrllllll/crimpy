using UnityEngine;
using System.Collections;

public class TransisiLanTester : MonoBehaviour
{
    [Header("Pengaturan View")]
    public CanvasGroup overView;
    public CanvasGroup readyView;

    [Header("Kecepatan Transisi")]
    public float durasiFade = 0.5f;

    void Start()
    {
        // Set kondisi awal saat game mulai: OverView nyala, ReadyView mati
        overView.gameObject.SetActive(true);
        overView.alpha = 1f;

        readyView.gameObject.SetActive(false);
        readyView.alpha = 0f;
    }

    // Fungsi ini yang akan dipanggil saat kabel di-klik
    public void KlikKabelUntukTransisi()
    {
        StartCoroutine(ProsesFadeInOut());
    }

    IEnumerator ProsesFadeInOut()
    {
        // 1. Fade Out OverView perlahan
        float waktu = 0;
        while (waktu < durasiFade)
        {
            waktu += Time.deltaTime;
            overView.alpha = Mathf.Lerp(1f, 0f, waktu / durasiFade);
            yield return null;
        }
        overView.gameObject.SetActive(false);

        // 2. Nyalakan ReadyView dengan Alpha 0
        readyView.gameObject.SetActive(true);
        readyView.alpha = 0f;

        // 3. Fade In ReadyView perlahan
        waktu = 0;
        while (waktu < durasiFade)
        {
            waktu += Time.deltaTime;
            readyView.alpha = Mathf.Lerp(0f, 1f, waktu / durasiFade);
            yield return null;
        }
        readyView.alpha = 1f;
        
        Debug.Log("Transisi Selesai! Kabel siap di-drag ke bawah.");
    }
}