using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Audio Source")]
    [SerializeField] private AudioSource bgmSource;

    [Header("Audio Clip BGM")]
    public AudioClip bgmClip;

    void Awake()
    {
        // Sistem Singleton agar musik tidak mati/mengulang dari awal saat pindah scene
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Otomatis setup AudioSource jika belum dipasang di Inspector
        if (bgmSource == null)
        {
            bgmSource = gameObject.AddComponent<AudioSource>();
        }

        bgmSource.loop = true;
        bgmSource.playOnAwake = false;
    }

    void Start()
    {
        // Memuat settingan volume terakhir yang disimpan player (Default: 0.5)
        float savedVolume = PlayerPrefs.GetFloat("BGM_Volume", 0.5f);
        SetBGMVolume(savedVolume);

        // Putar BGM jika ada clip yang dimasukkan
        if (bgmClip != null)
        {
            PlayBGM(bgmClip);
        }
    }

    public void PlayBGM(AudioClip clip)
    {
        bgmSource.clip = clip;
        bgmSource.Play();
    }

    // Fungsi utama untuk mengubah volume dari Slider
    public void SetBGMVolume(float volume)
    {
        bgmSource.volume = volume;
        // Simpan otomatis ke memory local agar saat game dibuka lagi, volumenya tetap sama
        PlayerPrefs.SetFloat("BGM_Volume", volume);
    }

    // Fungsi pembantu untuk mengambil nilai volume saat ini
    public float GetBGMVolume()
    {
        return bgmSource.volume;
    }
}