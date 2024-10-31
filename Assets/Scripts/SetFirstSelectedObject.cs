using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SetFirstSelectedObject : MonoBehaviour
{
    public GameObject m_selectedGameObject;

    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(m_selectedGameObject);
        //�n�C���C�g�΍�
        m_selectedGameObject.GetComponent<Button>().OnSelect(null);
    }
}

class IgnoreMouseInputModule : StandaloneInputModule
{
    public override void Process()
    {
        bool usedEvent = SendUpdateEventToSelectedObject();

        if (eventSystem.sendNavigationEvents)
        {
            if (!usedEvent)
                usedEvent |= SendMoveEventToSelectedObject();

            if (!usedEvent)
                SendSubmitEventToSelectedObject();
        }

        //������
        //ProcessMouseEvent();
    }
}
