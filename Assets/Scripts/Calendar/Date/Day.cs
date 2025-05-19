using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class DayUi
{
  public TextMeshProUGUI tmp;
  public Image image;
}

namespace Scripts.Calendar.Date
{
  public class Day : MonoBehaviour
  {
    public DayUi dayUi;
    public int day;
    void Start()
    {
      dayUi.tmp = GetComponentInChildren<TextMeshProUGUI>();
      dayUi.image = GetComponent<Image>();
    }
  }
}
