using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    private UnityEvent<float> onHealthChanged = new UnityEvent<float>();
    private UnityEvent onDeath  = new UnityEvent();

    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;
    private float damage = 25f;

    [SerializeField] private Image healthAmount;
    [SerializeField] private GameObject gameOverPanel;

    private void Awake()
    {
        gameOverPanel.SetActive(false);
        currentHealth = maxHealth;
        
    }
    private void Start()
    { 
            onHealthChanged.AddListener(Damage);
            onDeath.AddListener(GameOver);
    }

    public void OnDamage(InputAction.CallbackContext context)
    {
        if (context.started && onHealthChanged != null)
        {
           onHealthChanged.Invoke(damage);
        }  
    }
    public void Damage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0 && onDeath != null) { 
        
            onDeath.Invoke();
        }
        UpdateUI();
    }

    private void UpdateUI()
    {
        healthAmount.fillAmount = currentHealth/maxHealth;
    }

    public void GameOver()
    {
        gameOverPanel.SetActive(true);
    }

}
