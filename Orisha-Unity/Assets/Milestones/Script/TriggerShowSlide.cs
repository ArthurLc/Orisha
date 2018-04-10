using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using vd_Inputs;

namespace Milestone
{

    public class TriggerShowSlide : MonoBehaviour
    {
        public bool isMoving = true;

        public Image slide;
        public GameObject Modelisation;
        private bool isASlide;

        public GameObject sprite;
        private Vector3 initPosSprite;
        public GameObject target;
        public float maxTimerToMove;

        public GameObject nextPlateform;

        private bool isTriggered = false;
        private Color colorSlide;
        private Color colorSprite;

        float transparency = 0.0f;

        float timer = 0.0f;

        public KeyCode returnKey;
        public bool isAlwaysVisible = false;

        void Start()
        {
            if (Modelisation == null)
            {
                isASlide = true;
                if(isMoving)
                    initPosSprite = sprite.transform.position;

                if (isAlwaysVisible == true)
                {
                    colorSlide = slide.color;
                    colorSlide.a = 1.0f;
                    slide.color = colorSlide;

                    colorSprite = sprite.GetComponent<SpriteRenderer>().color;
                    colorSprite.a = 1.0f;
                    sprite.GetComponent<SpriteRenderer>().color = colorSprite;

                    Destroy(this);
                    return;
                }

                colorSlide = slide.color;
                colorSlide.a = transparency;
                slide.color = colorSlide;

                if (isMoving)
                {
                    colorSprite = sprite.GetComponent<SpriteRenderer>().color;
                    colorSprite.a = transparency;
                    sprite.GetComponent<SpriteRenderer>().color = colorSprite;
                }
            }
            else
            {
                isASlide = false;
                if(isMoving)
                    initPosSprite = Modelisation.transform.position;
            }
        }


        void Update()
        {
            if (isASlide)
            {
                if (isTriggered && transparency < 1.0f)
                {
                    timer += Time.deltaTime;
                    if (timer < 1.0f)
                    {
                        transparency = timer;
                    }
                    else
                    {
                        timer = 0.0f;
                        transparency = 1.0f;
                    }
                    colorSlide.a = transparency;
                    slide.color = colorSlide;

                    colorSprite.a = transparency;
                    if (isMoving)
                        sprite.GetComponent<SpriteRenderer>().color = colorSprite;
                }

                else if (isTriggered == false && transparency > 0.0f)
                {
                    timer += Time.deltaTime * 2.0f;
                    if (timer < 1.0f)
                    {
                        transparency = 1.0f - timer;
                    }
                    else
                    {
                        timer = 0.0f;
                        transparency = 0.0f;
                        if(isMoving)
                            StartCoroutine(MoveSprite());
                    }
                    colorSlide.a = transparency;
                    slide.color = colorSlide;

                }
            }
            else
            {
                if (isTriggered && transparency < 1.0f)
                {
                    Modelisation.SetActive(true);
                    transparency = 1.0f;
                }

                else if (isTriggered == false && transparency > 0.0f)
                {
                    transparency = 0.0f;
                    if(isMoving)
                        StartCoroutine(MoveSprite());
                }
            }
			if ((Input.GetKeyDown(returnKey) || Input.GetAxis(InputManager.Focus) > 0.5f) && isAlwaysVisible == false && isTriggered == true)
            {
                isTriggered = false;
                if(nextPlateform != null)
                    nextPlateform.SetActive(true);
            }

        }

        private IEnumerator MoveSprite()
        {
            bool isFinished = false;
            float timerToMove = 0.0f;
            bool isRightOrientation = false;
            Vector3 posOrigin;// = sprite.transform.position;
            Vector3 rotOrigin;// = sprite.transform.rotation.eulerAngles;
            float distance;// = target.transform.position.x - sprite.transform.position.x;

            if(isASlide)
            {
                posOrigin = sprite.transform.position;
                rotOrigin = sprite.transform.rotation.eulerAngles;
                distance = Vector3.Distance(target.transform.position, sprite.transform.position);
            }
            else
            {
                posOrigin = Modelisation.transform.position;
                rotOrigin = Modelisation.transform.rotation.eulerAngles;
                distance = Vector3.Distance(target.transform.position, Modelisation.transform.position);
            }

            if (isASlide)
            {
                if (Vector3.Dot(sprite.transform.forward, target.transform.right) < 0.0f)
                {
                    isRightOrientation = true;
                }
            }
            else
            {
                if (Vector3.Dot(Modelisation.transform.forward, target.transform.forward) < 0.0f)
                {
                    isRightOrientation = true;
                }
            }

            yield return new WaitForSeconds(1.0f);

            while(!isFinished)
            {
                timerToMove += Time.deltaTime;

                if(timerToMove < maxTimerToMove)
                {
                    if (isASlide)
                        sprite.transform.position = Vector3.Lerp(posOrigin, target.transform.position, timerToMove / maxTimerToMove);
                    else
                        Modelisation.transform.position = Vector3.Lerp(posOrigin, target.transform.position, timerToMove / maxTimerToMove);

                    if (isRightOrientation)
                    {
                        if (isASlide)
                            sprite.transform.Rotate(Vector3.up, Time.deltaTime * (90.0f / maxTimerToMove));
                        /*else
                            Modelisation.transform.Rotate(Vector3.up, Time.deltaTime * (90.0f / maxTimerToMove));*/
                    }
                    else
                    {
                        if (isASlide)
                            sprite.transform.Rotate(Vector3.up, Time.deltaTime * (-90.0f / maxTimerToMove));
                        /*else
                            Modelisation.transform.Rotate(Vector3.up, Time.deltaTime * (-90.0f / maxTimerToMove));*/
                    }
                }
                else
                {
                    if (isASlide)
                    {
                        sprite.transform.position = target.transform.position;
                        sprite.transform.rotation = target.transform.rotation;
                    }
                    else
                    {
                        Modelisation.transform.position = target.transform.position;
                        Modelisation.transform.rotation = target.transform.rotation;
                    }
                    isFinished = true;
                }
                yield return 0;
            }

        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                isTriggered = true;
            }

        }

        private void OnTriggerExit(Collider other)
        {
            if (other.tag == "Player")
                isTriggered = false;
        }
    }

}
