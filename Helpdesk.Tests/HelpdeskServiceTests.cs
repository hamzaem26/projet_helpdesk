using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Helpdesk.Core;
using Helpdesk.Core.Models;
using Helpdesk.Core.Interfaces;

namespace Helpdesk.Tests
{
    class TestTickets : ITicketRepository
    {
        public List<Ticket> liste = new List<Ticket>();
        int cpt = 1;

        public int Create(Ticket ticket)
        {
            ticket.TicketId = cpt++;
            liste.Add(ticket);
            return ticket.TicketId;
        }

        public Ticket? GetById(int id)
        {
            foreach (var t in liste)
                if (t.TicketId == id) return t;
            return null;
        }

        public void Update(Ticket ticket) { }

        public List<Ticket> GetAll() => liste;

        public int nbHist = 0;
        public void AddHistorique(int ticketId, string action) { nbHist++; }
    }

    class TestKb : IBaseConnaissanceRepository
    {
        public int nbAdd = 0;

        public void Add(BaseConnaissance article) { nbAdd++; }

        public List<BaseConnaissance> GetByCategorie(int categorieId)
        {
            return new List<BaseConnaissance>();
        }
    }

    [TestClass]
    public class HelpdeskServiceTests
    {
        [TestMethod]
        public void CreerTicket_ok()
        {
            TestTickets t = new TestTickets();
            TestKb kb = new TestKb();
            HelpdeskService s = new HelpdeskService(t, kb);

            int id = s.CreerTicket(1, 1, "Test", "Desc");

            Assert.IsTrue(id > 0);
            Assert.AreEqual(1, t.nbHist);
        }

        [TestMethod]
        public void Escalade_niveau2()
        {
            TestTickets t = new TestTickets();
            TestKb kb = new TestKb();
            HelpdeskService s = new HelpdeskService(t, kb);

            Ticket ticket = new Ticket();
            ticket.TicketId = 1;
            ticket.NiveauId = 1;
            ticket.Statut = "Ouvert";
            ticket.CategorieId = 1;
            t.liste.Add(ticket);

            s.TraiterTicket(1);

            Assert.AreEqual(2, ticket.NiveauId);
        }

        [TestMethod]
        public void N3_resout()
        {
            TestTickets t = new TestTickets();
            TestKb kb = new TestKb();
            HelpdeskService s = new HelpdeskService(t, kb);

            Ticket ticket = new Ticket();
            ticket.TicketId = 5;
            ticket.NiveauId = 3;
            ticket.Statut = "En cours";
            ticket.CategorieId = 2;
            t.liste.Add(ticket);

            s.TraiterTicket(5);

            Assert.AreEqual("Resolu", ticket.Statut);
        }

        [TestMethod]
        public void Serveur_bloque()
        {
            TestTickets t = new TestTickets();
            TestKb kb = new TestKb();
            HelpdeskService s = new HelpdeskService(t, kb);

            Ticket ticket = new Ticket();
            ticket.TicketId = 3;
            ticket.NiveauId = 3;
            ticket.Statut = "En cours";
            ticket.CategorieId = 6;
            t.liste.Add(ticket);

            s.TraiterTicket(3);

            Assert.AreEqual("Non resolu", ticket.Statut);
        }
    }
}
