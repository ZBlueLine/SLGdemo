﻿using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Global;

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
            new Vector3(0, 0, 0),
            new Vector3(length, 0, 0),
            new Vector3(0, 0, length),
            new Vector3(length, 0, length)
        };
        // foreach(Vector3 c in Mymeshfilter.mesh.vertices)
        // {
        //     Debug.Log(c+"  c");
        // }
        Mymeshfilter.mesh.triangles = new int[]{3, 1, 2, 2, 1, 0};
        Mymeshfilter.mesh.RecalculateNormals();
        // ClickMark.transform.SetParent(MyParent, false);
        return ClickMark;
    }

    public class Node
    {
        public Node(Vector2Int p, int len)
        {
            Pos = p;
            StepLen = len;
        }
        Vector2Int pos;
        int stepLen;

        public Vector2Int Pos { get => pos; set => pos = value; }
        public int StepLen { get => stepLen; set => stepLen = value; }
    }

    static int [] vis;
    public static int GetVis(int x, int z, int Width)
    {
        return vis[z*Width + x];
    }
    public static Queue<Vector2Int> BFS(int r, int c, GridStatus[,] map, Vector2Int StartP, Vector2Int EndP, int Range = 0x3f3f3f, int MinAttackRange = 0, int AttackRange = 1)
    {
        Vector2Int [] Parent = Enumerable.Repeat(new Vector2Int(-1, -1), r*c+10).ToArray();
        vis = Enumerable.Repeat(0, r*c+10).ToArray();
        int x = StartP.x;
        int y = StartP.y;
        Queue<Node> q = new Queue<Node>();
        Queue<Vector2Int> res = new Queue<Vector2Int>();
        if(x < 0|| x >= c||y < 0||y >= r)
            return res;
        int [,] dir = {{0, 1}, {0, -1}, {-1, 0}, {1, 0}};
        bool hasPath = false;
        q.Enqueue(new Node(StartP, 0));
        Vector2Int Ans = new Vector2Int();
        while(q.Count != 0)
        {
            Node NowPos = q.Dequeue();
            int nx, ny;
            nx = NowPos.Pos.x;
            ny = NowPos.Pos.y;
            vis[ny*c + nx] = 1;
            if(hasPath)break;
            if(NowPos.StepLen + AttackRange > Range)
            {
                if(NowPos.StepLen + AttackRange - MinAttackRange > Range)
                    vis[ny*c + nx] = 2;
                else
                    vis[ny*c + nx] = 3;
            }
            if((NowPos.StepLen + 1) > Range)
            {
                vis[ny*c + nx] = 2;
                continue;
            }
            for(int i = 0; i < 4; ++i)
            {
                int tnx = nx + dir[i, 0];
                int tny =ny + dir[i, 1];
                if(tnx < 0||tnx>=c||tny < 0|| tny >= r)continue;
                if(vis[tny*c + tnx] != 0)continue;
                if(new Vector2Int(tnx, tny) == EndP)
                {
                    hasPath = true;
                    Ans = NowPos.Pos;
                    if(map[tnx, tny].statucode > GlobalVar.CannotMove)  
                    {
                        Parent[tny*c + tnx] = NowPos.Pos;                 
                        Ans = new Vector2Int(tnx, tny);
                    }
                    break;
                }
                if(map[tnx, tny].statucode <= GlobalVar.CannotMove)
                {
                    if(map[tnx, tny].statucode == GlobalVar.IsEnemy||map[tnx, tny].statucode == GlobalVar.IsChara)
                            vis[tny*c + tnx] = 2;
                    continue;
                }
                q.Enqueue(new Node(new Vector2Int(tnx, tny), NowPos.StepLen+1));
                Parent[tny*c + tnx] = NowPos.Pos;
            }
        }
        Vector2Int Tmp = new Vector2Int(-1, -1);
        while(hasPath&&Parent[Ans.y*c + Ans.x] != Tmp)
        {
            res.Enqueue(Ans);
            Ans = Parent[Ans.y*c + Ans.x];
        }
        return res;
    }
    public static Queue<Vector2Int> UBFS(int r, int c, GridStatus[,] map, Vector2Int StartP, Vector2Int EndP, int Dis)
    {
        Vector2Int [] Parent = Enumerable.Repeat(new Vector2Int(-1, -1), r*c+10).ToArray();
        Queue<Vector2Int> res = new Queue<Vector2Int>();
        Queue<Vector2Int> q = new Queue<Vector2Int>();
        Vector2Int Ans = new Vector2Int(-1, -1);
        bool hasPath = false;
        q.Enqueue(StartP);
        int [,] dir = {{0, 1}, {0, -1}, {-1, 0}, {1, 0}};
        bool [,] vis = new bool [r + 5, c + 5];
        vis[StartP.x, StartP.y] = true;
        while(q.Count > 0)
        {
            Vector2Int NowPos = q.Dequeue();
            int len = Mathf.Abs(NowPos.x - EndP.x) + Mathf.Abs(NowPos.y - EndP.y);
            if(len > Dis)
            {
                hasPath = true;
                Ans = NowPos;
                break;
            }
            for(int i = 0; i < 4; ++i)
            {
                Vector2Int Tmp = NowPos;
                Tmp.x += dir[i, 0];
                Tmp.y += dir[i, 1];
                if(Tmp.x < 0||Tmp.x>=c||Tmp.y < 0|| Tmp.y >= r)continue;
                if(vis[Tmp.y , Tmp.x])continue;
                if(map[Tmp.x, Tmp.y].statucode <= GlobalVar.CannotMove)continue;
                q.Enqueue(Tmp);
                Parent[Tmp.y*c + Tmp.x] = NowPos;
            }
        }
        Vector2Int Error = new Vector2Int(-1, -1);
        while(hasPath&&Parent[Ans.y*c + Ans.x] != Error)
        {
            res.Enqueue(Ans);
            Ans = Parent[Ans.y*c + Ans.x];
        }
        return res;
    }
}

}

