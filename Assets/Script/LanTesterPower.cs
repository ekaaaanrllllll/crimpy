using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LanTesterPower : MonoBehaviour
{
    [Header("Scripts References")]
    public LanTesterManager testMgr; 

    [Header("UI Objects")]
    public RectTransform switchThumb; // PowerToggle
    public Image ledMerah;          // LED_Red

    [Header("Coordinate Settings")]
    public float posX_Off;          // Angka di Inspector kamu: -234.2
    public float posX_On;           // Angka di Inspector kamu: -203.7

    [Header("Animation Feel")]
    public float slideDuration = 0.15f;
    public float blinkInterval = 0.35f;

    private bool isKabelPlugged = false; 
    private bool isPowerOn = false;      
    private Coroutine blinkingCoroutine;
    private Coroutine slidingCoroutine;

    void Awake()
    {
        // Posisi awal selalu OFF
        if (switchThumb != null)
        {
            switchThumb.anchoredPosition = new Vector2(posX_Off, switchThumb.anchoredPosition.y);
        }
        if (ledMerah != null) SetLEDAlpha(0f);
    }

    public void AktifkanSwitchInput()
    {
        isKabelPlugged = true;
        Debug.Log("CONSOLE: Kabel terdeteksi! Siap dinyalakan.");
    }

    public void TogglePowerState()
    {
        if (!isKabelPlugged)
        {
            Debug.Log("CONSOLE: Colok kabel dulu!");
            return;
        }

        // --- LOGIKA TOGGLE (BOLAK-BALIK) ---
        isPowerOn = !isPowerOn; 

        if (isPowerOn)
        {
            // PROSES ON
            Debug.Log("CONSOLE: Power ON!");
            
            if (slidingCoroutine != null) StopCoroutine(slidingCoroutine);
            slidingCoroutine = StartCoroutine(AnimateSwitchSlide(posX_On));

            if (blinkingCoroutine != null) StopCoroutine(blinkingCoroutine);
            blinkingCoroutine = StartCoroutine(AnimateLEDBlinking());

            // --- HAPUS TANDA // DI BAWAH INI BOS ---
            if(testMgr != null) testMgr.MulaiSequence(true); 
        }
        else
        {
            // PROSES OFF
            Debug.Log("CONSOLE: Power OFF!");

            if (slidingCoroutine != null) StopCoroutine(slidingCoroutine);
            slidingCoroutine = StartCoroutine(AnimateSwitchSlide(posX_Off));

            if (blinkingCoroutine != null) StopCoroutine(blinkingCoroutine);
            SetLEDAlpha(0f);

            // --- HAPUS TANDA // DI BAWAH INI BOS ---
            if(testMgr != null) testMgr.MulaiSequence(false);
        }
    }

    IEnumerator AnimateSwitchSlide(float targetX)
    {
        Vector2 startPos = switchThumb.anchoredPosition;
        Vector2 targetPos = new Vector2(targetX, startPos.y);
        float elapsed = 0;

        while (elapsed < slideDuration)
        {
            elapsed += Time.deltaTime;
            switchThumb.anchoredPosition = Vector2.Lerp(startPos, targetPos, elapsed / slideDuration);
            yield return null;
        }
        switchThumb.anchoredPosition = targetPos;
    }

    IEnumerator AnimateLEDBlinking()
    {
        while (isPowerOn)
        {
            SetLEDAlpha(1f);
            yield return new WaitForSeconds(blinkInterval);
            SetLEDAlpha(0f);
            yield return new WaitForSeconds(blinkInterval);
        }
        SetLEDAlpha(0f);
    }

    void SetLEDAlpha(float alphaValue)
    {
        if (ledMerah == null) return;
        Color c = ledMerah.color;
        c.a = alphaValue;
        ledMerah.color = c;
    }

    void OnEnable()
    {
        isKabelPlugged = false;
        isPowerOn = false;
        if(blinkingCoroutine != null) StopCoroutine(blinkingCoroutine);
        if(slidingCoroutine != null) StopCoroutine(slidingCoroutine);
        Awake();
    }
}