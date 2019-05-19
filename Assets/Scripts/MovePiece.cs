using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePiece : MonoBehaviour
{

    private readonly List<string> SPACE_NAMES = new List<string>()
        {
            "A1",
            "A3",
            "A5",
            "A7",
            "B2",
            "B4",
            "B6",
            "B8",
            "C1",
            "C3",
            "C5",
            "C7",
            "D2",
            "D4",
            "D6",
            "D8",
            "E1",
            "E3",
            "E5",
            "E7",
            "F2",
            "F4",
            "F6",
            "F8",
            "G1",
            "G3",
            "G5",
            "G7",
            "H2",
            "H4",
            "H6",
            "H8",
        };

private void Update()
    {
        if (Input.GetMouseButtonDown(0)){
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, 100.00f)){

                if (hit.transform != null){
                    //PrintName(hit.transform.gameObject);

                    if (SPACE_NAMES.Contains(hit.transform.gameObject.name)) {

                        MoveChecker(hit.transform.gameObject);
                    }
                }
            }
        }
    }

    private void MoveChecker(GameObject go)
    {
        this.gameObject.transform.position = new Vector3(go.transform.position.x, 0.26f, go.transform.position.z);
    }
}
