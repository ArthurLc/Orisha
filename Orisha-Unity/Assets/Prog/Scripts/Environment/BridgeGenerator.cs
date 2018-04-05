using System.Collections.Generic;
using UnityEngine;

public class BridgeGenerator : MonoBehaviour {

    public Pool bridgePool;

    [SerializeField] private GameObject startPillars;
    [SerializeField] private GameObject endPillars;

    [SerializeField] private float spaceBetweenPlanks = 2.2f;
    [SerializeField] private float spaceBetweenFirstPillarsAndFirstPlanks = 0.5f;
    
    // Use this for initialization
    void Start () {
        
        Vector3 dirBridge = endPillars.transform.position - startPillars.transform.position;

        List<GameObject> Boards = new List<GameObject>();

        float bridgeSize = dirBridge.magnitude;
        dirBridge.Normalize();

        int nbrBoard = (int)System.Math.Round((bridgeSize / spaceBetweenPlanks));


        //Generate board
        for (int i = 0; i < nbrBoard; i++)
        {
            
            GameObject Board = bridgePool.TakeInPool("Board", startPillars.transform.position + ((i + spaceBetweenFirstPillarsAndFirstPlanks) * dirBridge * spaceBetweenPlanks), Vector3.one);
            Board.SetActive(true);
            Board.transform.LookAt(Board.transform.position + dirBridge, Vector3.up);
            Board.transform.Rotate(Vector3.up, 90.0f);


            HingeJoint leftJoin = Board.AddComponent<HingeJoint>();
            leftJoin.anchor = new Vector3(0.0f, 0.0f, 0.0f);
            leftJoin.axis = Vector3.up;

            leftJoin.connectedMassScale = 1;
            leftJoin.massScale = 1;

            //First board
            if (i == 0)
                leftJoin.connectedBody = startPillars.transform.GetChild(0).GetComponent<Rigidbody>();
            else
                leftJoin.connectedBody = Boards[i - 1].GetComponent<Rigidbody>();

            Boards.Add(Board);
        }



        // Obsolete (pour le moment...)
        {
            //List<GameObject> Pillars = new List<GameObject>();
            //List<GameObject> ropeUpLeft = new List<GameObject>();
            //List<GameObject> ropeUpRight = new List<GameObject>();
            //List<GameObject> ropeDownLeft = new List<GameObject>();
            //List<GameObject> ropeDownRight = new List<GameObject>();

            ////Generate pillars
            {
                //for (int i = 0; i < nbrPillar; i++)
                //{
                //    GameObject Pillar = bridgePool.TakeInPool("pillar", startPillars.transform.position + (((i + 1) * dirBridge / 1.5f) * 4), Vector3.one);
                //    Pillar.SetActive(true);
                //    Pillar.transform.forward = dirBridge;

                //    FixedJoint leftPoleJoin = Pillar.transform.GetChild(0).gameObject.AddComponent<FixedJoint>();
                //    FixedJoint rightPoleJoin = Pillar.transform.GetChild(1).gameObject.AddComponent<FixedJoint>();

                //    leftPoleJoin.connectedBody = Boards[((i + 1) * 4) - 1].GetComponent<Rigidbody>();
                //    rightPoleJoin.connectedBody = Boards[((i + 1) * 4) - 1].GetComponent<Rigidbody>();

                //    leftPoleJoin.connectedMassScale = 100;
                //    leftPoleJoin.massScale = 100;
                //    rightPoleJoin.connectedMassScale = 100;
                //    rightPoleJoin.massScale = 100;


                //    Pillars.Add(Pillar);
                //}
            }

            ////Generate rope up left
            {
                //for (int i = 0; i < nbrRope; i++)
                //{
                //    GameObject Rope = bridgePool.TakeInPool("rope", startPillars.transform.position + ((i * dirBridge / 1.5f) * 4), Vector3.one);
                //    Rope.SetActive(true);
                //    Rope.transform.LookAt(Rope.transform.position + dirBridge, Vector3.up);
                //    Rope.transform.Rotate(Vector3.up, 90.0f);
                //    Rope.transform.position -= (Rope.transform.forward * 1.7f);
                //    Rope.transform.position += (Rope.transform.up * 1.6f);
                //    Rope.transform.position -= (Rope.transform.right * 1.3f);

                //    FixedJoint startJoin = Rope.AddComponent<FixedJoint>();
                //    FixedJoint endJoin = Rope.AddComponent<FixedJoint>();

                //    startJoin.connectedMassScale = 100;
                //    startJoin.massScale = 100;
                //    endJoin.connectedMassScale = 100;
                //    endJoin.massScale = 100;

                //    //First rope
                //    if (i == 0)
                //    {
                //        startJoin.connectedBody = startPillars.transform.GetChild(0).GetComponent<Rigidbody>();
                //        endJoin.connectedBody = Pillars[0].transform.GetChild(0).GetComponent<Rigidbody>();
                //    }
                //    else if (i == (nbrRope - 1))//Last rope
                //    {
                //        startJoin.connectedBody = Pillars[nbrPillar - 1].transform.GetChild(0).GetComponent<Rigidbody>();
                //        endJoin.connectedBody = endPillars.transform.GetChild(0).GetComponent<Rigidbody>();
                //    }
                //    else
                //    {
                //        startJoin.connectedBody = Pillars[i - 1].transform.GetChild(0).GetComponent<Rigidbody>();
                //        endJoin.connectedBody = Pillars[i].transform.GetChild(0).GetComponent<Rigidbody>();
                //    }

                //    ropeUpLeft.Add(Rope);
                //}
            }

            ////Generate rope up right
            {
                //for (int i = 0; i < nbrRope; i++)
                //{
                //    GameObject Rope = bridgePool.TakeInPool("rope", startPillars.transform.position + ((i * dirBridge / 1.5f) * 4), Vector3.one);
                //    Rope.SetActive(true);
                //    Rope.transform.LookAt(Rope.transform.position + dirBridge, Vector3.up);
                //    Rope.transform.Rotate(Vector3.up, 90.0f);
                //    Rope.transform.position += (Rope.transform.forward * 1.7f);
                //    Rope.transform.position += (Rope.transform.up * 1.6f);
                //    Rope.transform.position -= (Rope.transform.right * 1.3f);

                //    FixedJoint startJoin = Rope.AddComponent<FixedJoint>();
                //    FixedJoint endJoin = Rope.AddComponent<FixedJoint>();

                //    startJoin.connectedMassScale = 100;
                //    startJoin.massScale = 100;
                //    endJoin.connectedMassScale = 100;
                //    endJoin.massScale = 100;
                //    //First rope
                //    if (i == 0)
                //    {
                //        startJoin.connectedBody = startPillars.transform.GetChild(1).GetComponent<Rigidbody>();
                //        endJoin.connectedBody = Pillars[0].transform.GetChild(1).GetComponent<Rigidbody>();
                //    }
                //    else if (i == (nbrRope - 1))//Last rope
                //    {
                //        startJoin.connectedBody = Pillars[nbrPillar - 1].transform.GetChild(1).GetComponent<Rigidbody>();
                //        endJoin.connectedBody = endPillars.transform.GetChild(1).GetComponent<Rigidbody>();
                //    }
                //    else
                //    {
                //        startJoin.connectedBody = Pillars[i - 1].transform.GetChild(1).GetComponent<Rigidbody>();
                //        endJoin.connectedBody = Pillars[i].transform.GetChild(1).GetComponent<Rigidbody>();
                //    }
                //    ropeUpRight.Add(Rope);
                //}
            }

            ////Generate rope down left
            {
                //for (int i = 0; i < nbrRope; i++)
                //{
                //    GameObject Rope = bridgePool.TakeInPool("rope", startPillars.transform.position + ((i * dirBridge / 1.5f) * 4), Vector3.one);
                //    Rope.SetActive(true);
                //    Rope.transform.LookAt(Rope.transform.position + dirBridge, Vector3.up);
                //    Rope.transform.Rotate(Vector3.up, 90.0f);
                //    Rope.transform.position -= (Rope.transform.forward * 1.7f);
                //    Rope.transform.position += (Rope.transform.up * 0.8f);
                //    Rope.transform.position -= (Rope.transform.right * 1.3f);

                //    FixedJoint startJoin = Rope.AddComponent<FixedJoint>();
                //    FixedJoint endJoin = Rope.AddComponent<FixedJoint>();

                //    startJoin.connectedMassScale = 100;
                //    startJoin.massScale = 100;
                //    endJoin.connectedMassScale = 100;
                //    endJoin.massScale = 100;
                //    //First rope
                //    if (i == 0)
                //    {
                //        startJoin.connectedBody = startPillars.transform.GetChild(0).GetComponent<Rigidbody>();
                //        endJoin.connectedBody = Pillars[0].transform.GetChild(0).GetComponent<Rigidbody>();
                //    }
                //    else if (i == (nbrRope - 1))//Last rope
                //    {
                //        startJoin.connectedBody = Pillars[nbrPillar - 1].transform.GetChild(0).GetComponent<Rigidbody>();
                //        endJoin.connectedBody = endPillars.transform.GetChild(0).GetComponent<Rigidbody>();
                //    }
                //    else
                //    {
                //        startJoin.connectedBody = Pillars[i - 1].transform.GetChild(0).GetComponent<Rigidbody>();
                //        endJoin.connectedBody = Pillars[i].transform.GetChild(0).GetComponent<Rigidbody>();
                //    }
                //    ropeDownLeft.Add(Rope);
                //}
            }

            ////Generate rope down right
            {
                //for (int i = 0; i < nbrRope; i++)
                //{
                //    GameObject Rope = bridgePool.TakeInPool("rope", startPillars.transform.position + ((i * dirBridge / 1.5f) * 4), Vector3.one);
                //    Rope.SetActive(true);
                //    Rope.transform.LookAt(Rope.transform.position + dirBridge, Vector3.up);
                //    Rope.transform.Rotate(Vector3.up, 90.0f);
                //    Rope.transform.position += (Rope.transform.forward * 1.7f);
                //    Rope.transform.position += (Rope.transform.up * 0.8f);
                //    Rope.transform.position -= (Rope.transform.right * 1.3f);

                //    FixedJoint startJoin = Rope.AddComponent<FixedJoint>();
                //    FixedJoint endJoin = Rope.AddComponent<FixedJoint>();

                //    startJoin.connectedMassScale = 100;
                //    startJoin.massScale = 100;
                //    endJoin.connectedMassScale = 100;
                //    endJoin.massScale = 100;
                //    //First rope
                //    if (i == 0)
                //    {
                //        startJoin.connectedBody = startPillars.transform.GetChild(1).GetComponent<Rigidbody>();
                //        endJoin.connectedBody = Pillars[0].transform.GetChild(1).GetComponent<Rigidbody>();
                //    }
                //    else if (i == (nbrRope - 1))//Last rope
                //    {
                //        startJoin.connectedBody = Pillars[nbrPillar - 1].transform.GetChild(1).GetComponent<Rigidbody>();
                //        endJoin.connectedBody = endPillars.transform.GetChild(1).GetComponent<Rigidbody>();
                //    }
                //    else
                //    {
                //        startJoin.connectedBody = Pillars[i - 1].transform.GetChild(1).GetComponent<Rigidbody>();
                //        endJoin.connectedBody = Pillars[i].transform.GetChild(1).GetComponent<Rigidbody>();
                //    }
                //    ropeDownRight.Add(Rope);
                //}
            }

            ////Generate Joint between Rope and pillars
            {
                //for (int i = 0; i < nbrRope; i++)
                //{
                //    FixedJoint JoinUpLeft = ropeUpLeft[i].AddComponent<FixedJoint>();
                //    JoinUpLeft.connectedBody = ropeUpRight[i].GetComponent<Rigidbody>();
                //    JoinUpLeft.connectedMassScale = 100;
                //    JoinUpLeft.massScale = 100;

                //    FixedJoint JoinUpRight = ropeUpRight[i].AddComponent<FixedJoint>();
                //    JoinUpRight.connectedBody = ropeUpLeft[i].GetComponent<Rigidbody>();
                //    JoinUpRight.connectedMassScale = 100;
                //    JoinUpRight.massScale = 100;

                //    FixedJoint JoinDownLeft = ropeDownLeft[i].AddComponent<FixedJoint>();
                //    JoinDownLeft.connectedBody = ropeDownRight[i].GetComponent<Rigidbody>();
                //    JoinDownLeft.connectedMassScale = 100;
                //    JoinDownLeft.massScale = 100;

                //    FixedJoint JoinDownRight = ropeDownRight[i].AddComponent<FixedJoint>();
                //    JoinDownRight.connectedBody = ropeDownLeft[i].GetComponent<Rigidbody>();
                //    JoinDownRight.connectedMassScale = 100;
                //    JoinDownRight.massScale = 100;

                //}
                //for (int i = 0; i < nbrPillar; i++)
                //{
                //    FixedJoint leftPoleJoin = Pillars[i].transform.GetChild(0).gameObject.AddComponent<FixedJoint>();
                //    FixedJoint rightPoleJoin = Pillars[i].transform.GetChild(1).gameObject.AddComponent<FixedJoint>();

                //    leftPoleJoin.connectedBody = Pillars[i].transform.GetChild(1).GetComponent<Rigidbody>();
                //    rightPoleJoin.connectedBody = Pillars[i].transform.GetChild(0).GetComponent<Rigidbody>();

                //    leftPoleJoin.connectedMassScale = 100;
                //    leftPoleJoin.massScale = 100;
                //    rightPoleJoin.connectedMassScale = 100;
                //    rightPoleJoin.massScale = 100;

                //}
            }

            //Remise à la taille des ropes
            {
                //foreach (GameObject rope in Ropes)
                //{
                //    Vector3 Apoint = rope.transform.position;
                //    //First pole
                //    Vector3 Bpoint = rope.GetComponents<FixedJoint>()[0].connectedBody.gameObject.transform.position;
                //    Bpoint.y = Apoint.y;
                //    //Second pole
                //    Vector3 Cpoint = rope.GetComponents<FixedJoint>()[1].connectedBody.gameObject.transform.position;
                //    Cpoint.y = Apoint.y;

                //    if ((Cpoint - Bpoint).magnitude < 2.6)
                //    {
                //        rope.transform.localScale = new Vector3(rope.transform.localScale.x - (((Cpoint - Bpoint).magnitude - 2.63f) / 100.0f), rope.transform.localScale.y, rope.transform.localScale.z);
                //        Debug.Log((Cpoint - Bpoint).magnitude.ToString());
                //    }
                //    else if ((Cpoint - Bpoint).magnitude > 2.65)
                //    {
                //        rope.transform.localScale = new Vector3(rope.transform.localScale.x + (((Cpoint - Bpoint).magnitude - 2.63f) / 100.0f), rope.transform.localScale.y, rope.transform.localScale.z);
                //        Debug.Log((Cpoint - Bpoint).magnitude.ToString());
                //    }
                //}
            }
        }

        FixedJoint leftEndJoin = endPillars.transform.GetChild(0).gameObject.AddComponent<FixedJoint>();
        FixedJoint rightEndJoin = endPillars.transform.GetChild(1).gameObject.AddComponent<FixedJoint>();
        leftEndJoin.connectedBody = Boards[Boards.Count - 1].GetComponent<Rigidbody>();
        rightEndJoin.connectedBody = Boards[Boards.Count - 1].GetComponent<Rigidbody>();

        leftEndJoin.connectedMassScale = 1;
        leftEndJoin.massScale = 1;
        rightEndJoin.connectedMassScale = 1;
        rightEndJoin.massScale = 1;

        //Debug.Break();
    }


}
