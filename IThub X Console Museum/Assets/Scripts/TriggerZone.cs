using UnityEngine;

public class TriggerZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GetComponent<Collider>().enabled = false; // Защита от двойного срабатывания
            SkillCheckManager.Instance.StartSkillCheck(gameObject);
        }
    }
}