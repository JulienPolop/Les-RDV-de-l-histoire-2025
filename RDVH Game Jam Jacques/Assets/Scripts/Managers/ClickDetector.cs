using UnityEngine;
using UnityEngine.InputSystem;

public class ClickDetector : MonoBehaviour
{
    public bool IsActive => active;
    private bool active = false;

    private InteractWithPoint currentPointed = null;

    public void SetActive(bool active)
    {
        this.active = active;
        if (!this.active)
        {
            if (currentPointed != null)
            {
                currentPointed.OnHoverEnd?.Invoke();
                currentPointed = null;
            }
        }
    }

    void Update()
    {
        if (!active) return;
        // Cast a ray from the camera to the mouse position
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        RaycastHit hit;
        InteractWithPoint pointed = null;

        // If the ray hits something
        if (Physics.Raycast(ray, out hit))
        {
            // Oh oh oh Something is hoovered !
            Debug.Log("Mouse over: " + hit.collider.gameObject.name);
            pointed = hit.collider.GetComponent<InteractWithPoint>();
        }

        if (pointed != null)
        {
            // Oh oh oh Something interesting is hoovered !
            
            // If something is already pointed and is different, end the current hoover
            if (currentPointed != null && pointed != currentPointed)
            {
                currentPointed.OnHoverEnd?.Invoke();
                currentPointed = null;
            }
            // If the current pointed is new, call a start event
            if (currentPointed == null)
            {
                pointed.OnHoverStart?.Invoke();
                currentPointed = pointed;
            }
        }
        else
        {
            // Oooooh nothing interesting is hoovered...
            // If something was pointed, end the pointing
            if (currentPointed != null)
            {
                currentPointed.OnHoverEnd?.Invoke();
                currentPointed = null;
            }
        }


        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (currentPointed != null)
            {
                currentPointed.OnClick?.Invoke();
            }
        }
    }
}