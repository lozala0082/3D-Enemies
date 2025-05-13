using UnityEngine;
using TMPro;

public class score : MonoBehaviour
{
    public static score instance;
    [SerializeField]
    TextMeshProUGUI score_counter_TMP;
    [HideInInspector]
    public int score_count;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UpdateCounterUI()
    {
        score_counter_TMP.text = score_count.ToString();
    }
}
