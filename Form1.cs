using _OLC2_Proyecto1.src;
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
        private String ruta;

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
                ParseTree arbol = parser.Parse(editorRichText.Text);

                if (arbol.ParserMessages.Count == 0)
                {
                    GeneradorAST generadorAst = new GeneradorAST(arbol);//Se construye el AST pormedio del arbol que devuelve Irony
                    
                    //generadorAst.generarASTdot();//Se grafica el arbol que devuelve irony 

                    AST ast = generadorAst.ast;

                    if (ast != null)
                        foreach (Instruccion ins in ast.Instrucciones)
                            ins.execute();
                    else
                        ConsoleRichText.Text = "Error panic: no se pudo generar el AST :(";

                }
                else // si se encontraron errores no ejecuta
                {
                    MessageBox.Show("No se ah podido ejecutar la entrada ya que se encontraron errores");
                    List<LogMessage> errores = arbol.ParserMessages;
                    foreach (LogMessage error in errores)
                        if (error.Message.Contains("Syntax"))
                            ConsoleRichText.Text += error.Message.Replace("Syntax error, expected", "Error Sintactico, se esperaba") + " line: " + (error.Location.Line + 1) + " column: " + error.Location.Column + "\n";
                        else
                            ConsoleRichText.Text += "Error Lexico: " + error.Message.Replace("Invalid character", "Caracter invalido") + " line: " + (error.Location.Line + 1) + " column: " + error.Location.Column + "\n";

                }

            }

        }

        private void reportesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (File.Exists("C:\\compiladores2\\AST.jpg"))
            {
                string comandoDot = "C:\\compiladores2\\AST.jpg";
                var comando = string.Format(comandoDot);
                var procStart = new System.Diagnostics.ProcessStartInfo("cmd", "/C" + comando);
                var proc = new System.Diagnostics.Process();
                proc.StartInfo = procStart;
                proc.Start();
                proc.WaitForExit();
            }
            else
                MessageBox.Show("Advertencia: no se creo la imagen del AST");

        }

        private void traducirToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

    }
}
