using System.Collections.Generic;
using UnityEngine;

namespace Assets.Script
{
    public class BoardHighlights : MonoBehaviour
    {

        public static BoardHighlights Instance { set; get; }
        public GameObject highlightPrefab;
        private List<GameObject> highlights = new List<GameObject>();

        private void Start()
        {
            Instance = this;
        }

        private GameObject GetHighlightObject()
        {
            GameObject go = highlights.Find((g => !g.activeSelf));
            if (go == null)
            {
                go = Instantiate(highlightPrefab);
                highlights.Add(go);
            }
            return go;
        }

        public void HighLightAllowedMoves(bool[,] moves)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (moves[i, j])
                    {
                        GameObject go = GetHighlightObject();
                        go.SetActive(true);
                        go.transform.position = new Vector3(i + 0.5f - 4, 0.1f, j + 0.5f - 4);
                    }

                }
            }
        }


        public void Hidehighlights()
        {
            GameObject[] wub;
            wub = GameObject.FindGameObjectsWithTag("Highlight");
            foreach (GameObject go in wub)
            {
                Debug.Log(go);
                go.SetActive(false);
            }
        }
    }


}
