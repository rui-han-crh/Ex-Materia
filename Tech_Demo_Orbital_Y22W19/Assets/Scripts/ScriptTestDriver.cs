using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra.Factorization;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Single;

public class ScriptTestDriver : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Matrix<float> A = DenseMatrix.OfArray(new float[,]
        {
            { 2, 1 },
            { 6, 3 },
            { 7, 4 },
        });

        print(A.Rank() == Mathf.Min(A.ColumnCount, A.RowCount));

        Vector<float> B = DenseVector.OfArray(new float[]
        { 3,
          6,
          5
        });

        print(A.Append(B.ToColumnMatrix()));

        Vector<float> x = A.Solve(B);
        
        print(x);
    }

    private void TestA()
    {
        Node stored = new Node(Vector3Int.zero);

        MinHeap<Node> minHeap = new MinHeap<Node>(2);
        for (int i = 0; i < 400; i++)
        {
            Node node = new Node(new Vector3Int(i, 0, 0));
            if (i == 3)
            {
                stored = node;
            }
            node.Value = Random.Range(0, 566);
            minHeap.Add(node);
            if (Random.Range(0, 100) > 80)
            {
                node.Value = Random.Range(566, 2555);
                minHeap.IncreaseKey(node);
            }
        }

        stored.Value = 2;
        minHeap.DecreaseKey(stored);


        int lastOut = int.MaxValue;

        while (!minHeap.IsEmpty())
        {
            Node node = minHeap.Extract();
            print(node);

            Debug.Assert(lastOut >= node.Value, $"Last out was {lastOut}, yet currently it is {node.Value}");
        }
    }
}
