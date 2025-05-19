using System;
using System.Collections.Generic;
using Scripts.Calendar.Date;
using UnityEngine;
using Utils;

namespace Scripts.AllData
{
  /// <summary>
  /// 캘린더 데이터 캐싱 클래스
  /// </summary>
  [System.Serializable]
  public class CalendarManager
  {
    [NonSerialized] private List<Day> days = new();
    [NonSerialized] private Dictionary<int, Dictionary<int, List<int>>> Data = new();
    [SerializeField] private List<YearData> _data = new();

    private const int TOTAL_DATE_NUM = 42;

    public Day GetDay(DateTime end)    
    {
      int endDay = end.Date.Day;
      return days.Find(day => day.day <= endDay);
    }

    public bool IsDateRange(DateTime date)
    {
      var dataManager = DataManager.Instance;
      string id = dataManager.Task.CurrentTaskId;
      (DateTime, DateTime) taskRange = dataManager.Task.GetTaskDateRange(id);
      if (date.Month < taskRange.Item1.Month || date.Month > taskRange.Item2.Month) return false;

      return true;
    }

    public void SetDays(List<Day> tempDays)
    {
      days = tempDays;
    }

    public List<Day> GetDays()
    {
      return days;
    }

    /// <summary>
    /// 지정된 연도와 월에 데이터(Json)가 저장되있으면 반환, 없을시 생성
    /// </summary>
    public List<int> GetDaysData(int year, int month)
    {
      if (!Data.ContainsKey(year) || !Data.ContainsKey(month))
      {
        GenerateDaysForMonth(year, month);
      }

      return Data[year][month];
    }

    /// <summary>
    /// 지정된 연도와 월에 대한 Day계산 및 데이터 캐싱(Json저장) 
    /// </summary>
    private void GenerateDaysForMonth(int year, int month)
    {
      List<int> days = DayCalculate(year, month);

      if (!Data.ContainsKey(year))
      {
        Data[year] = new Dictionary<int, List<int>>();
      }

      Data[year][month] = days;
      Save();
    }

    private List<int> DayCalculate(int year, int month)
    {
      List<int> days = new List<int>();
      DateTime firstDay = new DateTime(year, month, 1);
      int lastIndex = DateTime.DaysInMonth(year, month);
      int firstIndex = (int)firstDay.DayOfWeek;

      for (int i = 0; i < TOTAL_DATE_NUM; i++)
      {
        if (i >= firstIndex && i < firstIndex + lastIndex)
        {
          days.Add(i - firstIndex + 1);
        }
        else
        {
          days.Add(0);
        }
      }

      return days;
    }

    public CalendarManager Load()
    {
      CalendarManager tempList = JsonManager.LoadJson<CalendarManager>("Calendar");
      if (tempList == null)
      {
        tempList = new CalendarManager();
      }
      return tempList;
    }

    private void Save()
    {
      DictionaryToList();
      JsonManager.SaveJson("Calendar", this);
    }

    private void DictionaryToList()
    {
      _data.Clear();
      foreach (var yearDatas in Data)
      {
        YearData yearData = new YearData { Year = yearDatas.Key };
        foreach (var monthDatas in yearDatas.Value)
        {
          MonthData monthData = new MonthData { Month = monthDatas.Key, Days = monthDatas.Value };
          yearData.Month.Add(monthData);
        }
        _data.Add(yearData);
      }
    }

    [System.Serializable]
    public class YearData
    {
      public int Year;
      public List<MonthData> Month = new List<MonthData>();
    }

    [System.Serializable]
    public class MonthData
    {
      public int Month;
      public List<int> Days = new List<int>();
    }
  }
}