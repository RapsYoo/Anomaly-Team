using System.Collections;
using UnityEngine;

public class PlayerController2 : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float boostMultiplier = 2f; // Multiplier for the speed boost
    [SerializeField] private Animator anime;
    [SerializeField] private AudioClip catchFishSFX;
    [SerializeField] private AudioClip stunSFX;
    [SerializeField] private AudioClip moveSFX;

    private Vector2 movement;
    private Rigidbody2D rb;
    private AudioSource audioSource;
    private AudioSource moveAudioSource;

    private GoldManager goldManager;
    private bool isStunned = false;
    private bool isBoosting = false; // Track if the player is boosting

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (PlayerPrefs.HasKey("InGameSea_X") && PlayerPrefs.HasKey("InGameSea_Y"))
        {
            float x = PlayerPrefs.GetFloat("InGameSea_X");
            float y = PlayerPrefs.GetFloat("InGameSea_Y");
            transform.position = new Vector3(x, y, 0);
        }

        goldManager = FindObjectOfType<GoldManager>();
        if (goldManager == null)
        {
            Debug.LogError("GoldManager tidak ditemukan");
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioSource tidak ditemukan di GameObject ini!");
        }

        moveAudioSource = gameObject.AddComponent<AudioSource>();
        moveAudioSource.clip = moveSFX;
        moveAudioSource.loop = true;
        moveAudioSource.playOnAwake = false;
    }

    private void Update()
    {
        if (isStunned) return;

        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        HandleMovementSFX();
        FlipAnimation();

        // Check if the right mouse button is being held
        isBoosting = Input.GetMouseButton(1);

        if (Input.GetMouseButtonDown(0))
        {
            TryCatchFish();
        }
    }

    private void FixedUpdate()
    {
        if (movement != Vector2.zero)
        {
            rb.gravityScale = 0;

            // Apply boost multiplier if boosting
            float currentSpeed = isBoosting ? speed * boostMultiplier : speed;

            rb.velocity = movement.normalized * currentSpeed * Time.deltaTime;
        }
        else
        {
            rb.velocity = Vector2.zero;
            rb.gravityScale = 10;
        }
    }

    private void HandleMovementSFX()
    {
        if (movement != Vector2.zero)
        {
            if (!moveAudioSource.isPlaying)
            {
                moveAudioSource.Play();
            }
        }
        else
        {
            if (moveAudioSource.isPlaying)
            {
                moveAudioSource.Stop();
            }
        }
    }

    private void FlipAnimation()
    {
        if (movement.x != 0)
        {
            anime.SetBool("SwimX", true);
            anime.SetBool("SwimY", false);
            anime.SetBool("Swim", false);

            transform.localScale = new Vector3(movement.x < 0 ? -1 : 1, 1, 1);
        }
        else if (movement.y > 0)
        {
            anime.SetBool("Swim", true);
            anime.SetBool("SwimX", false);
            anime.SetBool("SwimY", false);
        }
        else if (movement.y < 0)
        {
            anime.SetBool("SwimY", true);
            anime.SetBool("SwimX", false);
            anime.SetBool("Swim", false);
        }
        else
        {
            anime.SetBool("Swim", false);
            anime.SetBool("SwimX", false);
            anime.SetBool("SwimY", false);
        }
    }

    private void TryCatchFish()
    {
        Collider2D[] nearbyObjects = Physics2D.OverlapCircleAll(transform.position, 1f);

        foreach (var obj in nearbyObjects)
        {
            if (obj.CompareTag("Fish"))
            {
                if (goldManager != null)
                {
                    goldManager.ChangeGold(10);
                }

                if (audioSource != null && catchFishSFX != null)
                {
                    audioSource.PlayOneShot(catchFishSFX);
                }

                Destroy(obj.gameObject);
                break;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 1f);
    }

    public void Stun(float duration)
    {
        if (audioSource != null && stunSFX != null)
        {
            audioSource.PlayOneShot(stunSFX);
        }

        StartCoroutine(StunCoroutine(duration));
    }

    private IEnumerator StunCoroutine(float duration)
    {
        Debug.Log("Player stunned!");
        isStunned = true;

        // Hentikan suara gerakan saat stun
        if (moveAudioSource.isPlaying)
        {
            moveAudioSource.Stop();
        }

        yield return new WaitForSeconds(duration);

        Debug.Log("Player recovered from stun.");
        isStunned = false;
    }

    private void OnDisable()
    {
        PlayerPrefs.SetFloat("InGameSea_X", transform.position.x);
        PlayerPrefs.SetFloat("InGameSea_Y", transform.position.y);
        PlayerPrefs.Save();
    }
}