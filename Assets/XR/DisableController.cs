using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class DisableController : MonoBehaviour
{
    public GameObject leftControllerModel;
    public GameObject rightControllerModel;

    void Start()
    {
        XRGrabInteractable grabInteractable = GetComponent<XRGrabInteractable>();
        grabInteractable.selectEntered.AddListener(HideGrabbingController);
        grabInteractable.selectExited.AddListener(ShowGrabbingController);
    }

    public void HideGrabbingController(SelectEnterEventArgs args)
    {
        if (args.interactorObject.transform.tag == "Left Controller")
        {
            leftControllerModel.SetActive(false);
        }
        else if (args.interactorObject.transform.tag == "Right Controller")
        {
            rightControllerModel.SetActive(false);
        }
    }

    public void ShowGrabbingController(SelectExitEventArgs args)
    {
        if (args.interactorObject.transform.tag == "Left Controller")
        {
            leftControllerModel.SetActive(true);
        }
        else if (args.interactorObject.transform.tag == "Right Controller")
        {
            rightControllerModel.SetActive(true);
        }
    }
}