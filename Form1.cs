// Desactiva las advertencias de "nullable" de .NET moderno (solo son avisos, no errores)
#nullable disable

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Prueba_Biblioteca
{
    // Esta clase es la ventana principal del programa.
    // El diseñador (Form1.Designer.cs) solo crea la ventana vacía;
    // todos los controles se crean por código en ConstruirInterfaz().
    public partial class Form1 : Form
    {
        // =====================================================
        // 1. CLASES DEL MODELO (los "objetos" de la biblioteca)
        // =====================================================

        // Representa un libro de la biblioteca
        public class Libro
        {
            public int Id { get; set; }               // Número único del libro
            public string Titulo { get; set; }        // Nombre del libro
            public string Autor { get; set; }         // Quién lo escribió
            public bool Disponible { get; set; }      // true = se puede prestar

            // Constructor: se ejecuta al crear un libro nuevo
            public Libro(int id, string titulo, string autor)
            {
                Id = id;
                Titulo = titulo;
                Autor = autor;
                Disponible = true; // Todo libro nuevo empieza disponible
            }
        }

        // Representa a una persona registrada en la biblioteca
        public class Usuario
        {
            public int Id { get; set; }
            public string Nombre { get; set; }

            public Usuario(int id, string nombre)
            {
                Id = id;
                Nombre = nombre;
            }
        }

        // Representa un préstamo: qué libro, quién lo tiene y cuándo se devuelve
        public class Prestamo
        {
            public Libro Libro { get; set; }
            public Usuario Usuario { get; set; }
            public DateTime Fecha { get; set; }         // Cuándo se prestó
            public DateTime FechaLimite { get; set; }   // Hasta cuándo puede tenerlo

            public Prestamo(Libro libro, Usuario usuario)
            {
                Libro = libro;
                Usuario = usuario;
                Fecha = DateTime.Now;
                FechaLimite = Fecha.AddDays(DiasPrestamo); // Hoy + 7 días
            }
        }

        // =====================================================
        // 2. CONFIGURACIÓN (puedes cambiar estos valores)
        // =====================================================

        const string NombreBiblioteca = "Biblioteca de Alejandría Di Iscariote";
        const int DiasPrestamo = 7;          // Días que dura un préstamo
        const int MaxLibrosPorUsuario = 3;   // Máximo de libros por persona

        // =====================================================
        // 3. LISTAS DEL SISTEMA (aquí se guardan los datos)
        // =====================================================

        List<Libro> libros = new List<Libro>();
        List<Usuario> usuarios = new List<Usuario>();
        List<Prestamo> prestamos = new List<Prestamo>();

        // =====================================================
        // 4. CONTROLES DE LA PANTALLA
        // =====================================================

        TabControl tabs;                        // Contenedor de pestañas
        StatusStrip barraEstado;                // Barra inferior
        ToolStripStatusLabel lblEstado;         // Texto de la barra inferior

        // Pestaña Libros
        DataGridView dgvLibros;                 // Tabla de libros
        TextBox txtBuscar, txtTitulo, txtAutor; // Cajas de texto

        // Pestaña Usuarios
        DataGridView dgvUsuarios;               // Tabla de usuarios
        TextBox txtNombreUsuario;

        // Pestaña Préstamos
        DataGridView dgvPrestamos;              // Tabla de préstamos
        ComboBox cmbLibro, cmbUsuario;          // Listas desplegables

        // Colores del tema (madera, pergamino y oro)
        Color colorMadera = Color.FromArgb(62, 39, 35);
        Color colorPergamino = Color.FromArgb(245, 235, 210);
        Color colorOro = Color.FromArgb(212, 175, 55);

        // =====================================================
        // 5. CONSTRUCTOR Y CARGA
        // =====================================================

        // Se ejecuta al crear la ventana.
        public Form1()
        {
            InitializeComponent();   // Prepara la ventana vacía (viene del Designer)
            ConstruirInterfaz();     // Crea todos los controles de la biblioteca
        }

        // Se ejecuta cuando la ventana ya está abierta
        private void Form1_Load(object sender, EventArgs e)
        {
            CargarDatos();   // Pone libros y usuarios de ejemplo
            RefrescarTodo(); // Muestra todo en pantalla
        }

        // Datos de ejemplo con los que arranca el programa
        private void CargarDatos()
        {
            libros.Add(new Libro(1, "Cien años de soledad", "Gabriel García Márquez"));
            libros.Add(new Libro(2, "Don Quijote de la Mancha", "Miguel de Cervantes"));
            libros.Add(new Libro(3, "El principito", "Antoine de Saint-Exupéry"));
            libros.Add(new Libro(4, "1984", "George Orwell"));
            libros.Add(new Libro(5, "La Odisea", "Homero"));
            libros.Add(new Libro(6, "La divina comedia", "Dante Alighieri"));

            usuarios.Add(new Usuario(1, "Carlos"));
            usuarios.Add(new Usuario(2, "María"));
            usuarios.Add(new Usuario(3, "Juan"));
        }

        // =====================================================
        // 6. CONSTRUCCIÓN DE LA INTERFAZ (crea todos los controles)
        // =====================================================

        private void ConstruirInterfaz()
        {
            // Propiedades de la ventana
            Text = NombreBiblioteca;                    // Título de la ventana
            Size = new Size(1000, 650);                 // Tamaño inicial
            MinimumSize = new Size(900, 550);           // Tamaño mínimo
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = colorPergamino;
            Load += Form1_Load;                         // Conecta el evento de carga

            // Pestañas: ocupan todo el centro de la ventana.
            // Se agregan primero para que el resto se acomode a su alrededor.
            tabs = new TabControl();
            tabs.Dock = DockStyle.Fill;
            tabs.Font = new Font("Segoe UI", 10);
            tabs.TabPages.Add(CrearTabLibros());
            tabs.TabPages.Add(CrearTabUsuarios());
            tabs.TabPages.Add(CrearTabPrestamos());
            Controls.Add(tabs);

            // Barra de estado (abajo)
            barraEstado = new StatusStrip();
            lblEstado = new ToolStripStatusLabel();
            barraEstado.Items.Add(lblEstado);
            Controls.Add(barraEstado);

            // Encabezado con el nombre de la biblioteca (arriba)
            Panel header = new Panel();
            header.Dock = DockStyle.Top;
            header.Height = 70;
            header.BackColor = colorMadera;

            Label titulo = new Label();
            titulo.Text = NombreBiblioteca;
            titulo.ForeColor = colorOro;
            titulo.Font = new Font("Georgia", 22, FontStyle.Bold);
            titulo.Dock = DockStyle.Fill;
            titulo.TextAlign = ContentAlignment.MiddleCenter;
            header.Controls.Add(titulo);

            Controls.Add(header);
        }

        // ---------- Pestaña LIBROS ----------
        private TabPage CrearTabLibros()
        {
            TabPage tab = new TabPage("Libros");
            tab.BackColor = colorPergamino;

            // La tabla ocupa el espacio sobrante
            dgvLibros = CrearGrid();
            tab.Controls.Add(dgvLibros);

            // Panel superior con los controles
            Panel panel = new Panel();
            panel.Dock = DockStyle.Top;
            panel.Height = 100;

            // Fila 1: buscador
            panel.Controls.Add(CrearLabel("Buscar:", 10, 14));
            txtBuscar = new TextBox { Left = 70, Top = 10, Width = 250 };
            // Cada vez que se escribe algo, se vuelve a filtrar la tabla
            txtBuscar.TextChanged += (s, e) => RefrescarLibros();
            panel.Controls.Add(txtBuscar);

            // Fila 2: formulario para agregar libro
            panel.Controls.Add(CrearLabel("Título:", 10, 54));
            txtTitulo = new TextBox { Left = 70, Top = 50, Width = 220 };
            panel.Controls.Add(txtTitulo);

            panel.Controls.Add(CrearLabel("Autor:", 305, 54));
            txtAutor = new TextBox { Left = 360, Top = 50, Width = 200 };
            panel.Controls.Add(txtAutor);

            Button btnAgregar = CrearBoton("Agregar libro", 575, 48, 130);
            btnAgregar.Click += btnAgregarLibro_Click;
            panel.Controls.Add(btnAgregar);

            Button btnEliminar = CrearBoton("Eliminar seleccionado", 715, 48, 180);
            btnEliminar.Click += btnEliminarLibro_Click;
            panel.Controls.Add(btnEliminar);

            tab.Controls.Add(panel);
            return tab;
        }

        // ---------- Pestaña USUARIOS ----------
        private TabPage CrearTabUsuarios()
        {
            TabPage tab = new TabPage("Usuarios");
            tab.BackColor = colorPergamino;

            dgvUsuarios = CrearGrid();
            tab.Controls.Add(dgvUsuarios);

            Panel panel = new Panel();
            panel.Dock = DockStyle.Top;
            panel.Height = 55;

            panel.Controls.Add(CrearLabel("Nombre:", 10, 18));
            txtNombreUsuario = new TextBox { Left = 80, Top = 14, Width = 250 };
            panel.Controls.Add(txtNombreUsuario);

            Button btnAgregar = CrearBoton("Registrar usuario", 345, 12, 160);
            btnAgregar.Click += btnAgregarUsuario_Click;
            panel.Controls.Add(btnAgregar);

            tab.Controls.Add(panel);
            return tab;
        }

        // ---------- Pestaña PRÉSTAMOS ----------
        private TabPage CrearTabPrestamos()
        {
            TabPage tab = new TabPage("Préstamos");
            tab.BackColor = colorPergamino;

            dgvPrestamos = CrearGrid();
            tab.Controls.Add(dgvPrestamos);

            Panel panel = new Panel();
            panel.Dock = DockStyle.Top;
            panel.Height = 100;

            // Lista desplegable con los libros disponibles
            panel.Controls.Add(CrearLabel("Libro:", 10, 14));
            cmbLibro = new ComboBox
            {
                Left = 70,
                Top = 10,
                Width = 280,
                DropDownStyle = ComboBoxStyle.DropDownList // Solo elegir, no escribir
            };
            panel.Controls.Add(cmbLibro);

            // Lista desplegable con los usuarios
            panel.Controls.Add(CrearLabel("Usuario:", 370, 14));
            cmbUsuario = new ComboBox
            {
                Left = 440,
                Top = 10,
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            panel.Controls.Add(cmbUsuario);

            Button btnPrestar = CrearBoton("Prestar libro", 655, 8, 140);
            btnPrestar.Click += btnPrestar_Click;
            panel.Controls.Add(btnPrestar);

            // Texto informativo con las reglas
            panel.Controls.Add(CrearLabel(
                "Plazo: " + DiasPrestamo + " días · Máximo " +
                MaxLibrosPorUsuario + " libros por usuario",
                10, 58));

            Button btnDevolver = CrearBoton("Devolver seleccionado", 655, 52, 190);
            btnDevolver.Click += btnDevolver_Click;
            panel.Controls.Add(btnDevolver);

            tab.Controls.Add(panel);
            return tab;
        }

        // ---------- Métodos ayudantes para no repetir código ----------

        // Crea una tabla (DataGridView) con el estilo de la biblioteca
        private DataGridView CrearGrid()
        {
            DataGridView dgv = new DataGridView();
            dgv.Dock = DockStyle.Fill;
            dgv.ReadOnly = true;                         // No se edita a mano
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect; // Selecciona fila completa
            dgv.MultiSelect = false;                     // Una fila a la vez
            dgv.RowHeadersVisible = false;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; // Columnas se ajustan
            dgv.BackgroundColor = Color.White;
            dgv.EnableHeadersVisualStyles = false;       // Permite colores propios en el encabezado
            dgv.ColumnHeadersDefaultCellStyle.BackColor = colorMadera;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = colorOro;
            dgv.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 10, FontStyle.Bold);
            return dgv;
        }

        // Crea una etiqueta de texto en la posición (x, y)
        private Label CrearLabel(string texto, int x, int y)
        {
            Label l = new Label();
            l.Text = texto;
            l.Left = x;
            l.Top = y;
            l.AutoSize = true;
            return l;
        }

        // Crea un botón con el estilo de la biblioteca
        private Button CrearBoton(string texto, int x, int y, int ancho)
        {
            Button b = new Button();
            b.Text = texto;
            b.Left = x;
            b.Top = y;
            b.Width = ancho;
            b.Height = 32;
            b.BackColor = colorMadera;
            b.ForeColor = colorOro;
            b.FlatStyle = FlatStyle.Flat;
            b.Cursor = Cursors.Hand;
            return b;
        }

        // =====================================================
        // 7. REFRESCAR LO QUE SE VE EN PANTALLA
        // =====================================================

        // Actualiza todo de una vez (se llama después de cada cambio)
        private void RefrescarTodo()
        {
            RefrescarLibros();
            RefrescarUsuarios();
            RefrescarPrestamos();
            RefrescarCombos();
            ActualizarEstado();
        }

        // Llena la tabla de libros (aplicando el filtro del buscador)
        private void RefrescarLibros()
        {
            string filtro = txtBuscar.Text.Trim().ToLower();

            dgvLibros.DataSource = libros
                // Deja solo los libros que coinciden con el buscador
                .Where(l => filtro == "" ||
                            l.Titulo.ToLower().Contains(filtro) ||
                            l.Autor.ToLower().Contains(filtro))
                // Elige qué columnas mostrar
                .Select(l => new
                {
                    ID = l.Id,
                    Título = l.Titulo,
                    Autor = l.Autor,
                    Estado = l.Disponible ? "Disponible" : "Prestado"
                })
                .ToList();
        }

        // Llena la tabla de usuarios, con cuántos libros tiene cada uno
        private void RefrescarUsuarios()
        {
            dgvUsuarios.DataSource = usuarios
                .Select(u => new
                {
                    ID = u.Id,
                    Nombre = u.Nombre,
                    LibrosPrestados = prestamos.Count(p => p.Usuario.Id == u.Id)
                })
                .ToList();
        }

        // Llena la tabla de préstamos activos
        private void RefrescarPrestamos()
        {
            dgvPrestamos.DataSource = prestamos
                .Select(p => new
                {
                    IdLibro = p.Libro.Id,
                    Libro = p.Libro.Titulo,
                    Usuario = p.Usuario.Nombre,
                    Fecha = p.Fecha.ToString("dd/MM/yyyy HH:mm"),
                    Límite = p.FechaLimite.ToString("dd/MM/yyyy"),
                    // Si hoy es después de la fecha límite, está vencido
                    Estado = DateTime.Now > p.FechaLimite ? "VENCIDO" : "En plazo"
                })
                .ToList();
        }

        // Actualiza las listas desplegables de la pestaña Préstamos
        private void RefrescarCombos()
        {
            // Solo muestra libros que estén disponibles
            cmbLibro.DataSource = null;
            cmbLibro.DataSource = libros.Where(l => l.Disponible).ToList();
            cmbLibro.DisplayMember = "Titulo"; // Propiedad que se muestra como texto

            cmbUsuario.DataSource = null;
            cmbUsuario.DataSource = usuarios.ToList();
            cmbUsuario.DisplayMember = "Nombre";
        }

        // Escribe las estadísticas en la barra inferior
        private void ActualizarEstado()
        {
            int disponibles = libros.Count(l => l.Disponible);
            int vencidos = prestamos.Count(p => DateTime.Now > p.FechaLimite);

            lblEstado.Text =
                "Libros: " + libros.Count +
                "  |  Disponibles: " + disponibles +
                "  |  Prestados: " + prestamos.Count +
                "  |  Vencidos: " + vencidos +
                "  |  Usuarios: " + usuarios.Count;
        }

        // =====================================================
        // 8. EVENTOS: LIBROS
        // =====================================================

        // Botón "Agregar libro"
        private void btnAgregarLibro_Click(object sender, EventArgs e)
        {
            string titulo = txtTitulo.Text.Trim();
            string autor = txtAutor.Text.Trim();

            // Validación: no se permiten campos vacíos
            if (titulo == "" || autor == "")
            {
                MessageBox.Show("Ingrese el título y el autor del libro.");
                return;
            }

            // Nuevo ID = el ID más alto + 1
            int nuevoId = libros.Count == 0 ? 1 : libros.Max(l => l.Id) + 1;
            libros.Add(new Libro(nuevoId, titulo, autor));

            txtTitulo.Clear();
            txtAutor.Clear();

            RefrescarTodo();
        }

        // Botón "Eliminar seleccionado"
        private void btnEliminarLibro_Click(object sender, EventArgs e)
        {
            // Comprueba que haya una fila seleccionada
            if (dgvLibros.CurrentRow == null)
            {
                MessageBox.Show("Seleccione un libro de la lista.");
                return;
            }

            // Lee el ID de la fila seleccionada y busca ese libro
            int id = (int)dgvLibros.CurrentRow.Cells["ID"].Value;
            Libro libro = libros.FirstOrDefault(l => l.Id == id);

            if (libro == null) return;

            // No se puede borrar un libro que alguien tiene prestado
            if (!libro.Disponible)
            {
                MessageBox.Show("No se puede eliminar un libro que está prestado.");
                return;
            }

            // Pide confirmación antes de borrar
            DialogResult r = MessageBox.Show(
                "¿Eliminar \"" + libro.Titulo + "\"?",
                "Confirmar",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (r == DialogResult.Yes)
            {
                libros.Remove(libro);
                RefrescarTodo();
            }
        }

        // =====================================================
        // 9. EVENTOS: USUARIOS
        // =====================================================

        // Botón "Registrar usuario"
        private void btnAgregarUsuario_Click(object sender, EventArgs e)
        {
            string nombre = txtNombreUsuario.Text.Trim();

            if (nombre == "")
            {
                MessageBox.Show("Ingrese el nombre del usuario.");
                return;
            }

            int nuevoId = usuarios.Count == 0 ? 1 : usuarios.Max(u => u.Id) + 1;
            usuarios.Add(new Usuario(nuevoId, nombre));

            txtNombreUsuario.Clear();
            RefrescarTodo();
        }

        // =====================================================
        // 10. EVENTOS: PRÉSTAMOS
        // =====================================================

        // Botón "Prestar libro"
        private void btnPrestar_Click(object sender, EventArgs e)
        {
            // Obtiene lo que el usuario eligió en las listas desplegables
            Libro libro = cmbLibro.SelectedItem as Libro;
            Usuario usuario = cmbUsuario.SelectedItem as Usuario;

            if (libro == null)
            {
                MessageBox.Show("No hay libros disponibles para prestar.");
                return;
            }

            if (usuario == null)
            {
                MessageBox.Show("Seleccione un usuario.");
                return;
            }

            // Regla: máximo de libros por usuario
            int cantidad = prestamos.Count(p => p.Usuario.Id == usuario.Id);

            if (cantidad >= MaxLibrosPorUsuario)
            {
                MessageBox.Show(
                    usuario.Nombre + " ya tiene " + MaxLibrosPorUsuario +
                    " libros prestados (máximo permitido).");
                return;
            }

            // Crea el préstamo y marca el libro como no disponible
            Prestamo nuevo = new Prestamo(libro, usuario);
            prestamos.Add(nuevo);
            libro.Disponible = false;

            MessageBox.Show(
                "Préstamo realizado correctamente.\n\n" +
                "Libro: " + libro.Titulo + "\n" +
                "Usuario: " + usuario.Nombre + "\n" +
                "Devolver antes del: " + nuevo.FechaLimite.ToString("dd/MM/yyyy"));

            RefrescarTodo();
        }

        // Botón "Devolver seleccionado"
        private void btnDevolver_Click(object sender, EventArgs e)
        {
            if (dgvPrestamos.CurrentRow == null)
            {
                MessageBox.Show("Seleccione un préstamo de la lista.");
                return;
            }

            // Lee el ID del libro de la fila seleccionada y busca su préstamo
            int idLibro = (int)dgvPrestamos.CurrentRow.Cells["IdLibro"].Value;

            Prestamo prestamo = prestamos.FirstOrDefault(p => p.Libro.Id == idLibro);

            if (prestamo == null) return;

            string mensaje = "Libro devuelto correctamente.\n\nLibro: " +
                             prestamo.Libro.Titulo;

            // Si se pasó de la fecha límite, avisa cuántos días de retraso
            if (DateTime.Now > prestamo.FechaLimite)
            {
                int diasTarde = (DateTime.Now - prestamo.FechaLimite).Days;
                mensaje += "\n\nDevolución con " + diasTarde + " día(s) de retraso.";
            }

            // El libro vuelve a estar disponible y se elimina el préstamo
            prestamo.Libro.Disponible = true;
            prestamos.Remove(prestamo);

            MessageBox.Show(mensaje);
            RefrescarTodo();
        }
    }
}