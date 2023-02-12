using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;

namespace Benchmark
{
    public class BenchMarkBurst : MonoBehaviour
    {
        public int count = 10000;
        public UIDocument uiDocument;
        public int maxLoop = 10;
    
        private Stopwatch sw = new Stopwatch();

        [HideInInspector]public int loop;
        private ScrollView scrollView;
        
        private Random random;
    
        void Start()
        {
            random = new Random((uint)DateTime.Now.Ticks);
            scrollView = uiDocument.rootVisualElement.Query<ScrollView>("LogScrollView");
            loop = maxLoop;
        }

        private void Update()
        {
            if(loop >= 10) return;
            JobsystemLoop();
        }

        private void SingleThreadLoop()
        {
            Log2UI($"Start loop {++loop}");
        
            sw.Restart();
        
            float3[] vectors1 = RandomVectorArray(count, random);
            float3[] vectors2 = RandomVectorArray(count, random);
        
            sw.Stop();
        
            Log2UI($"RandomVectorArray generated. speed: {sw.ElapsedMilliseconds} ms");
        
            sw.Restart();
        
            int[] indices = GetClosedIndicesSingleThread(vectors1, vectors2);
        
            sw.Stop();
        
            Log2UI($"GetClosedIndices finished. speed: {sw.ElapsedMilliseconds} ms \n");
            Log2UI("");
        }

        private void JobsystemLoop()
        {
            Log2UI($"Start loop {++loop}");
        
            sw.Restart();
        
            float3[] vectors1 = RandomVectorArray(count, random);
            float3[] vectors2 = RandomVectorArray(count, random);
        
            sw.Stop();
        
            Log2UI($"RandomVectorArray generated. speed: {sw.ElapsedMilliseconds} ms");
        
            sw.Restart();

            NativeArray<float3> floatsA = new NativeArray<float3>(vectors1, Allocator.TempJob);
            NativeArray<float3> floatsB = new NativeArray<float3>(vectors2, Allocator.TempJob);

            NativeArray<int> result = new NativeArray<int>(count, Allocator.TempJob);

            BenchmarkJob benchmarkJob = new BenchmarkJob()
            {
                count = count,
                floatsA = floatsA,
                floatsB = floatsB,
                result = result
            };

            JobHandle jobHandle = benchmarkJob.Schedule(count, 64);
            jobHandle.Complete();
            
            int[] indices = benchmarkJob.result.ToArray();

            benchmarkJob.floatsA.Dispose();
            benchmarkJob.floatsB.Dispose();
            benchmarkJob.result.Dispose();
        
            sw.Stop();
        
            Log2UI($"GetClosedIndices finished. speed: {sw.ElapsedMilliseconds} ms");
            Log2UI($"Check result: {ResultCheck.CheckIndices(indices, vectors1, vectors2)}");
            Log2UI("--------------------------------------------");
        }
        
        [BurstCompile]
        float3[] RandomVectorArray(int count, Random random)
        {
            float3[] vectors = new float3[count];
            for (int i = 0; i < count; i++)
            {
                vectors[i] = new float3(random.NextFloat(), random.NextFloat(), random.NextFloat());
            }
            return vectors;
        }
        
        [BurstCompile]
        int[] GetClosedIndicesSingleThread(float3[] vectors1, float3[] vectors2)
        {
            int[] indices = new int[vectors1.Length];

            for(int i = 0; i < vectors1.Length; i++)
            {
                indices[i] = GetClosedIndex(vectors1[i], vectors2);
            }

            return indices;
        }
        
        [BurstCompile]
        int GetClosedIndex(float3 target, float3[] vectices)
        {
            float minDistance = float.MaxValue;
            int minIndex = 0;
            for (int j = 0; j < vectices.Length; j++)
            {
                float3 offset = target - vectices[j];
                float distance = math.dot(offset, offset);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    minIndex = j;
                }
            }
            return minIndex;
        }
        
        private void Log2UI(string log)
        {
            Label label = new Label(log);
            scrollView.Add(label);
        }
    }
}

[BurstCompile]
public struct BenchmarkJob : IJobParallelFor
{
    [ReadOnly]
    public int count;
    [ReadOnly]
    public NativeArray<float3> floatsA;
    [ReadOnly]
    public NativeArray<float3> floatsB;

    public NativeArray<int> result;

    [BurstCompile]
    public void Execute(int index)
    {
        float minLength2 = float.MaxValue;
        int min_index = 0;

        for (int i = 0; i < count; i++)
        {
            float length2 = math.distancesq(floatsA[index], floatsB[i]);
            if (length2 < minLength2)
            {
                minLength2 = length2;
                min_index = i;
            }
        }

        result[index] = min_index;
    }
}
