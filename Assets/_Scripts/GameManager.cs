using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int score = 0;
    public int health = 100;
    private bool changingHealth = false;
    [SerializeField]
    TextMeshProUGUI scoreText;
    [SerializeField]
    TextMeshProUGUI healthText;

    [SerializeField] 
    private GameObject redScreen;

    private void Update()
    {
        checkDead();
    }

    public void increaseScore(int amount)
    {
        score += amount;
        scoreText.text = ("Score: " + score);
    }

    public void takeHealth(int hAmount)
    {
        StartCoroutine(takeHealthDelay(hAmount));
    }

    IEnumerator takeHealthDelay(int hAmount)
    {
        if (changingHealth)
        {
            yield return null;
        }
        else
        {
            changingHealth = true;
            redScreen.SetActive(true);
            healthText.color = Color.red;
            health -= hAmount;
            healthText.text = ("HP: " + health);
            yield return new WaitForSeconds((float).25);
            changingHealth = false;
            redScreen.SetActive(false);
            healthText.color = Color.black;
        }
    }

    public void addHealth(int hAmount)
    {
        health += hAmount;
        if (health > 100)
        {
            health = 100;
        }
        healthText.text = ("HP: " + health);
    }

    private void checkDead()
    {
        if (health <= 0)
        {
            SceneManager.LoadScene("Dead");
        }
    }
}
