using UnityEngine;
using System.Collections.Generic;

/*
* @ArthurLacour
* @PlayerFight.cs
* @11/10/2017
* @Attaché au joueur, il permet d'attaquer et gère les animations à l'aide du FightAnimator.
*/

namespace vd_Player
{
    public class PlayerFight : MonoBehaviour
    {
        FightScriptable fightDatas;
        float strengthFactor = 1.0f; //Strength multiplicator (0 => 2)
        List<FightScriptable.InputRequire> listInputs;

        [SerializeField] CharacterInitialization ci;
        [SerializeField] Animator animator;
        
        FightScriptable.PlayerFightDatas playerFightDatas; //Datas d'attaque du joueur.
        float currentAnimDuration; //Stock la durée de l'animation joué.
        private bool isChain; //Booléan qui détermine si un combo va s'enchaîner.

        private GameObject ui_Masks;//Ui des masks
        private GameObject ui_Pause;//Ui du menu pause

        private PlayerPredictionAttack playerPredictionAttack;

        public FightScriptable.PlayerFightDatas PlayerFightDatas
        {
            get { return playerFightDatas; }
        }
        public List<FightScriptable.InputRequire> ListInputs
        {
            get { return listInputs; }
        }
        public float StrengthFactor
        {
            get { return strengthFactor;  }
            set { strengthFactor = value; }
        }

        void Start()
        {
            fightDatas = Resources.Load<FightScriptable>("FightAnimator"); //Récupération des datas liées au combat
            //numMaxInputsRequire = fightDatas.GetNumInputsMax(); //Je stock le nombre maximum d'inputs entrable par le joueur
            fightDatas.EraseCurrentPFD(); //Mise à zero des données de combats.
            listInputs = new List<FightScriptable.InputRequire>(); //Création du tableau d'inputs
            
            isChain = false;

            ui_Masks = GameObject.Find("Canvas_InGame").transform.GetChild(2).GetChild(0).gameObject;
            ui_Pause = GameObject.Find("Canvas_InGame").transform.GetChild(3).gameObject;

            playerPredictionAttack = animator.GetComponent<PlayerPredictionAttack>();
        }

        void Update()
        {
            if (ci.AreInputsFrozen)
                return;

            if (ci.PlayerController.IsGrounded)
            {
                if (ui_Masks != null && ui_Pause != null && !ui_Masks.activeSelf && !ui_Pause.activeSelf)
                {
                    if (IsPressAttackSlight())
                    {
                        if (listInputs.Count == 1)
                        {
                            PlayChainAttack();
                        }
                    }
                    else if (IsPressAttackHeavy())
                    {
                        if (listInputs.Count == 1)
                        {
                            PlayChainAttack();
                        }
                    }
                }
            }
        }


        /// <summary> Retourne si le joueur a appuyé sur le boutton AttackSlight.
        /// 
        /// </summary>
        /// <returns></returns>
        bool IsPressAttackSlight()
        {
            bool isButtonPress = Input.GetButtonDown(vd_Inputs.InputManager.AttackSlight);
            if (isButtonPress && !isChain && fightDatas.IsNextChainExist(listInputs, FightScriptable.InputRequire.AttackSlight))
            {
                listInputs.Add(FightScriptable.InputRequire.AttackSlight); //Ajoute l'input à la liste avant de retourner si le joueur a appuyé.
                isChain = true;
                return true;
            }
            return false;
        }
        /// <summary> Retourne si le joueur a appuyé sur le boutton AttackHeavy.
        /// 
        /// </summary>
        /// <returns></returns>
        bool IsPressAttackHeavy()
        {
            bool isButtonPress = Input.GetButtonDown(vd_Inputs.InputManager.AttackHeavy);
            if (isButtonPress && !isChain && fightDatas.IsNextChainExist(listInputs, FightScriptable.InputRequire.AttackHeavy))
            {
                listInputs.Add(FightScriptable.InputRequire.AttackHeavy); //Ajoute l'input à la liste avant de retourner si le joueur a appuyé.
                isChain = true;
                return true;
            }
            return false;
        }

        /// <summary> Joue une animation sur le player.
        /// 
        /// </summary>
        private void PlayNewAnimation()
        {
            playerFightDatas = fightDatas.SetCurrentPFD(listInputs); //Actualisation des données de combat.
            animator.SetTrigger(playerFightDatas.name_anim);
            animator.SetBool("IsAttacking", true);

            playerPredictionAttack.PredictAttack(playerFightDatas, true);

            ci.RotateToFaceEnemy(2.5f);
        }

        /// <summary> Lance la prochaine attaque selon les inputs si possible.
        /// 
        /// </summary>
        public void PlayChainAttack()
        {
            if (isChain) {
                PlayNewAnimation();
                isChain = false;
            }
            else {
                ClearListInputs();
            }
        }
        /// <summary> Clear la liste d'inputs.
        /// 
        /// </summary>
        /// <returns></returns>
        public void ClearListInputs()
        {
            animator.SetBool("IsAttacking", false);
            listInputs.Clear(); //La chaîne d'attaques se vide
            playerFightDatas = fightDatas.EraseCurrentPFD();
            isChain = false;
        }
    }
}
