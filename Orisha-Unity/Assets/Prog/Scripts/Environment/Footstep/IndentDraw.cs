using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Wacki.IndentSurface
{

    public class IndentDraw : MonoBehaviour
    {
        public Texture2D texture;
        public Texture2D stampTexture;
        public Texture2D resetStampTexture;
        public RenderTexture tempTestRenderTexture;
        public int rtWidth = 2048;
        public int rtHeight = 2048;

        private RenderTexture targetTexture;
        private RenderTexture auxTexture;

        public Material mat;

        public Queue<Vector2> WritingPos = new Queue<Vector2>();

        void Awake()
        {
            targetTexture = new RenderTexture(rtWidth, rtHeight, 32);

            // temporarily use a given render texture to be able to see how it looks
            targetTexture = tempTestRenderTexture;
            auxTexture = new RenderTexture(rtWidth, rtHeight, 32);
            
            Material[] mats = GetComponent<Renderer>().materials;
            foreach (Material mat in mats)
            {
                if(mat.name == "mat_island_sand (Instance)")
                {
                    mat.SetTexture("_Indentmap", targetTexture);
                    break;
                }
            }
            Graphics.Blit(texture, targetTexture);

        }

        // add an indentation at a raycast hit position
        public void IndentAt(RaycastHit hit)
        {
            if (hit.collider.gameObject != this.gameObject)
                return;

            float x = hit.textureCoord.x;
            float y = hit.textureCoord.y;

            DrawAt(x * targetTexture.width, y * targetTexture.height, 1.0f);
            WritingPos.Enqueue(new Vector2(x * targetTexture.width, y * targetTexture.height));
        }

        /// <summary>
        /// todo:   it would probably be a bit more straight forward if we didn't use Graphics.DrawTexture
        ///         and instead handle everything ourselves. This way we could directly provide multiple 
        ///         texture coordinates to each vertex.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="alpha"></param>
        void DrawAt(float x, float y, float alpha)
        {
            Graphics.Blit(targetTexture, auxTexture);

            // activate our render texture
            RenderTexture.active = targetTexture;

            GL.PushMatrix();
            GL.LoadPixelMatrix(0, targetTexture.width, targetTexture.height, 0);

            x = Mathf.Round(x);
            y = Mathf.Round(y);

            // setup rect for our indent texture stamp to draw into
            Rect screenRect = new Rect();
            // put the center of the stamp at the actual draw position
            screenRect.x = x - stampTexture.width * 0.5f;
            screenRect.y = (targetTexture.height - y) - stampTexture.height * 0.5f;
            screenRect.width = stampTexture.width;
            screenRect.height = stampTexture.height;

            var tempVec = new Vector4();

            tempVec.x = screenRect.x / ((float)targetTexture.width);
            tempVec.y = 1 - (screenRect.y / (float)targetTexture.height);
            tempVec.z = screenRect.width / targetTexture.width;
            tempVec.w = screenRect.height / targetTexture.height;
            tempVec.y -= tempVec.w;

            mat.SetTexture("_MainTex", stampTexture);
            mat.SetVector("_SourceTexCoords", tempVec);
            mat.SetTexture("_SurfaceTex", auxTexture);

            // Draw the texture
            Graphics.DrawTexture(screenRect, stampTexture, mat);
            
            GL.PopMatrix();
            RenderTexture.active = null;
            
        }
        
        public void UpdateResetTex(float x, float y, float fade)
        {

            Graphics.Blit(targetTexture, auxTexture);

            // activate our render texture
            RenderTexture.active = targetTexture;

            GL.PushMatrix();
            GL.LoadPixelMatrix(0, targetTexture.width, targetTexture.height, 0);

            x = Mathf.Round(x);
            y = Mathf.Round(y);

            // setup rect for our indent texture stamp to draw into
            Rect screenRect = new Rect();
            // put the center of the stamp at the actual draw position
            screenRect.x = x - resetStampTexture.width * 0.5f;
            screenRect.y = (targetTexture.height - y) - resetStampTexture.height * 0.5f;
            screenRect.width = resetStampTexture.width;
            screenRect.height = resetStampTexture.height;

            var tempVec = new Vector4();

            tempVec.x = screenRect.x / ((float)targetTexture.width);
            tempVec.y = 1.0f;
            tempVec.z = screenRect.width / targetTexture.width;
            tempVec.w = screenRect.height / targetTexture.height;
            tempVec.y -= tempVec.w;

            Texture2D resetTex = new Texture2D(resetStampTexture.width, resetStampTexture.height);
            for (int j = 0; j < resetStampTexture.height; j++)
            {
                for (int i = 0; i < resetStampTexture.width; i++)
                {
                    resetTex.SetPixel(i, j, Color.Lerp(stampTexture.GetPixel(i, j), resetStampTexture.GetPixel(i, j), fade));
                }
            }
            resetTex.Apply();

            mat.SetTexture("_MainTex", resetTex);
            mat.SetVector("_SourceTexCoords", tempVec);
            mat.SetTexture("_SurfaceTex", auxTexture);

            // Draw the texture
            Graphics.DrawTexture(screenRect, resetTex, mat);

            GL.PopMatrix();
            RenderTexture.active = null;

        }

    }

}