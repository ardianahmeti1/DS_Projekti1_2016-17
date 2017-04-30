using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;


namespace DS_Projekti1_EnkriptimIFjalekalimit
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();

        }


        public static string Salti()
        {
            char[] karakteret = new char[62];
            karakteret =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();

            byte[] data = new byte[1];
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {

                data = new byte[12];
                crypto.GetNonZeroBytes(data);
            }
            StringBuilder result = new StringBuilder(12);
            foreach (byte b in data)
            {
                result.Append(karakteret[b % (karakteret.Length)]);
            }
            return result.ToString();
        }

        private void btnRuaj_Click(object sender, EventArgs e)
        {
            krijoProfil();
        }

        private void btnKyqu_Click(object sender, EventArgs e)
        {
            login();
        }

        private void krijoProfil()
        {
            if (txtEmri.Text != null && txtMbiemri.Text != null)
            {
                if (txtRuajtjaPerdoruesi.TextLength >= 6)
                {
                    if (txtRuajtjaFjalekalimi.TextLength >= 8 && txtRuajtjaFjalekalimi.TextLength <= 20)
                    {
                        
                            try
                            {
                                string connectionString = "Server=127.0.0.1; Database=ds_projekti1; UserID=root; Password=805198;";

                                MySqlConnection myDbConn = new MySqlConnection(connectionString);

                                myDbConn.Open();

                            /* Ketu ruhet fjalekalimi i enkriptuar 
                             * mirepo meqenese ende nuk e kemi implementuar algoritmin vetem per test ja kam vene vleren 'test123'
                            */
                            string salti = Salti();
                                string fjalekalimiEnkriptuar = Enkripto(txtRuajtjaFjalekalimi.Text, salti);

                                string inserti = "insert into perdoruesit(username,emri,mbiemri,salt,fjalekalimi) values('" + txtRuajtjaPerdoruesi.Text + "','" + txtEmri.Text + "','" + txtMbiemri.Text + "','" + salti + "','" + fjalekalimiEnkriptuar + "')";

                                MySqlCommand command = new MySqlCommand(inserti, myDbConn);





                                command.ExecuteReader();
                                lblRuaj.Text = " ";
                                txtEmri.Text = "";
                                txtMbiemri.Text = "";
                                txtRuajtjaPerdoruesi.Text = "";
                                txtRuajtjaFjalekalimi.Text = "";

                                MessageBox.Show("Profili u ruajt me sukses");


                                myDbConn.Close();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }
                        
                    }
                    else
                    {
                        lblRuaj.Text = "Fjalekalimi nuk duhet te jete me i shkurter se 8 karaktere apo e i gjate se 20!";
                    }
                }
                else
                {
                    lblRuaj.Text = "Perdoruesi duhet te kete 6 karaktere apo me shume!";
                }
            }
            else
            {
                lblRuaj.Text = "Ju lutem plotesoni te dhenat!";
            }
        }


        private void login()
        {
            if (txtTestPerdoruesi.Text != null && txtTestFjalekalimi.Text != null)
            {
                try
                {

                    string connectionString = "Server=127.0.0.1; Database=ds_projekti1; UserID=root; Password=805198;";

                    MySqlConnection myDbConn = new MySqlConnection(connectionString);

                    myDbConn.Open();

                    /* Ketu ruhet fjalekalimi i enkriptuar 
                     * mirepo meqenese ende nuk e kemi implementuar algoritmin vetem per test ja kam vene vleren 'test123'
                    */
                    string fjalekalimiEnkriptuar = "test123";

                    string users = "Select * from perdoruesit where username='" + txtTestPerdoruesi.Text + "' and fjalekalimi='" + fjalekalimiEnkriptuar + "'";

                    MySqlCommand command = new MySqlCommand(users, myDbConn);

                    command.ExecuteNonQuery();

                    DataTable rezultatet = new DataTable();

                    MySqlDataAdapter DA = new MySqlDataAdapter(command);

                    DA.Fill(rezultatet);

                    int i = 0;
                    i = Convert.ToInt32(rezultatet.Rows.Count.ToString());

                    if (i == 0)
                    {
                        lblKyqja.Text = "Ju keni dhene Perdoruesi-n apo Fjalekalimi-n jovalid!";
                    }
                    else
                    {
                        lblKyqja.Text = "Ju jeni kyqur me sukses!";
                    }
                    myDbConn.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                lblKyqja.Text = "Ju lutem shkruani Perdoruesi-n dhe Fjalekalimi-n!";
            }

        }

        public static string Enkripto(string fjalekalimi, string salti)
        {
            StringBuilder strFjalekalimi = new StringBuilder(fjalekalimi);
            StringBuilder strSalti = new StringBuilder(salti);
            StringBuilder enkriptimi = new StringBuilder();

            
            for (int i = 1; i <= strFjalekalimi.Length; i++)
            {
                enkriptimi.Append(strSalti[i%8]);
                enkriptimi.Append(strFjalekalimi[i%strFjalekalimi.Length]);
            }
            return enkriptimi.ToString();
        }


        }
}
