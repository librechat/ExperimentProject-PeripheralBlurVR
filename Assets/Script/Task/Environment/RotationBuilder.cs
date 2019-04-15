using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationBuilder : BaseEnvBuilder {

    [SerializeField]
    float reachableDistance;

    public List<Vector3> CollectTaskPosition;

    public override void Init(Transform playerController)
    {
        CollectTaskPosition = new List<Vector3>();

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
                    distance = (pos - CollectTaskPosition[i - 1]).magnitude;
                }

            } while (distance < reachableDistance / 2.2f);

            CollectTaskPosition.Add(pos);
        }

        return;
    }
}
