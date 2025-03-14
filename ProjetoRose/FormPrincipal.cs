using System;
using System.Data;
using System.Windows.Forms;
using ClasseDeEntidades;
using ClasseDeNegocios;
using ProjetoRose.Entities;

namespace ProjetoRose
{
    public partial class FormPrincipal : Form
    {
        ClasseNegocios classeNegocios = new ClasseNegocios();
        FormInicial formInicial = new FormInicial();

        public static string user;

        public FormPrincipal()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void limpar()
        {
            textBox1.Text = "";
            textBox2.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ClasseEntidades entidade = new ClasseEntidades();
            entidade.usuario = textBox1.Text;
            entidade.senha = textBox2.Text;

            DataTable dt = classeNegocios.Nlogin(entidade);

            if (dt.Rows.Count > 0)
            {
                MessageBox.Show("Bem-vindo(a) " + dt.Rows[0][0].ToString() + "!", "Mensagem", MessageBoxButtons.OK, MessageBoxIcon.Information);

                user = dt.Rows[0][0].ToString();

                this.Hide();
                formInicial.ShowDialog();
                limpar();
            }
            else
            {
                MessageBox.Show("Usuário ou senha errados!", "Mensagem", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void FormPrincipal_Load(object sender, EventArgs e)
        {
        }
    }
}
