using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CDUI : MonoBehaviour
{
    [Header("Dash")]
    public Image abilityImage1;
    [SerializeField]
    public float cooldown1 = 5;
    bool isCooldown = false;
    //    public KeyCode ability1;

    [Header("Weapon Attack")]
    public Image abilityImage2;
    [SerializeField]
    public float cooldown2 = 1;
    bool isCooldown2 = false;
    //    public KeyCode ability2;    

    [Header("Ranged Attack")]
    public Image abilityImage3;
    [SerializeField]
    public float cooldown3 = 0.5f;
    bool isCooldown3 = false;


    // Start is called before the first frame update
    void Start()
    {
        abilityImage1.fillAmount = 0;
        abilityImage2.fillAmount = 0;
        abilityImage3.fillAmount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        DashCD();
        WeaponAttackCD();
        RangedAttackCD();
    }

    void DashCD()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isCooldown == false)
        {
            isCooldown = true;
            abilityImage1.fillAmount = 1;
        }

        if (isCooldown)
        {
            abilityImage1.fillAmount -= 1 / cooldown1 * Time.deltaTime;

            if (abilityImage1.fillAmount <= 0)
            {
                abilityImage1.fillAmount = 0;
                isCooldown = false;
            }
        }
    }
    void WeaponAttackCD()
    {
        if (Input.GetMouseButtonDown(0) && isCooldown2 == false)
        {
            isCooldown2 = true;
            abilityImage2.fillAmount = 1;
        }

        if (isCooldown2)
        {
            abilityImage2.fillAmount -= 1 / cooldown2 * Time.deltaTime;

            if (abilityImage2.fillAmount <= 0)
            {
                abilityImage2.fillAmount = 0;
                isCooldown2 = false;
            }
        }
    }
    void RangedAttackCD()
    {
        if (Input.GetKeyDown(KeyCode.F) && isCooldown3 == false)
        {
            isCooldown3 = true;
            abilityImage3.fillAmount = 1;
        }

        if (isCooldown3)
        {
            abilityImage3.fillAmount -= 1 / cooldown3 * Time.deltaTime;

            if (abilityImage3.fillAmount <= 0)
            {
                abilityImage3.fillAmount = 0;
                isCooldown3 = false;
            }
        }
    }
}
