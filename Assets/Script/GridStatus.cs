using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Global;
using Utils;

public abstract class GridStatus
{
    static public Map MyMap;
    public int statucode;
    public static GameObject SelectedChara;
    public virtual void Operation(int x, int y){}
}

public class IsCharaStatus : GridStatus
{
    static GridStatus Myinstance;
    private IsCharaStatus(){}
    public static GridStatus getInstance()
    {
        if(Myinstance == null)
        {
            Myinstance = new IsCharaStatus();
            Myinstance.statucode = GlobalVar.IsChara;
        }
        return Myinstance;
    }
    public override void Operation(int x, int y)
    {
        MyMap.PrepareAttack = false;
        CharaController selectchara;
        Debug.Log("have Chara");
        if(SelectedChara)
        {
            selectchara = SelectedChara.gameObject.GetComponent<CharaController>();
            selectchara.Closeproperties();
        }
        SelectedChara = MyMap.GetObject(new Vector2Int(x, y));
        selectchara = SelectedChara.gameObject.GetComponent<CharaController>();
        if(selectchara.Status < GlobalVar.Moved)
        {
            selectchara.ShowMoveRange();
        }
        else if(selectchara.Status < GlobalVar.Attacked)
        {
            Debug.Log("ShowAttackRange");
            MyMap.PrepareAttack = true;
            selectchara.ShowAttackRange();
        }
        selectchara.Showproperties();
    }
}

public class IsEnemyStatus : GridStatus
{
    static GridStatus Myinstance;
    private IsEnemyStatus(){}
    public static GridStatus getInstance()
    {
        if(Myinstance == null)
        {
            Myinstance = new IsEnemyStatus();
            Myinstance.statucode = GlobalVar.IsEnemy;
        }
        return Myinstance;
    }
    public override void Operation(int x, int y)
    {
        MyMap.PrepareAttack = false;
        CharaController selectchara;
        Debug.Log(" Is Enemy");
        if(SelectedChara)
        {
            selectchara = SelectedChara.gameObject.GetComponent<CharaController>();
            selectchara.Closeproperties();
        }
        SelectedChara = MyMap.GetObject(new Vector2Int(x, y));
        selectchara = SelectedChara.gameObject.GetComponent<CharaController>();
        if(selectchara.Status < GlobalVar.Moved)
            selectchara.ShowMoveRange(false);
        selectchara.Showproperties();
    }
}

public class PrepareMoveStatus : GridStatus
{
    static GridStatus Myinstance;
    private PrepareMoveStatus(){}
    public static GridStatus getInstance()
    {
        if(Myinstance == null)
        {
            Myinstance = new PrepareMoveStatus();
            Myinstance.statucode = GlobalVar.PrepareMove;
        }
        return Myinstance;
    }
    public override void Operation(int x, int y)
    {
        Debug.Log("PrepareMove");
        CharaController m_Controller = SelectedChara.GetComponent<CharaController>();
        if(m_Controller.TeamTag == GlobalVar.IsEnemy)return;
        Vector2Int StartPos = m_Controller.GetIndex();
        int Height, Width;
        Height = MyMap.Height;
        Width = MyMap.Width;
        Queue<Vector2Int> Path =  UtilsTool.BFS(Height, Width,  MyMap.gridArry, StartPos, new Vector2Int(x, y));
        if(Path.Count == 0)
        {
            MyMap.ShowValue();
            return;
        }
        while(Path.Count != 0)
            m_Controller.AddMoveAction(Path.Dequeue());
        MyMap.Moveing = true;

        //原位置标记为空位置
        MyMap.SetGridValue(new Vector2Int(StartPos.x, StartPos.y), EmptyLcationstatus.getInstance());

        m_Controller.Move();
        m_Controller.Status = GlobalVar.Moved;
        GameObject Tmp = MyMap.ObjectArry[StartPos.x][StartPos.y];
        MyMap.ObjectArry[StartPos.x][StartPos.y] =  MyMap.ObjectArry[x][y];
        MyMap.ObjectArry[x][y] = Tmp;
        MyMap.ShowValue();
    }
}

public class PrepareAttackststus : GridStatus
{
    static GridStatus Myinstance;
    private PrepareAttackststus(){}
    public static GridStatus getInstance()
    {
        if(Myinstance == null)
        {
            Myinstance = new PrepareAttackststus();
            Myinstance.statucode = GlobalVar.PrepareAttack;
        }
        return Myinstance;
    }
}

public class Attackststus : GridStatus
{
    static GridStatus Myinstance;
    private Attackststus(){}
    public static GridStatus getInstance()
    {
        if(Myinstance == null)
        {
            Myinstance = new Attackststus();
            Myinstance.statucode = GlobalVar.Attack;
        }
        return Myinstance;
    }
    public override void Operation(int x, int y)
    {
        MyMap.PrepareAttack = false;
        CharaController selectedchara = SelectedChara.GetComponent<CharaController>();
        CharaController ToChara = MyMap.GetObject(new Vector2Int(x, y)).GetComponent<CharaController>();
        ToChara.Damaged(selectedchara.ATK);
        ++MyMap.ActionEnd;
        MyMap.CheckTurn();
        selectedchara.Status = GlobalVar.Attacked;
    }
}

public class EmptyLcationstatus : GridStatus
{
    static GridStatus Myinstance;
    private EmptyLcationstatus(){}
    public static GridStatus getInstance()
    {
        if(Myinstance == null)
        {
            Myinstance = new EmptyLcationstatus();
            Myinstance.statucode = GlobalVar.EmptyLcation;
        }
        return Myinstance;
    }
    public override void Operation(int x, int y)
    {
        MyMap.PrepareAttack = false;
        if(SelectedChara)
        {
            CharaController selectchara = SelectedChara.gameObject.GetComponent<CharaController>();
            selectchara.Closeproperties();
        }
        SelectedChara = MyMap.GetObject(new Vector2Int(x, y));
        MyMap.AddClickMark(UtilsTool.CreatePlane(MyMap.AimPoint, MyMap.CellSize, MyMap.transform, MyMap.SelectBlockColor));
        MyMap.ShowValue();
    }
}

public class ErrorStatus : GridStatus
{
    static GridStatus Myinstance;
    private ErrorStatus(){}
    public static GridStatus getInstance()
    {
        if(Myinstance == null)
        {
            MyMap.PrepareAttack = false;
            Myinstance = new ErrorStatus();
            Myinstance.statucode = GlobalVar.FailLcation;
        }
        return Myinstance;
    }
}
