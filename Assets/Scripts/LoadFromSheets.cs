using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public static class LoadFromSheets
{ 
    public static string googleSheetDocID = "1CWr1c8zjcoBO_POa4q1oppJPdT_1aahRylgNKeMMVaA";
    public static string levelAddress = ListAddressText.levelAddress;
    private static string url = "https://docs.google.com/spreadsheets/d/" +
        googleSheetDocID + "/export?" + levelAddress + "format=csv";

    internal static IEnumerator DownloadData ( System.Action<string> onCompleted )
    {
        yield return new WaitForEndOfFrame();

        string downloadData = null;

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                Debug.Log("Download Error: " + webRequest.error);

                downloadData = PlayerPrefs.GetString("LastDataDownloaded", null);
            }
            else
            {
                Debug.Log("DownLoad success");
                Debug.Log("Data: " + webRequest.downloadHandler.text);

                PlayerPrefs.SetString("LastDataDownloaded", webRequest.downloadHandler.text);

                downloadData = webRequest.downloadHandler.text.Substring(0);
            }
        }

        onCompleted(downloadData);
    }
}
