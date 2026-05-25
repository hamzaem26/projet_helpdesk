using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using Helpdesk.Data.Repositories;

namespace Helpdesk.Tests
{
    [TestClass]
    public class MasterDataRepositoryTests
    {
        const string CS = "Host=localhost;Port=5432;Database=helpdesk_db;Username=postgres;Password=commey123";

        private DonneesRepository repo = null!;

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
                Assert.Inconclusive("postgres pas dispo : " + ex.Message);
            }

            repo = new DonneesRepository(CS);
        }

        [TestMethod]
        public void GetClients_Doit_Renvoyer_Une_Liste_Non_Vide()
        {
            var liste = repo.GetClients();

            Assert.IsNotNull(liste);
            Assert.IsTrue(liste.Count > 0);        }

        [TestMethod]
        public void GetClients_Chaque_Element_A_Un_Id_Et_Un_Nom()
        {
            var liste = repo.GetClients();

            foreach (var c in liste)
            {
                Assert.IsTrue(c.ClientId > 0);
                Assert.IsFalse(string.IsNullOrWhiteSpace(c.Nom));            }
        }

        [TestMethod]
        public void GetCategories_Doit_Renvoyer_Au_Moins_Une_Categorie()
        {
            var liste = repo.GetCategories();

            Assert.IsNotNull(liste);
            Assert.IsTrue(liste.Count > 0);
        }

        [TestMethod]
        public void GetCategories_Contient_Mot_De_Passe()
        {
            var liste = repo.GetCategories();
            bool trouvee = false;
            foreach (var cat in liste)
            {
                if (cat.NomCategorie == "Mot de passe oublié")
                {
                    trouvee = true;
                    break;
                }
            }

            Assert.IsTrue(trouvee);        }
    }
}
