﻿using BLL;
using ENTITY;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Presentation
{
    public partial class frmLogin : Form
    {
        private readonly UserLogic userLogic;
        public frmLogin()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            userLogic = new UserLogic();
            textBox1.KeyDown += LoginTextBox_KeyDown;
            textBox2.KeyDown += LoginTextBox_KeyDown;
        }

        [DllImport("user32.dll", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hwnd, int wmsg, int wparam, int lparam);

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized)
            {
                this.MaximumSize = Screen.FromHandle(this.Handle).WorkingArea.Size;
                this.Location = Screen.FromHandle(this.Handle).WorkingArea.Location;
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            if (textBox1.Text == "User Name")
            {
                textBox1.Text = "";
                textBox1.ForeColor = Color.Black;
            }
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                textBox1.Text = "User Name";
                textBox1.ForeColor = Color.Gray;
            }
        }

        private void textBox2_Enter(object sender, EventArgs e)
        {
            if (textBox2.Text == "Type Your Password")
            {
                textBox2.Text = "";
                textBox2.ForeColor = Color.Black;
                textBox2.UseSystemPasswordChar = true; // Ocultar caracteres
            }
        }

        private void textBox2_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox2.Text))
            {
                textBox2.UseSystemPasswordChar = false;
                textBox2.Text = "Type Your Password";
                textBox2.ForeColor = Color.Gray;
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void Panel2_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private void label3_Click(object sender, EventArgs e)
        {
            this.Hide();
            frmRegister registerForm = new frmRegister();
            registerForm.ShowDialog();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            // Validar campos vacíos o valores por defecto
            if (string.IsNullOrWhiteSpace(textBox1.Text) || textBox1.Text == "User Name" ||
                string.IsNullOrWhiteSpace(textBox2.Text) || textBox2.Text == "Type Your Password")
            {
                frmMessageBox.Show("Por favor, complete todos los campos.", "Campos vacíos");
                return;
            }

            // Intentar login
            User user = userLogic.Login(textBox1.Text.ToUpper(), textBox2.Text);

            if (user != null)
            {
                frmMessageBox.Show("¡Inicio de sesión exitoso!", "Bienvenido");
                Session.Start(user);
                this.Hide();
                frmMain main = new frmMain();
                main.ShowDialog();
            }
            else
            {
                frmMessageBox.Show("Usuario o contraseña incorrectos.", "Error de autenticación");
            }
        }

        private void btnMinimizar_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        //private void btnClose_Click(object sender, EventArgs e)
        //{
        //    Application.Exit();
        //}

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnClse_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;

        }


        private void LoginTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnLogin_Click(sender, EventArgs.Empty);
            }
        }
        private void pictureBox1_Click(object sender, EventArgs e)
        {


        }
    }
}
