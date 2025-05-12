using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SelectablePanel : MonoBehaviour, IPointerClickHandler
{
    public Image backgroundImage;
    public Color selectedColor = new Color(0.7f, 0.7f, 1f, 1f);
    public Color normalColor = Color.white;

    public static SelectablePanel currentlySelected;

    private void Start()
    {
        if (backgroundImage == null)
            backgroundImage = GetComponent<Image>();
        backgroundImage.color = normalColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (currentlySelected != null && currentlySelected != this)
            currentlySelected.Deselect();

        Select();
        currentlySelected = this;
    }

    public void Select()
    {
        backgroundImage.color = selectedColor;
        // TODO: 추가 동작 (예: 선택된 데이터 처리)
    }

    public void Deselect()
    {
        backgroundImage.color = normalColor;
    }
}
