using UnityEngine;

public static class AI_Tools
{
    /// <summary>
    /// Return the closest object of type(layer) if there is one.
    /// If not, return null
    /// </summary>
    /// <param name="_fromPosition">center of the checking sphere</param>
    /// <param name="_radius">radius of the checking sphere</param>
    /// <param name="_layerToCheckName"> layermask of the object to get</param>
    /// <param name="_triggerInteraction">the object to get have a trigger? (or just a collider)</param>
    /// <returns></returns>
    public static GameObject CheckForObject(Vector3 _fromPosition, float _radius, string _layerToCheckName, bool _triggerInteraction)
    {
        LayerMask layerToSelectOnlyEnemies = 1 << LayerMask.NameToLayer(_layerToCheckName);
        QueryTriggerInteraction tempTrigger = (_triggerInteraction)? QueryTriggerInteraction.Collide: QueryTriggerInteraction.Ignore;

        Collider[] tempObjectDetected = Physics.OverlapSphere(_fromPosition, _radius, layerToSelectOnlyEnemies, tempTrigger);

        if (tempObjectDetected.Length > 0)
        {
            float minDistance = float.MaxValue;
            int enemyindex = -1;

            for (int i = 0; i < tempObjectDetected.Length; i++)
            {
                float tempDist = Vector3.Distance(tempObjectDetected[i].transform.position, _fromPosition);
                if (tempDist < minDistance)
                {
                    minDistance = tempDist;
                    enemyindex = i;
                }
            }

            if (enemyindex != -1)
            {
                return tempObjectDetected[enemyindex].gameObject;
            }
        }

        return null;
    }

}
