using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;


public class MovePiece : MonoBehaviourPunCallbacks
{
    private GameManager _gameManager; 
    
    public PhotonView playerPhotonView;
    
    public bool selected;
    
    public bool isKing;

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

    void Start()
    {
        //Get the game manager component script
        _gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            
            // Only let the player move the piece if they instantiated it. 
            if (!photonView.IsMine && PhotonNetwork.IsConnected)
            {
                return;
            }
            
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, 100.00f)){

                if (hit.transform != null){

                    if(hit.transform.gameObject.name == this.gameObject.name)
                    {
                        selected = true;
                    }
                    else if (SPACE_NAMES.Contains(hit.transform.gameObject.name) && selected)
                    {
                        // Validate movement here.
                        if (IsMoveValid(hit.transform.gameObject.name))
                        {
                            _gameManager.boardState[hit.transform.gameObject.name] = true;
                            _gameManager.boardState[this.gameObject.tag] = false;
                            MoveChecker(hit.transform.gameObject);
                            selected = false;
                            this.gameObject.tag = hit.transform.gameObject.name;
                        }
                    }
                    else
                    {
                        selected = false;
                    }
                }
            }
        }
    }

    // Pass in the name of the space.
    private bool IsMoveValid(string name)
    {
        // Path 1 - Invalid - Adjacent space (forward) with a piece on it.

        if (_gameManager.boardState[name])
        {
            return false;
        }

        // Path 2 - Valid - Adjacent space (forward) with no piece on it.
        // TODO TIM: Condense this into only 1 loop using local variables.
        if (_gameManager._player1)
        {
            if (isKing)
            {
                // TODO: Validate Movement if piece is a king.
            }
            else
            {

                //TODO Brandon & Tim: Figure out if we should consolidate the logic between these two functions
                // First, see if the game manager says it's okay to make a move.
                if (_gameManager.IsMoveValid())
                {
                    foreach (KeyValuePair<string, List<string>> coords in validBlackMovements_Regular)
                    {
                        if (this.gameObject.tag.Equals(coords.Key) && coords.Value.Contains(name))
                        {

                            return true;
                        }
                    }
                }
            }
        }
        else
        {
            if (isKing)
            {
                // TODO: Validate Movement if piece is a king.
            }
            else
            {

                //TODO Brandon & Tim: Figure out if we should consolidate the logic between these two functions
                // First, see if the game manager says it's okay to make a move.
                if (_gameManager.IsMoveValid())
                {
                    foreach (KeyValuePair<string, List<string>> coords in validWhiteMovements_Regular)
                    {
                        if (this.gameObject.tag.Equals(coords.Key) && coords.Value.Contains(name))
                        {

                            return true;
                        }
                    }
                }
            }
        }

        // Path 3 - Invalid - Adjacent space (backward)

        // Path 4 - Valid - 2 spaces ahead with only an opposing piece in between.

        // Path 5 - Invalid - 2 spaces ahead with a piece in between and a piece on it.

        return false;
    }

    private void MoveChecker(GameObject go)
    {
        this.gameObject.transform.position = new Vector3(go.transform.position.x, 0.26f, go.transform.position.z);
    }

    private Dictionary<string, List<string>> validBlackMovements_Regular = new Dictionary<string, List<string>>(){
        { "A1",
            new List<string>(){
                { "B2" }
            }
        },

        { "A3",
            new List<string>(){
                { "B2" },
                { "B4" }
            }
        },

        { "A5",
            new List<string>(){
                { "B4" },
                { "B6" }
            }
        },

        { "A7",
            new List<string>(){
                { "B6" },
                { "B8" }
            }
        },

        { "B2",
            new List<string>(){
                { "C1" },
                { "C3" }
            }
        },

        { "B4",
            new List<string>(){
                { "C3" },
                { "C5" }
            }
        },

        { "B6",
            new List<string>(){
                { "C5" },
                { "C7" }
            }
        },

        { "B8",
            new List<string>(){
                { "C7" }
            }
        },

        { "C1",
            new List<string>(){
                { "D2" }
            }
        },

        { "C3",
            new List<string>(){
                { "D2" },
                { "D4" }
            }
        },

        { "C5",
            new List<string>(){
                { "D4" },
                { "D6" }
            }
        },

        { "C7",
            new List<string>(){
                { "D6" },
                { "D8" }
            }
        },

        { "D2",
            new List<string>(){
                { "E1" },
                { "E3" }
            }
        },

        { "D4",
            new List<string>(){
                { "E3" },
                { "E5" }
            }
        },

        { "D6",
            new List<string>(){
                { "E5" },
                { "E7" }
            }
        },

        { "D8",
            new List<string>(){
                { "E7" }
            }
        },

        { "E1",
            new List<string>(){
                { "F2" }
            }
        },

        { "E3",
            new List<string>(){
                { "F2" },
                { "F4" }
            }
        },

        { "E5",
            new List<string>(){
                { "F4" },
                { "F6" }
            }
        },

        { "E7",
            new List<string>(){
                { "F6" },
                { "F8" }
            }
        },

        { "F2",
            new List<string>(){
                { "G1" },
                { "G3" }
            }
        },

        { "F4",
            new List<string>(){
                { "G3" },
                { "G5" }
            }
        },

        { "F6",
            new List<string>(){
                { "G5" },
                { "G7" }
            }
        },

        { "F8",
            new List<string>(){
                { "G7" }
            }
        },

        { "G1",
            new List<string>(){
                { "H2" }
            }
        },

        { "G3",
            new List<string>(){
                { "H2" },
                { "H4" }
            }
        },

        { "G5",
            new List<string>(){
                { "H4" },
                { "H6" }
            }
        },

        { "G7",
            new List<string>(){
                { "H6" },
                { "H8" }
            }
        },
    };

    private Dictionary<string, List<string>> validWhiteMovements_Regular = new Dictionary<string, List<string>>(){
        { "B2",
            new List<string>(){
                { "A1" },
                { "A3" }
            }
        },

        { "B4",
            new List<string>(){
                { "A3" },
                { "A5" }
            }
        },

        { "B6",
            new List<string>(){
                { "A5" },
                { "A7" }
            }
        },

        { "B8",
            new List<string>(){
                { "A7" }
            }
        },

        { "C1",
            new List<string>(){
                { "B2" }
            }
        },

        { "C3",
            new List<string>(){
                { "B2" },
                { "B4" }
            }
        },

        { "C5",
            new List<string>(){
                { "B4" },
                { "B6" }
            }
        },

        { "C7",
            new List<string>(){
                { "B6" },
                { "B8" }
            }
        },

        { "D2",
            new List<string>(){
                { "C1" },
                { "C3" }
            }
        },

        { "D4",
            new List<string>(){
                { "C3" },
                { "C5" }
            }
        },

        { "D6",
            new List<string>(){
                { "C5" },
                { "C7" }
            }
        },

        { "D8",
            new List<string>(){
                { "C7" }
            }
        },

        { "E1",
            new List<string>(){
                { "D2" }
            }
        },

        { "E3",
            new List<string>(){
                { "D2" },
                { "D4" }
            }
        },

        { "E5",
            new List<string>(){
                { "D4" },
                { "D6" }
            }
        },

        { "E7",
            new List<string>(){
                { "D6" },
                { "D8" }
            }
        },

        { "F2",
            new List<string>(){
                { "E1" },
                { "E3" }
            }
        },

        { "F4",
            new List<string>(){
                { "E3" },
                { "E5" }
            }
        },

        { "F6",
            new List<string>(){
                { "E5" },
                { "E7" }
            }
        },

        { "F8",
            new List<string>(){
                { "E7" }
            }
        },

        { "G1",
            new List<string>(){
                { "F2" }
            }
        },

        { "G3",
            new List<string>(){
                { "F2" },
                { "F4" }
            }
        },

        { "G5",
            new List<string>(){
                { "F4" },
                { "F6" }
            }
        },

        { "G7",
            new List<string>(){
                { "F6" },
                { "F8" }
            }
        },

        { "H2",
            new List<string>(){
                { "G1" },
                { "G3" }
            }
        },

        { "H4",
            new List<string>(){
                { "G3" },
                { "G5" }
            }
        },

        { "H6",
            new List<string>(){
                { "G5" },
                { "G7" }
            }
        },

        { "H8",
            new List<string>(){
                { "G7" }
            }
        },
    };
}
