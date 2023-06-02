using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using static CsharpFormsProject.Form1;
using static CsharpFormsProject.Piece;
using System.Threading.Tasks;


namespace CsharpFormsProject
{
    class Tile
    {
        public int PosX { get; set; }
        public int PosY { get; set; }
        public int Index { get; set; }
        public bool Occupied { get; set; }
        public static List<Tile> tiles = new List<Tile>();

        public bool TileIsOccupied()
        {
            foreach(Piece piece in pieces)
            {
                if (piece.PosX > PosX + 5 && piece.PosX < PosX + 20 &&
                    piece.PosY > PosX + 5 && piece.PosY < PosY + 20)
                {
                    return true;
                }
            }
            return false;
        }
        public bool IsLegalMove(Piece piece)
        {
            List<int> legalMoves = piece.CheckLegalMoves(WhiteTurn);
            if(legalMoves == null) { return false; }

            foreach(int move in legalMoves)
            {
                if (Index == move)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
