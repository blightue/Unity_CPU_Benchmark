using System.Linq;
using Unity.Mathematics;
using UnityEngine;

using Random = System.Random;

public static class ResultCheck
{
    private static Random random = new System.Random(0);
    
    public static bool CheckIndices(int[] indices, Vector3[] vectors1, Vector3[] vectors2)
    {
        int[] testIndices = new int[10];
        int count = vectors1.Length;
        testIndices.Select(i => random.Next(count)).ToArray();

        foreach (int i in testIndices)
        {
            if(!CheckIndex(indices[i], vectors1[i], vectors2))
                return false;
        }

        return true;
    }

    public static bool CheckIndex(int index, Vector3 result, Vector3[] vectors)
    {
        Vector3 offset = result - vectors[index];
        float minDistance = Vector3.Dot(offset, offset);

        foreach (Vector3 v in vectors)
        {
            Vector3 offsetNew = result - v;
            float distance = Vector3.Dot(offsetNew, offsetNew);

            if (distance < minDistance)
            {
                return false;
            }
        }
        return true;
    }
    
    public static bool CheckIndices(int[] indices, float3[] vectors1, float3[] vectors2)
    {
        int[] testIndices = new int[10];
        int count = vectors1.Length;
        testIndices.Select(i => random.Next(count)).ToArray();

        foreach (int i in testIndices)
        {
            if(!CheckIndex(indices[i], vectors1[i], vectors2))
                return false;
        }

        return true;
    }

    public static bool CheckIndex(int index, float3 result, float3[] vectors)
    {
        float minDistance = math.distancesq(result, vectors[index]);

        foreach (float3 v in vectors)
        {
            float distance = math.distancesq(result, v);

            if (distance < minDistance)
            {
                return false;
            }
        }
        return true;
    }
}
