using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClasseDeEntidades;
using ClasseDeNegocios;
using ProjetoRose.Entities;
using ProjetoRose.Utils;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ProjetoRose
{
    public partial class FormFuncionarios : Form
    {
        ClasseEntidades classeEntidades = new ClasseEntidades();
        ClasseNegocios classeNegocios = new ClasseNegocios();
        public FormFuncionarios()
        {
            InitializeComponent();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void maskedTextBox3_Leave(object sender, EventArgs e)
        {
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            FormInicial formInicial = new FormInicial();
            this.Hide();
            formInicial.ShowDialog();
        }

        private void maskedTextBox1_Leave(object sender, EventArgs e)
        {
            if (!maskedTextBox1.MaskCompleted || !ValidadorCPF.ValidarCPF(maskedTextBox1.Text))
            {
                MessageBox.Show("CPF inválido!", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                maskedTextBox1.Focus();
            }
        }

        private async void FormFuncionarios_Load(object sender, EventArgs e)
        {
            await CarregamentoHelper.CarregarDadosAsync(
                panelLoading,
                // Passando os métodos e as ações para cada tela
                (() => classeNegocios.NlistarInfosFuncionarios(), (dados) => dataGridView1.DataSource = dados),
                (() => classeNegocios.NlistarVeiculos(), (dados) =>
                {
                    comboBox1.DataSource = dados;
                    comboBox1.ValueMember = "id_veiculo";
                    comboBox1.DisplayMember = "veiculo";
                }
            )
            );
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            classeEntidades.nome = textBox5.Text + "%"; // Garante que "%" seja incluído
            classeEntidades.cpf = textBox5.Text + "%"; // Garante que "%" seja incluído
            classeEntidades.veiculo = textBox5.Text + "%"; // Garante que "%" seja incluído
            classeEntidades.celular = textBox5.Text + "%";
            DataTable dt = classeNegocios.NbuscarFuncionarios(classeEntidades);
            dataGridView1.DataSource = dt;
        }

        private void limpar()
        {
            textBox2.Text = "";
            textBox3.Text = "";
            maskedTextBox1.Text = "";
            maskedTextBox2.Text = "";
            maskedTextBox3.Text = "";
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null) return;

            string opcaoSelecionada = comboBox1.Text; // Pegando o texto real do item selecionado

            if (opcaoSelecionada == "Carro")
            {
                maskedTextBox3.ReadOnly = false;
                maskedTextBox3.BackColor = Color.White;
            }
            else
            {
                maskedTextBox3.ReadOnly = true;
                maskedTextBox3.BackColor = Color.Gray;
                maskedTextBox3.Text = "";
            }
        }

        void CRUD(string acao)
        {
            int.TryParse(textBox3.Text, out int codigo);
            decimal.TryParse(maskedTextBox3.Text, out decimal salario);

            classeEntidades.codigo = codigo;
            classeEntidades.veiculo = comboBox1.SelectedValue.ToString();
            classeEntidades.nome = textBox2.Text;
            classeEntidades.dataNascimento = dateTimePicker1.Value;
            classeEntidades.cpf = maskedTextBox1.Text;
            classeEntidades.celular = maskedTextBox2.Text;
            classeEntidades.salarioBase = salario;
            classeEntidades.acao = acao;

            if (string.IsNullOrEmpty(classeEntidades.nome) || string.IsNullOrEmpty(classeEntidades.cpf))
            {
                MessageBox.Show("Por favor, preencha todos os campos obrigatórios.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            String manutencao = classeNegocios.NCRUDfuncionarios(classeEntidades);
            MessageBox.Show(manutencao, "Mensagem", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            limpar();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Deseja cadastrar uma novo funcionário?", "Mensagem", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == System.Windows.Forms.DialogResult.Yes)
            {
                CRUD("1");
                dataGridView1.DataSource = classeNegocios.NlistarInfosFuncionarios();
                limpar();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Deseja alterar este funcionário?", "Mensagem", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == System.Windows.Forms.DialogResult.Yes)
            {
                CRUD("2");
                dataGridView1.DataSource = classeNegocios.NlistarInfosFuncionarios();
                limpar();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Deseja eliminar este funcionário?", "Mensagem", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == System.Windows.Forms.DialogResult.Yes)
            {
                CRUD("3");
                dataGridView1.DataSource = classeNegocios.NlistarInfosFuncionarios();
                limpar();
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int fila = dataGridView1.CurrentCell.RowIndex;

            textBox2.Text = dataGridView1[1, fila].Value.ToString();
            maskedTextBox1.Text = dataGridView1[2, fila].Value.ToString();
            maskedTextBox2.Text = dataGridView1[3, fila].Value.ToString();
            maskedTextBox3.Text = dataGridView1[6, fila].Value.ToString();
            dateTimePicker1.Value = Convert.ToDateTime(dataGridView1[4, fila].Value);
            comboBox1.SelectedIndex = comboBox1.FindStringExact(dataGridView1[5, fila].Value.ToString());
            textBox3.Text = dataGridView1[0, fila].Value.ToString();
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void panelLoading_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
