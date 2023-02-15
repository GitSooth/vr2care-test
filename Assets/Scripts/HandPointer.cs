using NuitrackSDK;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HandPointer : MonoBehaviour
{
    public enum Hands { left = 0, right = 1 };

    [SerializeField]
    Hands currentHand;

    [Header("Visualization")]
    [SerializeField]
    RectTransform parentRectTransform, button;

    [SerializeField]
    RectTransform baseRect;

    [SerializeField]
    Sprite defaultSprite;

    [SerializeField]
    Sprite pressSprite;

    [SerializeField]
    [Range(0, 50)]
    float minVelocityInteractivePoint = 2f;

    bool active = false;
    bool Press = false;

    bool col = false;

    float timer = 0, maxTime = 2;


    private void Update()
    {
        active = false;

        UserData user = NuitrackManager.Users.Current;

        if (user != null)
        {
            UserData.Hand handContent = currentHand == Hands.right ? user.RightHand : user.LeftHand;

            if (handContent != null)
            {
                Vector3 lastPosition = baseRect.position;
                baseRect.anchoredPosition = handContent.AnchoredPosition(parentRectTransform.rect, baseRect);

                float velocity = (baseRect.position - lastPosition).magnitude / Time.deltaTime;

                // To avoid false positives, check the hand clenching
                // if the movement velocity is lower than the set speed
                if (velocity < minVelocityInteractivePoint)
                    Press = handContent.Click;
                active = true;
            }
        }

        Press = Press && active;


        if (this.transform.localPosition.x >= button.localPosition.x - button.rect.width / 2 && this.transform.localPosition.x <= button.localPosition.x + button.rect.width / 2
        && this.transform.localPosition.y >= button.localPosition.y - button.rect.height / 2 && this.transform.localPosition.y <= button.localPosition.y + button.rect.height / 2)
            col = true;
        else
            col = false;

        Debug.Log("Col + " + col);

        if (col == true)
        {
            timer += Time.deltaTime;

            if (timer >= maxTime)
            {
                Debug.Log("EEEEEEENNNNNNNDDDDD");
                nuitrack.Nuitrack.Release();
                Application.Quit();
            }
        }
        else
        {
            timer = 0;
        }
    }
}
