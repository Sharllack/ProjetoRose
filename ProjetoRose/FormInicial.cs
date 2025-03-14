using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjetoRose
{
    public partial class FormInicial : Form
    {
        public FormInicial()
        {
            InitializeComponent();
        }

        private void FormInicial_Load(object sender, EventArgs e)
        {
            label7.Text = FormPrincipal.user;
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label10.Text = DateTime.Now.ToString("HH:mm:ss");
            label11.Text = DateTime.Now.ToString("D");
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {
            FormPrincipal form = new FormPrincipal();
            this.Hide();
            form.ShowDialog();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            FormEntregas entregas = new FormEntregas();
            this.Hide();
            entregas.ShowDialog();
        }

        private void label2_Click(object sender, EventArgs e)
        {
            FormEntregas entregas = new FormEntregas();
            this.Hide();
            entregas.ShowDialog();
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            FormFuncionarios funcionarios = new FormFuncionarios();
            this.Hide();
            funcionarios.ShowDialog();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            FormEstatisticas formEstatisticas = new FormEstatisticas();
            this.Hide();
            formEstatisticas.ShowDialog();
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            FormFinanceiro formFinanceiro = new FormFinanceiro();
            this.Hide();
            formFinanceiro.ShowDialog();
        }

        private void pictureBox8_Click(object sender, EventArgs e)
        {
            FormRelatorio formRelatorio = new FormRelatorio();
            this.Hide();
            formRelatorio.ShowDialog();
        }
    }
}
