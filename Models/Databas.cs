using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Http;

namespace ProjektBostad.Models
{
    public class Databas
    {

        public List<BostadsDetalj> VisaBostad(out string errormsg)
        {
            SqlConnection dbConnection = new SqlConnection();
            dbConnection.ConnectionString = "Data Source = (localdb)\\mssqllocaldb; Initial Catalog = Bostad; Integrated Security = True";

            String sqlstring = "SELECT * FROM Bostader";
            SqlCommand dbCommand = new SqlCommand(sqlstring, dbConnection);

            SqlDataAdapter adapter = new SqlDataAdapter(dbCommand);
            DataSet myDS = new DataSet();

            List<BostadsDetalj> Bostadslista = new List<BostadsDetalj>();

            errormsg = "";

            try
            {
                dbConnection.Open();

                adapter.Fill(myDS, "myBostad");

                int count = 0;
                int i = 0;
                count = myDS.Tables["myBostad"].Rows.Count;
                if (count > 0)
                {
                    while (i < count)
                    {
                        BostadsDetalj b = new BostadsDetalj();
                        b.Anlagg_Id = Convert.ToInt16(myDS.Tables["myBostad"].Rows[i]["Anlagg_Id"]);
                        b.Kostnad = Convert.ToInt32(myDS.Tables["myBostad"].Rows[i]["Kostnad"]);
                        b.Kvm = Convert.ToInt32(myDS.Tables["myBostad"].Rows[i]["Kvm"]);
                        b.Typ_Id = Convert.ToInt32(myDS.Tables["myBostad"].Rows[i]["Typ_Id"]);
                        b.Maklar_Id = Convert.ToInt16(myDS.Tables["myBostad"].Rows[i]["Maklar_Id"]);
                        b.Adress = myDS.Tables["myBostad"].Rows[i]["Adress"].ToString();
                        b.Bostadsnamn = myDS.Tables["myBostad"].Rows[i]["Bostadsnamn"].ToString();
                        b.Postkod = myDS.Tables["myBostad"].Rows[i]["Postkod"].ToString();
                        b.Stad = myDS.Tables["myBostad"].Rows[i]["Stad"].ToString();
                        b.Bild = myDS.Tables["myBostad"].Rows[i]["Bild"].ToString();
                        


                        i++;
                        Bostadslista.Add(b);

                    }
                    errormsg = "";
                    return Bostadslista;
                }
                else
                {
                    errormsg = "Det hämtas ingen bostad";
                    return null;
                }

            }
            catch (Exception e)
            {
                errormsg = e.Message;
                return null;
            }

        }
        public int LoggaIn(Anvandare anv, out string errormsg)
        {
            SqlConnection dbConnection = new SqlConnection();
            dbConnection.ConnectionString = "Data Source = (localdb)\\mssqllocaldb; Initial Catalog = Bostad; Integrated Security = True";

            String sqlstring = "SELECT Losenord, Salt FROM Anvandare WHERE Anv_Namn = @Anv_Namn;";
            SqlCommand dbCommand = new SqlCommand(sqlstring, dbConnection);

            dbCommand.Parameters.Add("@Anv_Namn", SqlDbType.VarChar, 50).Value = anv.Anv_Namn;

            try
            {
                dbConnection.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(dbCommand);
                DataSet ds = new DataSet();
                adapter.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    string hashedLosenordFromDatabase = ds.Tables[0].Rows[0]["Losenord"].ToString();
                    string salt = ds.Tables[0].Rows[0]["Salt"].ToString();

                    bool isValidPassword = BCrypt.Net.BCrypt.Verify(anv.Losenord + salt, hashedLosenordFromDatabase);

                    if (isValidPassword)
                    {
                        errormsg = "";
                        return 1;
                    }
                }
                errormsg = "Fel användarnamn eller lösenord";
                return 0;
            }
            catch (Exception e)
            {
                errormsg = e.Message;
                return 0;
            }
            finally
            {
                dbConnection.Close();
            }
        }
        public int SkapaAnv(Anvandare anv, out string errormsg)
        {
            SqlConnection dbConnection = new SqlConnection();
            dbConnection.ConnectionString = "Data Source = (localdb)\\mssqllocaldb; Initial Catalog = Bostad; Integrated Security = True";

            String sqlstring = "INSERT INTO Anvandare ( Email,Anv_Namn, Losenord, Salt) VALUES ( @Email,@Anv_Namn,@Losenord, @Salt)";
            SqlCommand dbCommand = new SqlCommand(sqlstring, dbConnection);

            dbCommand.Parameters.Add("@Email", SqlDbType.VarChar, 50).Value = anv.Email;
            dbCommand.Parameters.Add("@Anv_Namn", SqlDbType.VarChar, 50).Value = anv.Anv_Namn;


            string salt = BCrypt.Net.BCrypt.GenerateSalt(12);
            string hashedLosenord = BCrypt.Net.BCrypt.HashPassword(anv.Losenord + salt);

            dbCommand.Parameters.Add("@Losenord", SqlDbType.VarChar, 100).Value = hashedLosenord;
            dbCommand.Parameters.Add("@Salt", SqlDbType.VarChar, 100).Value = salt;

            try
            {
                dbConnection.Open();
                int i = 0;
                i = dbCommand.ExecuteNonQuery();

                if (i == 1) { errormsg = ""; }
                else { errormsg = "Det skapas inte en ny intagen i databasen"; }
                return i;
            }
            catch (Exception e)
            {
                errormsg = e.Message;
                return 0;
            }
            finally
            {
                dbConnection.Close();
            }
        }
        public int TaBortAnv(Anvandare anv, out string errormsg)
        {

            SqlConnection dbConnection = new SqlConnection();

            dbConnection.ConnectionString = "Data Source = (localdb)\\mssqllocaldb; Initial Catalog = Bostad; Integrated Security = True";

            String sqlstring = "DELETE FROM Anvandare WHERE Anv_Namn = @Anv_Namn AND Losenord = @Losenord";

            SqlCommand dbCommand = new SqlCommand(sqlstring, dbConnection);


            dbCommand.Parameters.Add("@Anv_Namn", SqlDbType.VarChar, 50).Value = anv.Anv_Namn;
            dbCommand.Parameters.Add("@Losenord", SqlDbType.VarChar, 50).Value = anv.Losenord;

            try
            {
                dbConnection.Open();
                int i = 0;
                i = dbCommand.ExecuteNonQuery();
                if (i == 1) { errormsg = " "; }
                else { errormsg = "Det togs inte bort någon från "; }
                return i;
            }
            catch (Exception e)
            {
                errormsg = e.Message;
                return 0;
            }
            finally
            {
                dbConnection.Close();
            }
        }
        public int UppdateraAnv(Anvandare anv, out string errormsg)
        {
            SqlConnection dbConnection = new SqlConnection();
            dbConnection.ConnectionString = "Data Source = (localdb)\\mssqllocaldb; Initial Catalog = Bostad; Integrated Security = True";

            String sqlstring = "UPDATE Anvandare SET Losenord = @Losenord, Salt = @Salt WHERE Anv_Namn = @Anv_Namn;";

            SqlCommand dbCommand = new SqlCommand(sqlstring, dbConnection);


            string salt = BCrypt.Net.BCrypt.GenerateSalt(12);
            string hashedLosenord = BCrypt.Net.BCrypt.HashPassword(anv.Losenord, salt);

            dbCommand.Parameters.Add("@Anv_Namn", SqlDbType.VarChar, 50).Value = anv.Anv_Namn;
            dbCommand.Parameters.Add("@Losenord", SqlDbType.VarChar, 50).Value = hashedLosenord;
            dbCommand.Parameters.Add("@Salt", SqlDbType.VarChar, 50).Value = salt;

            try
            {
                dbConnection.Open();
                int i = dbCommand.ExecuteNonQuery();
                if (i == 1)
                {
                    errormsg = "";
                }
                else
                {
                    errormsg = "Det togs inte bort någon från ";
                }
                return i;
            }
            catch (Exception e)
            {
                errormsg = e.Message;
                return 0;
            }
            finally
            {
                dbConnection.Close();
            }
        }
        public BostadsDetalj GetDetaljer(int Anlagg_Id, out string errormsg)
        {
            SqlConnection dbConnection = new SqlConnection();
            dbConnection.ConnectionString = "Data Source = (localdb)\\mssqllocaldb; Initial Catalog = Bostad; Integrated Security = True";


            String sqlstring = "SELECT Anlagg_Id, Adress, Bostadsnamn, Postkod, Stad, Kostnad, Kvm, Bild FROM Bostader WHERE Anlagg_Id = @Anlagg_Id";

            Console.WriteLine($"SQL-fråga: {sqlstring}");

            SqlCommand dbCommand = new SqlCommand(sqlstring, dbConnection);
            dbCommand.Parameters.Add("Anlagg_Id", SqlDbType.Int).Value = Anlagg_Id;

            SqlDataAdapter adapter = new SqlDataAdapter(dbCommand);
            DataSet myDS = new DataSet();


            List<BostadsDetalj> Bostadslista = new List<BostadsDetalj>();

            errormsg = "";

            try
            {
                dbConnection.Open();

                adapter.Fill(myDS, "myBostad");

                int count = myDS.Tables["myBostad"].Rows.Count;

                Console.WriteLine($"Antal rader i DataSet: {count}");

                if (count > 0)
                {
                    BostadsDetalj b = new BostadsDetalj();

                    b.Anlagg_Id = Convert.ToInt16(myDS.Tables["myBostad"].Rows[0]["Anlagg_Id"]);
                    b.Kostnad = Convert.ToInt32(myDS.Tables["myBostad"].Rows[0]["Kostnad"]);
                    b.Kvm = Convert.ToInt32(myDS.Tables["myBostad"].Rows[0]["Kvm"]);
                    b.Adress = myDS.Tables["myBostad"].Rows[0]["Adress"].ToString();
                    b.Bostadsnamn = myDS.Tables["myBostad"].Rows[0]["Bostadsnamn"].ToString();
                    b.Postkod = myDS.Tables["myBostad"].Rows[0]["Postkod"].ToString();
                    b.Stad = myDS.Tables["myBostad"].Rows[0]["Stad"].ToString();
                    b.Bild = myDS.Tables["myBostad"].Rows[0]["Bild"].ToString();
                    
                    return b;
                }
                else
                {
                    errormsg = "Det hämtas ingen bostad";
                    throw new Exception(errormsg);
                }
            }
            catch (Exception e)
            {
                errormsg = e.Message;
                return null;
            }
            finally
            {
                dbConnection.Close();
            }

        }
        public int BankUpg(BankModell model, out string errormsg)
        {
            SqlConnection dbConnection = new SqlConnection();
            dbConnection.ConnectionString = "Data Source = (localdb)\\mssqllocaldb; Initial Catalog = Bostad; Integrated Security = True";

            String sqlstring = "INSERT INTO Bankuppgifter (Email, Kontonummer, Fakturering) VALUES (@Email, @Kontonummer, @Fakturering);";

            SqlCommand dbCommand = new SqlCommand(sqlstring, dbConnection);

            dbCommand.Parameters.Add("@Email", SqlDbType.VarChar, 50).Value = model.Email;
            dbCommand.Parameters.Add("@Kontonummer", SqlDbType.Int).Value = model.Kontonummer;
            dbCommand.Parameters.Add("@Fakturering", SqlDbType.VarChar, 50).Value = model.Fakturering;

            try
            {
                dbConnection.Open();
                int i = dbCommand.ExecuteNonQuery();
                if (i == 1)
                {
                    errormsg = "";
                }
                else
                {
                    errormsg = "Det lades inte till bankuppgifter ";
                }
                return i;
            }
            catch (Exception e)
            {
                errormsg = e.Message;
                return 0;
            }
            finally
            {
                dbConnection.Close();
            }

        }
        public (string hashedLosenord, string salt) GetHashedLosenordAndSaltFromDatabase(string anvNamn)
        {
            SqlConnection dbConnection = new SqlConnection();
            dbConnection.ConnectionString = "Data Source = (localdb)\\mssqllocaldb; Initial Catalog = Bostad; Integrated Security = True";

            String sqlstring = "SELECT Losenord, Salt FROM Anvandare WHERE Anv_Namn = @Anv_Namn;";
            SqlDataAdapter dbAdapter = new SqlDataAdapter(sqlstring, dbConnection);
            dbAdapter.SelectCommand.Parameters.Add("@Anv_Namn", SqlDbType.VarChar, 50).Value = anvNamn;

            DataSet ds = new DataSet();

            try
            {
                dbConnection.Open();
                dbAdapter.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    string hashedLosenord = ds.Tables[0].Rows[0]["Losenord"].ToString();
                    string salt = ds.Tables[0].Rows[0]["Salt"].ToString();

                    return (hashedLosenord, salt);
                }
                else
                {
                    return (null, null);
                }
            }
            catch (Exception)
            {
                return (null, null);
            }
            finally
            {
                dbConnection.Close();
            }
        }
        public bool VerifyLosenord(string inputLosenord, string storedHashedLosenord, string salt)
        {
            string combinedLosenord = inputLosenord + salt;
            bool isValidPassword = BCrypt.Net.BCrypt.Verify(combinedLosenord, storedHashedLosenord);

            return isValidPassword;
        }
       
    }
}



