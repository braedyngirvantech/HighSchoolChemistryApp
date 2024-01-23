using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HighSchoolChemistryApp
{
    public partial class MainForm : Form
    {
        Graphics grph;
        List<Atom> sidePanelAtoms;
        List<AtomButton> atomButtons;
        string currentAtom;
        int selectX, selectY;
        Font fnt = new Font("Arial", 10);

        Atom[,] atomArray = new Atom[13, 11];
        Atom[,] refAtomArray = new Atom[13, 11];



        //Pentane is max
        //Methyl, ethyl chains
        //Pentyl butanoate/butyl pentanoate max ester

        int bondStage = 0;
        Point sBA = new Point(0, 0);

        Rectangle rect = new Rectangle(0, 0, 39, 39);


        //Base Atoms
        readonly Atom carbon, oxygen, hydrogen, nitrogen, bromine, chlorine, fluorine, iodine, sulfur;
        List<string> atomTypes = new List<string> { "Carbon", "Oxygen", "Nitrogen", "Bromine", "Chlorine", "Fluorine", "Iodine", "Sulfur" };


        public MainForm()
        {
            InitializeComponent();

            grph = CreateGraphics();

            //Initialize Base Atoms
            carbon = new Atom("C", Brushes.White, new Size(10, 9), rect, Brushes.Black, 4);
            oxygen = new Atom("O", Brushes.White, new Size(10, 9), rect, Brushes.Red, 2);
            hydrogen = new Atom("H", Brushes.Black, new Size(5, 4), new Rectangle(0, 0, 29, 29), Brushes.White, 1);
            nitrogen = new Atom("N", Brushes.White, new Size(10, 9), rect, Brushes.Blue, 3);
            bromine = new Atom("Br", Brushes.White, new Size(7, 9), rect, Brushes.DarkRed, 1);
            chlorine = new Atom("Cl", Brushes.Black, new Size(7, 9), rect, Brushes.Gold, 1);
            fluorine = new Atom("F", Brushes.Black, new Size(11, 9), rect, Brushes.LimeGreen, 1);
            iodine = new Atom("I", Brushes.White, new Size(15, 9), rect, Brushes.DarkViolet, 1);
            sulfur = new Atom("S", Brushes.Black, new Size(10, 9), rect, Brushes.Yellow, 2);






            sidePanelAtoms = new List<Atom> { carbon.Create(5, 5), oxygen.Create(55,5),
                nitrogen.Create(5,55), bromine.Create(55,55),
                chlorine.Create(5,105), fluorine.Create(55,105),
                iodine.Create(5,155), sulfur.Create(55,155)};
            Size size = new Size(50, 50);
            atomButtons = new List<AtomButton> {
                new AtomButton(new Point(0,0), size, "Carbon"),
                new AtomButton(new Point(50,0), size, "Oxygen"),
                new AtomButton(new Point(0,50), size, "Nitrogen"),
                new AtomButton(new Point(50,50), size, "Bromine"),
                new AtomButton(new Point(0,100), size, "Chlorine"),
                new AtomButton(new Point(50,100), size, "Fluorine"),
                new AtomButton(new Point(0,150), size, "Iodine"),
                new AtomButton(new Point(50,150), size, "Sulfur"),
                new AtomButton(new Point(0, 200), size, "Eraser"),
                new AtomButton(new Point(50, 200), size, "Bond"),
                new AtomButton(new Point(50, 250), size, "BondEraser")};
            currentAtom = "Carbon";






            Shown += OnShow;
        }

        void OnShow(object sender, EventArgs e)
        {
            grph.Clear(Color.White);
            DrawPanel();
        }

        void DrawPanel()
        {
            //Clears Panel
            grph.FillRectangle(Brushes.White, 0, 0, 100, 600);

            //Draws Selected Outline
            grph.FillRectangle(Brushes.LightBlue, selectX, selectY, 50, 50);
            grph.DrawRectangle(Pens.Blue, selectX, selectY, 49, 49);

            //Draws each atom
            foreach (Atom a in sidePanelAtoms)
            {
                a.Draw(grph);
            }

            //Eraser
            grph.DrawLine(Pens.Red, 20, 210, 30, 215);
            grph.DrawLine(Pens.Red, 20, 235, 30, 240);
            grph.DrawLine(Pens.Red, 20, 210, 20, 235);
            grph.DrawLine(Pens.Red, 30, 215, 30, 240);

            //Bond
            grph.DrawLine(Pens.Black, 61, 215, 85, 239);
            grph.DrawLine(Pens.Black, 60, 215, 85, 240);
            grph.DrawLine(Pens.Black, 60, 216, 84, 240);


            //Clear All
            grph.DrawString("Erase\n   All", fnt, Brushes.Black, new Point(4, 310));
            grph.DrawRectangle(Pens.Black, 0, 300, 49, 49);

            grph.DrawRectangle(Pens.Black, 0, 0, 99, 249);
        }

        void ClearDrawingSpace()
        {
            grph.FillRectangle(Brushes.White, 150, 0, 650, 600);
        }

        void DrawAtoms(Action<int, int, bool, bool> act)
        {
            for (int x = 0; x < 13; x++)
            {
                for (int y = 0; y < 11; y++)
                {
                    if (atomArray[x, y] != null)
                    {
                        atomArray[x, y].Draw(grph);
                        if (act != null)
                            act(x, y, false, true);
                    }
                }
            }
        }

        void DrawsAvailable(int x, int y, bool placingBonds, bool isItTrue)
        {

            if (!atomArray[x, y].MaxBonds())
            {
                if (x > 0 && (atomArray[x - 1, y] == null) == isItTrue)
                {
                    if (placingBonds)
                    {
                         if (CheckAvailable(x - 1, y) && atomArray[x,y].leftBond < 3)
                            grph.FillRectangle(Brushes.Yellow, x * 50 + 100, y * 50, 50, 50);
                    }
                    else
                        grph.FillRectangle(Brushes.Yellow, x * 50 + 100, y * 50, 50, 50);
                }
                if (x < 12 && (atomArray[x + 1, y] == null) == isItTrue)
                {
                    if (placingBonds)
                    {
                        if (CheckAvailable(x + 1, y) && atomArray[x, y].rightBond < 3)
                            grph.FillRectangle(Brushes.Yellow, x * 50 + 200, y * 50, 50, 50);
                    }
                    else
                        grph.FillRectangle(Brushes.Yellow, x * 50 + 200, y * 50, 50, 50);
                }
                if (y > 0 && (atomArray[x, y - 1] == null) == isItTrue)
                {
                    if (placingBonds)
                    {
                        if (CheckAvailable(x, y - 1) && atomArray[x, y].upBond < 3)
                            grph.FillRectangle(Brushes.Yellow, x * 50 + 150, y * 50 - 50, 50, 50);
                    }
                    else
                        grph.FillRectangle(Brushes.Yellow, x * 50 + 150, y * 50 - 50, 50, 50);
                }
                if (y < 10 && (atomArray[x, y + 1] == null) == isItTrue)
                {
                    if (placingBonds)
                    {
                        if (CheckAvailable(x, y + 1) && atomArray[x, y].downBond < 3)
                            grph.FillRectangle(Brushes.Yellow, x * 50 + 150, y * 50 + 50, 50, 50);
                    }
                    else
                        grph.FillRectangle(Brushes.Yellow, x * 50 + 150, y * 50 + 50, 50, 50);
                }
            }
        }

        bool CheckAvailable(int x, int y)
        {
            return !atomArray[x, y].MaxBonds();
        }

        void DrawSelected()
        {
            grph.FillRectangle(Brushes.Red, sBA.X * 50 + 150, sBA.Y * 50, 50, 50);
        }

        void DrawBonds()
        {
            for (int a = 0; a < 13; a++)
            {
                for (int b = 0; b < 11; b++)
                {
                    if (atomArray[a, b] != null)
                    {
                        switch (atomArray[a, b].rightBond)
                        {
                            case 1:
                                {
                                    grph.DrawLine(Pens.Black, 194 + 50 * a, 25 + 50 * b, 204 + 50 * a, 25 + 50 * b);
                                    break;
                                }
                            case 2:
                                {
                                    grph.DrawLine(Pens.Black, 194 + 50 * a, 23 + 50 * b, 204 + 50 * a, 23 + 50 * b);
                                    grph.DrawLine(Pens.Black, 194 + 50 * a, 27 + 50 * b, 204 + 50 * a, 27 + 50 * b);
                                    break;
                                }
                            case 3:
                                {
                                    grph.DrawLine(Pens.Black, 194 + 50 * a, 22 + 50 * b, 204 + 50 * a, 22 + 50 * b);
                                    grph.DrawLine(Pens.Black, 194 + 50 * a, 25 + 50 * b, 204 + 50 * a, 25 + 50 * b);
                                    grph.DrawLine(Pens.Black, 194 + 50 * a, 28 + 50 * b, 204 + 50 * a, 28 + 50 * b);
                                    break;
                                }
                        }

                        switch (atomArray[a, b].downBond)
                        {
                            case 1:
                                {
                                    grph.DrawLine(Pens.Black, 175 + 50 * a, 44 + 50 * b, 175 + 50 * a, 54 + 50 * b);
                                    break;
                                }
                            case 2:
                                {
                                    grph.DrawLine(Pens.Black, 173 + 50 * a, 44 + 50 * b, 173 + 50 * a, 54 + 50 * b);
                                    grph.DrawLine(Pens.Black, 177 + 50 * a, 44 + 50 * b, 177 + 50 * a, 54 + 50 * b);
                                    break;
                                }
                            case 3:
                                {
                                    grph.DrawLine(Pens.Black, 172 + 50 * a, 44 + 50 * b, 172 + 50 * a, 54 + 50 * b);
                                    grph.DrawLine(Pens.Black, 175 + 50 * a, 44 + 50 * b, 175 + 50 * a, 54 + 50 * b);
                                    grph.DrawLine(Pens.Black, 178 + 50 * a, 44 + 50 * b, 178 + 50 * a, 54 + 50 * b);
                                    break;
                                }
                        }
                    }
                }
            }
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            //Get mouse position for button click checking and atom placing.
            Point mousePos = this.PointToClient(Cursor.Position);
            int x = mousePos.X - mousePos.X % 50;
            int y = mousePos.Y - mousePos.Y % 50;

            //Position mouse repersented in array
            int indexX = x / 50 - 3;
            int indexY = y / 50;

            //If clicking on drawing board
            if (mousePos.X >= 150 && mousePos.X <= 800 && mousePos.Y >= 0 && mousePos.Y <= 550)
            {
                //If placing an atom (Not Erasing, Erasing bonds or adding Bonds)
                if (currentAtom != "Eraser" && currentAtom != "Bond" && currentAtom != "BondEraser")
                {
                    if (atomArray[indexX, indexY] == null)
                    {
                        if (indexY > 0 && atomArray[indexX, indexY - 1] != null && !atomArray[indexX, indexY - 1].MaxBonds())
                        {
                            atomArray[indexX, indexY] = sidePanelAtoms[atomTypes.FindIndex(i => i == currentAtom)].Create(x + 5, y + 5);
                            atomArray[indexX, indexY].Draw(grph);
                            atomArray[indexX, indexY].AddBond(1);
                            atomArray[indexX, indexY - 1].AddBond(3);
                        }
                        else if (indexY < 10 && atomArray[indexX, indexY + 1] != null && !atomArray[indexX, indexY + 1].MaxBonds())
                        {
                            atomArray[indexX, indexY] = sidePanelAtoms[atomTypes.FindIndex(i => i == currentAtom)].Create(x + 5, y + 5);
                            atomArray[indexX, indexY].Draw(grph);
                            atomArray[indexX, indexY].AddBond(3);
                            atomArray[indexX, indexY + 1].AddBond(1);
                        }
                        else if (indexX > 0 && atomArray[indexX - 1, indexY] != null && !atomArray[indexX - 1, indexY].MaxBonds())
                        {
                            atomArray[indexX, indexY] = sidePanelAtoms[atomTypes.FindIndex(i => i == currentAtom)].Create(x + 5, y + 5);
                            atomArray[indexX, indexY].Draw(grph);
                            atomArray[indexX, indexY].AddBond(0);
                            atomArray[indexX - 1, indexY].AddBond(2);
                        }
                        else if (indexX < 12 && atomArray[indexX + 1, indexY] != null && !atomArray[indexX + 1, indexY].MaxBonds())
                        {
                            atomArray[indexX, indexY] = sidePanelAtoms[atomTypes.FindIndex(i => i == currentAtom)].Create(x + 5, y + 5);
                            atomArray[indexX, indexY].Draw(grph);
                            atomArray[indexX, indexY].AddBond(2);
                            atomArray[indexX + 1, indexY].AddBond(0);
                        }
                        else if (AtomArrayEmpty())
                        {
                            atomArray[indexX, indexY] = sidePanelAtoms[atomTypes.FindIndex(i => i == currentAtom)].Create(x + 5, y + 5);
                        }

                        //Refresh drawing space
                        ClearDrawingSpace();
                        DrawAtoms(DrawsAvailable);
                        DrawBonds();
                    }
                }
                //If the Eraser is selected and the bond clicked is only bonded to one 
                else if (currentAtom == "Eraser" && atomArray[indexX, indexY].SingleAtomBonded())
                {
                    //Remove atom
                    atomArray[indexX, indexY] = null;
                    //For each direction Remove bonds
                    if (atomArray[indexX - 1, indexY] != null)
                        atomArray[indexX - 1, indexY].rightBond = 0;
                    if (atomArray[indexX + 1, indexY] != null)
                        atomArray[indexX + 1, indexY].leftBond = 0;
                    if (atomArray[indexX, indexY - 1] != null)
                        atomArray[indexX, indexY - 1].downBond = 0;
                    if (atomArray[indexX, indexY + 1] != null)
                        atomArray[indexX, indexY + 1].upBond = 0;

                    //Refresh drawing space
                    ClearDrawingSpace();
                    DrawAtoms(DrawsAvailable);
                    DrawBonds();
                }
                //If Bond tool is selected
                else if (currentAtom == "Bond")
                {
                    //If the atom clicked on exists
                    if (atomArray[indexX, indexY] != null)
                    {
                        //If you are sellecting an atom to add a bond to, it exists and the atoms around exist but dont have full bonds
                        if (
                            bondStage == 0 && !atomArray[indexX, indexY].MaxBonds() &&
                            !(
                                indexX > 0 && (atomArray[indexX - 1, indexY] == null || atomArray[indexX - 1, indexY].MaxBonds())
                                && indexX < 12 && (atomArray[indexX + 1, indexY] == null || atomArray[indexX + 1, indexY].MaxBonds())
                                && indexY > 0 && (atomArray[indexX, indexY - 1] == null || atomArray[indexX, indexY - 1].MaxBonds())
                                && indexY < 11 && (atomArray[indexX, indexY + 1] == null || atomArray[indexX, indexY + 1].MaxBonds())
                            )
                            )
                        {
                            //Set the stage so that you are selecting the other atpm to add the bond to
                            bondStage = 1;
                            sBA.X = indexX;
                            sBA.Y = indexY;
                            
                            //
                            ClearDrawingSpace();
                            //Highlight the available atoms you can bond to
                            DrawsAvailable(sBA.X, sBA.Y, true, false);
                            //Highlights the first atom clicked
                            DrawSelected();
                            //Draws atoms and bonds
                            DrawAtoms(null);
                            DrawBonds();
                        }
                        //If you are selecting second atom to bond to and it doesnt have max bonds
                        else if (bondStage == 1 && !atomArray[indexX, indexY].MaxBonds())
                        {
                            //Reset the bondstage
                            bondStage = 0;

                            if (indexX == sBA.X - 1)
                            {
                                atomArray[indexX, indexY].rightBond++;
                                atomArray[sBA.X, sBA.Y].leftBond++;
                            }
                            else if (indexX == sBA.X + 1)
                            {
                                atomArray[indexX, indexY].leftBond++;
                                atomArray[sBA.X, sBA.Y].rightBond++;
                            }
                            else if (indexY == sBA.Y - 1)
                            {
                                atomArray[indexX, indexY].downBond++;
                                atomArray[sBA.X, sBA.Y].upBond++;
                            }
                            else if (indexY == sBA.Y + 1)
                            {
                                atomArray[indexX, indexY].upBond++;
                                atomArray[sBA.X, sBA.Y].downBond++;
                            }
                            ClearDrawingSpace();
                            DrawAtoms(DrawsAvailable);
                            DrawBonds();
                        }
                    }
                    else
                    {
                        bondStage = 0;
                        ClearDrawingSpace();
                        DrawAtoms(DrawsAvailable);
                        DrawBonds();
                    }
                }
                //If bond eraser is selected
                else if (currentAtom == "BondEraser")
                {
                    //If the atom exists
                    if (atomArray[indexX, indexY] != null)
                    {
                        atomArray[indexX, indexY].ResetBonds();
                        if (atomArray[indexX - 1, indexY] != null)
                            atomArray[indexX - 1, indexY].rightBond = 1;
                        if (atomArray[indexX + 1, indexY] != null)
                            atomArray[indexX + 1, indexY].leftBond = 1;
                        if (atomArray[indexX, indexY - 1] != null)
                            atomArray[indexX, indexY - 1].downBond = 1;
                        if (atomArray[indexX, indexY + 1] != null)
                            atomArray[indexX, indexY - 1].upBond = 1;
                        ClearDrawingSpace();
                        DrawAtoms(DrawsAvailable);
                        DrawBonds();
                    }
                }
            }
            else
            {
                bondStage = 0;
                sBA.X = 0;
                sBA.Y = 0;

                //If clicking on the panel, change what it is set to.
                string str = currentAtom;
                foreach (AtomButton a in atomButtons)
                {
                    currentAtom = a.Click(mousePos, currentAtom);
                    if (str != currentAtom)
                    {
                        selectX = a.point.X;
                        selectY = a.point.Y;
                        DrawPanel();
                        ClearDrawingSpace();
                        DrawAtoms(DrawsAvailable);
                        DrawBonds();
                        break;
                    }
                }

                //Erase All Button
                if (x == 0 && y == 300)
                {
                    atomArray = new Atom[13, 11];
                    grph.FillRectangle(Brushes.White, 100, 0, 700, 600);
                    currentAtom = "Carbon";
                    selectX = selectY = 0;
                    DrawPanel();
                }
            }
        }

        bool AtomArrayEmpty()
        {
            for (int a = 0; a < 13; a++)
            {
                for (int b = 0; b < 11; b++)
                {
                    if (atomArray[a, b] != null)
                        return false;
                }
            }
            return true;
        }
    }

    class Atom
    {
        string text; Brush textBrush; Point textPosition; Size textOffset;
        public Rectangle rect; Brush fill;
        Font font = new Font("Arial", 14);
        int bondMax;
        public int leftBond, upBond, rightBond, downBond;
        public int bonds = 0;

        public Atom(string setText, Brush setTextBrush, Size setTextOffset, Rectangle setRect, Brush setFill, int setBondMax)
        {
            text = setText;
            textBrush = setTextBrush;
            textOffset = setTextOffset;
            rect = setRect;
            fill = setFill;
            bondMax = setBondMax;
        }

        public Atom Create(int setX, int setY)
        {
            Atom atm = new Atom(text, textBrush, textOffset, rect, fill, bondMax);
            atm.rect.X = setX;
            atm.rect.Y = setY;
            atm.textPosition.X = setX + textOffset.Width;
            atm.textPosition.Y = setY + textOffset.Height;
            return atm;
        }

        public void AddBond(int direction)
        {
            if (direction == 0) leftBond++;
            else if (direction == 1) upBond++;
            else if (direction == 2) rightBond++;
            else if (direction == 3) downBond++;
        }

        public void ResetBonds()
        {
            if (upBond > 0) upBond = 1;
            if (downBond > 0) downBond = 1;
            if (leftBond > 0) leftBond = 1;
            if (rightBond > 0) rightBond = 1;
        }

        public bool SingleAtomBonded()
        {
            //Returns true  
            return (
                ((upBond == 0 && downBond == 0) && (leftBond == 0 || rightBond == 0)) ||
                ((leftBond == 0 && rightBond == 0) && (upBond == 0 || downBond == 0))
                );
        }

        public bool MaxBonds()
        {
            //Returns true if the atom has the max amount of bonds
            return (leftBond + upBond + rightBond + downBond == bondMax);
        }

        public void Draw(Graphics grph)
        {
            //Draws the circle for the atom, then the symobl.
            grph.FillEllipse(fill, rect);
            grph.DrawEllipse(Pens.Black, rect);
            grph.DrawString(text, font, textBrush, textPosition);
        }
    }

    class AtomButton
    {
        public Point point; Size size; string atom;

        public AtomButton(Point setPoint, Size setSize, string setAtom)
        {
            //Sets position size and atom.
            point = setPoint;
            size = setSize;
            atom = setAtom;
        }

        public string Click(Point mouse, string currentAtom)
        {
            //If the mouse is between the bounds of this button, return this atom, otherwise return the current atom.
            if ((mouse.X >= point.X && mouse.X <= point.X + size.Width) && (mouse.Y >= point.Y && mouse.Y <= point.Y + size.Height))
                return atom;
            return currentAtom;
        }
    }
}