using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Builder : Actor
{
    Building currentBuilding;
    private void Start()
    {
        animationEvent.attackEvent.AddListener(DoWork);
    }
    public void GiveJob(Building job)
    {
        currentBuilding = job;
        job.AssignBuilder();

        if (currentTask != null)
            StopCoroutine(currentTask);

        currentTask = StartCoroutine(StartJob());
        IEnumerator StartJob()
        {
            Vector3 jobPosition = job.transform.position;
            Vector2 randomPosition = Random.insideUnitCircle.normalized * currentBuilding.radius;
            jobPosition.x += randomPosition.x;
            jobPosition.z += randomPosition.y;
            SetDestination(jobPosition);
            yield return WaitForNavMesh();
            transform.LookAt(currentBuilding.transform);
            while (!currentBuilding.IsComplete)
            {
                yield return new WaitForSeconds(1);
                if (!currentBuilding.IsComplete)
                {
                    animator.SetTrigger("Attack");
                }
            }
            currentBuilding = null;
            currentTask = null;
        }
    }
    public bool HasTask()
    {
        return currentTask != null;
    }
    override public void StopTask()
    {
        base.StopTask();
        currentBuilding = null;
    }

    void DoWork()
    {
        if (currentBuilding)
        {
            currentBuilding.Build(10);
        }
    }
}
