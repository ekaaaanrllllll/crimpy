using UnityEngine;
using System.Collections;

public class UIPopUpAnim : MonoBehaviour
{
    [Header("1. Hubungkan ke Script Ngetik")]
    // Variabel ini wajib ada supaya kita bisa memanggil script di teksnya
    public TypewriterEffect scriptNgetik; 

    [Header("2. Pengaturan Punchy Scale-In")]
    public Vector3 targetScale = Vector3.one; 
    public Vector3 overshootScale = new Vector3(1.1f, 1.1f, 1.1f); 
    public float durasiTumbuh = 0.3f;
    public float durasiMembal = 0.15f;

    void OnEnable()
    {
        MulaiPopUp();
    }

    public void MulaiPopUp()
    {
        StopAllCoroutines(); 
        StartCoroutine(AnimatePunchyScaleIn());
    }

    IEnumerator AnimatePunchyScaleIn()
    {
        // --- LANGKAH 1: RESET ---
        transform.localScale = Vector3.zero;

        // --- LANGKAH 2: FASE TUMBUH (0 -> 1.1) ---
        float elapsedTime = 0f;
        while (elapsedTime < durasiTumbuh)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsedTime / durasiTumbuh);
            transform.localScale = Vector3.Lerp(Vector3.zero, overshootScale, t);
            yield return null;
        }
        transform.localScale = overshootScale;

        // --- LANGKAH 3: FASE MEMBAL (1.1 -> 1.0) ---
        elapsedTime = 0f;
        while (elapsedTime < durasiMembal)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsedTime / durasiMembal);
            transform.localScale = Vector3.Lerp(overshootScale, targetScale, t);
            yield return null;
        }
        transform.localScale = targetScale; 

        // --- LANGKAH 4: MULAI NGETIK ---
        // Kita panggil ngetiknya DI SINI (setelah animasi pop-up selesai)
        if (scriptNgetik != null)
        {
            scriptNgetik.MulaiNgetik();
        }
        else 
        {
            Debug.LogWarning("Woi, Script Ngetik belum ditarik ke slot Inspector!");
        }
    }
}