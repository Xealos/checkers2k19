using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;


public class MovePiece : MonoBehaviourPunCallbacks
{
    private GameManager _gameManager;

    public PhotonView playerPhotonView;

    public bool selected;

    public static bool isKing;

    private static bool _instantiated;

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

    void Awake()
    {
        //Get the game manager component script
        _gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Don't let player move a piece if nobody else has connected yet. 
            if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                return;
            }

            // Only let the player move the piece if they instantiated it. 
            if (!photonView.IsMine && PhotonNetwork.IsConnected)
            {
                return;
            }

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, 100.00f)) {

                if (hit.transform != null) {

                    if (hit.transform.gameObject.name == this.gameObject.name)
                    {
                        selected = true;
                    }
                    else if (SPACE_NAMES.Contains(hit.transform.gameObject.name) && selected)
                    {
                        // If it's our turn, Validate movement here.
                        if (GameManager.MyTurn &&
                            IsMoveValid(hit.transform.gameObject.name))
                        {
                            // TODO Brandon: I think instead of editing the board state directly, it'd be safer 
                            // TODO          to make the data structure private and have an accessor function in 
                            // TODO          the game manager. 
                            GameManager.BoardState[hit.transform.gameObject.name] = true;
                            GameManager.BoardState[this.gameObject.tag] = false;
                            MoveChecker(hit.transform.gameObject);
                            selected = false;
                            this.gameObject.tag = hit.transform.gameObject.name;

                            // Tell the game manager to update the game state
                            _gameManager.UpdateGameState();
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
        Dictionary<string, List<string>> validMovements;
        List<JumpPositions> validJumps;

        // Path 1 - Invalid - Adjacent space (forward) with a piece on it.

        if (GameManager.BoardState[name])
        {
            return false;
        }

        // Path 2 - Valid - Adjacent space (forward) with no piece on it.
        if (isKing)
        {
            validMovements = validKingMovements;
            validJumps = null; //validKingJumps;
        }
        else if (_gameManager.player1)
        {
            validMovements = validBlackMovements_Regular;
            validJumps = validBlackJumps;
        }
        else
        {
            validMovements = validWhiteMovements_Regular;
            validJumps = validWhiteJumps;
        }

        // First, see if the game manager says it's okay to make a move.
        foreach (KeyValuePair<string, List<string>> coords in validMovements)
        {
            if (this.gameObject.tag.Equals(coords.Key) && coords.Value.Contains(name))
            {

                return true;
            }
        }

        foreach(JumpPositions jumps in validJumps)
        {
                // If we're on the current position...
            if (this.gameObject.tag.Equals(jumps.getCurrentPosition())){

                string jumpOverSpace;

                // and name is a key in the leftJump dictionary...
                if (jumps.getJumps().ContainsKey(name)) {

                    jumpOverSpace = jumps.getJumps()[name];
                    // Then the associated value must be occupied by an opposing piece.

                    // TODO TIM: For now, as long as any piece is occupying the square, the jump is valid.
                    // This must be changed to check for opposing pieces.
                    if (GameManager.BoardState[jumpOverSpace])
                    {
                        return true;
                    }

                }
            }
        }


        return false;
    }

    private void MoveChecker(GameObject go)
    {
        this.gameObject.transform.position = new Vector3(go.transform.position.x, 0.26f, go.transform.position.z);
    }

    private class JumpPositions {
        private string currentPosition;
        // Key is jump destination, value is the position in between.
        private Dictionary<string, string> jumps;

        public JumpPositions(string currentPosition, Dictionary<string, string> jumps)
        {
            this.currentPosition = currentPosition;
            this.jumps = jumps;
        }

        public string getCurrentPosition()
        {
            return currentPosition;
        }

        public Dictionary<string, string> getJumps()
        {
            return jumps;
        }
    }

    private List<JumpPositions> validWhiteJumps = new List<JumpPositions>()
    {
        { new JumpPositions("H2",
            new Dictionary<string, string>(){
                { "F4", "G3" }
            })
        },

        { new JumpPositions("H4",
            new Dictionary<string, string>(){
                { "F2", "G3" },
                { "F6", "G5" }
            })
        },

        { new JumpPositions("H6",
            new Dictionary<string, string>(){
                { "F4", "G5" },
                { "F8", "G7" }
            })
        },

        { new JumpPositions("H8",
            new Dictionary<string, string>(){
                { "C5", "B6" },
                { string.Empty, string.Empty }
            })
        },

        { new JumpPositions("G1",
            new Dictionary<string, string>(){
                { "E3", "F2" }
            })
        },

        { new JumpPositions("G3",
            new Dictionary<string, string>(){
                { "E1", "F2" },
                { "E5", "F4" }
            })
        },

        { new JumpPositions("G5",
            new Dictionary<string, string>(){
                { "E3", "F4" },
                { "E7", "F6" }
            })
        },

        { new JumpPositions("G7",
            new Dictionary<string, string>(){
                { "E5", "F6" }
            })
        },

        { new JumpPositions("F2",
            new Dictionary<string, string>(){
                { "D4", "E3" }
            })
        },

        { new JumpPositions("F4",
            new Dictionary<string, string>(){
                { "D2", "E3" },
                { "D6", "E5" }
            })
        },

        { new JumpPositions("F6",
            new Dictionary<string, string>(){
                { "D4", "E5" },
                { "D8", "E7" }
            })
        },

        { new JumpPositions("F8",
            new Dictionary<string, string>(){
                { "E7", "D6" }
            })
        },

        { new JumpPositions("E1",
            new Dictionary<string, string>(){
                { "C3", "D2" }
            })
        },

        { new JumpPositions("E3",
            new Dictionary<string, string>(){
                { "C1", "D2" },
                { "C5", "D4" }
            })
        },

        { new JumpPositions("E5",
            new Dictionary<string, string>(){
                { "C3", "D4" },
                { "C7", "D6" }
            })
        },

        { new JumpPositions("E7",
            new Dictionary<string, string>(){
                { "C5", "D6" }
            })
        },

        { new JumpPositions("D2",
            new Dictionary<string, string>(){
                { "B4", "C3" }
            })
        },

        { new JumpPositions("D4",
            new Dictionary<string, string>(){
                { "B2", "C3" },
                { "B6", "C5" }
            })
        },

        { new JumpPositions("D6",
            new Dictionary<string, string>(){
                { "B4", "C5" },
                { "B8", "C7" }
            })
        },

        { new JumpPositions("D8",
            new Dictionary<string, string>(){
                { "B6", "C7" }
            })
        },

        { new JumpPositions("C1",
            new Dictionary<string, string>(){
                { "A3", "B2" }
            })
        },

        { new JumpPositions("C3",
            new Dictionary<string, string>(){
                { "A1", "B2" },
                { "A5", "B4" }
            })
        },

        { new JumpPositions("C5",
            new Dictionary<string, string>(){
                { "A3", "B4" },
                { "A7", "B6" }
            })
        },

        { new JumpPositions("C7",
            new Dictionary<string, string>(){
                { "A5", "B6" }
            })
        }
    };

    private List<JumpPositions> validBlackJumps = new List<JumpPositions>()
    {
        { new JumpPositions("A1",
            new Dictionary<string, string>(){
                { "C3", "B2" }
            })
        },

        { new JumpPositions("A3",
            new Dictionary<string, string>(){
                { "C1", "B2" },
                { "C5", "B4" }
            })
        },

        { new JumpPositions("A5",
            new Dictionary<string, string>(){
                { "C3", "B4" },
                { "C7", "B6" }
            })
        },

        { new JumpPositions("A7",
            new Dictionary<string, string>(){
                { "C5", "B6" }
            })
        },

        { new JumpPositions("B2",
            new Dictionary<string, string>(){
                { "D4", "C3" }
            })
        },

        { new JumpPositions("B4",
            new Dictionary<string, string>(){
                { "D2", "C3" },
                { "D6", "C5" }
            })
        },

        { new JumpPositions("B6",
            new Dictionary<string, string>(){
                { "D4", "C5" },
                { "D8", "C7" }
            })
        },

        { new JumpPositions("B8",
            new Dictionary<string, string>(){
                { "D6", "C7" }
            })
        },

        { new JumpPositions("C1",
            new Dictionary<string, string>(){
                { "E3", "D2" }
            })
        },

        { new JumpPositions("C3",
            new Dictionary<string, string>(){
                { "E1", "D2" },
                { "E5", "D4" }
            })
        },

        { new JumpPositions("C5",
            new Dictionary<string, string>(){
                { "E3", "D4" },
                { "E7", "D6" }
            })
        },

        { new JumpPositions("C7",
            new Dictionary<string, string>(){
                { "E5", "D6" }
            })
        },

        { new JumpPositions("D2",
            new Dictionary<string, string>(){
                { "F4", "E3" }
            })
        },

        { new JumpPositions("D4",
            new Dictionary<string, string>(){
                { "F2", "E3" },
                { "F6", "E5" }
            })
        },

        { new JumpPositions("D6",
            new Dictionary<string, string>(){
                { "F4", "E5" },
                { "F8", "E7" }
            })
        },

        { new JumpPositions("D8",
            new Dictionary<string, string>(){
                { "F6", "E7" }
            })
        },

        { new JumpPositions("E1",
            new Dictionary<string, string>(){
                { "G3", "F2" }
            })
        },

        { new JumpPositions("E3",
            new Dictionary<string, string>(){
                { "G1", "F2" },
                { "G5", "F4" }
            })
        },

        { new JumpPositions("E5",
            new Dictionary<string, string>(){
                { "G3", "F4" },
                { "G7", "F6" }
            })
        },

        { new JumpPositions("E7",
            new Dictionary<string, string>(){
                { "G5", "F6" }
            })
        },

        { new JumpPositions("F2",
            new Dictionary<string, string>(){
                { "H4", "G3" }
            })
        },

        { new JumpPositions("F4",
            new Dictionary<string, string>(){
                { "H2", "G3" },
                { "H6", "G5" }
            })
        },

        { new JumpPositions("F6",
            new Dictionary<string, string>(){
                { "H4", "G5" },
                { "H8", "G7" }
            })
        },

        { new JumpPositions("F8",
            new Dictionary<string, string>(){
                { "H6", "G7" }
            })
        }
    };

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

    private Dictionary<string, List<string>> validKingMovements = new Dictionary<string, List<string>>()
    {
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
                { "A1" },
                { "A3" },
                { "C1" },
                { "C3" }
            }
        },

        { "B4",
            new List<string>(){
                { "A3" },
                { "A5" },
                { "C3" },
                { "C5" }
            }
        },

        { "B6",
            new List<string>(){
                { "A5" },
                { "A7" },
                { "C5" },
                { "C7" }
            }
        },

        { "B8",
            new List<string>(){
                { "A7" },
                { "C7" }
            }
        },

        { "C1",
            new List<string>(){
                { "B2" },
                { "D2" }
            }
        },

        { "C3",
            new List<string>(){
                { "B2" },
                { "B4" },
                { "D2" },
                { "D4" }
            }
        },

        { "C5",
            new List<string>(){
                { "B4" },
                { "B6" },
                { "D4" },
                { "D6" }
            }
        },

        { "C7",
            new List<string>(){
                { "B6" },
                { "B8" },
                { "D6" },
                { "D8" }
            }
        },

        { "D2",
            new List<string>(){
                { "C1" },
                { "C3" },
                { "E1" },
                { "E3" }
            }
        },

        { "D4",
            new List<string>(){
                { "C3" },
                { "C5" },
                { "E3" },
                { "E5" }
            }
        },

        { "D6",
            new List<string>(){
                { "C5" },
                { "C7" },
                { "E5" },
                { "E7" }
            }
        },

        { "D8",
            new List<string>(){
                { "C7" },
                { "E7" }
            }
        },

        { "E1",
            new List<string>(){
                { "D2" },
                { "F2" }
            }
        },

        { "E3",
            new List<string>(){
                { "D2" },
                { "D4" },
                { "F2" },
                { "F4" }
            }
        },

        { "E5",
            new List<string>(){
                { "D4" },
                { "D6" },
                { "F4" },
                { "F6" }
            }
        },

        { "E7",
            new List<string>(){
                { "D6" },
                { "D8" },
                { "F6" },
                { "F8" }
            }
        },

        { "F2",
            new List<string>(){
                { "E1" },
                { "E3" },
                { "G1" },
                { "G3" }
            }
        },

        { "F4",
            new List<string>(){
                { "E3" },
                { "E5" },
                { "G3" },
                { "G5" }
            }
        },

        { "F6",
            new List<string>(){
                { "E5" },
                { "E7" },
                { "G5" },
                { "G7" }
            }
        },

        { "F8",
            new List<string>(){
                { "E7" },
                { "G7" }
            }
        },

        { "G1",
            new List<string>(){
                { "F2" },
                { "H2" }
            }
        },

        { "G3",
            new List<string>(){
                { "F2" },
                { "F4" },
                { "H2" },
                { "H4" }
            }
        },

        { "G5",
            new List<string>(){
                { "F4" },
                { "F6" },
                { "H4" },
                { "H6" }
            }
        },

        { "G7",
            new List<string>(){
                { "F6" },
                { "F8" },
                { "H6" },
                { "H8" }
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

internal class jumpPositions
{
}