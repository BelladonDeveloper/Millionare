using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public static class LoadFromSheets
{ 
    public static string googleSheetDocID = "1CWr1c8zjcoBO_POa4q1oppJPdT_1aahRylgNKeMMVaA";
    private static string url =
        "https://docs.google.com/spreadsheets/d/" + googleSheetDocID + "/export?format=csv";

    internal static IEnumerator DownloadData (System.Action<string> onCompleted )
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
                //string versionText = PlayerPrefs.GetString("LastDataDownloadedVersion", null);
                //Debug.Log("Using stale data version: " + versionText);
            }
            else
            {
                Debug.Log("DownLoad success");
                Debug.Log("Data: " + webRequest.downloadHandler.text);

                //Frist term will be preceeded by version number, e.g. "100=English"
                //string versionSection = webRequest.downloadHandler.text.Substring(0, 5);
                //int equalsIndex = versionSection.IndexOf('=');
                //UnityEngine.Assertions.Assert.IsFalse(equalsIndex == -1, "Could not find a '=' at the start of the CSV");

                //string versionText = webRequest.downloadHandler.text.Substring(0, equalsIndex);
                //Debug.Log("Download data version: " + versionText);

                PlayerPrefs.SetString("LastDataDownloaded", webRequest.downloadHandler.text);
                //PlayerPrefs.SetString("LastDataDownloadedVersion", versionText);

                downloadData = webRequest.downloadHandler.text.Substring(0); //equalsIndex + 1);
            }
        }

        onCompleted(downloadData);
    }
}
