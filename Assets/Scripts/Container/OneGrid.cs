using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Universe
{
    public class OneGrid : MonoBehaviour 
    {
        public Image image;
        public Sprite highlightSprite;
        public Sprite deHighlightSprite;
        public Vector2Int pos;// 记录当前格子是第几排第几格
        public void ChooseThisOneGrid()
        {
            ShowContainer.Instance.ShowGrid(this);
        }
        public void HighlightThis()
        {
            image.sprite = highlightSprite;
        }
        public void DeHighlightThis()
        {
            image.sprite = deHighlightSprite;
        }
    }
}