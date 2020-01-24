using UnityEngine;
using System;
using System.Threading;

namespace AgaQ.Bricks
{
    /// <summary>
    /// Provides set of unique vertises from mesh
    /// </summary>
    [RequireComponent(typeof(MeshFilter))]
    public class Vertices : MonoBehaviour
    {
        Vector3[] _uniqueVertices;
        VerticesArray[] allVertices;

        Thread thread;
        Mutex mutex = new Mutex();
        bool threadRuning = false;

		void Start()
		{
            if (!threadRuning)
            {
                var meshFilters = GetComponentsInChildren<MeshFilter>();
                allVertices = new VerticesArray[meshFilters.Length];
                for (int i = 0; i < meshFilters.Length; i++)
                {
                    allVertices[i] = new VerticesArray();
                    allVertices[i].vertices = meshFilters[i].mesh.vertices;
                }
                    
                thread = new Thread(PrepareVertices);
                thread.Start();
            }
		}

		void OnDisable()
		{
            if (threadRuning)
            {
                threadRuning = false;
                thread.Join();
            }
		}

		/// <summary>
		/// Return unique vertises.
		/// </summary>
		/// <value>The unique vertises.</value>
		public Vector3[] uniqueVertices
        {
            get
            {
                mutex.WaitOne();
                mutex.ReleaseMutex();

                return _uniqueVertices;
            }
        }

        /// <summary>
        /// Function to run in separate thread, to prepare unique vertises.
        /// </summary>
        void PrepareVertices()
        {
            mutex.WaitOne();

            try
            {
                threadRuning = true;

                int verticesCount = 0;
                foreach (var v in allVertices)
                    verticesCount += v.vertices.Length;

                _uniqueVertices = new Vector3[verticesCount];

                int vIdx = 0;
                int idx;
                int i;
                bool contains;
                foreach (var vert in allVertices)
                {
                    foreach (var v in vert.vertices)
                    {
                        if (vIdx == 0)
                            _uniqueVertices[vIdx++] = v;
                        else
                        {
                            //compare to previos points but no more than 500 points back
                            contains = false;
                            for (i = -1; i >= -500; i--)
                            {
                                idx = vIdx + i;
                                if (idx < 0)
                                    break;
                                if (_uniqueVertices[idx] == v)
                                {
                                    contains = true;
                                    break;
                                }
                            }
                            if (!contains)
                                _uniqueVertices[vIdx++] = v;
                        }
                    }    
                }

                Array.Resize<Vector3>(ref _uniqueVertices, vIdx);

                allVertices = null;
                threadRuning = false;
            }
            finally
            {
                mutex.ReleaseMutex();
            }
        }

        struct VerticesArray
        {
            public Vector3[] vertices;
        }
    }
}
