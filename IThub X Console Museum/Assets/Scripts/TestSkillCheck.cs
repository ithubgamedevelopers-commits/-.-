using UnityEngine;

public class TestSkillCheck : MonoBehaviour
{
    public SkillCheckUI skillCheckUI;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            skillCheckUI.StartCheck(
                () => Debug.Log("Игрок починил! +100"),
                () => {
                    Debug.Log("Игрок промахнулся! -500");
                    skillCheckUI.ResetArrow();
                }
            );
        }
    }
}