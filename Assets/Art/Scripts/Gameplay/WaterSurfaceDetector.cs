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

    [SerializeField] private AudioClip oxygenAddSFX; 
    private AudioSource audioSource;

    // Boost Speed Variables
    [SerializeField] private float maxBoostPower = 6f; 
    [SerializeField] private float boostRegenRate = 1f; 
    [SerializeField] private float oxygenDepletionMultiplier = 2f; 
    [SerializeField] private UnityEngine.UI.Slider boostPowerSlider; 
    private float currentBoostPower; 
    private bool isBoosting = false; 
    [SerializeField] private float boostMultiplier = 2f; 

    private Rigidbody2D rb;

    void Start()
    {
        currentOxygen = maxOxygen;
        UpdateOxygenSlider();
        goldManager = FindObjectOfType<GoldManager>();
        countdownText.text = "";
        gameOverScreen.SetActive(false);

        currentBoostPower = maxBoostPower;
        if (boostPowerSlider != null)
        {
            boostPowerSlider.maxValue = maxBoostPower;
            boostPowerSlider.value = currentBoostPower;
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioSource tidak ditemukan pada GameObject ini.");
        }

        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D tidak ditemukan pada GameObject ini.");
        }
    }

    void Update()
    {
        if (Input.GetMouseButton(1)) 
        {
            isBoosting = true;

            if (currentBoostPower > 0)
            {
                currentBoostPower -= Time.deltaTime;
                if (currentBoostPower < 0)
                {
                    currentBoostPower = 0;
                }
                rb.velocity *= boostMultiplier; 
            }
            else
            {
                if (isUnderwater)
                {
                    currentOxygen -= oxygenRate * oxygenDepletionMultiplier * Time.deltaTime;
                    if (currentOxygen < 0)
                    {
                        currentOxygen = 0;
                    }
                }
            }
        }
        else
        {
            isBoosting = false;

            if (currentBoostPower < maxBoostPower)
            {
                currentBoostPower += boostRegenRate * Time.deltaTime;
                if (currentBoostPower > maxBoostPower)
                {
                    currentBoostPower = maxBoostPower;
                }
            }
        }

        if (boostPowerSlider != null)
        {
            boostPowerSlider.value = currentBoostPower;
        }

        if (isUnderwater)
        {
            currentOxygen -= oxygenRate * Time.deltaTime;
            if (currentOxygen < 0)
            {
                currentOxygen = 0;
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
        }

        if (currentOxygen <= 0 && !isOxygenDepleted)
        {
            isOxygenDepleted = true;
            oxygenDepletedCoroutine = StartCoroutine(HandleOxygenDepletionCountdown());
        }

        UpdateOxygenSlider();
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

        PlayOxygenAddSFX(); 
    }

    private void PlayOxygenAddSFX()
    {
        if (audioSource != null && oxygenAddSFX != null)
        {
            audioSource.PlayOneShot(oxygenAddSFX);
        }
        else
        {
            Debug.LogWarning("SFX oksigen tidak diatur atau AudioSource tidak ditemukan.");
        }
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
        currentBoostPower = maxBoostPower;
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

    private IEnumerator ShowWarningTemporary(string message, float delay)
    {
        warningText.text = message;
        yield return new WaitForSecondsRealtime(delay);
        warningText.text = "";
    }
}
