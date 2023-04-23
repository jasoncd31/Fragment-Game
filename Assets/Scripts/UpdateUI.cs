using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpdateUI : MonoBehaviour
{
    public TextMeshProUGUI healthUI;
    public PlayerStats player;
    // Start is called before the first frame update
    void Start()
    {
        updateHealthText();
    }

    // Update is called once per frame
    void Update()
    {
        updateHealthText();
    }
    private void updateHealthText()
    {
        healthUI.text = "Health: " + player.cHealth;
    }
}
