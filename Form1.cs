using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;

namespace Hafta7
{
    public partial class Form1 : Form
    {
        OleDbConnection conn = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + Application.StartupPath + "\\ogrenciler.mdb");
        DataSet ds = new DataSet();
        BindingSource bs = new BindingSource();
        int kacincikayit;
        bool yenikayitmi;
        public Form1()
        {
            InitializeComponent();
        }
        void verileri_cek()
        {
            string sec = "select * from bolumler";
            OleDbDataAdapter da = new OleDbDataAdapter(sec, conn);
            if (ds.Tables["bolumler"] != null) ds.Tables["bolumler"].Clear();
            da.Fill(ds, "bolumler");
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            tbbadi.ReadOnly = tbbkodu.ReadOnly = true;//başlangıçta sadece okunabilir yapıyor
            btnkaydet.Visible = btniptal.Visible = false;
            if (conn.State == ConnectionState.Closed) conn.Open();

            verileri_cek();

            bs.DataSource = ds.Tables["bolumler"];
            dataGridView1.DataSource = bs;
            tbbkodu.DataBindings.Add("Text", bs, "bkodu");
            tbbadi.DataBindings.Add("Text", bs, "badi");
        }

        private void btnyeni_Click(object sender, EventArgs e)
        {
            tbbadi.ReadOnly = false;
            tbbadi.Clear(); tbbkodu.Clear();
            tbbadi.Focus();
            btnkaydet.Visible = btniptal.Visible = true;
            kacincikayit = ds.Tables["bolumler"].Rows.Count;
            yenikayitmi = true;
        }

        private void btnkaydet_Click(object sender, EventArgs e)
        {
            if (tbbadi.Text != "")
            {
                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = conn;
                if (yenikayitmi)
                {
                    cmd.CommandText = "insert into bolumler (badi) Values (@badi)";
                    cmd.Parameters.AddWithValue("@badi", tbbadi.Text);
                }
                else
                {
                    cmd.CommandText = "update bolumler set badi=@badi where bkodu=@bkodu";//sadece bkodu=bkodu olanları güncelle demek
                    cmd.Parameters.AddWithValue("@badi", tbbadi.Text);
                    cmd.Parameters.AddWithValue("@bkodu", tbbkodu.Text);
                }

                cmd.ExecuteNonQuery(); //üstteki komutları çalıştırır
                MessageBox.Show("İşlem Gerçekleştirildi!");
                verileri_cek();
                bs.Position = kacincikayit;

                tbbadi.ReadOnly = true;
                btnkaydet.Visible = btniptal.Visible = false;
            }
            else MessageBox.Show("Lütfen Bölüm Adı Giriniz");
        }

        private void btniptal_Click(object sender, EventArgs e)
        {
            btnkaydet.Visible = btniptal.Visible = false;
        }

        private void btnileri_Click(object sender, EventArgs e)
        {
            if (++bs.Position == ds.Tables["bolumler"].Rows.Count - 1)
            {
                btnileri.Enabled = false;
            }
            btngeri.Enabled = true;
        }
        private void btngeri_Click(object sender, EventArgs e)
        {
            if (--bs.Position <= 0)
            {
                btngeri.Enabled = false;
            }
            btnileri.Enabled = true;
        }

        private void btnduzelt_Click(object sender, EventArgs e)
        {
            tbbadi.ReadOnly = false;
            tbbadi.Focus();
            btnkaydet.Visible = btniptal.Visible = true;
            //kacincikayit = ds.Tables["bolumler"].Rows.Count;
            yenikayitmi = false;
            kacincikayit = bs.Position;
        }

        private void btnsil_Click(object sender, EventArgs e)
        {
            kacincikayit = bs.Position;
            DialogResult cevap = MessageBox.Show("Kaydı Silmek İstiyormusunuz?", "Uyarı", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (cevap == DialogResult.Yes)
            {
                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = conn;
                cmd.CommandText = "delete from bolumler where bkodu=@bkodu";
                cmd.Parameters.AddWithValue("@bkodu", tbbkodu.Text);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Kaydınız Silindi!");
                verileri_cek();
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            conn.Close();
        }

        private void aranan_TextChanged(object sender, EventArgs e)
        {
            string sec = "select * from bolumler where badi like'%" + aranan.Text + "%'";//benzeyenleri ara
            OleDbDataAdapter da = new OleDbDataAdapter(sec, conn);
            if (ds.Tables["bolumler"] != null) ds.Tables["bolumler"].Clear();
            da.Fill(ds, "bolumler");
        }

        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete) btnsil_Click(sender, e);
        }
    }
}
