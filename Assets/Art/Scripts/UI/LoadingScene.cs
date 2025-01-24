using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingScene : MonoBehaviour
{
    [SerializeField] private GameObject loadingScreen; // UI Loading Screen
    [SerializeField] private Slider loadingBarSlider; // Slider untuk progress loading
    [SerializeField] private float minLoadingTime = 2f; // Waktu minimum loading dalam detik
    [SerializeField] private float sliderSpeed = 0.5f; // Kecepatan slider saat bergerak (lebih besar = lebih cepat)

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        float startTime = Time.unscaledTime;
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        loadingScreen.SetActive(true);

        float progress = 0f;

        // Simulasi loading
        while (progress < 1f)
        {
            progress += Time.deltaTime / 2f; // Simulasi loading dalam 2 detik
            loadingBarSlider.value = Mathf.Clamp01(progress);
            Debug.Log($"Simulated loading progress: {progress}");
            yield return null;
        }

        // Aktifkan scene baru
        operation.allowSceneActivation = true;
    }

}
