using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    public float interactDistance = 3f;

    private IInteractable currentInteractable;

    void Update()
    {
        //луч
        Debug.DrawRay(transform.position, transform.forward * interactDistance, Color.red);

        Ray ray = new Ray(transform.position, transform.forward);

        //луч с объектами
        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();

            if (interactable != null)
            {
                if (currentInteractable != interactable)
                {
                    currentInteractable?.OnFocusExit();

                    currentInteractable = interactable;

                    currentInteractable.OnFocusEnter();
                }

                if (Input.GetMouseButtonDown(0))
                {
                    currentInteractable.Interact();
                }

                return;
            }
        }

        currentInteractable?.OnFocusExit();

        currentInteractable = null;
    }
}