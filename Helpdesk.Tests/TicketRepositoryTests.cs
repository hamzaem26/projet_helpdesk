using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using Helpdesk.Core.Models;
using Helpdesk.Data.Repositories;

namespace Helpdesk.Tests
{
    [TestClass]
    public class TicketRepositoryTests
    {
        const string CS = "Host=localhost;Port=5432;Database=helpdesk_db;Username=postgres;Password=commey123";

        private TicketRepository repo = null!;
        private int clientIdTest;
        private int categorieIdTest;
        private int niveauIdTest;

        [TestInitialize]
        public void Init()
        {
            try
            {
                using var c = new NpgsqlConnection(CS);
                c.Open();

                using var cmd = new NpgsqlCommand(
                    "SELECT (SELECT ClientId FROM Clients ORDER BY ClientId LIMIT 1), " +
                    "       (SELECT CategorieId FROM CategoriesIncidents ORDER BY CategorieId LIMIT 1), " +
                    "       (SELECT NiveauId FROM NiveauxSupport ORDER BY NiveauId LIMIT 1);", c);

                using var r = cmd.ExecuteReader();
                if (!r.Read() || r.IsDBNull(0))
                {
                    Assert.Inconclusive("Base vide.");
                }

                clientIdTest = r.GetInt32(0);
                categorieIdTest = r.GetInt32(1);
                niveauIdTest = r.GetInt32(2);
            }
            catch (Exception)
            {
                Assert.Inconclusive("Postgres pas accessible.");
            }

            repo = new TicketRepository(CS);
        }

        private Ticket NouveauTicketDeTest()
        {
            return new Ticket
            {
                ClientId = clientIdTest,
                CategorieId = categorieIdTest,
                NiveauId = niveauIdTest,
                Titre = "Test",
                Description = "Desc test",
                Statut = "Ouvert"
            };
        }

        [TestMethod]
        public void Create_Test()
        {
            var t = NouveauTicketDeTest();
            int id = repo.Create(t);
            Assert.IsTrue(id > 0);
        }

        [TestMethod]
        public void GetById_Test()
        {
            var t = NouveauTicketDeTest();
            int id = repo.Create(t);

            Ticket? lu = repo.GetById(id);

            Assert.IsNotNull(lu);
            Assert.AreEqual(id, lu!.TicketId);
        }
    }
}
