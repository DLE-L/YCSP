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
    
    public void InitTodoCompleteManager(TodoGroup todoGroup)
    {
      string taskId = todoGroup.TaskId;
      if (GroupLookup.ContainsKey(taskId)) return;

      CompleteGroup completeGroup = new()
      {
        TaskId = todoGroup.TaskId,
        TodoCompletes = new()
      };

      foreach (var todoData in todoGroup.TodoDatas)
      {
        GroupLookup[taskId] = completeGroup;
        GroupIndexLookup[taskId] = GroupIndexLookup.Count - 1;
        foreach (var todoSet in todoData.TodoSets)
        {
          TodoComplete todoComplete = new()
          {
            TodoId = todoSet.TodoId,
            Complete = 0
          };

          CompleteLookup[todoSet.TodoId] = todoComplete;
          completeGroup.TodoCompletes.Add(todoComplete);
        }
      }
      TodoCompletes.Add(completeGroup);
      Save();
    }

    public void SetTodoComplete(string todoId, int isComplete)
    {
      CompleteLookup[todoId].Complete = isComplete;
      Save();
    }

    public TodoComplete GetTodoComplete(string todoId)
    {
      return CompleteLookup[todoId];
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
          CompleteLookup[complete.TodoId] = complete;
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