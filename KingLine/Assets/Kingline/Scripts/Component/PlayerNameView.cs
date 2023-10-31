using TMPro;
using UnityEngine;

public class PlayerNameView : MonoBehaviour
{
    [SerializeField]
    private TMP_Text m_nameLabel;
    private void OnEnable()
    {
        m_nameLabel.text = PlayerNetworkController.LocalPlayer.Name;
    }
}
