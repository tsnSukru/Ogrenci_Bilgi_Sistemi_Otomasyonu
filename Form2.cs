using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;//veritabanına bağlantı için burayı unutma

namespace Hafta7
{
    public partial class Form2 : Form
    {
        OleDbConnection conn = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + Application.StartupPath + "\\ogrenciler.mdb");
        DataSet ds = new DataSet();
        BindingSource bs = new BindingSource();

        int kacincikayit;
        bool yenikayitmi;
        public Form2()
        {
            InitializeComponent();
        }

        void verileri_cek()
        {
            string sec = "select a.*,b.badi from anabilimdali as a, bolumler as b where a.bkodu=b.bkodu"; // "as" önceki şeyin kısaltmasını yapabilmeni sağlar yani bolumlerin kısaltmsı b'dir
            OleDbDataAdapter da = new OleDbDataAdapter(sec, conn);
            if (ds.Tables["anabilimdali"] != null) ds.Tables["anabilimdali"].Clear();
            da.Fill(ds, "anabilimdali");

        }
        void ilerigerikontrol(object sender, EventArgs e)
        {
            if (bs.Position == 0) btngeri.Enabled = false;
            else btngeri.Enabled = true;
            if (bs.Position == bs.Count - 1) btnileri.Enabled = false;
            else btnileri.Enabled = true;
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            badi.Enabled = abdadi.Enabled = abdadi.Enabled = false;
            btnkaydet.Visible = btniptal.Visible = false;
            btngeri.Enabled = false;
            if (conn.State == ConnectionState.Closed) conn.Open();

            verileri_cek();

            bs.DataSource = ds.Tables["anabilimdali"];
            dataGridView1.DataSource = bs;
            abdkodu.DataBindings.Add("Text", bs, "abdkodu");
            abdadi.DataBindings.Add("Text", bs, "abdadi");
            badi.DataBindings.Add("SelectedValue", bs, "bkodu");

            string sec = "select * from bolumler";
            OleDbDataAdapter da = new OleDbDataAdapter(sec, conn);
            da.Fill(ds, "bolumler"); da.Fill(ds, "bolumler1");

            badi.DataSource = ds.Tables["bolumler"];
            badi.DisplayMember = "badi"; //fiziksel tablodaki badi field ı
            badi.ValueMember = "bkodu"; //fiziksel tablodaki bkodu field ı
            badiara.DataSource = ds.Tables["bolumler1"];
            badiara.DisplayMember = "badi";
            badiara.ValueMember = "bkodu";

            checkBox1.Checked = true;
            bs.PositionChanged += new EventHandler(ilerigerikontrol);
            abdkodu.Enabled = false;
        }

        private void btnyeni_Click(object sender, EventArgs e)
        {
            badi.Enabled = abdadi.Enabled = true;
            abdadi.Clear();
            abdadi.Focus();
            btnkaydet.Visible = btniptal.Visible = true;
            kacincikayit = ds.Tables["anabilimdali"].Rows.Count;
            yenikayitmi = true;
        }

        private void btnkaydet_Click(object sender, EventArgs e)
        {
            if (abdadi.Text != "")
            {
                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = conn;
                if (yenikayitmi)//yenikayitmi true'ysa çalışır
                {
                    cmd.CommandText = "insert into anabilimdali (abdadi,bkodu) Values (@abdadi,@bkodu)";
                    cmd.Parameters.AddWithValue("@abdadi", abdadi.Text);
                    cmd.Parameters.AddWithValue("@bkodu", badi.SelectedValue);
                }
                else
                {
                    cmd.CommandText = "update anabilimdali set abdadi=@abdadi,bkodu=@bkodu where abdkodu=@abdkodu";
                    cmd.Parameters.AddWithValue("@abdadi", abdadi.Text);
                    cmd.Parameters.AddWithValue("@bkodu", badi.SelectedValue);
                    cmd.Parameters.AddWithValue("@abdkodu", abdkodu.Text);
                }

                cmd.ExecuteNonQuery(); //üstteki komutları çalıştırır
                MessageBox.Show("İşlem Gerçekleştirildi!");
                verileri_cek();
                bs.Position = kacincikayit;

                abdadi.Enabled = badi.Enabled = false;
                btnkaydet.Visible = btniptal.Visible = false;
            }
            else MessageBox.Show("Lütfen Anabilimdalı Adını Giriniz");
        }

        private void btnduzelt_Click(object sender, EventArgs e)
        {
            badi.Enabled = abdadi.Enabled = true;
            abdadi.Focus();
            btnkaydet.Visible = btniptal.Visible = true;
            yenikayitmi = false;
            kacincikayit = bs.Position;
        }

        private void btnsil_Click(object sender, EventArgs e)
        {
            kacincikayit = bs.Position;
            if (kacincikayit > -1)
            {
                DialogResult cevap = MessageBox.Show("Kaydı Silmek İstiyormusunuz?", "Uyarı", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (cevap == DialogResult.Yes)
                {
                    OleDbCommand cmd = new OleDbCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = "delete from anabilimdali where abdkodu=@abdkodu";
                    cmd.Parameters.AddWithValue("@abdkodu", abdkodu.Text);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Kayıt Silindi!");
                    verileri_cek();
                    bs.Position = kacincikayit;
                }
            }
            else MessageBox.Show("Lütfen Silinecek Kaydı Seçiniz");
        }

        private void btnileri_Click(object sender, EventArgs e)
        {
            bs.Position++;
        }

        private void btngeri_Click(object sender, EventArgs e)
        {
            bs.Position--;
        }

        private void aranan_TextChanged(object sender, EventArgs e)
        {
            string sec = "select a.*,b.badi from anabilimdali as a, bolumler as b where a.bkodu=b.bkodu and abdadi like'%" + aranan.Text + "%'";//and iki tane koşul için
            OleDbDataAdapter da = new OleDbDataAdapter(sec, conn);
            if (ds.Tables["anabilimdali"] != null) ds.Tables["anabilimdali"].Clear();
            da.Fill(ds, "anabilimdali");
        }

        private void badiara_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string sec = "select a.*,b.badi from anabilimdali as a, bolumler as b where a.bkodu=b.bkodu and a.bkodu=" + badiara.SelectedValue;
                OleDbDataAdapter da = new OleDbDataAdapter(sec, conn);
                if (ds.Tables["anabilimdali"] != null) ds.Tables["anabilimdali"].Clear();
                da.Fill(ds, "anabilimdali");
            }
            catch
            {

            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked) verileri_cek();
            else
            {
                if (aranan.Text != "") aranan_TextChanged(sender, e);
                else
                {
                    badiara_SelectedIndexChanged(sender, e);
                }
            }
        }
    }
}
