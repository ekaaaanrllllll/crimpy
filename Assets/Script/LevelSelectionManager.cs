using UnityEngine;
using UnityEngine.UI; // Wajib ditambahkan untuk ngatur UI Button
using UnityEngine.SceneManagement;

public class LevelSelectionManager : MonoBehaviour
{
    // Ini akan jadi tempat kita masukin tombol Pertemuan 1, 2, 3, 4 dari Unity
    public Button[] tombolPertemuan; 

    void Start()
    {
        // Mengambil data "MateriTerbuka" dari sistem.
        // Angka 1 di belakang artinya: kalau belum ada data sama sekali, nilai default-nya adalah 1.
        int materiTerbuka = PlayerPrefs.GetInt("MateriTerbuka", 1);

        // Ngecek satu-satu tombol yang ada di list
        for (int i = 0; i < tombolPertemuan.Length; i++)
        {
            // Karena array dimulai dari 0 (tombol 1 = index 0), kita pakai i + 1
            if (i + 1 > materiTerbuka)
            {
                // Kalau urutan tombol lebih besar dari materi yang terbuka, matikan tombolnya (Lock)
                tombolPertemuan[i].interactable = false;
            }
        }
    }

    // Fungsi ini dipanggil pas tombol di-klik untuk masuk ke materi
    public void PilihMateri(string namaSceneMateri)
    {
        SceneManager.LoadScene(namaSceneMateri);
    }
}