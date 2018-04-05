/* CounterSlider.cs
* script contenant ce qui est utile à actualiser un Text.UI lorsqu'un slider bouge
* Objectifs : 
    - Modifier le text dans l'UI lorsque son slider bouge.

* Crée par Arthur LACOUR le 01/12/2018
* Dernière modification par Arthur LACOUR le 01/12/2018

*/
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class CounterSlider : MonoBehaviour {

    [SerializeField] Slider sliderUI;
    Text textUI;

    private void Start() {
        textUI = GetComponent<Text>();
        textUI.text = sliderUI.value.ToString("F");
    }

    public void UpdateCounter() {
        textUI.text = sliderUI.value.ToString("F");
    }
}
