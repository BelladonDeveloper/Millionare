using UnityEngine;
using UnityEngine.UI;

public class SelectTable : MonoBehaviour
{
    private string levelAddress = "gid=0&";

    public void SelectLevel(Text numberLevel)
    {
        int number = int.Parse(numberLevel.name);

        switch (number)
        {
            case 1: levelAddress = "gid=0&";          break;
            case 2: levelAddress = "gid=705584265&";  break;
            case 3: levelAddress = "gid=311005884&";  break;
            case 4: levelAddress = "gid=2049673348&"; break;
            case 5: levelAddress = "gid=340487417&";  break;

            default: levelAddress = "gid=0&"; break;
        }
        
        ListAddressText.levelAddress = levelAddress;
        
        LoadQuestion.instance.LoadTable();

        gameObject.SetActive(false);
    }
}
