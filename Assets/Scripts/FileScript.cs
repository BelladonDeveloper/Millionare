using UnityEngine;
using UnityEngine.UI;

public class FileScript : MonoBehaviour
{
    public Text fileNameText;

    [HideInInspector]
    public int index;

    public void OnClick()
    {
        AvatarSelect.instance.SelectAvatar(index);
    }

    public void OnClickSelectFolder()
    {
        LoadQuestion.instance.SelectFolder(index);
    }
}
