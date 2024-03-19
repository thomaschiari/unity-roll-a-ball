using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float speed = 0;
    private Rigidbody rb;
    private int count;
    private float movementX;
    private float movementY;
    public TextMeshProUGUI countText;
    public GameObject winTextObject;
    public float timeRemaining = 10f;
    public TextMeshProUGUI timeText;
    private bool timerIsRunning = false;
    public GameObject loseTextObject;
    public AudioClip victorySound;
    public AudioClip collectSound;
    public AudioClip enemySound;
    public AudioClip loseSound;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent <Rigidbody>();
        count = 0;
        SetCountText();
        winTextObject.SetActive(false);
        loseTextObject.SetActive(false);
        timerIsRunning = true;
        UpdateTimerDisplay();
    }

    void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();
        movementX = movementVector.x;
        movementY = movementVector.y;
        
    }

    void FixedUpdate()
    {
        Vector3 movement = new Vector3(movementX, 0.0f, movementY);
        rb.AddForce(movement * speed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PickUp"))
        {
            AudioSource.PlayClipAtPoint(collectSound, transform.position, 1.0f);
            other.gameObject.SetActive(false);
            count++;
            SetCountText();
            timeRemaining += 3;
        }
        else if (other.gameObject.CompareTag("Enemy"))
        {
            AudioSource.PlayClipAtPoint(enemySound, transform.position, 1.0f);
            other.gameObject.SetActive(false);
            timeRemaining -= 3;
            if (timeRemaining < 0)
            {
                timeRemaining = 0;
            }
            UpdateTimerDisplay();
        }
    }

    void SetCountText()
    {
        countText.text = "Count: " + count.ToString();
        if (count >= 8) 
        {
            AudioSource.PlayClipAtPoint(victorySound, transform.position, 1.0f);
            winTextObject.SetActive(true);
            timerIsRunning = false;
            StartCoroutine(ReturnToMenuAfterDelay(5));
        }
    }

    IEnumerator ReturnToMenuAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("menu");
    }

    void Update()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                UpdateTimerDisplay();
            }
            else
            {
                timeRemaining = 0;
                timerIsRunning = false;
                loseTextObject.SetActive(true);
                AudioSource.PlayClipAtPoint(loseSound, transform.position, 1.0f);
                StartCoroutine(ReturnToMenuAfterDelay(5));
            }
        }
    }

    void UpdateTimerDisplay()
    {
        timeText.text = "Time: " + Mathf.Round(timeRemaining).ToString();
    }
}
