using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public int attackForce;
    public int health;

    public Slider slider;

    public void Damage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }
    private void Die()
    {
        Destroy(gameObject);
        slider.value += 1; // 减少回合条的值
    
    }
    // Start is called before the first frame update
    void Start()
    {
        GameObject gameObject = GameObject.Find("RoundSlider");
        if (gameObject != null)
        {
            slider = gameObject.GetComponent<Slider>();
        }
        else
        {
            Debug.LogError("RoundSlider object not found in the scene.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
