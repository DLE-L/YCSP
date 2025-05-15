using System;
using Utils;
using UnityEngine;
using System.Collections.Generic;

namespace Scripts.AllData
{
  [System.Serializable]
  public class TodoCompleteManager
  {
    [NonSerialized] private Dictionary<string, int> GroupIndexLookup = new();
    [NonSerialized] private Dictionary<string, CompleteGroup> GroupLookup = new();
    [NonSerialized] private Dictionary<string, TodoComplete> CompleteLookup = new();    
    [SerializeField] private List<CompleteGroup> TodoCompletes = new();

    public void InitCompleteGroup(string taskId)
    {
      GroupLookup[taskId] = new() { TaskId = taskId, TodoCompletes = new() };
    }

    public void InitTodoComplete(string todoId)
    {
      CompleteLookup[todoId] = new() { TodoId = todoId, Complete = 0 };
    }

    public void InitTodoCompleteManager()
    {
      int count = 0;
      foreach (var group in GroupLookup)
      {
        GroupIndexLookup[group.Key] = count++;
        CompleteGroup completeGroup = new() { TaskId = group.Key, TodoCompletes = new() };
        foreach (var complete in CompleteLookup)
        {
          TodoComplete todoComplete = new() { TodoId = complete.Key, Complete = 0 };
          completeGroup.TodoCompletes.Add(todoComplete);
        }
        TodoCompletes.Add(completeGroup);
      }
      Save();
    }

    public void SetLookupTable()
    {
      CompleteLookup.Clear();
      GroupLookup.Clear();
      GroupIndexLookup.Clear();
      int count = 0;
      foreach (var group in TodoCompletes)
      {
        string key = group.TaskId;
        GroupLookup[key] = group;
        GroupIndexLookup[key] = count++;
        foreach (var complete in group.TodoCompletes)
        {
          CompleteLookup[key] = complete;
        }
      }
    }

    public void RemoveTodoComplete(string taskId)
    {
      int index = GroupIndexLookup[taskId];
      TodoCompletes.RemoveAt(index);

      SetLookupTable();
      Save();
    }

    private void Save(string fileName = "TodoComplete")
    {
      JsonManager.SaveJson<TodoCompleteManager>(fileName, this);
    }

    public TodoCompleteManager Load(string fileName = "TodoComplete")
    {
      var tempComplete = JsonManager.LoadJson<TodoCompleteManager>(fileName);
      if (tempComplete != null)
      {
        TodoCompletes = tempComplete.TodoCompletes;
        SetLookupTable();
      }
      return tempComplete ?? new TodoCompleteManager();
    }
  }

  [System.Serializable]
  public class CompleteGroup
  {
    public string TaskId;
    public List<TodoComplete> TodoCompletes = new();
  }

  [System.Serializable]
  public class TodoComplete
  {
    public string TodoId;
    public int Complete;
  }
}