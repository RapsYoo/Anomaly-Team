using TMPro;
using UnityEngine;
using System.Collections;

public class WaterSurfaceDetector : MonoBehaviour
{
    [SerializeField] private float maxOxygen = 100f;
    [SerializeField] private float currentOxygen;
    [SerializeField] private float oxygenRate = 1f;
    private bool isUnderwater = false;

    [SerializeField] private int goldCostForRespawn = 100;
    private GoldManager goldManager;
    [SerializeField] private UnityEngine.UI.Slider oxygenSlider;

    [SerializeField] private TextMeshProUGUI warningText;
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private Transform startingPoint;
    [SerializeField] private GameObject gameOverScreen;

    private bool isOxygenDepleted = false;
    private Coroutine oxygenDepletedCoroutine;

    void Start()
    {
        currentOxygen = maxOxygen;
        UpdateOxygenSlider();
        goldManager = FindObjectOfType<GoldManager>();
        countdownText.text = "";
        gameOverScreen.SetActive(false);
    }

    void Update()
    {
        if (isUnderwater)
        {
            currentOxygen -= oxygenRate * Time.deltaTime;
            if (currentOxygen <= 0)
            {
                currentOxygen = 0;
                if (!isOxygenDepleted)
                {
                    isOxygenDepleted = true;
                    oxygenDepletedCoroutine = StartCoroutine(HandleOxygenDepletionCountdown());
                }
            }
        }
        else
        {
            if (currentOxygen < maxOxygen)
            {
                currentOxygen += oxygenRate * Time.deltaTime;
                if (currentOxygen > maxOxygen)
                {
                    currentOxygen = maxOxygen;
                }
            }

            if (isOxygenDepleted)
            {
                isOxygenDepleted = false;
                if (oxygenDepletedCoroutine != null)
                {
                    StopCoroutine(oxygenDepletedCoroutine);
                    countdownText.text = "";
                }
            }
        }

        oxygenSlider.value = currentOxygen;
    }

    public void RespawnPlayer()
    {
        DeductGoldForRespawn();
        ResetPlayerState();
        Time.timeScale = 1;
        gameOverScreen.SetActive(false);
    }

    public void ReturnToMainMenu()
    {
        DeductGoldForRespawn();
        ResetPlayerState();
        Time.timeScale = 1;
        Debug.Log("Kembali ke Main Menu");
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    private void DeductGoldForRespawn()
    {
        if (goldManager != null)
        {
            int currentGold = goldManager.GetGold();
            if (currentGold >= goldCostForRespawn)
            {
                goldManager.ChangeGold(-goldCostForRespawn);
                Debug.Log("Gold berkurang 100 untuk respawn.");
            }
            else
            {
                goldManager.ChangeGold(-currentGold);
                Debug.Log("Semua gold habis untuk respawn.");
            }
        }
        else
        {
            Debug.LogError("GoldManager tidak ditemukan!");
        }
    }

    private void ResetPlayerState()
    {
        if (startingPoint != null)
        {
            transform.position = startingPoint.position;
            Debug.Log("Respawn ke posisi awal: " + startingPoint.position);
        }
        else
        {
            Debug.LogError("StartingPoint tidak diatur!");
        }

        currentOxygen = maxOxygen;
        isOxygenDepleted = false;
        UpdateOxygenSlider();
    }

    private void UpdateOxygenSlider()
    {
        oxygenSlider.maxValue = maxOxygen;
        oxygenSlider.value = currentOxygen;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("WaterSurface"))
        {
            isUnderwater = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("WaterSurface"))
        {
            isUnderwater = false;
        }
    }

    private IEnumerator HandleOxygenDepletionCountdown()
    {
        float countdown = 5f;
        StartCoroutine(ShowWarningTemporary("Oksigen habis! Tinggalkan air atau cari bubble", countdown));

        while (countdown > 0)
        {
            countdownText.text = "Game Over in: " + Mathf.CeilToInt(countdown);
            yield return new WaitForSeconds(1f);
            countdown--;

            if (!isUnderwater || !isOxygenDepleted)
            {
                countdownText.text = "";
                yield break;
            }
        }

        countdownText.text = "";
        Time.timeScale = 0;
        gameOverScreen.SetActive(true);
    }

    public void AddOxygen(float amount)
    {
        currentOxygen += amount;
        if (currentOxygen > maxOxygen)
        {
            currentOxygen = maxOxygen;
        }
        UpdateOxygenSlider();
        Debug.Log($"Oksigen bertambah sebanyak {amount}. Total oksigen sekarang: {currentOxygen}");
    }

    private IEnumerator ShowWarningTemporary(string message, float delay)
    {
        warningText.text = message;
        yield return new WaitForSecondsRealtime(delay);
        warningText.text = "";
    }
}
