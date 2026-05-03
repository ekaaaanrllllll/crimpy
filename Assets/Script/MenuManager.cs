using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void PindahScene(string namaScene) 
    {
        // Cek apakah nama scene tidak kosong
        if (!string.IsNullOrEmpty(namaScene))
        {
            SceneManager.LoadScene(namaScene);
        }
        else
        {
            Debug.LogError("Nama scene kosong! Masukkan nama scene di Inspector tombol.");
        }
    }
}