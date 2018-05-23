/*
* @ArthurLacour
* @MasksMenu.cs
* @14/03/2018
* @ - Le Script s'attache sur un GameObject d'un Canvas.
*   
* Contient :
*   - La liste static des buttons voués à équiper les masques que le joueur possède.
* 
* A faire:
*   - Changer le visuel des masque dans la fonction UpdateVisualMasks()
*/


using UnityEngine;
using UnityEngine.UI;
using vd_Inputs;

public class MasksMenu : MonoBehaviour {

    [System.Serializable]
    struct ButtonMask
    {
        public Button button;
        public Image imgMask;
    }

    [SerializeField] GameObject masksParent;
	[SerializeField] ButtonMask[] masks;

	[SerializeField] GameObject menu;

    private LifeBar lifeBarHUD;

    private int idSelected = 0;

	//parce que sinon le get plante
	private int IdSelected
	{
		get{ return idSelected; }
		set 
		{ 
			if (idSelected != value)
				ChangeSelection (idSelected, value);
			
			idSelected = value; 
		}
	}


	[SerializeField] bool keepLastSelectedMode = true;

	private void Start()
	{
        lifeBarHUD = FindObjectOfType<LifeBar>();

        IdSelected = 0;
	}

    private void Update()
    {
		if (menu.activeSelf == false) 
		{
			if (Input.GetButtonDown ("OpenMaskMenu")) 
			{
                lifeBarHUD.SetActiveHUD(true);
                lifeBarHUD.SetActiveNewMaskFB(false);

                TimeManager.Instance.Slow_AllScene (0.01f);
				transform.localScale = Vector3.zero;
			}
			if (Input.GetButton ("OpenMaskMenu")) 
			{			
				UpdateVisualMasks(); 
				masksParent.SetActive (true);
				GameLoopManager.EnableMouse ();
			} else if (Input.GetButtonUp ("OpenMaskMenu")) 
			{
                if(Potential_Enemy.IsOnFight == false)
                    lifeBarHUD.SetActiveHUD(false);

                TimeManager.Instance.Slow_UnactiveAll ();
				if (InputManager.GetInputMode == InputMode.joy && masks [idSelected].button.interactable)
					masks [idSelected].button.onClick.Invoke ();
				masksParent.SetActive (false);
				GameLoopManager.DisableMouse ();
			}

            if (transform.localScale.x < Vector3.one.x)
            {
                transform.localScale += Vector3.one * 10 * Time.unscaledDeltaTime;
            }
            else
                transform.localScale = Vector3.one;
		} 
		else if (masksParent.activeSelf == true) 
		{
			//TimeManager.Instance.Slow_UnactiveAll ();
			masksParent.SetActive (false);
		}
    }


    private void UpdateVisualMasks()
    {
        for(int i = 1; i < 4; i++) {
            if(MaskInventory.Instance.ListMasks.Count >= i)
            {
                masks[i].imgMask.sprite = MaskInventory.Instance.ListMasks[i - 1].Sprite2D;
                masks[i].imgMask.enabled = true;
                masks[i].button.interactable = true;
            }
            else
            {
                masks[i].imgMask.enabled = false;
                masks[i].button.interactable = false;
            }
        }

		if(InputManager.GetInputMode == InputMode.joy)
			IdSelected = MaskSelection ();

    }

    public void EquipDefaultMask()
    {
        MaskInventory.Instance.EquipDefaultMask();
    }

    public void EquipAMask(int _idMask)
    {
        MaskInventory.Instance.EquipAMask(MaskInventory.Instance.ListMasks[_idMask]);
    }

	void ChangeSelection(int lastId, int newId)
	{
		masks [newId].button.transform.localScale *= 1.5f;
		masks [lastId].button.transform.localScale = Vector3.one;
	}

	public int MaskSelection()
	{
		Vector2 dir2D = new Vector2 (Input.GetAxis (vd_Inputs.InputManager.CamX), Input.GetAxis (vd_Inputs.InputManager.CamY));
		int id = 0;

		Vector2 left_down = new Vector2 (-1, -1).normalized;
		Vector2 left_up = new Vector2 (-1, 1).normalized;

		float angleRight = 150.0f;

		bool up, right;

		float angleUp = Vector2.Dot (left_down, dir2D);
		if (angleUp > 0)
			up = true;
		else
			up = false;
			//id = 3; // masque du haut

		angleRight = Vector2.Dot (left_up, dir2D);
		if (angleRight > 0)
			right = true;
			//id = 2;//masque de droite
		else
			right = false;
			//id = 1;//masque de gauche
			
		if (up && !right)
			id = 0;
		else if (up && right)
			id = 3;
		else if (!up && right)
			id = 2;
		else if (!up && !right)
			id = 1;

		//Debug.Log ("up : " +up + " / right : " + right);

		if (dir2D.magnitude < 0.01f)
			return IdSelected;

		//Debug.Log (id);
		return id;
	}
}
