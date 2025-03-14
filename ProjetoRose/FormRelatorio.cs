using System;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Windows.Controls;
using System.Windows.Forms;
using ClasseDeEntidades;
using ClasseDeNegocios;
using ProjetoRose.Utils;

namespace ProjetoRose
{
    public partial class FormRelatorio : Form
    {
        ClasseNegocios classeNegocios = new ClasseNegocios();
        ClasseEntidades classeEntidades = new ClasseEntidades();
        PrintDocument printDocument = new PrintDocument(); // Adicionando o controle de impressão

        public FormRelatorio()
        {
            InitializeComponent();
            printDocument.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(printDocument_PrintPage);
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private async void FormRelatorio_Load(object sender, EventArgs e)
        {
            try
            {
                // Chama a tela de carregamento enquanto os dados são carregados
                await CarregamentoHelper.CarregarDadosAsync(
                    panelLoading,
                    // Passando os métodos e as ações para cada tela
                    // Método para carregar o Relatório e atualizar o DataGridView
                    (() => classeNegocios.NlistarRelatorio(), (dados) => dataGridView1.DataSource = dados),
                    // Método para carregar os Veículos e atualizar o ComboBox
                    (() => classeNegocios.NlistarVeiculos(), (dados) =>
                    {
                        // Criando uma nova linha vazia para o ComboBox
                        DataRow row = dados.NewRow();
                        row["id_veiculo"] = DBNull.Value; // Valor nulo para não filtrar
                        row["veiculo"] = "Todos"; // Texto visível no ComboBox
                        dados.Rows.InsertAt(row, 0); // Adiciona no início

                        // Atualizando o ComboBox
                        comboBox1.DataSource = dados;
                        comboBox1.ValueMember = "id_veiculo";
                        comboBox1.DisplayMember = "veiculo";
                    }
                )
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar os dados: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            FormInicial formInicial = new FormInicial();
            this.Hide();
            formInicial.ShowDialog();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedValue == DBNull.Value)
            {
                dataGridView1.DataSource = classeNegocios.NlistarRelatorio();
            }
            else
            {
                string opcaoSelecionada = comboBox1.SelectedValue.ToString();

                classeEntidades.veiculo = opcaoSelecionada;

                // Consulta no banco
                DataTable relat = classeNegocios.NlistarFuncionariosRelatorio(classeEntidades);
                dataGridView1.DataSource = relat;
            }
        }


        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            classeEntidades.dataInicio = dateTimePicker1.Value.Date;
            classeEntidades.dataFim = dateTimePicker2.Value.Date;
            classeEntidades.veiculo = comboBox1.SelectedValue.ToString();
            DataTable filtro = classeNegocios.NlistarFuncionariosFiltro(classeEntidades);
            dataGridView1.DataSource = filtro;
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            // Configurando a pré-visualização de impressão
            PrintPreviewDialog previewDialog = new PrintPreviewDialog();
            previewDialog.Document = printDocument;

            // Definindo a página como paisagem
            printDocument.DefaultPageSettings.Landscape = true;

            // Ajustando as margens (diminuí para aproveitar mais espaço)
            printDocument.DefaultPageSettings.Margins = new Margins(10, 10, 10, 10);

            previewDialog.ShowDialog();
        }

        // Método que será chamado durante a impressão
        private void printDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            Font font = new Font("Arial", 7);  // Reduzi um pouco o tamanho da fonte
            SolidBrush brush = new SolidBrush(Color.Black);
            Pen pen = new Pen(Color.Black, 1); // Caneta para desenhar as bordas

            int margemEsquerda = e.MarginBounds.Left;
            int margemTopo = e.MarginBounds.Top;

            // **Cabeçalho do Relatório**
            g.DrawString("Relatório de Funcionários", new Font("Arial", 12, FontStyle.Bold), brush, margemEsquerda, margemTopo);
            g.DrawLine(Pens.Black, margemEsquerda, margemTopo + 20, e.MarginBounds.Right, margemTopo + 20);

            int linhaY = margemTopo + 40; // Posição inicial para o cabeçalho da tabela
            int colX = margemEsquerda; // Posição inicial das colunas

            int colWidth = 104;  // Ajustei a largura das colunas
            int rowHeight = 30;  // Altura das linhas

            // **Cabeçalhos das Colunas**
            foreach (DataGridViewColumn coluna in dataGridView1.Columns)
            {
                g.DrawRectangle(pen, colX, linhaY, colWidth, rowHeight); // Desenha a borda do cabeçalho
                g.DrawString(coluna.HeaderText, new Font("Arial", 8, FontStyle.Bold), brush, new RectangleF(colX + 5, linhaY + 5, colWidth - 10, rowHeight - 10), new StringFormat { Alignment = StringAlignment.Center }); // Centraliza o texto
                colX += colWidth;  // Próxima coluna
            }

            linhaY += rowHeight; // Avança para a primeira linha de dados

            // **Dados das Linhas**
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                colX = margemEsquerda; // Volta para a primeira coluna

                foreach (DataGridViewCell cell in row.Cells)
                {
                    g.DrawRectangle(pen, colX, linhaY, colWidth, rowHeight); // Desenha a borda da célula
                    if (cell.Value != null)
                    {
                        // Configuração para cortar texto longo e evitar sobreposição
                        StringFormat format = new StringFormat
                        {
                            Trimming = StringTrimming.EllipsisCharacter,
                            FormatFlags = StringFormatFlags.LineLimit,
                            Alignment = StringAlignment.Center,  // **Centraliza o conteúdo**
                            LineAlignment = StringAlignment.Center  // **Centraliza na vertical**
                        };

                        // Escreve o valor dentro da célula, ajustado e centralizado
                        g.DrawString(cell.Value.ToString(), font, brush, new RectangleF(colX + 5, linhaY + 5, colWidth - 10, rowHeight - 10), format);
                    }
                    colX += colWidth; // Próxima célula
                }

                linhaY += rowHeight; // Próxima linha
                if (linhaY > e.MarginBounds.Bottom)
                {
                    e.HasMorePages = true;
                    return; // Indica que tem mais páginas
                }
            }

            e.HasMorePages = false; // Finaliza se não houver mais páginas
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panelLoading_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
