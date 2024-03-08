using System.Data.Common;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace IMLab4
{
    public partial class Form1 : Form
    {
        int generation = 0, fieldSize = 30;
        int[,] NewGenerationArray = new int[30, 30];
        //Модель хищник-жертва со стохастическими правилами, хищник - красный, жертва - зелёный
        public Form1()
        {
            InitializeComponent();
            FillField();
        }
        private Label CreateLabel()
        {
            Label label = new Label();
            label.TextAlign = ContentAlignment.MiddleCenter;
            label.Dock = DockStyle.Fill;
            label.Margin = new System.Windows.Forms.Padding(0);
            return label;
        }
        private void FillField()
        {
            for (int column = 0; column < 30; column++)
            {
                for (int row = 0; row < 30; row++)
                {
                    Label TableLabel = CreateLabel();
                    TableLabel.BackColor = Color.White;
                    TableLabel.Click += new EventHandler(CellClick);
                    tableLayoutPanel1.Controls.Add(TableLabel, column, row);
                }
            }
        }
        private void CellClick(object sender, EventArgs e)
        {
            Label cell = sender as Label;
            ChangeCellColor(cell);
        }
        private int PredatorsAroundCell(int column, int row)
        {
            int Predators = 0;
            for (int c = column - 1; c <= column + 1; c++)
            {
                for (int r = row - 1; r <= row + 1; r++)
                {
                    Label l = tableLayoutPanel1.GetControlFromPosition(c, r) as Label;
                    if ((l.BackColor == Color.Red) && (c != 0) && (r != 0))
                        Predators++;
                }
            }
            return Predators;
        }
        private int PreyAroundCell(int column, int row)
        {
            int Prey = 0;
            for (int c = column - 1; c <= column + 1; c++)
            {
                for (int r = row - 1; r <= row + 1; r++)
                {
                    Label l = tableLayoutPanel1.GetControlFromPosition(c, r) as Label;
                    if ((l.BackColor == Color.Green) && (c != 0) && (r != 0))
                        Prey++;
                }
            }
            return Prey;
        }
        private int NoneAroundCell(int column, int row)
        {
            int None = 0;
            for (int c = column - 1; c <= column + 1; c++)
            {
                for (int r = row - 1; r <= row + 1; r++)
                {
                    Label l = tableLayoutPanel1.GetControlFromPosition(c, r) as Label;
                    if ((l.BackColor == Color.White) && (c != 0) && (r != 0))
                        None++;
                }
            }
            return None;
        }
        private int GetCellState(int column, int row)
        {
            Label l = tableLayoutPanel1.GetControlFromPosition(column, row) as Label;
            if (l.BackColor == Color.Red)
                return -1;
            else if (l.BackColor == Color.Green)
                return 1;
            else
                return 0;
        }
        private double RandomCoeff()//случайный коэффициент в диапазоне от 0.25 до -0.25
        {
            Random rand = new Random();
            return ((rand.NextDouble() - 0.5));
        }
        private double Formula(int n)//основная формула, диапазон от 0 до 1, чем больше n тем ближе к 1
        {
            if (n == 0)
                return 0;
            return Math.Round(Convert.ToDouble(1 - 1 / n + RandomCoeff()));
        }
        private int PredictCellState(int column, int row)
        {
            int state = GetCellState(column, row);
            int predators = PredatorsAroundCell(column, row);
            int prey = PreyAroundCell(column, row);
            int none = NoneAroundCell(column, row);
            int diff = prey - predators;
            if (state == 0)
            {
                if (Formula(none) > Formula(prey))
                    return 1;
                else
                    return -1;
            }
            else if (state == -1)
            {
                if (Formula(prey) >= Formula(predators+none))
                    return -1;
                else
                    return 0;
            }
            else if (state == 1)
            {
                if (Formula(none) >= Formula(prey+predators))
                    return 1;
                else
                    return 0;
            }
            return 0;
        }
        private void ChangeCellColor(Label cell)
        {
            if (cell.BackColor == Color.White)
                cell.BackColor = Color.Green;
            else if (cell.BackColor == Color.Green)
                cell.BackColor = Color.Red;
            else
                cell.BackColor = Color.White;
        }
        private void PaintCell(int column, int row, int i)
        {
            Label l = tableLayoutPanel1.GetControlFromPosition(column, row) as Label;
            if (i == -1)
                l.BackColor = Color.Red;
            else if (i == 0)
                l.BackColor = Color.White;
            else
                l.BackColor = Color.Green;
        }
        private void FieldClear()
        {
            for (int column = 0; column < fieldSize; column++)
            {
                for (int row = 0; row < fieldSize; row++)
                {
                    NewGenerationArray[column, row] = 0;
                    PaintCell(column, row, 0);
                }
            }
        }
        private void RandomizeField()
        {
            Random rand = new Random();
            for (int column = 0; column < fieldSize; column++)
            {
                for (int row = 0; row < fieldSize; row++)
                {
                    int r = rand.Next(3) - 1;
                    NewGenerationArray[column, row] = r;
                    PaintCell(column, row, r);
                }
            }
        }
        private void WriteGenerationNumber()
        {
            GenerationLabel.Text = "Поколение: " + Convert.ToString(generation);
        }
        private void NewGeneration()
        {
            for (int column = 1; column < fieldSize - 1; column++)
            {
                for (int row = 1; row < fieldSize - 1; row++)
                {
                    NewGenerationArray[column, row] = PredictCellState(column, row);
                }
            }
            for (int column = 0; column < fieldSize; column++)
            {
                for (int row = 0; row < fieldSize; row++)
                {
                    PaintCell(column, row, NewGenerationArray[column, row]);
                }
            }
            WriteGenerationNumber();
            generation++;
        }
        private void StartStopButton_Click(object sender, EventArgs e)
        {

            if (!timer1.Enabled)
                timer1.Start();
            else
                timer1.Stop();
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            generation = 0;
            WriteGenerationNumber();
            FieldClear();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            NewGeneration();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            RandomizeField();
        }
    }
}
