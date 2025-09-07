using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;

public class VRUIClickHelper : MonoBehaviour
{
    public Camera mouseCamera;

    public void CheckUIClick(Ray ray)
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = mouseCamera.WorldToScreenPoint(ray.origin + ray.direction * 5f);

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (var result in results)
        {
            var button = result.gameObject.GetComponent<Button>();
            if (button != null)
            {
                ExecuteEvents.Execute(button.gameObject, pointerData, ExecuteEvents.pointerClickHandler);
            }
        }
    }
}
