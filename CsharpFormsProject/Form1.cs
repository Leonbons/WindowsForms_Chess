using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static CsharpFormsProject.Piece;
using static CsharpFormsProject.Tile;

namespace CsharpFormsProject
{
    public partial class Form1 : Form
    {
        public static bool WhiteTurn; 
        public List<PictureBox> tileBoxes = new List<PictureBox>();
        public List<PictureBox> pieceBoxes = new List<PictureBox>();
        
        public Form1()
        {
            InitializeComponent();
            StartGame();
            foreach(PictureBox tile in tileBoxes)
            {
                if(tile.BackColor == Color.SaddleBrown) { tile.AccessibleDescription = "DarkTile"; }
                else if(tile.BackColor == Color.PeachPuff) { tile.AccessibleDescription = "LightTile"; }
            }
            foreach (Control x in this.Controls)
            {
                if (x is PictureBox)
                {
                    x.Click += PictureBoxClick;
                }
            }
        }
        public void PictureBoxClick(object sender, EventArgs e)
        {
            PictureBox selectedBox = sender as PictureBox;
            BoardText.Text = selectedBox.AccessibleName;

            foreach(PictureBox tile in tileBoxes)
            {
                tile.BackColor = Color.Transparent;
                if (tile.BackColor == Color.SaddleBrown) { tile.AccessibleDescription = "DarkTile"; }
                else if (tile.BackColor == Color.PeachPuff) { tile.AccessibleDescription = "LightTile"; }
            }
            foreach(Piece piece in pieces)
            {
                if (selectedBox.Name == piece.Name)
                {
                    //PickedBox.BackColor = Color.Blue;
                    //piece.Selected = true;                    
                    ViewLegalMoves(piece);
                    if(piece.Type.Contains("White") && WhiteTurn == true)
                    {
                        foreach(Piece otherPiece in pieces)
                        {
                            otherPiece.Selected = false;
                        }
                        tileBoxes[piece.TileIndex].BackColor = Color.LightGreen;
                        piece.Selected = true;
                    }
                    else if(piece.Type.Contains("Black") && WhiteTurn == false)
                    {
                        foreach(Piece otherPiece in pieces)
                        {
                            otherPiece.Selected = false;
                        }
                        tileBoxes[piece.TileIndex].BackColor = Color.LightGreen;
                        piece.Selected = false;
                    }
                }
            }
            foreach(PictureBox tileBox in tileBoxes)
            {
                Tile tile = tiles[tileBoxes.IndexOf(tileBox)];
                if(selectedBox.Name == tileBox.Name)
                {
                    foreach(Piece piece in pieces)
                    {
                        if(piece.Selected && tile.IsLegalMove(piece))
                        {
                            int pieceRow = CheckRowNumber(piece.TileIndex) + 1;
                            int pieceColumn = 8 - (piece.TileIndex - pieceRow);
                            int tileRow = CheckRowNumber(tile.Index) + 1;
                            int tileColumn = 8 - (tile.Index - tileRow) / 8;

                            BoardText.Text = $"{piece.Type} tile {ConvertColumn(pieceColumn)}{pieceRow} to {ConvertColumn(tileColumn)}{tileRow}";
                            piece.TileIndex = tile.Index;
                            piece.Moves += 1;
                            piece.Selected = false;
                            WhiteTurn = !WhiteTurn;
                        }
                    }
                }
            }
        }
        public void StartGame()
        {
            tileBoxes.Clear();
            tileBoxes = VerifyBoxes("Tile");
            pieceBoxes = VerifyBoxes("Piece");
            foreach (PictureBox piece in pieceBoxes)
            {
                foreach (PictureBox tile in tileBoxes)
                {
                    int tileIndex = tileBoxes.IndexOf(tile);
                    tiles.Add(new Tile {
                        PosX = tile.Location.X,
                    PosY = tile.Location.Y,
                    Index = tileIndex,
                    Occupied = false });
                    if(piece.Location.X > tile.Location.X + 5 && piece.Location.X < tile.Location.X + 20 && piece.Location.Y > tile.Location.Y + 5 && piece.Location.Y < tile.Location.Y + 20)
                    {
                        pieces.Add(new Piece
                        {
                            Name = piece.Name,
                            Type = piece.AccessibleName,
                            PosX = piece.Location.X,
                            PosY = piece.Location.Y,
                            TileIndex = tileIndex,
                            Selected = false,
                            Moves = 0
                        });
                    }
                }
            }
            timer1.Start();
        }
        List<PictureBox> VerifyBoxes(string AccessName)
        {
            List<PictureBox> PictureBoxes = new List<PictureBox>();

            foreach (Control x in this.Controls)
            {
                PictureBox pic = x as PictureBox;
                if (pic != null && pic.AccessibleName == AccessName)
                {
                    PictureBoxes.Add(pic);
                }
                else if (pic != null && pic.AccessibleDescription == AccessName)
                {
                    PictureBoxes.Add(pic);
                }
            }
            PictureBoxes.OrderBy(x => x.Location.X).ThenBy(y => y.Location.Y); 
            return PictureBoxes;

        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            foreach(Piece piece in pieces)
            {
                piece.UpdatePieceInfo(pieceBoxes);

                if(piece.Type == "WhiteKing" && piece.TileIndex < 0)
                {
                    BoardText.Text = "Black wins";
                    timer1.Stop();
                }
                if(piece.Type == "BlackKing" && piece.TileIndex < 0)
                {
                    BoardText.Text = "White wins";
                    timer1.Stop();
                }
            }
            foreach(Tile tile in tiles)
            {
                tile.Occupied = tile.TileIsOccupied();
            }
        }
        private void ViewLegalMoves(Piece piece)
        {
            List<int> legalMoves = piece.CheckLegalMoves(WhiteTurn);
            string strMoves = "";

            foreach(int move in legalMoves)
            {
                int row = CheckRowNumber(move) + 1;
                int column = 8 - (move - row) / 8;
                string columnLetter = ConvertColumn(column);
                strMoves += $"{columnLetter}{row}";
            }
            BoardText.Text = strMoves;
        }
        private string ConvertColumn(int column)
        {
            string columnLetter = "";
            switch (column)
            {
                case 1:
                    columnLetter = "A";
                    break;
                case 2:
                    columnLetter = "B";
                    break;
                case 3:
                    columnLetter = "C";
                    break;
                case 4:
                    columnLetter = "D";
                    break;
                case 5:
                    columnLetter = "E";
                    break;
                case 6:
                    columnLetter = "F";
                    break;
                case 7:
                    columnLetter = "G";
                    break;
                case 8:
                    columnLetter = "H";
                    break;
            }
            return columnLetter;
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click_1(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox14_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox23_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox41_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox54_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox52_Click(object sender, EventArgs e)
        {

        }

        private void label13_Click(object sender, EventArgs e)
        {

        }

        private void BlackKing(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox68_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox74_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox72_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox70_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox88_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox96_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox29_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox8_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox9_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox15_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox18_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox20_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox19_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox17_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox13_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox11_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox78_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox10_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox16_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox28_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox21_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox12_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox27_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox24_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox38_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox45_Click(object sender, EventArgs e)
        {

        }

        private void BoardText_Click(object sender, EventArgs e)
        {

        }
    }
}
