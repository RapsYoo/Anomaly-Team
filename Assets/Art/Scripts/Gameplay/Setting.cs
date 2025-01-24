using UnityEngine;
using UnityEngine.SceneManagement;

public class Setting : MonoBehaviour
{
    [SerializeField] private GameObject settingMenuPanel;
    [SerializeField] private GameObject pilihanMenuPanel;
    [SerializeField] private GameObject storePanel;
    

    private bool isPilihanMenuOpen = false;

    void Start()
    {
        settingMenuPanel.SetActive(true);
        pilihanMenuPanel.SetActive(false);
        storePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    void Update()
    {
        if (storePanel.activeSelf )
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            if (isPilihanMenuOpen)
            {
                BackButtonPilihanMenu();
            }
            else
            {
                PilihanButton();
            }
        }
    }

    public void InGameButton(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void PilihanButton()
    {
        settingMenuPanel.SetActive(false);
        pilihanMenuPanel.SetActive(true);
        isPilihanMenuOpen = true;
        Time.timeScale = 0f;
    }

    public void BackButtonPilihanMenu()
    {
        pilihanMenuPanel.SetActive(false);
        settingMenuPanel.SetActive(true);
        isPilihanMenuOpen = false;
        Time.timeScale = 1f;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Time.timeScale = 1f;
    }
}
