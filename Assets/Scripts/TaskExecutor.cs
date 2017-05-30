using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public delegate void Task();

public class TaskExecutor : MonoBehaviour
{

    private Queue<Task> TaskQueue = new Queue<Task>();
    private object _queueLock = new object();

    // Update is called once per frame
    void Update()
    {
        lock (_queueLock)
        {
            if (TaskQueue.Count > 0)
                TaskQueue.Dequeue()();
        }
    }

    public void ScheduleTask(Task newTask)
    {
        //int chunkCount = (TerrainManager.RENDER_DISTANCE / TerrainManager.CHUNK_SIZE) * (TerrainManager.RENDER_DISTANCE / TerrainManager.CHUNK_SIZE);
        //lock (_queueLock)
        //{
        //    if (TaskQueue.Count < chunkCount)
        //        TaskQueue.Enqueue(newTask);
        //}

        TaskQueue.Enqueue(newTask);

    }
}
