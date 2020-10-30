using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


namespace Utils
{
// 创建网格需要 MeshFilter和MeshRenderer组件
// 然后通过MeshRenderer中的Material来设置材质
// 网格中要保存的顶点放在vertices中
class UtilsTool
{
    public static GameObject CreatePlane(Vector3 LeftBottomPosition, int length, Transform MyParent, Material Mymaterial = null)
    {
        GameObject ClickMark = new GameObject();
        MeshFilter Mymeshfilter = ClickMark.AddComponent<MeshFilter>();
        MeshRenderer Mymeshrenderer = ClickMark.AddComponent<MeshRenderer>();
        float x = LeftBottomPosition.x;
        float y = MyParent.localPosition.y;
        float z = LeftBottomPosition.z;
        x *= length;
        z *= length;
        x += MyParent.transform.localPosition.x;
        z += MyParent.transform.localPosition.z;
        ClickMark.transform.localPosition = new Vector3(x, y, z);
        if(Mymaterial == null)
        {
            Mymeshrenderer.material = new Material(Shader.Find("Diffuse"));
            Mymeshrenderer.material.color = Color.blue;
        }
        else 
            Mymeshrenderer.material = Mymaterial;
        Mymeshrenderer.shadowCastingMode = ShadowCastingMode.Off;
        Mymeshfilter.mesh.vertices = new Vector3 []
        {
            new Vector3(0, y, 0),
            new Vector3(length, y, 0),
            new Vector3(0, y, length),
            new Vector3(length, y, length)
        };
        // foreach(Vector3 c in Mymeshfilter.mesh.vertices)
        // {
        //     Debug.Log(c+"  c");
        // }
        Mymeshfilter.mesh.triangles = new int[]{3, 1, 2, 2, 1, 0};
        Mymeshfilter.mesh.RecalculateNormals();
        ClickMark.transform.SetParent(MyParent);
        return ClickMark;
    }

    class Node
    {
        Vector2Int Pos;

    }
    public static Queue<Vector2Int> BFS(int r, int c, ref int[,] map, Vector2Int StartP, Vector2Int EndP)
    {
        Vector2Int [] Parent = Enumerable.Repeat(new Vector2Int(-1, -1), r*c+10).ToArray();
        bool [] vis = Enumerable.Repeat(false, r*c+10).ToArray();
        int x = StartP.x;
        int y = StartP.y;
        Queue<Vector2Int> q = new Queue<Vector2Int>();
        Queue<Vector2Int> res = new Queue<Vector2Int>();
        if(x < 0|| x >= c||y < 0||y >= r)
            return q;
        int [,] dir = {{0, 1}, {0, -1}, {-1, 0}, {1, 0}};
        vis[y*c + x] = true;
        q.Enqueue(StartP);
        Vector2Int Ans = new Vector2Int();
        while(q.Count != 0)
        {
            Vector2Int NowPos = q.Dequeue();
            int nx, ny;
            nx = NowPos.x;
            ny = NowPos.y;
            if(NowPos == EndP)
            {
                Ans = NowPos;
                break;
            }
            for(int i = 0; i < 4; ++i)
            {
                int tnx = nx + dir[i, 0];
                int tny =ny + dir[i, 1];
                if(tnx < 0||tnx>=c||tny < 0|| tny >= r)continue;
                if(vis[tny*c + tnx] == true)continue;
                q.Enqueue(new Vector2Int(tnx, tny));
                vis[tny*c + tnx] = true;
                Parent[tny*c + tnx] = NowPos;
            }
        }
        Vector2Int Tmp = new Vector2Int(-1, -1);
        while(Parent[Ans.y*c + Ans.x] != Tmp)
        {
            res.Enqueue(Ans);
            Ans = Parent[Ans.y*c + Ans.x];
        }
        return res;
    }
}



// void  tttCreatePlane()
// {
//     GameObject plane = new GameObject ();
//     plane.name  = "plane";
//     //  添加组件
//     MeshFilter mfilter =plane.AddComponent<MeshFilter> ();
//     MeshRenderer render =   plane.AddComponent<MeshRenderer> ();
//     // 添加默认的材质
//     render.material=new Material(Shader.Find("Diffuse"));
//     //mfilter.mesh  = new Mesh ();
//     Mesh mesh = mfilter.mesh;
//     //  设置三个顶点
//     mesh .vertices = new Vector3[] {
//         new Vector3 (0, 0, 0),
//         new Vector3 (0, 1, 0),
//         new Vector3 (1, 1, 0)
//     };
//     GameObject a0 = GameObject.CreatePrimitive (PrimitiveType.Cube);
//     a0.transform.position = mesh .vertices [0];
//     Vector3 miniScale = new Vector3 (0.1f, 0.1f, 0.1f);
//     a0.transform.localScale = miniScale;
//     a0.GetComponent<MeshRenderer> ().material.color = Color.red;
//     GameObject a1 = GameObject.CreatePrimitive (PrimitiveType.Sphere);
//     a1.transform.position = mesh .vertices [1];
//     a1.transform.localScale = miniScale;
//     a1.GetComponent<MeshRenderer> ().material.color = Color.green;
//     GameObject a2 = GameObject.CreatePrimitive (PrimitiveType.Cylinder);
//     a2.transform.position = mesh .vertices [2];
//     a2.transform.localScale = miniScale;
//     a2.GetComponent<MeshRenderer> ().material.color = Color.blue;
//     //  设置三角形 （面）
//     mesh.triangles = new int[]{
//         0,1,2
//     };
//     // 重新计算法线
//     mesh.RecalculateNormals ();
// }
}

