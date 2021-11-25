using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(BoxCollider))]
public class VRClickableButton : MonoBehaviour
{
    private Button button;
    private BoxCollider boxCollider;
    private RectTransform rect;

    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent<Button>();
        boxCollider = GetComponent<BoxCollider>();
        rect = GetComponent<RectTransform>();

        if (rect) boxCollider.size = new Vector3(rect.rect.width, rect.rect.height, 1);
    }

    /// <summary>
    /// This method is called by the Main Camera when it starts gazing at this GameObject.
    /// </summary>
    public void OnPointerHover()
    {
        var funcs = button.onClick;
        funcs.Invoke();
    }

    private void OnGUI()
    {
        if (rect) boxCollider.size = new Vector3(rect.rect.width, rect.rect.height, 1);
    }
}
