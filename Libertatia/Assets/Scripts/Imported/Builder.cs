using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Builder : Actor
{
    private Building currentBuilding;
    [SerializeField] private bool isBuilding = false;

    public bool IsBuilding
    {
        get
        {
            return isBuilding;
        }
    }

    public void GiveJob(Building job)
    {
        currentBuilding = job;
        isBuilding = true;
        Vector3 jobPosition = job.transform.position;
        Vector2 randomPosition = Random.insideUnitCircle.normalized * job.Radius;
        jobPosition.x += randomPosition.x;
        jobPosition.z += randomPosition.y;
        SetDestination(jobPosition);
        UnitSelections.Instance.RemoveSelection(gameObject);

        //if (currentTask != null)
        //    StopCoroutine(currentTask);

        //currentTask = StartCoroutine(StartJob());
        //IEnumerator StartJob()
        //{
        //    Vector3 jobPosition = job.transform.position;
        //    Vector2 randomPosition = Random.insideUnitCircle.normalized * currentBuilding.radius;
        //    jobPosition.x += randomPosition.x;
        //    jobPosition.z += randomPosition.y;
        //    SetDestination(jobPosition);
        //    yield return WaitForNavMesh();
        //    transform.LookAt(currentBuilding.transform);
        //    while (!currentBuilding.IsComplete)
        //    {
        //        yield return new WaitForSeconds(1);
        //        if (!currentBuilding.IsComplete)
        //        {
        //            animator.SetTrigger("Attack");
        //        }
        //    }
        //    currentBuilding = null;
        //    currentTask = null;
        //}
    }

    public void Free()
    {
        currentBuilding = null;
        isBuilding = false;
    }
}
