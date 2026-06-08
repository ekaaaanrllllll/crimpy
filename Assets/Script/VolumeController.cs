using UnityEngine;
using UnityEngine.UI;

public class VolumeController : MonoBehaviour
{
    [Header("UI Slider Reference")]
    [Tooltip("Tarik UI Slider komponen BGM ke sini")]
    public Slider bgmSlider;

    void OnEnable()
    {
        // Saat popup terbuka, samakan posisi slider dengan volume BGM aktif saat ini
        if (AudioManager.instance != null && bgmSlider != null)
        {
            bgmSlider.value = AudioManager.instance.GetBGMVolume();
            
            // Daftarkan fungsi pemicu saat slider digeser secara dinamis
            bgmSlider.onValueChanged.RemoveAllListeners();
            bgmSlider.onValueChanged.AddListener(HandleBGMVolumeChange);
        }
    }

    void HandleBGMVolumeChange(float value)
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.SetBGMVolume(value);
        }
    }
}