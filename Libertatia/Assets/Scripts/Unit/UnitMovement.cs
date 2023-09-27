using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitMovement : MonoBehaviour
{
    Camera myCam;
    NavMeshAgent myAgent;
    public LayerMask ground;

    // Start is called before the first frame update
    void Start()
    {
        myCam = Camera.main;
        myAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        //Moves units to mouse position when right clicking
        if(Input.GetMouseButtonDown(1))
        {

            //List<Vector3> targetPositionList = GetPositionListAround(Input.mousePosition, new float[] { 5f, 10f, 15f}, new int[] { 5, 10, 20 });

            //int randNum = Random.Range(0, targetPositionList.Count);
            //Debug.Log(randNum);


            RaycastHit hit;
            //Ray ray = myCam.ScreenPointToRay(targetPositionList[randNum]);
            Ray ray = myCam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity,ground))
            {
                myAgent.SetDestination(hit.point);
            }
        }
    }



    //private List<Vector3> GetPositionListAround(Vector3 startPosition, float[] ringDistanceArray, int[] ringPositionCountArray)
    //{
    //    List<Vector3> positionList = new List<Vector3>();
    //    positionList.Add(startPosition);

    //    for (int i = 0; i < ringDistanceArray.Length; i++)
    //    {
    //        positionList.AddRange(GetPositionListAround(startPosition, ringDistanceArray[i], ringPositionCountArray[i]));
    //    }

    //    return positionList;
    //}

    //private List<Vector3> GetPositionListAround(Vector3 startPosition, float distance, int positionCount)
    //{
    //    List<Vector3> positionList = new List<Vector3>();

    //    for (int i = 0; i < positionCount; i++)
    //    {
    //        float angle = i * (360f / positionCount);
    //        Vector3 dir = ApplyRotationToVector(new Vector3(1, 0), angle);
    //        Vector3 position = startPosition + dir * distance;
    //        positionList.Add(position);
    //    }

    //    return positionList;
    //}

    //private Vector3 ApplyRotationToVector(Vector3 vec, float angle)
    //{
    //    return Quaternion.Euler(0, 0, angle) * vec;
    //}
}
