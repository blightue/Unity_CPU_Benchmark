using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using Debug = UnityEngine.Debug;

public class BenchMarkMonoBehavior : MonoBehaviour
{
    public int count = 10000;
    public UIDocument uiDocument;
    public int maxLoop = 10;

    private Stopwatch sw = new Stopwatch();

    [HideInInspector]public int loop;
    private ScrollView scrollView;
    
    void Start()
    {
        scrollView = uiDocument.rootVisualElement.Query<ScrollView>("LogScrollView");
        loop = maxLoop;
    }
    
    void Update()
    {
        if (loop >= 10) return;
        
        Log2UI($"Start loop {++loop}");
        
        sw.Restart();
        
        Vector3[] vectors1 = RandomVectorArray(count);
        Vector3[] vectors2 = RandomVectorArray(count);
        
        sw.Stop();
        
        Log2UI($"RandomVectorArray generated. speed: {sw.ElapsedMilliseconds} ms");
        
        sw.Restart();
        
        int[] indices = GetClosedIndicesMultiThread(vectors1, vectors2);
        
        sw.Stop();
        
        Log2UI($"GetClosedIndices finished. speed: {sw.ElapsedMilliseconds} ms");
        Log2UI($"Check result: {ResultCheck.CheckIndices(indices, vectors1, vectors2)}");
        Log2UI("-----------------------------------------");
    }
    
    
    Vector3[] RandomVectorArray(int count)
    {
        Vector3[] vectors = new Vector3[count];
        for (int i = 0; i < count; i++)
        {
            vectors[i] = new Vector3(Random.value, Random.value, Random.value);
        }
        return vectors;
    }

    int[] GetClosedIndicesMultiThread(Vector3[] vectors1, Vector3[] vectors2)
    {
        int[] indices = new int[vectors1.Length];

        Parallel.For(0, vectors1.Length, i =>
        {
            indices[i] = GetClosedIndex(vectors1[i], vectors2);
        });

        return indices;
    }

    int[] GetClosedIndicesSingleThread(Vector3[] vectors1, Vector3[] vectors2)
    {
        int[] indices = new int[vectors1.Length];

        for(int i = 0; i < vectors1.Length; i++)
        {
            indices[i] = GetClosedIndex(vectors1[i], vectors2);
        }

        return indices;
    }

    int GetClosedIndex(Vector3 target, Vector3[] vectices)
    {
        float minDistance = float.MaxValue;
        int minIndex = 0;
        for (int j = 0; j < vectices.Length; j++)
        {
            Vector3 offset = target - vectices[j];
            float distance = Vector3.Dot(offset, offset);
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
