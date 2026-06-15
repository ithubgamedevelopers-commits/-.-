using UnityEngine;

public class AddScore : MonoBehaviour
{
    [Tooltip("каждый кадр будет шанс на активацию равный 1/chance(чем меньше chance тем больше шанс активации)"), Range(2, 2147483647)]
    public int chance;  
    public int scoreValue = 10;
    public bool active = false;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || active == true)
        {
            ScoreManager.Instance.AddScore(scoreValue);
            /*Destroy(gameObject);*/
            active = false;
        }
    }

    private void Update()
    {
        int rnd =  Random.Range(0, chance);
        Debug.Log(rnd);
        if (rnd > chance - 1 || active == false)
            active = true;
    }
}
