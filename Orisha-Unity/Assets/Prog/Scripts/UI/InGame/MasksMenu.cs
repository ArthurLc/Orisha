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

public class MasksMenu : MonoBehaviour {

    [System.Serializable]
    struct ButtonMask
    {
        public Button button;
        public Image imgMask;
    }

    [SerializeField] GameObject masksParent;
    [SerializeField] ButtonMask[] masks;

    private void Update()
    {
        if (Input.GetButton("OpenMaskMenu"))
        {
            UpdateVisualMasks();
            masksParent.SetActive(true);
            GameLoopManager.EnableMouse();
        }
        else if(Input.GetButtonUp("OpenMaskMenu"))
        {
            masksParent.SetActive(false);
            GameLoopManager.DisableMouse();
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
    }

    public void EquipDefaultMask()
    {
        MaskInventory.Instance.EquipDefaultMask();
    }
    public void EquipAMask(int _idMask)
    {
        MaskInventory.Instance.EquipAMask(MaskInventory.Instance.ListMasks[_idMask]);
    }
}
