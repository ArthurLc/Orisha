/* Author: Romain Seurot
 * Description: Indique ou se trouve l'autre joueur lorsqu'il n'apparait pas à l'écrant
 * 
 * Date of creation: 20/11/2017
 * Last date of creation and last modificator: 27/11/2017 Romain Seurot
 */


using UnityEngine;
using UnityEngine.UI;

public class OtherPlayerIndicator : MonoBehaviour
{
    public new Transform camera;
    public Transform myPlayer;
    public Transform otherPlayer;
    public Image cursor;

    private Vector3 camToOtherPlayerHorizontalAxis;

    [SerializeField]
    private bool debug = false;
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        CursorPositionOnScreen();
    }
    private void OnDrawGizmos()
    {

        if (debug)
        {

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(camera.position, (camera.position + (camera.forward * 3.0f)));

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(camera.position, (camera.position + (camToOtherPlayerHorizontalAxis * 3.0f)));
        }
    }

    private void CursorPositionOnScreen()
    {

        Vector3 tempPlayerVertPosition = new Vector3(otherPlayer.position.x, camera.position.y, otherPlayer.position.z);
        camToOtherPlayerHorizontalAxis = Vector3.Normalize(tempPlayerVertPosition - camera.position);
        Vector3 tempCamForwardHorizontal = Vector3.Normalize(new Vector3(camera.forward.x, camera.position.y, camera.forward.z));

        Vector3 pointOnScreen = camera.GetComponent<Camera>().WorldToViewportPoint(otherPlayer.position);
        float pointOnScreenX = (pointOnScreen.x - 0.5f);
        float pointOnScreenY = (pointOnScreen.y - 0.5f);

        pointOnScreenX = (pointOnScreenX > 0.45f) ? 0.45f : pointOnScreenX;
        pointOnScreenX = (pointOnScreenX < -0.45f) ? -0.45f : pointOnScreenX;


        // force the cursor to be down if the player is backward the camera
        if (Vector3.Dot(tempCamForwardHorizontal, camToOtherPlayerHorizontalAxis) >= 0)
        {
            pointOnScreenY = (pointOnScreenY > 0.45f) ? 0.45f : pointOnScreenY;
            pointOnScreenY = (pointOnScreenY < -0.45f) ? -0.45f : pointOnScreenY;
        }
        else
        {
            pointOnScreenY = -0.5f;
        }

        cursor.transform.localPosition = new Vector3(pointOnScreenX * 1920, pointOnScreenY * 1080, 1.0f); // set the position of the cursor

        float reScaleRatio = 1.0f;

        // Size of the cursor Image (between 0.0f and 1.0f)
        if (pointOnScreenY >= 0.44f || pointOnScreenY <= -0.44f || pointOnScreenX >= 0.44f || pointOnScreenX <= -0.44f)
        {
            reScaleRatio = 1.0f;
        }
        else
        {
            reScaleRatio = 0.0f;
        }

        ResizeCursor(reScaleRatio);

        if (debug)
        {
            Debug.Log("Dot = " + Vector3.Dot(tempCamForwardHorizontal, camToOtherPlayerHorizontalAxis));
        }

    }
    private void ResizeCursor(float ratio)
    {
        cursor.transform.localScale = new Vector3(ratio, ratio, 1.0f);
    }
}
