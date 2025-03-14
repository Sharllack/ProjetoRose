using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using ClasseDeEntidades;
using ClasseDeNegocios;
using LiveCharts;
using LiveCharts.Wpf;
using System.Linq;
using System.Drawing;


namespace ProjetoRose
{
    public partial class FormEstatisticas : Form
    {
        ClasseNegocios classeNegocios = new ClasseNegocios();
        ClasseEntidades classeEntidades = new ClasseEntidades();
        public FormEstatisticas()
        {
            InitializeComponent();
        }

        private void CriarGraficos(DataTable dt)
        {
            // Criando o gráfico corretamente
            LiveCharts.WinForms.PieChart graficoGeral = new LiveCharts.WinForms.PieChart
            {
                Dock = DockStyle.Fill
            };

            // Adicionando os dados ao gráfico
            foreach (DataRow row in dt.Rows)
            {
                string categoria = row["nome"].ToString(); // Pega o nome do funcionário
                double valor = Convert.ToDouble(row["total_entregas"]); // Pega o valor de entregas

                graficoGeral.Series.Add(new PieSeries
                {
                    Title = categoria,
                    Values = new ChartValues<double> { valor },
                    DataLabels = true // Exibe os valores no gráfico
                });
            }

            // Limpando o painel antes de adicionar o gráfico (evita sobreposição)
            panel2.Controls.Clear();
            panel2.Controls.Add(graficoGeral);
        }

        private void CriarGraficosIndividuais(DataTable dt)
        {
            panel3.Controls.Clear();
            panel3.AutoScroll = true;

            Dictionary<string, Dictionary<string, double>> entregasPorFuncionario = new Dictionary<string, Dictionary<string, double>>();

            foreach (DataRow row in dt.Rows)
            {
                string nomeFuncionario = row["nome"].ToString();
                string dataEntrega = Convert.ToDateTime(row["data_entrega"]).ToString("dd/MM");
                double quantidadeEntregas = Convert.ToDouble(row["quantidade_entregas"]);

                if (!entregasPorFuncionario.ContainsKey(nomeFuncionario))
                {
                    entregasPorFuncionario[nomeFuncionario] = new Dictionary<string, double>();
                }

                if (!entregasPorFuncionario[nomeFuncionario].ContainsKey(dataEntrega))
                {
                    entregasPorFuncionario[nomeFuncionario][dataEntrega] = 0;
                }

                entregasPorFuncionario[nomeFuncionario][dataEntrega] += quantidadeEntregas;
            }

            int alturaTotal = 10;

            foreach (var funcionario in entregasPorFuncionario)
            {
                string nomeFuncionario = funcionario.Key;
                var entregasDiarias = funcionario.Value;

                Panel panelGrafico = new Panel
                {
                    Width = panel3.Width - 20,
                    Height = 300,
                    BackColor = Color.WhiteSmoke,
                    BorderStyle = BorderStyle.FixedSingle,
                    Padding = new Padding(10),
                    Left = 10
                };

                Label titulo = new Label
                {
                    Text = $"Entregador: {nomeFuncionario}",
                    Font = new Font("Arial", 12, FontStyle.Bold),
                    ForeColor = Color.Black,
                    AutoSize = true,
                    Top = 5,
                    Left = 5
                };

                LiveCharts.WinForms.CartesianChart graficoFuncionario = new LiveCharts.WinForms.CartesianChart
                {
                    Dock = DockStyle.Bottom,
                    Height = 250
                };

                ColumnSeries serie = new ColumnSeries
                {
                    Title = "Entregas Diárias",
                    Values = new ChartValues<double>(entregasDiarias.Values),
                    DataLabels = true,
                    Foreground = System.Windows.Media.Brushes.White // Muda a cor das letras dos valores
                };

                graficoFuncionario.Series = new SeriesCollection { serie };

                graficoFuncionario.AxisX.Add(new Axis
                {
                    Title = "Data",
                    Labels = entregasDiarias.Keys.ToArray()
                });

                graficoFuncionario.AxisY.Add(new Axis
                {
                    Title = "Entregas"
                });

                panelGrafico.Controls.Add(graficoFuncionario);
                panelGrafico.Controls.Add(titulo);

                panelGrafico.Top = alturaTotal;
                alturaTotal += panelGrafico.Height + 10;

                panel3.Controls.Add(panelGrafico);
            }

            panel3.AutoScrollMinSize = new Size(0, alturaTotal);
        }

        private void FormEstatisticas_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            classeEntidades.dataInicio = dateTimePicker1.Value.Date;
            classeEntidades.dataFim = dateTimePicker2.Value.Date;

            // Pegando os dois DataTables retornados pela função
            (DataTable dtTotal, DataTable dtDetalhado) = classeNegocios.NbuscarDadosEntregas(classeEntidades);

            // Criando gráficos para o total de entregas por funcionário
            CriarGraficos(dtTotal);

            // Criando gráficos individuais para cada entrega
            CriarGraficosIndividuais(dtDetalhado);
        }


        private void pictureBox1_Click(object sender, EventArgs e)
        {
            FormInicial formInicial = new FormInicial();
            this.Hide();
            formInicial.ShowDialog();
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
    }
}
