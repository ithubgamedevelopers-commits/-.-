using UnityEngine;

public class TriggerZone : MonoBehaviour
{
    private BreakdownPoint myPoint;
    private Collider2D myCollider;

    private void Awake()
    {
        // Ищем компонент точки на этом же объекте или на родителе (если триггер - дочерний объект)
        myPoint = GetComponentInParent<BreakdownPoint>();
        myCollider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Проверяем, что точка существует и она действительно сломана
            if (myPoint != null && myPoint.IsBroken)
            {
                // Временно отключаем коллайдер, чтобы не спамить вызовами во время мини-игры
                myCollider.enabled = false;
                
                // Передаем именно компонент BreakdownPoint (this или myPoint)
                SkillCheckManager.Instance.StartSkillCheck(myPoint);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Игрок ушел, не починив компьютер -> включаем коллайдер обратно, 
            // чтобы он мог вернуться и попробовать снова
            myCollider.enabled = true;
        }
    }

    // Этот метод мы вызовем из SkillCheckManager, если игрок промахнулся
    public void ReenableTrigger()
    {
        myCollider.enabled = true;
    }
}