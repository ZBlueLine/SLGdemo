using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;
public class Map : MonoBehaviour
{
    public int Height;
    public int Width;
    public int CellSize;
    public int CamHeight;
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
        float x = Width/2f;
        float z = Height/2f;
        float y = CamHeight + transform.localPosition.y;
        x *= CellSize;
        z *= CellSize;
        x += transform.localPosition.x;
        z += transform.localPosition.z;
        camtrans.localPosition = new Vector3(x, y, z);
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
 