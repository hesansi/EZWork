using System;
using UnityEngine;

namespace EZWork
{
    public class EZGrid:MonoBehaviour
    {
        protected EZGrid() { }
        // 行
        public int numOfRows;
        // 列
        public int numOfColumns;
        // 正方形
        //public float gridCellSize;
        // 矩形
        public float gridCellSizeColumnX;
        public float gridCellSizeRowZ;

        public bool showGrid = true;

        public Node[,] nodes { get; set; }

        // 起点
//        public Transform OriginTrans;
        private Vector3 Origin;

        void Awake()
        {
            CalculateGridNodes();
        }

        void CalculateGridNodes()
        {
            Origin = transform.position;//OriginTrans.position;
            nodes = new Node [numOfRows, numOfColumns];
            int index = 0;
            for (int i = 0; i < numOfRows; i++)
            {
                for (int j = 0; j < numOfColumns; j++)
                {
                    Vector3 cellPos = GetGridCellCenter(index);
                    Node node = new Node(cellPos);
                    nodes[i, j] = node;
                    index++;
                }
            }
        }

        public Vector3 GetGridCellCenter(int index)
        {
            Vector3 cellPosition = GetGridCellPosition(index);
            //cellPosition.x += (gridCellSize / 2.0f);
            //cellPosition.z += (gridCellSize / 2.0f);
            cellPosition.x += (gridCellSizeColumnX / 2.0f);
            cellPosition.z += (gridCellSizeRowZ / 2.0f);
            return cellPosition;
        }

        public Vector3 GetGridCellPosition(int index)
        {
            int col = GetColumn(index);
            int row = GetRow(index);
            //float xPosInGrid = col * gridCellSize;
            //float zPosInGrid = row * gridCellSize;
            float xPosInGrid = col * gridCellSizeColumnX;
            float zPosInGrid = row * gridCellSizeRowZ;
            return Origin + new Vector3(xPosInGrid, 0.0f, zPosInGrid);
        }

        public int GetGridIndex(Vector3 pos)
        {
            if (!IsInBounds(pos))
            {
                return -1;
            }

            pos -= Origin;
            //int col = (int)(pos.x / gridCellSize);
            //int row = (int)(pos.z / gridCellSize);
            int col = (int) (pos.x / gridCellSizeColumnX);
            int row = (int) (pos.z / gridCellSizeRowZ);
            return (row * numOfColumns + col);
        }

        public bool IsInBounds(Vector3 pos)
        {
            //float width = numOfColumns * gridCellSize;
            //float height = numOfRows * gridCellSize;
            float width = numOfColumns * gridCellSizeColumnX;
            float height = numOfRows * gridCellSizeRowZ;
            return (pos.x >= Origin.x && pos.x <= Origin.x + width &&
                    pos.z <= Origin.z + height && pos.z >= Origin.z);
        }

        public int GetRow(int index)
        {
            int row = index / numOfColumns;
            return row;
        }

        public int GetColumn(int index)
        {
            int col = index % numOfColumns;
            return col;
        }

        void OnDrawGizmos()
        {
            if (showGrid)
            {
                DebugDrawGrid(transform.position, numOfRows, numOfColumns,
                    gridCellSizeColumnX, gridCellSizeRowZ, Color.yellow);
            }

            Gizmos.DrawSphere(transform.position, 0.5f);
        }

        public void DebugDrawGrid(Vector3 origin, int numRows, int
            numCols, float cellSizeColumn, float cellSizeRow, Color color)
        {
            //float width = (numCols * cellSize);
            //float height = (numRows * cellSize);
            float width = (numCols * cellSizeColumn);
            float height = (numRows * cellSizeRow);
            // Draw the horizontal grid lines  
            for (int i = 0; i < numRows + 1; i++)
            {
                Vector3 startPos = origin + i * gridCellSizeRowZ * new Vector3(0.0f,
                                       0.0f, 1.0f);
                Vector3 endPos = startPos + width * new Vector3(1.0f, 0.0f,
                                     0.0f);
                Debug.DrawLine(startPos, endPos, color);
            }

            // Draw the vertial grid lines  
            for (int i = 0; i < numCols + 1; i++)
            {
                Vector3 startPos = origin + i * gridCellSizeColumnX * new Vector3(1.0f,
                                       0.0f, 0.0f);
                Vector3 endPos = startPos + height * new Vector3(0.0f, 0.0f,
                                     1.0f);
                Debug.DrawLine(startPos, endPos, color);
            }
        }
    }

    public class Node
    {
        public float estimatedCost;
        public Vector3 position;

        public Node()
        {
            position = Vector3.zero;
        }

        public Node(Vector3 pos)
        {
            position = pos;
        }
    }
}