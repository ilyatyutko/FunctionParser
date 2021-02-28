using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Graphics
{
    public partial class Form1 : Form
    {
        List<PrecisePoint> PointArr = new List<PrecisePoint>();
        //this array holds all the points that should bu visualised
        public Form1()
        {
            InitializeComponent();

            InitializeGrid();

            toolStripComboBox2.SelectedIndex = 0;
            //this sets default value of chart painting to line

            chart1.Series.Clear();
            //delete the default line
        }
        public void InitializeGrid()
        {
            {
                var column0 = new DataGridViewColumn();
                column0.HeaderText = "n";
                column0.Width = 50;
                column0.ReadOnly = false;
                column0.Name = "table_n";
                column0.CellTemplate = new DataGridViewTextBoxCell();
                column0.DefaultCellStyle.Font = new Font("Arial", 14, GraphicsUnit.Pixel);

                var column1 = new DataGridViewColumn();
                column1.HeaderText = "a";
                column1.Width = 50;
                column1.ReadOnly = false;
                column1.Name = "table_a";
                column1.CellTemplate = new DataGridViewTextBoxCell();
                column1.DefaultCellStyle.Font = new Font("Arial", 14, GraphicsUnit.Pixel);

                var column2 = new DataGridViewColumn();
                column2.HeaderText = "b";
                column2.Name = "table_b";
                column2.Width = 50;
                column2.ReadOnly = false;
                column2.CellTemplate = new DataGridViewTextBoxCell();
                column2.DefaultCellStyle.Font = new Font("Arial", 14, GraphicsUnit.Pixel);

                var column3 = new DataGridViewColumn();
                column3.HeaderText = "function";
                column3.Name = "table_func";
                column3.Width = 450;
                column3.ReadOnly = false;
                column3.CellTemplate = new DataGridViewTextBoxCell();
                column3.DefaultCellStyle.Font = new Font("Arial", 14, GraphicsUnit.Pixel);

                dataGridView1.Columns.Add(column0);
                dataGridView1.Columns.Add(column1);
                dataGridView1.Columns.Add(column2);
                dataGridView1.Columns.Add(column3);

                dataGridView1.AllowUserToAddRows = true;
            }
            {
                var column10 = new DataGridViewColumn();
                column10.HeaderText = "x";
                column10.Width = 83;
                column10.ReadOnly = false;
                column10.Name = "table_x";
                column10.CellTemplate = new DataGridViewTextBoxCell();

                var column11 = new DataGridViewColumn();
                column11.HeaderText = "y";
                column11.Width = 83;
                column11.ReadOnly = false;
                column11.Name = "table_y";
                column11.CellTemplate = new DataGridViewTextBoxCell();

                dataGridView2.Columns.Add(column10);
                dataGridView2.Columns.Add(column11);

                dataGridView2.AllowUserToAddRows = false;
            }
        }
        //create columns for both grids
        

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
                try
                {
                    var fileContent = string.Empty;
                    var filePath = string.Empty;
                    using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                    {
                        saveFileDialog.InitialDirectory = "c:\\";
                        saveFileDialog.Filter = "values files (*.vls)|*.vls";
                        saveFileDialog.FilterIndex = 2;
                        saveFileDialog.RestoreDirectory = true;

                        if (saveFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            filePath = saveFileDialog.FileName;

                            string[] strArr = new string[dataGridView1.Rows.Count * dataGridView1.Columns.Count];
                            int writePos = 0;
                            for (int i = 0; i < dataGridView1.Rows.Count; ++i)
                            {
                                strArr[writePos++] = dataGridView1[0, i].Value as string;
                                strArr[writePos++] = dataGridView1[1, i].Value as string;
                                strArr[writePos++] = dataGridView1[2, i].Value as string;
                                strArr[writePos++] = dataGridView1[3, i].Value as string;
                            }
                            System.IO.File.WriteAllText(filePath, String.Join("|", strArr));
                        }

                    }
                }
                catch
                {
                    MessageBox.Show("Файл Не удалось сохранить - проверьте, что другие приложения не используют его");
                }
        }
        //saves the function table

        private void openFromToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var fileContent = string.Empty;
            var filePath = string.Empty;
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "values files (*.vls)|*.vls";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    filePath = openFileDialog.FileName;
                    var fileStream = openFileDialog.OpenFile();

                    StreamReader reader = new StreamReader(fileStream);
                    fileContent = reader.ReadToEnd();
                    var cuttedValues = fileContent.Split('|');

                    dataGridView1.Rows.Clear();
                    dataGridView1.Refresh();

                    for (int i = 0; i < cuttedValues.Length; i += 4)
                        dataGridView1.Rows.Add(cuttedValues[i], cuttedValues[i + 1], cuttedValues[i + 2], cuttedValues[i + 3]);
                }
            }
        }
        //reads the function table from the given file

        private void calculateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PointArr.Clear();
            try{
                var i = dataGridView1.CurrentRow.Index;
            }catch{
                MessageBox.Show("Выберите строку для вычислений");
                return;
            }
            //checks the line was chosen in the dataGrid

            Function func;
            try{
                func = new Function(dataGridView1[3, dataGridView1.CurrentRow.Index].Value as string);
            }catch (Exception ex){
                MessageBox.Show("Ошибка парсинга функции :" + ex.Message);
                return;
            }
            //checks the function can be correctly parsed
            
            double left, right;
            try{
                if (dataGridView1[1, dataGridView1.CurrentRow.Index].Value as string == ""
                        || dataGridView1[2, dataGridView1.CurrentRow.Index].Value as string == "")
                    throw new Exception();
                left = Convert.ToDouble((dataGridView1[1, dataGridView1.CurrentRow.Index].Value as string).Replace(".", ","));
                right = Convert.ToDouble((dataGridView1[2, dataGridView1.CurrentRow.Index].Value as string).Replace(".", ","));
            }catch{
                MessageBox.Show("Ошибка определения границ");
                return;
            }
            //checks the edges are correct
            int n;
            try{
                if (dataGridView1[0, dataGridView1.CurrentRow.Index].Value as string == "")
                    throw new Exception();
                n = Convert.ToInt32(dataGridView1[0, dataGridView1.CurrentRow.Index].Value as string);
            }catch{
                MessageBox.Show("Ошибка определения шага");
                return;
            }
            //checks the accuracy is correct

            for (int i = 0; i <= n; ++i){
                var pointX = left + (right - left) * i / n;
                var pointY = func.Execute(pointX);
                PointArr.Add(new PrecisePoint(pointX, pointY));              
            }
            //makes calculatin and write it into PointsArray
            visualise();
        }
        //read all data from selected row in datagrid and figure out function on the given span

        private void visualise()
        {
            dataGridView2.Rows.Clear();
            dataGridView2.Refresh();
            chart1.Series.Clear();

            var series1 = new System.Windows.Forms.DataVisualization.Charting.Series
            {
                Name = "",
                Color = System.Drawing.Color.Black,
                BorderWidth = 3,
                IsVisibleInLegend = false,
                IsXValueIndexed = true
            };
            switch (toolStripComboBox2.SelectedItem.ToString())
            {
                case "Line":
                    series1.ChartType = SeriesChartType.Line;
                    break;
                case "Spline":
                    series1.ChartType = SeriesChartType.Spline;
                    break;
                case "Point":
                    series1.ChartType = SeriesChartType.Point;
                    break;
                case "Step Line":
                    series1.ChartType = SeriesChartType.StepLine;
                    break;
                default:
                    series1.ChartType = SeriesChartType.Line;
                    break;
            }
            this.chart1.Series.Add(series1);

            double yMin = 0, yMax = 0;
            PointArr.Sort();

            for (int i = 0; i < PointArr.Count; ++i)
            {
                var pointX = PointArr[i].x;
                var pointY = PointArr[i].y;
                dataGridView2.Rows.Add(pointX.ToString("F3"), pointY.ToString("F3"));
                if (!double.IsInfinity(pointY) || !double.IsNaN(pointY))
                {
                    if (pointY > Int64.MaxValue)
                        pointY = Int64.MaxValue;
                    if (pointY < Int64.MinValue)
                        pointY = Int64.MinValue;
                    series1.Points.AddXY(pointX, pointY);
                    yMax = (yMax > pointY) ? yMax : pointY;
                    yMin = (yMin < pointY) ? yMin : pointY;
                }

            }
            yMax = yMax + (yMax - yMin) * 0.2 + 1;
            yMin = yMin - (yMax - yMin) * 0.2 - 1;


            chart1.ChartAreas[0].AxisY.Minimum = yMin;
            chart1.ChartAreas[0].AxisY2.Minimum = yMin;
            chart1.ChartAreas[0].AxisY.Maximum = yMax;
            chart1.ChartAreas[0].AxisY2.Maximum = yMax;
            chart1.ChartAreas[0].RecalculateAxesScale();
            chart1.Invalidate();
        }
        //takes all points from PointArr and visualise them

        private void clearAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            dataGridView2.Rows.Clear();
            chart1.Series.Clear();
            PointArr.Clear();
        }
        //delete all datas

        private void saveResultsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (PointArr.Count == 0)
            {
                MessageBox.Show("Нечего сохранять");
                return;
            }
            try
            {
                var fileContent = string.Empty;
                var filePath = string.Empty;
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.InitialDirectory = "c:\\";
                    saveFileDialog.Filter = "Comma-separated values (*.csv)|*.csv";
                    saveFileDialog.FilterIndex = 2;
                    saveFileDialog.RestoreDirectory = true;

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        filePath = saveFileDialog.FileName;

                        string[] strArr = new string[PointArr.Count + 1];
                        int writePos = 0;
                        strArr[writePos++] = "x; y";
                        foreach (PrecisePoint point in PointArr)
                        {
                            strArr[writePos++] = point.x.ToString() + ";" + point.y.ToString();
                        }
                        System.IO.File.WriteAllText(filePath, String.Join("\n", strArr));
                    }

                }
            }
            catch
            {
                MessageBox.Show("Файл Не удалось сохранить - проверьте, что другие приложения не используют его");
            }
        }
        //save figured out points and save them to the file

        private void loadDatasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PointArr.Clear();
            chart1.Series.Clear();
            dataGridView2.Rows.Clear();
            
            var fileContent = string.Empty;
            var filePath = string.Empty;
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "Comma-separated values (*.csv)|*.csv";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    filePath = openFileDialog.FileName;
                    var fileStream = openFileDialog.OpenFile();

                    StreamReader reader = new StreamReader(fileStream);
                    fileContent = reader.ReadToEnd();
                    var cuttedValues = fileContent.Split('\n');

                    dataGridView1.Rows.Clear();
                    dataGridView1.Refresh();

                    for (int i = 1; i < cuttedValues.Length; i += 2)
                    {
                        try
                        {
                            var pointStr = cuttedValues[i].Split(';');
                            PrecisePoint point = new PrecisePoint(Convert.ToDouble(pointStr[0]), Convert.ToDouble(pointStr[1]));
                            PointArr.Add(point);
                        }
                        catch { }
                        
                    }
                    visualise();
                }
            }
        }
        //load points from the file and visualise them
    }

    public class PrecisePoint : IComparable<PrecisePoint>
    {
        public double x;
        public double y;

        public PrecisePoint(double _x, double _y) {
            x = _x;
            y = _y;
        }
        public int CompareTo(PrecisePoint other)
        {
            if (this.x < other.x)
                return -1;
            else if (this.x > other.x)
                return 1;
            else
                return 0;
        }
    }
    //this class allow to hold not just (int,int) Point
    //but (double, double) 

    public class FormulaObject
    { 
        public virtual double Execute()
        {
            throw new Exception("This can not be executed, basic holder class");
        }
    }
    //this class is basic class fow formula meaning units 
    public class Const : FormulaObject
    {
        public double value;
        public Const(double _value)
        {
            value = _value;
        }
        public override double Execute()
        {
            return value;
        }
    }
    //holds a const inside and return it's value
    public class Operation : FormulaObject
    {
        public static double x;

        public int index;
        //global index of this operand type in dictionary

        public bool wasActivated;
        //holds if this operator already got it right and lefy operands

        public FormulaObject left;
        public FormulaObject right;
        //holds operands and alwasys both
        //however if exactly unary function - meaningfull just right
        //the left will be empty

        public Operation(string name, FormulaObject _left, FormulaObject _right)
        {
            if (!OperationDictionary.ContainsKey(name))
                throw new Exception("Unacceptable formula : " + name);
            index = OperationDictionary[name];
            left = _left;
            right = _right;
            wasActivated = false;
        }
        public override double Execute()
        {
            switch (index)
            {
                case 1:
                    return Math.Cos(right.Execute());
                case 2:
                    return Math.Sin(right.Execute());
                case 3:
                    return Math.Asin(right.Execute());
                case 4:
                    return Math.Acos(right.Execute());
                case 5:
                    return Math.Sqrt(right.Execute());
                case 6:
                    return left.Execute() + right.Execute();
                case 7:
                    return left.Execute() - right.Execute();
                case 8:
                    return left.Execute() * right.Execute();
                case 9:
                    return left.Execute() / right.Execute();
                case 10:
                    return Math.Pow(left.Execute(), right.Execute());
                case 11:
                    return -1 * right.Execute();
                case 12:
                    return Math.Abs(right.Execute());
                case 13:
                    return x;
                case 14:
                    return Math.E;
                case 15:
                    return Math.PI;
                default:
                    throw new Exception("Not Assigned Formula was executed");

            }
            
        }
        //returns the result of computing this operand with the given right and left operands
        public int Priority()
        {
            return OperationPriority[index];
        }
        //returns the operator calculating priority to the function parser
        //(based on static Dictionary)

        public int ArgumentsCount()
        {
            return OperationArgumentsCount[index];
        }
        //returns the operator arguments count to the function parser
        //(based on static Dictionary)

        private static Dictionary<string, int> OperationDictionary = new Dictionary<string, int>(){
            {"cos",  1 },
            {"sin",  2 },
            {"asin", 3 },
            {"acos", 4 },
            {"sqrt", 5 },
            {"log", 16 },
            {"ln", 17 },
            {"abs",  12 },
            {"opposite", 11 },
            {"^",    10 },
            {"*",    8 },
            {"/",    9 },
            {"+",    6 },
            {"-",    7 },
            {"x",    13 },
            {"e",    14 },
            {"pi",    15 }
        };
        private static Dictionary<int, int> OperationPriority = new Dictionary<int, int>(){
            {1, 9},
            {2, 9},
            {3, 9},
            {4, 9},
            {5, 9},
            {16, 9},
            {17, 9},
            {12, 5},
            {11, 5},
            {10, 4},
            {8, 3},
            {9, 3},
            {7, 2},
            {6, 2},
            {13, 1},
            {14, 1},
            {15, 1},
        };
        private static Dictionary<int, int> OperationArgumentsCount = new Dictionary<int, int>(){
            {1, 1},
            {2, 1},
            {3, 1},
            {4, 1},
            {5, 1},
            {6, 2},
            {7, 2},
            {8, 2},
            {9, 2},
            {10, 2},
            {13, 0},
            {12, 1},
            {11, 1},
            {14, 0},
            {15, 0},
            {16, 1},
            {17, 1},

        };
        public static bool CharIsMethodPart(char ch)
        {
            return Char.IsLetter(ch);
        }
        //in this case method is operation called by name
        //sqrt or abs for example
        public static bool CharIsOperatorPart(char ch)
        {
            return ch == '+' || ch == '-' || ch == '*' || ch == '/' || ch == '^';
        }
        //in this case operator is operator which declared as symbol
        //+ or ^ for example
    }
    //holds an Operation (binary or unary) and it's operands
    //calculating means calculating the neccessary operands 
    //and returning the result of the exactly operation

    public class Function
    {
        FormulaObject root = new FormulaObject();
        //this class reshape the text of formula to
        //it's binary tree of computing
        //so any node (which roly is being playing by FormulaObject - it's just node container)
        //contains either operation and forks (operands) or constant
        //so we need FormulaObject just for these purpose - unified Container

        public Function(string formula)
        {
            if (formula == null)
                throw new Exception("Empty Formula");
            formula = formula.ToLower().Trim(' ');
            //there the formula is being prePared for parsing and transforms onto simplified form

            List<FormulaObject> parsedFormula = new List<FormulaObject>();
            parsedFormula = new List<FormulaObject>();
            //this list will hold the sequence of recognized FormulaObjects

            int parenthesesCounter = 0;
            //if we need to properly parse formula we have to notify if parentheses are positioned correctly
            //therefore keep the count of still opened parentheses

            for (int i = 0; i < formula.Length;)
                //and going through the text formula
            {
                if (formula[i] == '(')
                //if we meet the Opening Parenthese - we are looking for it's end
                //if there is no end - "parenthesesCounter" will let us know
                {
                    parenthesesCounter++;
                    string inParenteses = "";
                    //here will be the part which stay in parentheses

                    ++i;
                    while (i < formula.Length && parenthesesCounter > 0)
                        //going down and seek the end
                        //by the way recording the internal part
                    {  
                        if (formula[i] == '(')
                            parenthesesCounter++;
                        if (formula[i] == ')')
                            parenthesesCounter--;
                        if (parenthesesCounter > 0)
                            inParenteses += formula[i];
                        i++;
                    }
                    Function FuncinParentheses = new Function(inParenteses);
                    //we need to calculate the part in parentheses earlier than any another operation
                    //so let's assume this part is independent formula
                    // and put on this place already parsed tree of it
                    parsedFormula.Add(FuncinParentheses.root);
                    i++;
                }
                else if (Char.IsDigit(formula[i]))
                    //here we check if numeric value was began parsing
                    //and in the same way recognise it's value and create constant, adding it to formula
                {
                    double value = 0;
                    while (i < formula.Length && Char.IsDigit(formula[i]))
                        value = value * 10 + formula[i++] - '0';
                    if (i < formula.Length && (formula[i] == '.' || formula[i] == ','))
                    {
                        ++i;
                        double divider = 10;
                        while (i < formula.Length && Char.IsDigit(formula[i]))
                        {
                            value += (formula[i++] - '0') / divider;
                            divider *= 10;
                        }
                    }
                    parsedFormula.Add(new Const(value));
                }
                else if (Operation.CharIsOperatorPart(formula[i]))
                    //if char is operator part - we need to read only one it's symbol
                    //there is impossible to let the operator have two symbols by the definition
                    //try to define it and push back to the parsed formula
                {
                    if (formula[i] == '-' && 
                        ( parsedFormula.Count == 0 
                                || (parsedFormula[parsedFormula.Count - 1] is Operation) 
                                    && (parsedFormula[parsedFormula.Count - 1] as Operation).index < 13)){
                        parsedFormula.Add(new Operation("opposite", new FormulaObject(), new FormulaObject()));
                        i++;
                    }
                    else 
                        parsedFormula.Add(new Operation("" + formula[i++], new FormulaObject(), new FormulaObject()));
                    //here is solving the problem of '-' definition 
                    //it can be the unary and equally likely to be binary
                    //so we need to look it's predecessor
                    //if const - only binary, else - only unary because of standart math syntax
                }
                else if (Operation.CharIsMethodPart(formula[i]))
                {
                    string formulaName = "";
                    while (i < formula.Length && Operation.CharIsMethodPart(formula[i]))
                        formulaName += formula[i++];
                    parsedFormula.Add(new Operation(formulaName, new FormulaObject(), new FormulaObject()));
                    //here we read the full operation (Method) name and push it at the end of formula
                }
                else if (formula[i++] != ' ') throw new Exception("Unacceptable symbol : " + formula[i]);
            }
            if (parenthesesCounter != 0)
                throw new Exception("Uncorrect parentheses arrangement");
            //and we should check if all parentheses are correct
            //if not - stop working - the result has no matter

            for (int priority = 10; priority > 0; --priority)
                //looking for the most priority operations in funtion
                //and assign them their operands
                //there we must use field wasActivated not to let operators be assigned twice
            {
                for (int i = parsedFormula.Count - 1; i >= 0; --i)
                    if (parsedFormula[i] is Operation
                        && (parsedFormula[i] as Operation).Priority() == priority
                        && (parsedFormula[i] as Operation).ArgumentsCount() == 1
                        && (parsedFormula[i] as Operation).wasActivated == false)
                    {
                        (parsedFormula[i] as Operation).right = parsedFormula[i + 1];
                        (parsedFormula[i] as Operation).wasActivated = true;
                        parsedFormula.RemoveAt(i + 1);
                    }
                //check the unary functions from right to left
                //note: to let cos cos cos cos x be computed witho
                for (int i = 0; i < parsedFormula.Count; ++i)
                    if (parsedFormula[i] is Operation
                        && (parsedFormula[i] as Operation).Priority() == priority
                        && (parsedFormula[i] as Operation).ArgumentsCount() == 2
                        && (parsedFormula[i] as Operation).wasActivated == false)
                    {
                        (parsedFormula[i] as Operation).right = parsedFormula[i + 1];
                        (parsedFormula[i] as Operation).left = parsedFormula[i - 1];
                        (parsedFormula[i] as Operation).wasActivated = true;

                        parsedFormula.RemoveAt(i + 1);
                        parsedFormula.RemoveAt(i - 1);
                        i = 0;
                    }
                //and the binary from left to right
            }
            if (parsedFormula.Count != 1)
                throw new Exception("Unacceptable formula");
            //check all formula was reduced up to singular tree

            root = parsedFormula[0];
        }
        //in constructor we get the text of neccessary formula
        //parse it and form the binary tree of calculating order
        //and we do it just once, that let to mitigate the count of computing for each X when Charting
        public double Execute(double argument)
        {
            Operation.x = argument;
            return root.Execute();
        }
        //get the value of the formula root - it will find all neccessary values by itself
    }
}
