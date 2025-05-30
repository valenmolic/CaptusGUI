﻿using BLL;
using ENTITY;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Presentation
{
    public partial class frmTaskDetails: Form
    {
        private ENTITY.Task task;
        private TaskLogic taskLogic;
        private CategoryLogic categoryLogic;
        private PriorityLogic priorityLogic;
        private ENTITY.Task originalTask;
        private ENTITY.Task editableTask;
        public frmTaskDetails(ENTITY.Task task)
        {
            InitializeComponent();
            this.task = task;
            taskLogic = new TaskLogic(Configuration.ConnectionString);
            categoryLogic = new CategoryLogic();
            priorityLogic = new PriorityLogic();
            EditCalendar();
            originalTask = task;
            editableTask = new ENTITY.Task
            {
                id = task.id,
                Title = task.Title,
                Description = task.Description,
                EndDate = task.EndDate,
                Priority = task.Priority,
                Category = task.Category
            };
            LoadPriorities();
                LoadCategories();
            ApliessColor(task.Priority.Id_Priority);
            label1.Text = task.Category.Name;
        }
        private void VerificarCambios()
        {
            var selectedPriority = cbPriority.SelectedItem as Priority;
            var selectedCategory = cbCategory.SelectedItem as Category;

            bool cambiado = rtxtTitle.Text != originalTask.Title ||
                            rtxtDescription.Text != originalTask.Description ||
                            dtpFecha.Value.Date != originalTask.EndDate.Date ||
                            (selectedCategory != null && selectedCategory.id != originalTask.Category.id) ||
                            (selectedPriority != null && selectedPriority.Id_Priority != originalTask.Priority.Id_Priority);

            btnSave.Visible = cambiado;
        }
        private void LoadPriorities()
        {
            var priorities = priorityLogic.GetAll();

            cbPriority.DataSource = priorities;
            cbPriority.DisplayMember = "Name";
            cbPriority.ValueMember = "Id_Priority";
            cbPriority.SelectedValue = originalTask.Priority.Id_Priority;
        }
        private void LoadCategories()
        {
            var categories = categoryLogic.GetAll();
            cbCategory.DataSource = categories;
            cbCategory.DisplayMember = "Name";
            cbCategory.ValueMember = "Id";
            cbCategory.SelectedValue = originalTask.Category.id;
        }
        private void EditCalendar()
        {
            dtpFecha.MinDate = DateTime.Today;
            if (task.EndDate == DateTime.Today)
            {
                dtpFecha.Format = DateTimePickerFormat.Custom;
                dtpFecha.CustomFormat = "\"Hoy\"";
            }
            else if (task.EndDate ==DateTime.Today.AddDays(1))
            {
                    dtpFecha.Format = DateTimePickerFormat.Custom;
                    dtpFecha.CustomFormat = "\"Mañana\"";
            }
            else
            {
                dtpFecha.Format = DateTimePickerFormat.Custom;
                dtpFecha.CustomFormat = "dddd, dd/MMM";
            }   
        }
        private void frmTaskDetails_Load(object sender, EventArgs e)
        {
            rtxtTitle.Text = editableTask.Title;
            rtxtDescription.Text = editableTask.Description;
            dtpFecha.MinDate = DateTime.Today;

            cbCategory.SelectedItem = editableTask.Category;
            cbPriority.SelectedItem = editableTask.Priority;

            ApliessColorByPriority(editableTask.Priority.Id_Priority);
            VerificarCambios();
        }

        private void btnDelete_MouseClick(object sender, MouseEventArgs e)
        {
            DeleteTask();
        }
        private async void DeleteTask()
        {
            var confirm = MessageBox.Show("¿Estás seguro de eliminar esta tarea?", "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (confirm == DialogResult.Yes)
            {
                var result = taskLogic.Delete(task.id);
                if (result.Success)
                {
                    string mensaje = NotifyEmails.GetMessageDelete(task.Title);
                    await NotifyEmails.SendNotifyAsync(Session.CurrentUser.Email, "Tarea eliminada", mensaje);
                    frmMessageBox.Show("Tarea eliminada.", "Éxito");
                    this.Close();
                }
                else
                {
                    frmMessageBox.Show(result.Message, "Error");
                }
                
            }
        }

        private void btnSave_MouseClick(object sender, MouseEventArgs e)
        {
            UpdateTask();
            this.Dispose();
        }
        private async void UpdateTask()
        {
            if (string.IsNullOrWhiteSpace(rtxtTitle.Text))
            {
                frmMessageBox.Show("El título no puede estar vacío.", "Advertencia");
                return;
            }
            string titulo = rtxtTitle.Text.Trim();
            string descripcion = string.IsNullOrWhiteSpace(rtxtDescription.Text) ? null : rtxtDescription.Text.Trim();
            DateTime fecha = dtpFecha.Value.Date;
            int prioridad = Convert.ToInt32(cbPriority.SelectedValue);
            int categoria = Convert.ToInt32(cbCategory.SelectedValue);
            task.Title = titulo;
            task.Category = categoryLogic.GetById(categoria);
            task.Description = descripcion;
            task.EndDate = fecha;
            task.Priority = priorityLogic.GetById(prioridad);
            var result = taskLogic.Update(task);
            if (!result.Success)
            {
                frmMessageBox.Show(result.Message, "Error");
                return;
            }
            string mensaje = NotifyEmails.GetMessageUpdate(task.Title, task.EndDate.ToShortDateString(), task.Category.Name);
            await NotifyEmails.SendNotifyAsync(Session.CurrentUser.Email, "Tarea actualizada", mensaje);
            frmMessageBox.Show("Tarea actualizada.", "Éxito");
            this.Close();
        }

        private void rtxtTitle_TextChanged(object sender, EventArgs e)
        {
            VerificarCambios();
        }

        private void rtxtDescription_TextChanged(object sender, EventArgs e)
        {
            VerificarCambios();
        }

        private void cbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.ActiveControl = null;
            VerificarCambios();
        }

        private void cbCategory_SelectedValueChanged(object sender, EventArgs e)
        {
            this.ActiveControl = null;
            VerificarCambios();
        }

        private void dtpFecha_ValueChanged(object sender, EventArgs e)
        {
            this.ActiveControl = null;
            VerificarCambios();
        }

        private void cbPriority_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.ActiveControl = null;
            VerificarCambios();
            ApliessColorByPriority(cbPriority.SelectedIndex);
        }

        private void cbPriority_SelectedValueChanged(object sender, EventArgs e)
        {
            VerificarCambios();
            this.ActiveControl = null;
            ApliessColorByPriority(cbPriority.SelectedIndex);
        }
        private void ApliessColorByPriority(int idPrioridad)
        {
            switch (idPrioridad)
            {
                
                case 0: // Alta
                    cbPriority.BackColor = Color.Red;
                    cbPriority.ForeColor = Color.White;
                    break;
                case 1: // Baja
                    cbPriority.BackColor = Color.Green;
                    cbPriority.ForeColor = Color.White;
                    
                    break;
                case 2: // MEdia
                    cbPriority.BackColor = Color.Orange;
                    cbPriority.ForeColor = Color.White;
                    break;
                default:
                    cbPriority.BackColor = SystemColors.Window;
                    cbPriority.ForeColor = SystemColors.ControlText;
                    break;
            }
        }
        private void ApliessColor(int idPrioridad)
        {
            switch (idPrioridad)
            {

                case 4: // Alta
                    cbPriority.BackColor = Color.Red;
                    cbPriority.ForeColor = Color.White;
                    break;
                case 3: // Media
                    cbPriority.BackColor = Color.Orange;
                    cbPriority.ForeColor = Color.White;
                    break;
                case 1: // Baja
                    cbPriority.BackColor = Color.Green;
                    cbPriority.ForeColor = Color.White;
                    break;
                default:
                    cbPriority.BackColor = SystemColors.Window;
                    cbPriority.ForeColor = SystemColors.ControlText;
                    break;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {

        }

        private void btnClse_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
