/* Author: Romain Seurot
* Description: Permet de parametrer le blockage du perso/ennemies et la slow motion sur chaque player et globalement
*               
* Date of creation: 27/03/2018
* Last date of creation and last modificator: 28/03/2018 Romain Seurot
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

#if UNITY_EDITOR
using UnityEditor;

/// <summary>
/// Allow the user to modify curves(add, remove, change,...) in the manager inspector
/// </summary>
[CustomEditor(typeof(TimeManager))]
public class TimeManagerEditor : Editor
{
    TimeManager myTarget;
    bool isOpen = false;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        myTarget = (TimeManager)target;

        isOpen = EditorGUILayout.Foldout(isOpen, "Curves");

        if (isOpen && myTarget != null)
        {
            if (myTarget.curvesDataBase.SlowMotionCurves != null)
            {
                EditorGUILayout.Space();
                EditorGUILayout.HelpBox("Be carefull, you have to set curves between 0 and 1 sec!", MessageType.Info);
                EditorGUILayout.Space();
                if (myTarget.curvesDataBase.SlowMotionCurves.Length > 0)
                {
                    for (int i = 0; i < myTarget.curvesDataBase.SlowMotionCurves.Length; i++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        myTarget.curvesDataBase.SlowMotionCurveNames[i] = EditorGUILayout.TextField("Name", myTarget.curvesDataBase.SlowMotionCurveNames[i]);
                        myTarget.curvesDataBase.SlowMotionCurves[i] = EditorGUILayout.CurveField("Curve", myTarget.curvesDataBase.SlowMotionCurves[i]);
                        EditorGUILayout.EndHorizontal();
                    }
                }
                EditorGUILayout.Space();
                EditorGUILayout.Space();

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Add curve"))
                {
                    AddCurve();
                    EditorUtility.SetDirty(myTarget.curvesDataBase);
                    AssetDatabase.SaveAssets();
                }
                if (myTarget.curvesDataBase.SlowMotionCurves.Length == 0)
                {
                    GUI.enabled = false;
                }
                if (GUILayout.Button("Remove curve"))
                {
                    RemoveCurve();
                    EditorUtility.SetDirty(myTarget.curvesDataBase);
                    AssetDatabase.SaveAssets();
                }
                EditorGUILayout.EndHorizontal();
                GUI.enabled = true;

                if (GUILayout.Button("Save curves"))
                {
                    EditorUtility.SetDirty(myTarget.curvesDataBase);
                    AssetDatabase.SaveAssets();
                }

            }
            else
            {
                myTarget.curvesDataBase.SlowMotionCurves = new AnimationCurve[0];
                myTarget.curvesDataBase.SlowMotionCurveNames = new string[0];
                EditorUtility.SetDirty(myTarget.curvesDataBase);
                AssetDatabase.SaveAssets();
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        myTarget.IsDebugActiveOn = EditorGUILayout.Toggle("Active Debug On", myTarget.IsDebugActiveOn);

        if (myTarget.IsDebugActiveOn)
        {
            EditorGUILayout.Space();
            EditorGUI.indentLevel = 1;
            EditorGUILayout.LabelField("SM scene: " + myTarget.IsSMGlobalSceneActive.ToString() + " " + Time.timeScale.ToString());

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("SM on charater: ");
            EditorGUI.indentLevel = 3;
            EditorGUILayout.LabelField("count =  " + myTarget.SlowMotionList.Count);
            EditorGUI.indentLevel = 1;
        }
        EditorGUI.indentLevel = 0;
    }

    private void AddCurve()
    {
        string[] tempTabName = new string[myTarget.curvesDataBase.SlowMotionCurveNames.Length + 1];
        AnimationCurve[] tempTab = new AnimationCurve[myTarget.curvesDataBase.SlowMotionCurves.Length + 1];
        for (int i = 0; i < myTarget.curvesDataBase.SlowMotionCurves.Length; i++)
        {
            tempTabName[i] = myTarget.curvesDataBase.SlowMotionCurveNames[i];
            tempTab[i] = myTarget.curvesDataBase.SlowMotionCurves[i];
        }
        for (int i = myTarget.curvesDataBase.SlowMotionCurves.Length; i < tempTabName.Length; i++)
        {
            tempTabName[i] = "";
            tempTab[i] = new AnimationCurve();
        }


        myTarget.curvesDataBase.SlowMotionCurveNames = tempTabName;
        myTarget.curvesDataBase.SlowMotionCurves = tempTab;
    }
    private void RemoveCurve()
    {
        string[] tempTabName = new string[myTarget.curvesDataBase.SlowMotionCurveNames.Length - 1];
        AnimationCurve[] tempTab = new AnimationCurve[myTarget.curvesDataBase.SlowMotionCurves.Length - 1];
        for (int i = 0; i < tempTabName.Length; i++)
        {
            tempTabName[i] = myTarget.curvesDataBase.SlowMotionCurveNames[i];
            tempTab[i] = myTarget.curvesDataBase.SlowMotionCurves[i];
        }

        myTarget.curvesDataBase.SlowMotionCurveNames = tempTabName;
        myTarget.curvesDataBase.SlowMotionCurves = tempTab;
    }
}

#endif

/// <summary>
/// Description: Permet de parametrer le blockage du perso/ennemies et la slow motion sur chaque character ou globalement
/// </summary>
public class TimeManager : MonoBehaviour
{
    private static TimeManager instance;
    public static TimeManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameObject("Blockage_SlowMo_Manager").AddComponent<TimeManager>();
            }
            return instance;
        }
    }

    [SerializeField]
    public Curves curvesDataBase; // scriptableObject qui contient les curves

    private Coroutine smGlobalScene;
    private bool isSMGlobalSceneActive;
    public bool IsSMGlobalSceneActive
    {
        get
        {
            return isSMGlobalSceneActive;
        }

        set
        {
            isSMGlobalSceneActive = value;
        }
    }

    private vd_Player.CharacterInitialization ciPlayer = null;
    private Coroutine blockPlayer;

    private Dictionary<Animator, Coroutine> slowMotionList = new Dictionary<Animator, Coroutine>();
    public Dictionary<Animator, Coroutine> SlowMotionList
    {
        get
        {
            return slowMotionList;
        }

        set
        {
            slowMotionList = value;
        }
    }
    private Dictionary<Animator, float> slowMotionSpeedAtBegin = new Dictionary<Animator, float>();
    public Dictionary<Animator, float> SlowMotionSpeedAtBegin
    {
        get
        {
            return slowMotionSpeedAtBegin;
        }

        set
        {
            slowMotionSpeedAtBegin = value;
        }
    }

    private Dictionary<GameObject, Coroutine> blockageEventList = new Dictionary<GameObject, Coroutine>();
    public Dictionary<GameObject, Coroutine> BlockageEventList
    {
        get
        {
            return blockageEventList;
        }

        set
        {
            blockageEventList = value;
        }
    }

    void Awake()
    {
        if (instance != null)
        {
            Destroy(instance.gameObject);
        }

        instance = this;
    }


    #region SlowMo_GlobalScene

    /// <summary>
    /// Active the slow motion on all the current scene with a timer => the slow motion will be unactivate automaticly at the end of the timer
    /// </summary>
    /// <param name="_timer"> duration of the slow motion </param>
    /// <param name="_value">value of the slow motion => between 0.0f & 1.0f (normal = 1.0f)</param>
    public void Slow_AllScene_WithTimer(float _timer, float _value)
    {
        Slow_UnactiveAll();
        isSMGlobalSceneActive = true;
        smGlobalScene = StartCoroutine(Slow_AllSceneTimer(_timer, _value));
    }
    private IEnumerator Slow_AllSceneTimer(float _timer, float _value)
    {
        float currentTimer = 0.0f;
        Time.timeScale = _value;

        while (currentTimer < _timer)
        {
            currentTimer += Time.unscaledDeltaTime;
            yield return 0;
        }

        Time.timeScale = 1.0f;
        isSMGlobalSceneActive = false;
    }
    /// <summary>
    /// Active the slow motion on all the current scene without timer => you must unactive it manually later
    /// </summary>
    /// <param name="_value"> value of the slow motion => between 0.0f & 1.0f (normal = 1.0f)</param>
    public void Slow_AllScene(float _value)
    {
        Slow_UnactiveAll();
        Time.timeScale = _value;
        isSMGlobalSceneActive = true;
    }
    /// <summary>
    /// Active the slow motion on all the current scene with timer and curve => the slow motion will be unactivate automaticly at the end of the timer
    /// </summary>
    /// <param name="_timer"> duration of the slow motion</param>
    /// <param name="_curveName"> name of the curve to use</param>
    public void Slow_AllScene_WithCurve(float _timer, string _curveName)
    {
        Slow_UnactiveAll();

        if (curvesDataBase.SlowMotionCurveNames.Length > 0)
        {
            AnimationCurve theOne = null;
            for (int i = 0; i < curvesDataBase.SlowMotionCurveNames.Length; i++)
            {
                if (curvesDataBase.SlowMotionCurveNames[i] == _curveName)
                {
                    theOne = curvesDataBase.SlowMotionCurves[i];
                }
            }

            if (theOne != null)
            {
                isSMGlobalSceneActive = true;
                smGlobalScene = StartCoroutine(SlowMotion_GlobalSceneTimer_WithCurve(_timer, theOne));
            }
        }
    }
    private IEnumerator SlowMotion_GlobalSceneTimer_WithCurve(float _timer, AnimationCurve _value)
    {
        float currentTimer = 0.0f;

        while (currentTimer < _timer)
        {
            currentTimer += Time.unscaledDeltaTime;

            Time.timeScale = _value.Evaluate((currentTimer / _timer));

            yield return 0;
        }

        Time.timeScale = 1.0f;
        isSMGlobalSceneActive = false;
    }

    /// <summary>
    /// Unactive all slow motions
    /// </summary>
    public void Slow_UnactiveAll()
    {
        if (smGlobalScene != null)
        {
            StopCoroutine(smGlobalScene);
        }
        Slow_OneCharacter_UnactiveAll();

        Time.timeScale = 1.0f;
        isSMGlobalSceneActive = false;
    }

    #endregion

    #region SlowMo_OneCharacter

    /// <summary>
    /// Active the slow motion for one character with a timer => the slow motion will be unactivate automaticly at the end of the timer
    /// </summary>
    /// <param name="_characterAnimator">the animator of the character to slow</param>
    /// <param name="_timer">duration of the slow motion</param>
    /// <param name="_value">value of the slow motion => between 0.0f & 1.0f (normal = 1.0f)</param>
    public void Slow_OneCharacter_WithTimer(GameObject _goCharacter, float _timer, float _value)
    {
		//Debug.Log ("SM_OneCharacter_Timer");
        Animator currentAnimator;
        currentAnimator = _goCharacter.GetComponent<Animator>();


        if (currentAnimator == null)
        {
            currentAnimator = _goCharacter.GetComponentInParent<Animator>();
        }
        if (currentAnimator == null)
        {
            currentAnimator = _goCharacter.GetComponentInChildren<Animator>();
        }

        if (currentAnimator != null)
        {
			
			if (!Slow_OneCharacter_Unactive (currentAnimator.gameObject, true)) 
			{
				float animationSpeedAtBegin = currentAnimator.speed;
				//Debug.Log (_goCharacter.name + " " + animationSpeedAtBegin);
				slowMotionSpeedAtBegin.Add (currentAnimator, animationSpeedAtBegin);
			}
            Coroutine currentCoroutine = StartCoroutine(Slow_OneCharacterTimer(currentAnimator, _timer, _value));
            slowMotionList.Add(currentAnimator, currentCoroutine);
        }

    }
    private IEnumerator Slow_OneCharacterTimer(Animator _characterAnimator, float _timer, float _value)
    {
		if (_characterAnimator) 
		{			
	        float currentTimer = 0.0f;
	        _characterAnimator.speed = _value;
			//Debug.Log (_characterAnimator.gameObject.name + " " + _value);

	        while (currentTimer < _timer)
	        {
	            currentTimer += Time.deltaTime;
	            yield return 0;
	        }

			//if(slowMotionSpeedAtBegin.ContainsKey(_characterAnimator))

			_characterAnimator.speed = slowMotionSpeedAtBegin [_characterAnimator];
			//Debug.Log (_characterAnimator.gameObject.name + " " + _characterAnimator.speed);
			slowMotionSpeedAtBegin.Remove (_characterAnimator);
			slowMotionList.Remove (_characterAnimator);
		}
    }
    /// <summary>
    /// Active the slow motion for one character without timer => you must unactive it manually later
    /// </summary>
    /// <param name="_characterAnimator">the animator of the character to slow</param>
    /// <param name="_value">value of the slow motion => between 0.0f & 1.0f (normal = 1.0f)</param>
    public void Slow_OneCharacter(GameObject _goCharacter, float _value)
    {
        Animator currentAnimator;
        currentAnimator = _goCharacter.GetComponent<Animator>();

        if (currentAnimator == null)
        {
            currentAnimator = _goCharacter.GetComponentInParent<Animator>();
        }
        if (currentAnimator == null)
        {
            currentAnimator = _goCharacter.GetComponentInChildren<Animator>();
        }

        if (currentAnimator != null)
        {
            float animationSpeedAtBegin = currentAnimator.speed;

			if(!Slow_OneCharacter_Unactive(currentAnimator.gameObject, true))
               	slowMotionSpeedAtBegin.Add(currentAnimator, animationSpeedAtBegin);


            currentAnimator.speed = _value;
        }
    }
    /// <summary>
    /// Active the slow motion for one character with timer and curve => the slow motion will be unactivate automaticly at the end of the timer
    /// </summary>
    /// <param name="_characterAnimator">the animator of the character to slow</param>
    /// <param name="_timer">duration of the slow motion</param>
    /// <param name="_curveName">name of the curve to use</param>
    public void Slow_OneCharacter_WithCurve(GameObject _goCharacter, float _timer, string _curveName)
    {
        if (curvesDataBase.SlowMotionCurveNames.Length > 0)
        {
            AnimationCurve theOne = null;
            for (int i = 0; i < curvesDataBase.SlowMotionCurveNames.Length; i++)
            {
                if (curvesDataBase.SlowMotionCurveNames[i] == _curveName)
                {
                    theOne = curvesDataBase.SlowMotionCurves[i];
                }
            }

            if (theOne != null)
            {
				
                Animator currentAnimator;
                currentAnimator = _goCharacter.GetComponent<Animator>();

                if (currentAnimator == null)
                {
                    currentAnimator = _goCharacter.GetComponentInParent<Animator>();
                }
                if (currentAnimator == null)
                {
                    currentAnimator = _goCharacter.GetComponentInChildren<Animator>();
                }

                if (currentAnimator != null)
                {
                    float animationSpeedAtBegin = currentAnimator.speed;

					if(!Slow_OneCharacter_Unactive(currentAnimator.gameObject, true))
						slowMotionSpeedAtBegin.Add(currentAnimator, animationSpeedAtBegin);

                    Coroutine currentCoroutine = StartCoroutine(Slow_OneCharacterCurve(currentAnimator, _timer, theOne));
                    slowMotionList.Add(currentAnimator, currentCoroutine);
                }
            }
        }
    }
    private IEnumerator Slow_OneCharacterCurve(Animator _characterAnimator, float _timer, AnimationCurve _value)
    {
		if (_characterAnimator) 
		{
	        float currentTimer = 0.0f;

	        while (currentTimer < _timer)
	        {
	            currentTimer += Time.deltaTime;

	            _characterAnimator.speed = _value.Evaluate((currentTimer / _timer));

	            yield return 0;
	        }

			_characterAnimator.speed = slowMotionSpeedAtBegin [_characterAnimator];
			slowMotionSpeedAtBegin.Remove (_characterAnimator);
			slowMotionList.Remove (_characterAnimator);
		}
    }

    /// <summary>
    /// Unactive the slow motion for one character
    /// </summary>
    /// <param name="_characterAnimator">the animator of the character to unactive slow motion</param>
	public bool Slow_OneCharacter_Unactive(GameObject _goCharacter, bool _becauseNewSlowMotion = false)
    {
		bool hasAlreadyASM = false;
        Animator currentAnimator;
        currentAnimator = _goCharacter.GetComponent<Animator>();

        if (currentAnimator == null)
        {
            currentAnimator = _goCharacter.GetComponentInParent<Animator>();
        }
        if (currentAnimator == null)
        {
            currentAnimator = _goCharacter.GetComponentInChildren<Animator>();
        }

        if (currentAnimator != null)
        {			
            if (slowMotionList.ContainsKey(currentAnimator))
            {
				StopCoroutine (slowMotionList[currentAnimator]);
                slowMotionList.Remove(currentAnimator);
            }
			if (slowMotionSpeedAtBegin.ContainsKey(currentAnimator))
            {
				hasAlreadyASM = true;
                currentAnimator.speed = slowMotionSpeedAtBegin[currentAnimator];
				if (!_becauseNewSlowMotion) 
				{
					Debug.Log ("DestroyInforrmation " + currentAnimator.gameObject.name);
					slowMotionSpeedAtBegin.Remove (currentAnimator);
				}
            }
            else
            {
                currentAnimator.speed = 1.0f;
            }
        }

		return hasAlreadyASM;
    }
    /// <summary>
    /// Unactive the slow motion for each characters slow
    /// </summary>
    public void Slow_OneCharacter_UnactiveAll()
    {
        if (slowMotionList.Count > 0)
        {
            foreach (KeyValuePair<Animator, Coroutine> current in slowMotionList)
            {
                Slow_OneCharacter_Unactive(current.Key.gameObject);
            }
        }
    }

    #endregion

    #region Blockage_Player

    /// <summary>
    /// Active the blockage of the player position and rotation => you must unfreeze it manually (call "Unblock_Player")
    /// </summary>
    /// <param name="freezeAnimation">if true => the player will be freeze in the current animation (like if the time was stopped)</param>
    public void Block_Player(bool freezeAnimation = false)
    {
        Unblock_Player();

        if (ciPlayer == null)
        {
            ciPlayer = GameObject.FindObjectOfType<vd_Player.CharacterInitialization>();
        }

        if (ciPlayer != null)
        {
            ciPlayer.FreezeInputs();
            if (freezeAnimation)
            {
                Slow_OneCharacter(ciPlayer.Anim.gameObject, 0.0f);
            }
        }
        else
        {
            Debug.LogError("Impossible de trouver le CharacterInitialization du Player pour freeze ses Input!");
        }
    }

    /// <summary>
    /// Active the blockage of the player position and rotation => the blockage will be remove automaticly at the end of the timer
    /// </summary>
    /// <param name="_timer">duration of the blockage</param>
    /// <param name="freezeAnimation">if true => the player will be freeze in the current animation (like if the time was stopped)</param>
    public void Block_Player_WithTimer(float _timer, bool freezeAnimation = false)
    {
        Unblock_Player();

        if (ciPlayer == null)
        {
            ciPlayer = GameObject.FindObjectOfType<vd_Player.CharacterInitialization>();
        }

        if (ciPlayer != null)
        {
            blockPlayer = StartCoroutine(BlockPlayer_Timer(_timer));
            if (freezeAnimation)
            {
                Slow_OneCharacter_WithTimer(ciPlayer.Anim.gameObject, _timer, 0.0f);
            }
        }
        else
        {
            Debug.LogError("Impossible de trouver le CharacterInitialization du Player pour freeze ses Input!");
        }
    }
    private IEnumerator BlockPlayer_Timer(float _timer)
    {
        ciPlayer.FreezeInputs();

        float currentTimer = 0.0f;

        while (currentTimer < _timer)
        {
            currentTimer += Time.unscaledDeltaTime;
            yield return 0;
        }
        ciPlayer.UnfreezeInputs();
    }

    /// <summary>
    /// Unblock the player => the player will be able to move
    /// </summary>
    /// <param name="UnFreezeAnimation">if true => the player animation will be unfreeze</param>
    public void Unblock_Player(bool UnFreezeAnimation = false)
    {
        if (ciPlayer == null)
        {
            ciPlayer = GameObject.FindObjectOfType<vd_Player.CharacterInitialization>();
        }

        if (ciPlayer != null)
        {
            if (blockPlayer != null)
                StopCoroutine(blockPlayer);
            ciPlayer.UnfreezeInputs();
            if (UnFreezeAnimation)
            {
                Slow_OneCharacter_Unactive(ciPlayer.Anim.gameObject);
            }
        }
        else
        {
            Debug.LogError("Impossible de trouver le CharacterInitialization du Player pour freeze ses Input!");
        }
    }

    #endregion

    #region Blockage_Character


    /// <summary>
    /// Active the blockage of the Character position and rotation => you must unfreeze it manually (call "Unblock_Character")
    /// </summary>
    /// <param name="_goCharacter">Any gameObject of the character hierarchy</param>
    /// <param name="freezeAnimation">if true => the character will be freeze in the current animation (like if the time was stopped)</param>
    public void Block_Character(GameObject _goCharacter, bool freezeAnimation = false)
    {
        AI_Enemy_Basic currentAgent;
        currentAgent = _goCharacter.GetComponent<AI_Enemy_Basic>();

        if (currentAgent == null)
        {
            currentAgent = _goCharacter.GetComponentInParent<AI_Enemy_Basic>();
        }
        if (currentAgent == null)
        {
            currentAgent = _goCharacter.GetComponentInChildren<AI_Enemy_Basic>();
        }

        if (currentAgent != null)
        {
            Unblock_Character(currentAgent.gameObject, true);

            currentAgent.FreezePosRot();
            if (freezeAnimation)
            {
                Slow_OneCharacter(currentAgent.gameObject, 0.0f);
            }
        }
        else
        {
            Debug.LogError("Impossible de trouver le AI_Enemy_Basic du Character pour freeze ses Input!");
        }
    }

    /// <summary>
    /// Active the blockage of the Character position and rotation => the blockage will be remove automaticly at the end of the timer
    /// </summary>
    /// <param name="_goCharacter">Any gameObject of the character hierarchy</param>
    /// <param name="_timer">duration of the blockage</param>
    /// <param name="freezeAnimation">if true => the character will be freeze in the current animation (like if the time was stopped)</param>
    public void Block_Character_WithTimer(GameObject _goCharacter, float _timer, bool freezeAnimation = false)
    {

        AI_Enemy_Basic currentAgent;
        currentAgent = _goCharacter.GetComponent<AI_Enemy_Basic>();

        if (currentAgent == null)
        {
            currentAgent = _goCharacter.GetComponentInParent<AI_Enemy_Basic>();
        }
        if (currentAgent == null)
        {
            currentAgent = _goCharacter.GetComponentInChildren<AI_Enemy_Basic>();
        }

        if (currentAgent != null)
        {
            Unblock_Character(currentAgent.gameObject, true);

            Coroutine tempNewCoroutine = StartCoroutine(BlockCharacter_Timer(currentAgent, _timer));
            blockageEventList.Add(currentAgent.gameObject, tempNewCoroutine);

            if (freezeAnimation)
            {
                Slow_OneCharacter_WithTimer(currentAgent.gameObject, _timer, 0.0f);
            }
        }
        else
        {
            Debug.LogError("Impossible de trouver le AI_Enemy_Basic du Character pour freeze ses Input!");
        }
    }
    private IEnumerator BlockCharacter_Timer(AI_Enemy_Basic _basic_Enemy, float _timer)
    {
        float currentTimer = 0.0f;

        _basic_Enemy.FreezePosRot();

        while (currentTimer < _timer)
        {
            currentTimer += Time.unscaledDeltaTime;
            yield return 0;
        }
        _basic_Enemy.UnfreezePosRot();

        if (blockageEventList.ContainsKey(_basic_Enemy.gameObject))
        {
            blockageEventList.Remove(_basic_Enemy.gameObject);
        }
    }

    /// <summary>
    /// Unblock the character => the character will be able to move
    /// </summary>
    /// <param name="_goCharacter">Any gameObject of the character hierarchy</param>
    /// <param name="UnFreezeAnimation">if true => the player animation will be unfreeze</param>
    public void Unblock_Character(GameObject _goCharacter, bool UnFreezeAnimation = false)
    {
        AI_Enemy_Basic currentAgent = null;
        currentAgent = _goCharacter.GetComponent<AI_Enemy_Basic>();

        if (currentAgent == null)
        {
            currentAgent = _goCharacter.GetComponentInParent<AI_Enemy_Basic>();
        }
        if (currentAgent == null)
        {
            currentAgent = _goCharacter.GetComponentInChildren<AI_Enemy_Basic>();
        }

        if (currentAgent != null)
        {
            currentAgent.UnfreezePosRot();
            if (blockageEventList.ContainsKey(currentAgent.gameObject))
            {
                blockageEventList.Remove(currentAgent.gameObject);
            }

                Slow_OneCharacter_Unactive(currentAgent.gameObject);
        }
        else
        {
            Debug.LogWarning("Impossible de trouver le AI_Enemy_Basic du Character pour Unfreeze ses Input!");

        }
    }

    #endregion

    #region Blockage_Ennemies
    /// <summary>
    /// Active the blockage of all Ennemies (around the Player) position and rotation => the blockage will be remove automaticly at the end of the timer
    /// </summary>
    /// <param name="_timer">duration of the blockage</param>
    /// <param name="freezeAnimation">if true => the character will be freeze in the current animation (like if the time was stopped)</param>
    public void Block_Ennemies_WithTimer(float _timer, bool freezeAnimation = false)
    {
        LayerMask l_mask = 1 << LayerMask.NameToLayer("Enemy");
        Collider[] results = Physics.OverlapSphere(transform.position, 750.0f, l_mask, QueryTriggerInteraction.Collide);

        if (results.Length > 0)
        {
            bool enemyIsAlive = false;
            for (int i = 0; i < results.Length; i++)
            {
                enemyIsAlive = false;

                AI_Enemy_Frontal enFront = results[i].gameObject.GetComponent<AI_Enemy_Frontal>();
                AI_Enemy_Harasser enHarass = results[i].gameObject.GetComponent<AI_Enemy_Harasser>();
                AI_Enemy_SandTracker enSand = results[i].gameObject.GetComponent<AI_Enemy_SandTracker>();

                if (enFront != null && enFront.myState != AI_Enemy_Frontal.State.Die)
                {
                    enemyIsAlive = true;
                }
                else if (enHarass != null && enHarass.myState != AI_Enemy_Harasser.State.Die)
                {
                    enemyIsAlive = true;
                }
                else if (enSand != null && enSand.myState != AI_Enemy_SandTracker.State.Die)
                {
                    enemyIsAlive = true;
                }

                if (enemyIsAlive)
                    TimeManager.Instance.Block_Character_WithTimer(results[i].gameObject, _timer, freezeAnimation);
            }
        }
    }
    #endregion


    #region Debug Tools

    private bool isDebugActiveOn = false;
    public bool IsDebugActiveOn
    {
        get
        {
            return isDebugActiveOn;
        }

        set
        {
            isDebugActiveOn = value;
        }
    }

    private void Update()
    {
        if (isDebugActiveOn)
        {
            if (Input.GetKeyDown(KeyCode.End)) // slow mo on global scene
            {
                //Slow_AllScene(0.2f);
                //Slow_AllScene_WithTimer(2.0f, 0.2f);
                Slow_AllScene_WithCurve(10.0f, "Parabole");
            }
        }
    }

    #endregion
}
