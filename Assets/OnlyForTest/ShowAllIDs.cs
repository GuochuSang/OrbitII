using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manager;
public class ShowAllIDs : MonoBehaviour {

    public int idCount = 0;
    public List<string> ids;

    void Start()
    {
        StartCoroutine(CheckIDs());
    }
    IEnumerator CheckIDs()
    {
        while (true)
        {
            if (idCount != ID.ids.Count)
            {
                ids = new List<string>();
                foreach (var id in ID.ids)
                {
                    if(id.className != "Space" && id.className != "Chunk")
                    ids.Add(id.ToString());
                }
            }
            yield return new WaitForSeconds(1f);
        }

    }
}
