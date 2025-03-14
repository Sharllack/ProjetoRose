using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClasseDeEntidades;
using ClasseDeNegocios;
using ProjetoRose.Utils;

namespace ProjetoRose
{
    public partial class FormFinanceiro : Form
    {
        ClasseEntidades classeEntidades = new ClasseEntidades();
        ClasseNegocios classeNegocios = new ClasseNegocios();
        public FormFinanceiro()
        {
            InitializeComponent();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            FormInicial formInicial = new FormInicial();
            this.Hide();
            formInicial.ShowDialog();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private async void FormFinanceiro_Load(object sender, EventArgs e)
        {
            await CarregamentoHelper.CarregarDadosAsync(
                panelLoading,
                // Passando os métodos e as ações para cada tela
                (() => classeNegocios.NlistarAdcDesc(), (dados) => dataGridView1.DataSource = dados),
                (() => classeNegocios.NlistarFuncionarios(), (dados) =>
                {
                    comboBox1.DataSource = dados;
                    comboBox1.ValueMember = "id";
                    comboBox1.DisplayMember = "nome";
                }
            )
            );
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void limpar()
        {
            comboBox1.Text = "";
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
        }

        private void button5_Click(object sender, EventArgs e)
        {
            limpar();
        }

        void CRUD(string acao)
        {
            int.TryParse(textBox5.Text, out int codigo);
            classeEntidades.codigo = codigo;
            classeEntidades.id = int.Parse(comboBox1.SelectedValue.ToString());
            classeEntidades.desconto = decimal.Parse(textBox3.Text);
            classeEntidades.passagem = decimal.Parse(textBox1.Text);
            classeEntidades.alimentacao = decimal.Parse(textBox2.Text);
            classeEntidades.dataInicio = dateTimePicker1.Value;
            classeEntidades.dataFim = dateTimePicker2.Value;
            classeEntidades.acao = acao;

            String manutencao = classeNegocios.NCRUDadcdesc(classeEntidades);
            MessageBox.Show(manutencao, "Mensagem", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (MessageBox.Show("Deseja cadastrar um novo registro?", "Mensagem", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == System.Windows.Forms.DialogResult.Yes)
            {
                CRUD("1");
                dataGridView1.DataSource = classeNegocios.NlistarAdcDesc();
                limpar();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Deseja alterar este registro?", "Mensagem", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == System.Windows.Forms.DialogResult.Yes)
            {
                CRUD("2");
                dataGridView1.DataSource = classeNegocios.NlistarAdcDesc();
                limpar();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Deseja eliminar este registro?", "Mensagem", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == System.Windows.Forms.DialogResult.Yes)
            {
                CRUD("3");
                dataGridView1.DataSource = classeNegocios.NlistarAdcDesc();
                limpar();
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int fila = dataGridView1.CurrentCell.RowIndex;

            dateTimePicker1.Value = Convert.ToDateTime(dataGridView1[1, fila].Value);
            dateTimePicker2.Value = Convert.ToDateTime(dataGridView1[2, fila].Value);
            comboBox1.SelectedIndex = comboBox1.FindStringExact(dataGridView1[3, fila].Value.ToString());
            textBox1.Text = dataGridView1[4, fila].Value.ToString();
            textBox2.Text = dataGridView1[5, fila].Value.ToString();
            textBox3.Text = dataGridView1[6, fila].Value.ToString();
            textBox5.Text = dataGridView1[0, fila].Value.ToString();
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            classeEntidades.nome = textBox4.Text + "%"; // Mantém o wildcard pra busca por nome

            // Mantém data como DateTime, sem converter pra string aqui
            classeEntidades.dataInicio = dateTimePicker1.Value;
            classeEntidades.dataFim = dateTimePicker2.Value;

            // Chama a busca e preenche a tabela
            AtualizarGrid();
        }

        private void AtualizarGrid()
        {
            DataTable dt = classeNegocios.NbuscarAdc_desc(classeEntidades);
            dataGridView1.DataSource = dt;
        }


        private void groupBox1_Enter_1(object sender, EventArgs e)
        {

        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void panelLoading_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
