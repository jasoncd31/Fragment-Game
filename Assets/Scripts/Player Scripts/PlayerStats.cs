using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerStats : MonoBehaviour
{
    //Health Stats
    [SerializeField]
    private int maxHealth = 5;
    private int currentHealth;
    public int mHealth { get { return maxHealth; } }
    public int cHealth { get { return currentHealth; } }
    private int fragmentCount = 0;
    public int cFragments { get { return fragmentCount; } }
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }
    public void changeHealth(int amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        if (currentHealth <= 0)
        {
            deathReset();
        }
        Debug.Log(currentHealth);
    }
    public void fragmentPickup(int amount)
    {
        fragmentCount = fragmentCount += amount;
        
    }
    public void deathReset()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
