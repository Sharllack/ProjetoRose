using System;
using System.Data;
using System.Windows.Forms;
using ClasseDeEntidades;
using ClasseDeNegocios;
using ProjetoRose.Entities;
using ProjetoRose.Utils;

namespace ProjetoRose
{
    public partial class FormEntregas : Form
    {
        ClasseEntidades classeEntidades = new ClasseEntidades();
        ClasseNegocios classeNegocios = new ClasseNegocios();

        public FormEntregas()
        {
            InitializeComponent();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            FormInicial formInicial = new FormInicial();
            this.Hide();
            formInicial.ShowDialog();
        }

        private async void FormEntregas_Load(object sender, EventArgs e)
        {
            await CarregamentoHelper.CarregarDadosAsync(
                panelLoading,
                // Passando os métodos e as ações para cada tela
                (() => classeNegocios.NlistarEntregas(), (dados) => dataGridView1.DataSource = dados),
                (() => classeNegocios.NlistarFuncionarios(), (dados) =>
                {
                    comboBox1.DataSource = dados;
                    comboBox1.ValueMember = "id";
                    comboBox1.DisplayMember = "nome";
                }
            )
            );
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            classeEntidades.nome = textBox5.Text + "%"; // Garante que "%" seja incluído
            classeEntidades.cpf = textBox5.Text + "%"; // Garante que "%" seja incluído
            classeEntidades.veiculo = textBox5.Text + "%"; // Garante que "%" seja incluído
            DataTable dt = classeNegocios.NbuscarEntregas(classeEntidades);
            dataGridView1.DataSource = dt;
        }

        private void limpar()
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox6.Text = "";
            textBox7.Text = "";
        }

        void CRUD(string acao)
        {
            int.TryParse(textBox3.Text, out int codigo);
            int.TryParse(comboBox1.SelectedValue.ToString(), out int id);
            int.TryParse(textBox2.Text, out int documentosTotais);
            int.TryParse(textBox6.Text, out int fc);
            int.TryParse(textBox7.Text, out int caixa);
            int.TryParse(textBox1.Text, out int devolucao);
            int.TryParse(textBox4.Text, out int carta);

            classeEntidades.codigo = codigo;
            classeEntidades.id = id;
            classeEntidades.dataEntrega = dateTimePicker1.Value;
            classeEntidades.documentosTotais = documentosTotais;
            classeEntidades.fc = fc;
            classeEntidades.caixa = caixa;
            classeEntidades.devolucao = devolucao;
            classeEntidades.carta = carta;
            classeEntidades.entregasTotais = fc + caixa + carta;
            classeEntidades.acao = acao;

            String manutencao = classeNegocios.NCRUDentregas(classeEntidades);
            MessageBox.Show(manutencao, "Mensagem", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


        private void button5_Click(object sender, EventArgs e)
        {
            limpar();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Deseja cadastrar uma nova entrega?", "Mensagem", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == System.Windows.Forms.DialogResult.Yes)
            {
                CRUD("1");
                dataGridView1.DataSource = classeNegocios.NlistarEntregas();
                limpar();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Deseja alterar esta entrega?", "Mensagem", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == System.Windows.Forms.DialogResult.Yes)
            {
                CRUD("2");
                dataGridView1.DataSource = classeNegocios.NlistarEntregas();
                limpar();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Deseja eliminar esta entrega?", "Mensagem", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == System.Windows.Forms.DialogResult.Yes)
            {
                CRUD("3");
                dataGridView1.DataSource = classeNegocios.NlistarEntregas();
                limpar();
            }
        }

        // Método para calcular e atualizar textBox4
        private void CalcularResultado()
        {
            int valor1, valor2, valor3, valor4;

            // Converte os valores, assumindo 0 se estiver vazio ou inválido
            int.TryParse(textBox2.Text, out valor1);
            int.TryParse(textBox6.Text, out valor2);
            int.TryParse(textBox7.Text, out valor3);
            int.TryParse(textBox1.Text, out valor4);

            // Faz a conta e atualiza o resultado
            textBox4.Text = (valor1 - valor2 - valor3 - valor4).ToString();
        }

        // Eventos que chamam o cálculo sempre que um dos campos for alterado
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            CalcularResultado();
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            CalcularResultado();
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            CalcularResultado();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            CalcularResultado();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int fila = dataGridView1.CurrentCell.RowIndex;

            textBox1.Text = dataGridView1[8, fila].Value.ToString();
            textBox3.Text = dataGridView1[0, fila].Value.ToString();
            textBox4.Text = dataGridView1[5, fila].Value.ToString();
            textBox6.Text = dataGridView1[4, fila].Value.ToString();
            textBox7.Text = dataGridView1[6, fila].Value.ToString();
            comboBox1.SelectedIndex = comboBox1.FindStringExact(dataGridView1[1, fila].Value.ToString());
            dateTimePicker1.Value = Convert.ToDateTime(dataGridView1[3, fila].Value);

            // Convertendo os valores e somando
            int valor1 = int.TryParse(textBox1.Text, out int v1) ? v1 : 0;
            int valor4 = int.TryParse(textBox4.Text, out int v4) ? v4 : 0;
            int valor6 = int.TryParse(textBox6.Text, out int v6) ? v6 : 0;
            int valor7 = int.TryParse(textBox7.Text, out int v7) ? v7 : 0;

            textBox2.Text = (valor1 + valor4 + valor6 + valor7).ToString();
        }

        private void panelLoading_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
