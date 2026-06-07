using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    [Header("Semua Panel UI")]
    public GameObject mainMenuPanel;
    public GameObject settingsPanel;
    public GameObject infoPanel;
    public GameObject petunjukPanel;
    public GameObject cpatpPanel;

    // Fungsi untuk mematikan semua panel terlebih dahulu
    private void HideAllPanels()
    {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(false);
        infoPanel.SetActive(false);
        petunjukPanel.SetActive(false);
        cpatpPanel.SetActive(false);
    }

    // Fungsi-fungsi yang akan dipanggil oleh Tombol
    public void OpenMainMenu() { HideAllPanels(); mainMenuPanel.SetActive(true); }
    public void OpenSettings() { HideAllPanels(); settingsPanel.SetActive(true); }
    public void OpenInfo() { HideAllPanels(); infoPanel.SetActive(true); }
    public void OpenPetunjuk() { HideAllPanels(); petunjukPanel.SetActive(true); }
    public void OpenCPATP() { HideAllPanels(); cpatpPanel.SetActive(true); }
}