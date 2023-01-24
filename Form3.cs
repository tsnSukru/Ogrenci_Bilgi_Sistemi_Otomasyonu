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
    public partial class Form3 : Form
    {
        OleDbConnection conn = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + Application.StartupPath + "\\ogrenciler.mdb");
        DataSet ds = new DataSet();
        BindingSource bs = new BindingSource();

        int kacincikayit;
        string düzeltileceknumara;
        public Form3()
        {
            InitializeComponent();
        }
        void verileri_cek()
        {
            string sec = "select g.*, a.abdadi, b.badi from genel as g, anabilimdali as a, bolumler as b where g.bkodu=b.bkodu and g.abdkodu=a.abdkodu";
            OleDbDataAdapter da = new OleDbDataAdapter(sec, conn);
            if (ds.Tables["genel"] != null) ds.Tables["genel"].Clear();
            da.Fill(ds, "genel");
        }
        bool kayit_varmi(string aranannumara)
        {
            string sec = "select * from genel where numara='" + aranannumara + "'";
            OleDbDataAdapter da = new OleDbDataAdapter(sec, conn);
            if (ds.Tables["arama"] != null) ds.Tables["arama"].Clear();
            da.Fill(ds, "arama");
            if (ds.Tables["arama"].Rows.Count <= 0) return false;
            else return true;
        }
        void veritabanina_bagla()
        {
            tbadi.DataBindings.Add("Text", bs, "adi");
            tbsoyadi.DataBindings.Add("Text", bs, "soyadi");
            tbnumara.DataBindings.Add("Text", bs, "numara");
            tbdyeri.DataBindings.Add("Text", bs, "dyeri");
            cbbadi.DataBindings.Add("SelectedValue", bs, "bkodu");
            cbabdadi.DataBindings.Add("SelectedValue", bs, "abdkodu");
            cbdtarihi.DataBindings.Add("Text", bs, "dtarihi");
            cbcinsiyet.DataBindings.Add("SelectedItem", bs, "cinsiyet");
            pictureBox1.DataBindings.Add("ImageLocation", bs, "resim");
        }
        void ilerigerikontrol(object sender, EventArgs e)
        {
            if (bs.Position == 0) btngeri.Enabled = false;
            else btngeri.Enabled = true;
            if (bs.Position == bs.Count - 1) btnileri.Enabled = false;
            else btnileri.Enabled = true;
        }
        private void Form3_Load(object sender, EventArgs e)
        {
            if (conn.State == ConnectionState.Closed) conn.Open();

            string seckomutu = "select * from bolumler";
            OleDbDataAdapter da = new OleDbDataAdapter(seckomutu, conn);
            da.Fill(ds, "bolumler"); da.Fill(ds, "bolumler1"); //arama comboboxı içinde ayrı bi tane dolduruyoruz
            cbbadi.DataSource = ds.Tables["bolumler"];
            cbbadi.DisplayMember = "badi";
            cbbadi.ValueMember = "bkodu";
            cbbadiara.DataSource = ds.Tables["bolumler1"];
            cbbadiara.DisplayMember = "badi";
            cbbadiara.ValueMember = "bkodu";

            cbbadi_SelectedIndexChanged(sender, e);

            btnkaydet.Visible = btniptal.Visible = false;

            verileri_cek();
            bs.DataSource = ds.Tables["genel"];
            dataGridView1.DataSource = bs;
            veritabanina_bagla();

            ilerigerikontrol(sender, e);
            bs.PositionChanged += new EventHandler(ilerigerikontrol);

            dataGridView1_SelectionChanged(sender, e);

            checkBox1.Checked = true;
        }

        private void cbbadi_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string seckomutu = "select * from anabilimdali where bkodu=" + cbbadi.SelectedValue;
                OleDbDataAdapter da = new OleDbDataAdapter(seckomutu, conn);
                if (ds.Tables["anabilimdali"] != null) ds.Tables["anabilimdali"].Clear();
                da.Fill(ds, "anabilimdali");
                cbabdadi.DataSource = ds.Tables["anabilimdali"];
                cbabdadi.ValueMember = "abdkodu";
                cbabdadi.DisplayMember = "abdadi";
            }
            catch
            {

            }
        }

        private void btnyeni_Click(object sender, EventArgs e)
        {
            btnkaydet.Visible = btniptal.Visible = true;
            tbadi.Clear(); tbsoyadi.Clear(); tbnumara.Clear(); tbdyeri.Clear(); pictureBox1.ImageLocation = "";
            cbcinsiyet.SelectedIndex = 0;
            tbnumara.Focus();
            kacincikayit = ds.Tables["genel"].Rows.Count;

        }

        private void btnresimekle_Click(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory = Application.StartupPath + "\\resim\\";
            DialogResult sonuc = openFileDialog1.ShowDialog();
            if (sonuc == DialogResult.OK)
            {
                pictureBox1.ImageLocation = openFileDialog1.FileName;
            }
        }

        private void btnkaydet_Click(object sender, EventArgs e)
        {
            if (!kayit_varmi(tbnumara.Text))
            {
                try
                {
                    OleDbCommand cmd = new OleDbCommand();
                    cmd.Connection = conn;

                    cmd.CommandText = "insert into genel (numara,adi,soyadi,dtarihi,dyeri,cinsiyet,resim,abdkodu,bkodu) Values (@numara,@adi,@soyadi,@dtarihi,@dyeri,@cinsiyet,@resim,@abdkodu,@bkodu)";
                    cmd.Parameters.AddWithValue("@numara", tbnumara.Text);//birinci parametre veritabanındaki yeri, ikincisi ise o yere ne yazılacağı
                    cmd.Parameters.AddWithValue("@adi", tbadi.Text);
                    cmd.Parameters.AddWithValue("@soyadi", tbsoyadi.Text);
                    cmd.Parameters.AddWithValue("@dtarihi", cbdtarihi.Text);
                    cmd.Parameters.AddWithValue("@dyeri", tbdyeri.Text);
                    cmd.Parameters.AddWithValue("@cinsiyet", cbcinsiyet.SelectedItem);
                    cmd.Parameters.AddWithValue("@resim", pictureBox1.ImageLocation);
                    cmd.Parameters.AddWithValue("@abdkodu", cbabdadi.SelectedValue);
                    cmd.Parameters.AddWithValue("@bkodu", cbbadi.SelectedValue);

                    cmd.ExecuteNonQuery(); //üstteki komutları çalıştırır

                    verileri_cek();
                    bs.Position = kacincikayit;

                    MessageBox.Show("Kayıt Yapıldı!");

                    btnkaydet.Visible = btniptal.Visible = false;
                }
                catch
                {
                    MessageBox.Show("Bilgileri Eksiksiz Giriniz");
                }
            }
            else MessageBox.Show(tbnumara.Text + " Numaralı Öğrenci Zaten Kayıtlı");
        }

        private void btnduzelt_Click(object sender, EventArgs e)
        {
            kacincikayit = bs.Position;
            if (kacincikayit > -1)
            {
                try
                {
                    if (kayit_varmi(tbnumara.Text) & tbnumara.Text != düzeltileceknumara)
                    {
                        MessageBox.Show(tbnumara.Text + " Numaralı Kayıt Zaten Var");
                        return;
                    }
                    int kayitno = bs.Position;
                    OleDbCommand cmd = new OleDbCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = "update genel set numara=@numara,adi=@adi,soyadi=@soyadi,dtarihi=@dtarihi,dyeri=@dyeri,cinsiyet=@cinsiyet,resim=@resim,abdkodu=@abdkodu,bkodu=@bkodu where numara=@tutnumara";
                    cmd.Parameters.AddWithValue("@numara", tbnumara.Text);
                    cmd.Parameters.AddWithValue("@adi", tbadi.Text);
                    cmd.Parameters.AddWithValue("@soyadi", tbsoyadi.Text);
                    cmd.Parameters.AddWithValue("@dtarihi", cbdtarihi.Text);
                    cmd.Parameters.AddWithValue("@dyeri", tbdyeri.Text);
                    cmd.Parameters.AddWithValue("@cinsiyet", cbcinsiyet.SelectedItem);
                    cmd.Parameters.AddWithValue("@resim", pictureBox1.ImageLocation);
                    cmd.Parameters.AddWithValue("@abdkodu", cbabdadi.SelectedValue);
                    cmd.Parameters.AddWithValue("@bkodu", cbbadi.SelectedValue);
                    cmd.Parameters.AddWithValue("@tutnumara", düzeltileceknumara);
                    cmd.ExecuteNonQuery();
                    verileri_cek();
                    bs.Position = kayitno;
                    MessageBox.Show("Kaydınız Düzeltildi!");
                }
                catch
                {
                    MessageBox.Show("Bilgileri Eksiksiz Giriniz");
                }
            }
            else MessageBox.Show("Düzeltilecek Kaydı Seçin");
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
                    cmd.CommandText = "delete from genel where numara=@numara";
                    cmd.Parameters.AddWithValue("@numara", tbnumara.Text);
                    cmd.ExecuteNonQuery();
                    verileri_cek();
                    bs.Position = kacincikayit;
                    MessageBox.Show("Kayıt Silindi!");
                }
            }
            else MessageBox.Show("Silinecek Kayıt Yok!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void btnileri_Click(object sender, EventArgs e)
        {
            bs.Position++;
        }

        private void btngeri_Click(object sender, EventArgs e)
        {
            bs.Position--;
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            düzeltileceknumara = tbnumara.Text;
        }

        private void cbbadiara_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string sec = "select g.*,a.abdadi,b.badi from genel as g,anabilimdali as a, bolumler as b where g.bkodu=b.bkodu and g.abdkodu=a.abdkodu and g.bkodu=" + cbbadiara.SelectedValue;
                OleDbDataAdapter da = new OleDbDataAdapter(sec, conn);
                if (ds.Tables["genel"] != null) ds.Tables["genel"].Clear();
                da.Fill(ds, "genel");
            }
            catch { }
            checkBox1.Checked = false;
        }

        private void tbnumaraara_TextChanged(object sender, EventArgs e)
        {
            string sec = "select g.*, a.abdadi, b.badi from genel as g, anabilimdali as a, bolumler as b where g.bkodu=b.bkodu and g.abdkodu=a.abdkodu and numara like '%" + tbnumara.Text + "%'";
            OleDbDataAdapter da = new OleDbDataAdapter(sec, conn);
            if (ds.Tables["genel"] != null) ds.Tables["genel"].Clear();
            da.Fill(ds, "genel");
            checkBox1.Checked = false;
        }

        private void tbadiara_TextChanged(object sender, EventArgs e)
        {
            string sec = "select g.*,a.abdadi,b.badi from genel as g,anabilimdali as a, bolumler as b where g.bkodu=b.bkodu and g.abdkodu=a.abdkodu and adi like '%" + tbadiara.Text + "%'";
            OleDbDataAdapter da = new OleDbDataAdapter(sec, conn);
            if (ds.Tables["genel"] != null) ds.Tables["genel"].Clear();
            da.Fill(ds, "genel");
            checkBox1.Checked = false;
        }

        private void tbsoyadiara_TextChanged(object sender, EventArgs e)
        {
            string sec = "select g.*,a.abdadi,b.badi from genel as g,anabilimdali as a, bolumler as b where g.bkodu=b.bkodu and g.abdkodu=a.abdkodu and soyadi like '%" + tbsoyadiara.Text + "%'";
            OleDbDataAdapter da = new OleDbDataAdapter(sec, conn);
            if (ds.Tables["genel"] != null) ds.Tables["genel"].Clear();
            da.Fill(ds, "genel");
            checkBox1.Checked = false;
        }

        private void checkBox1_Click(object sender, EventArgs e)
        {
            if (checkBox1.Checked) verileri_cek();
        }
    }
}
