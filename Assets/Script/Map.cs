using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;
public class Map : MonoBehaviour
{
    public int Height;
    public int Width;
    public int CellSize;
    public Vector3 TargetPos;
    private Vector3 AimPoint;
    private List<GameObject> ClickMark;
    public Material SelectBlockColor;

    void Awake()
    {
        ClickMark = new List<GameObject>();
    }
    void Start()
    {
        CreateGrid grid = gameObject.AddComponent<CreateGrid>() as CreateGrid;
        grid.Create(Height, Width, CellSize);
        Transform camtrans = Camera.main.transform;
        int x = Width>>1;
        int z = Height>>1;
        x *= CellSize;
        z *= CellSize;
        camtrans.localPosition = new Vector3(x, 150, z);
    }
    void Update()
    {   
        if(Input.GetMouseButtonUp(0))
        {
            Vector3 ScreenPositon = Input.mousePosition;
            Ray Mray = Camera.main.ScreenPointToRay(ScreenPositon);
            Debug.DrawRay(Mray.origin, Mray.direction * 1000, Color.white, 100, true);
            RaycastHit Mhit;
            if(Physics.Raycast(Mray, out Mhit))
            {
                if(Mhit.collider.gameObject.tag == "Map")
                {
                    if(ClickMark.Count != 0)
                    {
                        foreach(GameObject c in ClickMark)
                        {
                            Object.Destroy(c);
                        }
                        ClickMark.Clear();
                    }

                    // GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    // cube.transform.position = Mhit.point;
                    TargetPos = Mhit.point;

                    //计算点击的格子左下角的index
                    AimPoint = gameObject.GetComponent<CreateGrid>().WorldToGridIndex(TargetPos);
                    
                    ClickMark.Add(Utils.UtilsTool.CreatePlane(AimPoint, CellSize, transform, SelectBlockColor));
                }
            }
        }
    }
}
 