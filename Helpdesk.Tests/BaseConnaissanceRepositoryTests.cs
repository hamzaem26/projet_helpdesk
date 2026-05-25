using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using Helpdesk.Core.Models;
using Helpdesk.Data.Repositories;

namespace Helpdesk.Tests
{
    [TestClass]
    public class BaseConnaissanceRepositoryTests
    {
        const string CS = "Host=localhost;Port=5432;Database=helpdesk_db;Username=postgres;Password=commey123";

        BaseConnaissanceRepository repo = null!;
        int catId;

        [TestInitialize]
        public void Init()
        {
            try
            {
                using var c = new NpgsqlConnection(CS);
                c.Open();
                using var cmd = new NpgsqlCommand(
                    "SELECT CategorieId FROM CategoriesIncidents ORDER BY CategorieId LIMIT 1", c);
                object? res = cmd.ExecuteScalar();
                if (res == null)
                    Assert.Inconclusive("lance le program avant");
                catId = Convert.ToInt32(res);
            }
            catch (Exception ex)
            {
                Assert.Inconclusive("postgres : " + ex.Message);
            }
            repo = new BaseConnaissanceRepository(CS);
        }

        [TestMethod]
        public void Add_ok()
        {
            repo.Add(new BaseConnaissance
            {
                CategorieId = catId,
                Sujet = "test " + DateTime.Now.Ticks,
                Solution = "solution test"
            });
        }

        [TestMethod]
        public void GetByCategorie_trouve()
        {
            string sujet = "kb-" + Guid.NewGuid();
            repo.Add(new BaseConnaissance
            {
                CategorieId = catId,
                Sujet = sujet,
                Solution = "ok"
            });

            var res = repo.GetByCategorie(catId);
            bool ok = false;
            foreach (var r in res)
                if (r.Sujet == sujet) ok = true;
            Assert.IsTrue(ok);
        }

        [TestMethod]
        public void Search_trouve()
        {
            string mot = "xyz" + DateTime.Now.Ticks;
            repo.Add(new BaseConnaissance
            {
                CategorieId = catId,
                Sujet = mot,
                Solution = "x"
            });
            Assert.IsTrue(repo.Search(mot).Count > 0);
        }
    }
}
