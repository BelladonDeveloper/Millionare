using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;

public class AvatarSelect : MonoBehaviour
{
    public GameObject filesListPan, filesContent, filePrefab;
    public RawImage avatarImg;
    public GameObject logo;
    public GameObject answerButtonGroup;

    public static AvatarSelect instance;

    private DirectoryInfo directoryInfo = new DirectoryInfo("Avatars");
    private FileInfo[] files;
    private GameObject[] instancedObjects;

    private void Awake()
    {
        instance = this;
    }

    public void LoadAvatarList()
    {
        filesListPan.SetActive(true);
        logo.SetActive(false);
        answerButtonGroup.SetActive(false);
        files = new string[] { "*.jpeg", "*.jpg", "*.png" }.SelectMany(ext => directoryInfo.GetFiles(ext, SearchOption.TopDirectoryOnly)).ToArray();
        instancedObjects = new GameObject[files.Length];
        for (int i = 0; i < files.Length; i++)
        {
            FileScript file = Instantiate(filePrefab, filesContent.transform).GetComponent<FileScript>();
            file.fileNameText.text = files[i].Name;
            file.index = i;
            instancedObjects[i] = file.gameObject;
        }
    }

    public void SelectAvatar(int index)
    {
        WWW www = new WWW("file://" + files[index].FullName);
        avatarImg.texture = www.texture;

        filesListPan.SetActive(false);
        logo.SetActive(true);
        answerButtonGroup.SetActive(true);

        foreach (GameObject obj in instancedObjects)
            Destroy(obj);
    }
}
