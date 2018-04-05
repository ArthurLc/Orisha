using UnityEngine;
using System.Collections;


namespace Wacki.IndentSurface
{
    /// <summary>
    /// Simple control script for our sphere that leaves a track in the snow.
    /// </summary>
    public class IndentActor : MonoBehaviour
    {
        public float distance;
        public IndentDraw texDraw = null;

        private float timer = 0.0f;

        void Update()
        {
            //Debug.DrawLine(transform.position, transform.position + Vector3.down);

            RaycastHit hit;
            Vector2 tmpPos = Vector2.zero;
            timer += Time.deltaTime;
            if (timer >= 0.3f)
            {
                if (Physics.Raycast(transform.position, Vector3.down, out hit, distance))
                {
                    IndentDraw texDrawActual = hit.collider.gameObject.GetComponent<IndentDraw>();
                    if (texDrawActual == null)
                        return;

                    texDraw = texDrawActual;
                    texDraw.IndentAt(hit);
                    timer = 0.0f;
                }
            }
            if (texDraw == null)
                return;



            if (texDraw.WritingPos.Count > 2)
            {
                tmpPos = texDraw.WritingPos.Dequeue();
                StartCoroutine(FadeResetTex(tmpPos));
            }
        }


        public IEnumerator FadeResetTex(Vector2 _pos)
        {
            float actualFade = 0.0f;
            do
            {
                actualFade += 0.005f;

                if (actualFade > 0.9f)
                    actualFade = 0.9f;

                texDraw.UpdateResetTex(_pos.x, _pos.y, actualFade);

                yield return 0;
            } while (actualFade < 0.9f);

            yield break;
        }
    }
}