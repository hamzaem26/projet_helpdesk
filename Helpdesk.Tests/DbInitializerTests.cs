using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using Helpdesk.Data;

namespace Helpdesk.Tests
{
    [TestClass]
    public class DbInitializerTests    {
        const string CS = "Host=localhost;Port=5432;Database=helpdesk_db;Username=postgres;Password=commey123";

        [TestInitialize]
        public void Init()
        {
            try
            {
                using var c = new NpgsqlConnection(CS);
                c.Open();
            }
            catch (Exception ex)
            {
                Assert.Inconclusive("Pas de postgres dispo : " + ex.Message);
            }
        }

        private bool TableExiste(string nomTable)        {
            using var c = new NpgsqlConnection(CS);
            c.Open();
            using var cmd = new NpgsqlCommand(
                "SELECT COUNT(*) FROM information_schema.tables WHERE LOWER(table_name) = LOWER(@n)", c);            cmd.Parameters.AddWithValue("n", nomTable);
            object? res = cmd.ExecuteScalar();
            return Convert.ToInt64(res) > 0;
        }

        [TestMethod]
        public void Initialize_Cree_Bien_Les_Tables_Attendues()
        {
            var init = new DbInitializer(CS);
            init.Initialize();

            Assert.IsTrue(TableExiste("Clients"), "Table Clients absente");            Assert.IsTrue(TableExiste("Tickets"),             "Table Tickets absente");
            Assert.IsTrue(TableExiste("CategoriesIncidents"), "Table CategoriesIncidents absente");
            Assert.IsTrue(TableExiste("NiveauxSupport"),      "Table NiveauxSupport absente");
            Assert.IsTrue(TableExiste("BaseConnaissances"),   "Table BaseConnaissances absente");
            Assert.IsTrue(TableExiste("HistoriqueTickets"),   "Table HistoriqueTickets absente");
        }

        [TestMethod]
        public void Initialize_Insere_Les_3_Niveaux_De_Support()
        {
            var init = new DbInitializer(CS);
            init.Initialize();

            using var c = new NpgsqlConnection(CS);
            c.Open();
            using var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM NiveauxSupport", c);
            long nb = Convert.ToInt64(cmd.ExecuteScalar());

            Assert.IsTrue(nb >= 3);        }

        [TestMethod]
        public void Initialize_Peut_Etre_Appele_Plusieurs_Fois_Sans_Erreur()
        {
            // Le script utilise WHERE NOT EXISTS pour ne pas dupliquer les données
            // donc on doit pouvoir le rejouer en boucle sans souci
            var init = new DbInitializer(CS);

            init.Initialize();
            init.Initialize();
            init.Initialize();
            Assert.IsTrue(true);        }
    }
}
