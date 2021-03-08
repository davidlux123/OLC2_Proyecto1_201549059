using _OLC2_Proyecto1.src;
using _OLC2_Proyecto1.src.Ambientes;
using _OLC2_Proyecto1.src.Gramatica;
using _OLC2_Proyecto1.src.Interfaces;
using Irony;
using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _OLC2_Proyecto1
{
    public partial class Form1 : Form
    {
        private string ruta;
        private GeneradorAST generadorAst;

        public Form1()
        {
            InitializeComponent();
            editorRichText.Text = File.ReadAllText("C:\\Users\\David Lux\\Desktop\\OLC2\\Proyecto1\\[OLC2]Proyecto1\\src\\entrada.txt");
        }

        private void abrirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog abrir = new OpenFileDialog()
            {
                Title = "Seleccione el Archivo",
                Filter = "Documantos de texto|*.txt",
                AddExtension = true,
            };

            if (abrir.ShowDialog() == DialogResult.OK)
            {
                editorRichText.Text = File.ReadAllText(abrir.FileName);
                ruta = abrir.FileName;
            }

        }

        private void guardarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            if (File.Exists(ruta))
            {
                String texto = editorRichText.Text;
                String[] lineas = texto.Split('\n');
                File.WriteAllLines(ruta, lineas);
            }
            else
            {

                SaveFileDialog guardar = new SaveFileDialog()
                {
                    Title = "Seleccione el Archivo",
                    Filter = "Documantos de texto|*.txt",
                    AddExtension = true,
                };

                if (guardar.ShowDialog() == DialogResult.OK)
                {
                    String texto = editorRichText.Text;
                    String[] lineas = texto.Split('\n');
                    File.WriteAllLines(guardar.FileName, lineas);

                    ruta = guardar.FileName;
                }

            }
        }

        private void guardarComoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog guardarComo = new SaveFileDialog()
            {
                Title = "Seleccione el Archivo",
                Filter = "Documantos de texto|*.txt",
            };

            if (guardarComo.ShowDialog() == DialogResult.OK)
            {
                String texto = editorRichText.Text;
                String[] lineas = texto.Split('\n');
                File.WriteAllLines(guardarComo.FileName, lineas);

                ruta = guardarComo.FileName;
            }
        }

        private void salirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Environment.Exit(0);
        }

        private void acercaDeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Deivid Alexander lux Revolorio, 201549059" + "\n"
           + "Proyecto 1 OLC2, Seccion: C" + "\n"
           + "Ing. Luis Espino");
        }

        private void ejecutarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (editorRichText.Text != "")
            {
                Gramatica grammar = new Gramatica();
                LanguageData lenguaje = new LanguageData(grammar);
                Parser parser = new Parser(lenguaje);
                ParseTree arbolIrony = parser.Parse(editorRichText.Text);

                //ERRORES LEXICO-SINTACTICOS
                List<string> ErroresLexSynt = new List<string>();
                if (arbolIrony.ParserMessages.Count != 0)
                    MessageBox.Show("Advertencia: encontraron errores lexico-Sintacticos en la entrada");
                foreach (LogMessage error in arbolIrony.ParserMessages)
                {
                    if (error.Message.Contains("Syntax"))
                    {
                        ErroresLexSynt.Add("<tr>\n" +
                        "\t<td>" + "Error Sintactico" + " </td>\n" +
                        "\t<td>" + error.Message.Replace("Syntax error, expected", "Se esperaba ") + "</td>\n" +
                        "\t<td>" + (error.Location.Line + 1) + "</td>\n" +
                        "\t<td>" + (error.Location.Column + 1) + "</td>\n" +
                        "</tr>\n\n");

                        ConsoleRichText.Text += " Error Sintactico: line: " + (error.Location.Line + 1) + " column: " + (error.Location.Column + 1) +
                        error.Message.Replace("Syntax error, expected:", ", Se esperaba un ") + "\n";
                    }
                    else
                    {
                        ErroresLexSynt.Add("<tr>\n" +
                        "\t<td>" + "Error Lexico" + " </td>\n" +
                        "\t<td>" + error.Message.Replace("Invalid character", "Caracter invalido") + "</td>\n" +
                        "\t<td>" + (error.Location.Line + 1) + "</td>\n" +
                        "\t<td>" + (error.Location.Column + 1) + "</td>\n</tr>\n\n");

                        ConsoleRichText.Text += "Error Lexico: line: " + (error.Location.Line + 1) + " column: " + (error.Location.Column + 1) +
                        error.Message.Replace("Invalid character:", " ,Caracter invalido ") + "\n";
                    }
                }

                //GENERAR AST
                this.generadorAst = new GeneradorAST(arbolIrony, ErroresLexSynt);//Se construye el AST pormedio del arbol que devuelve Irony
                Entorno entGlobal = new Entorno(null,"GLOBAL");
                ProgramClass programClas = new ProgramClass();

                if (this.generadorAst.Instrucciones != null) 
                {
                    foreach (Instruccion instruccion in this.generadorAst.Instrucciones)//ejecuta las instruciones del Begin main
                    {
                        try
                        {
                            if (instruccion != null)
                                instruccion.execute(entGlobal, programClas);
                        }
                        catch (Exception error)
                        {
                            string[] listErrs = error.Message.Split("\n|\n");
                            foreach (string err in listErrs)
                                if (err != "")
                                    generadorAst.Errores.Add(err);

                        }
                    }
                }
                else
                    MessageBox.Show("ERROR: No se pudo ejecutar las instrucciones de BEGIN-main porque IRONY no devolvio un AST ya sea porque " +
                    "se encontraron errores FATALES en la entrada y no logro recurperse");
            }

        }

        private void reportesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.generadorAst.arbolIrony.Root != null)
            {
                this.generadorAst.generarASTdot();
            }
            else
            {
                MessageBox.Show("ERROR: No se puede generar el Grafico del AST porque IRONY no devolvio un Sytax Tree ya sea porque " +
                "se encontraron errores Lexico-Sintacticos en la entrada");
            }
            this.generadorAst.generarErrores();
        }

        private void traducirToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

    }
}
