using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

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
    public DateTime date;
    void Start()
    {
      dayUi.tmp = GetComponentInChildren<TextMeshProUGUI>();
      dayUi.image = GetComponent<Image>();
    }
  }
}
