using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationTask : Task {

    [SerializeField]
    float reachableDistance;

    public override List<CollectTarget> Init(Transform playerController)
    {
        List<CollectTarget> targetList = new List<CollectTarget>();

        Vector3 initialPos = playerController.position;
        for (int i = 0; i < NumOfCollectTargets; i++)
        {

            Vector2 circle;
            Vector3 pos;
            float distance = reachableDistance;

            do
            {
                circle = UnityEngine.Random.insideUnitCircle.normalized;
                pos = initialPos + new Vector3(circle.x, 0, circle.y) * reachableDistance;

                distance = reachableDistance;
                if (i != 0)
                {
                    distance = (pos - targetList[i - 1].gameObject.transform.position).magnitude;
                }

            } while (distance < reachableDistance / 2.2f);


            GameObject gm = Instantiate(collectTargetPrefab);
            gm.transform.position = pos;
            targetList.Add(gm.GetComponent<CollectTarget>());
            targetList[i].TargetIndex = i;
        }

        return targetList;
    }
}
