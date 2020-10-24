using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
        if(Mymaterial == null)
        {
            Mymeshrenderer.material = new Material(Shader.Find("Diffuse"));
            Mymeshrenderer.material.color = Color.blue;
        }
        else 
            Mymeshrenderer.material = Mymaterial;
        Mymeshfilter.mesh.vertices = new Vector3 []
        {
            new Vector3(x, y, z),
            new Vector3(x + length, y, z),
            new Vector3(x, y, z + length),
            new Vector3(x + length, y, z + length)
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

