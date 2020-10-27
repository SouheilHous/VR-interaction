using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.Prisom
{
    public class BookCoverData
    {
        public struct BookCover
        {
            public string coverUrl;
            public Renderer bookRenderer;
        }

        public static List<BookCover> books = new List<BookCover>();
    }
}